"use client";

import React, { useState } from "react";
import { useTranslations, useLocale } from "next-intl";
import { useOpportunityList } from "@/features/opportunities/api/opportunity-queries";
import type {
  FieldSurveyItem,
  FieldSurveyFormData,
} from "../../schemas/field-survey-schema";
import { FieldSurveyMeasurementPad } from "./field-survey-measurement-pad";
import { FieldSurveyCatalogPicker } from "./field-survey-catalog-picker";
import { FieldSurveyItemPhotoManager } from "./field-survey-item-photo-manager";
import { IconPlus, IconTrash, IconCheckCircle } from "@/components/common/Icons";

export interface FieldSurveyWorkspaceProps {
  initialOpportunityId?: string;
  onSave?: (data: FieldSurveyFormData) => Promise<void>;
}

export function FieldSurveyWorkspace({
  initialOpportunityId = "",
  onSave,
}: FieldSurveyWorkspaceProps) {
  const t = useTranslations("fieldSurvey");
  const locale = useLocale();
  const oppQuery = useOpportunityList({ limit: 50 });

  const [opportunityId, setOpportunityId] = useState<string>(initialOpportunityId);
  const [roomName, setRoomName] = useState<string>("");
  const [surveyorNotes, setSurveyorNotes] = useState<string>("");
  const [items, setItems] = useState<FieldSurveyItem[]>([]);
  const [isSaving, setIsSaving] = useState<boolean>(false);
  const [saveSuccess, setSaveSuccess] = useState<boolean>(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // Flatten opportunities from paginated query
  const opportunities =
    oppQuery.data?.pages.flatMap((page) => page.items ?? []) ?? [];

  const handleAddItem = () => {
    const newItem: FieldSurveyItem = {
      id: `item-${Date.now()}-${Math.random().toString(36).substring(2, 7)}`,
      title: "",
      category: "built_in",
      measurementType: "box",
      boxDimensions: { widthMm: 0, depthMm: 0, heightMm: 0 },
      specSelection: {},
      photos: [],
    };
    setItems((prev) => [...prev, newItem]);
  };

  const handleUpdateItem = (index: number, updated: Partial<FieldSurveyItem>) => {
    setItems((prev) =>
      prev.map((item, i) => (i === index ? { ...item, ...updated } : item))
    );
  };

  const handleRemoveItem = (index: number) => {
    setItems((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMessage(null);
    setSaveSuccess(false);

    if (!roomName.trim()) {
      setErrorMessage(t("roomName") + " is required");
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
      } else {
        // Fallback simulated save
        await new Promise((resolve) => setTimeout(resolve, 600));
      }
      setSaveSuccess(true);
    } catch {
      setErrorMessage("Failed to save survey data. Please try again.");
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="mx-auto max-w-5xl space-y-6 pb-24">
      {/* Page Header */}
      <div className="border border-erp-border bg-white p-5 shadow-sm">
        <h1 className="text-xl font-bold text-erp-navy">{t("title")}</h1>
        <p className="mt-1 text-xs text-erp-text-muted">{t("subtitle")}</p>

        {saveSuccess && (
          <div className="mt-4 flex items-center gap-2 border border-green-500 bg-green-50 p-3 text-xs font-semibold text-green-800">
            <IconCheckCircle size={16} />
            <span>{t("actions.savedSuccess")}</span>
          </div>
        )}

        {errorMessage && (
          <div className="mt-4 border border-erp-danger-border bg-erp-danger-bg p-3 text-xs font-semibold text-erp-danger">
            {errorMessage}
          </div>
        )}

        <div className="mt-5 grid grid-cols-1 gap-4 sm:grid-cols-2">
          {/* Opportunity Selector */}
          <div>
            <label className="mb-1 block text-xs font-semibold text-erp-text-main">
              {t("opportunity")}
            </label>
            <select
              value={opportunityId}
              onChange={(e) => setOpportunityId(e.target.value)}
              disabled={isSaving}
              className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-xs font-medium text-erp-text-main focus:border-erp-navy focus:outline-none"
            >
              <option value="">{t("selectOpportunity")}</option>
              {opportunities.map((opp) => {
                const customerName =
                  locale === "en"
                    ? opp.customer?.displayNameEn || opp.customer?.displayNameTh || "-"
                    : opp.customer?.displayNameTh || opp.customer?.displayNameEn || "-";
                return (
                  <option key={opp.id} value={opp.id}>
                    {opp.title} ({customerName})
                  </option>
                );
              })}
            </select>
          </div>

          {/* Room Name */}
          <div>
            <label className="mb-1 block text-xs font-semibold text-erp-text-main">
              {t("roomName")} <span className="text-erp-danger">*</span>
            </label>
            <input
              type="text"
              required
              value={roomName}
              onChange={(e) => setRoomName(e.target.value)}
              disabled={isSaving}
              placeholder={t("roomPlaceholder")}
              className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-xs font-medium text-erp-text-main focus:border-erp-navy focus:outline-none"
            />
          </div>

          {/* Surveyor Overview Notes */}
          <div className="sm:col-span-2">
            <label className="mb-1 block text-xs font-semibold text-erp-text-main">
              {t("surveyorNotes")}
            </label>
            <textarea
              rows={2}
              value={surveyorNotes}
              onChange={(e) => setSurveyorNotes(e.target.value)}
              disabled={isSaving}
              placeholder={t("surveyorNotesPlaceholder")}
              className="w-full border border-erp-border bg-white p-3 text-xs text-erp-text-main focus:border-erp-navy focus:outline-none"
            />
          </div>
        </div>
      </div>

      {/* Items Section Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-base font-bold text-erp-navy">{t("itemsSectionTitle")}</h2>
          <span className="text-xs text-erp-text-muted">
            {items.length} {t("itemsSectionTitle")}
          </span>
        </div>
        <button
          type="button"
          onClick={handleAddItem}
          disabled={isSaving}
          className="inline-flex min-h-[44px] items-center gap-2 border border-erp-navy bg-erp-navy px-4 py-2 text-xs font-semibold text-white transition-colors hover:bg-erp-navy-hover active:bg-erp-navy-active disabled:cursor-not-allowed disabled:opacity-50"
        >
          <IconPlus size={16} />
          {t("addItem")}
        </button>
      </div>

      {/* Items List */}
      {items.length === 0 ? (
        <div className="border border-dashed border-erp-border bg-white p-10 text-center">
          <h3 className="text-sm font-semibold text-erp-navy">{t("empty.noItemsTitle")}</h3>
          <p className="mt-1 text-xs text-erp-text-muted">{t("empty.noItemsDesc")}</p>
          <button
            type="button"
            onClick={handleAddItem}
            className="mt-4 inline-flex min-h-[44px] items-center gap-2 border border-erp-navy bg-white px-4 py-2 text-xs font-semibold text-erp-navy hover:bg-erp-surface"
          >
            <IconPlus size={16} />
            {t("addItem")}
          </button>
        </div>
      ) : (
        <div className="space-y-6">
          {items.map((item, index) => (
            <div
              key={item.id}
              className="border border-erp-border bg-white shadow-sm"
            >
              {/* Item Card Header */}
              <div className="flex flex-wrap items-center justify-between border-b border-erp-border bg-erp-surface-subtle p-3 sm:px-4">
                <span className="text-xs font-bold uppercase tracking-wider text-erp-navy">
                  {t("actions.itemNumber", { number: index + 1 })}
                </span>
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

              <div className="space-y-4 p-4 sm:p-5">
                {/* Item Name & Category */}
                <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
                  <div className="sm:col-span-2">
                    <label className="mb-1 block text-xs font-semibold text-erp-text-main">
                      {t("itemTitle")} <span className="text-erp-danger">*</span>
                    </label>
                    <input
                      type="text"
                      required
                      value={item.title}
                      onChange={(e) => handleUpdateItem(index, { title: e.target.value })}
                      disabled={isSaving}
                      placeholder={t("itemTitlePlaceholder")}
                      className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-xs font-medium text-erp-text-main focus:border-erp-navy focus:outline-none"
                    />
                  </div>
                  <div>
                    <label className="mb-1 block text-xs font-semibold text-erp-text-main">
                      {t("category")}
                    </label>
                    <select
                      value={item.category ?? "built_in"}
                      onChange={(e) => handleUpdateItem(index, { category: e.target.value })}
                      disabled={isSaving}
                      className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-xs font-medium text-erp-text-main focus:border-erp-navy focus:outline-none"
                    >
                      <option value="built_in">{t("categories.built_in")}</option>
                      <option value="loose">{t("categories.loose")}</option>
                      <option value="flooring">{t("categories.flooring")}</option>
                      <option value="wall_ceiling">{t("categories.wall_ceiling")}</option>
                      <option value="curtain">{t("categories.curtain")}</option>
                      <option value="mep">{t("categories.mep")}</option>
                    </select>
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
            </div>
          ))}
        </div>
      )}

      {/* Floating Bottom Action Bar for Mobile & Desktop */}
      <div className="fixed bottom-0 left-0 right-0 z-30 border-t border-erp-border bg-white/95 p-3 backdrop-blur-sm sm:px-8">
        <div className="mx-auto flex max-w-5xl items-center justify-between gap-3">
          <button
            type="button"
            onClick={handleAddItem}
            disabled={isSaving}
            className="inline-flex min-h-[44px] items-center gap-1.5 border border-erp-border bg-erp-surface-subtle px-3 py-2 text-xs font-semibold text-erp-navy hover:bg-erp-surface"
          >
            <IconPlus size={16} />
            <span className="hidden sm:inline">{t("addItem")}</span>
          </button>
          <div className="flex items-center gap-2">
            <button
              type="submit"
              disabled={isSaving || items.length === 0}
              className="inline-flex min-h-[44px] items-center gap-2 border border-erp-navy bg-erp-navy px-5 py-2 text-xs font-semibold text-white transition-colors hover:bg-erp-navy-hover active:bg-erp-navy-active disabled:cursor-not-allowed disabled:opacity-50"
            >
              {isSaving ? t("actions.saving") : t("actions.saveDraft")}
            </button>
          </div>
        </div>
      </div>
    </form>
  );
}
