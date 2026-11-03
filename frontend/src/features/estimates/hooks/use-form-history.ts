"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import type { DefaultValues, FieldValues, UseFormReturn } from "react-hook-form";

const HISTORY_LIMIT = 50;
const GROUP_EDITS_MS = 600;

const snapshot = <T,>(value: T): T => structuredClone(value);

/**
 * Whole-form undo/redo. Edits made within GROUP_EDITS_MS collapse into one step so typing is not undone per character.
 * Call `rebase` after any programmatic reset (open, server save) so those are not treated as user edits.
 */
export function useFormHistory<T extends FieldValues>({ watch, getValues, reset }: UseFormReturn<T>) {
  const past = useRef<T[]>([]);
  const future = useRef<T[]>([]);
  const present = useRef<T>(snapshot(getValues()));
  const timer = useRef<ReturnType<typeof setTimeout> | null>(null);
  const [, rerender] = useState(0);

  const cancelTimer = useCallback(() => {
    if (timer.current) clearTimeout(timer.current);
    timer.current = null;
  }, []);

  const commit = useCallback(() => {
    cancelTimer();
    const next = snapshot(getValues());
    if (JSON.stringify(next) === JSON.stringify(present.current)) return;
    past.current.push(present.current);
    if (past.current.length > HISTORY_LIMIT) past.current.shift();
    present.current = next;
    future.current = [];
    rerender((n) => n + 1);
  }, [cancelTimer, getValues]);

  useEffect(() => {
    const subscription = watch(() => {
      cancelTimer();
      timer.current = setTimeout(commit, GROUP_EDITS_MS);
    });
    return () => {
      subscription.unsubscribe();
      cancelTimer();
    };
  }, [watch, commit, cancelTimer]);

  const jumpTo = useCallback((target: T) => {
    present.current = target;
    reset(snapshot(target) as DefaultValues<T>, { keepDefaultValues: true });
    cancelTimer();
    rerender((n) => n + 1);
  }, [reset, cancelTimer]);

  const undo = useCallback(() => {
    commit();
    const previous = past.current.pop();
    if (!previous) return;
    future.current.push(present.current);
    jumpTo(previous);
  }, [commit, jumpTo]);

  const redo = useCallback(() => {
    commit();
    const next = future.current.pop();
    if (!next) return;
    past.current.push(present.current);
    jumpTo(next);
  }, [commit, jumpTo]);

  const rebase = useCallback(() => {
    cancelTimer();
    present.current = snapshot(getValues());
    past.current = [];
    future.current = [];
    rerender((n) => n + 1);
  }, [cancelTimer, getValues]);

  return { undo, redo, rebase, canUndo: past.current.length > 0, canRedo: future.current.length > 0 };
}
