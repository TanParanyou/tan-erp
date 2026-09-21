"use client";

import React, { useEffect, useState, useCallback, useRef } from "react";
import { createPortal } from "react-dom";
import { useTranslations } from "next-intl";
import {
  IconClose,
  IconChevronLeft,
  IconChevronRight,
  IconExternalLink,
  IconTrash,
} from "@/components/common/Icons";
import { CopyButton } from "@/components/common/CopyButton";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { cn } from "@/lib/utils/cn";

export interface GalleryItemMetadata {
  id: string;
  imageUrl?: string;
  fileId?: string;
  caption?: string | null;
  stageBadge?: React.ReactNode;
  createdByName?: string | null;
  createdAtUtc?: string | null;
  onDetach?: () => void;
  canDetach?: boolean;
  extraMetadata?: { label: string; value: React.ReactNode }[];
}

export interface GalleryLightboxModalProps {
  isOpen: boolean;
  onClose: () => void;
  items: GalleryItemMetadata[];
  currentIndex: number;
  onIndexChange: (index: number) => void;
  title?: string;
}

/**
 * Central Reusable GalleryLightboxModal component:
 * - Rendered via createPortal directly into document.body (z-[100]) to guarantee it is never clipped or occluded
 * - Atelier Architectural Navy Sharp aesthetic: 0px border-radius, Solid Navy #0B3056, 1px rules
 * - Full width top header bar with non-overlapping badges, title, counter, and actions
 * - Split View Master-Detail layout: High-contrast midnight canvas + Architectural metadata panel
 * - Carousel navigation: Large solid Prev/Next buttons, Thumbnails strip, and keyboard shortcuts (ArrowLeft, ArrowRight, Escape)
 */
export function GalleryLightboxModal({
  isOpen,
  onClose,
  items,
  currentIndex,
  onIndexChange,
  title,
}: GalleryLightboxModalProps) {
  const tLightbox = useTranslations("common.lightbox");
  const tActions = useTranslations("common.actions");
  const tOpportunities = useTranslations("opportunities");

  const [mounted, setMounted] = useState(false);
  const thumbnailContainerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    setMounted(true);
  }, []);

  const safeIndex = items.length > 0 ? Math.max(0, Math.min(currentIndex, items.length - 1)) : 0;
  const currentItem = items[safeIndex] ?? null;
  const hasMultiple = items.length > 1;

  const handlePrev = useCallback(() => {
    if (items.length <= 1) return;
    onIndexChange(safeIndex > 0 ? safeIndex - 1 : items.length - 1);
  }, [items.length, onIndexChange, safeIndex]);

  const handleNext = useCallback(() => {
    if (items.length <= 1) return;
    onIndexChange(safeIndex < items.length - 1 ? safeIndex + 1 : 0);
  }, [items.length, onIndexChange, safeIndex]);

  // Keyboard navigation
  const handleKeyDown = useCallback(
    (e: KeyboardEvent) => {
      if (!isOpen) return;
      if (e.key === "Escape") {
        e.preventDefault();
        onClose();
      } else if (e.key === "ArrowLeft") {
        e.preventDefault();
        handlePrev();
      } else if (e.key === "ArrowRight") {
        e.preventDefault();
        handleNext();
      }
    },
    [isOpen, onClose, handlePrev, handleNext]
  );

  // Scroll active thumbnail into view
  useEffect(() => {
    if (!isOpen || !thumbnailContainerRef.current) return;
    const container = thumbnailContainerRef.current;
    const activeThumbnail = container.children[safeIndex] as HTMLElement | undefined;
    if (activeThumbnail && typeof activeThumbnail.scrollIntoView === "function") {
      activeThumbnail.scrollIntoView({
        behavior: "smooth",
        block: "nearest",
        inline: "center",
      });
    }
  }, [isOpen, safeIndex]);

  // Body scroll lock and key listeners
  useEffect(() => {
    if (isOpen) {
      window.addEventListener("keydown", handleKeyDown);
      const originalOverflow = document.body.style.overflow;
      document.body.style.overflow = "hidden";
      return () => {
        window.removeEventListener("keydown", handleKeyDown);
        document.body.style.overflow = originalOverflow;
      };
    }
  }, [isOpen, handleKeyDown]);

  if (!isOpen || !currentItem || !mounted) return null;

  return createPortal(
    <div
      className="fixed inset-0 z-[1300] flex items-center justify-center bg-black/90 backdrop-blur-md p-0 sm:p-4 md:p-6 select-none"
      onClick={onClose}
      role="dialog"
      aria-modal="true"
      aria-label={title || tLightbox("preview")}
    >
      {/* Modal Container Box - Fullscreen on mobile, architectural framed window on desktop */}
      <div
        className="relative flex flex-col w-full h-full sm:h-[88vh] sm:max-h-[88vh] sm:max-w-6xl bg-erp-surface rounded-none sm:border sm:border-erp-border sm:shadow-2xl overflow-hidden"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Top Header Bar across full width */}
        <div className="shrink-0 h-12 sm:h-13 border-b border-erp-border bg-erp-surface flex items-center justify-between px-3 sm:px-6 gap-2 sm:gap-3 z-30">
          {/* Left: Title, Counter & Stage Badge */}
          <div className="flex items-center gap-2 sm:gap-3 min-w-0">
            <h2 className="text-xs sm:text-sm font-bold text-erp-navy uppercase tracking-wider truncate">
              {title || tOpportunities("workImagesTitle")}
            </h2>

            {hasMultiple && (
              <span className="px-2 py-0.5 text-xs font-mono font-bold bg-erp-navy/10 text-erp-navy border border-erp-navy/20 rounded-none shrink-0">
                {safeIndex + 1} / {items.length}
              </span>
            )}

            {currentItem.stageBadge && (
              <div className="shrink-0">{currentItem.stageBadge}</div>
            )}
          </div>

          {/* Right: Quick Actions & Prominent Close Button */}
          <div className="flex items-center gap-2 shrink-0">
            {/* Copy image URL */}
            {!currentItem.fileId && currentItem.imageUrl && (
              <CopyButton
                text={currentItem.imageUrl}
                variant="button"
                label={tLightbox("copyUrl")}
                copiedLabel={tLightbox("copiedUrl")}
                className="!h-8 !text-xs !bg-erp-surface-subtle hover:!bg-erp-surface-muted text-erp-text-main border border-erp-border hidden sm:inline-flex"
              />
            )}

            {/* Open Original link */}
            {!currentItem.fileId && currentItem.imageUrl && (
              <a
                href={currentItem.imageUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="h-8 px-2.5 inline-flex items-center gap-1.5 text-xs font-medium text-erp-text-main bg-erp-surface-subtle hover:bg-erp-surface-muted border border-erp-border transition-colors rounded-none"
                title={tLightbox("openNewTabAction")}
              >
                <IconExternalLink size={14} />
                <span className="hidden md:inline">{tLightbox("openNewTabAction")}</span>
              </a>
            )}

            {/* Close button */}
            <button
              type="button"
              onClick={onClose}
              className="h-8 px-3 inline-flex items-center gap-1 text-xs font-bold text-erp-text-main hover:bg-destructive hover:text-white border border-erp-border hover:border-destructive transition-colors rounded-none cursor-pointer"
              aria-label={tLightbox("close")}
              title={tLightbox("close")}
            >
              <IconClose size={16} strokeWidth={2.5} />
              <span className="hidden sm:inline">{tLightbox("closeAction")}</span>
            </button>
          </div>
        </div>

        {/* Split Master-Detail Body */}
        <div className="flex-1 flex flex-col md:flex-row min-h-0 overflow-hidden">
          {/* Left Canvas: Main Image Viewer & Carousel Controls */}
          <div className="flex-1 bg-[#050E1A] flex flex-col items-center justify-between p-3 relative overflow-hidden min-h-0">
            {/* Previous Button */}
            {hasMultiple && (
              <button
                type="button"
                onClick={handlePrev}
                className="absolute left-3 top-1/2 -translate-y-1/2 z-20 p-2 sm:p-3 rounded-none bg-erp-navy/90 hover:bg-erp-navy text-white border border-white/50 transition-all backdrop-blur cursor-pointer shadow-2xl hover:scale-105 active:scale-95"
                aria-label={tLightbox("previous")}
                title={tLightbox("previous")}
              >
                <IconChevronLeft size={26} strokeWidth={2.5} />
              </button>
            )}

            {/* Next Button */}
            {hasMultiple && (
              <button
                type="button"
                onClick={handleNext}
                className="absolute right-3 top-1/2 -translate-y-1/2 z-20 p-2 sm:p-3 rounded-none bg-erp-navy/90 hover:bg-erp-navy text-white border border-white/50 transition-all backdrop-blur cursor-pointer shadow-2xl hover:scale-105 active:scale-95"
                aria-label={tLightbox("next")}
                title={tLightbox("next")}
              >
                <IconChevronRight size={26} strokeWidth={2.5} />
              </button>
            )}

            {/* Centered Image View */}
            <div className="flex-1 w-full flex items-center justify-center p-2 min-h-0 overflow-hidden">
              {currentItem.fileId ? (
                <AuthenticatedFileImage
                  fileId={currentItem.fileId}
                  alt={currentItem.caption || title || tLightbox("preview")}
                  className="max-h-full max-w-full object-contain pointer-events-none select-none border border-white/10 shadow-2xl"
                  draggable={false}
                />
              ) : (
                <img
                  src={currentItem.imageUrl}
                  alt={currentItem.caption || title || tLightbox("preview")}
                  className="max-h-full max-w-full object-contain pointer-events-none select-none border border-white/10 shadow-2xl"
                  draggable={false}
                />
              )}
            </div>

            {/* Bottom Thumbnails Carousel Strip */}
            {hasMultiple && (
              <div
                ref={thumbnailContainerRef}
                className="w-full shrink-0 flex items-center justify-center gap-2 pt-2 pb-1 overflow-x-auto z-20 bg-black/50 backdrop-blur-sm border-t border-white/10 px-2"
              >
                {items.map((item, idx) => (
                  <button
                    key={item.id}
                    type="button"
                    onClick={() => onIndexChange(idx)}
                    className={cn(
                      "relative w-14 h-10 shrink-0 border transition-all rounded-none overflow-hidden cursor-pointer",
                      safeIndex === idx
                        ? "border-white ring-2 ring-white opacity-100 scale-105"
                        : "border-white/30 opacity-50 hover:opacity-100 hover:border-white/70"
                    )}
                    aria-label={`${tLightbox("image")} ${idx + 1}`}
                  >
                    {item.fileId ? (
                      <AuthenticatedFileImage
                        fileId={item.fileId}
                        alt={item.caption || `${tLightbox("image")} ${idx + 1}`}
                        loading="lazy"
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <img
                        src={item.imageUrl}
                        alt={item.caption || `${tLightbox("image")} ${idx + 1}`}
                        loading="lazy"
                        className="w-full h-full object-cover"
                      />
                    )}
                  </button>
                ))}
              </div>
            )}
          </div>

          {/* Right Sidebar: Architectural Metadata Panel */}
          <div className="w-full md:w-80 shrink-0 border-t md:border-t-0 md:border-l border-erp-border p-5 flex flex-col justify-between bg-erp-surface overflow-y-auto max-h-[35vh] md:max-h-none">
            <div className="space-y-4">
              {/* Caption Section */}
              <div className="space-y-1.5">
                <span className="text-[11px] font-mono uppercase tracking-wider text-erp-text-muted">
                  {tOpportunities("imageCaptionPlaceholder").replace("...", "")}
                </span>
                <p className="text-sm font-medium text-erp-text-main leading-relaxed break-words bg-erp-surface-subtle p-3 border border-erp-border">
                  {currentItem.caption ? (
                    currentItem.caption
                  ) : (
                    <span className="text-erp-text-muted italic">-</span>
                  )}
                </p>
              </div>

              {/* Upload Details Table */}
              <div className="space-y-2.5 pt-3 border-t border-erp-border text-xs">
                <div className="flex items-center justify-between text-erp-text-muted">
                  <span className="font-mono text-[11px] uppercase">
                    {tOpportunities("uploadedBy")}:
                  </span>
                  <span className="font-medium text-erp-text-main truncate max-w-[170px]">
                    {currentItem.createdByName ?? "-"}
                  </span>
                </div>

                <div className="flex items-center justify-between text-erp-text-muted">
                  <span className="font-mono text-[11px] uppercase">
                    {tOpportunities("stageBadge")}:
                  </span>
                  <span className="font-medium text-erp-text-main">
                    {currentItem.stageBadge ?? "-"}
                  </span>
                </div>

                <div className="flex items-center justify-between text-erp-text-muted">
                  <span className="font-mono text-[11px] uppercase">
                    {tLightbox("preview")}:
                  </span>
                  <span className="font-mono text-[11px] text-erp-text-main">
                    {currentItem.createdAtUtc
                      ? new Date(currentItem.createdAtUtc).toLocaleString()
                      : "-"}
                  </span>
                </div>

                {/* Extra Metadata */}
                {currentItem.extraMetadata?.map((meta, index) => (
                  <div
                    key={index}
                    className="flex items-center justify-between text-erp-text-muted"
                  >
                    <span className="font-mono text-[11px] uppercase">{meta.label}:</span>
                    <span className="text-erp-text-main">{meta.value}</span>
                  </div>
                ))}
              </div>
            </div>

            {/* Bottom Actions inside Sidebar */}
            <div className="mt-6 pt-4 border-t border-erp-border space-y-2">
              {/* Optional Detach Button */}
              {currentItem.canDetach && currentItem.onDetach && (
                <button
                  type="button"
                  onClick={currentItem.onDetach}
                  className="w-full flex items-center justify-center gap-2 px-3 py-2.5 text-xs font-semibold text-destructive hover:bg-destructive/10 border border-destructive/30 transition-colors rounded-none uppercase cursor-pointer"
                  title={tActions("delete")}
                >
                  <IconTrash size={14} />
                  <span>{tActions("delete")}</span>
                </button>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>,
    document.body
  );
}

export default GalleryLightboxModal;
