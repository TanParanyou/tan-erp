"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface MapEmbedPreviewProps {
  mapUrl?: string;
  title?: string;
  height?: string;
  className?: string;
}

export function MapEmbedPreview({
  mapUrl,
  title = "ตำแหน่งที่ตั้งโครงการ",
  height = "260px",
  className,
}: MapEmbedPreviewProps) {
  if (!mapUrl) {
    return (
      <div
        style={{ height }}
        className={cn(
          "flex items-center justify-center border border-erp-border bg-erp-surface-subtle text-xs text-erp-text-muted rounded-none",
          className
        )}
      >
        ยังไม่ได้ระบุลิงก์แผนที่ Google Maps Embed
      </div>
    );
  }

  return (
    <div
      style={{ height }}
      className={cn("w-full overflow-hidden border border-erp-border bg-erp-surface rounded-none shadow-xs", className)}
    >
      <iframe
        src={mapUrl}
        title={title}
        width="100%"
        height="100%"
        className="border-none"
        loading="lazy"
      />
    </div>
  );
}

MapEmbedPreview.displayName = "MapEmbedPreview";
