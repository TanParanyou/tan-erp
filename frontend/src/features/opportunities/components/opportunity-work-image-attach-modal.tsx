"use client";

import React, { useState } from "react";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { IconUpload } from "@/components/common/Icons";
import { MultiImagePicker, type PendingImageItem } from "@/components/forms/MultiImagePicker";
import { fileClient } from "@/lib/api/file-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useAttachWorkImages } from "../api/opportunity-queries";
import { useTranslations } from "next-intl";

export interface OpportunityWorkImageAttachModalProps {
  isOpen: boolean;
  onClose: () => void;
  opportunityId: string;
  expectedVersion: string;
}

/**
 * Modal for attaching work images to an Opportunity.
 * Follows the 3-step deferred upload protocol:
 * 1. User picks/captures/compresses images (MultiImagePicker - local only)
 * 2. On submit: Create file session -> Complete upload session -> Attach work images to Opportunity (all-or-nothing)
 * 3. Rotates opportunity rowVersion and invalidates queries
 */
export function OpportunityWorkImageAttachModal({
  isOpen,
  onClose,
  opportunityId,
  expectedVersion,
}: OpportunityWorkImageAttachModalProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common.actions");
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  const [pendingItems, setPendingItems] = useState<PendingImageItem[]>([]);
  const [uploadedFileMap, setUploadedFileMap] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const attachMutation = useAttachWorkImages();

  const handleClose = () => {
    if (isSubmitting) return;
    setPendingItems([]);
    setUploadedFileMap({});
    setErrorMessage(null);
    onClose();
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (pendingItems.length === 0 || isSubmitting) return;

    // Check if any image is still being optimized
    const stillOptimizing = pendingItems.some((i) => i.isOptimizing);
    if (stillOptimizing) {
      setErrorMessage("Please wait for all images to finish optimizing.");
      return;
    }

    setIsSubmitting(true);
    setErrorMessage(null);

    try {
      const token = await getAuthToken();
      if (!token || !membershipId) {
        throw new Error("Authentication and active membership required.");
      }

      // Filter only items that haven't been uploaded and verified yet
      const itemsToUpload = pendingItems.filter((i) => !uploadedFileMap[i.id]);
      const currentMap = { ...uploadedFileMap };

      if (itemsToUpload.length > 0) {
        const filesToUpload = itemsToUpload.map(
          (i) => i.optimizedFile ?? i.originalFile
        );

        // 1. Create upload session with parent binding
        const createSessionReq = {
          parentType: "opportunity",
          parentId: opportunityId,
          creationIntentId: null,
          files: filesToUpload.map((f) => ({
            filename: f.name,
            mediaType: f.type || "image/webp",
            fileSizeBytes: f.size,
          })),
        };

        const idempotencyKey = `file-sess-${Date.now()}-${Math.random().toString(36).substring(2, 10)}`;

        const sessionRes = await fileClient.createSession(createSessionReq, {
          token,
          membershipId,
          idempotencyKey,
          locale: normalizedLocale,
        });

        if (!sessionRes.sessionId || !sessionRes.slots) {
          throw new Error("Failed to create file upload session.");
        }

        // 2. Complete session with actual binaries and slot mapping
        const filesWithSlots = sessionRes.slots.map((slot, idx) => ({
          slotId: slot.slotId ?? "",
          file: filesToUpload[idx],
        }));

        const completeRes = await fileClient.completeSession(
          sessionRes.sessionId,
          filesWithSlots,
          {
            token,
            membershipId,
            locale: normalizedLocale,
          }
        );

        if (!completeRes.files || completeRes.files.length === 0) {
          throw new Error("File verification failed on server.");
        }

        completeRes.files.forEach((cf, idx) => {
          if (cf.fileId) {
            currentMap[itemsToUpload[idx].id] = cf.fileId;
          }
        });

        setUploadedFileMap(currentMap);
      }

      // 3. Attach verified file IDs to Opportunity
      const attachImagesPayload = pendingItems
        .map((item) => ({
          fileId: currentMap[item.id] ?? "",
          caption: item.caption?.trim() || undefined,
        }))
        .filter((item) => Boolean(item.fileId));

      if (attachImagesPayload.length !== pendingItems.length) {
        throw new Error("Some files could not be verified. Please retry.");
      }

      const attachIdempotencyKey = `opp-img-${Date.now()}-${Math.random().toString(36).substring(2, 10)}`;

      await attachMutation.mutateAsync({
        opportunityId,
        expectedVersion,
        images: attachImagesPayload,
        idempotencyKey: attachIdempotencyKey,
      });

      handleClose();
    } catch (err: unknown) {
      if (err instanceof Error) {
        setErrorMessage(err.message);
      } else {
        setErrorMessage("An unexpected error occurred during upload.");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title={t("attachImagesModalTitle")}
      size="xl"
    >
      <form onSubmit={handleSubmit} className="space-y-6">
        <p className="text-xs text-muted-foreground">{t("attachImagesModalDesc")}</p>

        <MultiImagePicker
          items={pendingItems}
          onChange={setPendingItems}
          disabled={isSubmitting}
          maxFiles={20}
          enableCamera={true}
        />

        {errorMessage && (
          <div className="p-3 bg-destructive/10 border border-destructive text-destructive text-xs font-mono">
            {errorMessage}
          </div>
        )}

        {/* Action Buttons (Atelier 44px md buttons) */}
        <div className="flex justify-end gap-3 pt-4 border-t border-erp-border">
          <Button
            type="button"
            variant="secondary"
            size="md"
            onClick={handleClose}
            disabled={isSubmitting}
          >
            {tCommon("cancel")}
          </Button>
          <Button
            type="submit"
            variant="primary"
            size="md"
            disabled={pendingItems.length === 0 || isSubmitting}
            isLoading={isSubmitting}
            icon={<IconUpload size={16} />}
          >
            {isSubmitting ? t("attachingImages") : `${t("attachImagesAction")} (${pendingItems.length})`}
          </Button>
        </div>
      </form>
    </Modal>
  );
}
