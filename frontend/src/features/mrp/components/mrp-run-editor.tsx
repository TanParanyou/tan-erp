"use client";

import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import { useMrpMutations } from "../api/mrp-queries";
import { mrpErrorCode } from "../mrp-status";

interface DemandLine {
  key: string;
  itemId: string;
  label: string;
  quantity: string;
  needBy: string;
  reference: string;
}

function todayIso(): string {
  return new Date().toISOString().slice(0, 10);
}

export function MrpRunEditor() {
  const t = useTranslations("mrp.runs");
  const tErrors = useTranslations("mrp.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const mutations = useMrpMutations();

  const [asOfDate, setAsOfDate] = useState(todayIso);
  const [purchaseLead, setPurchaseLead] = useState("7");
  const [productionLead, setProductionLead] = useState("3");
  const [includeOpen, setIncludeOpen] = useState(true);
  const [lines, setLines] = useState<DemandLine[]>([]);
  const [pickerKey, setPickerKey] = useState(0);
  const [error, setError] = useState<string | null>(null);
  // One key per form session so a retried run replays instead of creating a second one.
  const createKeyRef = useRef(crypto.randomUUID());
  const isBusy = mutations.create.isPending;

  function updateLine(key: string, patch: Partial<DemandLine>): void {
    setLines((current) => current.map((line) => (line.key === key ? { ...line, ...patch } : line)));
  }

  async function run(): Promise<void> {
    setError(null);
    const purchase = Number(purchaseLead);
    const production = Number(productionLead);
    const demands = lines.map((line) => ({ itemId: line.itemId, quantity: Number(line.quantity), needBy: line.needBy, reference: line.reference.trim() || null }));
    if (!asOfDate || !Number.isInteger(purchase) || !Number.isInteger(production) || purchase < 0 || production < 0) {
      setError(t("paramsInvalid"));
      return;
    }

    if ((demands.length === 0 && !includeOpen) || demands.some((d) => !(d.quantity > 0) || !d.needBy || d.needBy < asOfDate)) {
      setError(t("demandsInvalid"));
      return;
    }

    try {
      const saved = await mutations.create.mutateAsync({
        payload: { asOfDate, purchaseLeadTimeDays: purchase, productionLeadTimeDays: production, includeOpenWorkOrders: includeOpen, demands },
        idempotencyKey: createKeyRef.current,
      });
      toast.success(t("created"));
      router.push(`/${locale}/production/mrp/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? mrpErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader
        title={t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/production/mrp` }, { label: t("create") }]}
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <div className="erp-card p-5 grid grid-cols-1 gap-4 md:grid-cols-3">
        <Input type="date" label={t("asOfDate")} value={asOfDate} disabled={isBusy} onChange={(event) => setAsOfDate(event.target.value)} />
        <Input type="number" min={0} step="1" label={t("purchaseLeadTime")} value={purchaseLead} disabled={isBusy} onChange={(event) => setPurchaseLead(event.target.value)} />
        <Input type="number" min={0} step="1" label={t("productionLeadTime")} value={productionLead} disabled={isBusy} onChange={(event) => setProductionLead(event.target.value)} />
        <label className="flex items-center gap-2 text-sm md:col-span-3">
          <input type="checkbox" className="h-4 w-4" checked={includeOpen} disabled={isBusy} onChange={(event) => setIncludeOpen(event.target.checked)} />
          {t("includeOpenWorkOrders")}
        </label>
      </div>

      <div className="erp-card p-5 space-y-4">
        <h3 className="text-sm font-bold text-erp-navy">{t("demandsTitle")}</h3>
        <ul className="space-y-2">
          {lines.map((line) => (
            <li key={line.key} className="grid grid-cols-1 gap-2 border border-erp-border bg-erp-surface-subtle p-3 sm:grid-cols-[1fr_120px_150px_160px_auto] sm:items-end">
              <div className="text-sm font-medium text-erp-text-main">{line.label}</div>
              <Input type="number" min={0} step="0.0001" aria-label={t("quantity")} placeholder={t("quantity")} value={line.quantity} disabled={isBusy} onChange={(event) => updateLine(line.key, { quantity: event.target.value })} />
              <Input type="date" aria-label={t("needBy")} value={line.needBy} disabled={isBusy} onChange={(event) => updateLine(line.key, { needBy: event.target.value })} />
              <Input aria-label={t("reference")} placeholder={t("reference")} maxLength={100} value={line.reference} disabled={isBusy} onChange={(event) => updateLine(line.key, { reference: event.target.value })} />
              <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={isBusy} onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}>{t("removeLine")}</Button>
            </li>
          ))}
          {lines.length === 0 && <li className="text-xs text-erp-text-muted">{t("demandsEmpty")}</li>}
        </ul>
        <ItemAutocomplete
          key={pickerKey}
          label={t("addDemand")}
          disabled={isBusy}
          onChange={(itemId, item) => {
            if (!itemId) {
              setPickerKey((k) => k + 1);
              return;
            }

            const name = (locale === "en" ? item?.name?.english : item?.name?.thai) ?? "-";
            setLines((current) => [...current, {
              key: crypto.randomUUID(),
              itemId,
              label: `${item?.code ?? "-"} · ${name} (${item?.baseUnit?.code ?? "-"})`,
              quantity: "1",
              needBy: asOfDate,
              reference: "",
            }]);
            setPickerKey((k) => k + 1);
          }}
        />
      </div>

      <div className="flex justify-end gap-3">
        <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => router.back()}>{tCommon("actions.cancel")}</Button>
        <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void run()}>{t("runPlan")}</Button>
      </div>
    </section>
  );
}
