"use client";

import React, { useState } from "react";
import { Modal } from "@/components/ui/Modal";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";

export interface RichTextLinkDialogProps {
  isOpen: boolean;
  initialUrl?: string;
  initialText?: string;
  onClose: () => void;
  onApply: (url: string, text?: string) => void;
  onRemove?: () => void;
}

export function RichTextLinkDialog({
  isOpen,
  initialUrl = "",
  initialText = "",
  onClose,
  onApply,
  onRemove,
}: RichTextLinkDialogProps) {
  const [url, setUrl] = useState(initialUrl);
  const [text, setText] = useState(initialText);

  const handleApply = (e: React.FormEvent) => {
    e.preventDefault();
    if (!url.trim()) return;
    onApply(url.trim(), text.trim() || undefined);
    onClose();
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="แทรกลิงก์ (Insert Link)" size="sm">
      <form onSubmit={handleApply} className="space-y-3.5 text-left">
        <Input
          label="ข้อความแสดง (Display Text)"
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="ข้อความลิงก์ (ไม่ระบุได้)"
        />
        <Input
          label="URL ปลายทาง"
          value={url}
          onChange={(e) => setUrl(e.target.value)}
          placeholder="https://example.com"
          required
        />
        <div className="flex items-center justify-between border-t border-erp-border pt-3 mt-4">
          {onRemove && initialUrl ? (
            <button
              type="button"
              onClick={() => {
                onRemove();
                onClose();
              }}
              className="text-xs text-red-600 hover:underline font-medium"
            >
              ลบลิงก์
            </button>
          ) : <div />}
          <div className="flex gap-2">
            <Button variant="secondary" size="sm" onClick={onClose}>
              ยกเลิก
            </Button>
            <Button variant="primary" size="sm" type="submit" disabled={!url.trim()}>
              แทรก
            </Button>
          </div>
        </div>
      </form>
    </Modal>
  );
}
