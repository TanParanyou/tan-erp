"use client";

import React from "react";
import { IconPhone, IconMapPin, IconGlobe } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface ContactDetailsPreviewProps {
  title?: string;
  phone?: string;
  email?: string;
  address?: string;
  website?: string;
  className?: string;
}

export function ContactDetailsPreview({
  title = "ข้อมูลติดต่อ",
  phone,
  email,
  address,
  website,
  className,
}: ContactDetailsPreviewProps) {
  return (
    <div className={cn("border border-erp-border bg-erp-surface p-4 rounded-none text-left space-y-2.5", className)}>
      <h4 className="text-xs font-bold uppercase tracking-wider text-erp-text-main border-b border-erp-border pb-2">
        {title}
      </h4>
      <div className="space-y-1.5 text-xs text-erp-text-muted">
        {phone && (
          <div className="flex items-center gap-2">
            <IconPhone size={14} className="text-erp-navy shrink-0" />
            <span className="text-erp-text-main font-mono">{phone}</span>
          </div>
        )}
        {email && (
          <div className="flex items-center gap-2">
            <span className="text-erp-navy font-bold text-xs shrink-0">@</span>
            <span className="text-erp-text-main">{email}</span>
          </div>
        )}
        {address && (
          <div className="flex items-start gap-2">
            <IconMapPin size={14} className="text-erp-navy shrink-0 mt-0.5" />
            <span className="text-erp-text-main leading-relaxed">{address}</span>
          </div>
        )}
        {website && (
          <div className="flex items-center gap-2">
            <IconGlobe size={14} className="text-erp-navy shrink-0" />
            <a href={website} target="_blank" rel="noreferrer" className="text-erp-navy hover:underline truncate">
              {website}
            </a>
          </div>
        )}
      </div>
    </div>
  );
}

ContactDetailsPreview.displayName = "ContactDetailsPreview";
