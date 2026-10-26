"use client";

/* eslint-disable no-restricted-syntax -- token-exempt: fallback design-style swatch palettes are data, not theme colours */

import React, { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useTranslations } from "next-intl";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import type {
  FieldSurveyItem,
  FieldSurveyFormData,
} from "../../schemas/field-survey-schema";
import { resolveFieldSurveyPhotoPurpose, resolveFieldSurveyPhotoTag, resolveGrossAreaSqm, resolveNetAreaSqm } from "../../schemas/field-survey-schema";
import {
  useOpportunitySurvey,
  useCreateSiteSurvey,
  useSurveyReferenceData,
} from "../../api/survey-queries";
import { useOpportunityDetail } from "@/features/opportunities/api/opportunity-queries";
import { apiClient } from "@/lib/api/api-client";
import type {
  UpdateSurveyAreaRequest,
  UpdateSurveyMeasurementRequest,
  UpdateSurveyDraftRequest,
} from "@/lib/api/api-client";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useOptionalSelectedMembership } from "@/lib/membership/selected-membership-context";
import { FieldSurveyMeasurementPad } from "./field-survey-measurement-pad";
import { FieldSurveyCatalogPicker } from "./field-survey-catalog-picker";
import { FieldSurveyItemPhotoManager } from "./field-survey-item-photo-manager";
import { RoomNameCombobox } from "./room-name-combobox";
import { OpportunityAutocomplete } from "@/features/opportunities/components/opportunity-autocomplete";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormSection } from "@/components/forms/FormSection";
import { Alert } from "@/components/ui/Alert";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { FormActionBar } from "@/components/forms/FormActionBar";
import {
  IconPlus,
  IconTrash,
  IconCheckCircle,
  IconChevronDown,
  IconCamera,
  IconArrowRight,
} from "@/components/common/Icons";

export interface DesignStyleOption {
  code: string;
  label: string;
  description: string;
}

export interface MoodToneOption {
  code: string;
  label: string;
  description: string;
  colorPalette: readonly string[];
}

export interface FieldSurveyWorkspaceProps {
  initialOpportunityId?: string;
  onSave?: (data: FieldSurveyFormData) => Promise<void>;
}

export function FieldSurveyWorkspace({
  initialOpportunityId = "",
  onSave,
}: FieldSurveyWorkspaceProps) {
  const router = useRouter();
  const locale = useSafeLocale();
  const t = useTranslations("fieldSurvey");
  const tShell = useTranslations("shell");

  const [opportunityId, setOpportunityId] = useState<string>(initialOpportunityId);
  const [roomName, setRoomName] = useState<string>("");
  const [surveyorNotes, setSurveyorNotes] = useState<string>("");
  const [designStyle, setDesignStyle] = useState<string>("");
  const [moodTone, setMoodTone] = useState<string>("");
  const [customStyleNotes, setCustomStyleNotes] = useState<string>("");
  const [items, setItems] = useState<FieldSurveyItem[]>([]);
  const [collapsedItemIds, setCollapsedItemIds] = useState<Set<string>>(new Set());
  const [isSaving, setIsSaving] = useState<boolean>(false);
  const [saveSuccess, setSaveSuccess] = useState<boolean>(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // Next Action Modal state after save
  const [isSuccessModalOpen, setIsSuccessModalOpen] = useState<boolean>(false);
  const [lastSavedSummary, setLastSavedSummary] = useState<{
    roomName: string;
    itemsCount: number;
    photosCount: number;
  } | null>(null);

  const { data: refData } = useSurveyReferenceData();
  const membershipContext = useOptionalSelectedMembership();
  const opportunityQuery = useOpportunityDetail(opportunityId || undefined);
  const surveyQuery = useOpportunitySurvey(opportunityId || undefined);
  const createSurveyMutation = useCreateSiteSurvey(opportunityId);
  const photoUploader = useDeferredFileUpload({ parentType: "survey-area" });

  const categoryOptions = refData?.categories && refData.categories.length > 0
    ? refData.categories
        .filter((c): c is typeof c & { code: string } => Boolean(c.code))
        .map((c) => ({
          code: c.code,
          label: locale === "en" ? c.name?.english || c.code : c.name?.thai || c.code,
        }))
    : [
        { code: "built_in", label: t("categories.built_in") },
        { code: "loose", label: t("categories.loose") },
        { code: "flooring", label: t("categories.flooring") },
        { code: "wall_ceiling", label: t("categories.wall_ceiling") },
        { code: "curtain", label: t("categories.curtain") },
        { code: "mep", label: t("categories.mep") },
      ];

  const designStyleOptions: DesignStyleOption[] = refData?.designStyles && refData.designStyles.length > 0
    ? refData.designStyles
        .filter((s): s is typeof s & { code: string } => Boolean(s.code))
        .map((s) => ({
          code: s.code,
          label: locale === "en" ? s.name?.english || s.code : s.name?.thai || s.code,
          description: s.description || "",
        }))
    : [
        { code: "modern_luxury", label: "Modern Luxury", description: "โมเดิร์น ลักชูรี่ หรูหรา กระจก/หินอ่อน/โลหะทอง" },
        { code: "japandi", label: "Japandi / Muji", description: "จาแปนดี้ / มูจิ เน้นไม้ธรรมชาติ อบอุ่น เรียบง่าย" },
        { code: "modern_contemporary", label: "Modern Contemporary", description: "โมเดิร์นร่วมสมัย เส้นสายเรียบคม ฟังก์ชันลงตัว" },
        { code: "scandinavian", label: "Scandinavian", description: "โปร่งสบาย โทนสีสว่าง ไม้ธรรมชาติ ผ่อนคลาย" },
        { code: "minimalist", label: "Minimalist", description: "มินิมอล เรียบเกลี้ยง ซ่อนฟังก์ชัน จัดเก็บสบายตา" },
        { code: "industrial_loft", label: "Industrial Loft", description: "โครงเหล็ก ปูนเปลือย อิฐโชว์แนว ไม้สีเข้ม" },
        { code: "classic_neo_classic", label: "Classic / Neo Classic", description: "บัวคิ้ว หน้าบานลูกฟัก สมมาตรภูมิฐาน" },
        { code: "vintage_retro", label: "Vintage / Retro", description: "กลิ่นอายย้อนยุค มีเสน่ห์เฉพาะตัว" },
      ];

  const moodToneOptions: MoodToneOption[] = refData?.moodTones && refData.moodTones.length > 0
    ? refData.moodTones
        .filter((m): m is typeof m & { code: string } => Boolean(m.code))
        .map((m) => ({
          code: m.code,
          label: locale === "en" ? m.name?.english || m.code : m.name?.thai || m.code,
          description: m.description || "",
          colorPalette: m.colorPalette || [],
        }))
    : [
        { code: "warm_earthy", label: "Warm & Earthy", description: "โทนอุ่น น้ำตาล ครีม เบจ ไม้ธรรมชาติ", colorPalette: ["#F5EBE0", "#D6CCC2", "#8D6E63", "#4E342E"] },
        { code: "cool_monotone", label: "Cool & Monotone", description: "โทนเย็น ขาว เทา ดำ หินอ่อน เรียบเท่", colorPalette: ["#FFFFFF", "#E0E0E0", "#757575", "#212121"] },
        { code: "dark_elegant", label: "Dark & Elegant", description: "ดาร์กลักชูรี่ ดำ วอลนัท ทอง หรูหราลุ่มลึก", colorPalette: ["#1A1A1A", "#3E2723", "#C5A059", "#0B3056"] },
        { code: "light_natural", label: "Light & Natural", description: "สว่างโปร่ง ไม้สน/โอ๊คธรรมชาติ สบายตา", colorPalette: ["#FAFAFA", "#FFF8E7", "#E2D4C9", "#A3B18A"] },
        { code: "colorful_pastel", label: "Pastel & Colorful", description: "สีสันสดใส เขียวเซจ น้ำเงินเข้ม พาสเทล", colorPalette: ["#C2D8B9", "#8E9AAF", "#CBC0D3", "#EFD3D7"] },
      ];

  useEffect(() => {
    if (initialOpportunityId) {
      setOpportunityId(initialOpportunityId);
    }
  }, [initialOpportunityId]);

  const handleAddItem = () => {
    const newItemId = `item-${Date.now()}-${Math.random().toString(36).substring(2, 7)}`;
    const newItem: FieldSurveyItem = {
      id: newItemId,
      title: "",
      category: "built_in",
      measurementType: "box",
      boxDimensions: { widthMm: 0, depthMm: 0, heightMm: 0 },
      specSelection: {},
      photos: [],
    };
    setItems((prev) => [...prev, newItem]);
    // Ensure newly added item is expanded
    setCollapsedItemIds((prev) => {
      const next = new Set(prev);
      next.delete(newItemId);
      return next;
    });
  };

  const handleUpdateItem = (index: number, updated: Partial<FieldSurveyItem>) => {
    setItems((prev) =>
      prev.map((item, i) => (i === index ? { ...item, ...updated } : item))
    );
  };

  const handleRemoveItem = (index: number) => {
    setItems((prev) => prev.filter((_, i) => i !== index));
  };

  const toggleCollapse = (id: string) => {
    setCollapsedItemIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }
      return next;
    });
  };

  const handleCollapseAll = () => {
    setCollapsedItemIds(new Set(items.map((it) => it.id)));
  };

  const handleExpandAll = () => {
    setCollapsedItemIds(new Set());
  };

  const scrollToItem = (id: string) => {
    const el = document.getElementById(id);
    if (el) {
      el.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMessage(null);
    setSaveSuccess(false);

    if (!roomName.trim()) {
      setErrorMessage(t("roomNameRequired"));
      return;
    }

    if (items.length === 0) {
      setErrorMessage(t("empty.noItemsTitle"));
      return;
    }

    const payload: FieldSurveyFormData = {
      opportunityId: opportunityId || "00000000-0000-4000-a000-000000000000",
      roomName: roomName.trim(),
      surveyorNotes: surveyorNotes.trim() || undefined,
      items,
    };

    setIsSaving(true);
    try {
      if (onSave) {
        await onSave(payload);
      } else if (opportunityId && opportunityId !== "00000000-0000-4000-a000-000000000000") {
        let survey = surveyQuery.data;

        // If survey does not exist yet, auto-create it for this opportunity
        if (!survey) {
          const opp = opportunityQuery.data;
          const siteId = opp?.primarySite?.id || "00000000-0000-0000-0000-000000000000";
          const assignedSurveyorId = opp?.owner?.id || "00000000-0000-0000-0000-000000000000";
          const idempotencyKey = `create-survey-${opportunityId}-${Date.now()}`;
          survey = await createSurveyMutation.mutateAsync({
            payload: {
              siteId,
              assignedSurveyorId,
              expectedOpportunityVersion: opp?.rowVersion || "00000000-0000-0000-0000-000000000000",
            },
            idempotencyKey,
          });
        }

        if (!survey?.id || !survey.currentRevision?.id) {
          throw new Error("No active survey draft revision available");
        }
        const surveyId = survey.id;
        const currentRevision = survey.currentRevision;
        const revisionId = survey.currentRevision.id;

        // Convert items into measurements
        const measurements: UpdateSurveyMeasurementRequest[] = [];
        let mSort = 1;
        for (const it of items) {
          const categoryTag = it.category ? `[${it.category}]` : "";
          if (it.measurementType === "box" && it.boxDimensions) {
            if (it.boxDimensions.widthMm > 0) {
              measurements.push({
                id: null,
                measurementType: "width",
                value: it.boxDimensions.widthMm,
                unitCode: "mm",
                captureMethod: "measured",
                notes: `${it.title} (กว้าง) ${categoryTag}`.trim(),
                sortOrder: mSort++,
              });
            }
            if (it.boxDimensions.depthMm > 0) {
              measurements.push({
                id: null,
                measurementType: "depth",
                value: it.boxDimensions.depthMm,
                unitCode: "mm",
                captureMethod: "measured",
                notes: `${it.title} (ลึก) ${categoryTag}`.trim(),
                sortOrder: mSort++,
              });
            }
            if (it.boxDimensions.heightMm > 0) {
              measurements.push({
                id: null,
                measurementType: "height",
                value: it.boxDimensions.heightMm,
                unitCode: "mm",
                captureMethod: "measured",
                notes: `${it.title} (สูง) ${categoryTag}`.trim(),
                sortOrder: mSort++,
              });
            }
          } else if (it.measurementType === "area" && it.areaDimensions) {
            const netArea = resolveNetAreaSqm(it.areaDimensions);
            if (netArea > 0) {
              measurements.push({
                id: null,
                measurementType: "area",
                value: netArea,
                unitCode: "sqm",
                captureMethod: it.areaDimensions.photoMeasurement ? "photo_estimated" : "measured",
                notes: `${it.title} (พื้นที่) ${categoryTag}`.trim(),
                sortOrder: mSort++,
              });
            }
          } else if (it.measurementType === "custom" && it.customMeasurements) {
            for (const pt of it.customMeasurements) {
              if (pt.valueMm > 0) {
                measurements.push({
                  id: null,
                  measurementType: "custom",
                  value: pt.valueMm,
                  unitCode: "mm",
                  captureMethod: "measured",
                  notes: `${it.title}: ${pt.label} ${categoryTag}`.trim(),
                  sortOrder: mSort++,
                });
              }
            }
          }
        }

        // Format Customer Style & Mood/Tone info
        const stylePrefixParts: string[] = [];
        if (designStyle) {
          const matched = designStyleOptions.find((d) => d.code === designStyle);
          stylePrefixParts.push(`สไตล์: ${matched?.label || designStyle}`);
        }
        if (moodTone) {
          const matched = moodToneOptions.find((m) => m.code === moodTone);
          stylePrefixParts.push(`Mood & Tone: ${matched?.label || moodTone}`);
        }
        if (customStyleNotes.trim()) {
          stylePrefixParts.push(`ข้อกำหนดพิเศษ: ${customStyleNotes.trim()}`);
        }
        const styleHeader = stylePrefixParts.length > 0 ? `[${stylePrefixParts.join(" | ")}]` : "";

        // Build or merge Area for this room
        const existingAreas = currentRevision.areas || [];
        const roomCode = `RM-${(existingAreas.length + 1).toString().padStart(2, "0")}`;
        const roomDescription = [styleHeader, surveyorNotes.trim()].filter(Boolean).join("\n") || null;
        const newAreaRequest: UpdateSurveyAreaRequest = {
          id: null,
          code: roomCode,
          name: roomName.trim(),
          description: roomDescription,
          sortOrder: existingAreas.length + 1,
          measurements,
        };

        const existingAreaRequests: UpdateSurveyAreaRequest[] = existingAreas.map((ea, idx) => ({
          id: ea.id,
          code: ea.code,
          name: ea.name,
          description: ea.description || null,
          sortOrder: ea.sortOrder || idx + 1,
          measurements: (ea.measurements || []).map((em, mIdx) => ({
            id: em.id,
            measurementType: em.measurementType,
            value: em.value,
            unitCode: em.unitCode,
            captureMethod: em.captureMethod,
            notes: em.notes || null,
            sortOrder: em.sortOrder || mIdx + 1,
          })),
        }));

        const combinedScopeSummary = [
          styleHeader,
          surveyorNotes.trim() || currentRevision.scopeSummary,
        ].filter(Boolean).join("\n\n") || null;

        const updatePayload: UpdateSurveyDraftRequest = {
          expectedRevisionVersion: currentRevision.rowVersion,
          visitedAtUtc: currentRevision.visitedAtUtc || new Date().toISOString(),
          scopeSummary: combinedScopeSummary,
          assumptions: [
            ...(currentRevision.assumptions || []),
            ...(stylePrefixParts.length > 0 && !currentRevision.assumptions?.some((a) => a.startsWith("[สไตล์/Mood&Tone]"))
              ? [`[สไตล์/Mood&Tone] ${stylePrefixParts.join(" | ")}`]
              : []),
          ],
          constraints: [...(currentRevision.constraints || [])],
          missingDetails: [...(currentRevision.missingDetails || [])],
          areas: [...existingAreaRequests, newAreaRequest],
        };

        const token = await getAuthToken();
        if (!token) {
          throw new Error("Authentication required");
        }

        const savedRevision = await apiClient.updateSurveyDraft(
          opportunityId,
          surveyId,
          revisionId,
          updatePayload,
          {
            token,
            membershipId: membershipContext?.selectedMembership?.id,
            ifMatch: `"${currentRevision.rowVersion}"`,
          }
        );

        // Photos upload only now: the area id exists once the draft is saved.
        const photoFiles = items.flatMap((it) => it.photos).filter((p): p is typeof p & { file: File } => p.file instanceof File);
        if (photoFiles.length > 0) {
          const savedAreaId = savedRevision.areas?.find((a) => a.code === roomCode)?.id;
          if (!savedAreaId) {
            throw new Error("Saved survey area not found");
          }
          const uploadOptions = {
            token,
            membershipId: membershipContext?.selectedMembership?.id,
            locale,
            parentId: savedAreaId,
          };
          const uploaded = await photoUploader.uploadFiles(
            photoFiles.map((p) => ({ file: p.file, caption: p.caption ?? undefined })),
            uploadOptions
          );
          // One attach per photo: tag and caption belong to a single file.
          for (const [i, photo] of photoFiles.entries()) {
            const fileId = uploaded.files[i]?.fileId;
            if (!fileId) continue;
            await apiClient.attachFiles(
              "survey-area",
              savedAreaId,
              {
                purpose: resolveFieldSurveyPhotoPurpose(photo.tag),
                tag: resolveFieldSurveyPhotoTag(photo.tag),
                caption: photo.caption?.trim() || null,
                fileIds: [fileId],
              },
              {
                token,
                membershipId: uploadOptions.membershipId,
                locale: uploadOptions.locale,
                idempotencyKey: `survey-photo-${crypto.randomUUID()}`,
              }
            );
          }
        }
      } else {
        // Fallback simulated save
        await new Promise((resolve) => setTimeout(resolve, 600));
      }
      setSaveSuccess(true);
      setLastSavedSummary({
        roomName: roomName.trim(),
        itemsCount: items.length,
        photosCount: totalPhotosCount,
      });
      setIsSuccessModalOpen(true);
    } catch {
      setErrorMessage(t("actions.saveFailed"));
    } finally {
      setIsSaving(false);
    }
  };

  const handleStartNextRoom = () => {
    setIsSuccessModalOpen(false);
    setRoomName("");
    setSurveyorNotes("");
    setItems([]);
    setCollapsedItemIds(new Set());
    setSaveSuccess(false);
    if (typeof window !== "undefined") {
      try {
        window.scrollTo({ top: 0, behavior: "smooth" });
      } catch {
        // Fallback for environments where scrollTo is not implemented
      }
    }
  };

  const handleGoToOpportunity = () => {
    setIsSuccessModalOpen(false);
    if (opportunityId && opportunityId !== "00000000-0000-4000-a000-000000000000") {
      router.push(`/${locale}/opportunities/${opportunityId}`);
    } else {
      router.push(`/${locale}/opportunities`);
    }
  };

  const handleBack = () => {
    if (opportunityId && opportunityId !== "00000000-0000-4000-a000-000000000000") {
      router.push(`/${locale}/opportunities/${opportunityId}`);
    } else {
      router.push(`/${locale}/surveys`);
    }
  };

  const totalPhotosCount = items.reduce((acc, it) => acc + (it.photos?.length ?? 0), 0);

  return (
    <FormContainer
      asForm
      onSubmit={handleSubmit}
      header={
        <PageHeader
          title={t("title")}
          subtitle={t("subtitle")}
          onBack={handleBack}
          backLabel={t("backToList")}
          breadcrumbs={[
            { label: tShell("fieldSurveys"), href: `/${locale}/surveys` },
            { label: t("title") },
          ]}
          actions={
            items.length > 0 ? (
              <div className="flex items-center gap-3 border border-erp-border bg-erp-surface-subtle px-3 py-1.5 text-xs font-semibold text-erp-navy">
                <span>{t("itemsCount", { count: items.length })}</span>
                <span className="text-erp-border">|</span>
                <span className="flex items-center gap-1">
                  <IconCamera size={14} />
                  {t("totalPhotos", { count: totalPhotosCount })}
                </span>
              </div>
            ) : undefined
          }
        />
      }
      errorBanner={
        errorMessage ? (
          <Alert variant="danger" onClose={() => setErrorMessage(null)}>
            {errorMessage}
          </Alert>
        ) : null
      }
      topAlert={
        saveSuccess ? (
          <Alert variant="success" onClose={() => setSaveSuccess(false)}>
            {t("actions.savedSuccess")}
          </Alert>
        ) : null
      }
      actionBar={
        <FormActionBar
          sticky={true}
          isLoading={isSaving}
          isSaveDisabled={isSaving || items.length === 0}
          saveText={isSaving ? t("actions.saving") : t("actions.saveDraft")}
          saveButtonType="submit"
          showCancel={false}
          statusContent={
            <Button
              type="button"
              variant="outline"
              size="md"
              onClick={handleAddItem}
              disabled={isSaving}
              icon={<IconPlus size={16} />}
            >
              {t("addItem")}
            </Button>
          }
        />
      }
    >
      {/* Section 1: Target Opportunity & Survey Room */}
      <FormSection
        title={t("surveyTargetSection")}
        description={t("surveyTargetSectionDesc")}
      >
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          {/* 1. Opportunity Autocomplete Dropdown */}
          <div>
            <OpportunityAutocomplete
              value={opportunityId}
              onChange={(id) => setOpportunityId(id)}
              disabled={isSaving}
            />
          </div>

          {/* 2. Room Name Combobox Dropdown */}
          <div>
            <RoomNameCombobox
              value={roomName}
              onChange={setRoomName}
              disabled={isSaving}
              required
            />
          </div>

          {/* Surveyor Overview Notes */}
          <div className="sm:col-span-2">
            <Textarea
              label={t("surveyorNotes")}
              rows={2}
              value={surveyorNotes}
              onChange={(e) => setSurveyorNotes(e.target.value)}
              disabled={isSaving}
              placeholder={t("surveyorNotesPlaceholder")}
              wrapperClassName="!mb-0"
              className="text-xs"
            />
          </div>
        </div>
      </FormSection>

      {/* Section 1.5: Customer Preferred Style & Mood/Tone */}
      <FormSection
        title={t("style.sectionTitle")}
        description={t("style.sectionDesc")}
      >
        <div className="space-y-4">
          {/* Design Style */}
          <div>
            <label className="erp-label mb-2 block font-medium text-erp-text-main">
              {t("style.designStyleLabel")}
            </label>
            <div className="grid grid-cols-2 gap-2 sm:grid-cols-4">
              {designStyleOptions.map((style) => {
                const isSelected = designStyle === style.code;
                return (
                  <button
                    key={style.code}
                    type="button"
                    onClick={() => setDesignStyle(isSelected ? "" : style.code)}
                    disabled={isSaving}
                    className={`flex flex-col items-start border p-2.5 text-left transition-colors ${
                      isSelected
                        ? "border-erp-navy bg-erp-navy text-white shadow-sm"
                        : "border-erp-border bg-white text-erp-text-main hover:border-erp-navy/50 hover:bg-erp-surface-subtle"
                    }`}
                  >
                    <span className="text-xs font-semibold">{style.label}</span>
                    {style.description ? (
                      <span
                        className={`mt-1 line-clamp-2 text-[10px] ${
                          isSelected ? "text-white/80" : "text-erp-text-muted"
                        }`}
                      >
                        {style.description}
                      </span>
                    ) : null}
                  </button>
                );
              })}
            </div>
          </div>

          {/* Mood & Tone with Color Swatches */}
          <div>
            <label className="erp-label mb-2 block font-medium text-erp-text-main">
              {t("style.moodToneLabel")}
            </label>
            <div className="grid grid-cols-1 gap-2 sm:grid-cols-2 lg:grid-cols-5">
              {moodToneOptions.map((tone) => {
                const isSelected = moodTone === tone.code;
                return (
                  <button
                    key={tone.code}
                    type="button"
                    onClick={() => setMoodTone(isSelected ? "" : tone.code)}
                    disabled={isSaving}
                    className={`flex flex-col border p-2.5 text-left transition-colors ${
                      isSelected
                        ? "border-erp-navy bg-erp-navy text-white shadow-sm"
                        : "border-erp-border bg-white text-erp-text-main hover:border-erp-navy/50 hover:bg-erp-surface-subtle"
                    }`}
                  >
                    {tone.colorPalette && tone.colorPalette.length > 0 ? (
                      <div className="mb-2 flex h-3 w-full border border-black/10 overflow-hidden">
                        {tone.colorPalette.map((color, cIdx) => (
                          <div
                            key={cIdx}
                            className="h-full flex-1"
                            style={{ backgroundColor: color }}
                          />
                        ))}
                      </div>
                    ) : null}
                    <span className="text-xs font-semibold">{tone.label}</span>
                    {tone.description ? (
                      <span
                        className={`mt-1 line-clamp-2 text-[10px] ${
                          isSelected ? "text-white/80" : "text-erp-text-muted"
                        }`}
                      >
                        {tone.description}
                      </span>
                    ) : null}
                  </button>
                );
              })}
            </div>
          </div>

          {/* Style & Color Notes */}
          <div>
            <Input
              label={t("style.customStyleNotes")}
              value={customStyleNotes}
              onChange={(e) => setCustomStyleNotes(e.target.value)}
              disabled={isSaving}
              placeholder={t("style.customStyleNotesPlaceholder")}
              wrapperClassName="!mb-0"
              className="text-xs"
            />
          </div>
        </div>
      </FormSection>

      {/* Section 2: Custom Items in this Room */}
      <FormSection
        title={t("itemsSectionTitle")}
        description={t("itemsSectionDesc")}
        headerAction={
          <div className="flex items-center gap-2">
            {items.length > 0 && (
              <div className="flex items-center gap-1">
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={handleCollapseAll}
                >
                  {t("collapseAll")}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={handleExpandAll}
                >
                  {t("expandAll")}
                </Button>
              </div>
            )}
            <Button
              type="button"
              variant="primary"
              size="md"
              onClick={handleAddItem}
              disabled={isSaving}
              icon={<IconPlus size={16} />}
            >
              {t("addItem")}
            </Button>
          </div>
        }
      >
        {/* Quick Anchor Navigation Chips on Mobile/Tablet */}
        {items.length > 1 && (
          <div className="sticky top-0 z-20 flex items-center gap-2 overflow-x-auto border border-erp-border bg-white/95 px-3 py-2 shadow-sm backdrop-blur-sm scrollbar-none">
            <span className="shrink-0 text-[11px] font-semibold text-erp-text-muted">
              {t("jumpToItem")}
            </span>
            {items.map((it, idx) => (
              <button
                key={it.id}
                type="button"
                onClick={() => scrollToItem(it.id)}
                className="inline-flex min-h-[32px] shrink-0 items-center border border-erp-border bg-erp-surface-subtle px-2.5 py-1 text-xs font-medium text-erp-navy hover:border-erp-navy hover:bg-erp-surface"
              >
                #{idx + 1} {it.title || t("itemTitlePlaceholder").split(",")[0]}
              </button>
            ))}
          </div>
        )}

        {/* Items List */}
        {items.length === 0 ? (
          <div className="border border-dashed border-erp-border bg-white p-10 text-center">
            <h3 className="text-sm font-semibold text-erp-navy">{t("empty.noItemsTitle")}</h3>
            <p className="mt-1 text-xs text-erp-text-muted">{t("empty.noItemsDesc")}</p>
            <Button
              type="button"
              variant="outline"
              size="md"
              onClick={handleAddItem}
              className="mt-4"
              icon={<IconPlus size={16} />}
            >
              {t("addItem")}
            </Button>
          </div>
        ) : (
          <div className="space-y-4">
          {items.map((item, index) => {
            const isCollapsed = collapsedItemIds.has(item.id);

            // Summary text for collapsed card
            let dimensionSummary = "";
            if (item.measurementType === "box" && item.boxDimensions) {
              const { widthMm, depthMm, heightMm } = item.boxDimensions;
              if (widthMm || depthMm || heightMm) {
                dimensionSummary = t("dimensions.boxSummary", {
                  w: widthMm,
                  d: depthMm,
                  h: heightMm,
                });
              }
            } else if (item.measurementType === "area" && item.areaDimensions) {
              if (resolveGrossAreaSqm(item.areaDimensions) > 0) {
                const area = resolveNetAreaSqm(item.areaDimensions).toFixed(2);
                dimensionSummary = t("dimensions.areaSummary", { area });
              }
            } else if (item.measurementType === "custom" && item.customMeasurements) {
              dimensionSummary = t("dimensions.customSummary", {
                count: item.customMeasurements.length,
              });
            }

            const specSummary = item.specSelection?.coreSkuName || item.specSelection?.finishSkuName || "";

            return (
              <div
                key={item.id}
                id={item.id}
                className="border border-erp-border bg-white shadow-sm transition-shadow"
              >
                {/* Collapsible Item Card Header */}
                <div
                  onClick={() => toggleCollapse(item.id)}
                  className="flex cursor-pointer flex-wrap items-center justify-between border-b border-erp-border bg-erp-surface-subtle p-3 transition-colors hover:bg-erp-surface sm:px-4"
                >
                  <div className="flex min-w-0 flex-1 items-center gap-2 pr-2">
                    <span
                      className={`transition-transform duration-200 ${
                        isCollapsed ? "-rotate-90" : "rotate-0"
                      }`}
                    >
                      <IconChevronDown size={16} />
                    </span>
                    <span className="text-xs font-bold uppercase tracking-wider text-erp-navy">
                      {t("actions.itemNumber", { number: index + 1 })}:
                    </span>
                    <span className="truncate text-xs font-semibold text-erp-text-main">
                      {item.title || (
                        <span className="italic text-erp-text-muted">
                          {t("unnamedItem")}
                        </span>
                      )}
                    </span>

                    {/* Summary badges when collapsed */}
                    {isCollapsed && (
                      <div className="hidden flex-wrap items-center gap-2 sm:flex">
                        {dimensionSummary && (
                          <span className="border border-erp-border bg-white px-2 py-0.5 text-[10px] text-erp-text-muted">
                            {dimensionSummary}
                          </span>
                        )}
                        {specSummary && (
                          <span className="truncate max-w-[160px] border border-erp-border bg-white px-2 py-0.5 text-[10px] text-erp-text-muted">
                            {specSummary}
                          </span>
                        )}
                        {item.photos.length > 0 && (
                          <span className="inline-flex items-center gap-1 border border-erp-border bg-white px-1.5 py-0.5 text-[10px] text-erp-navy">
                            <IconCamera size={12} />
                            {item.photos.length}
                          </span>
                        )}
                      </div>
                    )}
                  </div>

                  <div
                    className="flex items-center gap-2"
                    onClick={(e) => e.stopPropagation()}
                  >
                    <button
                      type="button"
                      disabled={isSaving}
                      onClick={() => handleRemoveItem(index)}
                      className="inline-flex min-h-[36px] items-center gap-1 border border-erp-danger-border bg-white px-2.5 py-1 text-xs font-medium text-erp-danger hover:bg-erp-danger-bg"
                    >
                      <IconTrash size={14} />
                      {t("actions.deleteItem")}
                    </button>
                  </div>
                </div>

                {/* Expanded Item Body */}
                {!isCollapsed && (
                  <div className="space-y-4 p-4 sm:p-5">
                    {/* Item Name & Category */}
                    <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
                      <div className="sm:col-span-2">
                        <Input
                          label={t("itemTitle")}
                          required
                          value={item.title}
                          onChange={(e) => handleUpdateItem(index, { title: e.target.value })}
                          disabled={isSaving}
                          placeholder={t("itemTitlePlaceholder")}
                          wrapperClassName="!mb-0"
                          className="text-xs"
                        />
                      </div>
                      <div>
                        <Select
                          label={t("category")}
                          value={item.category ?? "built_in"}
                          onChange={(e) => handleUpdateItem(index, { category: e.target.value })}
                          disabled={isSaving}
                          wrapperClassName="!mb-0"
                          className="text-xs"
                          options={categoryOptions.map((cat) => ({
                            value: cat.code,
                            label: cat.label,
                          }))}
                        />
                      </div>
                    </div>

                    {/* Sub-component 1: Measurement Pad */}
                    <div>
                      <FieldSurveyMeasurementPad
                        measurementType={item.measurementType}
                        boxDimensions={item.boxDimensions}
                        areaDimensions={item.areaDimensions}
                        customMeasurements={item.customMeasurements}
                        onTypeChange={(type) => handleUpdateItem(index, { measurementType: type })}
                        onBoxChange={(box) => handleUpdateItem(index, { boxDimensions: box })}
                        onAreaChange={(area) => handleUpdateItem(index, { areaDimensions: area })}
                        onPhotoAdd={(photo) =>
                          handleUpdateItem(index, { photos: [...item.photos, photo] })
                        }
                        onCustomChange={(custom) =>
                          handleUpdateItem(index, { customMeasurements: custom })
                        }
                        disabled={isSaving}
                      />
                    </div>

                    {/* Sub-component 2: Catalog & Spec Picker */}
                    <div>
                      <FieldSurveyCatalogPicker
                        value={item.specSelection}
                        onChange={(spec) => handleUpdateItem(index, { specSelection: spec })}
                        disabled={isSaving}
                      />
                    </div>

                    {/* Sub-component 3: Photos Manager */}
                    <div>
                      <FieldSurveyItemPhotoManager
                        photos={item.photos}
                        onChange={(photos) => handleUpdateItem(index, { photos })}
                        disabled={isSaving}
                      />
                    </div>
                  </div>
                )}
              </div>
            );
          })}
        </div>
      )}
      </FormSection>

      {/* Next Action Success Modal - Atelier Architectural Navy Sharp */}
      <Modal
        isOpen={isSuccessModalOpen}
        onClose={() => setIsSuccessModalOpen(false)}
        size="md"
        showCloseButton={true}
        title={t("completionModal.title")}
        description={t("completionModal.subtitle")}
      >
        <div className="space-y-4 py-2">
          {/* Summary Box */}
          <div className="border border-erp-border bg-erp-surface-subtle p-3.5 text-xs">
            <div className="flex items-center gap-2 font-semibold text-green-700">
              <IconCheckCircle size={16} />
              <span>{lastSavedSummary?.roomName}</span>
            </div>
            <div className="mt-2 flex items-center gap-3 text-erp-text-muted">
              <span>
                {t("completionModal.summaryItems", {
                  count: lastSavedSummary?.itemsCount ?? 0,
                })}
              </span>
              <span>•</span>
              <span>
                {t("completionModal.summaryPhotos", {
                  count: lastSavedSummary?.photosCount ?? 0,
                })}
              </span>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex flex-col gap-2 pt-2">
            <Button
              type="button"
              variant="primary"
              size="md"
              className="w-full justify-center gap-2"
              onClick={handleStartNextRoom}
            >
              <IconPlus size={16} />
              <span>{t("completionModal.nextRoomBtn")}</span>
            </Button>

            <Button
              type="button"
              variant="outline"
              size="md"
              className="w-full justify-center gap-2"
              onClick={handleGoToOpportunity}
            >
              <IconArrowRight size={16} />
              <span>{t("completionModal.viewOpportunityBtn")}</span>
            </Button>

            <button
              type="button"
              onClick={() => setIsSuccessModalOpen(false)}
              className="mt-1 w-full py-1 text-center text-xs text-erp-text-muted hover:text-erp-navy hover:underline"
            >
              {t("completionModal.stayOnPageBtn")}
            </button>
          </div>
        </div>
      </Modal>
    </FormContainer>
  );
}
