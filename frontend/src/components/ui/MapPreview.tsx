"use client";

import React from "react";
import { useTranslations, useLocale } from "next-intl";
import { cn } from "@/lib/utils/cn";
import { IconGlobe } from "@/components/common/Icons";

export interface MapPreviewProps {
  latitude: number | null | undefined;
  longitude: number | null | undefined;
  zoom?: number;
  height?: number | string;
  className?: string;
  showExternalLink?: boolean;
  title?: string;
}

/**
 * MapPreview: Global reusable component for displaying an interactive embedded map preview
 * based on Latitude and Longitude coordinates.
 * Adheres strictly to Atelier Architectural Navy Sharp design principles (sharp corners, border-radius: 0px).
 */
export function MapPreview({
  latitude,
  longitude,
  zoom = 15,
  height = 192, // default 12rem (h-48)
  className,
  showExternalLink = true,
  title,
}: MapPreviewProps) {
  const t = useTranslations("common.geolocation");
  const locale = useLocale();

  const isValidCoordinate =
    typeof latitude === "number" &&
    !isNaN(latitude) &&
    typeof longitude === "number" &&
    !isNaN(longitude);

  if (!isValidCoordinate) {
    return null;
  }

  const iframeSrc = `https://maps.google.com/maps?q=${latitude},${longitude}&hl=${locale}&z=${zoom}&output=embed`;
  const googleMapsExternalUrl = `https://www.google.com/maps/search/?api=1&query=${latitude},${longitude}`;
  const displayTitle = title || t("previewTitle");

  return (
    <div className={cn("flex flex-col gap-1.5", className)}>
      <div
        className="w-full border border-erp-border bg-erp-surface-muted overflow-hidden relative"
        style={{
          height: typeof height === "number" ? `${height}px` : height,
          borderRadius: "0px",
        }}
      >
        <iframe
          title={displayTitle}
          width="100%"
          height="100%"
          style={{ border: 0 }}
          loading="lazy"
          referrerPolicy="no-referrer-when-downgrade"
          src={iframeSrc}
        />
      </div>

      {showExternalLink && (
        <div className="flex justify-end">
          <a
            href={googleMapsExternalUrl}
            target="_blank"
            rel="noopener noreferrer"
            className="inline-flex items-center gap-1 text-xs font-medium text-erp-navy hover:underline focus-visible:outline-2 focus-visible:outline-erp-navy"
          >
            <IconGlobe size={13} className="shrink-0" />
            <span>{t("openInMaps")}</span>
          </a>
        </div>
      )}
    </div>
  );
}
