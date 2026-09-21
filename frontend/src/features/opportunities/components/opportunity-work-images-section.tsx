"use client";

import React, { useState } from "react";
import { IconUpload, IconAlertCircle, IconCamera } from "@/components/common/Icons";
import { Button } from "@/components/ui/Button";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { EmptyState } from "@/components/ui/EmptyState";
import { Badge } from "@/components/ui/Badge";
import { GalleryLightboxModal } from "@/components/common/GalleryLightboxModal";
import { useGalleryLightbox } from "@/hooks/useGalleryLightbox";
import { useToast } from "@/hooks/useToast";
import {
  useInfiniteOpportunityWorkImages,
  useDetachWorkImage,
} from "../api/opportunity-queries";
import { OpportunityWorkImageAttachModal } from "./opportunity-work-image-attach-modal";
import { OpportunityWorkImageCard } from "./opportunity-work-image-card";
import { OpportunityWorkImageStageFilter } from "./opportunity-work-image-stage-filter";
import { resolveOpportunityStageLabel } from "../opportunity-labels";
import { useTranslations } from "next-intl";
import type { OpportunityWorkImageResponse } from "@/lib/api/api-client";

export interface OpportunityWorkImagesSectionProps {
  opportunityId: string;
  currentStage: string;
  rowVersion: string;
  canManage: boolean;
}

const CANONICAL_STAGES = ["draft", "qualified", "surveying", "estimating", "proposed"] as const;

/**
 * Opportunity Work Images Section Component:
 * - Displays gallery of attached work photos with captions and localized stage badges
 * - Full Carousel Gallery Lightbox with Metadata Sidebar (using central useGalleryLightbox & GalleryLightboxModal)
 * - Stage filter tabs with live counts
 * - Safe detach flow with central ConfirmationModal and toast notification
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
  const { toast } = useToast();

  const [selectedStageFilter, setSelectedStageFilter] = useState<string | null>(null);
  const [isAttachModalOpen, setIsAttachModalOpen] = useState(false);
  const [imageToDetach, setImageToDetach] = useState<OpportunityWorkImageResponse | null>(null);

  // Fetch all images for this opportunity using cursor pagination
  const {
    data,
    isLoading,
    isError,
    hasNextPage,
    fetchNextPage,
    isFetchingNextPage,
  } = useInfiniteOpportunityWorkImages(opportunityId);
  const detachMutation = useDetachWorkImage();

  const isClosed =
    currentStage === "won" || currentStage === "lost" || currentStage === "cancelled";

  const allImages: OpportunityWorkImageResponse[] = (
    data?.pages.flatMap((page) => page.items ?? []) ?? []
  ).filter((img): img is OpportunityWorkImageResponse => Boolean(img));
  const filteredImages = selectedStageFilter
    ? allImages.filter((img) => img.stageAtAttach === selectedStageFilter)
    : allImages;

  // Central Carousel Gallery Lightbox hook
  const lightbox = useGalleryLightbox(filteredImages);

  const stageCounts: Record<string, number> = {};
  for (const img of allImages) {
    if (img.stageAtAttach) {
      stageCounts[img.stageAtAttach] = (stageCounts[img.stageAtAttach] ?? 0) + 1;
    }
  }

  // Active stages that have at least 1 image
  const availableStages = CANONICAL_STAGES.filter((stage) => (stageCounts[stage] ?? 0) > 0);

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
      toast.success(t("detachImageSuccess"));
      setImageToDetach(null);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : t("loadWorkImagesError");
      toast.error(msg);
    }
  };

  return (
    <div className="erp-card p-6 flex flex-col gap-5">
      {/* Header with Title and Action */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 border-b border-erp-border pb-4">
        <div className="flex flex-col gap-1">
          <div className="flex items-center gap-2">
            <h3 className="text-base font-bold text-erp-navy">
              {t("workImagesTitle")}
            </h3>
            <span className="px-1.5 py-0.5 text-[11px] font-mono font-semibold bg-erp-surface-muted text-erp-navy border border-erp-border">
              {allImages.length}
            </span>
          </div>
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
            icon={<IconUpload size={14} />}
            className="h-8 text-xs self-start sm:self-auto"
          >
            {t("attachImagesAction")}
          </Button>
        )}
      </div>

      {/* Stage Filter Component - Always visible */}
      <OpportunityWorkImageStageFilter
        selectedStage={selectedStageFilter}
        onSelectStage={setSelectedStageFilter}
        totalCount={allImages.length}
        stageCounts={stageCounts}
      />

      {/* Gallery Grid / Loading / Error / Empty States */}
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
        <EmptyState
          icon={<IconCamera size={28} className="text-erp-navy dark:text-erp-slate-300" />}
          title={t("workImagesEmpty")}
          description={t("workImagesEmptyHint")}
          actionLabel={canManage && !isClosed ? t("attachImagesAction") : undefined}
          onAction={canManage && !isClosed ? () => setIsAttachModalOpen(true) : undefined}
        />
      ) : filteredImages.length === 0 ? (
        <EmptyState
          icon="search"
          title={t("workImagesEmptyInStage")}
          actionLabel={`${t("viewAllStagesAction")} (${allImages.length})`}
          onAction={() => setSelectedStageFilter(null)}
          secondaryActionLabel={canManage && !isClosed ? t("attachImagesAction") : undefined}
          onSecondaryAction={canManage && !isClosed ? () => setIsAttachModalOpen(true) : undefined}
        />
      ) : (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
          {filteredImages.map((image, idx) => (
            <OpportunityWorkImageCard
              key={image.id ?? image.fileId ?? `img-${idx}`}
              image={image}
              onClick={() => lightbox.openAt(idx)}
              canManage={canManage}
              isClosed={isClosed}
              onDetach={(img) => setImageToDetach(img)}
            />
          ))}
        </div>
      )}

      {/* Keyset Cursor Load More Button */}
      {hasNextPage && (
        <div className="flex justify-center pt-2">
          <Button
            type="button"
            variant="secondary"
            size="md"
            onClick={() => void fetchNextPage()}
            disabled={isFetchingNextPage}
            isLoading={isFetchingNextPage}
          >
            {isFetchingNextPage ? t("loadingMoreImages") : t("loadMoreImages")}
          </Button>
        </div>
      )}

      {/* Central Reusable Carousel Gallery Lightbox with Split Master-Detail & Thumbnails */}
      <GalleryLightboxModal
        isOpen={lightbox.isOpen}
        onClose={lightbox.close}
        currentIndex={lightbox.currentIndex}
        onIndexChange={lightbox.goTo}
        items={filteredImages.map((img, idx) => ({
          id: img.id ?? img.fileId ?? `item-${idx}`,
          fileId: img.fileId,
          caption: img.caption,
          stageBadge: (
            <Badge variant="primary" size="sm">
              {resolveOpportunityStageLabel(img.stageAtAttach, t)}
            </Badge>
          ),
          createdByName: img.createdBy?.displayName ?? "-",
          createdAtUtc: img.createdAtUtc,
          canDetach: canManage && !isClosed,
          onDetach: () => {
            lightbox.close();
            setImageToDetach(img);
          },
        }))}
      />

      {/* Central Safety Confirmation Modal for Detach */}
      <ConfirmationModal
        isOpen={Boolean(imageToDetach)}
        onClose={() => setImageToDetach(null)}
        onConfirm={handleDetach}
        title={t("detachImageConfirmTitle")}
        message={t("detachImageConfirmMessage")}
        confirmText={tCommon("delete")}
        cancelText={tCommon("cancel")}
        variant="danger"
        isLoading={detachMutation.isPending}
      />

      {/* Deferred Upload Attach Modal */}
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

export default OpportunityWorkImagesSection;
