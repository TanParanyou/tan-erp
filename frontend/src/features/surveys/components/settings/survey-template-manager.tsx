"use client";

import React, { useMemo, useState } from "react";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Drawer } from "@/components/ui/Drawer";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import {
  IconPlus,
  IconRefresh,
  IconClose,
  IconCheck,
  IconAlertTriangle,
} from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import {
  useSurveyTemplateVersions,
  usePublishSurveyTemplateVersion,
  useRetireSurveyTemplateVersion,
} from "../../api/survey-queries";
import type {
  SurveyTemplateVersionResponse,
  PublishSurveyTemplateVersionRequest,
} from "@/lib/api/api-client";

interface NewTemplateFormState {
  code: string;
  requiredChecklistItems: string[];
  minimumEvidenceCount: number;
  minimumAreaEvidenceCount: number;
  snapshotHashVersion: string;
  effectiveFromUtc: string;
}

const initialFormState: NewTemplateFormState = {
  code: "",
  requiredChecklistItems: [
    "ตรวจสอบโครงสร้างและจุดรับน้ำหนัก",
    "ตรวจสอบจุดจ่ายกระแสไฟฟ้าและเมนเบรกเกอร์",
    "ตรวจสอบเส้นทางขนส่งและระยะทางเดินท่อ",
  ],
  minimumEvidenceCount: 1,
  minimumAreaEvidenceCount: 1,
  snapshotHashVersion: "v4",
  effectiveFromUtc: "",
};

export function SurveyTemplateManager() {
  const t = useTranslations("surveyTemplates");
  const tCommon = useTranslations("common");
  const { toast } = useToast();

  const [isDrawerOpen, setIsDrawerOpen] = useState(false);
  const [formState, setFormState] = useState<NewTemplateFormState>(initialFormState);
  const [newChecklistInput, setNewChecklistInput] = useState("");
  const [validationError, setValidationError] = useState<string | null>(null);

  // Retire Confirmation Modal state
  const [templateToRetire, setTemplateToRetire] = useState<SurveyTemplateVersionResponse | null>(null);

  // Queries & Mutations
  const {
    data: templateData,
    isLoading,
    isError,
    refetch,
  } = useSurveyTemplateVersions();

  const publishMutation = usePublishSurveyTemplateVersion();
  const retireMutation = useRetireSurveyTemplateVersion();

  const isSaving = publishMutation.isPending || retireMutation.isPending;

  const handleOpenPublishDrawer = () => {
    setFormState(initialFormState);
    setNewChecklistInput("");
    setValidationError(null);
    setIsDrawerOpen(true);
  };

  const handleCloseDrawer = () => {
    if (isSaving) return;
    setIsDrawerOpen(false);
    setValidationError(null);
  };

  const handleAddChecklistItem = () => {
    const trimmed = newChecklistInput.trim();
    if (!trimmed) return;
    if (formState.requiredChecklistItems.includes(trimmed)) return;
    setFormState((prev) => ({
      ...prev,
      requiredChecklistItems: [...prev.requiredChecklistItems, trimmed],
    }));
    setNewChecklistInput("");
  };

  const handleRemoveChecklistItem = (indexToRemove: number) => {
    setFormState((prev) => ({
      ...prev,
      requiredChecklistItems: prev.requiredChecklistItems.filter((_, idx) => idx !== indexToRemove),
    }));
  };

  const handlePublishSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formState.code.trim()) {
      setValidationError(t("messages.codeRequired"));
      return;
    }
    if (formState.requiredChecklistItems.length === 0) {
      setValidationError(t("messages.checklistRequired"));
      return;
    }
    setValidationError(null);

    const payload: PublishSurveyTemplateVersionRequest = {
      code: formState.code.trim().toUpperCase(),
      requiredChecklistItems: formState.requiredChecklistItems,
      minimumEvidenceCount: formState.minimumEvidenceCount,
      minimumAreaEvidenceCount: formState.minimumAreaEvidenceCount,
      snapshotHashVersion: formState.snapshotHashVersion,
      effectiveFromUtc: formState.effectiveFromUtc.trim()
        ? new Date(formState.effectiveFromUtc).toISOString()
        : new Date().toISOString(),
    };

    try {
      await publishMutation.mutateAsync(payload);
      toast.success(t("messages.publishSuccess"));
      setIsDrawerOpen(false);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : t("messages.publishError");
      toast.error(message);
    }
  };

  const handleConfirmRetire = async () => {
    if (!templateToRetire?.code) return;
    try {
      await retireMutation.mutateAsync(templateToRetire.code);
      toast.success(t("messages.retireSuccess"));
      setTemplateToRetire(null);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : t("messages.retireError");
      toast.error(message);
    }
  };

  const columns = useMemo<Column<SurveyTemplateVersionResponse>[]>(
    () => [
      {
        id: "code",
        header: t("table.code"),
        className: "min-w-[140px]",
        cell: (_val, item) => (
          <div className="flex items-center gap-2">
            <span className="font-mono font-bold text-erp-navy">{item.code || "-"}</span>
            {item.isCurrent && (
              <Badge variant="primary" size="sm">
                {t("status.current")}
              </Badge>
            )}
          </div>
        ),
      },
      {
        id: "hashVersion",
        header: t("table.hashVersion"),
        className: "w-28 text-center",
        cell: (_val, item) => (
          <Badge variant="neutral" size="sm" className="font-mono">
            {item.snapshotHashVersion || "-"}
          </Badge>
        ),
      },
      {
        id: "evidenceRequirements",
        header: t("table.evidenceRequirements"),
        className: "min-w-[180px]",
        cell: (_val, item) => (
          <div className="flex flex-col text-xs text-erp-text-main gap-0.5">
            <span>
              {t("labels.minTotalEvidence")}:{" "}
              <strong className="text-erp-navy font-mono">{item.minimumEvidenceCount ?? 0}</strong>
            </span>
            <span>
              {t("labels.minAreaEvidence")}:{" "}
              <strong className="text-erp-navy font-mono">{item.minimumAreaEvidenceCount ?? 0}</strong>
            </span>
          </div>
        ),
      },
      {
        id: "checklist",
        header: t("table.checklistItems"),
        className: "min-w-[240px]",
        cell: (_val, item) => {
          const checklist = item.requiredChecklistItems ?? [];
          return (
            <div className="flex flex-col gap-1 max-w-md">
              <span className="text-xs font-semibold text-erp-text-muted">
                {checklist.length} {t("labels.items")}
              </span>
              <ul className="text-xs text-erp-text-main list-disc list-inside space-y-0.5">
                {checklist.slice(0, 3).map((chk, idx) => (
                  <li key={idx} className="truncate" title={chk}>
                    {chk}
                  </li>
                ))}
                {checklist.length > 3 && (
                  <li className="text-erp-text-muted list-none pl-1">
                    +{checklist.length - 3} {t("labels.moreItems")}
                  </li>
                )}
              </ul>
            </div>
          );
        },
      },
      {
        id: "effectivePeriod",
        header: t("table.effectivePeriod"),
        className: "min-w-[200px]",
        cell: (_val, item) => {
          const from = item.effectiveFromUtc
            ? new Date(item.effectiveFromUtc).toLocaleDateString("th-TH")
            : "-";
          const to = item.effectiveToUtc
            ? new Date(item.effectiveToUtc).toLocaleDateString("th-TH")
            : t("labels.indefinite");
          return (
            <div className="flex flex-col text-xs font-mono">
              <span className="text-erp-text-main">{from} -</span>
              <span className="text-erp-text-muted">{to}</span>
            </div>
          );
        },
      },
      {
        id: "status",
        header: t("table.status"),
        className: "w-28 text-center",
        cell: (_val, item) => {
          const isRetired = Boolean(item.effectiveToUtc && new Date(item.effectiveToUtc) <= new Date());
          return (
            <Badge variant={isRetired ? "neutral" : "success"} size="sm">
              {isRetired ? t("status.retired") : t("status.active")}
            </Badge>
          );
        },
      },
      {
        id: "actions",
        header: t("table.actions"),
        isAction: true,
        className: "w-28 text-right",
        cell: (_val, item) => {
          const isRetired = Boolean(item.effectiveToUtc && new Date(item.effectiveToUtc) <= new Date());
          if (isRetired) {
            return (
              <span className="text-xs text-erp-text-muted select-none">
                {t("labels.retired")}
              </span>
            );
          }
          return (
            <Button
              variant="outline"
              size="sm"
              onClick={() => setTemplateToRetire(item)}
              className="text-erp-destructive border-erp-destructive/30 hover:bg-erp-destructive/10"
              title={t("actions.retire")}
            >
              <span>{t("actions.retire")}</span>
            </Button>
          );
        },
      },
    ],
    [t]
  );

  return (
    <div className="flex flex-col gap-4">
      {/* Action Toolbar */}
      <div className="flex items-center justify-between gap-4">
        <div>
          <h2 className="text-base font-semibold text-erp-text-main">
            {t("sectionTitle")}
          </h2>
          <p className="text-xs text-erp-text-muted mt-0.5">
            {t("sectionDescription")}
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => refetch()}
            title={tCommon("actions.refresh")}
            disabled={isLoading}
          >
            <IconRefresh size={16} />
            <span>{tCommon("actions.refresh")}</span>
          </Button>
          <Button
            variant="primary"
            size="sm"
            onClick={handleOpenPublishDrawer}
            icon={<IconPlus size={16} />}
          >
            <span>{t("actions.publishNew")}</span>
          </Button>
        </div>
      </div>

      {/* Table */}
      <DataTable<SurveyTemplateVersionResponse>
        columns={columns}
        data={templateData?.items ?? []}
        isLoading={isLoading}
        isError={isError}
        error={isError ? t("messages.loadError") : null}
        onRetry={() => refetch()}
        hidePagination={true}
      />

      {/* Publish New Template Drawer */}
      <Drawer
        isOpen={isDrawerOpen}
        onClose={handleCloseDrawer}
        title={t("drawer.title")}
        size="lg"
        footer={
          <div className="flex items-center justify-end gap-3 w-full">
            <Button
              variant="outline"
              size="sm"
              onClick={handleCloseDrawer}
              disabled={isSaving}
            >
              <span>{tCommon("actions.cancel")}</span>
            </Button>
            <Button
              variant="primary"
              size="sm"
              onClick={handlePublishSubmit}
              isLoading={isSaving}
              disabled={isSaving}
              icon={<IconCheck size={16} />}
            >
              <span>{t("actions.publishConfirm")}</span>
            </Button>
          </div>
        }
      >
        <form onSubmit={handlePublishSubmit} className="flex flex-col gap-5 py-2">
          {validationError && (
            <div className="flex items-start gap-2 p-3 bg-erp-destructive/10 border border-erp-destructive/30 text-erp-destructive text-sm">
              <IconAlertTriangle size={18} className="shrink-0 mt-0.5" />
              <span>{validationError}</span>
            </div>
          )}

          <div className="space-y-1.5">
            <label className="text-sm font-semibold text-erp-text-main">
              {t("fields.code")} <span className="text-erp-destructive">*</span>
            </label>
            <Input
              value={formState.code}
              onChange={(e) =>
                setFormState((prev) => ({ ...prev, code: e.target.value.toUpperCase() }))
              }
              placeholder={t("placeholders.code")}
              className="font-mono h-11 min-h-[44px]"
              disabled={isSaving}
            />
            <p className="text-xs text-erp-text-muted">{t("hints.code")}</p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div className="space-y-1.5">
              <label className="text-sm font-semibold text-erp-text-main">
                {t("fields.hashVersion")}
              </label>
              <select
                value={formState.snapshotHashVersion}
                onChange={(e) =>
                  setFormState((prev) => ({
                    ...prev,
                    snapshotHashVersion: e.target.value,
                  }))
                }
                className="w-full h-11 min-h-[44px] px-3 border border-erp-border bg-erp-surface text-sm text-erp-text-main focus:outline-none focus:border-erp-navy font-mono"
                disabled={isSaving}
              >
                <option value="v4">v4 (Area evidence)</option>
                <option value="v3">v3 (Baseline)</option>
                <option value="v2">v2 (Measurements)</option>
              </select>
            </div>

            <div className="space-y-1.5">
              <label className="text-sm font-semibold text-erp-text-main">
                {t("fields.minEvidence")}
              </label>
              <Input
                type="number"
                min={0}
                max={20}
                value={formState.minimumEvidenceCount}
                onChange={(e) =>
                  setFormState((prev) => ({
                    ...prev,
                    minimumEvidenceCount: Number(e.target.value),
                  }))
                }
                className="h-11 min-h-[44px]"
                disabled={isSaving}
              />
            </div>

            <div className="space-y-1.5">
              <label className="text-sm font-semibold text-erp-text-main">
                {t("fields.minAreaEvidence")}
              </label>
              <Input
                type="number"
                min={0}
                max={20}
                value={formState.minimumAreaEvidenceCount}
                onChange={(e) =>
                  setFormState((prev) => ({
                    ...prev,
                    minimumAreaEvidenceCount: Number(e.target.value),
                  }))
                }
                className="h-11 min-h-[44px]"
                disabled={isSaving}
              />
            </div>
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-semibold text-erp-text-main">
              {t("fields.effectiveFromUtc")}
            </label>
            <Input
              type="datetime-local"
              value={formState.effectiveFromUtc}
              onChange={(e) =>
                setFormState((prev) => ({ ...prev, effectiveFromUtc: e.target.value }))
              }
              className="h-11 min-h-[44px]"
              disabled={isSaving}
            />
            <p className="text-xs text-erp-text-muted">{t("hints.effectiveFrom")}</p>
          </div>

          {/* Checklist items dynamic builder */}
          <div className="space-y-2 border-t border-erp-border pt-4">
            <label className="text-sm font-semibold text-erp-text-main">
              {t("fields.checklistItems")} <span className="text-erp-destructive">*</span>
            </label>
            <div className="flex gap-2">
              <Input
                value={newChecklistInput}
                onChange={(e) => setNewChecklistInput(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    handleAddChecklistItem();
                  }
                }}
                placeholder={t("placeholders.checklistItem")}
                className="h-11 min-h-[44px] flex-1"
                disabled={isSaving}
              />
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={handleAddChecklistItem}
                disabled={!newChecklistInput.trim() || isSaving}
                className="h-11 min-h-[44px] px-4"
              >
                <span>{tCommon("actions.add")}</span>
              </Button>
            </div>

            <div className="flex flex-col gap-2 mt-3 max-h-60 overflow-y-auto">
              {formState.requiredChecklistItems.map((item, idx) => (
                <div
                  key={idx}
                  className="flex items-center justify-between p-2.5 bg-erp-surface-subtle border border-erp-border text-sm"
                >
                  <span className="text-erp-text-main break-all">{item}</span>
                  <button
                    type="button"
                    onClick={() => handleRemoveChecklistItem(idx)}
                    className="text-erp-text-muted hover:text-erp-destructive p-1"
                    title={tCommon("actions.delete")}
                    disabled={isSaving}
                  >
                    <IconClose size={14} />
                  </button>
                </div>
              ))}
            </div>
          </div>
        </form>
      </Drawer>

      {/* Safety Confirmation Modal for Retiring a Template */}
      <Modal
        isOpen={Boolean(templateToRetire)}
        onClose={() => !isSaving && setTemplateToRetire(null)}
        title={t("retireModal.title")}
        description={t("retireModal.description", { code: templateToRetire?.code ?? "" })}
        footer={
          <div className="flex items-center justify-end gap-3 w-full">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setTemplateToRetire(null)}
              disabled={isSaving}
            >
              <span>{tCommon("actions.cancel")}</span>
            </Button>
            <Button
              variant="primary"
              size="sm"
              onClick={handleConfirmRetire}
              isLoading={isSaving}
              disabled={isSaving}
              className="bg-erp-destructive hover:bg-erp-destructive/90 text-white"
            >
              <span>{t("actions.retireConfirm")}</span>
            </Button>
          </div>
        }
      >
        <div className="flex items-start gap-3 p-3 bg-erp-destructive/10 border border-erp-destructive/20 text-sm text-erp-text-main">
          <IconAlertTriangle size={20} className="shrink-0 text-erp-destructive mt-0.5" />
          <p>{t("retireModal.warning")}</p>
        </div>
      </Modal>
    </div>
  );
}
