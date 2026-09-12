"use client";

import React from "react";
import { QRCodeDisplay } from "@/components/ui/QRCodeDisplay";
import { CopyButton } from "./CopyButton";
import { cn } from "@/lib/utils/cn";

export interface QRCodePassProps {
  code: string;
  title: string;
  subtitle?: string;
  dateText?: string;
  className?: string;
}

export function QRCodePass({
  code,
  title,
  subtitle,
  dateText,
  className,
}: QRCodePassProps) {
  return (
    <div
      className={cn(
        "flex flex-col items-center border border-erp-border bg-erp-surface p-5 text-center rounded-none shadow-xs max-w-sm",
        className
      )}
    >
      <div className="mb-3">
        <h4 className="text-sm font-bold text-erp-text-main uppercase tracking-wider">
          {title}
        </h4>
        {subtitle && <p className="text-xs text-erp-text-muted mt-0.5">{subtitle}</p>}
      </div>

      <div className="my-2 bg-white p-2 border border-erp-border">
        <QRCodeDisplay value={code} size={150} />
      </div>

      <div className="mt-3 flex items-center gap-2">
        <span className="font-mono text-xs font-bold text-erp-text-main">{code}</span>
        <CopyButton text={code} label="คัดลอก" />
      </div>

      {dateText && (
        <span className="mt-3 text-[11px] text-erp-text-muted font-mono">
          {dateText}
        </span>
      )}
    </div>
  );
}

QRCodePass.displayName = "QRCodePass";
