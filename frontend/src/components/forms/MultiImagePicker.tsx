"use client";

import React, { useState, useRef, useCallback, useEffect } from "react";
import Image from "next/image";
import { IconUpload, IconClose, IconAlertCircle, IconCamera } from "@/components/common/Icons";
import { optimizeImageToWebP, type OptimizationResult } from "@/lib/media/image-optimization";
import { CameraCaptureModal } from "./CameraCaptureModal";
import { useTranslations } from "next-intl";

export interface PendingImageItem {
  id: string;
  originalFile: File;
  optimizedFile?: File;
  previewUrl: string;
  caption: string;
  isOptimizing: boolean;
  savingsSummary?: string;
}

export interface MultiImagePickerProps {
  label?: string;
  maxFiles?: number;
  items: PendingImageItem[];
  onChange: (items: PendingImageItem[]) => void;
  className?: string;
  error?: string;
  disabled?: boolean;
  enableCamera?: boolean;
}

/**
 * Global reusable MultiImagePicker component following Atelier Architectural Navy Sharp standards:
 * - 0px border radius sharp aesthetics
 * - Client-side WebP compression + EXIF stripping with progress indicators
 * - Deferred upload (returns local File objects, does not upload immediately)
 * - Per-image caption input
 * - Native drag-and-drop support
 * - In-app WebRTC camera viewfinder with live preview, shutter snapshot review & retake
 * - Device native camera capture fallback
 */
interface ImagePickerItemProps {
  item: PendingImageItem;
  index: number;
  disabled?: boolean;
  onRemove: (id: string) => void;
  onCaptionChange: (id: string, caption: string) => void;
  placeholderText: string;
  removeTitleText: string;
  optimizingText: string;
}

function ImagePickerItem({
  item,
  index,
  disabled,
  onRemove,
  onCaptionChange,
  placeholderText,
  removeTitleText,
  optimizingText,
}: ImagePickerItemProps) {
  const [localCaption, setLocalCaption] = useState(item.caption);

  useEffect(() => {
    setLocalCaption(item.caption);
  }, [item.caption]);

  const handleCaptionChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setLocalCaption(val);
    onCaptionChange(item.id, val);
  };

  return (
    <div
      className="border border-erp-border bg-white dark:bg-erp-slate-900 p-3 space-y-2 relative shadow-none"
      style={{ borderRadius: "0px" }}
    >
      {/* Thumbnail with overlay status */}
      <div className="relative aspect-video w-full bg-erp-slate-100 dark:bg-erp-slate-800 overflow-hidden border border-erp-border">
        <Image
          src={item.previewUrl}
          alt={item.caption || `Image ${index + 1}`}
          fill
          unoptimized
          className="object-cover"
        />

        {/* Remove button */}
        {!disabled && (
          <button
            type="button"
            onClick={() => onRemove(item.id)}
            className="absolute top-1 right-1 p-1 bg-erp-navy/90 hover:bg-destructive text-white transition-colors z-10"
            style={{ borderRadius: "0px" }}
            title={removeTitleText}
          >
            <IconClose className="w-4 h-4" />
          </button>
        )}

        {/* Compression Status Badge - High contrast readable background */}
        <div className="absolute bottom-1.5 left-1.5 px-2 py-0.5 bg-slate-900/90 dark:bg-black/90 backdrop-blur-sm border border-white/20 text-[10px] font-mono font-medium text-white shadow-sm z-10">
          {item.isOptimizing ? (
            <span className="text-amber-300">{optimizingText}</span>
          ) : (
            <span className="text-emerald-300">{item.savingsSummary}</span>
          )}
        </div>
      </div>

      {/* Caption Input */}
      <input
        type="text"
        value={localCaption}
        onChange={handleCaptionChange}
        placeholder={placeholderText}
        disabled={disabled}
        maxLength={500}
        className="w-full px-2 py-1.5 text-xs bg-white dark:bg-erp-slate-950 border border-erp-border text-erp-text-main focus:outline-none focus:border-erp-navy"
        style={{ borderRadius: "0px" }}
      />
    </div>
  );
}

export function MultiImagePicker({
  label,
  maxFiles = 20,
  items,
  onChange,
  className = "",
  error,
  disabled = false,
  enableCamera = true,
}: MultiImagePickerProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common.actions");
  const tFeedback = useTranslations("common.feedback");
  const tCamera = useTranslations("common.camera");

  const [isDragging, setIsDragging] = useState(false);
  const [localError, setLocalError] = useState<string | null>(null);
  const [isCameraModalOpen, setIsCameraModalOpen] = useState(false);

  const fileInputRef = useRef<HTMLInputElement>(null);
  const nativeCameraInputRef = useRef<HTMLInputElement>(null);

  // Clean up object URLs on unmount
  const itemsRef = useRef(items);
  useEffect(() => {
    itemsRef.current = items;
  }, [items]);

  useEffect(() => {
    return () => {
      itemsRef.current.forEach((item) => {
        if (item.previewUrl) {
          URL.revokeObjectURL(item.previewUrl);
        }
      });
    };
  }, []);

  const processFiles = useCallback(
    async (selectedFiles: File[]) => {
      setLocalError(null);

      if (disabled) return;

      const remainingSlots = maxFiles - items.length;
      if (remainingSlots <= 0) {
        setLocalError(t("maxImagesLimit"));
        return;
      }

      const filesToProcess = selectedFiles.slice(0, remainingSlots);
      if (selectedFiles.length > remainingSlots) {
        setLocalError(t("maxImagesLimit"));
      }

      // Initial pending items with blob previews
      const newPendingItems: PendingImageItem[] = filesToProcess.map((file) => ({
        id: crypto.randomUUID(),
        originalFile: file,
        previewUrl: URL.createObjectURL(file),
        caption: "",
        isOptimizing: true,
      }));

      const updatedList = [...items, ...newPendingItems];
      onChange(updatedList);

      // Concurrently optimize images to WebP
      const optimizedResults = await Promise.all(
        filesToProcess.map(async (file, idx) => {
          const targetId = newPendingItems[idx].id;
          try {
            const res: OptimizationResult = await optimizeImageToWebP(file, {
              maxDimension: 2048,
              quality: 0.85,
            });
            const savings = res.isOptimized
              ? t("imageOptimizedSummary", { percent: res.savedPercent })
              : t("imageOriginalSummary");
            return { targetId, file: res.file, savings };
          } catch {
            return { targetId, file, savings: t("imageOriginalSummary") };
          }
        })
      );

      // Update state with optimized files
      onChange(
        updatedList.map((item) => {
          const opt = optimizedResults.find((o) => o.targetId === item.id);
          if (opt) {
            return {
              ...item,
              optimizedFile: opt.file,
              isOptimizing: false,
              savingsSummary: opt.savings,
            };
          }
          return item;
        })
      );
    },
    [disabled, items, maxFiles, onChange, t]
  );

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.files || e.target.files.length === 0) return;
    const fileList = Array.from(e.target.files).filter((f) =>
      f.type.startsWith("image/")
    );
    if (fileList.length === 0) {
      setLocalError(tFeedback("invalidFileType"));
      return;
    }
    processFiles(fileList);
    if (fileInputRef.current) fileInputRef.current.value = "";
    if (nativeCameraInputRef.current) nativeCameraInputRef.current.value = "";
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    if (!disabled) setIsDragging(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
    if (disabled || !e.dataTransfer.files) return;

    const fileList = Array.from(e.dataTransfer.files).filter((f) =>
      f.type.startsWith("image/")
    );
    if (fileList.length === 0) {
      setLocalError(tFeedback("invalidFileType"));
      return;
    }
    processFiles(fileList);
  };

  const handleCameraCapture = (file: File) => {
    processFiles([file]);
  };

  const removeItem = (id: string) => {
    const itemToRemove = items.find((i) => i.id === id);
    if (itemToRemove?.previewUrl) {
      URL.revokeObjectURL(itemToRemove.previewUrl);
    }
    onChange(items.filter((i) => i.id !== id));
  };

  const updateCaption = (id: string, caption: string) => {
    onChange(
      items.map((i) => (i.id === id ? { ...i, caption } : i))
    );
  };

  return (
    <div className={`space-y-4 ${className}`}>
      {label && (
        <label className="block text-xs font-mono font-medium uppercase tracking-wider text-muted-foreground">
          {label} ({items.length}/{maxFiles})
        </label>
      )}

      {/* Hidden File Inputs */}
      <input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png,image/webp"
        multiple
        className="hidden"
        onChange={handleInputChange}
      />
      <input
        ref={nativeCameraInputRef}
        type="file"
        accept="image/*"
        capture="environment"
        className="hidden"
        onChange={handleInputChange}
      />

      {/* Drop Zone & Capture Controls */}
      {items.length < maxFiles && !disabled && (
        <div
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          onClick={() => fileInputRef.current?.click()}
          className={`border-2 border-dashed p-6 text-center cursor-pointer transition-colors ${
            isDragging
              ? "border-erp-navy bg-erp-navy/5"
              : "border-erp-slate-300 dark:border-erp-slate-700 bg-erp-slate-50/60 dark:bg-erp-slate-900/30 hover:border-erp-navy hover:bg-erp-slate-100/50"
          }`}
          style={{ borderRadius: "0px" }}
        >
          <div className="flex flex-col items-center justify-center space-y-3">
            <div>
              <p className="text-sm font-semibold text-erp-text-main">{t("dropImagesHere")}</p>
              <p className="text-xs font-mono text-erp-text-muted mt-0.5">{t("maxImagesLimit")}</p>
            </div>

            {/* Square Action Buttons: Choose File (Left) & Open Camera (Right) */}
            <div
              className="flex items-center justify-center gap-4 pt-1"
              onClick={(e) => e.stopPropagation()}
            >
              <button
                type="button"
                onClick={() => fileInputRef.current?.click()}
                className="w-28 h-28 aspect-square flex flex-col items-center justify-center gap-2 p-3 bg-white dark:bg-erp-slate-800 border-2 border-erp-border hover:border-erp-navy hover:bg-erp-surface-subtle text-erp-navy dark:text-erp-slate-200 transition-all select-none focus:outline-none focus:border-erp-navy active:translate-y-0.5"
                style={{ borderRadius: "0px" }}
                title={t("browseFiles")}
              >
                <IconUpload size={28} className="text-erp-navy dark:text-erp-slate-200 shrink-0" />
                <span className="text-xs font-semibold text-erp-text-main text-center leading-tight">
                  {t("browseFiles")}
                </span>
              </button>

              {enableCamera && (
                <button
                  type="button"
                  onClick={() => setIsCameraModalOpen(true)}
                  className="w-28 h-28 aspect-square flex flex-col items-center justify-center gap-2 p-3 bg-erp-navy hover:bg-erp-navy-950 border-2 border-erp-navy text-white transition-all select-none focus:outline-none active:translate-y-0.5"
                  style={{ borderRadius: "0px" }}
                  title={t("takePhotoAction")}
                >
                  <IconCamera size={28} className="text-white shrink-0" />
                  <span className="text-xs font-semibold text-white text-center leading-tight">
                    {t("takePhotoAction")}
                  </span>
                </button>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Selected Items Grid */}
      {items.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
          {items.map((item, index) => (
            <ImagePickerItem
              key={item.id}
              item={item}
              index={index}
              disabled={disabled}
              onRemove={removeItem}
              onCaptionChange={updateCaption}
              placeholderText={t("imageCaptionPlaceholder")}
              removeTitleText={tCommon("cancel")}
              optimizingText={t("imageOptimizing")}
            />
          ))}
        </div>
      )}

      {/* Errors */}
      {(error || localError) && (
        <div className="flex items-center gap-1.5 text-xs text-destructive">
          <IconAlertCircle className="w-4 h-4 shrink-0" />
          <span>{error || localError}</span>
        </div>
      )}

      {/* Camera Capture Modal (In-App Viewfinder) */}
      {enableCamera && (
        <CameraCaptureModal
          isOpen={isCameraModalOpen}
          onClose={() => setIsCameraModalOpen(false)}
          onCapture={handleCameraCapture}
          title={tCamera("modalTitle")}
          preferredFacingMode="environment"
          onTriggerNativeCamera={() => nativeCameraInputRef.current?.click()}
        />
      )}
    </div>
  );
}
