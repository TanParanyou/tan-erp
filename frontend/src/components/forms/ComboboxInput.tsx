"use client";

import React, { useState, useRef, useEffect, useId } from "react";
import { useTranslations } from "next-intl";
import { Input, type InputProps } from "@/components/ui/Input";
import { IconChevronDown, IconClose } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface ComboboxInputOption {
  value: string;
  label: string;
  description?: string;
}

export interface ComboboxInputProps
  extends Omit<InputProps, "value" | "onChange" | "list"> {
  value: string;
  onChange: (value: string) => void;
  options: ComboboxInputOption[];
  onSelectOption?: (option: ComboboxInputOption) => void;
  inputClassName?: string;
  popoverClassName?: string;
  /**
   * If true (default), always shows the full options list when opened/focused,
   * regardless of current input value (so the user can always pick any option/template).
   */
  alwaysShowAllOptions?: boolean;
}

/**
 * ComboboxInput - Atelier Architectural Navy Sharp
 *
 * Reusable single input combining freeform typing with a dense suggestions/preset dropdown.
 * - Free text input with autocomplete suggestions listbox.
 * - Integrated Clear button (x) to quickly clear input.
 * - Always shows all preset options on focus/toggle so users can always switch templates anytime.
 * - Supports keyboard navigation (ArrowDown, ArrowUp, Enter, Escape).
 * - Clicking or choosing an option updates value and optionally triggers onSelectOption.
 * - Adheres to Atelier Architectural Navy Sharp: 0px radius, Solid Navy accents, pure SVG stroke icons.
 */
export function ComboboxInput({
  value,
  onChange,
  options,
  onSelectOption,
  className,
  inputClassName,
  popoverClassName,
  alwaysShowAllOptions = true,
  disabled,
  placeholder,
  ...props
}: ComboboxInputProps) {
  const t = useTranslations("common");
  const [isOpen, setIsOpen] = useState(false);
  const [highlightedIndex, setHighlightedIndex] = useState(-1);
  const containerRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const listRef = useRef<HTMLUListElement>(null);
  const generatedId = useId();
  const listboxId = `combobox-input-${generatedId}`;

  // When alwaysShowAllOptions is true, always display all options.
  // When false, filter strictly by input query.
  const displayOptions = alwaysShowAllOptions
    ? options
    : options.filter((item) => {
        if (!value || !value.trim()) return true;
        const q = value.trim().toLowerCase();
        return (
          item.label.toLowerCase().includes(q) ||
          item.value.toLowerCase().includes(q) ||
          (item.description && item.description.toLowerCase().includes(q))
        );
      });

  // Handle outside clicks to close dropdown
  useEffect(() => {
    const handleOutsideClick = (event: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener("mousedown", handleOutsideClick);
    return () => {
      document.removeEventListener("mousedown", handleOutsideClick);
    };
  }, []);

  const handleClear = (e: React.MouseEvent) => {
    e.stopPropagation();
    onChange("");
    setIsOpen(false);
    setHighlightedIndex(-1);
    inputRef.current?.focus();
  };

  const handleSelect = (option: ComboboxInputOption) => {
    onChange(option.label || option.value);
    onSelectOption?.(option);
    setIsOpen(false);
    setHighlightedIndex(-1);
    inputRef.current?.focus();
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (disabled) return;

    if (e.key === "ArrowDown") {
      e.preventDefault();
      if (!isOpen) {
        setIsOpen(true);
        setHighlightedIndex(0);
      } else {
        setHighlightedIndex((prev) =>
          prev < displayOptions.length - 1 ? prev + 1 : 0
        );
      }
    } else if (e.key === "ArrowUp") {
      e.preventDefault();
      if (!isOpen) {
        setIsOpen(true);
        setHighlightedIndex(displayOptions.length - 1);
      } else {
        setHighlightedIndex((prev) =>
          prev > 0 ? prev - 1 : displayOptions.length - 1
        );
      }
    } else if (e.key === "Enter") {
      if (isOpen && highlightedIndex >= 0 && highlightedIndex < displayOptions.length) {
        e.preventDefault();
        handleSelect(displayOptions[highlightedIndex]);
      }
    } else if (e.key === "Escape") {
      setIsOpen(false);
      setHighlightedIndex(-1);
    }
  };

  const hasClearButton = !disabled && Boolean(value && value.trim().length > 0);
  const hasDropdownButton = !disabled && options.length > 0;

  return (
    <div ref={containerRef} className={cn("relative w-full", className)}>
      <div className="relative flex items-center">
        <Input
          ref={inputRef}
          value={value}
          onChange={(e) => {
            onChange(e.target.value);
            if (!isOpen) setIsOpen(true);
          }}
          onFocus={() => {
            if (!disabled && options.length > 0) {
              setIsOpen(true);
            }
          }}
          onKeyDown={handleKeyDown}
          disabled={disabled}
          placeholder={placeholder}
          className={cn(
            hasClearButton && hasDropdownButton
              ? "pr-14"
              : hasClearButton || hasDropdownButton
              ? "pr-8"
              : "",
            inputClassName
          )}
          wrapperClassName="!mb-0 w-full"
          role="combobox"
          aria-expanded={isOpen}
          aria-autocomplete="list"
          aria-controls={isOpen ? listboxId : undefined}
          {...props}
        />

        {/* Action icons container */}
        <div className="absolute right-1 flex items-center gap-0.5">
          {/* Clear button (x) */}
          {hasClearButton && (
            <button
              type="button"
              tabIndex={-1}
              onClick={handleClear}
              className="h-7 w-7 flex items-center justify-center text-erp-text-muted hover:text-erp-danger transition-colors cursor-pointer"
              title={t("actions.clear")}
              aria-label={t("actions.clear")}
            >
              <IconClose size={13} strokeWidth={2.5} />
            </button>
          )}

          {/* Options dropdown toggle button */}
          {hasDropdownButton && (
            <button
              type="button"
              tabIndex={-1}
              onClick={() => {
                setIsOpen((prev) => !prev);
                inputRef.current?.focus();
              }}
              className="h-7 w-7 flex items-center justify-center text-erp-text-muted hover:text-erp-navy transition-colors cursor-pointer"
              aria-label="Toggle options"
            >
              <IconChevronDown
                size={14}
                strokeWidth={2}
                className={cn("transition-transform duration-150", isOpen ? "rotate-180" : "")}
              />
            </button>
          )}
        </div>
      </div>

      {/* Floating Dense Options Listbox */}
      {isOpen && !disabled && displayOptions.length > 0 && (
        <ul
          id={listboxId}
          ref={listRef}
          role="listbox"
          className={cn(
            "absolute left-0 right-0 top-full mt-1 z-50 max-h-60 overflow-y-auto bg-erp-surface border border-erp-border shadow-md divide-y divide-erp-border-subtle",
            popoverClassName
          )}
        >
          {displayOptions.map((item, index) => {
            const isHighlighted = index === highlightedIndex;
            return (
              <li
                key={item.value}
                role="option"
                aria-selected={isHighlighted}
                onMouseEnter={() => setHighlightedIndex(index)}
                onClick={() => handleSelect(item)}
                className={cn(
                  "px-3 py-2 text-xs cursor-pointer flex items-center justify-between transition-colors",
                  isHighlighted
                    ? "bg-erp-surface-subtle text-erp-navy font-semibold"
                    : "text-erp-text-body hover:bg-erp-surface-subtle/50"
                )}
              >
                <span className="truncate">{item.label}</span>
                {item.description && (
                  <span className="text-[10px] text-erp-text-muted ml-2 shrink-0 font-normal">
                    {item.description}
                  </span>
                )}
              </li>
            );
          })}
        </ul>
      )}
    </div>
  );
}
