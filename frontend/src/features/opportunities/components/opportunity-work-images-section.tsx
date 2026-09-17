"use client";

import React, { useState } from "react";
import Image from "next/image";
import { IconUpload, IconEye, IconAlertCircle, IconCamera } from "@/components/common/Icons";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { cn } from "@/lib/utils/cn";
import {
  useOpportunityWorkImages,
  useDetachWorkImage,
} from "../api/opportunity-queries";
import { OpportunityWorkImageAttachModal } from "./opportunity-work-image-attach-modal";
import { resolveOpportunityStageLabel } from "../opportunity-labels";
import { useTranslations } from "next-intl";
import { fileClient } from "@/lib/api/file-client";
import type { OpportunityWorkImageResponse } from "@/lib/api/api-client";

export interface OpportunityWorkImagesSectionProps {
  opportunityId: string;
  currentStage: string;
  rowVersion: string;
  canManage: boolean;
}

/**
 * Opportunity Work Images Section Component:
 * - Displays gallery of attached work photos with captions and stage badges
 * - Lightbox modal for full-size image preview
 * - Filter by stage
 * - Soft-detach with confirmation modal (ETag protected)
 * - Attach button opening deferred upload modal
 * - Atelier Architectural Navy Sharp design compliant
 */
export function OpportunityWorkImagesSection({
  opportunityId,
  currentStage,
  rowVersion,
  canManage,
}: OpportunityWorkImagesSectionProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common.actions");

  const [selectedStageFilter, setSelectedStageFilter] = useState<string | null>(null);
  const [isAttachModalOpen, setIsAttachModalOpen] = useState(false);
  const [previewImage, setPreviewImage] = useState<OpportunityWorkImageResponse | null>(null);
  const [imageToDetach, setImageToDetach] = useState<OpportunityWorkImageResponse | null>(null);

  // Fetch all images for this opportunity (unfiltered by stage to keep tabs and counts stable)
  const { data, isLoading, isError } = useOpportunityWorkImages(opportunityId);

  const detachMutation = useDetachWorkImage();

  const isClosed =
    currentStage === "won" || currentStage === "lost" || currentStage === "cancelled";

  const handleDetach = async () => {
    if (!imageToDetach || !imageToDetach.id) return;

    try {
      const idempotencyKey = `detach-${Date.now()}-${Math.random().toString(36).substring(2, 10)}`;
      await detachMutation.mutateAsync({
        opportunityId,
        imageId: imageToDetach.id,
        expectedVersion: rowVersion,
        idempotencyKey,
      });
      setImageToDetach(null);
    } catch {
      // Error handled by mutation state
    }
  };

  const allImages = data?.items ?? [];
  const filteredImages = selectedStageFilter
    ? allImages.filter((img) => img.stageAtAttach === selectedStageFilter)
    : allImages;

  const stageCounts: Record<string, number> = {};
  for (const img of allImages) {
    if (img.stageAtAttach) {
      stageCounts[img.stageAtAttach] = (stageCounts[img.stageAtAttach] ?? 0) + 1;
    }
  }

  const STAGES = ["draft", "qualified", "surveying", "estimating", "proposed"] as const;

  return (
    <div className="erp-card p-6 flex flex-col gap-5">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-erp-border pb-4">
        <div className="flex flex-col gap-1">
          <h3 className="text-base font-bold text-erp-navy">
            {t("workImagesTitle")}
          </h3>
          <p className="text-xs text-erp-text-muted">
            {t("workImagesSubtitle")}
          </p>
        </div>

        {/* Action: Attach Work Images */}
        {canManage && !isClosed && (
          <Button
            type="button"
            variant="primary"
            size="sm"
            onClick={() => setIsAttachModalOpen(true)}
            icon={<IconUpload size={16} />}
            className="self-start sm:self-auto"
          >
            {t("attachImagesAction")}
          </Button>
        )}
      </div>

      {/* Filter Tabs / Stage Filter - Always visible if there are any images in the opportunity */}
      {allImages.length > 0 && (
        <div className="flex items-center gap-2 overflow-x-auto pb-1 text-xs">
          <span className="font-mono text-erp-text-muted mr-1 uppercase">
            {t("filterByStage")}:
          </span>
          <button
            type="button"
            onClick={() => setSelectedStageFilter(null)}
            className={cn(
              "px-2.5 py-1 text-xs font-mono tracking-wider transition-colors border rounded-none whitespace-nowrap",
              selectedStageFilter === null
                ? "bg-erp-navy text-white border-erp-navy"
                : "bg-white dark:bg-erp-slate-900 text-erp-text-muted border-erp-border hover:text-erp-text-main"
            )}
          >
            {t("allStages")} ({allImages.length})
          </button>
          {STAGES.map((stage) => {
            const count = stageCounts[stage] ?? 0;
            return (
              <button
                key={stage}
                type="button"
                onClick={() => setSelectedStageFilter(stage)}
                className={cn(
                  "px-2.5 py-1 text-xs font-mono tracking-wider transition-colors border rounded-none whitespace-nowrap",
                  selectedStageFilter === stage
                    ? "bg-erp-navy text-white border-erp-navy"
                    : "bg-white dark:bg-erp-slate-900 text-erp-text-muted border-erp-border hover:text-erp-text-main"
                )}
              >
                {resolveOpportunityStageLabel(stage, t)} ({count})
              </button>
            );
          })}
        </div>
      )}

      {/* Gallery Grid */}
      {isLoading ? (
        <div className="py-12 flex flex-col items-center justify-center gap-2 text-xs font-mono text-erp-text-muted">
          <MonoSpinner size="md" />
          <span>{t("loadingWorkImages")}</span>
        </div>
      ) : isError ? (
        <div className="py-8 flex flex-col items-center justify-center gap-2 text-xs font-mono text-destructive">
          <IconAlertCircle size={20} />
          <span>{t("loadWorkImagesError")}</span>
        </div>
      ) : allImages.length === 0 ? (
        <div className="py-12 px-6 border border-dashed border-erp-slate-300 dark:border-erp-slate-700 bg-erp-slate-50/60 dark:bg-erp-slate-900/30 text-center flex flex-col items-center justify-center gap-3">
          <div className="w-12 h-12 flex items-center justify-center bg-white dark:bg-erp-slate-800 border border-erp-border text-erp-navy dark:text-erp-slate-300">
            <IconCamera size={24} strokeWidth={1.6} />
          </div>
          <div className="space-y-1 max-w-md">
            <p className="text-sm font-semibold text-erp-text-main">{t("workImagesEmpty")}</p>
            <p className="text-xs text-erp-text-muted leading-relaxed">
              {t("workImagesEmptyHint")}
            </p>
          </div>
          {canManage && !isClosed && (
            <Button
              type="button"
              variant="secondary"
              size="sm"
              onClick={() => setIsAttachModalOpen(true)}
              icon={<IconUpload size={14} />}
              className="mt-1"
            >
              {t("attachImagesAction")}
            </Button>
          )}
        </div>
      ) : filteredImages.length === 0 ? (
        <div className="py-10 px-6 border border-dashed border-erp-slate-300 dark:border-erp-slate-700 bg-erp-slate-50/40 dark:bg-erp-slate-900/20 text-center flex flex-col items-center justify-center gap-3">
          <p className="text-xs font-mono text-erp-text-muted">
            {t("workImagesEmptyInStage")}
          </p>
          <div className="flex items-center gap-2">
            <Button
              type="button"
              variant="secondary"
              size="sm"
              onClick={() => setSelectedStageFilter(null)}
            >
              {t("viewAllStagesAction")} ({allImages.length})
            </Button>
            {canManage && !isClosed && (
              <Button
                type="button"
                variant="primary"
                size="sm"
                onClick={() => setIsAttachModalOpen(true)}
                icon={<IconUpload size={14} />}
              >
                {t("attachImagesAction")}
              </Button>
            )}
          </div>
        </div>
      ) : (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
          {filteredImages.map((image) => (
            <div
              key={image.id}
              className="border border-erp-border bg-white dark:bg-erp-slate-900 group relative flex flex-col justify-between overflow-hidden shadow-none rounded-none hover:border-erp-navy transition-colors"
            >
              {/* Thumbnail Container */}
              <div
                className="relative aspect-video w-full bg-erp-slate-100 dark:bg-erp-slate-800 cursor-pointer overflow-hidden"
                onClick={() => setPreviewImage(image)}
              >
                <Image
                  src={fileClient.getFileUrl(image.fileId)}
                  alt={image.caption || "Work photo"}
                  fill
                  unoptimized
                  className="object-cover transition-transform group-hover:scale-105"
                />

                {/* Stage Badge */}
                <div className="absolute top-1 left-1 px-1.5 py-0.5 bg-erp-navy/90 text-[10px] font-mono text-white uppercase rounded-none">
                  {image.stageAtAttach}
                </div>

                {/* Hover overlay with Eye icon */}
                <div className="absolute inset-0 bg-erp-navy/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                  <span className="p-1.5 bg-white text-erp-navy rounded-none shadow-sm">
                    <IconEye size={16} />
                  </span>
                </div>
              </div>

              {/* Caption & Metadata Footer */}
              <div className="p-3 space-y-2 text-xs bg-erp-slate-50/70 dark:bg-erp-slate-950/40 border-t border-erp-border flex-1 flex flex-col justify-between">
                <p className="text-erp-text-main line-clamp-2 text-xs font-medium" title={image.caption ?? undefined}>
                  {image.caption || <span className="text-erp-text-muted italic">-</span>}
                </p>
                <div className="flex items-center justify-between text-[10px] font-mono text-erp-text-muted pt-1.5 border-t border-erp-border/60">
                  <span className="truncate">{image.createdBy?.displayName ?? "-"}</span>
                  {canManage && !isClosed && (
                    <button
                      type="button"
                      onClick={(e) => {
                        e.stopPropagation();
                        setImageToDetach(image);
                      }}
                      className="text-destructive hover:underline ml-2 uppercase font-semibold"
                      title={tCommon("delete")}
                    >
                      {tCommon("delete")}
                    </button>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Lightbox Preview Modal */}
      {previewImage && (
        <Modal
          isOpen={Boolean(previewImage)}
          onClose={() => setPreviewImage(null)}
          title={previewImage.caption || t("viewOriginalImage")}
          size="xl"
        >
          <div className="space-y-4">
            <div className="relative aspect-video w-full bg-erp-navy/95 flex items-center justify-center overflow-hidden border border-erp-border">
              <Image
                src={fileClient.getFileUrl(previewImage.fileId)}
                alt={previewImage.caption || "Full preview"}
                fill
                unoptimized
                className="object-contain"
              />
            </div>
            <div className="flex flex-col sm:flex-row sm:items-center justify-between text-xs font-mono text-erp-text-muted gap-2 pt-2 border-t border-erp-border">
              <div>
                <span>{t("stageBadge")}: </span>
                <span className="font-bold text-erp-text-main uppercase">{previewImage.stageAtAttach}</span>
              </div>
              <div>
                <span>{t("uploadedBy")}: </span>
                <span className="text-erp-text-main">{previewImage.createdBy?.displayName ?? "-"}</span>
                <span className="mx-2">•</span>
                <span>{previewImage.createdAtUtc ? new Date(previewImage.createdAtUtc).toLocaleString() : "-"}</span>
              </div>
            </div>
          </div>
        </Modal>
      )}

      {/* Safety Confirmation Modal for Detach */}
      {imageToDetach && (
        <Modal
          isOpen={Boolean(imageToDetach)}
          onClose={() => setImageToDetach(null)}
          title={t("detachImageConfirmTitle")}
        >
          <div className="space-y-4">
            <p className="text-xs text-erp-text-main">
              {t("detachImageConfirmMessage")}
            </p>
            {detachMutation.isError && (
              <div className="p-3 bg-destructive/10 border border-destructive text-destructive text-xs font-mono">
                {detachMutation.error.message}
              </div>
            )}
            <div className="flex justify-end gap-3 pt-4 border-t border-erp-border">
              <Button
                type="button"
                variant="secondary"
                size="sm"
                onClick={() => setImageToDetach(null)}
                disabled={detachMutation.isPending}
              >
                {tCommon("cancel")}
              </Button>
              <Button
                type="button"
                variant="danger"
                size="sm"
                onClick={handleDetach}
                isLoading={detachMutation.isPending}
              >
                {detachMutation.isPending ? t("detachingImage") : tCommon("delete")}
              </Button>
            </div>
          </div>
        </Modal>
      )}

      {/* Attach Modal */}
      {isAttachModalOpen && (
        <OpportunityWorkImageAttachModal
          isOpen={isAttachModalOpen}
          onClose={() => setIsAttachModalOpen(false)}
          opportunityId={opportunityId}
          expectedVersion={rowVersion}
        />
      )}
    </div>
  );
}
