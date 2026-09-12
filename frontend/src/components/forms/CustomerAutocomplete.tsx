"use client";

import React, { useState, useEffect, useRef, useId, useMemo, useCallback } from "react";
import { useTranslations } from "next-intl";
import { useCustomerList } from "@/features/customers/api/customer-queries";
import { useDebounce } from "@/hooks/useDebounce";
import { useIsMobile } from "@/hooks/useMediaQuery";
import { Button } from "@/components/ui/Button";
import { IconSearch, IconClose, IconCheckCircle, IconArrowLeft } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";
import type { CustomerListItemResponse } from "@/lib/api/api-client";

export interface CustomerAutocompleteProps {
  value?: string | null;
  onChange: (customerId: string) => void;
  label?: string;
  error?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
}

/**
 * CustomerAutocomplete - Atelier Architectural Navy Sharp
 * Debounced customer search with dual-mode responsive layout:
 * - Desktop: Uniform 44px input with floating dense architectural listbox and keyboard navigation
 * - Mobile: Dedicated Full-screen Search View preventing keyboard jumping and preserving cursor focus
 * - Loading indicator visible on both viewport sizes
 * - Selected State: Crisp architectural summary card with 'Change Customer' action
 */
export function CustomerAutocomplete({
  value,
  onChange,
  label,
  error,
  placeholder,
  required,
  disabled = false,
  className,
}: CustomerAutocompleteProps) {
  const t = useTranslations("opportunities");
  const isMobile = useIsMobile();
  const generatedId = useId();
  const inputId = `customer-autocomplete-${generatedId}`;
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

  // Fetch active customers
  const { data: customerData, isLoading } = useCustomerList({
    search: debouncedQuery.trim() || undefined,
    status: "active",
    limit: 20,
  });

  const customerList: CustomerListItemResponse[] = useMemo(
    () => customerData?.items ?? [],
    [customerData?.items]
  );

  // Find currently selected customer details if value is present
  const selectedCustomer = useMemo(() => {
    if (!value) return null;
    return customerList.find((c) => c.id === value) ?? null;
  }, [value, customerList]);

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
    (customer: CustomerListItemResponse) => {
      if (!customer.id) return;
      onChange(customer.id);
      setIsOpen(false);
      setIsMobileFullscreenOpen(false);
      setQuery("");
      setActiveIndex(-1);
    },
    [onChange]
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
    if (!isOpen || customerList.length === 0) {
      if (e.key === "ArrowDown" && customerList.length > 0) {
        setIsOpen(true);
      }
      return;
    }

    switch (e.key) {
      case "ArrowDown":
        e.preventDefault();
        setActiveIndex((prev) => (prev < customerList.length - 1 ? prev + 1 : 0));
        break;
      case "ArrowUp":
        e.preventDefault();
        setActiveIndex((prev) => (prev > 0 ? prev - 1 : customerList.length - 1));
        break;
      case "Enter":
        e.preventDefault();
        if (activeIndex >= 0 && activeIndex < customerList.length) {
          handleSelect(customerList[activeIndex]);
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

  const resolvedLabel = label || t("customer");
  const resolvedPlaceholder = placeholder || t("customerSearchPlaceholder");

  return (
    <div ref={containerRef} className={cn("erp-form-group relative flex flex-col gap-1.5 w-full", className)}>
      {resolvedLabel && (
        <label htmlFor={inputId} className="erp-label">
          {resolvedLabel}
          {required && <span className="erp-label-required">*</span>}
        </label>
      )}

      {/* State 1: Selected Customer Summary Card (Uniform min-h-[44px]) */}
      {value ? (
        <div
          role="region"
          aria-label={resolvedLabel}
          className="flex flex-col sm:flex-row sm:items-center justify-between gap-2.5 sm:gap-3 border border-erp-navy/30 bg-erp-surface p-3 min-h-[44px] transition-colors shadow-sm"
        >
          <div className="flex items-center gap-2.5 min-w-0">
            <IconCheckCircle size={18} className="text-erp-navy shrink-0" />
            <div className="flex flex-col min-w-0">
              <span className="font-semibold text-sm text-erp-text-main truncate">
                {selectedCustomer
                  ? `${selectedCustomer.code ? `[${selectedCustomer.code}] ` : ""}${
                      selectedCustomer.displayNameTh || selectedCustomer.displayNameEn || "-"
                    }`
                  : `ID: ${value}`}
              </span>
              <span className="text-xs text-erp-text-muted">
                {t("customer")}
              </span>
            </div>
          </div>

          {!disabled && (
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleClear}
              className="shrink-0 text-xs font-semibold self-start sm:self-auto"
            >
              {t("changeCustomer")}
            </Button>
          )}
        </div>
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
              placeholder={resolvedPlaceholder}
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
                  aria-label={t("loadingMore")}
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
              aria-label={resolvedLabel}
              className={cn(
                "absolute z-50 top-full left-0 right-0 mt-1 max-h-64 overflow-y-auto",
                "bg-erp-surface border border-erp-border shadow-lg rounded-none py-1 list-none m-0 divide-y divide-erp-border-subtle"
              )}
            >
              {isLoading && customerList.length === 0 ? (
                <li className="px-3 py-3 text-sm text-erp-text-muted text-center italic">
                  {t("loadingMore")}
                </li>
              ) : customerList.length === 0 ? (
                <li className="px-3 py-3 text-sm text-erp-text-muted text-center italic">
                  {t("noCustomerFound")}
                </li>
              ) : (
                customerList.map((c, idx) => {
                  const isHighlighted = idx === activeIndex;
                  const displayName = c.displayNameTh || c.displayNameEn || "-";
                  return (
                    <li
                      key={c.id ?? `customer-${idx}`}
                      id={`${inputId}-opt-${idx}`}
                      role="option"
                      aria-selected={isHighlighted}
                      onClick={() => handleSelect(c)}
                      onMouseEnter={() => setActiveIndex(idx)}
                      className={cn(
                        "px-3 py-2.5 min-h-[44px] cursor-pointer transition-colors flex items-center justify-between gap-3 text-left",
                        isHighlighted ? "bg-erp-navy text-white" : "hover:bg-erp-surface-subtle text-erp-text-main"
                      )}
                    >
                      <div className="flex flex-col min-w-0">
                        <span className="font-semibold text-sm truncate">{displayName}</span>
                        {c.code && (
                          <span
                            className={cn(
                              "text-xs font-mono tracking-wider",
                              isHighlighted ? "text-white/80" : "text-erp-text-muted"
                            )}
                          >
                            {c.code}
                          </span>
                        )}
                      </div>
                      <span
                        className={cn(
                          "text-xs uppercase px-1.5 py-0.5 shrink-0 font-medium",
                          isHighlighted ? "bg-white/20 text-white" : "bg-erp-surface-subtle text-erp-text-muted"
                        )}
                      >
                        {c.status}
                      </span>
                    </li>
                  );
                })
              )}
            </ul>
          )}
        </div>
      )}

      {/* Mobile-dedicated Full-Screen Search View (Stable height, perfect focus, zero jumping) */}
      {isMobile && isMobileFullscreenOpen && (
        <div
          role="dialog"
          aria-modal="true"
          aria-label={resolvedLabel}
          className="fixed inset-0 z-50 bg-erp-surface flex flex-col w-full h-full"
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
                placeholder={resolvedPlaceholder}
                autoComplete="off"
                className="erp-input w-full min-h-[44px] pl-9 pr-16 text-sm text-erp-text-main rounded-none outline-none focus:border-erp-navy focus:ring-1 focus:ring-erp-navy"
              />

              <div className="absolute right-3 flex items-center gap-1.5">
                {isLoading && (
                  <div className="animate-spin text-erp-navy" aria-label={t("loadingMore")}>
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
            {isLoading && customerList.length === 0 ? (
              <div className="p-6 text-center text-sm text-erp-text-muted italic">
                {t("loadingMore")}
              </div>
            ) : customerList.length === 0 ? (
              <div className="p-6 text-center text-sm text-erp-text-muted italic">
                {t("noCustomerFound")}
              </div>
            ) : (
              customerList.map((c, idx) => {
                const displayName = c.displayNameTh || c.displayNameEn || "-";
                return (
                  <button
                    key={c.id ?? `m-customer-${idx}`}
                    type="button"
                    onClick={() => handleSelect(c)}
                    className="w-full px-4 py-3.5 min-h-[52px] flex items-center justify-between gap-3 text-left hover:bg-erp-surface-subtle transition-colors focus:bg-erp-surface-subtle outline-none"
                  >
                    <div className="flex flex-col min-w-0">
                      <span className="font-semibold text-sm text-erp-text-main truncate">
                        {displayName}
                      </span>
                      {c.code && (
                        <span className="text-xs font-mono text-erp-text-muted">
                          {c.code}
                        </span>
                      )}
                    </div>
                    <span className="text-xs uppercase px-2 py-0.5 font-medium bg-erp-surface-subtle text-erp-text-muted shrink-0">
                      {c.status}
                    </span>
                  </button>
                );
              })
            )}
          </div>
        </div>
      )}

      {error && (
        <p className="erp-error-text" role="alert">
          {error}
        </p>
      )}
    </div>
  );
}

export default CustomerAutocomplete;
