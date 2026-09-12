"use client";

import React, { useState } from "react";
import { IconClose, IconEye, IconUpload } from "@/components/common/Icons";
import { Button } from "@/components/ui/Button";
import { cn } from "@/lib/utils/cn";

export interface MediaImagePickerProps {
  value?: string | null;
  onChange: (url: string | null) => void;
  label?: string;
  helperText?: string;
  aspectRatio?: "1:1" | "16:9" | "4:3" | "auto";
  disabled?: boolean;
  className?: string;
}

export function MediaImagePicker({
  value,
  onChange,
  label,
  helperText,
  aspectRatio = "16:9",
  disabled,
  className,
}: MediaImagePickerProps) {
  const [isHovered, setIsHovered] = useState(false);

  const aspectClass =
    aspectRatio === "1:1"
      ? "aspect-square"
      : aspectRatio === "16:9"
      ? "aspect-video"
      : aspectRatio === "4:3"
      ? "aspect-[4/3]"
      : "";

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    const url = URL.createObjectURL(file);
    onChange(url);
  };

  return (
    <div className={cn("flex flex-col gap-1.5 text-left", className)}>
      {label && (
        <span className="text-xs font-semibold uppercase tracking-wider text-erp-text-main">
          {label}
        </span>
      )}

      {value ? (
        <div
          onMouseEnter={() => setIsHovered(true)}
          onMouseLeave={() => setIsHovered(false)}
          className={cn(
            "relative overflow-hidden border border-erp-border bg-erp-surface rounded-none",
            aspectClass
          )}
        >
          <img src={value} alt="Preview" className="h-full w-full object-cover" />
          {isHovered && !disabled && (
            <div className="absolute inset-0 flex items-center justify-center gap-2 bg-black/40 transition-opacity">
              <a
                href={value}
                target="_blank"
                rel="noreferrer"
                className="flex h-8 w-8 items-center justify-center bg-erp-surface text-erp-text-main shadow-xs rounded-none"
              >
                <IconEye size={16} />
              </a>
              <button
                type="button"
                onClick={() => onChange(null)}
                className="flex h-8 w-8 items-center justify-center bg-red-600 text-white shadow-xs rounded-none"
              >
                <IconClose size={16} />
              </button>
            </div>
          )}
        </div>
      ) : (
        <label
          className={cn(
            "flex flex-col items-center justify-center border border-dashed border-erp-border bg-erp-surface-subtle p-6 text-center rounded-none transition-colors",
            !disabled
              ? "cursor-pointer hover:border-erp-navy hover:bg-erp-surface"
              : "cursor-not-allowed opacity-60",
            aspectClass
          )}
        >
          <input
            type="file"
            accept="image/*"
            disabled={disabled}
            onChange={handleFileSelect}
            className="hidden"
          />
          <IconUpload size={22} className="mb-1.5 text-erp-text-muted" />
          <span className="text-xs font-medium text-erp-text-main">เลือกรูปภาพ</span>
          <span className="text-[10px] text-erp-text-muted mt-0.5">PNG, JPG หรือ WebP</span>
        </label>
      )}

      {helperText && <p className="text-xs text-erp-text-muted">{helperText}</p>}
    </div>
  );
}

MediaImagePicker.displayName = "MediaImagePicker";
