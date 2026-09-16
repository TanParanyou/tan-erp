"use client";

import React, { useState } from "react";
import Image from "next/image";
import { IconUpload, IconClose, IconEye, IconAlertCircle } from "@/components/common/Icons";
import { Modal } from "@/components/ui/Modal";
import {
  useOpportunityWorkImages,
  useDetachWorkImage,
} from "../api/opportunity-queries";
import { OpportunityWorkImageAttachModal } from "./opportunity-work-image-attach-modal";
import { useTranslations } from "next-intl";
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

  const { data, isLoading, isError } = useOpportunityWorkImages(
    opportunityId,
    selectedStageFilter
  );

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

  const images = data?.items ?? [];

  return (
    <div className="border border-border bg-card p-6 space-y-6" style={{ borderRadius: "0px" }}>
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-border pb-4">
        <div>
          <h2 className="text-sm font-mono font-bold uppercase tracking-wider text-foreground">
            {t("workImagesTitle")}
          </h2>
          <p className="text-xs text-muted-foreground mt-0.5">
            {t("workImagesSubtitle")}
          </p>
        </div>

        {/* Action: Attach Work Images */}
        {canManage && !isClosed && (
          <button
            type="button"
            onClick={() => setIsAttachModalOpen(true)}
            className="inline-flex items-center gap-2 px-3.5 py-1.5 text-xs font-mono uppercase tracking-wider bg-primary hover:bg-primary/90 text-primary-foreground transition-colors self-start sm:self-auto"
            style={{ borderRadius: "0px" }}
          >
            <IconUpload className="w-3.5 h-3.5" />
            <span>{t("attachImagesAction")}</span>
          </button>
        )}
      </div>

      {/* Filter Tabs / Stage Filter */}
      {images.length > 0 && (
        <div className="flex items-center gap-2 overflow-x-auto pb-1">
          <span className="text-xs font-mono text-muted-foreground mr-1">
            {t("filterByStage")}:
          </span>
          <button
            type="button"
            onClick={() => setSelectedStageFilter(null)}
            className={`px-2.5 py-1 text-xs font-mono uppercase tracking-wider transition-colors border ${
              selectedStageFilter === null
                ? "bg-primary text-primary-foreground border-primary"
                : "bg-muted text-muted-foreground border-border hover:text-foreground"
            }`}
            style={{ borderRadius: "0px" }}
          >
            {t("allStages")}
          </button>
          {["draft", "qualified", "surveying", "estimating", "proposed"].map((stage) => (
            <button
              key={stage}
              type="button"
              onClick={() => setSelectedStageFilter(stage)}
              className={`px-2.5 py-1 text-xs font-mono uppercase tracking-wider transition-colors border ${
                selectedStageFilter === stage
                  ? "bg-primary text-primary-foreground border-primary"
                  : "bg-muted text-muted-foreground border-border hover:text-foreground"
              }`}
              style={{ borderRadius: "0px" }}
            >
              {stage}
            </button>
          ))}
        </div>
      )}

      {/* Gallery Grid */}
      {isLoading ? (
        <div className="py-12 text-center text-xs font-mono text-muted-foreground">
          Loading work images...
        </div>
      ) : isError ? (
        <div className="py-8 text-center text-xs font-mono text-destructive">
          Failed to load work images.
        </div>
      ) : images.length === 0 ? (
        <div className="py-12 border border-dashed border-border text-center space-y-2">
          <p className="text-xs text-muted-foreground">{t("workImagesEmpty")}</p>
        </div>
      ) : (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
          {images.map((image) => (
            <div
              key={image.id}
              className="border border-border bg-card group relative flex flex-col justify-between overflow-hidden"
              style={{ borderRadius: "0px" }}
            >
              {/* Thumbnail Container */}
              <div
                className="relative aspect-video w-full bg-muted cursor-pointer overflow-hidden"
                onClick={() => setPreviewImage(image)}
              >
                <Image
                  src={`/api/v1/files/${image.fileId}`}
                  alt={image.caption || "Work photo"}
                  fill
                  unoptimized
                  className="object-cover transition-transform group-hover:scale-105"
                />

                {/* Stage Badge */}
                <div className="absolute top-1 left-1 px-1.5 py-0.5 bg-black/75 text-[10px] font-mono text-white uppercase">
                  {image.stageAtAttach}
                </div>

                {/* Hover overlay with Eye icon */}
                <div className="absolute inset-0 bg-black/30 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                  <span className="p-1.5 bg-black/70 text-white rounded-none">
                    <IconEye className="w-4 h-4" />
                  </span>
                </div>
              </div>

              {/* Caption & Metadata Footer */}
              <div className="p-2.5 space-y-1.5 text-xs bg-background/50 border-t border-border flex-1 flex flex-col justify-between">
                <p className="text-foreground line-clamp-2 text-xs font-medium">
                  {image.caption || <span className="text-muted-foreground italic">-</span>}
                </p>
                <div className="flex items-center justify-between text-[10px] font-mono text-muted-foreground pt-1 border-t border-border/50">
                  <span className="truncate">{image.createdBy?.displayName ?? "-"}</span>
                  {canManage && !isClosed && (
                    <button
                      type="button"
                      onClick={(e) => {
                        e.stopPropagation();
                        setImageToDetach(image);
                      }}
                      className="text-destructive hover:underline ml-2 uppercase"
                      title="Detach"
                    >
                      Detach
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
            <div className="relative aspect-video w-full bg-black flex items-center justify-center overflow-hidden border border-border">
              <Image
                src={`/api/v1/files/${previewImage.fileId}`}
                alt={previewImage.caption || "Full preview"}
                fill
                unoptimized
                className="object-contain"
              />
            </div>
            <div className="flex flex-col sm:flex-row sm:items-center justify-between text-xs font-mono text-muted-foreground gap-2 pt-2 border-t border-border">
              <div>
                <span>{t("stageBadge")}: </span>
                <span className="font-bold text-foreground uppercase">{previewImage.stageAtAttach}</span>
              </div>
              <div>
                <span>{t("uploadedBy")}: </span>
                <span className="text-foreground">{previewImage.createdBy?.displayName ?? "-"}</span>
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
            <p className="text-xs text-foreground">
              {t("detachImageConfirmMessage")}
            </p>
            {detachMutation.isError && (
              <div className="p-3 bg-destructive/10 border border-destructive text-destructive text-xs font-mono">
                {detachMutation.error.message}
              </div>
            )}
            <div className="flex justify-end gap-3 pt-4 border-t border-border">
              <button
                type="button"
                onClick={() => setImageToDetach(null)}
                disabled={detachMutation.isPending}
                className="px-4 py-2 text-xs font-mono uppercase tracking-wider bg-muted text-foreground border border-border"
                style={{ borderRadius: "0px" }}
              >
                {tCommon("cancel")}
              </button>
              <button
                type="button"
                onClick={handleDetach}
                disabled={detachMutation.isPending}
                className="px-5 py-2 text-xs font-mono uppercase tracking-wider bg-destructive text-destructive-foreground disabled:opacity-50"
                style={{ borderRadius: "0px" }}
              >
                {detachMutation.isPending ? t("detachingImage") : tCommon("save")}
              </button>
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
