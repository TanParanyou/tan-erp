"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { deleteDraft, readDraft, writeDraft, type StoredDraft } from "@/lib/drafts/draft-store";

const SAVE_DEBOUNCE_MS = 600;

/** loading: reading storage; found: an earlier draft awaits a decision; ready: safe to autosave. */
export type DraftRecoveryStatus = "loading" | "found" | "ready";

interface LoadedDraft<T> {
  key: string;
  draft: StoredDraft<T> | null;
}

/**
 * Debounced local autosave plus recovery of an earlier unsaved draft, keyed per user/entity.
 * Storage failures are ignored on purpose: the draft cache is best-effort and never blocks the form.
 */
export function useDraftRecovery<T>(key: string | null) {
  const [loaded, setLoaded] = useState<LoadedDraft<T> | null>(null);
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    if (!key) return;
    let cancelled = false;
    readDraft<T>(key)
      .then((draft) => {
        if (!cancelled) setLoaded({ key, draft });
      })
      .catch(() => {
        if (!cancelled) setLoaded({ key, draft: null });
      });
    return () => {
      cancelled = true;
    };
  }, [key]);

  useEffect(
    () => () => {
      if (timerRef.current) clearTimeout(timerRef.current);
    },
    []
  );

  const found = key !== null && loaded?.key === key ? loaded.draft : null;
  const status: DraftRecoveryStatus =
    key === null || loaded?.key === key ? (found ? "found" : "ready") : "loading";

  const save = useCallback(
    (payload: T) => {
      if (!key) return;
      if (timerRef.current) clearTimeout(timerRef.current);
      timerRef.current = setTimeout(() => {
        void writeDraft(key, payload).catch(() => undefined);
      }, SAVE_DEBOUNCE_MS);
    },
    [key]
  );

  const clear = useCallback(() => {
    if (timerRef.current) clearTimeout(timerRef.current);
    if (key) void deleteDraft(key).catch(() => undefined);
  }, [key]);

  /** Hands the found draft to the caller and resumes autosave. */
  const accept = useCallback((): T | null => {
    const payload = found?.payload ?? null;
    if (key) setLoaded({ key, draft: null });
    return payload;
  }, [found, key]);

  const discard = useCallback(() => {
    clear();
    if (key) setLoaded({ key, draft: null });
  }, [clear, key]);

  return { status, savedAt: found?.savedAt ?? null, save, clear, accept, discard };
}
