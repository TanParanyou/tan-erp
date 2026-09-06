"use client";

import { useEffect } from "react";

/**
 * Hook to lock body scroll when an overlay (Modal, Drawer) is open.
 * Restores the original overflow style upon unmount or close.
 */
export function useBodyScrollLock(isLocked: boolean): void {
  useEffect(() => {
    if (!isLocked || typeof document === "undefined") return;

    const originalOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";

    return () => {
      document.body.style.overflow = originalOverflow;
    };
  }, [isLocked]);
}
