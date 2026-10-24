"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import type { BranchRateOverrideResponse } from "@/lib/api/api-client";
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useBranchOverrideMutations, useBranchOverrides } from "../api/branch-override-queries";
import { isOverrideStatus, overrideStatusVariant, quickEstimateErrorCode } from "../quick-estimate-status";

const RATE = /^\d{1,8}(\.\d{1,4})?$/;
const formSchema = z.object({
  referenceRate: z.string().regex(RATE), minimumCharge: z.string().regex(RATE),
  effectiveFrom: z.string().min(1), effectiveTo: z.string().min(1),
  reasonCode: z.string().trim().min(1).max(64), reasonDetail: z.string().trim().min(1).max(500), sourceReference: z.string().trim().max(200),
}).refine((values) => values.effectiveTo >= values.effectiveFrom, { path: ["effectiveTo"] });
type FormValues = z.infer<typeof formSchema>;
const FIELDS = ["referenceRate", "minimumCharge", "effectiveFrom", "effectiveTo", "reasonCode", "reasonDetail", "sourceReference"] as const;
const EMPTY: FormValues = { referenceRate: "", minimumCharge: "0", effectiveFrom: "", effectiveTo: "", reasonCode: "", reasonDetail: "", sourceReference: "" };

type DialogMode = "approved" | "returned" | "revoke";
const DIALOG_TITLES = { approved: "approveTitle", returned: "returnTitle", revoke: "revokeTitle" } as const;

export function PricingTemplateOverrides({ templateId }: { templateId: string }) {
  const t = useTranslations("quickEstimates.overrides");
  const tErrors = useTranslations("quickEstimates.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canRequest = can(selectedMembership, PERMISSIONS.PRICING_TEMPLATES_OVERRIDE_RATE);
  const canDecide = can(selectedMembership, PERMISSIONS.PRICING_TEMPLATES_APPROVE);
  const query = useBranchOverrides(templateId);
  const mutations = useBranchOverrideMutations(templateId);
  const busy = mutations.request.isPending || mutations.decide.isPending || mutations.revoke.isPending;
  const [formOpen, setFormOpen] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [target, setTarget] = useState<{ mode: DialogMode; item: BranchRateOverrideResponse } | null>(null);
  const [note, setNote] = useState("");
  const [noteError, setNoteError] = useState(false);
  const keyRef = useRef(crypto.randomUUID()); // a retry replays with the same key; a new request gets a new key after success
  const { control, handleSubmit, reset } = useForm<FormValues>({ resolver: zodResolver(formSchema), defaultValues: EMPTY });

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? quickEstimateErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function submitRequest(values: FormValues): Promise<void> {
    setMessage(null);
    try {
      await mutations.request.mutateAsync({
        idempotencyKey: keyRef.current,
        payload: { referenceRate: Number(values.referenceRate), minimumCharge: Number(values.minimumCharge), effectiveFrom: values.effectiveFrom, effectiveTo: values.effectiveTo,
          reasonCode: values.reasonCode, reasonDetail: values.reasonDetail, sourceReference: values.sourceReference || null },
      });
      keyRef.current = crypto.randomUUID();
      toast.success(t("done"));
      reset(EMPTY);
      setFormOpen(false);
    } catch (err: unknown) {
      setMessage(describe(err));
    }
  }

  async function confirm(): Promise<void> {
    if (!target) return;
    const { mode, item } = target;
    const overrideId = item.id ?? "";
    const rowVersion = item.rowVersion ?? "";
    const text = note.trim();
    if (mode !== "approved" && !text) { setNoteError(true); return; }
    setMessage(null);
    try {
      if (mode === "revoke") await mutations.revoke.mutateAsync({ overrideId, rowVersion, reason: text });
      else await mutations.decide.mutateAsync({ overrideId, rowVersion, decision: mode, note: text || null });
      toast.success(t("done"));
    } catch (err: unknown) {
      setMessage(describe(err));
    }
    setTarget(null);
  }

  function open(mode: DialogMode, item: BranchRateOverrideResponse): void {
    setTarget({ mode, item });
    setNote("");
    setNoteError(false);
  }

  if (query.isLoading) return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  if (query.isError || !query.data) return <Alert variant="danger">{t("loadError")}</Alert>;
  const items = query.data.items ?? [];

  return (
    <section className="space-y-5">
      <PageHeader
        title={t("title")}
        subtitle={t("subtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/pricing-templates/${templateId}` }]}
        actions={canRequest ? <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => setFormOpen(true)}>{t("request")}</Button> : undefined}
      />
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}
      {items.length === 0 ? <p className="text-sm text-erp-text-muted">{t("empty")}</p> : (
        <ul className="space-y-3">
          {items.map((item) => (
            <li key={item.id} className="erp-card space-y-2 p-4">
              <div className="flex flex-wrap items-center gap-3">
                <span className="font-mono text-sm font-bold text-erp-navy">{item.branchCode ?? "-"}</span>
                <StatusBadge label={isOverrideStatus(item.status) ? t(`statuses.${item.status}`) : "-"} variant={overrideStatusVariant(item.status)} />
                <span className="text-xs text-erp-text-muted">{t("period", { from: item.effectiveFrom ? formatDate(item.effectiveFrom, locale) : "-", to: item.effectiveTo ? formatDate(item.effectiveTo, locale) : "-" })}</span>
              </div>
              <dl className="grid grid-cols-2 gap-3 text-sm md:grid-cols-4">
                <div><dt className="text-xs text-erp-text-muted">{t("referenceRate")}</dt><dd className="font-mono font-semibold">{formatCurrency(item.referenceRate, "THB", locale)}</dd></div>
                <div><dt className="text-xs text-erp-text-muted">{t("minimumCharge")}</dt><dd className="font-mono font-semibold">{formatCurrency(item.minimumCharge, "THB", locale)}</dd></div>
                <div><dt className="text-xs text-erp-text-muted">{t("reasonCode")}</dt><dd className="font-mono">{item.reasonCode ?? "-"}</dd></div>
                <div><dt className="text-xs text-erp-text-muted">{t("sourceReference")}</dt><dd>{item.sourceReference ?? "-"}</dd></div>
              </dl>
              <p className="text-xs text-erp-text-muted">{item.reasonDetail ?? "-"} · {t("requestedBy", { name: item.requestedBy?.displayName ?? "-" })}</p>
              {item.decisionNote && <p className="text-xs text-erp-text-muted">{item.decisionNote}</p>}
              <div className="flex flex-wrap gap-3">
                {canDecide && item.status === "requested" && <Button type="button" variant="primary" className="min-h-11" disabled={busy} onClick={() => open("approved", item)}>{t("approve")}</Button>}
                {canDecide && item.status === "requested" && <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => open("returned", item)}>{t("return")}</Button>}
                {canDecide && item.status === "approved" && <Button type="button" variant="danger" className="min-h-11" disabled={busy} onClick={() => open("revoke", item)}>{t("revoke")}</Button>}
              </div>
            </li>
          ))}
        </ul>
      )}

      <Modal isOpen={formOpen} onClose={() => { if (!busy) setFormOpen(false); }} title={t("requestTitle")} size="md" closeDisabled={busy} closeOnOverlayClick={!busy} closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setFormOpen(false)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={mutations.request.isPending} disabled={busy} onClick={() => void handleSubmit(submitRequest)()}>{tCommon("actions.confirm")}</Button>
          </div>
        )}
      >
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
          {FIELDS.map((name) => (
            <Controller key={name} name={name} control={control} render={({ field, fieldState }) => (
              <Input
                {...field}
                label={t(name)}
                type={name === "effectiveFrom" || name === "effectiveTo" ? "date" : "text"}
                placeholder={t(`${name}Placeholder`)}
                required={name !== "sourceReference"}
                disabled={busy}
                error={fieldState.error ? t("fieldInvalid") : undefined}
              />
            )} />
          ))}
        </div>
      </Modal>

      <Modal isOpen={target !== null} onClose={() => { if (!busy) setTarget(null); }} title={target ? t(DIALOG_TITLES[target.mode]) : undefined} size="sm" closeDisabled={busy} closeOnOverlayClick={!busy} closeOnEscape={!busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={busy} onClick={() => setTarget(null)}>{tCommon("actions.cancel")}</Button>
            <Button type="button" variant="primary" className="min-h-11" isLoading={busy} disabled={busy} onClick={() => void confirm()}>{tCommon("actions.confirm")}</Button>
          </div>
        )}
      >
        {target && <Input label={t("note")} value={note} maxLength={500} placeholder={t("notePlaceholder")} disabled={busy} required={target.mode !== "approved"} error={noteError ? t("reasonRequired") : undefined} onChange={(event) => setNote(event.target.value)} />}
      </Modal>
    </section>
  );
}
