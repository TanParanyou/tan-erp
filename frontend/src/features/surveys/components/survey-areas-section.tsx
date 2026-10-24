"use client";

import React, { useState } from "react";
import { useFormContext, useFieldArray } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { IconPlus } from "@/components/common/Icons";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { SurveyAreaCard } from "./survey-area-card";
import { SurveyBatchTemplateModal } from "./survey-batch-template-modal";
import type { SurveyWorkspaceFormData } from "../schemas/survey-workspace-schema";

interface SurveyAreasSectionProps {
  isReady: boolean;
  areaEvidenceEnabled: boolean;
}

export function SurveyAreasSection({ isReady, areaEvidenceEnabled }: SurveyAreasSectionProps) {
  const t = useTranslations("surveys");
  const tc = useTranslations("common");
  const { control, getValues } = useFormContext<SurveyWorkspaceFormData>();

  const { fields, append, remove } = useFieldArray({
    control,
    name: "areas",
  });

  const [areaToDeleteIndex, setAreaToDeleteIndex] = useState<number | null>(null);
  const [isBatchModalOpen, setIsBatchModalOpen] = useState(false);
  const [expandedMap, setExpandedMap] = useState<Record<number, boolean>>({});

  const handleToggleArea = (idx: number) => {
    setExpandedMap((prev) => ({
      ...prev,
      [idx]: !(prev[idx] ?? true),
    }));
  };

  const handleExpandAll = () => {
    const next: Record<number, boolean> = {};
    fields.forEach((_, idx) => {
      next[idx] = true;
    });
    setExpandedMap(next);
  };

  const handleCollapseAll = () => {
    const next: Record<number, boolean> = {};
    fields.forEach((_, idx) => {
      next[idx] = false;
    });
    setExpandedMap(next);
  };

  const handleAddArea = () => {
    const nextIdx = fields.length + 1;
    setExpandedMap((prev) => ({ ...prev, [fields.length]: true }));
    append({
      code: `AREA-${String(nextIdx).padStart(2, "0")}`,
      name: "",
      description: "",
      sortOrder: nextIdx,
      measurements: [
        {
          measurementType: "width",
          value: 1,
          unitCode: "m",
          captureMethod: "measured",
          notes: "",
          sortOrder: 1,
        },
      ],
    });
  };

  const handleBatchAddAreas = (selectedRooms: { code: string; name: string }[]) => {
    selectedRooms.forEach((room, i) => {
      const nextIdx = fields.length + i + 1;
      setExpandedMap((prev) => ({ ...prev, [fields.length + i]: true }));
      append({
        code: `AREA-${String(nextIdx).padStart(2, "0")}`,
        name: room.name,
        description: "",
        sortOrder: nextIdx,
        measurements: [
          {
            measurementType: "width",
            value: 1,
            unitCode: "m",
            captureMethod: "measured",
            notes: "",
            sortOrder: 1,
          },
        ],
      });
    });
  };

  const handleRequestRemoveArea = (index: number) => {
    const area = getValues(`areas.${index}`);
    // If area has a name or more than 1 measurement, require safety confirmation
    if (area?.name?.trim() || (area?.measurements && area.measurements.length > 1)) {
      setAreaToDeleteIndex(index);
    } else {
      remove(index);
    }
  };

  const handleConfirmRemoveArea = () => {
    if (areaToDeleteIndex !== null) {
      remove(areaToDeleteIndex);
      setAreaToDeleteIndex(null);
    }
  };

  const targetArea = areaToDeleteIndex !== null ? getValues(`areas.${areaToDeleteIndex}`) : null;

  return (
    <div className="pt-2">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-4">
        <h3 className="text-base font-bold text-erp-navy">
          {t("areasTitle")}
        </h3>
        <div className="flex items-center gap-2 flex-wrap">
          {fields.length > 1 && (
            <div className="flex items-center gap-1.5 mr-1">
              <Button
                variant="outline"
                size="sm"
                type="button"
                onClick={handleCollapseAll}
                className="text-xs min-h-[38px]"
                title={t("collapseAll")}
                aria-label={t("collapseAll")}
              >
                {t("collapseAll")}
              </Button>
              <Button
                variant="outline"
                size="sm"
                type="button"
                onClick={handleExpandAll}
                className="text-xs min-h-[38px]"
                title={t("expandAll")}
                aria-label={t("expandAll")}
              >
                {t("expandAll")}
              </Button>
            </div>
          )}

          {!isReady && (
            <>
              <Button
                variant="outline"
                size="sm"
                onClick={() => setIsBatchModalOpen(true)}
                type="button"
                className="text-xs min-h-[38px]"
              >
                {t("batchTemplateAction")}
              </Button>
              <Button
                variant="primary"
                size="sm"
                onClick={handleAddArea}
                type="button"
                className="flex items-center gap-1.5 min-h-[38px] font-semibold"
              >
                <IconPlus size={16} strokeWidth={2} />
                <span>{t("addAreaAction")}</span>
              </Button>
            </>
          )}
        </div>
      </div>

      {fields.length === 0 ? (
        <div className="p-8 text-center border-2 border-dashed border-erp-border bg-erp-surface-subtle/30 text-erp-text-muted text-sm">
          {t("readinessErrors.missingArea")}
        </div>
      ) : (
        <div className="space-y-4">
          {fields.map((fieldItem, aIdx) => (
            <SurveyAreaCard
              key={fieldItem.id}
              areaIndex={aIdx}
              isReady={isReady}
              areaEvidenceEnabled={areaEvidenceEnabled}
              onRemoveArea={() => handleRequestRemoveArea(aIdx)}
              isExpanded={expandedMap[aIdx] ?? true}
              onToggleExpand={() => handleToggleArea(aIdx)}
            />
          ))}
        </div>
      )}

      {/* Safety Confirmation Modal for Deleting Area */}
      <ConfirmationModal
        isOpen={areaToDeleteIndex !== null}
        onClose={() => setAreaToDeleteIndex(null)}
        onConfirm={handleConfirmRemoveArea}
        title={t("confirmDeleteAreaTitle")}
        message={t("confirmDeleteAreaDesc", {
          name: targetArea?.name || targetArea?.code || "",
        })}
        variant="danger"
        confirmText={tc("actions.delete")}
        cancelText={tc("actions.cancel")}
      />

      {/* Batch Template Modal */}
      <SurveyBatchTemplateModal
        isOpen={isBatchModalOpen}
        onClose={() => setIsBatchModalOpen(false)}
        onConfirm={handleBatchAddAreas}
      />
    </div>
  );
}
