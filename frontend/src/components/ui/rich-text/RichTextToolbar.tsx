"use client";

import React, { useState } from "react";
import { type Editor } from "@tiptap/react";
import { cn } from "@/lib/utils/cn";
import {
  IconBold,
  IconItalic,
  IconUnderline,
  IconStrikethrough,
  IconCode,
  IconListBullet,
  IconListNumbered,
  IconQuote,
  IconAlignLeft,
  IconAlignCenter,
  IconAlignRight,
  IconLink,
  IconImage,
  IconUndo,
  IconRedo,
} from "./RichTextIcons";
import { RichTextLinkDialog } from "./RichTextLinkDialog";

export interface RichTextToolbarProps {
  editor: Editor | null;
  disabled?: boolean;
}

export function RichTextToolbar({ editor, disabled }: RichTextToolbarProps) {
  const [isLinkDialogOpen, setIsLinkDialogOpen] = useState(false);

  if (!editor) return null;

  const handleLinkApply = (url: string) => {
    editor.chain().focus().extendMarkRange("link").setLink({ href: url }).run();
  };

  const handleLinkRemove = () => {
    editor.chain().focus().unsetLink().run();
  };

  const handleImageInsert = () => {
    const url = window.prompt("ระบุ URL รูปภาพ:");
    if (url) {
      editor.chain().focus().setImage({ src: url }).run();
    }
  };

  return (
    <div className="flex flex-wrap items-center gap-0.5 border-b border-erp-border bg-erp-surface-subtle p-1 rounded-none">
      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().undo().run()}
        className={cn("p-1.5 text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main rounded-none", disabled && "opacity-40")}
        title="เลิกทำ (Undo)"
      >
        <IconUndo size={14} />
      </button>
      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().redo().run()}
        className={cn("p-1.5 text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main rounded-none", disabled && "opacity-40")}
        title="ทำซ้ำ (Redo)"
      >
        <IconRedo size={14} />
      </button>

      <div className="mx-1 h-4 w-px bg-erp-border" />

      <select
        disabled={disabled}
        onChange={(e) => {
          const val = e.target.value;
          if (val === "p") editor.chain().focus().setParagraph().run();
          else if (val === "h1") editor.chain().focus().toggleHeading({ level: 1 }).run();
          else if (val === "h2") editor.chain().focus().toggleHeading({ level: 2 }).run();
          else if (val === "h3") editor.chain().focus().toggleHeading({ level: 3 }).run();
        }}
        value={
          editor.isActive("heading", { level: 1 })
            ? "h1"
            : editor.isActive("heading", { level: 2 })
            ? "h2"
            : editor.isActive("heading", { level: 3 })
            ? "h3"
            : "p"
        }
        className="border border-erp-border bg-erp-surface px-1.5 py-1 text-xs text-erp-text-main outline-none rounded-none cursor-pointer"
      >
        <option value="p">ย่อหน้า (Paragraph)</option>
        <option value="h1">หัวเรื่อง 1 (H1)</option>
        <option value="h2">หัวเรื่อง 2 (H2)</option>
        <option value="h3">หัวเรื่อง 3 (H3)</option>
      </select>

      <div className="mx-1 h-4 w-px bg-erp-border" />

      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().toggleBold().run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive("bold") ? "bg-erp-navy text-white font-bold" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="ตัวหนา (Bold)"
      >
        <IconBold size={14} />
      </button>
      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().toggleItalic().run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive("italic") ? "bg-erp-navy text-white font-bold" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="ตัวเอียง (Italic)"
      >
        <IconItalic size={14} />
      </button>
      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().toggleUnderline().run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive("underline") ? "bg-erp-navy text-white font-bold" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="ขีดเส้นใต้ (Underline)"
      >
        <IconUnderline size={14} />
      </button>
      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().toggleStrike().run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive("strike") ? "bg-erp-navy text-white font-bold" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="ขีดฆ่า (Strike)"
      >
        <IconStrikethrough size={14} />
      </button>

      <div className="mx-1 h-4 w-px bg-erp-border" />

      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().setTextAlign("left").run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive({ textAlign: "left" }) ? "bg-erp-navy text-white" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="ชิดซ้าย"
      >
        <IconAlignLeft size={14} />
      </button>
      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().setTextAlign("center").run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive({ textAlign: "center" }) ? "bg-erp-navy text-white" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="กึ่งกลาง"
      >
        <IconAlignCenter size={14} />
      </button>
      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().setTextAlign("right").run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive({ textAlign: "right" }) ? "bg-erp-navy text-white" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="ชิดขวา"
      >
        <IconAlignRight size={14} />
      </button>

      <div className="mx-1 h-4 w-px bg-erp-border" />

      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().toggleBulletList().run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive("bulletList") ? "bg-erp-navy text-white" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="รายการหัวข้อย่อย"
      >
        <IconListBullet size={14} />
      </button>
      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().toggleOrderedList().run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive("orderedList") ? "bg-erp-navy text-white" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="รายการลำดับตัวเลข"
      >
        <IconListNumbered size={14} />
      </button>
      <button
        type="button"
        disabled={disabled}
        onClick={() => editor.chain().focus().toggleBlockquote().run()}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive("blockquote") ? "bg-erp-navy text-white" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="ข้อความอ้างอิง"
      >
        <IconQuote size={14} />
      </button>

      <div className="mx-1 h-4 w-px bg-erp-border" />

      <button
        type="button"
        disabled={disabled}
        onClick={() => setIsLinkDialogOpen(true)}
        className={cn(
          "p-1.5 transition-colors rounded-none",
          editor.isActive("link") ? "bg-erp-navy text-white" : "text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main"
        )}
        title="แทรกลิงก์"
      >
        <IconLink size={14} />
      </button>
      <button
        type="button"
        disabled={disabled}
        onClick={handleImageInsert}
        className="p-1.5 text-erp-text-muted hover:bg-erp-surface hover:text-erp-text-main rounded-none"
        title="แทรกรูปภาพ"
      >
        <IconImage size={14} />
      </button>

      <RichTextLinkDialog
        isOpen={isLinkDialogOpen}
        initialUrl={editor.getAttributes("link").href || ""}
        onClose={() => setIsLinkDialogOpen(false)}
        onApply={handleLinkApply}
        onRemove={handleLinkRemove}
      />
    </div>
  );
}
