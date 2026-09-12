"use client";

import React, { useState, useRef, useEffect } from "react";
import { Checkbox } from "./Checkbox";
import { IconSearch, IconChevronDown } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface FilterOption {
  value: string;
  label: string;
  count?: number;
}

export interface ListMultiSelectFilterProps {
  label: string;
  options: FilterOption[];
  selectedValues: string[];
  onChange: (values: string[]) => void;
  placeholder?: string;
  className?: string;
}

export function ListMultiSelectFilter({
  label,
  options,
  selectedValues,
  onChange,
  placeholder = "ค้นหา...",
  className,
}: ListMultiSelectFilterProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const filteredOptions = options.filter((opt) =>
    opt.label.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const toggleOption = (val: string) => {
    if (selectedValues.includes(val)) {
      onChange(selectedValues.filter((v) => v !== val));
    } else {
      onChange([...selectedValues, val]);
    }
  };

  const selectAll = () => {
    onChange(options.map((o) => o.value));
  };

  const clearAll = () => {
    onChange([]);
  };

  return (
    <div ref={dropdownRef} className={cn("relative text-left", className)}>
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        className="flex h-9 items-center justify-between gap-2 border border-erp-border bg-erp-surface px-3 py-1.5 text-xs font-medium text-erp-text-main hover:bg-erp-surface-subtle focus:border-erp-navy rounded-none"
      >
        <span>
          {label}
          {selectedValues.length > 0 && (
            <span className="ml-1.5 bg-erp-navy px-1.5 py-0.2 text-[10px] text-white rounded-none font-bold">
              {selectedValues.length}
            </span>
          )}
        </span>
        <IconChevronDown size={14} className="text-erp-text-muted" />
      </button>

      {isOpen && (
        <div className="absolute left-0 top-full z-50 mt-1 w-64 border border-erp-border bg-erp-surface p-2 shadow-lg rounded-none animate-in fade-in zoom-in-95">
          <div className="relative mb-2">
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder={placeholder}
              className="w-full border border-erp-border bg-erp-surface pl-7 pr-2 py-1 text-xs text-erp-text-main outline-none focus:border-erp-navy rounded-none"
            />
            <IconSearch size={12} className="absolute left-2 top-2 text-erp-text-muted" />
          </div>

          <div className="mb-2 flex items-center justify-between border-b border-erp-border pb-1.5 px-1 text-[11px]">
            <button
              type="button"
              onClick={selectAll}
              className="text-erp-navy hover:underline font-medium"
            >
              เลือกทั้งหมด
            </button>
            <button
              type="button"
              onClick={clearAll}
              className="text-erp-text-muted hover:text-erp-text-main"
            >
              ล้างค่า
            </button>
          </div>

          <div className="max-h-48 overflow-y-auto space-y-1">
            {filteredOptions.length === 0 ? (
              <p className="p-2 text-center text-xs text-erp-text-muted">ไม่พบตัวเลือก</p>
            ) : (
              filteredOptions.map((opt) => (
                <label
                  key={opt.value}
                  className="flex cursor-pointer items-center justify-between px-2 py-1 text-xs hover:bg-erp-surface-subtle rounded-none"
                >
                  <div className="flex items-center gap-2">
                    <Checkbox
                      checked={selectedValues.includes(opt.value)}
                      onChange={() => toggleOption(opt.value)}
                    />
                    <span className="text-erp-text-main">{opt.label}</span>
                  </div>
                  {opt.count !== undefined && (
                    <span className="text-[10px] text-erp-text-muted font-mono">
                      ({opt.count})
                    </span>
                  )}
                </label>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}

ListMultiSelectFilter.displayName = "ListMultiSelectFilter";
