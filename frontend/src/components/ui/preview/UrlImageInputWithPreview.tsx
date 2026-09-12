"use client";

import React, { useState } from "react";
import { Input } from "@/components/ui/Input";
import { ImageInputPreview } from "@/components/forms/ImageInputPreview";
import { cn } from "@/lib/utils/cn";

export interface UrlImageInputWithPreviewProps {
  value?: string;
  onChange: (url: string) => void;
  label?: string;
  placeholder?: string;
  className?: string;
}

export function UrlImageInputWithPreview({
  value = "",
  onChange,
  label = "URL รูปภาพ",
  placeholder = "https://...",
  className,
}: UrlImageInputWithPreviewProps) {
  return (
    <div className={cn("space-y-2 text-left", className)}>
      <Input
        label={label}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
      />
      {value && (
        <ImageInputPreview
          url={value}
          onRemove={() => onChange("")}
          aspectRatio="16:9"
        />
      )}
    </div>
  );
}

UrlImageInputWithPreview.displayName = "UrlImageInputWithPreview";
