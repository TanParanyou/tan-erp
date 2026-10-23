"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import type {
  FieldSurveyItem,
  FieldSurveyFormData,
} from "../../schemas/field-survey-schema";
import { FieldSurveyMeasurementPad } from "./field-survey-measurement-pad";
import { FieldSurveyCatalogPicker } from "./field-survey-catalog-picker";
import { FieldSurveyItemPhotoManager } from "./field-survey-item-photo-manager";
import { RoomNameCombobox } from "./room-name-combobox";
import { OpportunityAutocomplete } from "@/features/opportunities/components/opportunity-autocomplete";
import {
  IconPlus,
  IconTrash,
  IconCheckCircle,
  IconChevronDown,
  IconCamera,
} from "@/components/common/Icons";

export interface FieldSurveyWorkspaceProps {
  initialOpportunityId?: string;
  onSave?: (data: FieldSurveyFormData) => Promise<void>;
}

export function FieldSurveyWorkspace({
  initialOpportunityId = "",
  onSave,
}: FieldSurveyWorkspaceProps) {
  const t = useTranslations("fieldSurvey");

  const [opportunityId, setOpportunityId] = useState<string>(initialOpportunityId);
  const [roomName, setRoomName] = useState<string>("");
  const [surveyorNotes, setSurveyorNotes] = useState<string>("");
  const [items, setItems] = useState<FieldSurveyItem[]>([]);
  const [collapsedItemIds, setCollapsedItemIds] = useState<Set<string>>(new Set());
  const [isSaving, setIsSaving] = useState<boolean>(false);
  const [saveSuccess, setSaveSuccess] = useState<boolean>(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

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

  const totalPhotosCount = items.reduce((acc, it) => acc + (it.photos?.length ?? 0), 0);

  return (
    <form onSubmit={handleSubmit} className="mx-auto max-w-5xl space-y-6 pb-24">
      {/* Page Header */}
      <div className="border border-erp-border bg-white p-5 shadow-sm">
        <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-xl font-bold text-erp-navy">{t("title")}</h1>
            <p className="mt-1 text-xs text-erp-text-muted">{t("subtitle")}</p>
          </div>
          {/* Quick Metrics Badge */}
          {items.length > 0 && (
            <div className="flex items-center gap-3 border border-erp-border bg-erp-surface-subtle px-3 py-1.5 text-xs font-semibold text-erp-navy">
              <span>{t("itemsCount", { count: items.length })}</span>
              <span className="text-erp-border">|</span>
              <span className="flex items-center gap-1">
                <IconCamera size={14} />
                {t("totalPhotos", { count: totalPhotosCount })}
              </span>
            </div>
          )}
        </div>

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

      {/* Quick Anchor Navigation Chips on Mobile/Tablet */}
      {items.length > 1 && (
        <div className="sticky top-0 z-20 flex items-center gap-2 overflow-x-auto border border-erp-border bg-white/95 px-3 py-2 shadow-sm backdrop-blur-sm scrollbar-none">
          <span className="shrink-0 text-[11px] font-semibold text-erp-text-muted">
            ข้ามไปยัง:
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

      {/* Items Section Header */}
      <div className="flex flex-wrap items-center justify-between gap-2">
        <div className="flex items-center gap-3">
          <h2 className="text-base font-bold text-erp-navy">{t("itemsSectionTitle")}</h2>
          <span className="text-xs text-erp-text-muted">
            ({t("itemsCount", { count: items.length })})
          </span>
        </div>

        <div className="flex items-center gap-2">
          {items.length > 0 && (
            <div className="flex items-center gap-1">
              <button
                type="button"
                onClick={handleCollapseAll}
                className="inline-flex min-h-[36px] items-center border border-erp-border bg-white px-2.5 text-[11px] font-medium text-erp-text-muted hover:text-erp-navy"
              >
                {t("collapseAll")}
              </button>
              <button
                type="button"
                onClick={handleExpandAll}
                className="inline-flex min-h-[36px] items-center border border-erp-border bg-white px-2.5 text-[11px] font-medium text-erp-text-muted hover:text-erp-navy"
              >
                {t("expandAll")}
              </button>
            </div>
          )}
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
        <div className="space-y-4">
          {items.map((item, index) => {
            const isCollapsed = collapsedItemIds.has(item.id);

            // Summary text for collapsed card
            let dimensionSummary = "";
            if (item.measurementType === "box" && item.boxDimensions) {
              const { widthMm, depthMm, heightMm } = item.boxDimensions;
              if (widthMm || depthMm || heightMm) {
                dimensionSummary = `${widthMm} × ${depthMm} × ${heightMm} มม.`;
              }
            } else if (item.measurementType === "area" && item.areaDimensions) {
              const { grossWidthMm, grossLengthMm } = item.areaDimensions;
              if (grossWidthMm && grossLengthMm) {
                dimensionSummary = `พื้นที่ ${(grossWidthMm * grossLengthMm / 1000000).toFixed(2)} ตร.ม.`;
              }
            } else if (item.measurementType === "custom" && item.customMeasurements) {
              dimensionSummary = `${item.customMeasurements.length} จุดวัด`;
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
                          (ยังไม่ได้ระบุชื่อ)
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
                )}
              </div>
            );
          })}
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
