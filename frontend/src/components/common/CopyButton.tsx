"use client";

import React from "react";
import { useClipboard } from "@/hooks/useClipboard";
import { IconCheck, IconCopy } from "@/components/common/Icons";

export interface CopyButtonProps {
  text: string;
  label?: string;
  copiedLabel?: string;
  className?: string;
  variant?: "button" | "icon" | "inline";
  size?: "sm" | "md";
  onCopySuccess?: (text: string) => void;
}

export function CopyButton({
  text,
  label = "Copy",
  copiedLabel = "Copied",
  className = "",
  variant = "inline",
  size = "sm",
  onCopySuccess,
}: CopyButtonProps) {
  const { copy, copied } = useClipboard({ onCopySuccess, timeout: 2000 });

  const handleCopy = (event: React.MouseEvent) => {
    event.preventDefault();
    event.stopPropagation();
    void copy(text);
  };

  if (variant === "icon") {
    return (
      <button
        type="button"
        onClick={handleCopy}
        className={`inline-flex items-center justify-center border border-erp-border bg-erp-surface text-erp-text-main hover:bg-erp-surface-muted focus-visible:outline-2 focus-visible:outline-erp-navy rounded-none ${
          size === "sm" ? "h-7 w-7 p-1" : "h-9 w-9 p-1.5"
        } ${className}`}
        title={copied ? copiedLabel : label}
        aria-label={copied ? copiedLabel : label}
      >
        {copied ? (
          <IconCheck size={14} className="text-erp-success" />
        ) : (
          <IconCopy size={14} className="text-erp-text-muted" />
        )}
      </button>
    );
  }

  if (variant === "button") {
    return (
      <button
        type="button"
        onClick={handleCopy}
        className={`inline-flex min-h-[36px] items-center justify-center gap-1.5 border border-erp-border bg-erp-surface px-3 py-1.5 text-xs font-semibold text-erp-text-main hover:bg-erp-surface-muted focus-visible:outline-2 focus-visible:outline-erp-navy rounded-none ${className}`}
      >
        {copied ? (
          <>
            <IconCheck size={14} className="text-erp-success" />
            <span className="text-erp-success">{copiedLabel}</span>
          </>
        ) : (
          <>
            <IconCopy size={14} className="text-erp-text-muted" />
            <span>{label}</span>
          </>
        )}
      </button>
    );
  }

  return (
    <button
      type="button"
      onClick={handleCopy}
      className={`inline-flex items-center gap-1 border border-erp-border bg-erp-surface px-1.5 py-0.5 text-xs font-medium text-erp-text-main hover:bg-erp-surface-muted focus-visible:outline-2 focus-visible:outline-erp-navy rounded-none ${className}`}
      title={copied ? copiedLabel : label}
      aria-label={copied ? copiedLabel : label}
    >
      {copied ? (
        <>
          <IconCheck size={12} className="text-erp-success" />
          <span className="text-erp-success">{copiedLabel}</span>
        </>
      ) : (
        <>
          <IconCopy size={12} className="text-erp-text-muted" />
          <span>{label}</span>
        </>
      )}
    </button>
  );
}

export default CopyButton;
