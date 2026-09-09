"use client";

import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useCallback, useMemo, useRef, useState } from "react";

export const LIST_PAGE_SIZES = [10, 25, 50, 100] as const;
export type ListPageSize = (typeof LIST_PAGE_SIZES)[number];
export type ListSortOrder = "asc" | "desc";
export type ListFilterValue = string | string[] | undefined;
export type ListFilterRecord = Record<string, ListFilterValue>;

export interface ListParams<TFilters extends ListFilterRecord = ListFilterRecord> {
  page: number;
  limit: ListPageSize;
  search: string;
  sort?: string;
  order: ListSortOrder;
  filters: TFilters;
}

export interface ListUrlSchema<TFilters extends ListFilterRecord> {
  defaultSort?: string;
  defaultOrder?: ListSortOrder;
  multi?: readonly (keyof TFilters & string)[];
  single?: readonly (keyof TFilters & string)[];
  allowedSorts?: readonly string[];
}

export interface ListActions<TFilters extends ListFilterRecord> {
  setSearch(value: string, immediate?: boolean): void;
  setFilter<K extends keyof TFilters>(key: K, value: TFilters[K]): void;
  setFilters(updates: Partial<TFilters>): void;
  removeFilterValue<K extends keyof TFilters>(key: K, value: string): void;
  clearFilters(): void;
  setSort(key: string): void;
  setPage(page: number): void;
  setLimit(limit: ListPageSize): void;
}

const positiveInt = (value: string | null, fallback: number) => {
  const parsed = Number(value);
  return Number.isInteger(parsed) && parsed > 0 ? parsed : fallback;
};

export function parseListParams<TFilters extends ListFilterRecord>(
  input: URLSearchParams,
  schema: ListUrlSchema<TFilters>
): ListParams<TFilters> {
  const rawLimit = positiveInt(input.get("limit"), 10);
  const limit = (
    LIST_PAGE_SIZES.includes(rawLimit as ListPageSize) ? rawLimit : 10
  ) as ListPageSize;

  const requestedSort = input.get("sort") || undefined;
  const sort =
    requestedSort && (schema.allowedSorts?.includes(requestedSort) ?? true)
      ? requestedSort
      : schema.defaultSort;

  const order = input.get("order") === "asc" ? "asc" : schema.defaultOrder ?? "desc";
  const filters: Record<string, ListFilterValue> = {};

  for (const key of schema.multi ?? []) {
    const values = [...new Set(input.getAll(key).map((v) => v.trim()).filter(Boolean))];
    if (values.length > 0) filters[key] = values;
  }

  for (const key of schema.single ?? []) {
    const value = input.get(key)?.trim();
    if (value) filters[key] = value;
  }

  return {
    page: positiveInt(input.get("page"), 1),
    limit,
    search: input.get("search")?.trim() ?? "",
    sort,
    order,
    filters: filters as TFilters,
  };
}

export function serializeListParams<TFilters extends ListFilterRecord>(
  params: ListParams<TFilters>
): URLSearchParams {
  const output = new URLSearchParams();
  if (params.page !== 1) output.set("page", String(params.page));
  if (params.limit !== 10) output.set("limit", String(params.limit));
  if (params.search) output.set("search", params.search);
  if (params.sort) output.set("sort", params.sort);
  if (params.order !== "desc") output.set("order", params.order);

  for (const key of Object.keys(params.filters).sort()) {
    const value = params.filters[key];
    if (Array.isArray(value)) {
      const values = [...value].sort();
      for (const item of values) output.append(key, item);
    } else if (value) {
      output.set(key, String(value));
    }
  }

  return output;
}

export interface UseListStateOptions<TFilters extends ListFilterRecord> {
  schema: ListUrlSchema<TFilters>;
  debounceMs?: number;
}

export function useListState<TFilters extends ListFilterRecord>(
  options: UseListStateOptions<TFilters>
): {
  params: ListParams<TFilters>;
  draftSearch: string;
  isDebouncing: boolean;
  actions: ListActions<TFilters>;
} {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const debounceMs = options.debounceMs ?? 350;

  const params = useMemo(() => {
    return parseListParams(searchParams, options.schema);
  }, [searchParams, options.schema]);

  const [draftSearch, setDraftSearch] = useState(params.search);
  const [isDebouncing, setIsDebouncing] = useState(false);
  const searchTimerRef = useRef<NodeJS.Timeout | null>(null);

  const [prevParamsSearch, setPrevParamsSearch] = useState(params.search);
  if (prevParamsSearch !== params.search) {
    setPrevParamsSearch(params.search);
    setDraftSearch(params.search);
  }

  const updateUrl = useCallback(
    (newParams: ListParams<TFilters>) => {
      const query = serializeListParams(newParams);
      const queryString = query.toString();
      const targetUrl = queryString ? `${pathname}?${queryString}` : pathname;
      router.replace(targetUrl, { scroll: false });
    },
    [pathname, router]
  );

  const setSearch = useCallback(
    (value: string, immediate = false) => {
      setDraftSearch(value);

      if (searchTimerRef.current) {
        clearTimeout(searchTimerRef.current);
        searchTimerRef.current = null;
      }

      if (immediate) {
        setIsDebouncing(false);
        const trimmed = value.trim();
        if (trimmed !== params.search) {
          updateUrl({
            ...params,
            search: trimmed,
            page: 1,
          });
        }
        return;
      }

      setIsDebouncing(true);
      searchTimerRef.current = setTimeout(() => {
        setIsDebouncing(false);
        const trimmed = value.trim();
        if (trimmed !== params.search) {
          updateUrl({
            ...params,
            search: trimmed,
            page: 1,
          });
        }
      }, debounceMs);
    },
    [debounceMs, params, updateUrl]
  );

  const setFilter = useCallback(
    <K extends keyof TFilters>(key: K, value: TFilters[K]) => {
      updateUrl({
        ...params,
        page: 1,
        filters: {
          ...params.filters,
          [key]: value,
        },
      });
    },
    [params, updateUrl]
  );

  const setFilters = useCallback(
    (updates: Partial<TFilters>) => {
      updateUrl({
        ...params,
        page: 1,
        filters: {
          ...params.filters,
          ...updates,
        },
      });
    },
    [params, updateUrl]
  );

  const removeFilterValue = useCallback(
    <K extends keyof TFilters>(key: K, value: string) => {
      const current = params.filters[key];
      if (Array.isArray(current)) {
        const next = current.filter((v) => v !== value);
        updateUrl({
          ...params,
          page: 1,
          filters: {
            ...params.filters,
            [key]: (next.length > 0 ? next : undefined) as TFilters[K],
          },
        });
      } else {
        updateUrl({
          ...params,
          page: 1,
          filters: {
            ...params.filters,
            [key]: undefined as unknown as TFilters[K],
          },
        });
      }
    },
    [params, updateUrl]
  );

  const clearFilters = useCallback(() => {
    updateUrl({
      ...params,
      page: 1,
      search: "",
      filters: {} as TFilters,
    });
    setDraftSearch("");
  }, [params, updateUrl]);

  const setSort = useCallback(
    (key: string) => {
      const nextOrder: ListSortOrder =
        params.sort === key && params.order === "asc" ? "desc" : "asc";
      updateUrl({
        ...params,
        sort: key,
        order: nextOrder,
      });
    },
    [params, updateUrl]
  );

  const setPage = useCallback(
    (page: number) => {
      updateUrl({
        ...params,
        page,
      });
    },
    [params, updateUrl]
  );

  const setLimit = useCallback(
    (limit: ListPageSize) => {
      updateUrl({
        ...params,
        page: 1,
        limit,
      });
    },
    [params, updateUrl]
  );

  return {
    params,
    draftSearch,
    isDebouncing,
    actions: {
      setSearch,
      setFilter,
      setFilters,
      removeFilterValue,
      clearFilters,
      setSort,
      setPage,
      setLimit,
    },
  };
}
