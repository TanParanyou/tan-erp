"use client";

import React, { useState } from "react";
import Image from "next/image";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { MapPreview } from "@/components/ui/MapPreview";
import { IconMapPin, IconEye } from "@/components/common/Icons";
import { fileClient } from "@/lib/api/file-client";
import { GalleryLightboxModal, type GalleryItemMetadata } from "@/components/common/GalleryLightboxModal";
import type { SiteResponse } from "@/lib/api/api-client";

export interface SiteDetailDrawerProps {
  site: SiteResponse | null;
  isOpen: boolean;
  onClose: () => void;
}

export function SiteDetailDrawer({
  site,
  isOpen,
  onClose,
}: SiteDetailDrawerProps) {
  const t = useTranslations("sites");
  const tCommon = useTranslations("common");

  const [lightboxIndex, setLightboxIndex] = useState<number | null>(null);

  if (!site) return null;

  const siteLabel = site.label || t("siteDetail");
  const images = site.images ?? [];
  const galleryItems: GalleryItemMetadata[] = images.map((img, idx) => ({
    id: img.id || `img-${idx}`,
    imageUrl: fileClient.getFileUrl(img.fileId),
    caption: img.caption,
  }));

  return (
    <>
      <Drawer
        isOpen={isOpen}
        onClose={onClose}
        title={siteLabel}
        description={t("siteDetail")}
        size="lg"
        footer={
          <div className="flex justify-end w-full">
            <Button variant="outline" size="sm" onClick={onClose}>
              {tCommon("actions.close")}
            </Button>
          </div>
        }
      >
        <div className="flex flex-col gap-6 py-2">
          {/* Header Status Bar */}
          <div className="flex items-center justify-between bg-erp-surface-muted p-3 border border-erp-border">
            <div className="flex items-center gap-2">
              <IconMapPin size={18} className="text-erp-navy" />
              <span className="font-semibold text-sm text-erp-navy">{siteLabel}</span>
            </div>
            <Badge
              variant={site.status === "active" ? "success" : "neutral"}
              size="sm"
            >
              {site.status === "active" ? t("statusActive") : t("statusInactive")}
            </Badge>
          </div>

          {/* Address Details */}
          <div className="erp-card p-4 flex flex-col gap-3">
            <h3 className="text-xs font-bold text-erp-navy uppercase tracking-wide border-b border-erp-border-subtle pb-2 m-0">
              {t("address")}
            </h3>
            <dl className="erp-dl text-xs">
              <dt>{t("addressLine1")}:</dt>
              <dd className="font-medium">{site.addressLine1 || "-"}</dd>

              <dt>{t("areaSelection")}:</dt>
              <dd>
                <span className="font-medium text-erp-text-main">
                  {[site.subdistrict, site.district, site.province].filter(Boolean).join(" » ") || "-"}
                </span>{" "}
                {site.postalCode && (
                  <span className="font-mono text-erp-text-muted">
                    ({site.postalCode})
                  </span>
                )}
              </dd>

              <dt>{t("countryCode")}:</dt>
              <dd className="font-mono">{site.countryCode || "TH"}</dd>

              {site.accessNote && (
                <>
                  <dt>{t("accessNote")}:</dt>
                  <dd className="text-erp-navy bg-erp-surface-subtle p-2 border border-erp-border-subtle">
                    {site.accessNote}
                  </dd>
                </>
              )}
            </dl>
          </div>

          {/* GPS Coordinates & Map */}
          <div className="erp-card p-4 flex flex-col gap-3">
            <h3 className="text-xs font-bold text-erp-navy uppercase tracking-wide border-b border-erp-border-subtle pb-2 m-0">
              {t("coordinatesTitle")}
            </h3>
            {site.latitude != null && site.longitude != null ? (
              <div className="flex flex-col gap-2">
                <div className="text-xs font-mono text-erp-text-muted flex gap-4">
                  <span>
                    <strong>{t("latitude")}:</strong> {site.latitude}
                  </span>
                  <span>
                    <strong>{t("longitude")}:</strong> {site.longitude}
                  </span>
                </div>
                <MapPreview
                  latitude={site.latitude}
                  longitude={site.longitude}
                  showExternalLink={true}
                  className="mt-1"
                />
              </div>
            ) : (
              <p className="text-xs text-erp-text-muted italic m-0">
                {t("noCoordinates")}
              </p>
            )}
          </div>

          {/* Site Photos Gallery */}
          <div className="erp-card p-4 flex flex-col gap-3">
            <div className="flex items-center justify-between border-b border-erp-border-subtle pb-2">
              <h3 className="text-xs font-bold text-erp-navy uppercase tracking-wide m-0">
                {t("sitePhotos")}
              </h3>
              <span className="erp-badge erp-badge-navy font-mono text-xs">
                {t("photoCount", { count: images.length })}
              </span>
            </div>

            {images.length === 0 ? (
              <p className="text-xs text-erp-text-muted italic py-2 m-0">
                {t("noPhotos")}
              </p>
            ) : (
              <div className="grid grid-cols-2 sm:grid-cols-3 gap-3 pt-1">
                {images.map((img, idx) => (
                  <div
                    key={img.id || `img-${idx}`}
                    className="border border-erp-border bg-erp-surface group relative flex flex-col overflow-hidden rounded-none hover:border-erp-navy transition-colors cursor-pointer"
                    onClick={() => setLightboxIndex(idx)}
                  >
                    <div className="relative aspect-video w-full bg-erp-surface-muted overflow-hidden">
                      <Image
                        src={fileClient.getFileUrl(img.fileId)}
                        alt={img.caption || `Site image ${idx + 1}`}
                        fill
                        unoptimized
                        className="object-cover transition-transform group-hover:scale-105"
                      />
                      <div className="absolute inset-0 bg-erp-navy/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                        <span className="p-1.5 bg-erp-surface text-erp-navy rounded-none shadow-md">
                          <IconEye size={16} strokeWidth={2} />
                        </span>
                      </div>
                    </div>
                    {img.caption && (
                      <div className="p-1.5 bg-erp-surface-subtle border-t border-erp-border">
                        <p className="text-[11px] text-erp-text-main truncate m-0" title={img.caption}>
                          {img.caption}
                        </p>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </Drawer>

      {/* Lightbox Modal for Full View */}
      {lightboxIndex !== null && galleryItems.length > 0 && (
        <GalleryLightboxModal
          isOpen={lightboxIndex !== null}
          onClose={() => setLightboxIndex(null)}
          items={galleryItems}
          currentIndex={lightboxIndex}
          onIndexChange={(newIdx) => setLightboxIndex(newIdx)}
          title={siteLabel}
        />
      )}
    </>
  );
}
