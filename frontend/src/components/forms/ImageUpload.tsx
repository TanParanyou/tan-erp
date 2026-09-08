"use client";

import React, { useState, useRef, useCallback, useEffect } from "react";
import { IconUpload, IconClose, IconEye, IconAlertCircle } from "@/components/common/Icons";
import { optimizeImageToWebP } from "@/lib/media/image-optimization";
import { Modal } from "@/components/ui/Modal";
import { useTranslations } from "next-intl";

export interface ImageUploadProps {
  label?: string;
  value?: string | File;
  onChange: (value: File | string | null) => void;
  className?: string;
  error?: string;
  maxDimension?: number;
}

export function ImageUpload({
  label,
  value,
  onChange,
  className = "",
  error,
  maxDimension = 2048,
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
      setUploadError(null);

      // Validate file type
      if (!file.type.startsWith("image/")) {
        setUploadError(tFeedback("invalidFileType"));
        return;
      }

      // Validate file size (15MB raw limit)
      if (file.size > 15 * 1024 * 1024) {
        setUploadError(tFeedback("fileTooLarge"));
        return;
      }

      // Deferred Flow: Optimize image to WebP locally in browser
      const optResult = await optimizeImageToWebP(file, { maxDimension });
      const targetFile = optResult.file;

      // Revoke previously created local preview if any to prevent memory leak
      if (localPreviewRef.current) {
        URL.revokeObjectURL(localPreviewRef.current);
      }

      // Instant local preview
      const objectUrl = URL.createObjectURL(targetFile);
      setLocalPreview(objectUrl);

      // Pass File object to parent form (to be submitted in onSubmit)
      onChange(targetFile);
    },
    [maxDimension, onChange, tFeedback]
  );

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      handleFile(files[0]);
    }
  };

  const handleRemove = () => {
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
    setIsDragging(false);
    const files = e.dataTransfer.files;
    if (files && files.length > 0) {
      handleFile(files[0]);
    }
  };

  const activeError = uploadError || error;

  return (
    <div className={`erp-form-group ${className}`}>
      <input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png,image/webp"
        className="hidden"
        style={{ display: "none" }}
        onChange={handleInputChange}
      />

      {label && <label className="erp-label">{label}</label>}

      {previewSrc ? (
        <div className="relative inline-block group/preview">
          <div className="relative h-36 w-36 overflow-hidden border border-erp-border bg-erp-surface-muted rounded-none">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img
              src={previewSrc}
              alt="Preview"
              className="h-full w-full object-cover rounded-none"
            />
          </div>

          <button
            type="button"
            onClick={() => setIsLightboxOpen(true)}
            className="absolute inset-0 bg-black/40 text-white rounded-none flex items-center justify-center opacity-0 hover:opacity-100 transition-opacity focus-visible:outline-2 focus-visible:outline-white"
            title={t("view")}
            aria-label={t("view")}
          >
            <IconEye size={20} />
          </button>
          <button
            type="button"
            onClick={handleRemove}
            className="absolute -top-2 -right-2 h-6 w-6 bg-erp-danger text-white rounded-none flex items-center justify-center shadow-md transition-colors z-10 focus-visible:outline-2 focus-visible:outline-white"
            title={t("delete")}
            aria-label={t("delete")}
          >
            <IconClose size={14} />
          </button>
        </div>
      ) : (
        <div
          onClick={() => fileInputRef.current?.click()}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          role="button"
          tabIndex={0}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === " ") {
              e.preventDefault();
              fileInputRef.current?.click();
            }
          }}
          className={`flex flex-col items-center justify-center w-36 h-36 border-2 border-dashed ${
            isDragging
              ? "border-erp-navy bg-erp-navy-light"
              : "border-erp-border bg-erp-surface hover:border-erp-navy hover:bg-erp-surface-muted"
          } rounded-none transition-all cursor-pointer group`}
        >
          <IconUpload
            size={22}
            className="text-erp-text-muted group-hover:text-erp-navy mb-1.5 transition-colors"
          />
          <span className="text-xs text-erp-text-muted group-hover:text-erp-navy font-medium text-center px-2">
            {t("upload")}
          </span>
        </div>
      )}

      {activeError && (
        <p className="erp-error-text flex items-center gap-1" role="alert">
          <IconAlertCircle size={12} />
          <span>{activeError}</span>
        </p>
      )}

      {/* Lightweight Sharp Modal Lightbox */}
      {previewSrc && (
        <Modal
          isOpen={isLightboxOpen}
          onClose={() => setIsLightboxOpen(false)}
          size="lg"
          title={label || tForm("imagePreview")}
        >
          <div className="flex items-center justify-center p-2 bg-erp-surface">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img
              src={previewSrc}
              alt="Expanded Preview"
              className="max-h-[70vh] max-w-full object-contain rounded-none border border-erp-border"
            />
          </div>
        </Modal>
      )}
    </div>
  );
}

export default ImageUpload;
