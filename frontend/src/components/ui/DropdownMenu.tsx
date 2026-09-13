"use client";

import React, { useState, useRef, useEffect, useCallback } from "react";
import { cn } from "@/lib/utils/cn";
import { IconChevronDown } from "@/components/common/Icons";

export interface DropdownMenuItem {
  key: string;
  label: string;
  icon?: React.ReactNode;
  onClick: () => void;
  variant?: "default" | "danger";
  disabled?: boolean;
  dividerAbove?: boolean;
}

export interface DropdownMenuProps {
  trigger?: React.ReactNode;
  triggerLabel?: string;
  items: DropdownMenuItem[];
  align?: "left" | "right";
  className?: string;
}

export function DropdownMenu({
  trigger,
  triggerLabel = "Actions",
  items,
  align = "right",
  className,
}: DropdownMenuProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [computedAlign, setComputedAlign] = useState<"left" | "right">(align);
  const containerRef = useRef<HTMLDivElement>(null);
  const menuRef = useRef<HTMLDivElement>(null);

  const toggle = useCallback(() => {
    setIsOpen((prev) => !prev);
  }, []);

  const close = useCallback(() => {
    setIsOpen(false);
  }, []);

  // Adjust alignment dynamically when menu opens to avoid overflowing viewport
  useEffect(() => {
    if (!isOpen) {
      setComputedAlign(align);
      return;
    }

    const updateAlignment = () => {
      if (!containerRef.current) return;
      const rect = containerRef.current.getBoundingClientRect();
      const viewportWidth = window.innerWidth;
      const estimatedMenuWidth = menuRef.current?.offsetWidth || 220;

      // If align is right: menu extends to the left from container's right edge
      // container.right - estimatedMenuWidth < 0 -> overflows left edge!
      // If align is left: menu extends to the right from container's left edge
      // container.left + estimatedMenuWidth > viewportWidth -> overflows right edge!
      if (align === "left") {
        if (rect.left + estimatedMenuWidth > viewportWidth - 16 && rect.right - estimatedMenuWidth >= 16) {
          setComputedAlign("right");
        } else {
          setComputedAlign("left");
        }
      } else {
        // default right align
        if (rect.right + (estimatedMenuWidth - rect.width) > viewportWidth - 16 && rect.right > viewportWidth - 16) {
          setComputedAlign("right");
        } else if (rect.right - estimatedMenuWidth < 16 && rect.left + estimatedMenuWidth <= viewportWidth - 16) {
          setComputedAlign("left");
        } else {
          setComputedAlign("right");
        }
      }
    };

    updateAlignment();
    window.addEventListener("resize", updateAlignment);
    return () => window.removeEventListener("resize", updateAlignment);
  }, [isOpen, align]);

  // Click outside to dismiss
  useEffect(() => {
    if (!isOpen) return;

    const handleClickOutside = (e: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
        close();
      }
    };

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        close();
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [isOpen, close]);

  const visibleItems = items.filter(Boolean);

  if (visibleItems.length === 0) return null;

  return (
    <div ref={containerRef} className={cn("relative inline-block text-left", className)}>
      {trigger ? (
        <div onClick={toggle} role="button" tabIndex={0} onKeyDown={(e) => e.key === "Enter" && toggle()}>
          {trigger}
        </div>
      ) : (
        <button
          type="button"
          onClick={toggle}
          aria-expanded={isOpen}
          aria-haspopup="menu"
          className="erp-btn erp-btn-outline erp-btn-md inline-flex items-center gap-1.5 font-semibold"
          style={{ borderRadius: "0px" }}
        >
          <span>{triggerLabel}</span>
          <IconChevronDown
            size={14}
            className={cn("transition-transform duration-200", isOpen && "rotate-180")}
          />
        </button>
      )}

      {isOpen && (
        <div
          ref={menuRef}
          role="menu"
          aria-orientation="vertical"
          className={cn(
            "absolute z-50 mt-1 min-w-[200px] max-w-[calc(100vw-2rem)] whitespace-nowrap border border-erp-border bg-erp-surface py-1 shadow-lg focus:outline-none",
            computedAlign === "right" ? "right-0" : "left-0"
          )}
          style={{ borderRadius: "0px" }}
        >
          {visibleItems.map((item) => (
            <React.Fragment key={item.key}>
              {item.dividerAbove && (
                <div className="my-1 border-t border-erp-border-subtle" role="separator" />
              )}
              <button
                type="button"
                role="menuitem"
                disabled={item.disabled}
                onClick={() => {
                  close();
                  item.onClick();
                }}
                className={cn(
                  "w-full flex items-center gap-2 px-3.5 py-2 text-left text-xs font-medium transition-colors outline-none",
                  item.variant === "danger"
                    ? "text-erp-danger hover:bg-erp-danger/10 focus:bg-erp-danger/10"
                    : "text-erp-text-main hover:bg-erp-navy/5 focus:bg-erp-navy/5",
                  item.disabled && "pointer-events-none opacity-40"
                )}
              >
                {item.icon && <span className="shrink-0">{item.icon}</span>}
                <span className="truncate">{item.label}</span>
              </button>
            </React.Fragment>
          ))}
        </div>
      )}
    </div>
  );
}
