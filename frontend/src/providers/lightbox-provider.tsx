"use client";

import React, {
  createContext,
  useContext,
  useState,
  useCallback,
  useEffect,
  useId,
  useRef,
} from "react";
import { createPortal } from "react-dom";
import { useTranslations } from "next-intl";
import {
  IconClose,
  IconChevronLeft,
  IconChevronRight,
  IconZoomIn,
  IconZoomOut,
  IconRefresh,
  IconRotateCw,
  IconRotateCcw,
  IconDownload,
  IconExternalLink,
} from "@/components/common/Icons";
import { CopyButton } from "@/components/common/CopyButton";
import { useScrollLock } from "@/hooks/useScrollLock";

export interface OpenLightboxOptions {
  images: string[];
  initialIndex?: number;
  title?: string;
  alt?: string;
}

export interface LightboxContextValue {
  isOpen: boolean;
  images: string[];
  currentIndex: number;
  currentImage: string | null;
  scale: number;
  rotation: number;
  isZoomed: boolean;
  title?: string;
  alt?: string;
  openLightbox: (options: OpenLightboxOptions) => void;
  closeLightbox: () => void;
  nextImage: () => void;
  prevImage: () => void;
  setImageIndex: (index: number) => void;
  zoomIn: () => void;
  zoomOut: () => void;
  resetZoom: () => void;
  toggleZoom: () => void;
  setZoom: (scale: number) => void;
  rotateCw: () => void;
  rotateCcw: () => void;
  resetRotation: () => void;
  resetAll: () => void;
  downloadCurrentImage: (fileName?: string) => void;
}

const defaultContextValue: LightboxContextValue = {
  isOpen: false,
  images: [],
  currentIndex: 0,
  currentImage: null,
  scale: 1,
  rotation: 0,
  isZoomed: false,
  openLightbox: () => {},
  closeLightbox: () => {},
  nextImage: () => {},
  prevImage: () => {},
  setImageIndex: () => {},
  zoomIn: () => {},
  zoomOut: () => {},
  resetZoom: () => {},
  toggleZoom: () => {},
  setZoom: () => {},
  rotateCw: () => {},
  rotateCcw: () => {},
  resetRotation: () => {},
  resetAll: () => {},
  downloadCurrentImage: () => {},
};

const LightboxContext = createContext<LightboxContextValue>(defaultContextValue);

export function useLightbox(): LightboxContextValue {
  return useContext(LightboxContext);
}

export interface LightboxProviderProps {
  children: React.ReactNode;
}

const SCALE_STEPS = [1, 1.5, 2, 2.5, 3] as const;

export function LightboxProvider({ children }: LightboxProviderProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [images, setImages] = useState<string[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [title, setTitle] = useState<string | undefined>(undefined);
  const [alt, setAlt] = useState<string | undefined>(undefined);

  // Transformation states
  const [scale, setScale] = useState(1);
  const [rotation, setRotation] = useState(0);
  const [position, setPosition] = useState({ x: 0, y: 0 });

  const resetTransforms = useCallback(() => {
    setScale(1);
    setRotation(0);
    setPosition({ x: 0, y: 0 });
  }, []);

  const openLightbox = useCallback(
    (options: OpenLightboxOptions) => {
      if (!options.images || options.images.length === 0) return;
      setImages(options.images);
      const validIndex =
        options.initialIndex !== undefined &&
        options.initialIndex >= 0 &&
        options.initialIndex < options.images.length
          ? options.initialIndex
          : 0;
      setCurrentIndex(validIndex);
      setTitle(options.title);
      setAlt(options.alt);
      resetTransforms();
      setIsOpen(true);
    },
    [resetTransforms]
  );

  const closeLightbox = useCallback(() => {
    setIsOpen(false);
    resetTransforms();
  }, [resetTransforms]);

  const nextImage = useCallback(() => {
    setImages((currImages) => {
      if (currImages.length <= 1) return currImages;
      setCurrentIndex((prev) => (prev + 1) % currImages.length);
      resetTransforms();
      return currImages;
    });
  }, [resetTransforms]);

  const prevImage = useCallback(() => {
    setImages((currImages) => {
      if (currImages.length <= 1) return currImages;
      setCurrentIndex((prev) => (prev - 1 + currImages.length) % currImages.length);
      resetTransforms();
      return currImages;
    });
  }, [resetTransforms]);

  const setImageIndex = useCallback(
    (index: number) => {
      setImages((currImages) => {
        if (index >= 0 && index < currImages.length) {
          setCurrentIndex(index);
          resetTransforms();
        }
        return currImages;
      });
    },
    [resetTransforms]
  );

  const zoomIn = useCallback(() => {
    setScale((prev) => {
      const next = SCALE_STEPS.find((s) => s > prev);
      return next ?? 3;
    });
  }, []);

  const zoomOut = useCallback(() => {
    setScale((prev) => {
      const reversed = [...SCALE_STEPS].reverse();
      const next = reversed.find((s) => s < prev);
      const result = next ?? 1;
      if (result === 1) {
        setPosition({ x: 0, y: 0 });
      }
      return result;
    });
  }, []);

  const resetZoom = useCallback(() => {
    setScale(1);
    setPosition({ x: 0, y: 0 });
  }, []);

  const toggleZoom = useCallback(() => {
    setScale((prev) => {
      if (prev > 1) {
        setPosition({ x: 0, y: 0 });
        return 1;
      }
      return 2;
    });
  }, []);

  const setZoom = useCallback((newScale: number) => {
    const clamped = Math.max(1, Math.min(3, newScale));
    setScale(clamped);
    if (clamped === 1) {
      setPosition({ x: 0, y: 0 });
    }
  }, []);

  const rotateCw = useCallback(() => {
    setRotation((prev) => (prev + 90) % 360);
  }, []);

  const rotateCcw = useCallback(() => {
    setRotation((prev) => (prev - 90 + 360) % 360);
  }, []);

  const resetRotation = useCallback(() => {
    setRotation(0);
  }, []);

  const resetAll = useCallback(() => {
    resetTransforms();
  }, [resetTransforms]);

  const currentImage = images[currentIndex] || null;

  const downloadCurrentImage = useCallback(
    async (fileName?: string) => {
      if (!currentImage) return;
      try {
        const response = await fetch(currentImage);
        const blob = await response.blob();
        const blobUrl = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = blobUrl;
        const fallbackName = `image-${currentIndex + 1}.jpg`;
        link.download = fileName || title?.replace(/[^a-z0-9]/gi, "_") || fallbackName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(blobUrl);
      } catch {
        // Direct link fallback if CORS blocks blob download
        const link = document.createElement("a");
        link.href = currentImage;
        link.target = "_blank";
        link.download = fileName || `image-${currentIndex + 1}.jpg`;
        link.click();
      }
    },
    [currentImage, currentIndex, title]
  );

  const contextValue: LightboxContextValue = {
    isOpen,
    images,
    currentIndex,
    currentImage,
    scale,
    rotation,
    isZoomed: scale > 1,
    title,
    alt,
    openLightbox,
    closeLightbox,
    nextImage,
    prevImage,
    setImageIndex,
    zoomIn,
    zoomOut,
    resetZoom,
    toggleZoom,
    setZoom,
    rotateCw,
    rotateCcw,
    resetRotation,
    resetAll,
    downloadCurrentImage,
  };

  return (
    <LightboxContext.Provider value={contextValue}>
      {children}
      {isOpen && (
        <LightboxModal
          images={images}
          currentIndex={currentIndex}
          title={title}
          alt={alt}
          scale={scale}
          rotation={rotation}
          position={position}
          setPosition={setPosition}
          onClose={closeLightbox}
          onNext={nextImage}
          onPrev={prevImage}
          onSelect={setImageIndex}
          onZoomIn={zoomIn}
          onZoomOut={zoomOut}
          onResetZoom={resetZoom}
          onToggleZoom={toggleZoom}
          onRotateCw={rotateCw}
          onRotateCcw={rotateCcw}
          onResetAll={resetAll}
          onDownload={downloadCurrentImage}
        />
      )}
    </LightboxContext.Provider>
  );
}

interface LightboxModalProps {
  images: string[];
  currentIndex: number;
  title?: string;
  alt?: string;
  scale: number;
  rotation: number;
  position: { x: 0; y: 0 } | { x: number; y: number };
  setPosition: React.Dispatch<React.SetStateAction<{ x: number; y: number }>>;
  onClose: () => void;
  onNext: () => void;
  onPrev: () => void;
  onSelect: (index: number) => void;
  onZoomIn: () => void;
  onZoomOut: () => void;
  onResetZoom: () => void;
  onToggleZoom: () => void;
  onRotateCw: () => void;
  onRotateCcw: () => void;
  onResetAll: () => void;
  onDownload: () => void;
}

function LightboxModal({
  images,
  currentIndex,
  title,
  alt,
  scale,
  rotation,
  position,
  setPosition,
  onClose,
  onNext,
  onPrev,
  onSelect,
  onZoomIn,
  onZoomOut,
  onToggleZoom,
  onRotateCw,
  onRotateCcw,
  onResetAll,
  onDownload,
}: LightboxModalProps) {
  const t = useTranslations("common");
  const titleId = useId();
  useScrollLock(true);

  // Dragging & Panning state
  const [isDragging, setIsDragging] = useState(false);
  const dragStartRef = useRef({ x: 0, y: 0, startX: 0, startY: 0 });

  // Touch swipe state
  const touchStartRef = useRef<{ x: number; y: number } | null>(null);

  // Keyboard navigation & Shortcuts
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        e.preventDefault();
        onClose();
      } else if (e.key === "ArrowRight") {
        e.preventDefault();
        onNext();
      } else if (e.key === "ArrowLeft") {
        e.preventDefault();
        onPrev();
      } else if (e.key === "+" || e.key === "=") {
        e.preventDefault();
        onZoomIn();
      } else if (e.key === "-" || e.key === "_") {
        e.preventDefault();
        onZoomOut();
      } else if (e.key.toLowerCase() === "r" || e.key === "0") {
        e.preventDefault();
        onResetAll();
      } else if (e.key === "]") {
        e.preventDefault();
        onRotateCw();
      } else if (e.key === "[") {
        e.preventDefault();
        onRotateCcw();
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [onClose, onNext, onPrev, onZoomIn, onZoomOut, onResetAll, onRotateCw, onRotateCcw]);

  // Mouse drag handlers
  const handleMouseDown = (e: React.MouseEvent) => {
    if (scale <= 1) return;
    e.preventDefault();
    setIsDragging(true);
    dragStartRef.current = {
      x: e.clientX,
      y: e.clientY,
      startX: position.x,
      startY: position.y,
    };
  };

  const handleMouseMove = (e: React.MouseEvent) => {
    if (!isDragging || scale <= 1) return;
    e.preventDefault();
    const dx = e.clientX - dragStartRef.current.x;
    const dy = e.clientY - dragStartRef.current.y;
    setPosition({
      x: dragStartRef.current.startX + dx,
      y: dragStartRef.current.startY + dy,
    });
  };

  const handleMouseUp = () => {
    setIsDragging(false);
  };

  // Wheel zoom handler
  const handleWheel = (e: React.WheelEvent) => {
    if (e.deltaY < 0) {
      onZoomIn();
    } else if (e.deltaY > 0) {
      onZoomOut();
    }
  };

  // Touch handlers for swipe (only when scale === 1)
  const handleTouchStart = (e: React.TouchEvent) => {
    if (e.touches.length === 1) {
      touchStartRef.current = {
        x: e.touches[0].clientX,
        y: e.touches[0].clientY,
      };
    }
  };

  const handleTouchEnd = (e: React.TouchEvent) => {
    if (!touchStartRef.current || scale > 1) return;
    const touchEnd = e.changedTouches[0];
    const dx = touchEnd.clientX - touchStartRef.current.x;
    const dy = touchEnd.clientY - touchStartRef.current.y;

    if (Math.abs(dx) > 60 && Math.abs(dy) < 50) {
      if (dx < 0) {
        onNext();
      } else {
        onPrev();
      }
    }
    touchStartRef.current = null;
  };

  if (typeof document === "undefined" || images.length === 0) return null;

  const currentImage = images[currentIndex];
  const hasMultiple = images.length > 1;
  const zoomPercent = Math.round(scale * 100);

  return createPortal(
    <div
      role="dialog"
      aria-modal="true"
      aria-labelledby={title ? titleId : undefined}
      style={{ backgroundColor: "rgba(7, 31, 56, 0.98)" }}
      className="fixed inset-0 z-[9999] flex flex-col justify-between backdrop-blur-md select-none"
      onClick={onClose}
    >
      {/* 1. Top Bar: Title & High-Contrast Utility Actions */}
      <div
        className="shrink-0 flex items-center justify-between px-4 sm:px-6 py-3 border-b border-white/20 bg-[#071F38] text-white shadow-lg"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Left: Title & Prominent Counter Badge */}
        <div className="min-w-0 flex items-center gap-3 mr-2">
          {hasMultiple && (
            <span className="font-mono text-xs font-bold text-[#0B3056] bg-white px-3 py-1 border border-white shrink-0 shadow-md">
              {currentIndex + 1} / {images.length}
            </span>
          )}
          {title ? (
            <h2 id={titleId} className="text-xs sm:text-sm font-semibold truncate text-white max-w-[200px] sm:max-w-md md:max-w-lg">
              {title}
            </h2>
          ) : (
            <span className="text-xs font-mono text-white/90 tracking-wider uppercase font-semibold">
              {t("lightbox.preview")}
            </span>
          )}
        </div>

        {/* Right: Prominent Top Action Buttons */}
        <div className="flex items-center gap-2 shrink-0">
          {/* Open Original in New Tab */}
          <a
            href={currentImage}
            target="_blank"
            rel="noopener noreferrer"
            className="hidden sm:inline-flex items-center gap-1.5 px-3.5 h-9 text-xs font-bold text-white bg-[#194B85] hover:bg-[#255FA3] border border-white/40 shadow-sm transition-colors rounded-none"
            title={t("lightbox.openNewTab")}
            aria-label={t("lightbox.openNewTab")}
          >
            <IconExternalLink size={16} />
            <span>{t("lightbox.openNewTabAction")}</span>
          </a>

          {/* Download Image Button */}
          <button
            type="button"
            onClick={() => onDownload()}
            className="inline-flex items-center gap-1.5 px-3.5 h-9 text-xs font-bold text-white bg-[#194B85] hover:bg-[#255FA3] border border-white/40 shadow-sm transition-colors rounded-none cursor-pointer"
            title={t("lightbox.download")}
            aria-label={t("lightbox.download")}
          >
            <IconDownload size={16} />
            <span className="hidden sm:inline">{t("lightbox.downloadAction")}</span>
          </button>

          {/* Prominent Close Button */}
          <button
            type="button"
            onClick={onClose}
            className="inline-flex items-center gap-1.5 px-4 h-9 text-xs font-bold text-white bg-red-600 hover:bg-red-700 border-2 border-red-400 shadow-md transition-colors rounded-none ml-1 cursor-pointer"
            aria-label={t("lightbox.close")}
            title={t("lightbox.close")}
          >
            <IconClose size={18} strokeWidth={2.5} />
            <span>{t("lightbox.closeAction")}</span>
          </button>
        </div>
      </div>

      {/* 2. Main Canvas Area with Floating Action Toolbar */}
      <div
        className="relative flex-1 flex items-center justify-center p-2 sm:p-6 overflow-hidden"
        onClick={(e) => e.stopPropagation()}
        onWheel={handleWheel}
        onMouseDown={handleMouseDown}
        onMouseMove={handleMouseMove}
        onMouseUp={handleMouseUp}
        onTouchStart={handleTouchStart}
        onTouchEnd={handleTouchEnd}
        style={{
          cursor: scale > 1 ? (isDragging ? "grabbing" : "grab") : "default",
        }}
      >
        {/* Previous Button (Large, high contrast, solid Navy) */}
        {hasMultiple && (
          <button
            type="button"
            onClick={onPrev}
            style={{ backgroundColor: "#0B3056" }}
            className="absolute left-3 sm:left-6 z-30 w-12 sm:w-14 h-16 sm:h-20 hover:brightness-125 text-white border-2 border-white/60 shadow-2xl transition-all flex items-center justify-center cursor-pointer rounded-none"
            aria-label={t("lightbox.previous")}
            title={t("lightbox.previous")}
          >
            <IconChevronLeft size={32} strokeWidth={2.5} />
          </button>
        )}

        {/* Display Image with Scale and Rotation */}
        <div
          className="relative max-w-5xl max-h-[70vh] flex items-center justify-center transition-transform duration-75"
          style={{
            transform: `translate(${position.x}px, ${position.y}px) scale(${scale}) rotate(${rotation}deg)`,
            transformOrigin: "center center",
          }}
          onDoubleClick={onToggleZoom}
        >
          <img
            src={currentImage}
            alt={alt || (title ? `${title} - ${currentIndex + 1}` : `Image ${currentIndex + 1}`)}
            className="max-w-full max-h-[70vh] object-contain border-2 border-white/30 shadow-2xl rounded-none pointer-events-none"
            draggable={false}
          />
        </div>

        {/* Next Button (Large, high contrast, solid Navy) */}
        {hasMultiple && (
          <button
            type="button"
            onClick={onNext}
            style={{ backgroundColor: "#0B3056" }}
            className="absolute right-3 sm:right-6 z-30 w-12 sm:w-14 h-16 sm:h-20 hover:brightness-125 text-white border-2 border-white/60 shadow-2xl transition-all flex items-center justify-center cursor-pointer rounded-none"
            aria-label={t("lightbox.next")}
            title={t("lightbox.next")}
          >
            <IconChevronRight size={32} strokeWidth={2.5} />
          </button>
        )}

        {/* Floating Action Dock (ชัดเจน เห็นได้ทันที มี icon + label) */}
        <div
          style={{ backgroundColor: "rgba(11, 48, 86, 0.96)" }}
          className="absolute bottom-3 sm:bottom-5 left-1/2 -translate-x-1/2 z-30 flex flex-wrap items-center justify-center gap-1 sm:gap-2 px-3 py-2 border-2 border-white/40 shadow-2xl rounded-none text-white max-w-[95vw]"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Zoom Out Button */}
          <button
            type="button"
            onClick={onZoomOut}
            disabled={scale <= 1}
            style={{ backgroundColor: "#194B85" }}
            className="inline-flex items-center gap-1.5 px-3 h-8 text-xs font-bold hover:brightness-125 border border-white/40 text-white disabled:opacity-40 disabled:cursor-not-allowed transition-all rounded-none cursor-pointer"
            title={t("lightbox.zoomOut")}
            aria-label={t("lightbox.zoomOut")}
          >
            <IconZoomOut size={16} />
            <span className="hidden sm:inline">{t("lightbox.zoomOutAction")}</span>
          </button>

          {/* Current Zoom Level Badge (Click to reset) */}
          <button
            type="button"
            onClick={onToggleZoom}
            className="px-3 h-8 text-xs font-mono font-bold bg-white text-[#0B3056] border-2 border-white hover:bg-slate-100 transition-all rounded-none cursor-pointer shadow-md"
            title={t("lightbox.resetZoom")}
          >
            {zoomPercent}%
          </button>

          {/* Zoom In Button */}
          <button
            type="button"
            onClick={onZoomIn}
            disabled={scale >= 3}
            style={{ backgroundColor: "#194B85" }}
            className="inline-flex items-center gap-1.5 px-3 h-8 text-xs font-bold hover:brightness-125 border border-white/40 text-white disabled:opacity-40 disabled:cursor-not-allowed transition-all rounded-none cursor-pointer"
            title={t("lightbox.zoomIn")}
            aria-label={t("lightbox.zoomIn")}
          >
            <IconZoomIn size={16} />
            <span className="hidden sm:inline">{t("lightbox.zoomInAction")}</span>
          </button>

          <span className="w-px h-5 bg-white/30 mx-0.5" />

          {/* Rotate CCW */}
          <button
            type="button"
            onClick={onRotateCcw}
            style={{ backgroundColor: "#194B85" }}
            className="p-1.5 sm:px-2.5 h-8 inline-flex items-center gap-1 text-xs font-bold hover:brightness-125 border border-white/40 text-white transition-all rounded-none cursor-pointer"
            title={t("lightbox.rotateCcw")}
            aria-label={t("lightbox.rotateCcw")}
          >
            <IconRotateCcw size={16} />
          </button>

          {/* Rotate CW */}
          <button
            type="button"
            onClick={onRotateCw}
            style={{ backgroundColor: "#194B85" }}
            className="inline-flex items-center gap-1.5 px-3 h-8 text-xs font-bold hover:brightness-125 border border-white/40 text-white transition-all rounded-none cursor-pointer"
            title={t("lightbox.rotateCw")}
            aria-label={t("lightbox.rotateCw")}
          >
            <IconRotateCw size={16} />
            <span className="hidden sm:inline">{t("lightbox.rotateAction")}</span>
          </button>

          {/* Reset View Button */}
          {(scale > 1 || rotation !== 0) && (
            <button
              type="button"
              onClick={onResetAll}
              className="inline-flex items-center gap-1.5 px-3 h-8 text-xs font-bold bg-amber-500 hover:bg-amber-600 text-black border border-amber-300 transition-all rounded-none cursor-pointer shadow-md"
              title={t("lightbox.resetZoom")}
              aria-label={t("lightbox.resetZoom")}
            >
              <IconRefresh size={16} />
              <span>{t("lightbox.resetAction")}</span>
            </button>
          )}

          <span className="hidden sm:inline-block w-px h-5 bg-white/30 mx-0.5" />

          {/* Copy Image Link */}
          <div className="hidden sm:inline-block">
            <CopyButton
              text={currentImage}
              variant="button"
              label={t("lightbox.copyUrl")}
              copiedLabel={t("lightbox.copiedUrl")}
              className="!h-8 !min-h-[32px] px-3 !text-xs font-bold text-white border border-white/40 !bg-[#194B85] hover:!bg-[#153e6d]"
            />
          </div>
        </div>
      </div>

      {/* 3. Bottom Thumbnails Strip */}
      {hasMultiple && (
        <div
          style={{ backgroundColor: "rgba(7, 31, 56, 0.98)" }}
          className="shrink-0 flex items-center justify-center gap-2.5 px-4 py-3 border-t border-white/20 overflow-x-auto shadow-2xl"
          onClick={(e) => e.stopPropagation()}
        >
          {images.map((img, idx) => (
            <button
              key={`${img}-${idx}`}
              type="button"
              onClick={() => onSelect(idx)}
              className={`relative w-16 h-11 shrink-0 border-2 overflow-hidden cursor-pointer transition-all rounded-none shadow-md ${
                currentIndex === idx
                  ? "border-white ring-2 ring-white opacity-100 scale-105"
                  : "border-white/40 opacity-60 hover:opacity-100"
              }`}
              aria-label={`${t("lightbox.image")} ${idx + 1}`}
            >
              <img
                src={img}
                alt={`Thumbnail ${idx + 1}`}
                className="w-full h-full object-cover"
              />
            </button>
          ))}
        </div>
      )}
    </div>,
    document.body
  );
}

export default LightboxProvider;
