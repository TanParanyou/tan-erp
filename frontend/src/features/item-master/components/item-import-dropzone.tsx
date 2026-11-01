"use client";

import { useState, type DragEvent, type RefObject } from "react";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { IconUpload } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";

interface ItemImportDropzoneProps {
  inputRef: RefObject<HTMLInputElement | null>;
  disabled: boolean;
  isLoading: boolean;
  onFile: (file: File | undefined) => void;
  /** Called when a dropped file is not a .csv file. */
  onReject: () => void;
}

/** Drag-and-drop or browse area for the CSV file; the hidden input keeps the native picker and test hooks. */
export function ItemImportDropzone({ inputRef, disabled, isLoading, onFile, onReject }: ItemImportDropzoneProps) {
  const t = useTranslations("itemMaster.import");
  const [dragging, setDragging] = useState(false);

  function handleDrop(event: DragEvent<HTMLDivElement>): void {
    event.preventDefault();
    setDragging(false);
    if (disabled) return;
    const file = event.dataTransfer.files[0];
    if (!file) return;
    if (!file.name.toLowerCase().endsWith(".csv")) {
      onReject();
      return;
    }
    onFile(file);
  }

  return (
    <div
      onDragOver={(event) => { event.preventDefault(); if (!disabled) setDragging(true); }}
      onDragLeave={() => setDragging(false)}
      onDrop={handleDrop}
      className={cn(
        "flex flex-col items-center gap-2 border-2 border-dashed px-4 py-8 text-center transition-colors",
        dragging ? "border-erp-navy bg-erp-navy-light" : "border-erp-border bg-erp-surface-subtle",
      )}
    >
      <input
        ref={inputRef}
        type="file"
        accept=".csv,text/csv"
        className="sr-only"
        aria-label={t("chooseFile")}
        data-testid="item-import-file"
        disabled={disabled}
        onChange={(event) => onFile(event.currentTarget.files?.[0])}
      />
      <IconUpload size={26} className="text-erp-navy" />
      <p className="text-sm font-semibold text-erp-text-main">{dragging ? t("dropzoneActive") : t("dropzoneTitle")}</p>
      <Button type="button" variant="secondary" icon={<IconUpload size={15} />} disabled={disabled} isLoading={isLoading} onClick={() => inputRef.current?.click()} className="min-h-11">
        {t("chooseFile")}
      </Button>
      <p className="text-xs text-erp-text-muted">{t("dropzoneHint")}</p>
    </div>
  );
}
