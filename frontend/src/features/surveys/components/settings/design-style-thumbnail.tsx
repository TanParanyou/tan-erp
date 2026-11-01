"use client";

import React, { useState } from "react";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { DesignStyleIllustration } from "./design-style-illustration";
import { GalleryLightboxModal, type GalleryItemMetadata } from "@/components/common/GalleryLightboxModal";
import { IconEye } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface DesignStyleThumbnailProps {
  code?: string;
  imageFileId?: string | null;
  imageFileIds?: (string | null | undefined)[] | null;
  name?: string | null;
  className?: string;
  size?: "sm" | "md" | "lg" | "full";
  allowLightbox?: boolean;
}

export function DesignStyleThumbnail({
  code = "default",
  imageFileId,
  imageFileIds,
  name,
  className,
  size = "md",
  allowLightbox = true,
}: DesignStyleThumbnailProps) {
  const [isLightboxOpen, setIsLightboxOpen] = useState(false);
  const [currentLightboxIndex, setCurrentLightboxIndex] = useState(0);

  const sizeClasses = {
    sm: "w-12 h-9",
    md: "w-20 h-14",
    lg: "w-32 h-22",
    full: "w-full aspect-16/10",
  }[size];

  // Resolve all unique valid file IDs (preserving order, cover first)
  const allFileIds: string[] = [];
  if (imageFileIds && imageFileIds.length > 0) {
    for (const id of imageFileIds) {
      if (id && typeof id === "string" && !allFileIds.includes(id)) {
        allFileIds.push(id);
      }
    }
  }
  if (imageFileId && !allFileIds.includes(imageFileId)) {
    allFileIds.unshift(imageFileId);
  }

  const primaryFileId = allFileIds[0] ?? imageFileId ?? null;
  const totalImages = allFileIds.length;

  const galleryItems: GalleryItemMetadata[] = allFileIds.map((fId, idx) => ({
    id: `${code}-lightbox-${fId}-${idx}`,
    fileId: fId,
    caption: `${name ?? code}${totalImages > 1 ? ` (${idx + 1}/${totalImages})` : ""}`,
  }));

  const hasImages = allFileIds.length > 0;

  return (
    <>
      <div
        className={cn(
          "relative group border border-erp-border bg-erp-surface-subtle overflow-hidden shrink-0 select-none shadow-2xs",
          sizeClasses,
          allowLightbox && hasImages && "cursor-pointer hover:border-erp-navy/60 transition-colors",
          className
        )}
        onClick={() => {
          if (allowLightbox && hasImages) {
            setCurrentLightboxIndex(0);
            setIsLightboxOpen(true);
          }
        }}
        title={name ?? code}
      >
        {primaryFileId ? (
          <AuthenticatedFileImage
            fileId={primaryFileId}
            alt={name ?? code}
            fallback={<DesignStyleIllustration code={code} alt={name ?? code} />}
            className="w-full h-full object-cover"
          />
        ) : (
          <DesignStyleIllustration code={code} alt={name ?? code} />
        )}

        {/* Counter badge for multiple images */}
        {totalImages > 1 && (
          <div className="absolute bottom-1 right-1 px-1.5 py-0.2 text-[10px] font-mono font-semibold bg-black/70 text-white leading-tight shadow-xs pointer-events-none">
            +{totalImages - 1}
          </div>
        )}

        {/* Hover overlay with eye icon */}
        {allowLightbox && hasImages && (
          <div className="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center text-white">
            <IconEye size={16} />
          </div>
        )}
      </div>

      {hasImages && (
        <GalleryLightboxModal
          isOpen={isLightboxOpen}
          onClose={() => setIsLightboxOpen(false)}
          items={galleryItems}
          currentIndex={currentLightboxIndex}
          onIndexChange={setCurrentLightboxIndex}
          title={name ?? code}
        />
      )}
    </>
  );
}
