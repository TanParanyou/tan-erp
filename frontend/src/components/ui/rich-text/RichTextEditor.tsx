"use client";

import React, { useEffect } from "react";
import { useEditor, EditorContent } from "@tiptap/react";
import { richTextExtensions } from "@/lib/rich-text/extensions";
import type { RichTextDocument } from "@/lib/rich-text/document";
import { RichTextToolbar } from "./RichTextToolbar";
import { cn } from "@/lib/utils/cn";

export interface RichTextEditorProps {
  value?: RichTextDocument | null;
  onChange?: (doc: RichTextDocument) => void;
  disabled?: boolean;
  minHeight?: string;
  className?: string;
  placeholder?: string;
}

export function RichTextEditor({
  value,
  onChange,
  disabled = false,
  minHeight = "200px",
  className,
}: RichTextEditorProps) {
  const editor = useEditor({
    extensions: richTextExtensions,
    content: value || { type: "doc", content: [{ type: "paragraph" }] },
    editable: !disabled,
    immediatelyRender: false,
    onUpdate: ({ editor: ed }) => {
      onChange?.(ed.getJSON() as RichTextDocument);
    },
  });

  useEffect(() => {
    if (!editor || !value) return;
    const currentJson = JSON.stringify(editor.getJSON());
    const incomingJson = JSON.stringify(value);
    if (currentJson !== incomingJson) {
      editor.commands.setContent(value);
    }
  }, [value, editor]);

  return (
    <div
      className={cn(
        "border border-erp-border bg-erp-surface rounded-none transition-colors focus-within:border-erp-navy",
        disabled && "opacity-60 bg-erp-surface-subtle cursor-not-allowed",
        className
      )}
    >
      <RichTextToolbar editor={editor} disabled={disabled} />
      <div style={{ minHeight }} className="p-3 text-sm text-erp-text-main font-sans leading-relaxed focus:outline-none">
        <EditorContent editor={editor} />
      </div>
    </div>
  );
}

RichTextEditor.displayName = "RichTextEditor";
