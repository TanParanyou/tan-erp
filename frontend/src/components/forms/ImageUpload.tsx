"use client";

import React, { useState, useRef, useCallback, useEffect } from "react";
import {
  IconUpload,
  IconTrash,
  IconEye,
  IconAlertCircle,
  IconCamera,
} from "@/components/common/Icons";
import { optimizeImageToWebP } from "@/lib/media/image-optimization";
import {
  GalleryLightboxModal,
  type GalleryItemMetadata,
} from "@/components/common/GalleryLightboxModal";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";

export interface ImageUploadProps {
  label?: string;
  value?: string | File;
  onChange: (value: File | string | null) => void;
  className?: string;
  error?: string;
  maxDimension?: number;
  maxFileSizeBytes?: number;
  disabled?: boolean;
  onProcessingChange?: (processing: boolean) => void;
}

export function ImageUpload({
  label,
  value,
  onChange,
  className = "",
  error,
  maxDimension = 2048,
  maxFileSizeBytes = 15 * 1024 * 1024,
  disabled = false,
  onProcessingChange,
}: ImageUploadProps) {
  const t = useTranslations("common.actions");
  const tFeedback = useTranslations("common.feedback");
  const tForm = useTranslations("common.form");

  const [isLightboxOpen, setIsLightboxOpen] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [localPreview, setLocalPreview] = useState<string | null>(null);

  const fileInputRef = useRef<HTMLInputElement>(null);
  const localPreviewRef = useRef<string | null>(null);

  // Sync ref with localPreview
  useEffect(() => {
    localPreviewRef.current = localPreview;
  }, [localPreview]);

  // Clean up Object URL on unmount
  useEffect(() => {
    return () => {
      if (localPreviewRef.current) {
        URL.revokeObjectURL(localPreviewRef.current);
      }
    };
  }, []);

  const handleFile = useCallback(
    async (file: File) => {
      if (disabled) return;
      setUploadError(null);

      // Validate file type
      if (!file.type.startsWith("image/")) {
        setUploadError(tFeedback("invalidFileType"));
        return;
      }

      // Validate file size (15MB raw limit)
      if (file.size > maxFileSizeBytes) {
        setUploadError(tFeedback("fileTooLarge"));
        return;
      }

      onProcessingChange?.(true);
      try {
        const optimized = await optimizeImageToWebP(file, { maxDimension });
        const targetFile = optimized.file;
        if (targetFile.size > maxFileSizeBytes) {
          setUploadError(tFeedback("fileTooLarge"));
          return;
        }
        if (localPreviewRef.current) URL.revokeObjectURL(localPreviewRef.current);
        const objectUrl = URL.createObjectURL(targetFile);
        localPreviewRef.current = objectUrl;
        setLocalPreview(objectUrl);
        onChange(targetFile);
      } catch {
        setUploadError(tFeedback("operationFailed"));
      } finally {
        onProcessingChange?.(false);
      }
    },
    [disabled, maxDimension, maxFileSizeBytes, onChange, onProcessingChange, tFeedback]
  );

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      handleFile(files[0]);
    }
  };

  const handleRemove = (e?: React.MouseEvent) => {
    e?.stopPropagation();
    if (disabled) return;
    if (localPreview) {
      URL.revokeObjectURL(localPreview);
    }
    onChange(null);
    setLocalPreview(null);
    setUploadError(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const previewSrc =
    localPreview || (typeof value === "string" && value.trim() ? value.trim() : "");

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (disabled) return;
    setIsDragging(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (disabled) return;
    setIsDragging(false);
    const files = e.dataTransfer.files;
    if (files && files.length > 0) {
      handleFile(files[0]);
    }
  };

  const activeError = uploadError || error;

  return (
    <div className={cn("erp-form-group", className)}>
      <input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png,image/webp"
        disabled={disabled}
        className="hidden"
        style={{ display: "none" }}
        onChange={handleInputChange}
      />

      {label && <label className="erp-label">{label}</label>}

      {previewSrc ? (
        <div
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          className="relative w-32 h-32 shrink-0 self-start group overflow-hidden border border-erp-border bg-erp-surface-subtle shadow-xs select-none transition-all duration-200"
        >
          {/* Active Drag & Drop Visual State */}
          {isDragging && (
            <div className="absolute inset-0 z-30 flex flex-col items-center justify-center p-2 text-center bg-erp-navy-50/95 text-erp-navy border-2 border-dashed border-erp-navy">
              <IconUpload size={24} className="animate-bounce mb-1 text-erp-navy" />
              <span className="text-[11px] font-semibold leading-tight">
                {t("upload")}
              </span>
            </div>
          )}

          {/* Clickable Image Surface: Opens Lightbox Preview */}
          <button
            type="button"
            onClick={() => setIsLightboxOpen(true)}
            aria-label={label || tForm("imagePreview")}
            title={tForm("imagePreview")}
            className="w-full h-full p-0 m-0 border-0 bg-transparent cursor-pointer block overflow-hidden focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-erp-navy focus-visible:ring-inset"
          >
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img
              src={previewSrc}
              alt={tForm("imagePreview")}
              className="h-full w-full object-cover"
            />
          </button>

          {/* Subtle eye badge in bottom-right corner when idle */}
          <div
            className="absolute bottom-1.5 right-1.5 z-10 w-6 h-6 bg-erp-surface/90 border border-erp-border text-erp-slate-500 flex items-center justify-center shadow-xs pointer-events-none group-hover:opacity-0 transition-opacity"
            aria-hidden="true"
          >
            <IconEye size={13} strokeWidth={2} />
          </div>

          {/* In-Image Action Scrim: Icon-Only Buttons on Hover / Focus */}
          {!disabled && (
            <div
              className={cn(
                "absolute inset-0 z-20 flex items-center justify-center gap-2 p-2",
                "bg-erp-navy-950/75 backdrop-blur-[1.5px] opacity-0 group-hover:opacity-100 group-focus-within:opacity-100 transition-opacity duration-200"
              )}
            >
              {/* Preview Icon Button */}
              <button
                type="button"
                onClick={() => setIsLightboxOpen(true)}
                title={tForm("imagePreview")}
                aria-label={tForm("imagePreview")}
                className={cn(
                  "w-8 h-8 flex items-center justify-center bg-erp-surface text-erp-navy hover:bg-white",
                  "border border-white/30 shadow-xs transition-transform active:scale-95 cursor-pointer",
                  "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white"
                )}
              >
                <IconEye size={15} strokeWidth={2} />
              </button>

              {/* Change / Upload Icon Button */}
              <button
                type="button"
                onClick={() => fileInputRef.current?.click()}
                title={t("upload")}
                aria-label={t("upload")}
                className={cn(
                  "w-8 h-8 flex items-center justify-center bg-erp-navy hover:bg-erp-navy-hover text-white",
                  "border border-erp-navy-700 shadow-xs transition-transform active:scale-95 cursor-pointer",
                  "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white"
                )}
              >
                <IconCamera size={15} strokeWidth={2} />
              </button>

              {/* Delete Icon Button */}
              <button
                type="button"
                onClick={handleRemove}
                title={t("delete")}
                aria-label={t("delete")}
                className={cn(
                  "w-8 h-8 flex items-center justify-center bg-erp-danger hover:bg-red-700 text-white",
                  "shadow-xs transition-transform active:scale-95 cursor-pointer",
                  "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white"
                )}
              >
                <IconTrash size={15} strokeWidth={2} />
              </button>
            </div>
          )}
        </div>
      ) : (
        /* Empty State: Blueprint Drag & Drop Tile */
        <button
          type="button"
          disabled={disabled}
          onClick={() => fileInputRef.current?.click()}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          aria-label={label || t("upload")}
          className={cn(
            "flex flex-col items-center justify-center w-32 h-32 p-2.5 text-center gap-1.5 transition-all cursor-pointer group border border-dashed select-none",
            isDragging
              ? "border-2 border-dashed border-erp-navy bg-erp-navy-50/90 text-erp-navy"
              : "border-erp-border hover:border-erp-navy/70 bg-erp-surface-subtle/40 hover:bg-erp-surface-subtle/70",
            disabled && "cursor-not-allowed opacity-50",
            "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-erp-navy focus-visible:ring-inset"
          )}
        >
          <div className="w-10 h-10 border border-erp-border bg-erp-surface flex items-center justify-center text-erp-slate-400 group-hover:text-erp-navy group-hover:border-erp-navy transition-colors mb-0.5">
            <IconCamera size={20} strokeWidth={1.8} />
          </div>
          <span className="text-xs text-erp-text-main group-hover:text-erp-navy font-semibold transition-colors truncate max-w-full">
            {t("upload")}
          </span>
        </button>
      )}

      {activeError && (
        <p className="erp-error-text flex items-center gap-1.5 mt-1 text-xs text-erp-danger font-medium" role="alert">
          <IconAlertCircle size={14} className="shrink-0" />
          <span>{activeError}</span>
        </p>
      )}

      {/* Full Architectural Lightbox via GalleryLightboxModal */}
      {previewSrc && (
        <GalleryLightboxModal
          isOpen={isLightboxOpen}
          onClose={() => setIsLightboxOpen(false)}
          items={[
            {
              id: "single-preview",
              imageUrl: previewSrc,
              caption: label || tForm("imagePreview"),
            } as GalleryItemMetadata,
          ]}
          currentIndex={0}
          onIndexChange={() => {}}
          title={label || tForm("imagePreview")}
        />
      )}
    </div>
  );
}

export default ImageUpload;
