"use client";

import { useMemo, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { IconPlus, IconTrash } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import { useProjectList } from "@/features/projects/api/project-queries";
import type { ItemResponse, PurchaseRequestRequest, PurchaseRequestResponse } from "@/lib/api/api-client";
import { usePurchaseRequest, usePurchaseRequestMutations } from "../api/procurement-queries";
import { procurementErrorCode } from "../procurement-status";

interface DraftLine {
  key: string;
  itemId: string;
  itemCode: string;
  itemName: string;
  unitCode: string;
  quantity: string;
  estimatedUnitPrice: string;
  note: string;
}

interface PurchaseRequestEditorProps {
  /** Existing draft to edit; omit to create a new purchase request. */
  purchaseRequestId?: string;
}

const PAGE_LIMIT = 100;

function fromRequest(req: PurchaseRequestResponse): DraftLine[] {
  return (req.lines ?? []).map((line) => ({
    key: line.id ?? crypto.randomUUID(),
    itemId: line.itemId ?? "",
    itemCode: line.itemCode ?? "-",
    itemName: line.itemNameTh ?? "-",
    unitCode: line.unitCode ?? "-",
    quantity: String(line.quantity ?? 1),
    estimatedUnitPrice: String(line.estimatedUnitPrice ?? 0),
    note: line.note ?? "",
  }));
}

export function PurchaseRequestEditor({ purchaseRequestId }: PurchaseRequestEditorProps) {
  const requestQuery = usePurchaseRequest(purchaseRequestId);
  const t = useTranslations("procurement.requests");

  if (purchaseRequestId && requestQuery.isLoading) {
    return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  }

  if (purchaseRequestId && (requestQuery.isError || !requestQuery.data)) {
    return <Alert variant="danger">{t("loadError")}</Alert>;
  }

  if (purchaseRequestId && requestQuery.data?.status !== "draft") {
    return <Alert variant="warning">{t("onlyDraftEditable")}</Alert>;
  }

  return <EditorForm key={requestQuery.data?.rowVersion ?? "new"} request={requestQuery.data} />;
}

function EditorForm({ request }: { request?: PurchaseRequestResponse }) {
  const t = useTranslations("procurement.requests");
  const tErrors = useTranslations("procurement.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canReadProjects = can(selectedMembership, PERMISSIONS.PROJECTS_READ);
  const mutations = usePurchaseRequestMutations();
  const projects = useProjectList({ page: 1, pageSize: PAGE_LIMIT }, canReadProjects);

  const [projectId, setProjectId] = useState(request?.project?.id ?? "");
  const [requiredByDate, setRequiredByDate] = useState(request?.requiredByDate ?? "");
  const [note, setNote] = useState(request?.note ?? "");
  const [lines, setLines] = useState<DraftLine[]>(request ? fromRequest(request) : []);
  const [selectedItem, setSelectedItem] = useState<ItemResponse | null>(null);
  const [selectedItemId, setSelectedItemId] = useState<string>("");
  const [itemPickerKey, setItemPickerKey] = useState(0);
  const [error, setError] = useState<string | null>(null);
  const createKeyRef = useRef(crypto.randomUUID());

  const isBusy = mutations.create.isPending || mutations.update.isPending;
  const isEdit = Boolean(request);

  const isDirty = useMemo(() => {
    if (!request) {
      return Boolean(projectId || requiredByDate || note || lines.length > 0);
    }
    const origProjectId = request.project?.id ?? "";
    const origRequiredByDate = request.requiredByDate ?? "";
    const origNote = request.note ?? "";
    const origLines = fromRequest(request);
    if (projectId !== origProjectId || requiredByDate !== origRequiredByDate || note !== origNote) {
      return true;
    }
    if (lines.length !== origLines.length) {
      return true;
    }
    return lines.some((line, index) => {
      const orig = origLines[index];
      return (
        line.itemId !== orig.itemId ||
        line.quantity !== orig.quantity ||
        line.estimatedUnitPrice !== orig.estimatedUnitPrice ||
        line.note !== orig.note
      );
    });
  }, [request, projectId, requiredByDate, note, lines]);

  const total = lines.reduce(
    (sum, line) => sum + (Number(line.quantity) || 0) * (Number(line.estimatedUnitPrice) || 0),
    0,
  );

  function updateLine(key: string, patch: Partial<DraftLine>): void {
    setLines((current) => current.map((line) => (line.key === key ? { ...line, ...patch } : line)));
  }

  function handleAddItem(): void {
    if (!selectedItemId || !selectedItem) return;
    if (lines.some((l) => l.itemId === selectedItemId)) {
      setSelectedItemId("");
      setSelectedItem(null);
      setItemPickerKey((k) => k + 1);
      return;
    }

    const name = (locale === "en" ? selectedItem?.name?.english : selectedItem?.name?.thai) ?? "-";
    setLines((current) => [
      ...current,
      {
        key: crypto.randomUUID(),
        itemId: selectedItemId,
        itemCode: selectedItem?.code ?? "-",
        itemName: name,
        unitCode: selectedItem?.baseUnit?.code ?? "-",
        quantity: "1",
        estimatedUnitPrice: "0",
        note: "",
      },
    ]);
    setSelectedItemId("");
    setSelectedItem(null);
    setItemPickerKey((k) => k + 1);
  }

  async function save(): Promise<void> {
    setError(null);

    const parsed = lines.map((line) => ({
      itemId: line.itemId,
      quantity: Number(line.quantity),
      estimatedUnitPrice: Number(line.estimatedUnitPrice),
      note: line.note.trim() || null,
    }));

    if (
      parsed.length === 0 ||
      parsed.some(
        (l) =>
          !(l.quantity > 0) ||
          !(l.estimatedUnitPrice >= 0) ||
          !Number.isFinite(l.quantity) ||
          !Number.isFinite(l.estimatedUnitPrice),
      )
    ) {
      setError(t("linesInvalid"));
      return;
    }

    const payload: PurchaseRequestRequest = {
      projectId: projectId || null,
      requiredByDate: requiredByDate || null,
      note: note.trim() || null,
      lines: parsed,
    };

    try {
      const saved = request
        ? await mutations.update.mutateAsync({ id: request.id ?? "", rowVersion: request.rowVersion ?? "", payload })
        : await mutations.create.mutateAsync({ payload, idempotencyKey: createKeyRef.current });
      toast.success(request ? t("updated") : t("created"));
      router.push(`/${locale}/procurement/purchase-requests/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? procurementErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-6 pb-20">
      <PageHeader
        title={request ? t("editTitle", { number: request.number ?? "-" }) : t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[
          { label: t("title"), href: `/${locale}/procurement/purchase-requests` },
          { label: request?.number ?? t("create") },
        ]}
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <FormSection title={t("title")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          {canReadProjects && (
            <Select
              label={t("project")}
              value={projectId}
              placeholder={t("noProject")}
              disabled={isBusy}
              options={(projects.data?.items ?? []).map((p) => ({ value: p.id ?? "", label: `${p.code ?? "-"} · ${p.name ?? "-"}` }))}
              onChange={(event) => setProjectId(event.target.value)}
            />
          )}
          <Input
            type="date"
            label={t("requiredByDate")}
            placeholder={t("datePlaceholder")}
            value={requiredByDate}
            disabled={isBusy}
            onChange={(event) => setRequiredByDate(event.target.value)}
          />
          <div className="md:col-span-2">
            <Input
              label={t("note")}
              value={note}
              maxLength={500}
              disabled={isBusy}
              placeholder={t("notePlaceholder")}
              onChange={(event) => setNote(event.target.value)}
            />
          </div>
        </div>
      </FormSection>

      <FormSection
        title={t("linesTitle")}
        headerAction={
          <span className="font-mono text-sm font-semibold text-erp-navy">
            {t("draftTotal", { amount: formatCurrency(total, "THB", locale) })}
          </span>
        }
      >
        <div className="flex flex-col gap-4">
          <div className="erp-form-group !mb-0">
            <label className="erp-label !mb-1.5">{t("addItem")}</label>
            <div className="flex items-center gap-2">
              <div className="flex-1">
                <ItemAutocomplete
                  key={itemPickerKey}
                  label=""
                  variant="filter"
                  purchasableOnly
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
                onClick={handleAddItem}
              >
                {t("addItem")}
              </Button>
            </div>
          </div>

          <div className="overflow-x-auto border border-erp-border bg-erp-surface">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border bg-erp-surface-subtle text-erp-navy font-bold">
                  <th className="py-2.5 px-3 w-12 text-center">{t("lineNo")}</th>
                  <th className="py-2.5 px-3 min-w-[200px]">{t("item")}</th>
                  <th className="py-2.5 px-3 w-32">{t("quantity")}</th>
                  <th className="py-2.5 px-3 w-36">{t("estimatedUnitPrice")}</th>
                  <th className="py-2.5 px-3 w-32 text-right">{t("estimatedLineTotal")}</th>
                  <th className="py-2.5 px-3 min-w-[180px]">{t("note")}</th>
                  <th className="py-2.5 px-3 w-14 text-center">
                    <span className="sr-only">{t("removeLine")}</span>
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-erp-border/60">
                {lines.map((line, index) => {
                  const lineTotal = (Number(line.quantity) || 0) * (Number(line.estimatedUnitPrice) || 0);
                  return (
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
                          placeholder={t("quantityPlaceholder")}
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
                          step="0.01"
                          aria-label={t("estimatedUnitPrice")}
                          placeholder={t("pricePlaceholder")}
                          value={line.estimatedUnitPrice}
                          disabled={isBusy}
                          wrapperClassName="!mb-0"
                          className="h-9 min-h-[36px] font-mono text-right"
                          onChange={(event) => updateLine(line.key, { estimatedUnitPrice: event.target.value })}
                        />
                      </td>
                      <td className="py-2 px-3 text-right font-mono font-semibold text-erp-navy text-sm">
                        {formatCurrency(lineTotal, "THB", locale)}
                      </td>
                      <td className="py-2 px-3">
                        <Input
                          aria-label={t("lineNote")}
                          placeholder={t("lineNotePlaceholder")}
                          value={line.note}
                          maxLength={200}
                          disabled={isBusy}
                          wrapperClassName="!mb-0"
                          className="h-9 min-h-[36px]"
                          onChange={(event) => updateLine(line.key, { note: event.target.value })}
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
                  );
                })}
                {lines.length === 0 && (
                  <tr>
                    <td colSpan={7} className="py-8 text-center text-sm text-erp-text-muted">
                      {t("linesEmpty")}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </FormSection>

      <FormActionBar
        isEditMode={isEdit}
        isDirty={isDirty}
        isLoading={isBusy}
        sticky
        saveButtonType="button"
        onSave={() => void save()}
        onCancel={() => router.back()}
        statusContent={
          <span className="font-mono text-sm font-semibold text-erp-navy">
            {t("draftTotal", { amount: formatCurrency(total, "THB", locale) })}
          </span>
        }
      />
    </section>
  );
}
