"use client";

import { useState, useCallback } from "react";

export interface UseGalleryLightboxReturn<T> {
  isOpen: boolean;
  currentIndex: number;
  currentItem: T | null;
  openAt: (index: number) => void;
  close: () => void;
  next: () => void;
  prev: () => void;
  goTo: (index: number) => void;
  setIsOpen: (isOpen: boolean) => void;
}

/**
 * Central reusable hook to manage carousel-style lightbox states.
 * - Supports opening at a specific index
 * - Next/Previous wrap-around carousel navigation
 * - Safe boundary checks
 * - Auto-derives currentItem based on current index and generic type T
 */
export function useGalleryLightbox<T = unknown>(
  items: T[] = [],
  defaultIndex: number = 0
): UseGalleryLightboxReturn<T> {
  const [isOpen, setIsOpen] = useState(false);
  const [currentIndex, setCurrentIndex] = useState(defaultIndex);

  const openAt = useCallback(
    (index: number) => {
      if (items.length === 0) {
        setCurrentIndex(0);
      } else {
        const safeIndex = Math.max(0, Math.min(index, items.length - 1));
        setCurrentIndex(safeIndex);
      }
      setIsOpen(true);
    },
    [items.length]
  );

  const close = useCallback(() => {
    setIsOpen(false);
  }, []);

  const next = useCallback(() => {
    if (items.length <= 1) return;
    setCurrentIndex((prev) => (prev < items.length - 1 ? prev + 1 : 0));
  }, [items.length]);

  const prev = useCallback(() => {
    if (items.length <= 1) return;
    setCurrentIndex((prev) => (prev > 0 ? prev - 1 : items.length - 1));
  }, [items.length]);

  const goTo = useCallback(
    (index: number) => {
      if (index >= 0 && index < items.length) {
        setCurrentIndex(index);
      }
    },
    [items.length]
  );

  const safeIndex = items.length > 0 && currentIndex >= items.length ? 0 : currentIndex;
  const currentItem = items.length > 0 ? items[safeIndex] ?? null : null;

  return {
    isOpen,
    currentIndex: safeIndex,
    currentItem,
    openAt,
    close,
    next,
    prev,
    goTo,
    setIsOpen,
  };
}
