"use client";

import React, { useState, useEffect, useMemo, useCallback } from "react";
import { useTranslations } from "next-intl";
import { FormTabs, type FormTabItem } from "@/components/forms/FormTabs";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { Switch } from "@/components/ui/Switch";
import { Button } from "@/components/ui/Button";
import { Modal } from "@/components/ui/Modal";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Alert } from "@/components/ui/Alert";
import {
  IconSave,
  IconPlus,
  IconTrash,
  IconPrinter,
  IconRotateCcw,
  IconImage,
  IconMaximize,
  IconEye,
} from "@/components/common/Icons";
import { A4DocumentPreview } from "./a4-document-preview";
import {
  useDocumentTemplates,
  useCreateDocumentTemplate,
  useUpdateDocumentTemplate,
  useDeleteDocumentTemplate,
} from "../api/document-template-queries";
import type {
  DocumentTemplateItem,
  DocumentTemplateLayoutConfig,
} from "../types";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

export type StudioTabId = "branding" | "columns" | "terms" | "signatures";

const DOCUMENT_TYPE_OPTIONS = [
  { value: "quotation", label: "ใบเสนอราคา (Quotation)" },
  { value: "estimate", label: "ใบประมาณราคา BOQ (Estimate)" },
  { value: "purchase_order", label: "ใบสั่งซื้อ (Purchase Order)" },
  { value: "billing", label: "ใบวางบิล / แจ้งหนี้ (Billing / Invoice)" },
  { value: "goods_receipt", label: "ใบรับสินค้า (Goods Receipt)" },
];

const LOGO_POSITION_OPTIONS = [
  { value: "left", label: "ชิดซ้าย (Left)" },
  { value: "right", label: "ชิดขวา (Right)" },
];

const LOGO_HEIGHT_OPTIONS = [
  { value: "40px", label: "เล็ก (40px)" },
  { value: "48px", label: "ปานกลาง (48px)" },
  { value: "60px", label: "ใหญ่ (60px)" },
  { value: "72px", label: "ใหญ่พิเศษ (72px)" },
];

const SIGNATURE_COUNT_OPTIONS = [
  { value: "2", label: "2 ช่อง (ผู้สั่งซื้อ / พนักงานขาย)" },
  { value: "3", label: "3 ช่อง (ผู้สั่งซื้อ / พนักงานขาย / ผู้อนุมัติ)" },
];

export function DocumentTemplateStudio() {
  const t = useTranslations("documentTemplates");
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  const [activeDocType, setActiveDocType] = useState<string>("quotation");
  const [activeTab, setActiveTab] = useState<StudioTabId>("branding");
  const [selectedTemplateId, setSelectedTemplateId] = useState<string>("");
  const [templateName, setTemplateName] = useState<string>("");
  const [isDefault, setIsDefault] = useState<boolean>(false);
  const [layout, setLayout] = useState<DocumentTemplateLayoutConfig | null>(null);

  // Preview Scale & Fullscreen states
  const [previewZoom, setPreviewZoom] = useState<"fit" | "100%">("fit");
  const [isFullscreenPreviewOpen, setIsFullscreenPreviewOpen] = useState<boolean>(false);

  // Status & modal states
  const [saveSuccessMessage, setSaveSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState<boolean>(false);
  const [newTemplateName, setNewTemplateName] = useState<string>("");
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState<boolean>(false);

  // Logo file deferred upload
  const [selectedLogoFile, setSelectedLogoFile] = useState<File | null>(null);
  const {
    uploadSingleFile,
    isUploading: isLogoUploading,
    resetIntent: resetFileUploadIntent,
  } = useDeferredFileUpload({
    parentType: "item",
  });
  const [logoPreviewUrl, setLogoPreviewUrl] = useState<string | null>(null);

  // Queries & Mutations
  const {
    data: templates,
    isLoading: isLoadingTemplates,
    refetch: refetchTemplates,
  } = useDocumentTemplates(activeDocType);

  const createMutation = useCreateDocumentTemplate();
  const updateMutation = useUpdateDocumentTemplate();
  const deleteMutation = useDeleteDocumentTemplate();

  // Find currently selected template
  const currentTemplate = useMemo(() => {
    if (!templates || templates.length === 0) return null;
    return templates.find((t) => t.id === selectedTemplateId) ?? templates[0];
  }, [templates, selectedTemplateId]);

  // Sync state when current template changes
  useEffect(() => {
    if (currentTemplate) {
      setSelectedTemplateId(currentTemplate.id);
      setTemplateName(currentTemplate.name);
      setIsDefault(currentTemplate.isDefault);
      try {
        const parsed = JSON.parse(currentTemplate.layoutConfigJson) as DocumentTemplateLayoutConfig;
        setLayout(parsed);
      } catch {
        setLayout(null);
      }
      setSaveSuccessMessage(null);
      setErrorMessage(null);
    }
  }, [currentTemplate]);

  // Handle Logo local selection for instant preview
  const handleLogoFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setSelectedLogoFile(file);
      const objectUrl = URL.createObjectURL(file);
      setLogoPreviewUrl(objectUrl);
    }
  };

  const handleRemoveLogo = () => {
    setSelectedLogoFile(null);
    resetFileUploadIntent();
    setLogoPreviewUrl(null);
  };

  // Switch document type
  const handleDocTypeChange = (newType: string) => {
    setActiveDocType(newType);
    setSelectedTemplateId("");
    setSaveSuccessMessage(null);
    setErrorMessage(null);
  };

  // Switch template
  const handleTemplateSelect = (templateId: string) => {
    setSelectedTemplateId(templateId);
  };

  // Save current template changes
  const handleSave = async () => {
    if (!currentTemplate || !layout) return;
    setErrorMessage(null);
    setSaveSuccessMessage(null);

    try {
      let logoFileId = currentTemplate.logoFileId;

      // Deferred file upload if logo was selected
      if (selectedLogoFile) {
        const token = await getAuthToken();
        if (token && membershipId) {
          const uploadResult = await uploadSingleFile(selectedLogoFile, {
            token,
            membershipId,
          });
          if (uploadResult.fileId) {
            logoFileId = uploadResult.fileId;
          }
        }
      }

      const updated = await updateMutation.mutateAsync({
        id: currentTemplate.id,
        ifMatch: currentTemplate.rowVersion,
        payload: {
          name: templateName.trim(),
          isDefault,
          layoutConfigJson: JSON.stringify(layout),
          logoFileId,
        },
      });

      setSelectedTemplateId(updated.id);
      setSaveSuccessMessage(t("saveSuccess"));
    } catch (err: unknown) {
      const errorText = err instanceof Error ? err.message : t("saveError");
      setErrorMessage(errorText);
    }
  };

  // Create new template modal confirm
  const handleCreateNewTemplate = async () => {
    if (!newTemplateName.trim() || !layout) return;
    setErrorMessage(null);

    try {
      const created = await createMutation.mutateAsync({
        documentType: activeDocType,
        name: newTemplateName.trim(),
        isDefault: false,
        layoutConfigJson: JSON.stringify(layout),
        logoFileId: currentTemplate?.logoFileId,
      });

      setIsCreateModalOpen(false);
      setNewTemplateName("");
      setSelectedTemplateId(created.id);
      setSaveSuccessMessage(t("createSuccess"));
    } catch (err: unknown) {
      const errorText = err instanceof Error ? err.message : t("createError");
      setErrorMessage(errorText);
    }
  };

  // Delete current template
  const handleDeleteTemplate = async () => {
    if (!currentTemplate || currentTemplate.isDefault) return;
    setErrorMessage(null);

    try {
      await deleteMutation.mutateAsync(currentTemplate.id);

      setIsDeleteModalOpen(false);
      setSelectedTemplateId("");
      await refetchTemplates();
      setSaveSuccessMessage(t("deleteSuccess"));
    } catch (err: unknown) {
      const errorText = err instanceof Error ? err.message : t("deleteError");
      setErrorMessage(errorText);
    }
  };

  // Reset to default settings
  const handleReset = () => {
    if (!currentTemplate) return;
    try {
      const parsed = JSON.parse(currentTemplate.layoutConfigJson) as DocumentTemplateLayoutConfig;
      setLayout(parsed);
      setTemplateName(currentTemplate.name);
      setSaveSuccessMessage(t("resetSuccess"));
    } catch {
      // ignore
    }
  };

  // Clean Print Action (only prints the A4 document sheet)
  const handlePrint = () => {
    window.print();
  };

  // Layout updates helpers
  const updateHeader = useCallback((patch: Partial<DocumentTemplateLayoutConfig["header"]>) => {
    setLayout((prev) => (prev ? { ...prev, header: { ...prev.header, ...patch } } : prev));
  }, []);

  const updateColumns = useCallback((patch: Partial<DocumentTemplateLayoutConfig["columns"]>) => {
    setLayout((prev) => (prev ? { ...prev, columns: { ...prev.columns, ...patch } } : prev));
  }, []);

  const updateSummary = useCallback((patch: Partial<DocumentTemplateLayoutConfig["summary"]>) => {
    setLayout((prev) => (prev ? { ...prev, summary: { ...prev.summary, ...patch } } : prev));
  }, []);

  const updateTerms = useCallback((patch: Partial<DocumentTemplateLayoutConfig["terms"]>) => {
    setLayout((prev) => (prev ? { ...prev, terms: { ...prev.terms, ...patch } } : prev));
  }, []);

  const updateSignatures = useCallback((patch: Partial<DocumentTemplateLayoutConfig["signatures"]>) => {
    setLayout((prev) => (prev ? { ...prev, signatures: { ...prev.signatures, ...patch } } : prev));
  }, []);

  const tabs: FormTabItem<StudioTabId>[] = [
    { id: "branding", label: t("tabs.branding") },
    { id: "columns", label: t("tabs.columns") },
    { id: "terms", label: t("tabs.terms") },
    { id: "signatures", label: t("tabs.signatures") },
  ];

  const templateOptions = useMemo(() => {
    if (!templates) return [];
    return templates.map((tmpl) => ({
      value: tmpl.id,
      label: `${tmpl.name}${tmpl.isDefault ? ` (${t("defaultBadge")})` : ""}`,
    }));
  }, [templates, t]);

  const isSaving = updateMutation.isPending || createMutation.isPending || isLogoUploading;

  return (
    <div className="space-y-4">
      {/* Top Architectural Toolbar (Hidden in Print) */}
      <div data-no-print="true" className="print:hidden bg-white border border-erp-border p-4 space-y-3">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-3">
          <div>
            <h1 className="text-xl font-bold text-slate-900 tracking-tight">
              {t("title")}
            </h1>
            <p className="text-xs text-slate-500 mt-0.5">
              {t("subtitle")}
            </p>
          </div>

          {/* Action Buttons */}
          <div className="flex flex-wrap items-center gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={() => setIsFullscreenPreviewOpen(true)}
              icon={<IconMaximize size={16} />}
            >
              {t("actions.fullscreen")}
            </Button>

            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handlePrint}
              icon={<IconPrinter size={16} />}
            >
              {t("actions.print")}
            </Button>

            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleReset}
              icon={<IconRotateCcw size={16} />}
            >
              {t("actions.reset")}
            </Button>

            {currentTemplate && !currentTemplate.isDefault && (
              <Button
                type="button"
                variant="danger"
                size="sm"
                onClick={() => setIsDeleteModalOpen(true)}
                icon={<IconTrash size={16} />}
              >
                {t("actions.delete")}
              </Button>
            )}

            <Button
              type="button"
              variant="primary"
              size="sm"
              onClick={handleSave}
              isLoading={isSaving}
              disabled={isSaving || !currentTemplate}
              icon={<IconSave size={16} />}
            >
              {t("actions.save")}
            </Button>
          </div>
        </div>

        {/* Document Type & Template Selector Grid */}
        <div className="grid grid-cols-1 md:grid-cols-12 gap-3 pt-2 border-t border-erp-border">
          <div className="md:col-span-4">
            <Select
              label={t("selectors.documentType")}
              options={DOCUMENT_TYPE_OPTIONS}
              value={activeDocType}
              onChange={(e) => handleDocTypeChange(e.target.value)}
              wrapperClassName="!mb-0"
            />
          </div>

          <div className="md:col-span-5 flex items-end gap-2">
            <div className="flex-1">
              <Select
                label={t("selectors.template")}
                options={templateOptions}
                value={selectedTemplateId}
                onChange={(e) => handleTemplateSelect(e.target.value)}
                disabled={isLoadingTemplates || templateOptions.length === 0}
                wrapperClassName="!mb-0"
              />
            </div>
            <Button
              type="button"
              variant="outline"
              size="md"
              onClick={() => setIsCreateModalOpen(true)}
              className="h-11 min-h-[44px] px-3 whitespace-nowrap"
              icon={<IconPlus size={16} />}
            >
              {t("actions.createTemplate")}
            </Button>
          </div>

          <div className="md:col-span-3 flex items-end">
            <div className="w-full h-11 min-h-[44px] border border-erp-border px-3 bg-slate-50 flex items-center justify-between">
              <span className="text-xs font-medium text-slate-700">
                {t("selectors.isDefaultTemplate")}
              </span>
              <input
                type="checkbox"
                id="is-default-checkbox"
                checked={isDefault}
                onChange={(e) => setIsDefault(e.target.checked)}
                className="h-4 w-4 accent-[#0B3056] cursor-pointer"
              />
            </div>
          </div>
        </div>

        {/* Notification Banners */}
        {saveSuccessMessage && (
          <Alert variant="success" className="py-2 text-xs">
            {saveSuccessMessage}
          </Alert>
        )}
        {errorMessage && (
          <Alert variant="danger" className="py-2 text-xs">
            {errorMessage}
          </Alert>
        )}
      </div>

      {/* Main Split-Pane Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-4 items-start">
        {/* Left Pane: Customizer Controls (5 cols) - Hidden in Print */}
        <div data-no-print="true" className="print:hidden lg:col-span-5 bg-white border border-erp-border p-4 space-y-4">
          {/* Template Name Input */}
          <Input
            label={t("fields.templateName")}
            value={templateName}
            onChange={(e) => setTemplateName(e.target.value)}
            placeholder={t("fields.templateNamePlaceholder")}
            required
            wrapperClassName="!mb-0"
          />

          {/* Form Tabs */}
          <FormTabs<StudioTabId>
            tabs={tabs}
            activeTab={activeTab}
            onChange={setActiveTab}
            ariaLabel="Template configuration sections"
          />

          {/* Tab 1: Branding & Header */}
          {layout && (
            <div className={activeTab === "branding" ? "space-y-4 pt-1" : "hidden"}>
              <Input
                label={t("fields.documentTitle")}
                value={layout.header.documentTitle}
                onChange={(e) => updateHeader({ documentTitle: e.target.value })}
                placeholder={t("fields.documentTitlePlaceholder")}
                required
              />

              <div className="border border-erp-border p-3 space-y-3 bg-slate-50/50">
                <Switch
                  label={t("fields.showLogo")}
                  checked={layout.header.showLogo}
                  onChange={(e) => updateHeader({ showLogo: e.target.checked })}
                />

                {layout.header.showLogo && (
                  <div className="space-y-3 pt-2 border-t border-erp-border">
                    <div className="grid grid-cols-2 gap-2">
                      <Select
                        label={t("fields.logoPosition")}
                        options={LOGO_POSITION_OPTIONS}
                        value={layout.header.logoPosition}
                        onChange={(e) =>
                          updateHeader({
                            logoPosition: e.target.value as "left" | "right",
                          })
                        }
                      />
                      <Select
                        label={t("fields.logoHeight")}
                        options={LOGO_HEIGHT_OPTIONS}
                        value={layout.header.logoHeight}
                        onChange={(e) => updateHeader({ logoHeight: e.target.value })}
                      />
                    </div>

                    {/* Logo File Selector */}
                    <div>
                      <label className="block text-xs font-semibold text-slate-700 mb-1">
                        {t("fields.uploadLogo")}
                      </label>
                      <div className="flex items-center gap-2">
                        <label className="cursor-pointer inline-flex items-center gap-1.5 px-3 py-2 border border-erp-border bg-white text-xs font-medium text-slate-700 hover:bg-slate-50">
                          <IconImage size={16} />
                          <span>{t("actions.chooseLogo")}</span>
                          <input
                            type="file"
                            accept="image/png,image/jpeg,image/webp,image/svg+xml"
                            className="sr-only"
                            onChange={handleLogoFileChange}
                          />
                        </label>
                        {logoPreviewUrl && (
                          <Button
                            type="button"
                            variant="ghost"
                            size="sm"
                            onClick={handleRemoveLogo}
                            className="text-red-600 hover:text-red-700 text-xs"
                          >
                            {t("actions.removeLogo")}
                          </Button>
                        )}
                      </div>
                    </div>
                  </div>
                )}
              </div>

              <Input
                label={t("fields.companyNameTh")}
                value={layout.header.companyNameTh ?? ""}
                onChange={(e) => updateHeader({ companyNameTh: e.target.value })}
                placeholder={t("fields.companyNameThPlaceholder")}
              />

              <Input
                label={t("fields.companyNameEn")}
                value={layout.header.companyNameEn ?? ""}
                onChange={(e) => updateHeader({ companyNameEn: e.target.value })}
                placeholder={t("fields.companyNameEnPlaceholder")}
              />

              <Input
                label={t("fields.taxId")}
                value={layout.header.taxId ?? ""}
                onChange={(e) => updateHeader({ taxId: e.target.value })}
                placeholder={t("fields.taxIdPlaceholder")}
                maxLength={13}
              />

              <Textarea
                label={t("fields.address")}
                value={layout.header.address ?? ""}
                onChange={(e) => updateHeader({ address: e.target.value })}
                placeholder={t("fields.addressPlaceholder")}
                rows={2}
              />

              <div className="grid grid-cols-2 gap-2">
                <Input
                  label={t("fields.phone")}
                  value={layout.header.phone ?? ""}
                  onChange={(e) => updateHeader({ phone: e.target.value })}
                  placeholder={t("fields.phonePlaceholder")}
                />
                <Input
                  label={t("fields.email")}
                  value={layout.header.email ?? ""}
                  onChange={(e) => updateHeader({ email: e.target.value })}
                  placeholder={t("fields.emailPlaceholder")}
                />
              </div>
            </div>
          )}

          {/* Tab 2: Columns & Summary */}
          {layout && (
            <div className={activeTab === "columns" ? "space-y-4 pt-1" : "hidden"}>
              <div className="border border-erp-border p-3 space-y-2">
                <h3 className="text-xs font-bold text-slate-900 mb-2">
                  {t("sections.tableColumns")}
                </h3>
                <Switch
                  label={t("fields.showSku")}
                  description={t("descriptions.showSku")}
                  checked={layout.columns.showSku}
                  onChange={(e) => updateColumns({ showSku: e.target.checked })}
                />
                <Switch
                  label={t("fields.showSpecs")}
                  description={t("descriptions.showSpecs")}
                  checked={layout.columns.showSpecs}
                  onChange={(e) => updateColumns({ showSpecs: e.target.checked })}
                />
                <Switch
                  label={t("fields.showThumbnail")}
                  description={t("descriptions.showThumbnail")}
                  checked={layout.columns.showThumbnail}
                  onChange={(e) => updateColumns({ showThumbnail: e.target.checked })}
                />
                <Switch
                  label={t("fields.showLineDiscount")}
                  description={t("descriptions.showLineDiscount")}
                  checked={layout.columns.showLineDiscount}
                  onChange={(e) => updateColumns({ showLineDiscount: e.target.checked })}
                />
              </div>

              <div className="border border-erp-border p-3 space-y-2">
                <h3 className="text-xs font-bold text-slate-900 mb-2">
                  {t("sections.summaryFields")}
                </h3>
                <Switch
                  label={t("fields.showVat")}
                  description={t("descriptions.showVat")}
                  checked={layout.summary.showVat}
                  onChange={(e) => updateSummary({ showVat: e.target.checked })}
                />
                <Switch
                  label={t("fields.showWithholdingTax")}
                  description={t("descriptions.showWithholdingTax")}
                  checked={layout.summary.showWithholdingTax}
                  onChange={(e) => updateSummary({ showWithholdingTax: e.target.checked })}
                />
                <Switch
                  label={t("fields.showBahtText")}
                  description={t("descriptions.showBahtText")}
                  checked={layout.summary.showBahtText}
                  onChange={(e) => updateSummary({ showBahtText: e.target.checked })}
                />
              </div>
            </div>
          )}

          {/* Tab 3: Terms & Bank Details */}
          {layout && (
            <div className={activeTab === "terms" ? "space-y-4 pt-1" : "hidden"}>
              <Input
                label={t("fields.validityPeriodText")}
                value={layout.terms.validityPeriodText ?? ""}
                onChange={(e) => updateTerms({ validityPeriodText: e.target.value })}
                placeholder={t("fields.validityPeriodTextPlaceholder")}
              />

              <Textarea
                label={t("fields.commercialTermsText")}
                value={layout.terms.commercialTermsText ?? ""}
                onChange={(e) => updateTerms({ commercialTermsText: e.target.value })}
                placeholder={t("fields.commercialTermsTextPlaceholder")}
                rows={3}
              />

              <div className="border border-erp-border p-3 space-y-3 bg-slate-50/50">
                <Switch
                  label={t("fields.showBankDetails")}
                  checked={layout.terms.showBankDetails}
                  onChange={(e) => updateTerms({ showBankDetails: e.target.checked })}
                />

                {layout.terms.showBankDetails && (
                  <div className="space-y-2 pt-2 border-t border-erp-border">
                    <Input
                      label={t("fields.bankName")}
                      value={layout.terms.bankName ?? ""}
                      onChange={(e) => updateTerms({ bankName: e.target.value })}
                      placeholder={t("fields.bankNamePlaceholder")}
                    />
                    <Input
                      label={t("fields.accountNo")}
                      value={layout.terms.accountNo ?? ""}
                      onChange={(e) => updateTerms({ accountNo: e.target.value })}
                      placeholder={t("fields.accountNoPlaceholder")}
                    />
                    <Input
                      label={t("fields.accountName")}
                      value={layout.terms.accountName ?? ""}
                      onChange={(e) => updateTerms({ accountName: e.target.value })}
                      placeholder={t("fields.accountNamePlaceholder")}
                    />
                  </div>
                )}
              </div>

              <Textarea
                label={t("fields.footerNote")}
                value={layout.terms.footerNote ?? ""}
                onChange={(e) => updateTerms({ footerNote: e.target.value })}
                placeholder={t("fields.footerNotePlaceholder")}
                rows={2}
              />
            </div>
          )}

          {/* Tab 4: Signatures */}
          {layout && (
            <div className={activeTab === "signatures" ? "space-y-4 pt-1" : "hidden"}>
              <Select
                label={t("fields.signatureCount")}
                options={SIGNATURE_COUNT_OPTIONS}
                value={String(layout.signatures.signatureCount)}
                onChange={(e) =>
                  updateSignatures({ signatureCount: parseInt(e.target.value, 10) })
                }
              />

              <div className="space-y-3">
                <Input
                  label={t("fields.box1Label")}
                  value={layout.signatures.box1Label}
                  onChange={(e) => updateSignatures({ box1Label: e.target.value })}
                  placeholder={t("fields.box1LabelPlaceholder")}
                  required
                />
                <Input
                  label={t("fields.box2Label")}
                  value={layout.signatures.box2Label ?? ""}
                  onChange={(e) => updateSignatures({ box2Label: e.target.value })}
                  placeholder={t("fields.box2LabelPlaceholder")}
                />
                {layout.signatures.signatureCount >= 3 && (
                  <Input
                    label={t("fields.box3Label")}
                    value={layout.signatures.box3Label ?? ""}
                    onChange={(e) => updateSignatures({ box3Label: e.target.value })}
                    placeholder={t("fields.box3LabelPlaceholder")}
                  />
                )}
              </div>
            </div>
          )}
        </div>

        {/* Right Pane: Live A4 Real-time Preview (7 cols) */}
        <div className="lg:col-span-7 sticky top-4 space-y-2 print:static print:w-full">
          {/* Preview Toolbar with Zoom & Fullscreen Controls */}
          <div data-no-print="true" className="print:hidden flex items-center justify-between px-1">
            <div className="flex items-center gap-2">
              <span className="text-xs font-semibold text-slate-700 uppercase tracking-wider">
                {t("previewTitle")}
              </span>
              <span className="text-[11px] text-slate-400 hidden sm:inline">
                A4 (210mm × 297mm)
              </span>
            </div>

            <div className="flex items-center gap-1">
              <button
                type="button"
                onClick={() => setPreviewZoom("fit")}
                className={`px-2.5 py-1 text-xs font-medium border border-erp-border transition-colors ${
                  previewZoom === "fit"
                    ? "bg-[#0B3056] text-white"
                    : "bg-white text-slate-700 hover:bg-slate-50"
                }`}
              >
                {t("actions.zoomFit")}
              </button>
              <button
                type="button"
                onClick={() => setPreviewZoom("100%")}
                className={`px-2.5 py-1 text-xs font-medium border border-erp-border transition-colors ${
                  previewZoom === "100%"
                    ? "bg-[#0B3056] text-white"
                    : "bg-white text-slate-700 hover:bg-slate-50"
                }`}
              >
                {t("actions.zoom100")}
              </button>
              <button
                type="button"
                onClick={() => setIsFullscreenPreviewOpen(true)}
                title={t("actions.fullscreen")}
                className="p-1.5 border border-erp-border bg-white text-slate-700 hover:bg-slate-50"
              >
                <IconMaximize size={14} />
              </button>
            </div>
          </div>

          {/* Preview Container: Scrollable without content truncation */}
          <div className="bg-slate-200/70 border border-erp-border p-3 sm:p-6 flex justify-center shadow-inner overflow-x-auto overflow-y-auto max-h-[calc(100vh-180px)] print:bg-white print:border-none print:p-0 print:max-h-none print:overflow-visible">
            {layout ? (
              <div
                className={`transition-all duration-200 origin-top flex justify-center w-full ${
                  previewZoom === "fit" ? "scale-[0.82] sm:scale-[0.88] xl:scale-95" : "scale-100"
                }`}
              >
                <A4DocumentPreview
                  layout={layout}
                  logoPreviewUrl={logoPreviewUrl}
                />
              </div>
            ) : (
              <div className="p-12 text-center text-xs text-slate-400">
                {t("noTemplateSelected")}
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Fullscreen A4 Preview Modal */}
      <Modal
        isOpen={isFullscreenPreviewOpen}
        onClose={() => setIsFullscreenPreviewOpen(false)}
        title={t("previewTitle")}
        size="2xl"
        footer={
          <div className="flex justify-between items-center w-full">
            <span className="text-xs text-slate-500">
              ขนาดกระดาษจริง A4 (210mm × 297mm) • ตัวอย่างความละเอียดสูง
            </span>
            <div className="flex items-center gap-2">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => setIsFullscreenPreviewOpen(false)}
              >
                {t("modal.cancel")}
              </Button>
              <Button
                type="button"
                variant="primary"
                size="sm"
                onClick={() => {
                  setIsFullscreenPreviewOpen(false);
                  setTimeout(() => window.print(), 100);
                }}
                icon={<IconPrinter size={16} />}
              >
                {t("actions.print")}
              </Button>
            </div>
          </div>
        }
      >
        <div className="bg-slate-200 p-4 sm:p-6 flex justify-center overflow-y-auto max-h-[75vh]">
          {layout && (
            <A4DocumentPreview
              layout={layout}
              logoPreviewUrl={logoPreviewUrl}
              className="shadow-2xl"
            />
          )}
        </div>
      </Modal>

      {/* Create New Template Modal */}
      <Modal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        title={t("modal.createTitle")}
        description={t("modal.createDesc")}
        size="md"
        footer={
          <div className="flex justify-end gap-2">
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={() => setIsCreateModalOpen(false)}
            >
              {t("modal.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              size="sm"
              onClick={handleCreateNewTemplate}
              isLoading={createMutation.isPending}
              disabled={!newTemplateName.trim() || createMutation.isPending}
            >
              {t("modal.confirmCreate")}
            </Button>
          </div>
        }
      >
        <div className="py-2">
          <Input
            label={t("fields.newTemplateName")}
            value={newTemplateName}
            onChange={(e) => setNewTemplateName(e.target.value)}
            placeholder={t("fields.newTemplateNamePlaceholder")}
            required
            autoFocus
          />
        </div>
      </Modal>

      {/* Delete Confirmation Modal */}
      <ConfirmationModal
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        onConfirm={handleDeleteTemplate}
        title={t("modal.deleteTitle")}
        message={t("modal.deleteMsg", { name: currentTemplate?.name ?? "" })}
        variant="danger"
        confirmText={t("modal.confirmDelete")}
        cancelText={t("modal.cancel")}
        isLoading={deleteMutation.isPending}
      />
    </div>
  );
}
