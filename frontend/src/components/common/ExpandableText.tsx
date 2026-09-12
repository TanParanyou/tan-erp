"use client";

import React, { useState } from "react";
import { cn } from "@/lib/utils/cn";

export interface ExpandableTextProps {
  text: string;
  maxChars?: number;
  className?: string;
  expandLabel?: string;
  collapseLabel?: string;
}

export function ExpandableText({
  text,
  maxChars = 140,
  className,
  expandLabel = "อ่านต่อ",
  collapseLabel = "ย่อข้อความ",
}: ExpandableTextProps) {
  const [isExpanded, setIsExpanded] = useState(false);

  if (!text || text.length <= maxChars) {
    return <span className={className}>{text}</span>;
  }

  const displayText = isExpanded ? text : `${text.slice(0, maxChars)}...`;

  return (
    <span className={className}>
      <span>{displayText}</span>{" "}
      <button
        type="button"
        onClick={() => setIsExpanded(!isExpanded)}
        className="text-xs font-semibold text-erp-navy hover:underline ml-1 inline-block"
      >
        {isExpanded ? collapseLabel : expandLabel}
      </button>
    </span>
  );
}

ExpandableText.displayName = "ExpandableText";
