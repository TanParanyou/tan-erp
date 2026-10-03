"use client";

import { useEffect, useRef, useState } from "react";
import { Controller, useFieldArray, useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useLocale, useTranslations } from "next-intl";
import { useRouter } from "next/navigation";
import { cn } from "@/lib/utils/cn";
import { PageHeader } from "@/components/layout/PageHeader";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { MultiLangInput } from "@/components/forms/MultiLangInput";
import { GeneratedCodeField } from "@/components/forms/GeneratedCodeField";
import { ImageUpload } from "@/components/forms/ImageUpload";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { FormSection } from "@/components/forms/FormSection";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Alert } from "@/components/ui/Alert";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { can } from "@/lib/permissions/can";
import { useItemDetail, useItemMasterLookups, useItemMasterMutations } from "@/features/item-master/api/item-master-queries";
import { itemFormSchema, type ItemFormValues } from "@/features/item-master/schemas/item-form-schema";
import { useToast } from "@/hooks/useToast";
import { CostRecordMaintenance } from "@/features/item-master/components/cost-record-maintenance";
import { ItemBarcodeMaintenance } from "@/features/item-master/components/item-barcode-maintenance";
import { FormTabs, useFormTabErrors } from "@/components/forms/FormTabs";
import { BrandAutocomplete } from "@/features/item-master/components/brand-autocomplete";
import { CategoryAutocomplete } from "@/features/item-master/components/category-autocomplete";
import type { ItemBrandResponse, ItemCategoryResponse } from "@/lib/api/api-client";
import { useOrganizationBranches } from "@/features/item-master/api/item-master-queries";
import { ItemAliasMaintenance } from "@/features/item-master/components/item-alias-maintenance";
import { ItemImageMaintenance } from "@/features/item-master/components/item-image-maintenance";
import { uploadVerifiedItemImage } from "@/features/item-master/api/upload-item-image";
import { ITEM_IMAGE_MAX_BYTES } from "@/features/item-master/item-image-constants";

type ItemFormTab = "identity" | "details" | "operations";

interface ItemEditorProps {
  id: string;
}

export function ItemEditor({ id }: ItemEditorProps) {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const [deactivationOpen, setDeactivationOpen] = useState(false);
  const [createdItemId, setCreatedItemId] = useState<string | null>(null);
  const createdItemIdRef = useRef<string | null>(null);
  const verifiedImageFileIdRef = useRef<string | null>(null);
  const imageUploadKeyRef = useRef<string | null>(null);
  const submitInFlightRef = useRef(false);
  const [imageUploadFailed, setImageUploadFailed] = useState(false);
  const [imagePreparing, setImagePreparing] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<ItemCategoryResponse | null>(null);
  const [selectedBrand, setSelectedBrand] = useState<ItemBrandResponse | null>(null);
  const { selectedMembership } = useSelectedMembership();
  const isCreate = id === "create" || id === "add";
  const item = useItemDetail(isCreate ? undefined : id);
  const lookups = useItemMasterLookups();
  const branches = useOrganizationBranches();
  const mutations = useItemMasterMutations();
  const [activeTab, setActiveTab] = useState<ItemFormTab>("identity");
  const canCreate = can(selectedMembership, "items.create");
  const canUpdate = can(selectedMembership, "items.update");
  const canManageImages = can(selectedMembership, "items.manage-images");
  const form = useForm<ItemFormValues>({
    resolver: zodResolver(itemFormSchema),
    defaultValues: {
      code: "", codeMode: "generated", itemType: "material", categoryId: "", brandId: "", baseUnitId: "", taxCategoryCode: "",
      name: { th: "", en: "" }, description: { th: "", en: "" },
      availabilityMode: "all_branches", selectedBranchIds: [],
      capabilities: { canSell: false, canCost: true, canPurchase: true, canStock: false, canProduce: false },
      attributesJson: "", aliases: [], imageFile: null, imageAltText: "",
    },
  });
  const selectedItemType = useWatch({ control: form.control, name: "itemType" });
  const aliases = useFieldArray({ control: form.control, name: "aliases" });
  const selectedImageFile = useWatch({ control: form.control, name: "imageFile" });
  const tabFieldsMap: Record<ItemFormTab, string[]> = {
    identity: ["code", "codeMode", "itemType", "categoryId", "brandId", "baseUnitId", "name"],
    details: ["description", "aliases", "imageFile", "imageAltText"],
    operations: ["taxCategoryCode", "availabilityMode", "selectedBranchIds", "capabilities", "attributesJson"],
  };
  const tabErrors = useFormTabErrors<ItemFormTab, ItemFormValues>({ tabFieldsMap, errors: form.formState.errors, setActiveTab });

  useEffect(() => {
    if (!isCreate && item.data) {
      setSelectedCategory(item.data.category ?? null);
      setSelectedBrand(item.data.brand ?? null);
      form.reset({
        code: item.data.code ?? "",
        codeMode: "manual",
        itemType: item.data.itemType === "labor" || item.data.itemType === "material" || item.data.itemType === "other" || item.data.itemType === "product" || item.data.itemType === "service" || item.data.itemType === "subcontract" ? item.data.itemType : "material",
        categoryId: item.data.category?.id ?? "",
        brandId: item.data.brand?.id ?? "",
        baseUnitId: item.data.baseUnit?.id ?? "",
        taxCategoryCode: item.data.taxCategoryCode ?? "",
        name: { th: item.data.name?.thai ?? "", en: item.data.name?.english ?? "" },
        description: { th: item.data.description?.thai ?? "", en: item.data.description?.english ?? "" },
        availabilityMode: item.data.availabilityMode === "selected_branches" ? "selected_branches" : "all_branches",
        selectedBranchIds: item.data.branchAvailabilities?.filter((branch) => branch.status === "active").map((branch) => branch.branchId ?? "") ?? [],
        capabilities: {
          canSell: item.data.capabilities?.canSell ?? false,
          canCost: item.data.capabilities?.canCost ?? false,
          canPurchase: item.data.capabilities?.canPurchase ?? false,
          canStock: item.data.capabilities?.canStock ?? false,
          canProduce: item.data.capabilities?.canProduce ?? false,
        },
        attributesJson: item.data.attributes ? JSON.stringify(item.data.attributes, null, 2) : "",
        aliases: [], imageFile: null, imageAltText: "",
      });
    }
  }, [form, isCreate, item.data]);

  const onSubmit = form.handleSubmit(async (values) => {
    if (submitInFlightRef.current) return;
    submitInFlightRef.current = true;
    const name = { thai: values.name.th, english: values.name.en?.trim() || null };
    const description = values.description.th?.trim() || values.description.en?.trim()
      ? { thai: values.description.th?.trim() ?? "", english: values.description.en?.trim() || null }
      : undefined;
    let attributes: Record<string, string> | undefined;
    if (values.attributesJson.trim()) {
      const parsed: unknown = JSON.parse(values.attributesJson);
      if (typeof parsed === "object" && parsed !== null && !Array.isArray(parsed)
        && Object.values(parsed).every((value) => typeof value === "string")) {
        attributes = parsed as Record<string, string>;
      }
    }
    const shared = {
      code: isCreate && values.codeMode === "generated" ? null : values.code,
      itemType: values.itemType,
      categoryId: values.categoryId,
      brandId: values.brandId || null,
      name,
      description,
      baseUnitId: values.baseUnitId,
      taxCategoryCode: values.taxCategoryCode?.trim() || null,
      availabilityMode: values.availabilityMode,
      selectedBranchIds: values.availabilityMode === "selected_branches" ? values.selectedBranchIds : [],
      capabilities: values.capabilities,
      attributes: attributes ?? null,
      attributesSchemaVersion: 1,
    };
    try {
      if (isCreate) {
        if (!canCreate) return;
        let itemId = createdItemIdRef.current;
        if (!itemId) {
          const created = await mutations.createItem.mutateAsync({ ...shared, selectedBranchIds: values.availabilityMode === "selected_branches" ? values.selectedBranchIds : null, aliases: values.aliases.map((alias) => ({ thai: alias.th, english: alias.en?.trim() || null })) });
          if (!created.id) throw new Error("Created item did not return an identifier.");
          itemId = created.id;
          createdItemIdRef.current = itemId;
          setCreatedItemId(itemId);
        }
        if (!itemId) throw new Error("Created item identifier is missing.");
        if (values.imageFile && canManageImages) {
          try {
            if (!selectedMembership?.id) throw new MembershipRequiredError();
            const token = await getAuthToken();
            if (!token) throw new AuthenticationRequiredError();
            let fileId = verifiedImageFileIdRef.current;
            if (!fileId) {
              const idempotencyKey = imageUploadKeyRef.current ?? crypto.randomUUID();
              imageUploadKeyRef.current = idempotencyKey;
              fileId = await uploadVerifiedItemImage({ file: values.imageFile, itemId, token, membershipId: selectedMembership.id, locale: locale === "en" ? "en" : "th", idempotencyKey });
              verifiedImageFileIdRef.current = fileId;
            }
            await mutations.attachItemImage.mutateAsync({ id: itemId, payload: { fileId, role: "gallery", isPrimary: true, altText: { thai: values.imageAltText.trim(), english: values.name.en?.trim() || null } } });
            setImageUploadFailed(false);
          } catch {
            setImageUploadFailed(true);
            toast.error(t("imageSaveFailed"));
            return;
          }
        }
        router.replace(`/${locale}/item-master/${itemId}`);
      } else {
        if (!canUpdate || !item.data?.rowVersion) return;
        await mutations.updateItem.mutateAsync({ id, rowVersion: item.data.rowVersion, payload: { ...shared, code: values.code } });
      }
    } catch {
      // The API error remains available on the mutation for the form-level message.
    } finally {
      submitInFlightRef.current = false;
    }
  }, tabErrors.handleFormError);

  if (!isCreate && item.isLoading) return <section><MonoSpinner label={t("loading")} /></section>;
  if (!isCreate && item.isError) return <section><p role="alert" className="border border-erp-danger p-4 text-erp-danger">{item.error.message}</p><Button type="button" variant="outline" onClick={() => { void item.refetch(); }}>{common("actions.retry")}</Button></section>;
  if (lookups.isLoading) return <section><MonoSpinner label={t("loading")} /></section>;
  const saving = form.formState.isSubmitting || mutations.createItem.isPending || mutations.updateItem.isPending || mutations.attachItemImage.isPending || mutations.setBranchAvailability.isPending;
  const fieldsLocked = saving || Boolean(createdItemId);
  const canSave = isCreate ? canCreate : canUpdate;
  const canActivate = can(selectedMembership, "items.activate");
  const canDeactivate = can(selectedMembership, "items.deactivate");
  const statusChanging = mutations.activateItem.isPending || mutations.deactivateItem.isPending;
  const changeStatus = async (nextStatus: "active" | "inactive") => {
    if (!item.data?.rowVersion) return;
    try {
      if (nextStatus === "active") {
        await mutations.activateItem.mutateAsync({ id, rowVersion: item.data.rowVersion });
      } else {
        await mutations.deactivateItem.mutateAsync({ id, rowVersion: item.data.rowVersion, reasonCode: "master_data_maintenance", reason: t("deactivationReason") });
      }
      setDeactivationOpen(false);
      toast.success(nextStatus === "active" ? t("itemActivated") : t("itemDeactivated"));
    } catch {
      toast.error(t("versionConflict"));
    }
  };
  const activeUnits = (lookups.data?.units ?? []).filter((unit) => unit.status === "active");
  const activeTaxCategories = (lookups.data?.taxCategories ?? []).filter((taxCategory) => taxCategory.status === "active");

  return (
    <section className="space-y-5 pb-40 sm:pb-28">
      <PageHeader title={isCreate ? t("createTitle") : t("editTitle")} subtitle={t("editorHelp")} backHref={`/${locale}/item-master`} breadcrumbs={[{ label: t("title"), href: `/${locale}/item-master` }, { label: isCreate ? t("createTitle") : (item.data?.code ?? t("editTitle")) }]} actions={!isCreate && item.data?.status !== "active" && canActivate ? <Button type="button" disabled={statusChanging || !item.data?.rowVersion} isLoading={mutations.activateItem.isPending} onClick={() => { void changeStatus("active"); }}>{t("activate")}</Button> : !isCreate && item.data?.status === "active" && canDeactivate ? <Button type="button" variant="danger" disabled={statusChanging || !item.data?.rowVersion} onClick={() => setDeactivationOpen(true)}>{t("deactivate")}</Button> : null} />
      {lookups.isError && <p role="alert" className="border border-erp-danger p-4 text-erp-danger">{lookups.error.message}</p>}
      <form onSubmit={(event) => { void onSubmit(event); }} noValidate className="mx-auto max-w-6xl space-y-5">
        <FormTabs ariaLabel={t("formSections")} activeTab={activeTab} onChange={setActiveTab} tabs={[{ id: "identity", label: t("identityTab"), hasError: tabErrors.tabErrorMap.identity }, { id: "details", label: t("detailsTab"), hasError: tabErrors.tabErrorMap.details }, { id: "operations", label: t("operationsTab"), hasError: tabErrors.tabErrorMap.operations }]} />
        <div role="tabpanel" id="tabpanel-identity" aria-labelledby="tab-identity" hidden={activeTab !== "identity"} className={cn("grid grid-cols-1 gap-5 lg:grid-cols-[minmax(0,2fr)_minmax(0,1fr)]", activeTab !== "identity" && "hidden")}>
        <FormSection title={t("identityTab")} description={t("identityHelp")}>
        <Controller control={form.control} name="name" render={({ field, fieldState }) => <MultiLangInput label={t("name")} required disabled={!canSave || fieldsLocked} value={field.value} onChange={field.onChange} error={fieldState.error ? t("validationRequired") : undefined} />} />
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Controller control={form.control} name="itemType" render={({ field, fieldState }) => (
            <div className="erp-form-group"><label className="erp-label" htmlFor="item-type">{t("type")}<span className="erp-label-required">*</span></label><select {...field} onChange={(event) => { const itemType = event.target.value; field.onChange(itemType); if (isCreate && !form.formState.dirtyFields.capabilities?.canPurchase) form.setValue("capabilities.canPurchase", itemType === "material" || itemType === "subcontract", { shouldDirty: false }); }} id="item-type" required className="erp-input" disabled={!canSave || fieldsLocked} aria-invalid={Boolean(fieldState.error)} aria-describedby={fieldState.error ? "item-type-error" : undefined}>
              <option value="material">{t("itemTypeMaterial")}</option><option value="labor">{t("itemTypeLabor")}</option><option value="service">{t("itemTypeService")}</option><option value="subcontract">{t("itemTypeSubcontract")}</option><option value="other">{t("itemTypeOther")}</option><option value="product">{t("itemTypeProduct")}</option>
            </select>{fieldState.error && <span id="item-type-error" role="alert" className="text-sm text-erp-danger">{t("validationRequired")}</span>}</div>
          )} />
          <Controller control={form.control} name="categoryId" render={({ field, fieldState }) => (
            <CategoryAutocomplete required value={field.value} onChange={field.onChange} selectedOption={selectedCategory} onSelectedOptionChange={setSelectedCategory} itemType={selectedItemType} error={fieldState.error ? t("validationRequired") : undefined} disabled={!canSave || fieldsLocked} />
          )} />
          <Controller control={form.control} name="brandId" render={({ field }) => (
            <BrandAutocomplete value={field.value} onChange={field.onChange} selectedOption={selectedBrand} onSelectedOptionChange={setSelectedBrand} disabled={!canSave || fieldsLocked} />
          )} />
          <Controller control={form.control} name="baseUnitId" render={({ field, fieldState }) => (
            <div className="erp-form-group"><label className="erp-label" htmlFor="item-unit">{t("unit")}<span className="erp-label-required">*</span></label><select {...field} id="item-unit" required className="erp-input" disabled={!canSave || fieldsLocked} aria-invalid={Boolean(fieldState.error)} aria-describedby={fieldState.error ? "item-unit-error" : undefined}><option value="">{common("actions.select")}</option>{activeUnits.map((unit) => <option key={unit.id} value={unit.id}>{unit.code} · {locale === "en" ? unit.name?.english ?? "-" : unit.name?.thai ?? "-"}</option>)}</select>{fieldState.error && <span id="item-unit-error" role="alert" className="text-sm text-erp-danger">{t("validationRequired")}</span>}</div>
          )} />
        </div>

        </FormSection>
        <FormSection title={t("itemCodeSection")} description={t("itemCodeHelp")} className="self-start border-t-2 border-t-erp-navy">
          <Controller control={form.control} name="code" render={({ field, fieldState }) => isCreate ? <GeneratedCodeField
            id="item-code"
            label={t("code")}
            value={field.value}
            mode={form.watch("codeMode")}
            maxLength={50}
            disabled={!canSave || fieldsLocked}
            error={fieldState.error ? fieldState.error.message === "CODE_REQUIRED" ? t("codeRequired") : t("codeLengthError") : undefined}
            onChange={field.onChange}
            onModeChange={(mode) => {
              form.setValue("codeMode", mode, { shouldDirty: true, shouldValidate: true });
              if (mode === "generated") field.onChange("");
            }}
          /> : <Input {...field} label={t("code")} required disabled={!canSave || fieldsLocked || item.data?.activatedOnce === true} helperText={item.data?.activatedOnce === true ? t("codeImmutable") : undefined} error={fieldState.error ? t("validationRequired") : undefined} />} />

        </FormSection>
        </div>
        <div role="tabpanel" id="tabpanel-details" aria-labelledby="tab-details" hidden={activeTab !== "details"} className="space-y-5">
        <FormSection title={t("detailsTab")} description={t("detailsHelp")}>

        <Controller control={form.control} name="description" render={({ field }) => <MultiLangInput label={t("description")} type="textarea" disabled={!canSave || fieldsLocked} value={field.value} onChange={field.onChange} />} />
        </FormSection>
        {isCreate && <FormSection title={t("aliases")} description={t("aliasesHelp")}>
          {aliases.fields.map((alias, index) => <div key={alias.id} className="flex items-start gap-2"><div className="min-w-0 flex-1"><Controller control={form.control} name={`aliases.${index}`} render={({ field }) => <MultiLangInput label={`${t("alias")} ${index + 1}`} disabled={!canSave || fieldsLocked} value={field.value} onChange={field.onChange} error={form.formState.errors.aliases?.[index] ? t("validationRequired") : undefined} />} /></div><Button type="button" variant="outline" disabled={!canSave || fieldsLocked} onClick={() => aliases.remove(index)}>{t("removeAlias")}</Button></div>)}
          <Button type="button" variant="outline" disabled={!canSave || fieldsLocked} onClick={() => aliases.append({ th: "", en: "" })}>{t("addAlias")}</Button>
        </FormSection>}
        {isCreate && canManageImages && <FormSection title={t("images")} description={t("imageCreateHelp")}>
          <Controller control={form.control} name="imageFile" render={({ field, fieldState }) => <ImageUpload
            label={t("imageFile")}
            value={field.value ?? undefined}
            onChange={(value) => {
              if (value !== null && !(value instanceof File)) return;
              field.onChange(value);
              if (value === null) form.setValue("imageAltText", "");
              verifiedImageFileIdRef.current = null;
              imageUploadKeyRef.current = null;
              setImageUploadFailed(false);
            }}
            maxFileSizeBytes={ITEM_IMAGE_MAX_BYTES}
            disabled={!canSave || fieldsLocked}
            onProcessingChange={setImagePreparing}
            error={fieldState.error ? t("imageSizeInvalid") : undefined}
          />} />
          <Controller control={form.control} name="imageAltText" render={({ field, fieldState }) => <Input {...field}
            label={t("imageAltText")}
            required={Boolean(selectedImageFile)}
            disabled={!canSave || fieldsLocked || !selectedImageFile}
            error={fieldState.error?.message === "IMAGE_ALT_REQUIRED" ? t("imageAltRequired") : fieldState.error ? t("imageAltTooLong") : undefined}
          />} />
          {imagePreparing && <p role="status" className="text-sm text-erp-text-muted">{t("imagePreparing")}</p>}
        </FormSection>}
        </div>
        <div role="tabpanel" id="tabpanel-operations" aria-labelledby="tab-operations" hidden={activeTab !== "operations"} className="space-y-5">
          <FormSection title={t("operationsTab")} description={t("operationsHelp")}>
          <div className="grid gap-4 sm:grid-cols-2">
          <Controller control={form.control} name="taxCategoryCode" render={({ field, fieldState }) => <div className="erp-form-group"><label className="erp-label" htmlFor="item-tax-category">{t("taxCategoryCode")}</label><select {...field} value={field.value ?? ""} id="item-tax-category" className="erp-input" disabled={!canSave || fieldsLocked} aria-invalid={Boolean(fieldState.error)} aria-describedby={fieldState.error ? "item-tax-category-error" : undefined}><option value="">{common("actions.select")}</option>{field.value && !activeTaxCategories.some((taxCategory) => taxCategory.code === field.value) && <option value={field.value ?? ""} disabled>{field.value} · {t("statusInactive")}</option>}{activeTaxCategories.map((taxCategory) => <option key={taxCategory.id} value={taxCategory.code ?? ""}>{taxCategory.code ?? "-"} · {locale === "en" ? taxCategory.name?.english ?? "-" : taxCategory.name?.thai ?? "-"}</option>)}</select>{fieldState.error && <span id="item-tax-category-error" role="alert" className="text-sm text-erp-danger">{t("validationRequired")}</span>}</div>} />
          <Controller control={form.control} name="availabilityMode" render={({ field }) => <div className="erp-form-group"><label className="erp-label" htmlFor="availability-mode">{t("availability")}</label><select {...field} id="availability-mode" className="erp-input" disabled={!canSave || fieldsLocked}><option value="all_branches">{t("allBranches")}</option><option value="selected_branches">{t("selectedBranches")}</option></select></div>} />
          </div>
          {form.watch("availabilityMode") === "selected_branches" && <fieldset className="space-y-2"><legend className="erp-label">{t("branches")}</legend>{(branches.data ?? []).map((branch) => <label key={branch.id} className="flex min-h-11 items-center gap-2"><input type="checkbox" value={branch.id} disabled={!canSave || fieldsLocked} checked={form.watch("selectedBranchIds").includes(branch.id)} onChange={(event) => { const next = new Set(form.getValues("selectedBranchIds")); if (event.target.checked) next.add(branch.id); else next.delete(branch.id); form.setValue("selectedBranchIds", [...next], { shouldValidate: true, shouldDirty: true }); }} />{branch.code} · {branch.name}</label>)}{form.formState.errors.selectedBranchIds && <p className="text-sm text-erp-danger">{t("branchesRequired")}</p>}</fieldset>}
          <fieldset className="grid grid-cols-1 gap-2 sm:grid-cols-2"><legend className="erp-label mb-2">{t("capabilities")}</legend>{([ ["canSell", "canSell"], ["canCost", "canCost"], ["canPurchase", "canPurchase"], ["canStock", "canStock"], ["canProduce", "canProduce"] ] as const).map(([key, label]) => <label key={key} className="flex min-h-11 items-center gap-3 border border-erp-border bg-erp-surface px-3 has-[:checked]:border-erp-navy has-[:checked]:bg-erp-surface-subtle"><input type="checkbox" disabled={!canSave || fieldsLocked} {...form.register(`capabilities.${key}`)} />{t(label)}</label>)}</fieldset>
          </FormSection>
          <FormSection title={t("attributes")} description={t("attributesHelp")}>
          <Controller control={form.control} name="attributesJson" render={({ field, fieldState }) => <div className="erp-form-group"><label className="erp-label" htmlFor="item-attributes">{t("attributes")}</label><textarea {...field} id="item-attributes" rows={6} className="erp-input font-mono" disabled={!canSave || fieldsLocked} aria-invalid={Boolean(fieldState.error)} aria-describedby={fieldState.error ? "item-attributes-error" : undefined} placeholder={t("attributesPlaceholder")} />{fieldState.error && <span id="item-attributes-error" role="alert" className="text-sm text-erp-danger">{t("attributesInvalid")}</span>}</div>} />
          </FormSection>
          {branches.isError && <p role="alert" className="text-sm text-erp-danger">{t("branchesLoadFailed")}</p>}
        </div>
        {(mutations.createItem.isError || mutations.updateItem.isError) && <p role="alert" className="border border-erp-danger p-3 text-sm text-erp-danger">{t("createFailed")}</p>}
        {imageUploadFailed && <Alert variant="warning" title={t("imageSaveFailed")}>{t("imagePendingRetry")}</Alert>}
        {mutations.setBranchAvailability.isError && <p role="alert" className="border border-erp-danger p-3 text-sm text-erp-danger">{t("branchSaveFailed")}</p>}
        <FormActionBar
          isDirty={form.formState.isDirty}
          isLoading={saving}
          isSaveDisabled={!canSave || imagePreparing}
          saveText={imageUploadFailed ? t("retryImageUpload") : t("save")}
          onCancel={() => router.push(createdItemId ? `/${locale}/item-master/${createdItemId}` : `/${locale}/item-master`)}
          actions={!canSave ? <Button type="button" variant="outline" onClick={() => router.push(`/${locale}/item-master`)}>{common("actions.cancel")}</Button> : undefined}
        />
      </form>
      {!isCreate && item.data && <ItemBarcodeMaintenance itemId={id} item={item.data} />}
      {!isCreate && item.data && <ItemAliasMaintenance itemId={id} item={item.data} />}
      {!isCreate && item.data && <ItemImageMaintenance itemId={id} item={item.data} />}
      {!isCreate && item.data && can(selectedMembership, "cost-records.read") && <CostRecordMaintenance itemId={id} />}
      <ConfirmationModal isOpen={deactivationOpen} onClose={() => { if (!mutations.deactivateItem.isPending) setDeactivationOpen(false); }} onConfirm={() => changeStatus("inactive")} title={t("deactivateTitle")} message={t("deactivateMessage")} confirmText={t("deactivate")} cancelText={common("actions.cancel")} isLoading={mutations.deactivateItem.isPending} />
    </section>
  );
}
