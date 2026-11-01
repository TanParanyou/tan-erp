"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Drawer } from "@/components/ui/Drawer";
import { EmptyState } from "@/components/ui/EmptyState";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/Table";
import { PageHeader } from "@/components/layout/PageHeader";
import { IconPlus } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import type { CalculationPolicyVersionResponse, CreateCalculationPolicyRequest } from "@/lib/api/api-client";
import { useCalculationPolicies, useCalculationPolicyMutations } from "../api/calculation-policy-queries";

const OVERHEAD_METHODS = ["none", "percent-direct-cost", "fixed-amount"] as const;
const UNIT_CONVERSION_MODES = ["none", "snapshot"] as const;
const KNOWN_ERROR_CODES = ["CALCULATION_POLICY_CONFLICT", "INVALID_CALCULATION_POLICY", "INVALID_POLICY_STATE", "MAKER_CHECKER_VIOLATION", "FORBIDDEN", "RESOURCE_NOT_FOUND"] as const;

interface FormState {
  policyCode: string;
  version: string;
  overheadMethod: string;
  overheadValue: string;
  unitConversionMode: string;
  effectiveFrom: string;
  effectiveTo: string;
}

const EMPTY_FORM: FormState = { policyCode: "", version: "1", overheadMethod: "none", overheadValue: "0", unitConversionMode: "none", effectiveFrom: "", effectiveTo: "" };

function isOneOf<T extends string>(values: readonly T[], value: string | null | undefined): value is T {
  return values.some((v) => v === value);
}

export function CalculationPolicyList() {
  const t = useTranslations("estimates.calculationPolicies");
  const tErrors = useTranslations("estimates.calculationPolicies.errors");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canCreate = can(selectedMembership, PERMISSIONS.ESTIMATES_UPDATE);
  const canPublish = can(selectedMembership, PERMISSIONS.ESTIMATES_APPROVE);
  const policies = useCalculationPolicies();
  const { create, publish } = useCalculationPolicyMutations();

  const [creating, setCreating] = useState(false);
  const [form, setForm] = useState<FormState>(EMPTY_FORM);
  const [error, setError] = useState<string | null>(null);
  const [publishing, setPublishing] = useState<CalculationPolicyVersionResponse | null>(null);

  const rows = policies.data?.items ?? [];
  const isBusy = create.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError && isOneOf(KNOWN_ERROR_CODES, err.code) ? err.code : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  function openNew(): void {
    setForm(EMPTY_FORM);
    setError(null);
    setCreating(true);
  }

  async function save(): Promise<void> {
    const version = Number(form.version);
    const overheadValue = Number(form.overheadValue);
    const from = form.effectiveFrom ? new Date(form.effectiveFrom) : null;
    const to = form.effectiveTo ? new Date(form.effectiveTo) : null;
    if (!form.policyCode.trim() || !Number.isInteger(version) || version < 1 || !(overheadValue >= 0) || !from || (to && to <= from)) {
      setError(t("invalid"));
      return;
    }

    const payload: CreateCalculationPolicyRequest = {
      policyCode: form.policyCode.trim(),
      version,
      overheadMethod: form.overheadMethod,
      overheadValue,
      roundingMode: "away-from-zero",
      unitConversionMode: form.unitConversionMode,
      effectiveFromUtc: from.toISOString(),
      effectiveToUtc: to ? to.toISOString() : null,
    };

    try {
      await create.mutateAsync(payload);
      toast.success(t("created"));
      setCreating(false);
    } catch (err: unknown) {
      setError(describe(err));
    }
  }

  async function confirmPublish(): Promise<void> {
    if (!publishing?.id) return;
    try {
      await publish.mutateAsync(publishing.id);
      toast.success(t("published"));
    } catch (err: unknown) {
      toast.error(describe(err));
    } finally {
      setPublishing(null);
    }
  }

  const methodLabel = (value: string | null | undefined) => (isOneOf(OVERHEAD_METHODS, value) ? t(`overheadMethods.${value}`) : "-");
  const modeLabel = (value: string | null | undefined) => (isOneOf(UNIT_CONVERSION_MODES, value) ? t(`unitConversionModes.${value}`) : "-");

  return (
    <section className="space-y-5" aria-busy={policies.isLoading}>
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title") }]}
        actions={canCreate ? <Button size="md" icon={<IconPlus size={16} />} onClick={openNew}>{t("create")}</Button> : undefined}
      />
      {policies.isError && <Alert variant="danger">{describe(policies.error)}</Alert>}
      {!policies.isLoading && rows.length === 0 ? (
        <EmptyState icon="empty" title={t("empty")} description={t("emptyDetail")} actionLabel={canCreate ? t("create") : undefined} onAction={canCreate ? openNew : undefined} />
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>{t("policyCode")}</TableHead>
              <TableHead>{t("version")}</TableHead>
              <TableHead>{t("status")}</TableHead>
              <TableHead>{t("overheadMethod")}</TableHead>
              <TableHead>{t("overheadValue")}</TableHead>
              <TableHead>{t("unitConversionMode")}</TableHead>
              <TableHead>{t("effectiveFrom")}</TableHead>
              <TableHead>{t("effectiveTo")}</TableHead>
              <TableHead><span className="sr-only">{t("actions")}</span></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {rows.map((row) => (
              <TableRow key={row.id}>
                <TableCell className="align-middle font-mono">{row.policyCode ?? "-"}</TableCell>
                <TableCell className="align-middle">{row.version ?? "-"}</TableCell>
                <TableCell className="align-middle">
                  <StatusBadge label={row.status === "published" ? t("statuses.published") : row.status === "draft" ? t("statuses.draft") : "-"} variant={row.status === "published" ? "success" : "neutral"} />
                </TableCell>
                <TableCell className="align-middle">{methodLabel(row.overheadMethod)}</TableCell>
                <TableCell className="align-middle">{row.overheadValue ?? "-"}</TableCell>
                <TableCell className="align-middle">{modeLabel(row.unitConversionMode)}</TableCell>
                <TableCell className="align-middle">{row.effectiveFromUtc ? new Date(row.effectiveFromUtc).toLocaleDateString() : "-"}</TableCell>
                <TableCell className="align-middle">{row.effectiveToUtc ? new Date(row.effectiveToUtc).toLocaleDateString() : t("noEnd")}</TableCell>
                <TableCell className="align-middle">
                  {canPublish && row.status === "draft" && (
                    <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={publish.isPending} onClick={() => setPublishing(row)}>{t("publish")}</Button>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}

      <Drawer
        isOpen={creating}
        onClose={() => { if (!isBusy) setCreating(false); }}
        title={t("create")}
        size="lg"
        closeDisabled={isBusy}
        closeOnOverlayClick={!isBusy}
        footer={(
          <div className="flex flex-col-reverse sm:flex-row sm:justify-end gap-2 sm:gap-3">
            <Button type="button" variant="outline" className="w-full sm:w-auto min-h-11" disabled={isBusy} onClick={() => setCreating(false)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="w-full sm:w-auto min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{tCommon("actions.save")}</Button>
          </div>
        )}
      >
        <div className="space-y-3 py-2">
          {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
          <Alert variant="info">{t("makerCheckerNote")}</Alert>
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <Input label={t("policyCode")} required maxLength={64} placeholder={t("policyCodePlaceholder")} value={form.policyCode} disabled={isBusy}
              onChange={(event) => setForm((current) => ({ ...current, policyCode: event.target.value }))} />
            <Input type="number" min={1} step="1" label={t("version")} placeholder={t("versionPlaceholder")} value={form.version} disabled={isBusy}
              onChange={(event) => setForm((current) => ({ ...current, version: event.target.value }))} />
            <Select label={t("overheadMethod")} value={form.overheadMethod} disabled={isBusy}
              options={OVERHEAD_METHODS.map((value) => ({ value, label: t(`overheadMethods.${value}`) }))}
              onChange={(event) => setForm((current) => ({ ...current, overheadMethod: event.target.value }))} />
            <Input type="number" min={0} step="0.0001" label={t("overheadValue")} helperText={t("overheadValueHint")} placeholder={t("overheadValuePlaceholder")} value={form.overheadValue} disabled={isBusy}
              onChange={(event) => setForm((current) => ({ ...current, overheadValue: event.target.value }))} />
            <Select label={t("unitConversionMode")} value={form.unitConversionMode} disabled={isBusy}
              options={UNIT_CONVERSION_MODES.map((value) => ({ value, label: t(`unitConversionModes.${value}`) }))}
              onChange={(event) => setForm((current) => ({ ...current, unitConversionMode: event.target.value }))} />
            <div />
            <Input type="datetime-local" label={t("effectiveFrom")} required placeholder={t("datePlaceholder")} value={form.effectiveFrom} disabled={isBusy}
              onChange={(event) => setForm((current) => ({ ...current, effectiveFrom: event.target.value }))} />
            <Input type="datetime-local" label={t("effectiveTo")} placeholder={t("datePlaceholder")} value={form.effectiveTo} disabled={isBusy}
              onChange={(event) => setForm((current) => ({ ...current, effectiveTo: event.target.value }))} />
          </div>
        </div>
      </Drawer>

      <ConfirmationModal
        isOpen={publishing !== null}
        onClose={() => setPublishing(null)}
        onConfirm={() => confirmPublish()}
        title={t("publishTitle")}
        message={t("publishMessage", { code: publishing?.policyCode ?? "-", version: publishing?.version ?? 0 })}
        variant="warning"
        isLoading={publish.isPending}
      />
    </section>
  );
}
