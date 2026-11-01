"use client";

import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { IconPlus, IconTrash } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import type { BomRevisionResponse, BomRoutingStepRequest, ItemResponse } from "@/lib/api/api-client";
import { useBom, useBomMutations, useWorkCenterList } from "../api/production-queries";
import { productionErrorCode } from "../production-status";

interface DraftLine {
  key: string;
  itemId: string;
  itemCode: string;
  itemName: string;
  unitCode: string;
  quantity: string;
  scrapPercent: string;
}

interface DraftRoutingStep {
  key: string;
  sequence: string;
  operationCode: string;
  operationName: string;
  workCenterId: string;
  setupHours: string;
  runHours: string;
  isSubcontracted: boolean;
  stdSubcontractCost: string;
  note: string;
}

interface BomEditorProps {
  /** Existing BOM when editing a draft revision; omit to create a new BOM. */
  bomId?: string;
  revisionId?: string;
}

function fromRevision(revision: BomRevisionResponse): DraftLine[] {
  return (revision.lines ?? []).map((line) => ({
    key: line.id ?? crypto.randomUUID(),
    itemId: line.component?.id ?? "",
    itemCode: line.component?.code ?? "-",
    itemName: line.component?.nameTh ?? "-",
    unitCode: line.component?.unitCode ?? "-",
    quantity: String(line.quantity ?? 1),
    scrapPercent: String(line.scrapPercent ?? 0),
  }));
}

function routingFromRevision(revision: BomRevisionResponse): DraftRoutingStep[] {
  return (revision.routingSteps ?? []).map((step) => ({
    key: step.id ?? crypto.randomUUID(),
    sequence: String(step.sequence ?? 1),
    operationCode: step.operationCode ?? "",
    operationName: step.operationName ?? "",
    workCenterId: step.workCenter?.id ?? "",
    setupHours: String(step.setupHours ?? 0),
    runHours: String(step.runHours ?? 0),
    isSubcontracted: step.isSubcontracted ?? false,
    stdSubcontractCost: String(step.stdSubcontractCost ?? 0),
    note: step.note ?? "",
  }));
}

export function BomEditor({ bomId, revisionId }: BomEditorProps) {
  const query = useBom(bomId);
  const t = useTranslations("production.boms");
  if (bomId && query.isLoading) {
    return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  }

  if (bomId && (query.isError || !query.data)) {
    return <Alert variant="danger">{t("loadError")}</Alert>;
  }

  const revision = query.data?.revisions?.find((r) => r.id === revisionId);
  if (bomId && (!revision || revision.status !== "draft")) {
    return <Alert variant="warning">{t("onlyDraftEditable")}</Alert>;
  }

  // Keyed by the loaded version so the form is rebuilt from server data, never patched in an effect.
  return (
    <EditorForm
      key={revision?.rowVersion ?? "new"}
      bomId={bomId}
      bomCode={query.data?.code ?? undefined}
      fixedItemLabel={query.data?.item ? `${query.data.item.code ?? "-"} · ${query.data.item.nameTh ?? "-"}` : undefined}
      revision={revision}
    />
  );
}

interface EditorFormProps {
  bomId?: string;
  bomCode?: string;
  fixedItemLabel?: string;
  revision?: BomRevisionResponse;
}

function EditorForm({ bomId, bomCode, fixedItemLabel, revision }: EditorFormProps) {
  const t = useTranslations("production.boms");
  const tErrors = useTranslations("production.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const mutations = useBomMutations();
  const workCentersQuery = useWorkCenterList({ status: "active", page: 1, pageSize: 100 });

  const [itemId, setItemId] = useState("");
  const [outputQuantity, setOutputQuantity] = useState(String(revision?.outputQuantity ?? 1));
  const [note, setNote] = useState(revision?.note ?? "");
  const [lines, setLines] = useState<DraftLine[]>(revision ? fromRevision(revision) : []);
  const [routingSteps, setRoutingSteps] = useState<DraftRoutingStep[]>(revision ? routingFromRevision(revision) : []);
  const [selectedComponent, setSelectedComponent] = useState<ItemResponse | null>(null);
  const [selectedComponentId, setSelectedComponentId] = useState("");
  const [pickerKey, setPickerKey] = useState(0);
  const [error, setError] = useState<string | null>(null);
  // One key per form session so a retried create replays instead of creating a second BOM.
  const createKeyRef = useRef(crypto.randomUUID());

  const isBusy = mutations.create.isPending || mutations.updateDraft.isPending;
  const isEdit = Boolean(bomId && revision);

  function updateLine(key: string, patch: Partial<DraftLine>): void {
    setLines((current) => current.map((line) => (line.key === key ? { ...line, ...patch } : line)));
  }

  function handleAddComponent(): void {
    if (!selectedComponentId || !selectedComponent) return;
    if (lines.some((l) => l.itemId === selectedComponentId)) {
      setSelectedComponentId("");
      setSelectedComponent(null);
      setPickerKey((k) => k + 1);
      return;
    }

    const name = (locale === "en" ? selectedComponent?.name?.english : selectedComponent?.name?.thai) ?? "-";
    setLines((current) => [
      ...current,
      {
        key: crypto.randomUUID(),
        itemId: selectedComponentId,
        itemCode: selectedComponent?.code ?? "-",
        itemName: name,
        unitCode: selectedComponent?.baseUnit?.code ?? "-",
        quantity: "1",
        scrapPercent: "0",
      },
    ]);
    setSelectedComponentId("");
    setSelectedComponent(null);
    setPickerKey((k) => k + 1);
  }

  function updateRoutingStep(key: string, patch: Partial<DraftRoutingStep>): void {
    setRoutingSteps((current) => current.map((step) => (step.key === key ? { ...step, ...patch } : step)));
  }

  async function save(): Promise<void> {
    setError(null);
    if (!bomId && !itemId) {
      setError(t("itemRequired"));
      return;
    }

    const output = Number(outputQuantity);
    const parsed = lines.map((line) => ({
      componentItemId: line.itemId,
      quantity: Number(line.quantity),
      scrapPercent: Number(line.scrapPercent),
    }));
    if (!(output > 0) || parsed.length === 0 || parsed.some((l) => !(l.quantity > 0) || !(l.scrapPercent >= 0 && l.scrapPercent <= 50))) {
      setError(t("linesInvalid"));
      return;
    }

    // Validate routing steps if any
    const parsedRouting: BomRoutingStepRequest[] = [];
    if (routingSteps.length > 0) {
      for (const step of routingSteps) {
        const seq = Number(step.sequence);
        if (!Number.isInteger(seq) || seq <= 0) {
          setError(t("routingInvalid"));
          return;
        }
        if (!step.operationCode.trim() || !step.operationName.trim()) {
          setError(t("routingInvalid"));
          return;
        }
        if (!step.isSubcontracted) {
          if (!step.workCenterId) {
            setError(t("routingInvalid"));
            return;
          }
          const setup = Number(step.setupHours);
          const run = Number(step.runHours);
          if (Number.isNaN(setup) || setup < 0 || Number.isNaN(run) || run < 0) {
            setError(t("routingInvalid"));
            return;
          }
        } else {
          const cost = Number(step.stdSubcontractCost);
          if (Number.isNaN(cost) || cost < 0) {
            setError(t("routingInvalid"));
            return;
          }
        }
      }

      const seqs = routingSteps.map((s) => Number(s.sequence));
      if (new Set(seqs).size !== seqs.length) {
        setError(t("routingInvalid"));
        return;
      }

      for (const step of routingSteps) {
        parsedRouting.push({
          sequence: Number(step.sequence),
          operationCode: step.operationCode.trim(),
          operationName: step.operationName.trim(),
          workCenterId: step.isSubcontracted ? null : (step.workCenterId || null),
          setupHours: step.isSubcontracted ? 0 : Number(step.setupHours || 0),
          runHours: step.isSubcontracted ? 0 : Number(step.runHours || 0),
          isSubcontracted: step.isSubcontracted,
          stdSubcontractCost: step.isSubcontracted ? Number(step.stdSubcontractCost || 0) : 0,
          note: step.note.trim() || null,
        });
      }
    }

    try {
      const saved = bomId && revision
        ? await mutations.updateDraft.mutateAsync({
            id: bomId,
            revisionId: revision.id ?? "",
            rowVersion: revision.rowVersion ?? "",
            payload: {
              outputQuantity: output,
              note: note.trim() || null,
              lines: parsed,
              routingSteps: parsedRouting,
            },
          })
        : await mutations.create.mutateAsync({
            payload: {
              itemId,
              outputQuantity: output,
              note: note.trim() || null,
              lines: parsed,
              routingSteps: parsedRouting,
            },
            idempotencyKey: createKeyRef.current,
          });
      toast.success(bomId ? t("updated") : t("created"));
      router.push(`/${locale}/production/boms/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? productionErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-6 pb-20">
      <PageHeader
        title={bomId ? t("editTitle", { code: bomCode ?? "-", revision: revision?.revisionNo ?? 0 }) : t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/production/boms` }, { label: bomCode ?? t("create") }]}
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <FormSection title={t("title")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          {bomId ? (
            <div className="erp-form-group !mb-0">
              <label className="erp-label !mb-1.5">{t("item")}</label>
              <div className="h-11 min-h-[44px] flex items-center px-3 border border-erp-border bg-erp-surface-disabled font-medium text-sm">
                {fixedItemLabel ?? "-"}
              </div>
            </div>
          ) : (
            <div className="erp-form-group !mb-0">
              <ItemAutocomplete label={t("item")} required producibleOnly stockableOnly disabled={isBusy} value={itemId} onChange={(id) => setItemId(id)} />
            </div>
          )}
          <Input type="number" min={0} step="0.0001" label={t("outputQuantity")} value={outputQuantity} disabled={isBusy} placeholder={t("outputQuantityPlaceholder")} onChange={(event) => setOutputQuantity(event.target.value)} />
          <div className="md:col-span-2">
            <Input label={t("note")} value={note} maxLength={500} disabled={isBusy} placeholder={t("notePlaceholder")} onChange={(event) => setNote(event.target.value)} />
          </div>
        </div>
      </FormSection>

      <FormSection title={t("componentsTitle")}>
        <div className="flex flex-col gap-4">
          <div className="erp-form-group !mb-0">
            <label className="erp-label !mb-1.5">{t("addComponent")}</label>
            <div className="flex items-center gap-2">
              <div className="flex-1">
                <ItemAutocomplete
                  key={pickerKey}
                  label=""
                  variant="filter"
                  stockableOnly
                  disabled={isBusy}
                  value={selectedComponentId}
                  onChange={(componentId, item) => {
                    setSelectedComponentId(componentId);
                    setSelectedComponent(item);
                  }}
                />
              </div>
              <Button
                type="button"
                variant="primary"
                size="md"
                className="h-11 min-h-[44px] shrink-0"
                icon={<IconPlus size={16} />}
                disabled={isBusy || !selectedComponentId}
                onClick={handleAddComponent}
              >
                {t("addComponent")}
              </Button>
            </div>
          </div>

          <div className="overflow-x-auto border border-erp-border bg-erp-surface">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border bg-erp-surface-subtle text-erp-navy font-bold">
                  <th className="py-2.5 px-3 w-12 text-center">#</th>
                  <th className="py-2.5 px-3 min-w-[200px]">{t("componentsTitle")}</th>
                  <th className="py-2.5 px-3 w-36">{t("quantity")}</th>
                  <th className="py-2.5 px-3 w-36">{t("scrapPercent")} (%)</th>
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
                        {line.unitCode}
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
                      <Input
                        type="number"
                        min={0}
                        max={50}
                        step="0.01"
                        aria-label={t("scrapPercent")}
                        placeholder={t("scrapPercent")}
                        value={line.scrapPercent}
                        disabled={isBusy}
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px] font-mono text-right"
                        onChange={(event) => updateLine(line.key, { scrapPercent: event.target.value })}
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
                    <td colSpan={5} className="py-8 text-center text-sm text-erp-text-muted">
                      {t("linesEmpty")}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </FormSection>

      <FormSection
        title={t("routing.title")}
        headerAction={
          <Button
            type="button"
            variant="outline"
            size="sm"
            className="h-9"
            icon={<IconPlus size={14} />}
            disabled={isBusy}
            onClick={() => {
              const nextSeq = routingSteps.length > 0
                ? Math.max(...routingSteps.map((s) => Number(s.sequence) || 0)) + 10
                : 10;
              setRoutingSteps((current) => [
                ...current,
                {
                  key: crypto.randomUUID(),
                  sequence: String(nextSeq),
                  operationCode: `OP-${String(nextSeq).padStart(3, "0")}`,
                  operationName: "",
                  workCenterId: "",
                  setupHours: "0",
                  runHours: "0",
                  isSubcontracted: false,
                  stdSubcontractCost: "0",
                  note: "",
                },
              ]);
            }}
          >
            {t("routing.addStep")}
          </Button>
        }
      >
        {routingSteps.length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("routing.empty")}</p>
        ) : (
          <div className="overflow-x-auto border border-erp-border bg-erp-surface">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border bg-erp-surface-subtle text-erp-navy font-bold">
                  <th className="py-2.5 px-3 w-20 text-center">{t("routing.sequence")}</th>
                  <th className="py-2.5 px-3 w-28">{t("routing.operationCode")}</th>
                  <th className="py-2.5 px-3 w-44">{t("routing.operationName")}</th>
                  <th className="py-2.5 px-3 w-32">{t("routing.subcontract")}</th>
                  <th className="py-2.5 px-3 w-48">{t("routing.workCenter")}</th>
                  <th className="py-2.5 px-3 w-24 text-right">{t("routing.setupHours")}</th>
                  <th className="py-2.5 px-3 w-24 text-right">{t("routing.runHours")}</th>
                  <th className="py-2.5 px-3 w-32 text-right">{t("routing.stdSubcontractCost")}</th>
                  <th className="py-2.5 px-3 min-w-[120px]">{t("routing.note")}</th>
                  <th className="py-2.5 px-3 text-center w-14">
                    <span className="sr-only">{tCommon("actions.delete")}</span>
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-erp-border/60">
                {routingSteps.map((step) => (
                  <tr key={step.key} className="align-middle hover:bg-erp-surface-subtle/50 transition-colors">
                    <td className="py-2 px-3 text-center">
                      <Input
                        type="number"
                        min={1}
                        step={1}
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px] w-14 text-center font-mono"
                        aria-label={t("routing.sequence")}
                        placeholder={t("routing.sequencePlaceholder")}
                        value={step.sequence}
                        disabled={isBusy}
                        onChange={(e) => updateRoutingStep(step.key, { sequence: e.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3">
                      <Input
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px] font-mono"
                        aria-label={t("routing.operationCode")}
                        placeholder={t("routing.operationCodePlaceholder")}
                        value={step.operationCode}
                        disabled={isBusy}
                        onChange={(e) => updateRoutingStep(step.key, { operationCode: e.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3">
                      <Input
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px]"
                        aria-label={t("routing.operationName")}
                        placeholder={t("routing.operationNamePlaceholder")}
                        value={step.operationName}
                        disabled={isBusy}
                        onChange={(e) => updateRoutingStep(step.key, { operationName: e.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3 text-center">
                      <Checkbox
                        checked={step.isSubcontracted}
                        label={t("routing.isSubcontracted")}
                        disabled={isBusy}
                        onCheckedChange={(checked) => updateRoutingStep(step.key, { isSubcontracted: checked })}
                      />
                    </td>
                    <td className="py-2 px-3">
                      {step.isSubcontracted ? (
                        <span className="text-erp-text-muted text-xs">-</span>
                      ) : (
                        <Select
                          wrapperClassName="!mb-0"
                          aria-label={t("routing.workCenter")}
                          placeholder={t("routing.selectWorkCenter")}
                          value={step.workCenterId}
                          disabled={isBusy}
                          options={(workCentersQuery.data?.items ?? []).map((wc) => ({
                            value: wc.id ?? "",
                            label: `${wc.code ?? "-"} · ${wc.name ?? "-"}`,
                          }))}
                          onChange={(e) => updateRoutingStep(step.key, { workCenterId: e.target.value })}
                        />
                      )}
                    </td>
                    <td className="py-2 px-3">
                      {step.isSubcontracted ? (
                        <span className="text-erp-text-muted text-xs">-</span>
                      ) : (
                        <Input
                          type="number"
                          min={0}
                          step="0.1"
                          wrapperClassName="!mb-0"
                          className="h-9 min-h-[36px] text-right font-mono"
                          aria-label={t("routing.setupHours")}
                          placeholder="0.0"
                          value={step.setupHours}
                          disabled={isBusy}
                          onChange={(e) => updateRoutingStep(step.key, { setupHours: e.target.value })}
                        />
                      )}
                    </td>
                    <td className="py-2 px-3">
                      {step.isSubcontracted ? (
                        <span className="text-erp-text-muted text-xs">-</span>
                      ) : (
                        <Input
                          type="number"
                          min={0}
                          step="0.1"
                          wrapperClassName="!mb-0"
                          className="h-9 min-h-[36px] text-right font-mono"
                          aria-label={t("routing.runHours")}
                          placeholder="0.0"
                          value={step.runHours}
                          disabled={isBusy}
                          onChange={(e) => updateRoutingStep(step.key, { runHours: e.target.value })}
                        />
                      )}
                    </td>
                    <td className="py-2 px-3">
                      {!step.isSubcontracted ? (
                        <span className="text-erp-text-muted text-xs">-</span>
                      ) : (
                        <Input
                          type="number"
                          min={0}
                          step="0.01"
                          wrapperClassName="!mb-0"
                          className="h-9 min-h-[36px] text-right font-mono"
                          aria-label={t("routing.stdSubcontractCost")}
                          placeholder="0.00"
                          value={step.stdSubcontractCost}
                          disabled={isBusy}
                          onChange={(e) => updateRoutingStep(step.key, { stdSubcontractCost: e.target.value })}
                        />
                      )}
                    </td>
                    <td className="py-2 px-3">
                      <Input
                        wrapperClassName="!mb-0"
                        className="h-9 min-h-[36px]"
                        aria-label={t("routing.note")}
                        placeholder={t("routing.notePlaceholder")}
                        value={step.note}
                        disabled={isBusy}
                        onChange={(e) => updateRoutingStep(step.key, { note: e.target.value })}
                      />
                    </td>
                    <td className="py-2 px-3 text-center">
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        className="h-9 w-9 text-erp-danger hover:text-erp-danger hover:bg-erp-danger/10"
                        disabled={isBusy}
                        title={t("routing.removeStep")}
                        aria-label={t("routing.removeStep")}
                        icon={<IconTrash size={16} />}
                        onClick={() => setRoutingSteps((current) => current.filter((s) => s.key !== step.key))}
                      />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </FormSection>

      <FormActionBar
        isEditMode={isEdit}
        isDirty={true}
        isLoading={isBusy}
        sticky
        saveButtonType="button"
        onSave={() => void save()}
        onCancel={() => router.back()}
      />
    </section>
  );
}
