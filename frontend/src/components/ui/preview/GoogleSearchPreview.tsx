"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface GoogleSearchPreviewProps {
  title: string;
  url: string;
  description: string;
  className?: string;
}

export function GoogleSearchPreview({
  title,
  url,
  description,
  className,
}: GoogleSearchPreviewProps) {
  return (
    <div className={cn("border border-erp-border bg-erp-surface p-4 rounded-none text-left space-y-1 max-w-xl", className)}>
      <span className="text-[10px] font-mono text-erp-text-muted block truncate">{url}</span>
      <h3 className="text-base font-semibold text-blue-800 dark:text-blue-400 hover:underline cursor-pointer truncate">
        {title || "Untitled Document - Project ERP"}
      </h3>
      <p className="text-xs text-erp-text-muted leading-relaxed line-clamp-2">
        {description || "คำอธิบายเนื้อหาและรายละเอียดของเอกสารหรือหน้านี้บนระบบ Project ERP"}
      </p>
    </div>
  );
}

GoogleSearchPreview.displayName = "GoogleSearchPreview";
