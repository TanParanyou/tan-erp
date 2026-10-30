"use client";

import { useMemo, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import { useItemMasterLookups } from "@/features/item-master/api/item-master-queries";
import { useMrpMutations } from "../api/mrp-queries";
import { mrpErrorCode } from "../mrp-status";
import { MrpSettingsDrawer } from "./mrp-settings-drawer";

interface DemandLine {
  key: string;
  itemId: string;
  label: string;
  quantity: string;
  needBy: string;
  reference: string;
  unitId: string;
  baseUnitCode: string;
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
  const lookups = useItemMasterLookups();

  const [asOfDate, setAsOfDate] = useState(todayIso);
  const [purchaseLead, setPurchaseLead] = useState("7");
  const [productionLead, setProductionLead] = useState("3");
  const [includeOpen, setIncludeOpen] = useState(true);
  const [lines, setLines] = useState<DemandLine[]>([]);
  const [pickerKey, setPickerKey] = useState(0);
  const [error, setError] = useState<string | null>(null);
  const [settingsOpen, setSettingsOpen] = useState(false);
  // One key per form session so a retried run replays instead of creating a second one.
  const createKeyRef = useRef(crypto.randomUUID());
  const isBusy = mutations.create.isPending;

  const unitOptions = useMemo(
    () =>
      (lookups.data?.units ?? [])
        .filter((u) => u.status !== "inactive")
        .map((u) => ({
          value: u.id ?? "",
          code: u.code ?? "",
          label: `${u.code ?? "-"} (${(locale === "en" ? u.name?.english : u.name?.thai) ?? u.code ?? "-"})`,
        })),
    [lookups.data?.units, locale]
  );

  function updateLine(key: string, patch: Partial<DemandLine>): void {
    setLines((current) => current.map((line) => (line.key === key ? { ...line, ...patch } : line)));
  }

  async function run(): Promise<void> {
    setError(null);
    const purchase = Number(purchaseLead);
    const production = Number(productionLead);
    const demands = lines.map((line) => ({
      itemId: line.itemId,
      quantity: Number(line.quantity),
      needBy: line.needBy,
      reference: line.reference.trim() || null,
      unitId: line.unitId || null,
    }));
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
        actions={
          <Button
            type="button"
            variant="outline"
            className="min-h-11"
            disabled={isBusy}
            onClick={() => setSettingsOpen(true)}
          >
            {t("settingsButton")}
          </Button>
        }
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <div className="erp-card p-5 grid grid-cols-1 gap-4 md:grid-cols-3">
        <Input type="date" label={t("asOfDate")} placeholder={t("datePlaceholder")} value={asOfDate} disabled={isBusy} onChange={(event) => setAsOfDate(event.target.value)} />
        <Input type="number" min={0} step="1" label={t("purchaseLeadTime")} value={purchaseLead} disabled={isBusy} placeholder={t("purchaseLeadTimePlaceholder")} onChange={(event) => setPurchaseLead(event.target.value)} />
        <Input type="number" min={0} step="1" label={t("productionLeadTime")} value={productionLead} disabled={isBusy} placeholder={t("productionLeadTimePlaceholder")} onChange={(event) => setProductionLead(event.target.value)} />
        <div className="flex items-center md:col-span-3">
          <Checkbox checked={includeOpen} disabled={isBusy} onCheckedChange={(checked) => setIncludeOpen(checked === true)} label={t("includeOpenWorkOrders")} />
        </div>
      </div>

      <div className="erp-card p-5 space-y-4">
        <h3 className="text-sm font-bold text-erp-navy">{t("demandsTitle")}</h3>
        <ul className="space-y-2">
          {lines.map((line) => (
            <li
              key={line.key}
              className="grid grid-cols-1 gap-2 border border-erp-border bg-erp-surface-subtle p-3 sm:grid-cols-[1fr_110px_150px_140px_140px_auto] sm:items-end"
            >
              <div className="text-sm font-medium text-erp-text-main pb-2 sm:pb-0">{line.label}</div>
              <Input
                type="number"
                min={0}
                step="0.0001"
                aria-label={t("quantity")}
                placeholder={t("quantity")}
                value={line.quantity}
                disabled={isBusy}
                wrapperClassName="!mb-0"
                className="h-11 min-h-[44px]"
                onChange={(event) => updateLine(line.key, { quantity: event.target.value })}
              />
              <Select
                aria-label={t("unit")}
                placeholder={t("unit")}
                value={line.unitId}
                disabled={isBusy}
                wrapperClassName="!mb-0"
                className="h-11 min-h-[44px]"
                options={[
                  { value: "", label: t("baseUnit", { code: line.baseUnitCode || "-" }) },
                  ...unitOptions
                    .filter((u) => u.code !== line.baseUnitCode)
                    .map((u) => ({ value: u.value, label: u.label })),
                ]}
                onChange={(event) => updateLine(line.key, { unitId: event.target.value })}
              />
              <Input
                type="date"
                aria-label={t("needBy")}
                placeholder={t("datePlaceholder")}
                value={line.needBy}
                disabled={isBusy}
                wrapperClassName="!mb-0"
                className="h-11 min-h-[44px]"
                onChange={(event) => updateLine(line.key, { needBy: event.target.value })}
              />
              <Input
                aria-label={t("reference")}
                placeholder={t("reference")}
                maxLength={100}
                value={line.reference}
                disabled={isBusy}
                wrapperClassName="!mb-0"
                className="h-11 min-h-[44px]"
                onChange={(event) => updateLine(line.key, { reference: event.target.value })}
              />
              <Button
                type="button"
                variant="outline"
                size="sm"
                className="min-h-11"
                disabled={isBusy}
                onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}
              >
                {t("removeLine")}
              </Button>
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
            const baseCode = item?.baseUnit?.code ?? "";
            setLines((current) => [
              ...current,
              {
                key: crypto.randomUUID(),
                itemId,
                label: `${item?.code ?? "-"} · ${name} (${baseCode || "-"})`,
                quantity: "1",
                needBy: asOfDate,
                reference: "",
                unitId: "",
                baseUnitCode: baseCode,
              },
            ]);
            setPickerKey((k) => k + 1);
          }}
        />
      </div>

      <div className="flex justify-end gap-3">
        <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => router.back()}>{tCommon("actions.cancel")}</Button>
        <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void run()}>{t("runPlan")}</Button>
      </div>

      <MrpSettingsDrawer isOpen={settingsOpen} onClose={() => setSettingsOpen(false)} />
    </section>
  );
}
