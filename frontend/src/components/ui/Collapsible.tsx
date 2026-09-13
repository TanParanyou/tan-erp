"use client";

import React, { useState, useId } from "react";
import { IconChevronDown } from "@/components/common/Icons";

export interface CollapsibleProps {
  /**
   * Title or header element to display in the collapsible bar
   */
  title: React.ReactNode;
  /**
   * Optional controlled open state
   */
  isOpen?: boolean;
  /**
   * Default open state when uncontrolled (default: true)
   */
  defaultOpen?: boolean;
  /**
   * Callback fired when open state changes
   */
  onToggle?: (isOpen: boolean) => void;
  /**
   * Optional action elements placed at the right of the header (e.g. search icon, badges, clear button)
   */
  actions?: React.ReactNode;
  /**
   * Children content revealed when expanded
   */
  children: React.ReactNode;
  /**
   * Additional wrapper class names
   */
  className?: string;
  /**
   * Additional header class names
   */
  headerClassName?: string;
  /**
   * Additional content container class names
   */
  contentClassName?: string;
  /**
   * Shows a top border separator (default: false)
   */
  borderedTop?: boolean;
}

export function Collapsible({
  title,
  isOpen: controlledIsOpen,
  defaultOpen = true,
  onToggle,
  actions,
  children,
  className = "",
  headerClassName = "",
  contentClassName = "",
  borderedTop = false,
}: CollapsibleProps) {
  const [uncontrolledIsOpen, setUncontrolledIsOpen] = useState(defaultOpen);
  const contentId = useId();

  const isControlled = controlledIsOpen !== undefined;
  const open = isControlled ? controlledIsOpen : uncontrolledIsOpen;

  const handleToggle = () => {
    const nextState = !open;
    if (!isControlled) {
      setUncontrolledIsOpen(nextState);
    }
    onToggle?.(nextState);
  };

  return (
    <div
      className={`shrink-0 ${
        borderedTop ? "pt-2 border-t border-erp-border/60" : ""
      } ${className}`}
    >
      <div className={`flex items-center justify-between ${headerClassName}`}>
        <button
          type="button"
          onClick={handleToggle}
          aria-expanded={open}
          aria-controls={contentId}
          className="flex items-center gap-1 text-left group cursor-pointer select-none"
        >
          {typeof title === "string" ? (
            <span className="font-bold text-erp-text-main text-[11px] group-hover:text-erp-navy transition-colors">
              {title}
            </span>
          ) : (
            title
          )}
          <IconChevronDown
            size={13}
            className={`text-erp-text-muted group-hover:text-erp-navy transition-transform duration-200 ${
              open ? "rotate-0" : "-rotate-90"
            }`}
          />
        </button>

        {actions && (
          <div className="flex items-center gap-1.5" onClick={(e) => e.stopPropagation()}>
            {actions}
          </div>
        )}
      </div>

      {open && (
        <div id={contentId} className={`pt-1.5 ${contentClassName}`}>
          {children}
        </div>
      )}
    </div>
  );
}
