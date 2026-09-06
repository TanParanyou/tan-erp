"use client";

import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";

export type SortOrder = "asc" | "desc";

export interface SortState {
  key: string | null;
  order: SortOrder;
}

export interface DataTableParams<TFilters extends Record<string, unknown> = Record<string, unknown>> {
  page: number;
  pageSize: number;
  search: string;
  sort: SortState;
  filters: TFilters;
}

export interface UseDataTableStateOptions<TFilters extends Record<string, unknown>> {
  defaultPageSize?: number;
  defaultSort?: SortState;
  defaultFilters?: TFilters;
  debounceMs?: number;
  syncUrl?: boolean;
}

export interface DataTableActions<TFilters extends Record<string, unknown>> {
  setPage: (page: number) => void;
  setPageSize: (size: number) => void;
  setSearch: (query: string, immediate?: boolean) => void;
  setSort: (key: string) => void;
  setFilter: <K extends keyof TFilters>(key: K, value: TFilters[K]) => void;
  setFilters: (updates: Partial<TFilters>) => void;
  clearFilters: () => void;
  resetAll: () => void;
}

/**
 * Hook to manage DataTable state with optional URL query sync and debounced search.
 */
export function useDataTableState<TFilters extends Record<string, unknown> = Record<string, unknown>>(
  options: UseDataTableStateOptions<TFilters> = {}
): {
  params: DataTableParams<TFilters>;
  draftSearch: string;
  isDebouncing: boolean;
  actions: DataTableActions<TFilters>;
} {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  const {
    defaultPageSize = 25,
    defaultSort = { key: null, order: "asc" },
    defaultFilters = {} as TFilters,
    debounceMs = 350,
    syncUrl = true,
  } = options;

  // Initial parse from URL
  const initialParams = useMemo<DataTableParams<TFilters>>(() => {
    if (!syncUrl || !searchParams) {
      return {
        page: 1,
        pageSize: defaultPageSize,
        search: "",
        sort: defaultSort,
        filters: defaultFilters,
      };
    }

    const page = parseInt(searchParams.get("page") || "1", 10);
    const pageSize = parseInt(searchParams.get("limit") || String(defaultPageSize), 10);
    const search = searchParams.get("search") || "";
    const sortKey = searchParams.get("sortKey") || defaultSort.key;
    const sortOrder = (searchParams.get("sortOrder") as SortOrder) || defaultSort.order;

    return {
      page: isNaN(page) || page < 1 ? 1 : page,
      pageSize: isNaN(pageSize) || pageSize < 1 ? defaultPageSize : pageSize,
      search,
      sort: { key: sortKey, order: sortOrder },
      filters: defaultFilters,
    };
  }, [syncUrl, searchParams, defaultPageSize, defaultSort, defaultFilters]);

  const [params, setParams] = useState<DataTableParams<TFilters>>(initialParams);
  const [draftSearch, setDraftSearch] = useState<string>(initialParams.search);
  const [isDebouncing, setIsDebouncing] = useState(false);
  const debounceTimerRef = useRef<NodeJS.Timeout | null>(null);

  // Sync to URL
  const updateUrl = useCallback(
    (newParams: DataTableParams<TFilters>) => {
      if (!syncUrl || !pathname) return;

      const sp = new URLSearchParams();
      if (newParams.page > 1) sp.set("page", String(newParams.page));
      if (newParams.pageSize !== defaultPageSize) sp.set("limit", String(newParams.pageSize));
      if (newParams.search) sp.set("search", newParams.search);
      if (newParams.sort.key) {
        sp.set("sortKey", newParams.sort.key);
        sp.set("sortOrder", newParams.sort.order);
      }

      const qs = sp.toString();
      const nextUrl = qs ? `${pathname}?${qs}` : pathname;
      router.replace(nextUrl);
    },
    [syncUrl, pathname, defaultPageSize, router]
  );

  const setPage = useCallback(
    (page: number) => {
      setParams((prev) => {
        const next = { ...prev, page };
        updateUrl(next);
        return next;
      });
    },
    [updateUrl]
  );

  const setPageSize = useCallback(
    (pageSize: number) => {
      setParams((prev) => {
        const next = { ...prev, pageSize, page: 1 };
        updateUrl(next);
        return next;
      });
    },
    [updateUrl]
  );

  const setSearch = useCallback(
    (query: string, immediate = false) => {
      setDraftSearch(query);

      if (debounceTimerRef.current) {
        clearTimeout(debounceTimerRef.current);
        debounceTimerRef.current = null;
      }

      if (immediate) {
        setIsDebouncing(false);
        const trimmed = query.trim();
        setParams((prev) => {
          if (prev.search === trimmed) return prev;
          const next = { ...prev, search: trimmed, page: 1 };
          updateUrl(next);
          return next;
        });
        return;
      }

      setIsDebouncing(true);
      debounceTimerRef.current = setTimeout(() => {
        setIsDebouncing(false);
        const trimmed = query.trim();
        setParams((prev) => {
          if (prev.search === trimmed) return prev;
          const next = { ...prev, search: trimmed, page: 1 };
          updateUrl(next);
          return next;
        });
      }, debounceMs);
    },
    [debounceMs, updateUrl]
  );

  const setSort = useCallback(
    (key: string) => {
      setParams((prev) => {
        let nextOrder: SortOrder = "asc";
        if (prev.sort.key === key) {
          nextOrder = prev.sort.order === "asc" ? "desc" : "asc";
        }
        const next = {
          ...prev,
          sort: { key, order: nextOrder },
          page: 1,
        };
        updateUrl(next);
        return next;
      });
    },
    [updateUrl]
  );

  const setFilter = useCallback(
    <K extends keyof TFilters>(key: K, value: TFilters[K]) => {
      setParams((prev) => {
        const next = {
          ...prev,
          page: 1,
          filters: {
            ...prev.filters,
            [key]: value,
          },
        };
        updateUrl(next);
        return next;
      });
    },
    [updateUrl]
  );

  const setFilters = useCallback(
    (updates: Partial<TFilters>) => {
      setParams((prev) => {
        const next = {
          ...prev,
          page: 1,
          filters: {
            ...prev.filters,
            ...updates,
          },
        };
        updateUrl(next);
        return next;
      });
    },
    [updateUrl]
  );

  const clearFilters = useCallback(() => {
    setParams((prev) => {
      const next = {
        ...prev,
        page: 1,
        filters: defaultFilters,
      };
      updateUrl(next);
      return next;
    });
  }, [defaultFilters, updateUrl]);

  const resetAll = useCallback(() => {
    setDraftSearch("");
    const next = {
      page: 1,
      pageSize: defaultPageSize,
      search: "",
      sort: defaultSort,
      filters: defaultFilters,
    };
    setParams(next);
    updateUrl(next);
  }, [defaultPageSize, defaultSort, defaultFilters, updateUrl]);

  useEffect(() => {
    return () => {
      if (debounceTimerRef.current) {
        clearTimeout(debounceTimerRef.current);
      }
    };
  }, []);

  const actions = useMemo(
    () => ({
      setPage,
      setPageSize,
      setSearch,
      setSort,
      setFilter,
      setFilters,
      clearFilters,
      resetAll,
    }),
    [setPage, setPageSize, setSearch, setSort, setFilter, setFilters, clearFilters, resetAll]
  );

  return {
    params,
    draftSearch,
    isDebouncing,
    actions,
  };
}
