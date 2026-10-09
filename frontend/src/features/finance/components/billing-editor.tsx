"use client";

import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency } from "@/lib/formatters/formatters";
import { useProjectList } from "@/features/projects/api/project-queries";
import { useBillingMutations, useProjectBillingSummary } from "../api/finance-queries";
import { BILLING_KINDS, financeErrorCode } from "../finance-status";

const PAGE_LIMIT = 100;

export function BillingEditor() {
  const t = useTranslations("finance.billings");
  const tErrors = useTranslations("finance.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const mutations = useBillingMutations();
  const projects = useProjectList({ page: 1, pageSize: PAGE_LIMIT });

  const [projectId, setProjectId] = useState("");
  const [kind, setKind] = useState("milestone");
  const [description, setDescription] = useState("");
  const [amount, setAmount] = useState("");
  const [dueDate, setDueDate] = useState("");
  const [error, setError] = useState<string | null>(null);
  const summary = useProjectBillingSummary(projectId || undefined);
  // One key per form session so a retried create replays instead of issuing a second billing.
  const createKeyRef = useRef(crypto.randomUUID());
  const isBusy = mutations.create.isPending;

  async function save(): Promise<void> {
    setError(null);
    const value = Number(amount);
    if (!projectId || !description.trim() || !(value > 0) || Math.round(value * 100) / 100 !== value) {
      setError(t("fieldsInvalid"));
      return;
    }

    try {
      const saved = await mutations.create.mutateAsync({
        payload: { projectId, kind, description: description.trim(), amount: value, dueDate: dueDate || null },
        idempotencyKey: createKeyRef.current,
      });
      toast.success(t("created"));
      router.push(`/${locale}/finance/billings/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? financeErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader title={t("createTitle")} subtitle={t("editorSubtitle")} breadcrumbs={[{ label: t("title"), href: `/${locale}/finance/billings` }, { label: t("create") }]} />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
      <div className="erp-card grid grid-cols-1 gap-4 p-5 md:grid-cols-2">
        <Select
          label={t("project")}
          required
          value={projectId}
          placeholder={t("selectProject")}
          disabled={isBusy}
          options={(projects.data?.items ?? []).map((p) => ({ value: p.id ?? "", label: `${p.code ?? "-"} · ${p.name ?? "-"}` }))}
          onChange={(event) => setProjectId(event.target.value)}
        />
        <Select label={t("kind")} value={kind} disabled={isBusy} options={BILLING_KINDS.map((value) => ({ value, label: t(`kinds.${value}`) }))} onChange={(event) => setKind(event.target.value)} />
        <Input label={t("description")} placeholder={t("descriptionPlaceholder")} required value={description} maxLength={200} disabled={isBusy} onChange={(event) => setDescription(event.target.value)} />
        <Input type="number" min={0} step="0.01" label={t("amount")} placeholder={t("amountPlaceholder")} required value={amount} disabled={isBusy} onChange={(event) => setAmount(event.target.value)} />
        <Input type="date" label={t("dueDate")} value={dueDate} disabled={isBusy} onChange={(event) => setDueDate(event.target.value)} />
        {summary.data && (
          <dl className="grid grid-cols-3 gap-3 text-xs md:col-span-2" aria-label={t("summaryTitle")}>
            <div><dt className="text-erp-text-muted">{t("contractAmount")}</dt><dd className="font-mono font-semibold">{formatCurrency(summary.data.contractAmount, "THB", locale)}</dd></div>
            <div><dt className="text-erp-text-muted">{t("billed")}</dt><dd className="font-mono font-semibold">{formatCurrency(summary.data.billed, "THB", locale)}</dd></div>
            <div><dt className="text-erp-text-muted">{t("unbilled")}</dt><dd className="font-mono font-semibold">{formatCurrency(summary.data.unbilled, "THB", locale)}</dd></div>
          </dl>
        )}
      </div>
      <div className="flex justify-end gap-3">
        <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => router.back()}>{tCommon("actions.cancel")}</Button>
        <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{t("issue")}</Button>
      </div>
    </section>
  );
}
