"use client";

import React from "react";
import { Button, type ButtonProps } from "./Button";
import { IconDownload, IconSpinner } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface ListExportButtonProps extends Omit<ButtonProps, "onClick"> {
  onExport: () => Promise<void> | void;
  isExporting?: boolean;
  label?: string;
}

export function ListExportButton({
  onExport,
  isExporting = false,
  label = "ส่งออก CSV",
  className,
  ...props
}: ListExportButtonProps) {
  return (
    <Button
      variant="secondary"
      size="sm"
      onClick={onExport}
      disabled={isExporting}
      className={cn("gap-1.5 rounded-none font-medium", className)}
      {...props}
    >
      {isExporting ? (
        <IconSpinner size={14} className="animate-spin text-erp-text-muted" />
      ) : (
        <IconDownload size={14} />
      )}
      <span>{label}</span>
    </Button>
  );
}

ListExportButton.displayName = "ListExportButton";
