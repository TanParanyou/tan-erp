"use client";

import { useMemo, useRef, useState, type FormEvent } from "react";
import { useLocale, useTranslations } from "next-intl";
import { z } from "zod";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from "@/components/ui/Table";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListSearchInput } from "@/components/ui/ListSearchInput";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { ActiveFilterChips, type ActiveFilterChipItem } from "@/components/ui/ActiveFilterChips";
import { ExportDropdown } from "@/components/ui/ExportDropdown";
import { Avatar } from "@/components/ui/Avatar";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Drawer } from "@/components/ui/Drawer";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { MultiLangInput } from "@/components/forms/MultiLangInput";
import { ImageUpload } from "@/components/forms/ImageUpload";
import { GeneratedCodeField } from "@/components/forms/GeneratedCodeField";
import { FormSection } from "@/components/forms/FormSection";
import { useToast } from "@/hooks/useToast";
import { useAuthenticatedFileUrl } from "@/hooks/useAuthenticatedFileUrl";
import { getAuthToken } from "@/lib/auth/auth-session";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { LIST_PAGE_SIZES, useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import { useDataExport } from "@/hooks/useDataExport";
import type { ExportColumn } from "@/lib/export/export-types";
import { uploadVerifiedItemImage } from "@/features/item-master/api/upload-item-image";
import { ITEM_IMAGE_MAX_BYTES } from "@/features/item-master/item-image-constants";
import { useItemMasterReferenceData, useItemMasterReferenceMutations, useSharedUnitConversionMutations, useSharedUnitConversions } from "@/features/item-master/api/item-master-queries";
import type { ItemBrandResponse, ItemCategoryResponse, UnitOfMeasureResponse } from "@/lib/api/api-client";

type Kind = "categories" | "brands" | "units" | "taxCategories";

interface ReferenceFilters extends ListFilterRecord {
  status?: string;
}

function isListPageSize(value: number): value is ListPageSize {
  return LIST_PAGE_SIZES.some((pageSize) => pageSize === value);
}

function hasActiveUnitIdentity(unit: UnitOfMeasureResponse): unit is UnitOfMeasureResponse & { id: string } {
  return unit.status === "active" && typeof unit.id === "string";
}

const itemTypeOptions = [
  ["material", "itemTypeMaterial"],
  ["labor", "itemTypeLabor"],
  ["service", "itemTypeService"],
  ["subcontract", "itemTypeSubcontract"],
  ["other", "itemTypeOther"],
  ["product", "itemTypeProduct"],
] as const;

const allItemTypes = itemTypeOptions.map(([value]) => value);

const unitDimensionOptions = [
  ["length", "unitDimensionLength"],
  ["area", "unitDimensionArea"],
  ["volume", "unitDimensionVolume"],
  ["mass", "unitDimensionMass"],
  ["time", "unitDimensionTime"],
  ["count", "unitDimensionCount"],
  ["custom", "unitDimensionCustom"],
] as const;

const unitRoundingModes = [
  ["half_up", "roundingHalfUp"],
  ["half_even", "roundingHalfEven"],
  ["up", "roundingAwayFromZero"],
  ["down", "roundingTowardZero"],
  ["ceiling", "roundingTowardPositive"],
  ["floor", "roundingTowardNegative"],
] as const;
const unitRoundingModeValues = unitRoundingModes.map(([value]) => value) as [string, ...string[]];

function normalizeUnitRoundingMode(value: string | null | undefined): string {
  if (!value?.trim()) return "half_up";
  const normalized = value?.trim().toLowerCase().replaceAll("_", "").replaceAll("-", "");
  const match = unitRoundingModes.find(([mode]) => mode.replaceAll("_", "") === normalized);
  return match?.[0] ?? value;
}

type ConversionField = "fromUnitId" | "toUnitId" | "factor" | "effectiveFrom" | "effectiveTo" | "reason";
type ConversionErrorKey = "conversionRequired" | "conversionSameUnit" | "conversionFactorInvalid" | "conversionDateRangeInvalid";
type ConversionErrors = Partial<Record<ConversionField, ConversionErrorKey>>;

function createSchema(kind: Kind) {
  return z.object({
  code: z.string().trim().max(64),
  codeMode: z.enum(["generated", "manual"]),
  name: z.object({ th: z.string().trim().min(1), en: z.string().optional() }),
  description: z.object({ th: z.string().optional(), en: z.string().optional() }),
  parentCategoryId: z.string(),
  allowedItemTypes: z.array(z.string()).min(1),
  sortOrder: z.number().int().min(0),
  imageFile: z.custom<File | null>((value) => value === null || (typeof File !== "undefined" && value instanceof File))
    .refine((file) => file === null || (file.size > 0 && file.size <= ITEM_IMAGE_MAX_BYTES), { message: "IMAGE_SIZE_INVALID" }),
  symbol: z.string().trim().max(16),
  dimension: z.string().trim().max(32),
  decimalScale: z.number().int().min(0).max(6),
  roundingMode: z.enum(unitRoundingModeValues),
  }).superRefine((values, context) => {
    if (values.codeMode === "manual" && !values.code) {
      context.addIssue({ code: "custom", path: ["code"], message: "CODE_REQUIRED" });
    }
    const maximumLength = kind === "units" ? 20 : 30;
    if (values.code.length > maximumLength) {
      context.addIssue({ code: "custom", path: ["code"], message: "CODE_LENGTH" });
    }
    if (kind === "units" && !values.symbol) {
      context.addIssue({ code: "custom", path: ["symbol"], message: "REQUIRED" });
    }
    if (kind === "units" && !values.dimension) {
      context.addIssue({ code: "custom", path: ["dimension"], message: "REQUIRED" });
    }
  });
}
type FormValues = z.infer<ReturnType<typeof createSchema>>;
interface ReferenceRow {
  id?: string;
  code?: string | null;
  name?: ItemCategoryResponse["name"];
  description?: ItemCategoryResponse["description"];
  parentCategoryId?: string | null;
  sortOrder?: number;
  status?: string | null;
  rowVersion?: string;
  imageFileId?: string | null;
  symbol?: string | null;
  dimension?: string | null;
  decimalScale?: number;
  roundingMode?: string | null;
  allowedItemTypes?: string[] | null;
}

function getDescendantCategoryIds(categories: ReferenceRow[], categoryId: string): Set<string> {
  const descendants = new Set<string>();
  let parentIds = new Set([categoryId]);
  while (parentIds.size > 0) {
    const nextParentIds = new Set<string>();
    for (const category of categories) {
      if (category.id && category.parentCategoryId && parentIds.has(category.parentCategoryId) && !descendants.has(category.id)) {
        descendants.add(category.id);
        nextParentIds.add(category.id);
      }
    }
    parentIds = nextParentIds;
  }
  return descendants;
}

export function TaxonomyMaintenance() {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const data = useItemMasterReferenceData();
  const mutations = useItemMasterReferenceMutations();
  const sharedConversions = useSharedUnitConversions();
  const sharedConversionMutations = useSharedUnitConversionMutations();
  const canManage = can(selectedMembership, "items.manage-taxonomy");
  const list = useListState<ReferenceFilters>({
    schema: { single: ["status"], defaultSort: "code", defaultOrder: "asc", allowedSorts: ["code", "name", "status"] },
  });
  const [kind, setKind] = useState<Kind>("categories");
  const [editing, setEditing] = useState<ReferenceRow | null>(null);
  const [discardOpen, setDiscardOpen] = useState(false);
  const [open, setOpen] = useState(false);
  const [isSavingReference, setIsSavingReference] = useState(false);
  const [imageFileId, setImageFileId] = useState<string | null>(null);
  const [imagePreparing, setImagePreparing] = useState(false);
  const uploadedImageFileIdRef = useRef<string | null>(null);
  const imageUploadKeyRef = useRef<string | null>(null);
  const savingReferenceRef = useRef(false);
  const [conversionFromUnitId, setConversionFromUnitId] = useState("");
  const [conversionToUnitId, setConversionToUnitId] = useState("");
  const [conversionFactor, setConversionFactor] = useState("");
  const [conversionEffectiveFrom, setConversionEffectiveFrom] = useState(() => new Date().toISOString().slice(0, 10));
  const [conversionEffectiveTo, setConversionEffectiveTo] = useState("");
  const [conversionReason, setConversionReason] = useState("");
  const [conversionErrors, setConversionErrors] = useState<ConversionErrors>({});
  const formSchema = useMemo(() => createSchema(kind), [kind]);
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      code: "", codeMode: "generated", name: { th: "", en: "" }, description: { th: "", en: "" }, parentCategoryId: "",
      allowedItemTypes: allItemTypes, sortOrder: 0, imageFile: null, symbol: "", dimension: "count", decimalScale: 0, roundingMode: "half_up",
    },
  });
  const { objectUrl: existingImageUrl } = useAuthenticatedFileUrl(imageFileId);
  const savingReference = isSavingReference || form.formState.isSubmitting || mutations.createCategory.isPending || mutations.updateCategory.isPending || mutations.createBrand.isPending || mutations.updateBrand.isPending || mutations.createTaxCategory.isPending || mutations.updateTaxCategory.isPending || mutations.createUnit.isPending || mutations.updateUnit.isPending;
  const savingConversion = sharedConversionMutations.create.isPending;
  const dimensionOptions = useMemo(() => unitDimensionOptions.map(([value, label]) => ({ value, label: t(label) })), [t]);
  const rawRows = kind === "categories" ? data.data?.categories ?? [] : kind === "brands" ? data.data?.brands ?? [] : kind === "units" ? data.data?.units ?? [] : data.data?.taxCategories ?? [];
  const allRows: ReferenceRow[] = rawRows;
  const filteredRows = useMemo(() => {
    const query = list.params.search.toLocaleLowerCase(locale);
    const status = list.params.filters.status;
    const collator = new Intl.Collator(locale, { numeric: true, sensitivity: "base" });
    return allRows.filter((row) => {
      const name = locale === "en" ? row.name?.english ?? "" : row.name?.thai ?? "";
      const matchesSearch = !query || [row.code ?? "", name, row.symbol ?? "", row.dimension ?? ""]
        .some((value) => value.toLocaleLowerCase(locale).includes(query));
      return matchesSearch && (!status || row.status === status);
    }).sort((left, right) => {
      const key = list.params.sort ?? "code";
      const leftValue = key === "name"
        ? (locale === "en" ? left.name?.english ?? "" : left.name?.thai ?? "")
        : key === "status" ? left.status ?? "" : left.code ?? "";
      const rightValue = key === "name"
        ? (locale === "en" ? right.name?.english ?? "" : right.name?.thai ?? "")
        : key === "status" ? right.status ?? "" : right.code ?? "";
      const compared = collator.compare(leftValue, rightValue);
      return (list.params.order === "asc" ? compared : -compared) || collator.compare(left.id ?? "", right.id ?? "");
    });
  }, [allRows, list.params.filters.status, list.params.order, list.params.search, list.params.sort, locale]);
  const pageCount = Math.ceil(filteredRows.length / list.params.limit);
  const pageRows = filteredRows.slice((list.params.page - 1) * list.params.limit, list.params.page * list.params.limit);
  const categories: ReferenceRow[] = data.data?.categories ?? [];
  const blockedParentIds = editing?.id ? new Set([editing.id, ...getDescendantCategoryIds(categories, editing.id)]) : new Set<string>();
  const parentCategoryOptions = categories.filter((category) => category.id && category.status === "active" && !blockedParentIds.has(category.id));
  const codeLabel = kind === "categories" ? t("categoryCode") : kind === "brands" ? t("brandCode") : kind === "units" ? t("unitCode") : t("taxCategoryCodeLabel");
  const nameLabel = kind === "categories" ? t("categoryName") : kind === "brands" ? t("brandName") : kind === "units" ? t("unitName") : t("taxCategoryName");
  const columns = useMemo<Column<ReferenceRow>[]>(() => [
    ...(kind === "categories" || kind === "brands" ? [{
      id: "image",
      header: kind === "categories" ? t("categoryImage") : t("brandImage"),
      cell: (_value: unknown, row: ReferenceRow) => {
        const localizedName = locale === "en" ? row.name?.english : row.name?.thai;
        return <Avatar initial={localizedName ?? undefined} fileId={row.imageFileId} alt={localizedName ?? ""} size="sm" />;
      },
    }] : []),
    { id: "code", header: codeLabel, accessorKey: "code", sortable: true, className: "font-mono" },
    { id: "name", header: nameLabel, sortable: true, cell: (_value, row) => locale === "en" ? row.name?.english ?? "-" : row.name?.thai ?? "-" },
    ...(kind === "units" ? [{ id: "symbol", header: t("unitSymbol"), accessorKey: "symbol" as const }, { id: "dimension", header: t("dimension"), accessorKey: "dimension" as const }] : []),
    { id: "status", header: t("status"), sortable: true, cell: (_value, row) => row.status === "active" ? t("statusActive") : t("statusInactive") },
    { id: "actions", header: t("rowActions"), isAction: true, sticky: "right", cell: (_value, row) => canManage ? <Button type="button" size="sm" variant="outline" className="min-h-[44px]" onClick={() => startEdit(row)}>{t("edit")}</Button> : null },
  ], [canManage, codeLabel, kind, locale, nameLabel, t]);
  const createLabel = kind === "categories"
    ? t("createCategory")
    : kind === "brands"
      ? t("createBrand")
      : kind === "units"
        ? t("createUnit")
        : t("createTaxCategory");
  const editLabel = kind === "categories" ? t("editCategory") : kind === "brands" ? t("editBrand") : kind === "units" ? t("editUnit") : t("editTaxCategory");

  const exportColumns = useMemo<ExportColumn<ReferenceRow>[]>(() => [
    { header: codeLabel, accessor: (row) => row.code ?? "" },
    { header: nameLabel, accessor: (row) => locale === "en" ? row.name?.english ?? "" : row.name?.thai ?? "" },
    ...(kind === "units" ? [
      { header: t("unitSymbol"), accessor: (row: ReferenceRow) => row.symbol ?? "" },
      { header: t("dimension"), accessor: (row: ReferenceRow) => row.dimension ?? "" },
    ] : []),
    { header: t("status"), accessor: (row) => row.status === "active" ? t("statusActive") : t("statusInactive") },
  ], [codeLabel, kind, locale, nameLabel, t]);
  const exportData = useDataExport<ReferenceRow>({
    filename: `item-master-${kind}`,
    columns: exportColumns,
    data: filteredRows,
  });
  const activeFilters = useMemo<ActiveFilterChipItem[]>(() => {
    const status = list.params.filters.status;
    return status ? [{ key: "status", value: status, label: `${t("status")}: ${status === "active" ? t("statusActive") : t("statusInactive")}` }] : [];
  }, [list.params.filters.status, t]);

  const startCreate = () => {
    setEditing(null);
    form.reset({
      code: "", codeMode: "generated", name: { th: "", en: "" }, description: { th: "", en: "" }, parentCategoryId: "",
      allowedItemTypes: allItemTypes, sortOrder: 0, imageFile: null, symbol: "", dimension: "count", decimalScale: 0, roundingMode: "half_up",
    });
    setImageFileId(null);
    uploadedImageFileIdRef.current = null;
    imageUploadKeyRef.current = null;
    setOpen(true);
  };
  const startEdit = (row: ReferenceRow) => {
    setEditing(row);
    form.reset({
      code: row.code ?? "",
      codeMode: "manual",
      name: { th: row.name?.thai ?? "", en: row.name?.english ?? "" },
      description: { th: row.description?.thai ?? "", en: row.description?.english ?? "" },
      parentCategoryId: row.parentCategoryId ?? "",
      allowedItemTypes: row.allowedItemTypes ?? allItemTypes,
      sortOrder: row.sortOrder ?? 0,
      symbol: row.symbol ?? "",
      dimension: row.dimension ?? "count",
      decimalScale: row.decimalScale ?? 0,
      roundingMode: normalizeUnitRoundingMode(row.roundingMode),
      imageFile: null,
    });
    setImageFileId(row.imageFileId ?? null);
    uploadedImageFileIdRef.current = null;
    imageUploadKeyRef.current = null;
    setOpen(true);
  };
  const submit = form.handleSubmit(async (values) => {
    if (savingReferenceRef.current) return;
    savingReferenceRef.current = true;
    setIsSavingReference(true);

    const name = { thai: values.name.th, english: values.name.en?.trim() || null };
    const description = values.description.th?.trim() || values.description.en?.trim()
      ? { thai: values.description.th?.trim() ?? "", english: values.description.en?.trim() || null }
      : undefined;
    try {
      let savedCode = editing?.code ?? values.code;
      if (kind === "categories") {
        const payload = { code: !editing && values.codeMode === "generated" ? null : values.code, name, description, parentCategoryId: values.parentCategoryId || null, allowedItemTypes: values.allowedItemTypes, sortOrder: values.sortOrder };
        let target = editing;
        if (!target) {
          const created = await mutations.createCategory.mutateAsync(payload);
          if (!created.id || !created.rowVersion || !created.code) throw new Error("Created category response is incomplete.");
          target = { id: created.id, rowVersion: created.rowVersion, code: created.code, imageFileId: null };
          setEditing(target);
          savedCode = created.code;
          form.setValue("code", created.code, { shouldDirty: true });
        }
        if (!target.id || !target.rowVersion) throw new Error("Category update requires a current row version.");
        let resolvedImageFileId = imageFileId;
        if (values.imageFile) {
          if (!selectedMembership?.id) throw new MembershipRequiredError();
          const token = await getAuthToken();
          if (!token) throw new AuthenticationRequiredError();
          let fileId = uploadedImageFileIdRef.current;
          if (!fileId) {
            imageUploadKeyRef.current ??= crypto.randomUUID();
            fileId = await uploadVerifiedItemImage({ file: values.imageFile, itemId: target.id, parentType: "item-category", token, membershipId: selectedMembership.id, locale: locale === "en" ? "en" : "th", idempotencyKey: imageUploadKeyRef.current });
            uploadedImageFileIdRef.current = fileId;
          }
          resolvedImageFileId = fileId;
        }
        await mutations.updateCategory.mutateAsync({ id: target.id, rowVersion: target.rowVersion, payload: { ...payload, code: target.code ?? values.code, imageFileId: resolvedImageFileId } });
      } else if (kind === "brands") {
        const payload = { code: !editing && values.codeMode === "generated" ? null : values.code, name, description, sortOrder: values.sortOrder };
        let target = editing;
        if (!target) {
          const created = await mutations.createBrand.mutateAsync(payload);
          if (!created.id || !created.rowVersion || !created.code) throw new Error("Created brand response is incomplete.");
          target = { id: created.id, rowVersion: created.rowVersion, code: created.code, imageFileId: null };
          setEditing(target);
          savedCode = created.code;
          form.setValue("code", created.code, { shouldDirty: true });
        }
        if (!target.id || !target.rowVersion) throw new Error("Brand update requires a current row version.");
        let resolvedImageFileId = imageFileId;
        if (values.imageFile) {
          if (!selectedMembership?.id) throw new MembershipRequiredError();
          const token = await getAuthToken();
          if (!token) throw new AuthenticationRequiredError();
          let fileId = uploadedImageFileIdRef.current;
          if (!fileId) {
            imageUploadKeyRef.current ??= crypto.randomUUID();
            fileId = await uploadVerifiedItemImage({ file: values.imageFile, itemId: target.id, parentType: "item-brand", token, membershipId: selectedMembership.id, locale: locale === "en" ? "en" : "th", idempotencyKey: imageUploadKeyRef.current });
            uploadedImageFileIdRef.current = fileId;
          }
          resolvedImageFileId = fileId;
        }
        await mutations.updateBrand.mutateAsync({ id: target.id, rowVersion: target.rowVersion, payload: { ...payload, code: target.code ?? values.code, imageFileId: resolvedImageFileId } });
      } else if (kind === "units") {
        const payload = { code: !editing && values.codeMode === "generated" ? null : values.code, name, symbol: values.symbol, dimension: values.dimension, decimalScale: values.decimalScale, roundingMode: values.roundingMode };
        if (editing) {
          if (!editing.id || !editing.rowVersion) throw new Error("Unit update requires a current row version.");
        await mutations.updateUnit.mutateAsync({ id: editing.id, rowVersion: editing.rowVersion, payload: { ...payload, code: values.code } });
        } else savedCode = (await mutations.createUnit.mutateAsync(payload)).code ?? "";
      } else {
        const payload = { code: !editing && values.codeMode === "generated" ? null : values.code, name, sortOrder: values.sortOrder };
        if (editing) {
          if (!editing.id || !editing.rowVersion) throw new Error("Tax category update requires a current row version.");
        await mutations.updateTaxCategory.mutateAsync({ id: editing.id, rowVersion: editing.rowVersion, payload: { ...payload, code: values.code } });
        } else savedCode = (await mutations.createTaxCategory.mutateAsync(payload)).code ?? "";
      }
      setOpen(false);
      setEditing(null);
      toast.success(!editing && savedCode ? t("referenceSavedWithCode", { code: savedCode }) : t("referenceSaved"));
    } catch {
      toast.error(t("referenceSaveFailed"));
    } finally {
      savingReferenceRef.current = false;
      setIsSavingReference(false);
    }
  });

  const submitSharedConversion = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const nextErrors: ConversionErrors = {};
    if (!conversionFromUnitId) nextErrors.fromUnitId = "conversionRequired";
    if (!conversionToUnitId) nextErrors.toUnitId = "conversionRequired";
    if (conversionFromUnitId && conversionToUnitId && conversionFromUnitId === conversionToUnitId) {
      nextErrors.toUnitId = "conversionSameUnit";
    }

    const factorValue = Number(conversionFactor);
    const factorDecimalPlaces = conversionFactor.split(".")[1]?.length ?? 0;
    const factorFormatIsValid = /^(?:\d+(?:\.\d{1,6})?|\.\d{1,6})$/.test(conversionFactor);
    if (!factorFormatIsValid || !Number.isFinite(factorValue) || factorValue <= 0 || factorValue >= 1_000_000_000_000 || factorDecimalPlaces > 6) {
      nextErrors.factor = "conversionFactorInvalid";
    }
    if (!conversionEffectiveFrom) nextErrors.effectiveFrom = "conversionRequired";
    if (conversionEffectiveTo && conversionEffectiveFrom && conversionEffectiveTo < conversionEffectiveFrom) {
      nextErrors.effectiveTo = "conversionDateRangeInvalid";
    }
    if (!conversionReason.trim()) nextErrors.reason = "conversionRequired";
    setConversionErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) return;

    try {
      await sharedConversionMutations.create.mutateAsync({
        fromUnitId: conversionFromUnitId,
        toUnitId: conversionToUnitId,
        factor: conversionFactor,
        effectiveFrom: conversionEffectiveFrom,
        effectiveTo: conversionEffectiveTo || null,
        reason: conversionReason.trim(),
      });
      setConversionFactor("");
      setConversionReason("");
      setConversionErrors({});
      toast.success(t("conversionSaved"));
    } catch {
      toast.error(t("conversionSaveFailed"));
    }
  };

  const isDirty = form.formState.isDirty;
  const requestClose = () => {
    if (savingReference || imagePreparing) return;
    if (isDirty) setDiscardOpen(true);
    else setOpen(false);
  };

  return (
    <section className="space-y-5">
      <PageHeader title={t("referenceData")} subtitle={t("referenceDataHelp")} backHref={`/${locale}/item-master`} breadcrumbs={[{ label: t("title"), href: `/${locale}/item-master` }, { label: t("referenceData") }]} actions={canManage ? <Button type="button" onClick={startCreate}>{createLabel}</Button> : null} />
      <div className="flex flex-wrap gap-2" role="tablist" aria-label={t("referenceData")}>
        {(["categories", "brands", "units", "taxCategories"] as const).map((value) => <Button key={value} type="button" size="sm" variant={kind === value ? "primary" : "outline"} role="tab" aria-selected={kind === value} onClick={() => { setKind(value); list.actions.clearFilters(); }}>{t(value)}</Button>)}
      </div>
      <ListToolbar activeFilters={<ActiveFilterChips filters={activeFilters} onRemove={(key) => { if (key === "status") list.actions.setFilter("status", undefined); }} onClear={list.actions.clearFilters} />}>
        <div className="flex w-full flex-wrap items-end gap-3">
          <ListSearchInput id="reference-data-search" label={common("actions.search")} value={list.draftSearch} isDebouncing={list.isDebouncing} onChange={(value) => list.actions.setSearch(value)} onClear={() => list.actions.setSearch("", true)} onSubmit={(value) => list.actions.setSearch(value, true)} placeholder={t("searchReferenceData")} widthClassName="w-full max-w-xl" />
          <ListFilterSelect id="reference-data-status-filter" label={t("status")} value={list.params.filters.status ?? ""} onChange={(value) => list.actions.setFilter("status", value || undefined)} options={[{ value: "active", label: t("statusActive") }, { value: "inactive", label: t("statusInactive") }]} />
          <ExportDropdown label={t("exportReferenceData")} isLoading={exportData.isExporting} disabled={filteredRows.length === 0} onExport={async (format) => { try { await exportData.exportAll(format); } catch { toast.error(t("exportMaintenanceFailed")); } }} />
        </div>
      </ListToolbar>
      {data.isError && <p role="alert" className="border border-erp-danger p-4 text-erp-danger">{data.error.message}</p>}
      <DataTable<ReferenceRow> columns={columns} data={pageRows} pagination={{ page: list.params.page, limit: list.params.limit, totalPages: pageCount, total: filteredRows.length }} sorting={{ key: list.params.sort ?? "code", order: list.params.order }} onPageChange={list.actions.setPage} onLimitChange={(limit) => { if (isListPageSize(limit)) list.actions.setLimit(limit); }} onSort={list.actions.setSort} isLoading={data.isLoading} isError={data.isError} error={data.error} onRetry={() => { void data.refetch(); }} emptyTitle={list.params.search || list.params.filters.status ? t("noReferenceResults") : t("noReferenceRows")} emptyDescription={filteredRows.length === 0 && (list.params.search || list.params.filters.status) ? t("noReferenceResultsHelp") : t("noReferenceRowsHelp")} />
      {kind === "units" && <FormSection title={t("sharedUnitConversions")} description={t("sharedUnitConversionHelp")}>
        {sharedConversions.isError && <p role="alert" className="border border-erp-danger p-3 text-sm text-erp-danger">{t("conversionLoadFailed")}</p>}
        {canManage && <form noValidate onSubmit={(event) => { void submitSharedConversion(event); }} className="grid grid-cols-1 gap-x-5 gap-y-4 border-t border-erp-border pt-5 sm:grid-cols-2 xl:grid-cols-3">
          <Select id="shared-conversion-from" label={t("fromUnit")} value={conversionFromUnitId} onChange={(event) => { setConversionFromUnitId(event.target.value); setConversionErrors((errors) => ({ ...errors, fromUnitId: undefined, toUnitId: undefined })); }} options={(data.data?.units ?? []).filter(hasActiveUnitIdentity).map((unit) => ({ value: unit.id, label: `${unit.code} · ${locale === "en" ? unit.name?.english ?? "-" : unit.name?.thai ?? "-"}` }))} placeholder={t("selectUnit")} required disabled={savingConversion || data.isLoading} error={conversionErrors.fromUnitId ? t(conversionErrors.fromUnitId) : undefined} />
          <Select id="shared-conversion-to" label={t("toUnit")} value={conversionToUnitId} onChange={(event) => { setConversionToUnitId(event.target.value); setConversionErrors((errors) => ({ ...errors, toUnitId: undefined })); }} options={(data.data?.units ?? []).filter(hasActiveUnitIdentity).map((unit) => ({ value: unit.id, label: `${unit.code} · ${locale === "en" ? unit.name?.english ?? "-" : unit.name?.thai ?? "-"}` }))} placeholder={t("selectUnit")} required disabled={savingConversion || data.isLoading} error={conversionErrors.toUnitId ? t(conversionErrors.toUnitId) : undefined} />
          <Input label={t("conversionFactor")} type="number" min="0.000001" max="999999999999" step="0.000001" value={conversionFactor} onChange={(event) => { setConversionFactor(event.target.value); setConversionErrors((errors) => ({ ...errors, factor: undefined })); }} required disabled={savingConversion} error={conversionErrors.factor ? t(conversionErrors.factor) : undefined} />
          <Input label={t("effectiveFrom")} type="date" value={conversionEffectiveFrom} onChange={(event) => { setConversionEffectiveFrom(event.target.value); setConversionErrors((errors) => ({ ...errors, effectiveFrom: undefined, effectiveTo: undefined })); }} required disabled={savingConversion} error={conversionErrors.effectiveFrom ? t(conversionErrors.effectiveFrom) : undefined} />
          <Input label={t("effectiveTo")} type="date" value={conversionEffectiveTo} onChange={(event) => { setConversionEffectiveTo(event.target.value); setConversionErrors((errors) => ({ ...errors, effectiveTo: undefined })); }} disabled={savingConversion} error={conversionErrors.effectiveTo ? t(conversionErrors.effectiveTo) : undefined} />
          <Input wrapperClassName="sm:col-span-2 xl:col-span-3" label={t("reason")} value={conversionReason} onChange={(event) => { setConversionReason(event.target.value); setConversionErrors((errors) => ({ ...errors, reason: undefined })); }} required disabled={savingConversion} maxLength={500} error={conversionErrors.reason ? t(conversionErrors.reason) : undefined} />
          {sharedConversionMutations.create.isError && <p role="alert" className="sm:col-span-2 xl:col-span-3 border border-erp-danger p-3 text-sm text-erp-danger">{t("conversionSaveFailed")}</p>}
          <div className="flex justify-end border-t border-erp-border pt-4 sm:col-span-2 xl:col-span-3"><Button type="submit" disabled={savingConversion || !conversionFromUnitId || !conversionToUnitId || !conversionFactor || !conversionReason.trim()} isLoading={savingConversion}>{t("addConversion")}</Button></div>
        </form>}
        {sharedConversions.isLoading ? (
          <p className="py-3 text-sm text-erp-muted">{t("loading")}</p>
        ) : (sharedConversions.data?.length ?? 0) === 0 ? (
          <p className="py-3 text-sm text-erp-muted">{t("noUnitConversions")}</p>
        ) : (
          <Table wrapperClassName="min-w-[560px]">
            <TableHeader>
              <TableRow>
                <TableHead>{t("fromUnit")}</TableHead>
                <TableHead>{t("toUnit")}</TableHead>
                <TableHead>{t("conversionFactor")}</TableHead>
                <TableHead>{t("effectiveFrom")}</TableHead>
                <TableHead>{t("reason")}</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {sharedConversions.data?.map((conversion) => (
                <TableRow key={conversion.id}>
                  <TableCell>{conversion.fromUnitCode ?? "-"}</TableCell>
                  <TableCell>{conversion.toUnitCode ?? "-"}</TableCell>
                  <TableCell>{conversion.factor ?? "-"}</TableCell>
                  <TableCell>{conversion.effectiveFrom ?? "-"}</TableCell>
                  <TableCell>{conversion.reason ?? "-"}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </FormSection>}
      <Drawer
        isOpen={open}
        onClose={requestClose}
        closeDisabled={savingReference || imagePreparing || discardOpen}
        closeLabel={common("actions.close")}
        title={editing ? editLabel : createLabel}
        description={t("referenceFormHelp")}
        size="xl"
        footer={<div className="flex flex-wrap justify-end gap-3"><Button type="button" variant="outline" disabled={savingReference || imagePreparing} onClick={requestClose}>{common("actions.cancel")}</Button><Button type="submit" form="reference-data-form" isLoading={savingReference} disabled={!canManage || savingReference || imagePreparing}>{t("saveReference")}</Button></div>}
      >
        <form id="reference-data-form" onSubmit={(event) => { void submit(event); }} noValidate className="space-y-5">
          <FormSection title={nameLabel} description={t("thaiNameRequiredEnglishOptional")} className="p-4">
          <div className="grid gap-4 sm:grid-cols-2">
            {editing ? <Input label={codeLabel} required maxLength={kind === "units" ? 20 : 30} {...form.register("code")} error={form.formState.errors.code ? form.formState.errors.code.message === "CODE_LENGTH" ? t("codeLengthError") : t("validationRequired") : undefined} disabled={savingReference} /> : <GeneratedCodeField
              id={`reference-code-${kind}`}
              label={codeLabel}
              value={form.watch("code")}
              mode={form.watch("codeMode")}
              maxLength={kind === "units" ? 20 : 30}
              disabled={savingReference}
              error={form.formState.errors.code ? form.formState.errors.code.message === "CODE_LENGTH" ? t("codeLengthError") : t("codeRequired") : undefined}
              onChange={(value) => form.setValue("code", value, { shouldDirty: true, shouldValidate: true })}
              onModeChange={(mode) => {
                form.setValue("codeMode", mode, { shouldDirty: true, shouldValidate: true });
                if (mode === "generated") form.setValue("code", "", { shouldDirty: true, shouldValidate: true });
              }}
            />}
            {kind !== "units" && <Input label={t("sortOrder")} type="number" min="0" step="1" required {...form.register("sortOrder", { valueAsNumber: true })} error={form.formState.errors.sortOrder ? t("validationRequired") : undefined} disabled={savingReference} />}
          </div>
          <div className="space-y-1"><Controller name="name" control={form.control} render={({ field }) => <MultiLangInput id="reference-name" label={nameLabel} required disabled={savingReference} value={field.value} onChange={field.onChange} error={form.formState.errors.name ? t("validationRequired") : undefined} />} /></div>
          {(kind === "categories" || kind === "brands") && <Controller name="description" control={form.control} render={({ field }) => <MultiLangInput id="reference-description" label={t("description")} type="textarea" disabled={savingReference} value={field.value} onChange={field.onChange} />} />}
          </FormSection>
          {kind === "categories" && <FormSection title={t("categorySettings")} className="p-4" contentClassName="gap-4">
            <div className="erp-form-group"><label className="erp-label" htmlFor="category-parent">{t("parentCategory")}</label><select id="category-parent" className="erp-input" {...form.register("parentCategoryId")} disabled={savingReference}><option value="">{t("noParentCategory")}</option>{parentCategoryOptions.map((category) => <option key={category.id} value={category.id}>{category.code} · {locale === "en" ? category.name?.english ?? "-" : category.name?.thai ?? "-"}</option>)}</select></div>
            <fieldset className="space-y-2"><legend className="erp-label">{t("allowedItemTypes")}<span className="erp-label-required">*</span></legend><div className="grid grid-cols-1 gap-2 sm:grid-cols-2">{itemTypeOptions.map(([value, label]) => <label key={value} className="flex min-h-11 items-center gap-3 border border-erp-border px-3 text-sm text-erp-text-main"><input type="checkbox" value={value} disabled={savingReference} {...form.register("allowedItemTypes")} />{t(label)}</label>)}</div>{form.formState.errors.allowedItemTypes && <p role="alert" className="text-sm text-erp-danger">{t("allowedItemTypeRequired")}</p>}</fieldset>
          </FormSection>}
          {kind === "units" && <FormSection title={t("unitSettings")} className="p-4" contentClassName="grid gap-4 sm:grid-cols-2"><Input label={t("unitSymbol")} required {...form.register("symbol")} error={form.formState.errors.symbol ? t("validationRequired") : undefined} disabled={savingReference} /><Select label={t("dimension")} required placeholder={t("selectDimension")} options={dimensionOptions} {...form.register("dimension")} error={form.formState.errors.dimension ? t("validationRequired") : undefined} disabled={savingReference} /><Input label={t("decimalScale")} type="number" min="0" max="6" step="1" required {...form.register("decimalScale", { valueAsNumber: true })} error={form.formState.errors.decimalScale ? t("validationRequired") : undefined} disabled={savingReference} /><Select label={t("roundingMode")} required options={[...unitRoundingModes.map(([value, messageKey]) => ({ value, label: t(messageKey) })), ...(unitRoundingModes.some(([value]) => value === form.getValues("roundingMode")) ? [] : [{ value: form.getValues("roundingMode"), label: t("roundingModeUnsupported", { mode: form.getValues("roundingMode") }) }])]} helperText={t("roundingModeHelp")} {...form.register("roundingMode")} error={form.formState.errors.roundingMode ? t("roundingModeRequired") : undefined} disabled={savingReference} /></FormSection>}
          {(kind === "categories" || kind === "brands") && <FormSection title={t("images")} description={t("imageDeferredHelp")} className="p-4">
            <Controller name="imageFile" control={form.control} render={({ field, fieldState }) => <ImageUpload
              label={t("imageFile")}
              value={field.value ?? existingImageUrl ?? undefined}
              maxFileSizeBytes={ITEM_IMAGE_MAX_BYTES}
              disabled={savingReference}
              error={fieldState.error ? t("imageSizeInvalid") : undefined}
              onProcessingChange={setImagePreparing}
              onChange={(value) => {
                const nextFile = value instanceof File ? value : null;
                field.onChange(nextFile);
                uploadedImageFileIdRef.current = null;
                imageUploadKeyRef.current = null;
                if (!nextFile) setImageFileId(null);
              }}
            />} />
          </FormSection>}
        </form>
      </Drawer>
      <ConfirmationModal isOpen={discardOpen} onClose={() => setDiscardOpen(false)} onConfirm={() => { setDiscardOpen(false); setOpen(false); }} title={common("dialog.confirmCancelTitle")} message={common("dialog.confirmCancelDesc")} confirmText={common("actions.discard")} cancelText={common("actions.cancel")} variant="warning" isLoading={savingReference} />
    </section>
  );
}
