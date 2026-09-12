"use client";

import React, { useState, useEffect, useRef, useId, useCallback } from "react";
import { createPortal } from "react-dom";
import { useDebounce } from "@/hooks/useDebounce";
import { useIsMobile } from "@/hooks/useMediaQuery";
import { useScrollLock } from "@/hooks/useScrollLock";
import { IconSearch, IconClose, IconArrowLeft } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface EntityAutocompleteProps<T> {
  value?: string | null;
  onChange: (id: string) => void;
  label?: string;
  placeholder?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;

  // Data & Query
  items: T[];
  isLoading?: boolean;
  emptyText?: string;
  loadingText?: string;
  onSearchChange?: (query: string) => void;

  // Key & Extraction
  getItemKey: (item: T) => string;

  // Custom Renderers
  renderSelectedCard: (onClear: () => void) => React.ReactNode;
  renderListItem: (item: T, isHighlighted: boolean, isMobile: boolean) => React.ReactNode;
}

/**
 * EntityAutocomplete - Atelier Architectural Navy Sharp
 *
 * Core reusable Autocomplete engine:
 * - Desktop: 44px uniform height input, floating dense dropdown with full keyboard navigation (Arrows, Enter, Esc), outside-click dismissal
 * - Mobile: Fullscreen dedicated overlay via Portal (z-[100]) with auto-focus input, back button, clear action, and touch-optimized list
 * - Selected State: Extensible via `renderSelectedCard`
 * - List Items: Extensible via `renderListItem`
 */
export function EntityAutocomplete<T>({
  value,
  onChange,
  label,
  placeholder,
  error,
  required,
  disabled = false,
  className,
  items,
  isLoading = false,
  emptyText = "No results found",
  loadingText = "Loading...",
  onSearchChange,
  getItemKey,
  renderSelectedCard,
  renderListItem,
}: EntityAutocompleteProps<T>) {
  const isMobile = useIsMobile();
  const generatedId = useId();
  const inputId = `entity-autocomplete-${generatedId}`;
  const listboxId = `${inputId}-listbox`;

  const [query, setQuery] = useState("");
  const [isOpen, setIsOpen] = useState(false);
  const [isMobileFullscreenOpen, setIsMobileFullscreenOpen] = useState(false);
  const [activeIndex, setActiveIndex] = useState(-1);

  const containerRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const mobileInputRef = useRef<HTMLInputElement>(null);
  const listboxRef = useRef<HTMLUListElement>(null);

  const debouncedQuery = useDebounce(query, 300);

  // Notify parent on debounced search change
  useEffect(() => {
    onSearchChange?.(debouncedQuery);
  }, [debouncedQuery, onSearchChange]);

  // Lock scroll on mobile fullscreen
  useScrollLock(isMobile && isMobileFullscreenOpen);

  // Close listbox on outside click (desktop)
  useEffect(() => {
    const handleOutsideClick = (e: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        setIsOpen(false);
        setActiveIndex(-1);
      }
    };
    document.addEventListener("mousedown", handleOutsideClick);
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, []);

  // Focus mobile input when fullscreen search opens
  useEffect(() => {
    if (isMobileFullscreenOpen) {
      const timer = setTimeout(() => {
        mobileInputRef.current?.focus();
      }, 50);
      return () => clearTimeout(timer);
    }
  }, [isMobileFullscreenOpen]);

  const handleSelect = useCallback(
    (item: T) => {
      const key = getItemKey(item);
      if (!key) return;
      onChange(key);
      setIsOpen(false);
      setIsMobileFullscreenOpen(false);
      setQuery("");
      setActiveIndex(-1);
    },
    [getItemKey, onChange]
  );

  const handleClear = useCallback(() => {
    onChange("");
    setQuery("");
    setIsOpen(false);
    setActiveIndex(-1);
    setTimeout(() => {
      if (isMobile) {
        setIsMobileFullscreenOpen(true);
      } else {
        inputRef.current?.focus();
      }
    }, 50);
  }, [isMobile, onChange]);

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (!isOpen || items.length === 0) {
      if (e.key === "ArrowDown" && items.length > 0) {
        setIsOpen(true);
      }
      return;
    }

    switch (e.key) {
      case "ArrowDown":
        e.preventDefault();
        setActiveIndex((prev) => (prev < items.length - 1 ? prev + 1 : 0));
        break;
      case "ArrowUp":
        e.preventDefault();
        setActiveIndex((prev) => (prev > 0 ? prev - 1 : items.length - 1));
        break;
      case "Enter":
        e.preventDefault();
        if (activeIndex >= 0 && activeIndex < items.length) {
          handleSelect(items[activeIndex]);
        }
        break;
      case "Escape":
      case "Tab":
        setIsOpen(false);
        setActiveIndex(-1);
        break;
    }
  };

  // Scroll active item into view
  useEffect(() => {
    if (activeIndex >= 0 && listboxRef.current) {
      const activeOption = listboxRef.current.children[activeIndex] as HTMLElement | undefined;
      if (activeOption && typeof activeOption.scrollIntoView === "function") {
        activeOption.scrollIntoView({ block: "nearest" });
      }
    }
  }, [activeIndex]);

  return (
    <div ref={containerRef} className={cn("erp-form-group relative flex flex-col gap-1.5 w-full", className)}>
      {label && (
        <label htmlFor={inputId} className="erp-label">
          {label}
          {required && <span className="erp-label-required">*</span>}
        </label>
      )}

      {/* State 1: Selected Item Summary Card */}
      {value ? (
        renderSelectedCard(handleClear)
      ) : (
        /* State 2: Search Input (Strict min-h-[44px] uniform with other form controls) */
        <div className="relative w-full">
          <div className="relative flex items-center">
            <div className="pointer-events-none absolute left-3 text-erp-text-muted flex items-center">
              <IconSearch size={16} />
            </div>

            <input
              ref={inputRef}
              id={inputId}
              type="text"
              role="combobox"
              aria-autocomplete="list"
              aria-expanded={isOpen}
              aria-controls={listboxId}
              aria-activedescendant={activeIndex >= 0 ? `${inputId}-opt-${activeIndex}` : undefined}
              disabled={disabled}
              value={query}
              onChange={(e) => {
                setQuery(e.target.value);
                setIsOpen(true);
              }}
              onFocus={() => {
                if (isMobile) {
                  setIsMobileFullscreenOpen(true);
                } else {
                  setIsOpen(true);
                }
              }}
              onClick={() => {
                if (isMobile) {
                  setIsMobileFullscreenOpen(true);
                }
              }}
              onKeyDown={handleKeyDown}
              placeholder={placeholder}
              autoComplete="off"
              className={cn(
                "erp-input w-full min-h-[44px] pl-9 pr-16 text-sm text-erp-text-main rounded-none outline-none transition-colors",
                "focus:border-erp-navy focus:ring-1 focus:ring-erp-navy",
                "disabled:cursor-not-allowed disabled:bg-erp-surface-subtle disabled:opacity-60",
                error ? "border-red-600 focus:border-red-600 focus:ring-red-600" : ""
              )}
            />

            {/* Right Action: Loading Spinner & Clear Button */}
            <div className="absolute right-3 flex items-center gap-1.5">
              {isLoading && (
                <div
                  className="animate-spin text-erp-navy inline-flex items-center justify-center"
                  aria-label={loadingText}
                >
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                    <path d="M21 12a9 9 0 1 1-6.219-8.56" />
                  </svg>
                </div>
              )}

              {query && !disabled && (
                <button
                  type="button"
                  onClick={() => {
                    setQuery("");
                    setActiveIndex(-1);
                  }}
                  aria-label="Clear search"
                  className="text-erp-text-muted hover:text-erp-navy transition-colors p-0.5"
                >
                  <IconClose size={16} />
                </button>
              )}
            </div>
          </div>

          {/* Desktop Autocomplete Dropdown Listbox */}
          {!isMobile && isOpen && (
            <ul
              ref={listboxRef}
              id={listboxId}
              role="listbox"
              aria-label={label}
              className={cn(
                "absolute z-50 top-full left-0 right-0 mt-1 max-h-64 overflow-y-auto",
                "bg-erp-surface border border-erp-border shadow-lg rounded-none py-1 list-none m-0 divide-y divide-erp-border-subtle"
              )}
            >
              {isLoading && items.length === 0 ? (
                <li className="px-3 py-3 text-sm text-erp-text-muted text-center italic">
                  {loadingText}
                </li>
              ) : items.length === 0 ? (
                <li className="px-3 py-3 text-sm text-erp-text-muted text-center italic">
                  {emptyText}
                </li>
              ) : (
                items.map((item, idx) => {
                  const isHighlighted = idx === activeIndex;
                  const key = getItemKey(item);
                  return (
                    <li
                      key={key || `item-${idx}`}
                      id={`${inputId}-opt-${idx}`}
                      role="option"
                      aria-selected={isHighlighted}
                      onClick={() => handleSelect(item)}
                      onMouseDown={(e) => {
                        e.preventDefault();
                        handleSelect(item);
                      }}
                      onMouseEnter={() => setActiveIndex(idx)}
                      className={cn(
                        "px-3 py-2.5 min-h-[44px] cursor-pointer transition-colors flex items-center justify-between gap-3 text-left",
                        isHighlighted ? "bg-erp-navy text-white" : "hover:bg-erp-surface-subtle text-erp-text-main"
                      )}
                    >
                      {renderListItem(item, isHighlighted, false)}
                    </li>
                  );
                })
              )}
            </ul>
          )}
        </div>
      )}

      {/* Mobile Fullscreen Search View (Always Portal to body to avoid modal clipping/backdrop-filter issues) */}
      {isMobile && isMobileFullscreenOpen && typeof document !== "undefined" && createPortal(
        <div
          role="dialog"
          aria-modal="true"
          aria-label={label}
          className="fixed inset-0 z-[9999] bg-erp-surface flex flex-col w-full h-full"
        >
          {/* Top Sticky Header */}
          <div className="flex items-center gap-2 border-b border-erp-border px-3 py-2.5 bg-erp-surface shadow-sm">
            <button
              type="button"
              onClick={() => setIsMobileFullscreenOpen(false)}
              aria-label="Back"
              className="p-2 min-h-[44px] min-w-[44px] flex items-center justify-center text-erp-text-main hover:text-erp-navy transition-colors"
            >
              <IconArrowLeft size={20} />
            </button>

            <div className="relative flex-1 flex items-center">
              <div className="pointer-events-none absolute left-3 text-erp-text-muted flex items-center">
                <IconSearch size={16} />
              </div>

              <input
                ref={mobileInputRef}
                type="text"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder={placeholder}
                autoComplete="off"
                className="erp-input w-full min-h-[44px] pl-9 pr-16 text-sm text-erp-text-main rounded-none outline-none focus:border-erp-navy focus:ring-1 focus:ring-erp-navy"
              />

              <div className="absolute right-3 flex items-center gap-1.5">
                {isLoading && (
                  <div className="animate-spin text-erp-navy" aria-label={loadingText}>
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                      <path d="M21 12a9 9 0 1 1-6.219-8.56" />
                    </svg>
                  </div>
                )}

                {query && (
                  <button
                    type="button"
                    onClick={() => {
                      setQuery("");
                      mobileInputRef.current?.focus();
                    }}
                    aria-label="Clear query"
                    className="p-1 text-erp-text-muted hover:text-erp-navy"
                  >
                    <IconClose size={16} />
                  </button>
                )}
              </div>
            </div>
          </div>

          {/* Full-Screen Scrollable List */}
          <div className="flex-1 overflow-y-auto divide-y divide-erp-border-subtle bg-erp-surface p-2">
            {isLoading && items.length === 0 ? (
              <div className="p-6 text-center text-sm text-erp-text-muted italic">
                {loadingText}
              </div>
            ) : items.length === 0 ? (
              <div className="p-6 text-center text-sm text-erp-text-muted italic">
                {emptyText}
              </div>
            ) : (
              items.map((item, idx) => {
                const key = getItemKey(item);
                return (
                  <button
                    key={key || `m-item-${idx}`}
                    type="button"
                    onClick={() => handleSelect(item)}
                    className="w-full px-4 py-3.5 min-h-[52px] flex items-center justify-between gap-3 text-left hover:bg-erp-surface-subtle transition-colors focus:bg-erp-surface-subtle outline-none"
                  >
                    {renderListItem(item, false, true)}
                  </button>
                );
              })
            )}
          </div>
        </div>,
        document.body
      )}

      {error && (
        <p className="erp-error-text" role="alert">
          {error}
        </p>
      )}
    </div>
  );
}

export default EntityAutocomplete;
