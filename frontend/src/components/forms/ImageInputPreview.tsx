"use client";

import React from "react";
import { IconClose, IconEye, IconUpload } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface ImageInputPreviewProps {
  url?: string | null;
  onRemove?: () => void;
  onUploadClick?: () => void;
  className?: string;
  aspectRatio?: "1:1" | "16:9" | "4:3" | "auto";
  alt?: string;
  disabled?: boolean;
}

export function ImageInputPreview({
  url,
  onRemove,
  onUploadClick,
  className,
  aspectRatio = "16:9",
  alt = "Preview",
  disabled,
}: ImageInputPreviewProps) {
  const aspectClass =
    aspectRatio === "1:1"
      ? "aspect-square"
      : aspectRatio === "16:9"
      ? "aspect-video"
      : aspectRatio === "4:3"
      ? "aspect-[4/3]"
      : "";

  if (!url) {
    return (
      <div
        onClick={!disabled ? onUploadClick : undefined}
        className={cn(
          "flex flex-col items-center justify-center border border-dashed border-erp-border bg-erp-surface-subtle p-6 text-center transition-colors rounded-none",
          !disabled ? "cursor-pointer hover:border-erp-navy hover:bg-erp-surface" : "opacity-60 cursor-not-allowed",
          aspectClass,
          className
        )}
      >
        <IconUpload size={24} className="mb-2 text-erp-text-muted" />
        <span className="text-xs font-medium text-erp-text-main">เลือกรูปภาพ</span>
        <span className="text-[10px] text-erp-text-muted mt-0.5">PNG, JPG หรือ WebP</span>
      </div>
    );
  }

  return (
    <div
      className={cn(
        "group relative overflow-hidden border border-erp-border bg-erp-surface rounded-none shadow-xs",
        aspectClass,
        className
      )}
    >
      <img
        src={url}
        alt={alt}
        className="h-full w-full object-cover rounded-none transition-transform group-hover:scale-105"
      />
      <div className="absolute inset-0 flex items-center justify-center gap-2 bg-black/40 opacity-0 transition-opacity group-hover:opacity-100">
        <a
          href={url}
          target="_blank"
          rel="noreferrer"
          className="flex h-8 w-8 items-center justify-center bg-erp-surface text-erp-text-main shadow-sm hover:bg-erp-surface-subtle rounded-none"
          title="ดูภาพขนาดเต็ม"
        >
          <IconEye size={16} />
        </a>
        {!disabled && onRemove && (
          <button
            type="button"
            onClick={(e) => {
              e.stopPropagation();
              onRemove();
            }}
            className="flex h-8 w-8 items-center justify-center bg-red-600 text-white shadow-sm hover:bg-red-700 rounded-none"
            title="ลบรูปภาพ"
          >
            <IconClose size={16} />
          </button>
        )}
      </div>
    </div>
  );
}

ImageInputPreview.displayName = "ImageInputPreview";
