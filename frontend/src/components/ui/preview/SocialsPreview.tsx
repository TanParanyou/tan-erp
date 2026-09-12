"use client";

import React from "react";
import { IconGlobe } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

export interface SocialLink {
  platform: string;
  url: string;
}

export interface SocialsPreviewProps {
  links: SocialLink[];
  title?: string;
  className?: string;
}

export function SocialsPreview({
  links,
  title = "ช่องทางการติดตาม (Social Media)",
  className,
}: SocialsPreviewProps) {
  return (
    <div className={cn("border border-erp-border bg-erp-surface p-4 rounded-none text-left space-y-2.5", className)}>
      <span className="text-xs font-bold uppercase tracking-wider text-erp-text-main border-b border-erp-border pb-2 block">
        {title}
      </span>
      <div className="flex flex-wrap gap-2">
        {links.map((link) => (
          <a
            key={link.platform}
            href={link.url}
            target="_blank"
            rel="noreferrer"
            className="inline-flex items-center gap-1.5 border border-erp-border bg-erp-surface-subtle px-2.5 py-1 text-xs font-medium text-erp-text-main hover:border-erp-navy hover:text-erp-navy rounded-none transition-colors"
          >
            <IconGlobe size={13} />
            <span>{link.platform}</span>
          </a>
        ))}
      </div>
    </div>
  );
}

SocialsPreview.displayName = "SocialsPreview";
