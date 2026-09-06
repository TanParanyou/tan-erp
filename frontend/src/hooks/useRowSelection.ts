"use client";

import { useState, useCallback, useMemo } from "react";

export interface UseRowSelectionReturn<TId extends string | number = string> {
  selectedIds: Set<TId>;
  selectedCount: number;
  isSelected: (id: TId) => boolean;
  toggle: (id: TId) => void;
  selectAll: (allIds: TId[]) => void;
  clearSelection: () => void;
  isAllSelected: (allIds: TId[]) => boolean;
  isPartiallySelected: (allIds: TId[]) => boolean;
}

/**
 * Hook to manage multi-row selection for ERP DataTables.
 */
export function useRowSelection<TId extends string | number = string>(
  initialSelected: TId[] = []
): UseRowSelectionReturn<TId> {
  const [selectedIds, setSelectedIds] = useState<Set<TId>>(() => new Set(initialSelected));

  const isSelected = useCallback(
    (id: TId) => selectedIds.has(id),
    [selectedIds]
  );

  const toggle = useCallback((id: TId) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }
      return next;
    });
  }, []);

  const selectAll = useCallback((allIds: TId[]) => {
    setSelectedIds((prev) => {
      const allSelected = allIds.length > 0 && allIds.every((id) => prev.has(id));
      if (allSelected) {
        return new Set();
      }
      return new Set(allIds);
    });
  }, []);

  const clearSelection = useCallback(() => {
    setSelectedIds(new Set());
  }, []);

  const isAllSelected = useCallback(
    (allIds: TId[]) => allIds.length > 0 && allIds.every((id) => selectedIds.has(id)),
    [selectedIds]
  );

  const isPartiallySelected = useCallback(
    (allIds: TId[]) => {
      if (allIds.length === 0) return false;
      const some = allIds.some((id) => selectedIds.has(id));
      const all = allIds.every((id) => selectedIds.has(id));
      return some && !all;
    },
    [selectedIds]
  );

  return useMemo(
    () => ({
      selectedIds,
      selectedCount: selectedIds.size,
      isSelected,
      toggle,
      selectAll,
      clearSelection,
      isAllSelected,
      isPartiallySelected,
    }),
    [selectedIds, isSelected, toggle, selectAll, clearSelection, isAllSelected, isPartiallySelected]
  );
}
