"use client";

import React, { useEffect, useId, useRef, useState, useCallback } from "react";
import { useTranslations } from "next-intl";
import {
  IconUpload,
  IconTrash,
  IconCamera,
  IconAlertCircle,
  IconEye,
} from "@/components/common/Icons";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { GalleryLightboxModal } from "@/components/common/GalleryLightboxModal";
import { cn } from "@/lib/utils/cn";

export interface AvatarUploadProps {
  file: File | null;
  existingFileId?: string | null;
  existingUrl?: string | null;
  onFileChange: (file: File | null) => void;
  displayName?: string;
  disabled?: boolean;
  uploadButtonText?: string;
  deleteButtonText?: string;
  hintText?: string;
  className?: string;
  maxFileSizeBytes?: number;
}

const DEFAULT_MAX_SIZE_BYTES = 5 * 1024 * 1024; // 5MB
const ACCEPTED_TYPES = ["image/png", "image/jpeg", "image/webp"];

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

export function AvatarUpload({
  file,
  existingFileId,
  existingUrl,
  onFileChange,
  displayName,
  disabled = false,
  uploadButtonText,
  deleteButtonText,
  hintText,
  className,
  maxFileSizeBytes = DEFAULT_MAX_SIZE_BYTES,
}: AvatarUploadProps) {
  const tUserAdmin = useTranslations("userAdmin");
  const tCommon = useTranslations("common");
  const inputId = useId();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isLightboxOpen, setIsLightboxOpen] = useState(false);

  useEffect(() => {
    if (!file) {
      setPreviewUrl(null);
      return;
    }
    const url = URL.createObjectURL(file);
    setPreviewUrl(url);
    return () => URL.revokeObjectURL(url);
  }, [file]);

  const validateFile = useCallback(
    (selected: File): string | null => {
      if (!ACCEPTED_TYPES.includes(selected.type) && !selected.type.startsWith("image/")) {
        return tUserAdmin("fields.invalidFileTypeError");
      }
      if (selected.size > maxFileSizeBytes) {
        return tUserAdmin("fields.fileTooLargeError");
      }
      return null;
    },
    [maxFileSizeBytes, tUserAdmin]
  );

  const handleProcessFile = useCallback(
    (selected: File) => {
      if (disabled) return;
      const error = validateFile(selected);
      if (error) {
        setErrorMessage(error);
        return;
      }
      setErrorMessage(null);
      onFileChange(selected);
    },
    [disabled, onFileChange, validateFile]
  );

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const selected = event.target.files?.[0];
    if (selected) {
      handleProcessFile(selected);
    }
  };

  const handleRemove = (e: React.MouseEvent) => {
    e.stopPropagation();
    onFileChange(null);
    setErrorMessage(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    if (disabled) return;
    setIsDragging(true);
  };

  const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
    if (disabled) return;
    const droppedFile = e.dataTransfer.files?.[0];
    if (droppedFile) {
      handleProcessFile(droppedFile);
    }
  };

  const initial = displayName?.trim().charAt(0).toUpperCase();
  const hasImage = Boolean(previewUrl || existingFileId || existingUrl);
  const activeImageUrl = previewUrl || existingUrl || undefined;

  const resolvedUploadText = uploadButtonText ?? tUserAdmin("fields.uploadAvatar");
  const resolvedDeleteText = deleteButtonText ?? tCommon("actions.delete");
  const resolvedHintText = hintText ?? tUserAdmin("fields.avatarHint");
  const resolvedPreviewText = tCommon("form.imagePreview");

  return (
    <div className={cn("flex flex-col sm:flex-row items-start sm:items-center gap-4", className)}>
      <input
        id={inputId}
        ref={fileInputRef}
        type="file"
        accept="image/png,image/jpeg,image/webp"
        className="hidden"
        disabled={disabled}
        onChange={handleFileSelect}
      />

      {/* Avatar Container Tile */}
      <div
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        className={cn(
          "relative w-28 h-28 sm:w-32 sm:h-32 shrink-0 overflow-hidden select-none transition-all duration-200 group border",
          isDragging
            ? "border-2 border-dashed border-erp-navy bg-erp-navy-50/90 text-erp-navy"
            : hasImage
              ? "border-erp-border bg-erp-surface-subtle shadow-xs"
              : "border-dashed border-erp-border hover:border-erp-navy/70 bg-erp-surface-subtle/40 hover:bg-erp-surface-subtle/70"
        )}
      >
        {/* Active Drag & Drop Visual State */}
        {isDragging ? (
          <div className="absolute inset-0 flex flex-col items-center justify-center p-2 text-center bg-erp-navy-50/95 text-erp-navy z-30">
            <IconUpload size={24} className="animate-bounce mb-1 text-erp-navy" />
            <span className="text-[11px] font-semibold leading-tight">
              {tUserAdmin("fields.dropAvatarHere")}
            </span>
          </div>
        ) : null}

        {hasImage ? (
          <>
            {/* Clickable Image: Opens Lightbox Preview */}
            <button
              type="button"
              onClick={() => setIsLightboxOpen(true)}
              aria-label={displayName ?? tUserAdmin("fields.avatarPreview")}
              title={resolvedPreviewText}
              className="w-full h-full p-0 m-0 border-0 bg-transparent cursor-pointer block overflow-hidden focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-erp-navy focus-visible:ring-inset"
            >
              {previewUrl ? (
                // eslint-disable-next-line @next/next/no-img-element
                <img
                  src={previewUrl}
                  alt={displayName ?? tUserAdmin("fields.avatarPreview")}
                  className="w-full h-full object-cover"
                />
              ) : existingFileId ? (
                <AuthenticatedFileImage
                  fileId={existingFileId}
                  alt={displayName ?? tUserAdmin("fields.avatarPreview")}
                  className="w-full h-full object-cover"
                />
              ) : existingUrl ? (
                // eslint-disable-next-line @next/next/no-img-element
                <img
                  src={existingUrl}
                  alt={displayName ?? tUserAdmin("fields.avatarPreview")}
                  className="w-full h-full object-cover"
                />
              ) : null}
            </button>

            {/* Subtle Eye indicator badge in bottom-right corner when idle */}
            <div
              className="absolute bottom-1.5 right-1.5 z-10 w-6 h-6 bg-erp-surface/90 border border-erp-border text-erp-slate-500 flex items-center justify-center shadow-xs pointer-events-none group-hover:opacity-0 transition-opacity"
              aria-hidden="true"
            >
              <IconEye size={13} strokeWidth={2} />
            </div>

            {/* In-Image Action Scrim: Appears on Hover / Focus */}
            {!disabled && (
              <div
                className={cn(
                  "absolute inset-0 z-20 flex flex-col items-center justify-center gap-1.5 p-2",
                  "bg-erp-navy-950/75 backdrop-blur-[1.5px] opacity-0 group-hover:opacity-100 group-focus-within:opacity-100 transition-opacity duration-200"
                )}
              >
                {/* Fullscreen Preview Button */}
                <button
                  type="button"
                  onClick={() => setIsLightboxOpen(true)}
                  aria-label={resolvedPreviewText}
                  className={cn(
                    "w-full py-1 px-2 bg-erp-surface/90 hover:bg-white text-erp-navy",
                    "text-xs font-semibold shadow-xs flex items-center justify-center gap-1.5",
                    "border border-white/20 transition-transform active:scale-95 cursor-pointer",
                    "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white"
                  )}
                >
                  <IconEye size={13} strokeWidth={2} className="shrink-0" />
                  <span className="truncate">{resolvedPreviewText}</span>
                </button>

                {/* Change / Upload Button */}
                <button
                  type="button"
                  onClick={() => fileInputRef.current?.click()}
                  aria-label={resolvedUploadText}
                  className={cn(
                    "w-full py-1 px-2 bg-erp-navy hover:bg-erp-navy-hover text-white",
                    "text-xs font-semibold shadow-xs flex items-center justify-center gap-1.5",
                    "border border-erp-navy-700 transition-transform active:scale-95 cursor-pointer",
                    "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white"
                  )}
                >
                  <IconCamera size={13} strokeWidth={2} className="shrink-0" />
                  <span className="truncate">{resolvedUploadText}</span>
                </button>

                {/* Delete Button */}
                <button
                  type="button"
                  onClick={handleRemove}
                  aria-label={resolvedDeleteText}
                  className={cn(
                    "w-full py-1 px-2 bg-erp-danger hover:bg-red-700 text-white",
                    "text-xs font-semibold shadow-xs flex items-center justify-center gap-1.5",
                    "transition-transform active:scale-95 cursor-pointer",
                    "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white"
                  )}
                >
                  <IconTrash size={13} strokeWidth={2} className="shrink-0" />
                  <span className="truncate">{resolvedDeleteText}</span>
                </button>
              </div>
            )}
          </>
        ) : (
          /* Empty / Initial State: Fully Clickable Blueprint Tile */
          <button
            type="button"
            disabled={disabled}
            onClick={() => fileInputRef.current?.click()}
            aria-label={resolvedUploadText}
            className={cn(
              "w-full h-full flex flex-col items-center justify-center p-2.5 text-center gap-2 group/empty transition-colors cursor-pointer",
              "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-erp-navy focus-visible:ring-inset",
              disabled && "cursor-not-allowed opacity-60"
            )}
          >
            {initial ? (
              <div className="w-10 h-10 border border-erp-border bg-gradient-to-br from-erp-surface to-erp-surface-subtle flex items-center justify-center text-erp-navy font-bold font-mono text-xl group-hover/empty:border-erp-navy transition-colors">
                {initial}
              </div>
            ) : (
              <div className="w-10 h-10 border border-erp-border bg-erp-surface flex items-center justify-center text-erp-slate-400 group-hover/empty:text-erp-navy group-hover/empty:border-erp-navy transition-colors">
                <IconCamera size={20} strokeWidth={1.8} />
              </div>
            )}

            <div className="flex flex-col items-center gap-0.5 max-w-full">
              <span className="text-xs font-semibold text-erp-text-main group-hover/empty:text-erp-navy transition-colors truncate max-w-full">
                {resolvedUploadText}
              </span>
              <span className="text-[10px] text-erp-text-muted">
                {tUserAdmin("fields.clickToUpload")}
              </span>
            </div>
          </button>
        )}
      </div>

      {/* Supporting Metadata & Feedback (Zero clutter: hints in sr-only) */}
      <div className="flex flex-col gap-1.5 min-w-0 justify-center">
        {/* Staged new file chip */}
        {file && (
          <div className="flex items-center gap-2 px-2.5 py-1 bg-erp-navy-50/80 border border-erp-navy-200 text-xs text-erp-navy w-fit max-w-full">
            <span className="w-1.5 h-1.5 bg-emerald-500 shrink-0" aria-hidden="true" />
            <span className="font-mono text-[11px] truncate max-w-[180px] sm:max-w-[240px] font-medium" title={file.name}>
              {file.name}
            </span>
            <span className="text-erp-slate-500 font-mono text-[11px] shrink-0">
              ({formatFileSize(file.size)})
            </span>
            <span className="inline-flex items-center px-1.5 py-0.5 bg-emerald-100 text-emerald-800 text-[10px] font-semibold shrink-0">
              {tUserAdmin("fields.pendingSave")}
            </span>
          </div>
        )}

        {/* Client validation error */}
        {errorMessage && (
          <div className="flex items-center gap-1.5 text-xs text-erp-danger font-medium" role="alert">
            <IconAlertCircle size={14} className="shrink-0" />
            <span>{errorMessage}</span>
          </div>
        )}

        {/* Accessible hint text kept for screen readers and test compliance without visual noise */}
        <p className="sr-only">{resolvedHintText}</p>
      </div>

      {/* Full Architectural Lightbox via GalleryLightboxModal */}
      {hasImage && isLightboxOpen && (
        <GalleryLightboxModal
          isOpen={isLightboxOpen}
          onClose={() => setIsLightboxOpen(false)}
          items={[
            {
              id: "avatar-preview",
              imageUrl: activeImageUrl,
              fileId: (!previewUrl && !existingUrl && existingFileId) ? existingFileId : undefined,
              caption: displayName ?? resolvedPreviewText,
            },
          ]}
          currentIndex={0}
          onIndexChange={() => {}}
          title={displayName ?? resolvedPreviewText}
        />
      )}
    </div>
  );
}
