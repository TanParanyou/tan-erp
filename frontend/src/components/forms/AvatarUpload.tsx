"use client";

import React, { useEffect, useId, useRef, useState } from "react";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { IconUpload, IconTrash, IconUser } from "@/components/common/Icons";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";

export interface AvatarUploadProps {
  file: File | null;
  existingFileId?: string | null;
  existingUrl?: string | null;
  onFileChange: (file: File | null) => void;
  displayName?: string;
  disabled?: boolean;
  uploadButtonText?: string;
  deleteButtonText?: string;
  hintText?: string;
}

export function AvatarUpload({
  file,
  existingFileId,
  existingUrl,
  onFileChange,
  displayName,
  disabled = false,
  uploadButtonText,
  deleteButtonText,
  hintText,
}: AvatarUploadProps) {
  const tUserAdmin = useTranslations("userAdmin");
  const tCommon = useTranslations("common");
  const inputId = useId();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);

  useEffect(() => {
    if (!file) {
      setPreviewUrl(null);
      return;
    }
    const url = URL.createObjectURL(file);
    setPreviewUrl(url);
    return () => URL.revokeObjectURL(url);
  }, [file]);

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const selected = event.target.files?.[0];
    if (selected) {
      onFileChange(selected);
    }
  };

  const handleRemove = () => {
    onFileChange(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const initial = displayName?.trim().charAt(0).toUpperCase();
  const hasImage = Boolean(previewUrl || existingFileId || existingUrl);

  const resolvedUploadText = uploadButtonText ?? tUserAdmin("fields.uploadAvatar");
  const resolvedDeleteText = deleteButtonText ?? tCommon("actions.delete");
  const resolvedHintText = hintText ?? tUserAdmin("fields.avatarHint");

  return (
    <div className="flex flex-col sm:flex-row items-start sm:items-center gap-4">
      <div className="relative w-20 h-20 border border-erp-border bg-erp-surface-muted flex items-center justify-center shrink-0 overflow-hidden">
        {previewUrl ? (
          // eslint-disable-next-line @next/next/no-img-element
          <img src={previewUrl} alt={displayName ?? "Avatar Preview"} className="w-full h-full object-cover" />
        ) : existingFileId ? (
          <AuthenticatedFileImage fileId={existingFileId} alt={displayName ?? "Avatar"} className="w-full h-full object-cover" />
        ) : existingUrl ? (
          // eslint-disable-next-line @next/next/no-img-element
          <img src={existingUrl} alt={displayName ?? "Avatar"} className="w-full h-full object-cover" />
        ) : initial ? (
          <span className="text-2xl font-bold text-erp-navy">{initial}</span>
        ) : (
          <IconUser size={32} className="text-erp-text-muted" />
        )}
      </div>

      <div className="flex flex-col gap-2">
        <div className="flex flex-wrap items-center gap-2">
          <input
            id={inputId}
            ref={fileInputRef}
            type="file"
            accept="image/png,image/jpeg,image/webp"
            className="hidden"
            disabled={disabled}
            onChange={handleFileSelect}
          />
          <Button
            type="button"
            variant="outline"
            size="sm"
            disabled={disabled}
            icon={<IconUpload size={14} />}
            onClick={() => fileInputRef.current?.click()}
          >
            {resolvedUploadText}
          </Button>

          {hasImage && !disabled && (
            <Button
              type="button"
              variant="outline"
              size="sm"
              icon={<IconTrash size={14} />}
              onClick={handleRemove}
            >
              {resolvedDeleteText}
            </Button>
          )}
        </div>
        <p className="text-xs text-erp-text-muted">{resolvedHintText}</p>
      </div>
    </div>
  );
}

