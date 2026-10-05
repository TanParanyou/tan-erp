"use client";

import { useMemo, useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Modal } from "@/components/ui/Modal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useToast } from "@/hooks/useToast";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { useItemCostMutations, useItemCosts, useItemMasterLookups, useCostSources } from "@/features/item-master/api/item-master-queries";
import type { CostRecordResponse } from "@/lib/api/api-client";
import { fileClient } from "@/lib/api/file-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";

const schema = z.object({
  costSourceId: z.string().uuid(),
  unitId: z.string().uuid(),
  amount: z.string().trim().regex(/^\d+(\.\d{1,4})?$/),
  currency: z.string().trim().length(3),
  effectiveFrom: z.string().min(1),
  sourceReference: z.string().trim().min(1).max(128),
  reason: z.string().trim().min(1).max(500),
});
type FormValues = z.infer<typeof schema>;

function parseAmount(value: string): number | null {
  const [whole = "", fractional = ""] = value.split(".");
  const scaled = Number(`${whole}${fractional.padEnd(4, "0")}`);
  if (!Number.isSafeInteger(scaled)) return null;
  return scaled / 10000;
}

interface CostRecordMaintenanceProps {
  itemId: string;
}

export function CostRecordMaintenance({ itemId }: CostRecordMaintenanceProps) {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const costs = useItemCosts(itemId);
  const lookups = useItemMasterLookups();
  const sources = useCostSources();
  const mutations = useItemCostMutations(itemId);
  const mayCreate = can(selectedMembership, "cost-records.create");
  const maySubmit = can(selectedMembership, "cost-records.submit");
  const [open, setOpen] = useState(false);
  const [evidenceFile, setEvidenceFile] = useState<File | null>(null);
  const [verifiedEvidenceFileId, setVerifiedEvidenceFileId] = useState<string | null>(null);
  const [createdDraft, setCreatedDraft] = useState<CostRecordResponse | null>(null);
  const createKeyRef = useRef<string | null>(null);
  const uploadKeyRef = useRef<string | null>(null);
  const defaultDate = new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 16);
  const form = useForm<FormValues>({ resolver: zodResolver(schema), defaultValues: { costSourceId: "", unitId: "", amount: "", currency: "THB", effectiveFrom: defaultDate, sourceReference: "", reason: "" } });
  const saving = form.formState.isSubmitting || mutations.create.isPending || mutations.update.isPending;
  const rows = costs.data ?? [];
  const columns = useMemo<Column<CostRecordResponse>[]>(() => [
    { id: "status", header: t("status"), cell: (_value, row) => row.status === "draft" ? t("costStatusDraft") : row.status === "submitted" ? t("statusSubmitted") : row.status === "approved" ? t("statusApproved") : row.status === "published" ? t("costStatusPublished") : row.status === "returned" ? t("costStatusReturned") : row.status === "disabled" ? t("costStatusDisabled") : "-" },
    { id: "source", header: t("costSources"), cell: (_value, row) => row.costSourceName ? (locale === "en" ? row.costSourceName.english ?? "-" : row.costSourceName.thai ?? "-") : "-" },
    { id: "amount", header: t("amount"), cell: (_value, row) => row.amount === undefined ? "-" : new Intl.NumberFormat(locale, { style: "currency", currency: row.currency ?? "THB", maximumFractionDigits: 4 }).format(row.amount), className: "text-right tabular-nums" },
    { id: "period", header: t("effectiveFrom"), cell: (_value, row) => row.effectiveFromUtc ? new Intl.DateTimeFormat(locale, { dateStyle: "medium", timeZone: "UTC" }).format(new Date(row.effectiveFromUtc)) : "-" },
    { id: "reference", header: t("sourceReference"), accessorKey: "sourceReference" },
    { id: "reason", header: t("reason"), accessorKey: "reason", className: "min-w-48" },
    { id: "evidence", header: t("evidence"), cell: (_value, row) => row.evidenceFileId ? t("evidenceAttached") : "-" },
    { id: "action", header: t("rowActions"), isAction: true, sticky: "right", cell: (_value, row) => row.status === "draft" && maySubmit && row.id && row.rowVersion ? <Button type="button" size="sm" className="min-h-[44px]" disabled={mutations.submit.isPending} isLoading={mutations.submit.isPending} onClick={() => { const costId = row.id; const rowVersion = row.rowVersion; if (!costId || !rowVersion) return; void mutations.submit.mutateAsync({ costId, rowVersion }).then(() => toast.success(t("costSubmitted"))).catch(() => toast.error(t("versionConflict"))); }}>{t("submitForReview")}</Button> : null },
  ], [locale, maySubmit, mutations.submit.isPending, t, toast]);

  const submit = form.handleSubmit(async (values) => {
    const amount = parseAmount(values.amount);
    if (amount === null) {
      form.setError("amount", { message: t("amountPrecisionError") });
      return;
    }
    try {
      const payload = {
        unitId: values.unitId,
        currency: values.currency.toUpperCase(),
        amount,
        minimumQuantity: 0,
        effectiveFromUtc: new Date(values.effectiveFrom).toISOString(),
        costSourceId: values.costSourceId,
        sourceReference: values.sourceReference,
        reason: values.reason,
      };
      let draft = createdDraft;
      if (!draft) {
        createKeyRef.current ??= crypto.randomUUID();
        draft = await mutations.create.mutateAsync({
          idempotencyKey: createKeyRef.current,
          payload: { ...payload, scope: "organization" },
        });
        createKeyRef.current = null;
        setCreatedDraft(draft);
      }
      if (evidenceFile) {
        const token = await getAuthToken();
        const membershipId = selectedMembership?.id;
        if (!token) throw new AuthenticationRequiredError();
        if (!membershipId) throw new MembershipRequiredError();
        const draftId = draft.id;
        const rowVersion = draft.rowVersion;
        if (!draftId || !rowVersion) throw new Error("Draft response omitted its identifier or version.");
        let evidenceFileId = verifiedEvidenceFileId;
        if (!evidenceFileId) {
          uploadKeyRef.current ??= crypto.randomUUID();
          const session = await fileClient.createSession({
            parentType: "costRecord",
            parentId: draftId,
            files: [{ filename: evidenceFile.name, mediaType: evidenceFile.type || "application/octet-stream", fileSizeBytes: evidenceFile.size }],
          }, { token, membershipId, locale: locale === "en" ? "en" : "th", idempotencyKey: uploadKeyRef.current });
          const slotId = session.slots?.[0]?.slotId;
          if (!session.sessionId || !slotId) throw new Error("Upload session omitted its session or slot identifier.");
          const completed = await fileClient.completeSession(session.sessionId, [{ slotId, file: evidenceFile }], { token, membershipId, locale: locale === "en" ? "en" : "th" });
          evidenceFileId = completed.files?.[0]?.fileId ?? null;
          if (!evidenceFileId) throw new Error("Verified upload omitted its file identifier.");
          setVerifiedEvidenceFileId(evidenceFileId);
        }
        await mutations.update.mutateAsync({ costId: draftId, rowVersion, payload: { ...payload, evidenceFileId } });
      } else if (createdDraft) {
        const draftId = draft.id;
        const rowVersion = draft.rowVersion;
        if (!draftId || !rowVersion) throw new Error("Draft response omitted its identifier or version.");
        await mutations.update.mutateAsync({ costId: draftId, rowVersion, payload });
      }
      setCreatedDraft(null);
      setEvidenceFile(null);
      setVerifiedEvidenceFileId(null);
      uploadKeyRef.current = null;
      setOpen(false);
      form.reset({ ...form.getValues(), amount: "", sourceReference: "", reason: "" });
      toast.success(t("costDraftCreated"));
    } catch {
      toast.error(t("createFailed"));
    }
  });

  if (costs.isLoading || lookups.isLoading || sources.isLoading) return <section className="border-t border-erp-border pt-5"><MonoSpinner label={t("loading")} /></section>;
  const activeSources = (sources.data ?? []).filter((source) => source.isActive && source.sourceType === "manual");
  const activeUnits = (lookups.data?.units ?? []).filter((unit) => unit.status === "active");
  return (
    <section className="space-y-3 border-t border-erp-border pt-5" aria-labelledby="item-costs-heading">
      <div className="flex flex-wrap items-center justify-between gap-3"><h2 id="item-costs-heading" className="text-lg font-semibold text-erp-text-main">{t("costHistory")}</h2>{mayCreate && <Button type="button" onClick={() => setOpen(true)}>{createdDraft ? t("finishCostDraft") : t("createCostDraft")}</Button>}</div>
      {(costs.isError || lookups.isError || sources.isError) && <p role="alert" className="border border-erp-danger p-3 text-erp-danger">{t("createFailed")}</p>}
      <DataTable<CostRecordResponse> columns={columns} data={rows} isLoading={costs.isLoading} isError={costs.isError} error={costs.error} onRetry={() => { void costs.refetch(); }} emptyTitle={t("noCostRecords")} hidePagination />
      <Modal isOpen={open} onClose={() => { if (!saving) setOpen(false); }} closeDisabled={saving} title={t("createCostDraft")}>
        <form className="space-y-4" noValidate onSubmit={(event) => { void submit(event); }}>
          <Select
            id="cost-source"
            label={t("costSources")}
            required
            placeholder={common("actions.select")}
            disabled={saving}
            error={form.formState.errors.costSourceId ? t("validationRequired") : undefined}
            options={activeSources.filter((source): source is typeof source & { id: string } => Boolean(source.id)).map((source) => ({
              value: source.id,
              label: `${source.code ?? "-"} · ${locale === "en" ? source.name?.english ?? "-" : source.name?.thai ?? "-"}`,
            }))}
            {...form.register("costSourceId")}
          />
          <Select
            id="cost-unit"
            label={t("unit")}
            required
            placeholder={common("actions.select")}
            disabled={saving}
            options={activeUnits.filter((unit): unit is typeof unit & { id: string } => Boolean(unit.id)).map((unit) => ({
              value: unit.id,
              label: `${unit.code ?? "-"} · ${locale === "en" ? unit.name?.english ?? "-" : unit.name?.thai ?? "-"}`,
            }))}
            {...form.register("unitId")}
          />
          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <Input
              label={t("amount")}
              required
              inputMode="decimal"
              placeholder={t("amountPlaceholder")}
              {...form.register("amount")}
              error={form.formState.errors.amount?.message}
              disabled={saving}
            />
            <Input
              label={t("currency")}
              required
              maxLength={3}
              placeholder={t("currencyPlaceholder")}
              {...form.register("currency")}
              disabled={saving}
            />
            <Input
              label={t("effectiveFrom")}
              required
              type="datetime-local"
              {...form.register("effectiveFrom")}
              disabled={saving}
            />
            <Input
              label={t("sourceReference")}
              required
              placeholder={t("sourceReferencePlaceholder")}
              {...form.register("sourceReference")}
              error={form.formState.errors.sourceReference?.message}
              disabled={saving}
            />
          </div>
          <Input
            label={t("reason")}
            required
            placeholder={t("reasonPlaceholder")}
            {...form.register("reason")}
            error={form.formState.errors.reason?.message}
            disabled={saving}
          />
          <div className="erp-form-group"><label className="erp-label" htmlFor="cost-evidence">{t("evidence")}</label><input id="cost-evidence" type="file" className="erp-input" disabled={saving} onChange={(event) => { setEvidenceFile(event.target.files?.item(0) ?? null); setVerifiedEvidenceFileId(null); uploadKeyRef.current = null; }} /><p className="text-xs text-erp-text-muted">{t("evidenceDeferredHelp")}</p></div>
          <div className="flex justify-end gap-2 border-t border-erp-border pt-4"><Button type="button" variant="outline" disabled={saving} onClick={() => setOpen(false)}>{common("actions.cancel")}</Button><Button type="submit" isLoading={saving} disabled={saving || !mayCreate}>{t("saveCostDraft")}</Button></div>
        </form>
      </Modal>
    </section>
  );
}
