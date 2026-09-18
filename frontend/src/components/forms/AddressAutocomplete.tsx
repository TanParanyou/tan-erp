"use client";

import React, { useState, useEffect, useRef, useId, useCallback } from "react";
import { useTranslations } from "next-intl";
import { apiClient, type AddressSearchResultItem } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

export interface SelectedAddress {
  subdistrict: string;
  district: string;
  province: string;
  postalCode: string;
  countryCode: string;
  latitude?: number | null;
  longitude?: number | null;
}

export interface AddressAutocompleteProps {
  id?: string;
  label?: string;
  hint?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  error?: string;
  onSelect: (address: SelectedAddress) => void;
  className?: string;
}

export function AddressAutocomplete({
  id: customId,
  label,
  hint,
  placeholder,
  required = false,
  disabled = false,
  error,
  onSelect,
  className = "",
}: AddressAutocompleteProps) {
  const generatedId = useId();
  const inputId = customId || `address-autocomplete-${generatedId}`;
  const listboxId = `${inputId}-listbox`;

  const t = useTranslations("common.addressAutocomplete");
  const { selectedMembership } = useSelectedMembership();

  const [query, setQuery] = useState("");
  const [items, setItems] = useState<AddressSearchResultItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [activeIndex, setActiveIndex] = useState(-1);

  const containerRef = useRef<HTMLDivElement>(null);
  const listboxRef = useRef<HTMLUListElement>(null);
  const abortControllerRef = useRef<AbortController | null>(null);

  // Close on outside click
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

  // Debounced search
  useEffect(() => {
    const trimmed = query.trim();
    if (!trimmed) {
      setItems([]);
      setIsLoading(false);
      setIsOpen(false);
      return;
    }

    const timer = setTimeout(async () => {
      abortControllerRef.current?.abort();
      const controller = new AbortController();
      abortControllerRef.current = controller;

      setIsLoading(true);
      try {
        const token = await getAuthToken();
        if (!token) return;

        const response = await apiClient.searchAddresses(
          trimmed,
          {
            token,
            membershipId: selectedMembership?.id,
            signal: controller.signal,
          },
          20
        );

        setItems(response.items || []);
        setIsOpen(true);
        setActiveIndex(-1);
      } catch (err: unknown) {
        if (err instanceof DOMException && err.name === "AbortError") {
          return;
        }
        setItems([]);
      } finally {
        setIsLoading(false);
      }
    }, 250);

    return () => clearTimeout(timer);
  }, [query, selectedMembership?.id]);

  const handleSelectItem = useCallback(
    (item: AddressSearchResultItem) => {
      setQuery(item.displayText ?? "");
      setIsOpen(false);
      setActiveIndex(-1);

      onSelect({
        subdistrict: item.subdistrict ?? "",
        district: item.district ?? "",
        province: item.province ?? "",
        postalCode: item.postalCode ?? "",
        countryCode: item.countryCode ?? "TH",
        latitude: item.latitude,
        longitude: item.longitude,
      });
    },
    [onSelect]
  );

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
          handleSelectItem(items[activeIndex]);
        }
        break;
      case "Escape":
        e.preventDefault();
        setIsOpen(false);
        setActiveIndex(-1);
        break;
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

  const handleClear = () => {
    setQuery("");
    setItems([]);
    setIsOpen(false);
    setActiveIndex(-1);
  };

  return (
    <div ref={containerRef} className={`flex flex-col gap-1.5 relative w-full ${className}`}>
      {label && (
        <label htmlFor={inputId} className="erp-label">
          {label}
          {required && <span className="erp-label-required">*</span>}
        </label>
      )}

      {hint && <span className="text-xs text-erp-text-muted">{hint}</span>}

      <div className="erp-input-wrapper">
        {/* Search Icon */}
        <div className="absolute left-3 pointer-events-none text-erp-text-muted flex items-center justify-center">
          <svg
            width="16"
            height="16"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="square"
            strokeLinejoin="miter"
            aria-hidden="true"
          >
            <circle cx="11" cy="11" r="8" />
            <line x1="21" y1="21" x2="16.65" y2="16.65" />
          </svg>
        </div>

        <input
          id={inputId}
          type="text"
          role="combobox"
          aria-autocomplete="list"
          aria-expanded={isOpen}
          aria-controls={listboxId}
          aria-activedescendant={activeIndex >= 0 ? `${inputId}-option-${activeIndex}` : undefined}
          disabled={disabled}
          aria-invalid={Boolean(error)}
          value={query}
          placeholder={placeholder || t("placeholder")}
          onChange={(e) => setQuery(e.target.value)}
          onFocus={() => {
            if (items.length > 0) setIsOpen(true);
          }}
          onKeyDown={handleKeyDown}
          autoComplete="off"
          className={`erp-input pl-9 pr-16 ${error ? "erp-input-error" : ""}`}
        />

        {/* Right action indicators: Spinner or Clear */}
        <div className="absolute right-3 flex items-center gap-1.5">
          {isLoading && (
            <div className="animate-spin text-erp-navy" aria-label={t("loading")}>
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <path d="M21 12a9 9 0 1 1-6.219-8.56" />
              </svg>
            </div>
          )}

          {query && !disabled && (
            <button
              type="button"
              onClick={handleClear}
              aria-label={t("clear")}
              className="text-erp-text-muted hover:text-erp-navy p-0.5 rounded-none transition-colors"
            >
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                <line x1="18" y1="6" x2="6" y2="18" />
                <line x1="6" y1="6" x2="18" y2="18" />
              </svg>
            </button>
          )}
        </div>
      </div>

      {/* Suggestion Dropdown */}
      {isOpen && (
        <ul
          ref={listboxRef}
          id={listboxId}
          role="listbox"
          aria-label={label || t("label")}
          className="absolute z-50 top-full left-0 right-0 mt-1 max-h-60 overflow-y-auto bg-white border border-erp-border shadow-md rounded-none py-1 list-none m-0"
        >
          {items.length === 0 ? (
            <li className="px-3 py-2 text-sm text-erp-text-muted text-center italic">
              {isLoading ? t("loading") : t("noResults")}
            </li>
          ) : (
            items.map((item, idx) => {
              const isHighlighted = idx === activeIndex;
              return (
                <li
                  key={`${item.subdistrictCode}-${item.postalCode}-${idx}`}
                  id={`${inputId}-option-${idx}`}
                  role="option"
                  aria-selected={isHighlighted}
                  onClick={() => handleSelectItem(item)}
                  onMouseEnter={() => setActiveIndex(idx)}
                  className={`px-3 py-2 text-sm cursor-pointer transition-colors flex items-center justify-between gap-2 border-b border-erp-border-subtle last:border-b-0 ${
                    isHighlighted ? "bg-erp-navy text-white" : "hover:bg-erp-bg-subtle text-erp-text"
                  }`}
                >
                  <div className="flex flex-col">
                    <span className="font-medium">{item.displayText}</span>
                  </div>
                  <span
                    className={`text-xs px-1.5 py-0.5 font-mono ${
                      isHighlighted ? "bg-white/20 text-white" : "bg-erp-bg-subtle text-erp-text-muted"
                    }`}
                  >
                    {item.postalCode}
                  </span>
                </li>
              );
            })
          )}
        </ul>
      )}
    </div>
  );
}
