"use client";

import React, { useState } from "react";
import { IconEye, IconClose } from "@/components/common/Icons";
import { MobilePreviewButton } from "./MobilePreviewButton";

export interface MobilePreviewDrawerProps {
  title?: string;
  children: React.ReactNode;
}

export function MobilePreviewDrawer({
  title = "พรีวิวการแสดงผลบนอุปกรณ์พกพา",
  children,
}: MobilePreviewDrawerProps) {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <MobilePreviewButton onClick={() => setIsOpen(true)} />
      {isOpen && (
        <div className="fixed inset-0 z-50 flex flex-col bg-erp-surface p-4 overflow-y-auto space-y-4 rounded-none">
          <div className="flex items-center justify-between border-b border-erp-border pb-3 sticky top-0 bg-erp-surface z-10">
            <div className="flex items-center gap-2">
              <IconEye size={18} className="text-erp-navy" />
              <h3 className="text-sm font-bold text-erp-text-main">{title}</h3>
            </div>
            <button
              type="button"
              onClick={() => setIsOpen(false)}
              className="border border-erp-border bg-erp-surface p-1.5 text-erp-text-muted hover:text-erp-text-main rounded-none"
            >
              <IconClose size={18} />
            </button>
          </div>
          <div className="mx-auto w-full max-w-sm border border-erp-border bg-white p-3 rounded-none shadow-md">
            {children}
          </div>
        </div>
      )}
    </>
  );
}

MobilePreviewDrawer.displayName = "MobilePreviewDrawer";
