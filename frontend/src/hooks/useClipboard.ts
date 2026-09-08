"use client";

import { useState, useCallback, useRef, useEffect } from "react";

export interface UseClipboardOptions {
  timeout?: number;
  onCopySuccess?: (text: string) => void;
}

/**
 * useClipboard: Hook for copying text to the clipboard with temporary copied state.
 */
export function useClipboard(options: UseClipboardOptions = {}) {
  const { timeout = 2000, onCopySuccess } = options;
  const [copied, setCopied] = useState(false);
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    return () => {
      if (timerRef.current) {
        clearTimeout(timerRef.current);
      }
    };
  }, []);

  const copy = useCallback(
    async (text: string): Promise<boolean> => {
      if (!navigator?.clipboard) {
        return false;
      }

      try {
        await navigator.clipboard.writeText(text);
        setCopied(true);
        onCopySuccess?.(text);

        if (timerRef.current) {
          clearTimeout(timerRef.current);
        }

        timerRef.current = setTimeout(() => {
          setCopied(false);
        }, timeout);

        return true;
      } catch {
        setCopied(false);
        return false;
      }
    },
    [timeout, onCopySuccess]
  );

  const reset = useCallback(() => {
    if (timerRef.current) {
      clearTimeout(timerRef.current);
    }
    setCopied(false);
  }, []);

  return { copy, copied, reset };
}
