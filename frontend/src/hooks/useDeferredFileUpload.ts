import { useState, useRef, useCallback } from "react";
import { fileClient } from "@/lib/api/file-client";
import { ApiError } from "@/lib/api/api-error";
import type { RequestOptions } from "@/lib/api/api-client";
import type { AttachmentOwnerType } from "@/lib/attachments/attachment-owner-types";

export type FileParentType =
  | "customer"
  | "site"
  | "opportunity"
  | "item"
  | "item-category"
  | "item-brand"
  | "costRecord"
  | AttachmentOwnerType;

export interface FileItemToUpload {
  file: File;
  caption?: string;
}

export interface UploadedFileResult {
  fileId: string;
  filename: string;
  mediaType: string;
  fileSizeBytes: number;
  servingUrl: string;
  caption?: string;
}

export interface UseDeferredFileUploadOptions {
  parentType: FileParentType;
  parentId?: string | null;
}

export interface UploadExecutionOptions {
  token: string;
  membershipId?: string | null;
  locale?: string;
  signal?: AbortSignal;
}

export interface UploadResult {
  uploadIntentId: string | null;
  files: UploadedFileResult[];
  primaryFileId?: string;
}

/**
 * Enterprise reusable hook for the 3-step deferred file upload protocol.
 *
 * Enforces:
 * 1. Proper parent binding (parentType, parentId, creationIntentId)
 * 2. Upload intent lifecycle (reuse on retry, rotate on resetIntent)
 * 3. Exact slot ID mapping during completion
 * 4. Error mapping and loading state tracking
 */
export function useDeferredFileUpload({
  parentType,
  parentId = null,
}: UseDeferredFileUploadOptions) {
  const [isUploading, setIsUploading] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);

  // One intent ID per unpersisted parent creation intent
  const creationIntentIdRef = useRef<string | null>(null);

  const resetIntent = useCallback(() => {
    creationIntentIdRef.current = null;
    setUploadError(null);
  }, []);

  const uploadFiles = useCallback(
    async (
      items: (File | FileItemToUpload)[],
      options: UploadExecutionOptions
    ): Promise<UploadResult> => {
      if (items.length === 0) {
        return {
          uploadIntentId: creationIntentIdRef.current,
          files: [],
        };
      }

      const { token, membershipId, locale = "th", signal } = options;
      const normalizedLocale: "th" | "en" = locale === "en" ? "en" : "th";

      setIsUploading(true);
      setUploadError(null);

      try {
        const normalizedItems: FileItemToUpload[] = items.map((item) =>
          item instanceof File ? { file: item } : item
        );

        const hasExistingParent = Boolean(parentId);
        if (!hasExistingParent) {
          creationIntentIdRef.current ??= crypto.randomUUID();
        }

        const effectiveIntentId = hasExistingParent
          ? null
          : creationIntentIdRef.current;

        // Step 1: Create parent-bound upload session
        const sessionPayload = {
          parentType,
          parentId: parentId ?? null,
          creationIntentId: effectiveIntentId,
          files: normalizedItems.map((item) => ({
            filename: item.file.name,
            mediaType: item.file.type || "image/webp",
            fileSizeBytes: item.file.size,
          })),
        };

        const sessionOptions: RequestOptions = {
          token,
          membershipId: membershipId || undefined,
          idempotencyKey: effectiveIntentId
            ? `file-sess-${effectiveIntentId}`
            : `file-sess-${parentId}-${Date.now()}`,
          locale: normalizedLocale,
          signal,
        };

        const sessionRes = await fileClient.createSession(
          sessionPayload,
          sessionOptions
        );

        if (!sessionRes.sessionId || !sessionRes.slots?.length) {
          throw new Error("Failed to create file upload session: missing session or slots.");
        }

        if (sessionRes.slots.length !== normalizedItems.length) {
          throw new Error("Slot count mismatch returned from upload session.");
        }

        // Step 2 & 3: Map slot IDs and complete upload session with file binaries
        const filesWithSlots = sessionRes.slots.map((slot, index) => {
          const slotId = slot.slotId;
          if (!slotId) {
            throw new Error(`Invalid or missing slot ID at index ${index}.`);
          }
          return {
            slotId,
            file: normalizedItems[index].file,
          };
        });

        const completeRes = await fileClient.completeSession(
          sessionRes.sessionId,
          filesWithSlots,
          {
            token,
            membershipId: membershipId || undefined,
            locale: normalizedLocale,
            signal,
          }
        );

        if (!completeRes.files || completeRes.files.length === 0) {
          throw new Error("No files were verified by upload completion.");
        }

        const uploadedResults: UploadedFileResult[] = completeRes.files
          .filter((cf) => Boolean(cf.fileId))
          .map((cf, index) => ({
            fileId: cf.fileId ?? "",
            filename: cf.filename ?? normalizedItems[index]?.file.name ?? "",
            mediaType: cf.mediaType ?? normalizedItems[index]?.file.type ?? "image/webp",
            fileSizeBytes: cf.fileSizeBytes ?? normalizedItems[index]?.file.size ?? 0,
            servingUrl: cf.servingUrl ?? "",
            caption: normalizedItems[index]?.caption?.trim() || undefined,
          }));

        return {
          uploadIntentId: effectiveIntentId,
          files: uploadedResults,
          primaryFileId: uploadedResults[0]?.fileId,
        };
      } catch (err: unknown) {
        const errorMsg =
          err instanceof ApiError
            ? err.message
            : err instanceof Error
            ? err.message
            : "Upload failed unexpectedly.";
        setUploadError(errorMsg);
        throw err;
      } finally {
        setIsUploading(false);
      }
    },
    [parentType, parentId]
  );

  const uploadSingleFile = useCallback(
    async (
      file: File | null | undefined,
      options: UploadExecutionOptions
    ): Promise<{ uploadIntentId: string | null; fileId?: string }> => {
      if (!file) {
        return {
          uploadIntentId: creationIntentIdRef.current,
          fileId: undefined,
        };
      }

      const res = await uploadFiles([file], options);
      return {
        uploadIntentId: res.uploadIntentId,
        fileId: res.primaryFileId,
      };
    },
    [uploadFiles]
  );

  return {
    uploadFiles,
    uploadSingleFile,
    resetIntent,
    intentId: creationIntentIdRef.current,
    isUploading,
    uploadError,
  };
}
