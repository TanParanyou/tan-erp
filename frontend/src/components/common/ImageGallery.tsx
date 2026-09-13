"use client";

import React, { useState, useEffect } from "react";
import { useTranslations } from "next-intl";
import { IconZoomIn } from "@/components/common/Icons";
import { useLightbox } from "@/providers/lightbox-provider";
import { cn } from "@/lib/utils/cn";

export interface ImageGalleryProps {
  images?: string[];
  alt?: string;
  title?: string;
  emptyText?: string;
  aspectRatio?: "video" | "square";
  enableLightbox?: boolean;
  className?: string;
}

export function ImageGallery({
  images = [],
  alt = "Image preview",
  title,
  emptyText,
  aspectRatio = "video",
  enableLightbox = true,
  className,
}: ImageGalleryProps) {
  const t = useTranslations("estimates");
  const lightbox = useLightbox();
  const [activeIndex, setActiveIndex] = useState(0);

  // Filter valid image strings
  const validImages = Array.isArray(images)
    ? images.filter((img): img is string => typeof img === "string" && img.trim().length > 0)
    : [];

  // Reset active index when image array changes
  useEffect(() => {
    setActiveIndex(0);
  }, [validImages[0], validImages.length]);

  const activeImage = validImages[activeIndex];
  const hasMultiple = validImages.length > 1;

  const handleImageClick = (e?: React.MouseEvent) => {
    e?.stopPropagation();
    if (!enableLightbox || !activeImage) return;
    lightbox.openLightbox({
      images: validImages,
      initialIndex: activeIndex,
      title: title || alt,
      alt,
    });
  };

  const aspectClass = aspectRatio === "video" ? "aspect-video" : "aspect-square";

  return (
    <div className={cn("border border-erp-border bg-erp-surface-subtle p-3 space-y-2 rounded-none", className)}>
      {activeImage ? (
        <div
          onClick={handleImageClick}
          className={cn(
            "relative w-full bg-erp-surface border border-erp-border flex items-center justify-center overflow-hidden group rounded-none",
            aspectClass,
            enableLightbox ? "cursor-zoom-in" : ""
          )}
        >
          <img
            src={activeImage}
            alt={alt}
            className="w-full h-full object-contain select-none"
          />

          {/* Action Button: View Full Image with clear text and icon */}
          {enableLightbox && (
            <button
              type="button"
              onClick={handleImageClick}
              className="absolute bottom-2 left-2 inline-flex items-center gap-1.5 px-3 py-1.5 bg-erp-navy hover:bg-erp-navy-hover text-white text-xs font-semibold border border-white/30 shadow-lg transition-all cursor-pointer rounded-none z-10"
              title={t("catalogDetail.viewDetail")}
              aria-label={t("catalogDetail.viewDetail")}
            >
              <IconZoomIn size={15} />
              <span>{t("catalogDetail.viewDetail")}</span>
            </button>
          )}

          {/* Image Counter Badge */}
          {hasMultiple && (
            <div className="absolute bottom-2 right-2 px-2 py-0.5 bg-erp-navy/80 text-white font-mono text-[10px] tracking-wider border border-white/10 select-none">
              {activeIndex + 1} / {validImages.length}
            </div>
          )}
        </div>
      ) : (
        <div
          className={cn(
            "w-full bg-erp-surface border border-erp-border border-dashed flex flex-col items-center justify-center text-erp-text-muted rounded-none",
            aspectClass
          )}
        >
          <svg
            width="36"
            height="36"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.5"
            strokeLinecap="square"
            aria-hidden="true"
            className="mb-1 text-erp-text-muted/60"
          >
            <rect x="3" y="3" width="18" height="18" rx="0" ry="0" />
            <circle cx="8.5" cy="8.5" r="1.5" />
            <polyline points="21 15 16 10 5 21" />
          </svg>
          <span className="text-xs">{emptyText || t("catalogDetail.noImage")}</span>
        </div>
      )}

      {/* Thumbnails if multiple images */}
      {hasMultiple && (
        <div className="flex gap-2 overflow-x-auto pt-1">
          {validImages.map((img, idx) => (
            <button
              key={`${img}-${idx}`}
              type="button"
              onClick={() => setActiveIndex(idx)}
              className={cn(
                "relative w-16 h-12 border shrink-0 overflow-hidden cursor-pointer rounded-none transition-all",
                activeIndex === idx
                  ? "border-erp-navy ring-1 ring-erp-navy opacity-100"
                  : "border-erp-border hover:border-erp-navy/50 opacity-70 hover:opacity-100"
              )}
              aria-label={`${t("catalogDetail.image")} ${idx + 1}`}
            >
              <img
                src={img}
                alt={`${alt} thumbnail ${idx + 1}`}
                className="w-full h-full object-cover"
              />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}

export default ImageGallery;
