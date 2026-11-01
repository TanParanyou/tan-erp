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
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { IconPlus, IconTrash } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useProjectList } from "@/features/projects/api/project-queries";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import { useItemMasterLookups } from "@/features/item-master/api/item-master-queries";
import type { ItemResponse } from "@/lib/api/api-client";
import { useMrpMutations } from "../api/mrp-queries";
import { mrpErrorCode } from "../mrp-status";
import { MrpSettingsDrawer } from "./mrp-settings-drawer";

interface DemandLine {
  key: string;
  itemId: string;
  itemCode: string;
  itemName: string;
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
  const { selectedMembership } = useSelectedMembership();
  const canReadProjects = can(selectedMembership, PERMISSIONS.PROJECTS_READ);
  const projects = useProjectList({ page: 1, pageSize: 100 }, canReadProjects);
  const openProjects = (projects.data?.items ?? []).filter((p) => p.id && (p.status === "planned" || p.status === "active"));

  const [asOfDate, setAsOfDate] = useState(todayIso);
  const [purchaseLead, setPurchaseLead] = useState("7");
  const [productionLead, setProductionLead] = useState("3");
  const [includeOpen, setIncludeOpen] = useState(true);
  const [lines, setLines] = useState<DemandLine[]>([]);
  const [projectIds, setProjectIds] = useState<string[]>([]);
  const [selectedItem, setSelectedItem] = useState<ItemResponse | null>(null);
  const [selectedItemId, setSelectedItemId] = useState("");
  const [pickerKey, setPickerKey] = useState(0);
  const [error, setError] = useState<string | null>(null);
  const [settingsOpen, setSettingsOpen] = useState(false);
  // One key per form session so a retried run replays instead of creating a second one.
  const createKeyRef = useRef(crypto.randomUUID());
  const isBusy = mutations.create.isPending;

  const isDirty = useMemo(() => {
    return Boolean(
      asOfDate !== todayIso() ||
      purchaseLead !== "7" ||
      productionLead !== "3" ||
      !includeOpen ||
      lines.length > 0 ||
      projectIds.length > 0
    );
  }, [asOfDate, purchaseLead, productionLead, includeOpen, lines, projectIds]);

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

  function handleAddDemand(): void {
    if (!selectedItemId || !selectedItem) return;

    const name = (locale === "en" ? selectedItem?.name?.english : selectedItem?.name?.thai) ?? "-";
    const baseCode = selectedItem?.baseUnit?.code ?? "";
    setLines((current) => [
      ...current,
      {
        key: crypto.randomUUID(),
        itemId: selectedItemId,
        itemCode: selectedItem?.code ?? "-",
        itemName: name,
        quantity: "1",
        needBy: asOfDate,
        reference: "",
        unitId: "",
        baseUnitCode: baseCode,
      },
    ]);
    setSelectedItemId("");
    setSelectedItem(null);
    setPickerKey((k) => k + 1);
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

    if ((demands.length === 0 && projectIds.length === 0 && !includeOpen) || demands.some((d) => !(d.quantity > 0) || !d.needBy || d.needBy < asOfDate)) {
      setError(t("demandsInvalid"));
      return;
    }

    try {
      const saved = await mutations.create.mutateAsync({
        payload: { asOfDate, purchaseLeadTimeDays: purchase, productionLeadTimeDays: production, includeOpenWorkOrders: includeOpen, demands, projectIds },
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
    <section className="space-y-6 pb-20">
      <PageHeader
        title={t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/production/mrp` }, { label: t("create") }]}
        actions={
          <Button
            type="button"
            variant="outline"
            className="h-11 min-h-[44px]"
            disabled={isBusy}
            onClick={() => setSettingsOpen(true)}
          >
            {t("settingsButton")}
          </Button>
        }
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <FormSection title={t("title")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
          <Input type="date" label={t("asOfDate")} placeholder={t("datePlaceholder")} value={asOfDate} disabled={isBusy} onChange={(event) => setAsOfDate(event.target.value)} />
          <Input type="number" min={0} step="1" label={t("purchaseLeadTime")} value={purchaseLead} disabled={isBusy} placeholder={t("purchaseLeadTimePlaceholder")} onChange={(event) => setPurchaseLead(event.target.value)} />
          <Input type="number" min={0} step="1" label={t("productionLeadTime")} value={productionLead} disabled={isBusy} placeholder={t("productionLeadTimePlaceholder")} onChange={(event) => setProductionLead(event.target.value)} />
          <div className="flex items-center md:col-span-3">
            <Checkbox checked={includeOpen} disabled={isBusy} onCheckedChange={(checked) => setIncludeOpen(checked === true)} label={t("includeOpenWorkOrders")} />
          </div>
          {canReadProjects && (
            <div className="md:col-span-3 space-y-2">
              <h3 className="text-sm font-bold text-erp-navy">{t("projectsTitle")}</h3>
              <p className="text-xs text-erp-text-muted">{t("projectsHint")}</p>
              {openProjects.length === 0 ? (
                <p className="text-xs text-erp-text-muted">{t("projectsEmpty")}</p>
              ) : (
                <ul className="grid grid-cols-1 gap-2 md:grid-cols-2">
                  {openProjects.map((project) => (
                    <li key={project.id}>
                      <Checkbox
                        checked={projectIds.includes(project.id ?? "")}
                        disabled={isBusy}
                        onCheckedChange={(checked) => setProjectIds((current) => checked === true ? [...current, project.id ?? ""] : current.filter((id) => id !== project.id))}
                        label={`${project.code ?? "-"} · ${project.name ?? "-"}`}
                      />
                    </li>
                  ))}
                </ul>
              )}
            </div>
          )}
        </div>
      </FormSection>

      <FormSection title={t("demandsTitle")}>
        <div className="flex flex-col gap-4">
          <div className="erp-form-group !mb-0">
            <label className="erp-label !mb-1.5">{t("addDemand")}</label>
            <div className="flex items-center gap-2">
              <div className="flex-1">
                <ItemAutocomplete
                  key={pickerKey}
                  label=""
                  variant="filter"
                  disabled={isBusy}
                  value={selectedItemId}
                  onChange={(itemId, item) => {
                    setSelectedItemId(itemId);
                    setSelectedItem(item);
                  }}
                />
              </div>
              <Button
                type="button"
                variant="primary"
                size="md"
                className="h-11 min-h-[44px] shrink-0"
                icon={<IconPlus size={16} />}
                disabled={isBusy || !selectedItemId}
                onClick={handleAddDemand}
              >
                {t("addDemand")}
              </Button>
            </div>
          </div>

          <div className="overflow-x-auto border border-erp-border bg-erp-surface">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border bg-erp-surface-subtle text-erp-navy font-bold">
                  <th className="py-2.5 px-3 w-12 text-center">#</th>
                  <th className="py-2.5 px-3 min-w-[200px]">{t("demandsTitle")}</th>
                  <th className="py-2.5 px-3 w-32">{t("quantity")}</th>
                  <th className="py-2.5 px-3 w-36">{t("unit")}</th>
                  <th className="py-2.5 px-3 w-36">{t("needBy")}</th>
                  <th className="py-2.5 px-3 min-w-[160px]">{t("reference")}</th>
                  <th className="py-2.5 px-3 w-14 text-center">
                    <span className="sr-only">{tCommon("actions.delete")}</span>
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-erp-border/60">
                {lines.map((line, index) => (
                  <tr key={line.key} className="align-middle hover:bg-erp-surface-subtle/50 transition-colors">
                    <td className="py-2 px-3 text-center font-mono text-erp-text-muted">
                      {index + 1}
                    </td>
                    <td className="py-2 px-3">
                      <div className="text-sm font-medium text-erp-text-main">
                        <span className="font-mono text-erp-navy font-semibold">{line.itemCode}</span> · {line.itemName}
                      </div>
                      <div className="text-xs text-erp-text-muted">
                        {line.baseUnitCode || "-"}
                      </div>
                    </td>
                    <td className="py-2 px-3">
                      <Input
                        type="number"
                        min={0}
                        step="0.0001"
                        aria-label={t("quantity")}
                        placeholder={t("quantity")}
                        value={line.quantity}
                        disabled={isBusy}
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px] font-mono text-right"
                        onChange={(event) => updateLine(line.key, { quantity: event.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3">
                      <Select
                        aria-label={t("unit")}
                        placeholder={t("unit")}
                        value={line.unitId}
                        disabled={isBusy}
                        wrapperClassName="!mb-0"
                        options={[
                          { value: "", label: t("baseUnit", { code: line.baseUnitCode || "-" }) },
                          ...unitOptions
                            .filter((u) => u.code !== line.baseUnitCode)
                            .map((u) => ({ value: u.value, label: u.label })),
                        ]}
                        onChange={(event) => updateLine(line.key, { unitId: event.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3">
                      <Input
                        type="date"
                        aria-label={t("needBy")}
                        placeholder={t("datePlaceholder")}
                        value={line.needBy}
                        disabled={isBusy}
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px]"
                        onChange={(event) => updateLine(line.key, { needBy: event.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3">
                      <Input
                        aria-label={t("reference")}
                        placeholder={t("reference")}
                        maxLength={100}
                        value={line.reference}
                        disabled={isBusy}
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px]"
                        onChange={(event) => updateLine(line.key, { reference: event.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3 text-center">
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        className="h-9 w-9 text-erp-danger hover:text-erp-danger hover:bg-erp-danger/10"
                        disabled={isBusy}
                        title={t("removeLine")}
                        aria-label={t("removeLine")}
                        icon={<IconTrash size={16} />}
                        onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}
                      />
                    </td>
                  </tr>
                ))}
                {lines.length === 0 && (
                  <tr>
                    <td colSpan={7} className="py-8 text-center text-sm text-erp-text-muted">
                      {t("demandsEmpty")}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </FormSection>

      <FormActionBar
        isEditMode={false}
        isDirty={isDirty}
        isLoading={isBusy}
        sticky
        saveText={t("runPlan")}
        saveButtonType="button"
        onSave={() => void run()}
        onCancel={() => router.back()}
      />

      <MrpSettingsDrawer isOpen={settingsOpen} onClose={() => setSettingsOpen(false)} />
    </section>
  );
}
