"use client";

import React, { useMemo } from "react";
import { generateHTML } from "@tiptap/html";
import { richTextExtensions } from "@/lib/rich-text/extensions";
import { getLocalizedRichText, type RichTextDocument } from "@/lib/rich-text/document";
import { sanitizeHtml } from "@/lib/utils/sanitize";
import { cn } from "@/lib/utils/cn";

export interface RichTextContentProps {
  content: unknown;
  locale?: string;
  className?: string;
  fallbackText?: string;
}

export function RichTextContent({
  content,
  locale = "th",
  className,
  fallbackText = "",
}: RichTextContentProps) {
  const html = useMemo(() => {
    if (!content) return "";
    const doc = getLocalizedRichText(content, locale) as RichTextDocument;
    if (!doc || !doc.content || doc.content.length === 0) return "";
    try {
      const rawHtml = generateHTML(doc, richTextExtensions);
      return sanitizeHtml(rawHtml);
    } catch {
      return "";
    }
  }, [content, locale]);

  if (!html) {
    return fallbackText ? <p className={cn("text-erp-text-muted text-sm", className)}>{fallbackText}</p> : null;
  }

  return (
    <div
      className={cn("prose prose-slate dark:prose-invert max-w-none text-sm leading-relaxed", className)}
      dangerouslySetInnerHTML={{ __html: html }}
    />
  );
}

RichTextContent.displayName = "RichTextContent";
