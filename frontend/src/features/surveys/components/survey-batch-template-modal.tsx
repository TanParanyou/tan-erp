"use client";

import React, { useState } from "react";
import { useTranslations, useLocale } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { useAreaPresets, type RoomPresetOption } from "../hooks/use-area-presets";

interface SurveyBatchTemplateModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (selectedRooms: { code: string; name: string }[]) => void;
}

export function SurveyBatchTemplateModal({
  isOpen,
  onClose,
  onConfirm,
}: SurveyBatchTemplateModalProps) {
  const t = useTranslations("surveys");
  const tc = useTranslations("common");
  const locale = useLocale();
  const { presets, packages } = useAreaPresets();

  const [selectedCodes, setSelectedCodes] = useState<string[]>([
    "living_room",
    "master_bedroom",
    "kitchen",
  ]);

  const handleSelectPackage = (pkgId: string) => {
    const pkg = packages.find((p) => p.id === pkgId);
    if (pkg) {
      setSelectedCodes(pkg.rooms);
    }
  };

  const handleToggleRoom = (code: string) => {
    setSelectedCodes((prev) =>
      prev.includes(code) ? prev.filter((c) => c !== code) : [...prev, code]
    );
  };

  const handleSelectAll = () => {
    setSelectedCodes(presets.map((p) => p.code));
  };

  const handleClearAll = () => {
    setSelectedCodes([]);
  };

  const handleConfirm = () => {
    const selectedRooms = selectedCodes.map((code) => {
      const preset = presets.find((p) => p.code === code);
      const name = preset
        ? locale === "en"
          ? preset.defaultNameEn
          : preset.defaultNameTh
        : code;
      return { code, name };
    });

    onConfirm(selectedRooms);
    onClose();
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={t("batchTemplateModal.title")}
      description={t("batchTemplateModal.description")}
      size="lg"
    >
      <div className="space-y-6">
        {/* Section 1: Quick Preset Packages */}
        <div className="space-y-2.5">
          <h4 className="text-xs font-bold text-erp-navy uppercase tracking-wider">
            {t("batchTemplateModal.quickPackages")}
          </h4>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
            {packages.map((pkg) => (
              <button
                key={pkg.id}
                type="button"
                onClick={() => handleSelectPackage(pkg.id)}
                className="text-left p-2.5 text-xs font-semibold border border-erp-border bg-erp-surface hover:bg-erp-surface-subtle hover:border-erp-navy transition-colors flex items-center justify-between"
              >
                <span>{t(pkg.labelKey as Parameters<typeof t>[0])}</span>
                <span className="text-erp-navy font-mono text-[11px] underline">
                  {tc("actions.select")}
                </span>
              </button>
            ))}
          </div>
        </div>

        {/* Section 2: Checklist */}
        <div className="space-y-2.5 border-t border-erp-border pt-4">
          <div className="flex items-center justify-between">
            <h4 className="text-xs font-bold text-erp-navy uppercase tracking-wider">
              {t("batchTemplateModal.selectRooms")}
            </h4>
            <div className="flex items-center gap-3 text-xs">
              <button
                type="button"
                onClick={handleSelectAll}
                className="text-erp-navy hover:underline font-semibold"
              >
                {t("batchTemplateModal.selectAll")}
              </button>
              <span className="text-erp-border">|</span>
              <button
                type="button"
                onClick={handleClearAll}
                className="text-erp-text-muted hover:underline"
              >
                {t("batchTemplateModal.clearAll")}
              </button>
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-2.5 max-h-64 overflow-y-auto p-2 border border-erp-border bg-erp-surface-subtle/30">
            {presets.map((preset: RoomPresetOption) => {
              const isChecked = selectedCodes.includes(preset.code);
              return (
                <label
                  key={preset.code}
                  className="flex items-center gap-2 p-1.5 hover:bg-erp-surface cursor-pointer text-xs font-medium text-erp-text-main select-none transition-colors"
                >
                  <Checkbox
                    checked={isChecked}
                    onChange={() => handleToggleRoom(preset.code)}
                  />
                  <span>{t(preset.labelKey as Parameters<typeof t>[0])}</span>
                </label>
              );
            })}
          </div>
        </div>

        {/* Modal Footer Actions */}
        <div className="flex items-center justify-end gap-3 pt-4 border-t border-erp-border">
          <Button variant="outline" onClick={onClose} type="button">
            {tc("actions.cancel")}
          </Button>
          <Button
            variant="primary"
            onClick={handleConfirm}
            disabled={selectedCodes.length === 0}
            type="button"
          >
            {t("batchTemplateModal.confirmAdd", { count: selectedCodes.length })}
          </Button>
        </div>
      </div>
    </Modal>
  );
}
