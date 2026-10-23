"use client";

import React, { useRef } from "react";
import { useTranslations } from "next-intl";
import type {
  FieldSurveyPhoto,
  FieldSurveyPhotoTag,
} from "../../schemas/field-survey-schema";
import { IconCamera, IconTrash } from "@/components/common/Icons";

export interface FieldSurveyItemPhotoManagerProps {
  photos: readonly FieldSurveyPhoto[];
  onChange: (photos: FieldSurveyPhoto[]) => void;
  disabled?: boolean;
}

export function FieldSurveyItemPhotoManager({
  photos = [],
  onChange,
  disabled = false,
}: FieldSurveyItemPhotoManagerProps) {
  const t = useTranslations("fieldSurvey.photos");
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleUploadClick = () => {
    fileInputRef.current?.click();
  };

  const handleFilesSelected = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (!files || files.length === 0) return;

    const newPhotos: FieldSurveyPhoto[] = Array.from(files).map((file) => ({
      id: `photo-${Date.now()}-${Math.random().toString(36).substring(2, 7)}`,
      url: URL.createObjectURL(file),
      fileName: file.name,
      tag: "site_location" as const,
      caption: "",
    }));

    onChange([...photos, ...newPhotos]);

    // Reset input so same file can be re-selected if needed
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleTagChange = (photoId: string, newTag: FieldSurveyPhotoTag) => {
    const nextPhotos = photos.map((p) => (p.id === photoId ? { ...p, tag: newTag } : p));
    onChange(nextPhotos);
  };

  const handleCaptionChange = (photoId: string, newCaption: string) => {
    const nextPhotos = photos.map((p) =>
      p.id === photoId ? { ...p, caption: newCaption } : p
    );
    onChange(nextPhotos);
  };

  const handleRemovePhoto = (photoId: string) => {
    const nextPhotos = photos.filter((p) => p.id !== photoId);
    onChange(nextPhotos);
  };

  return (
    <div className="border border-erp-border bg-white p-4">
      <div className="mb-3 flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-sm font-semibold text-erp-navy">{t("title")}</h3>
          <p className="text-xs text-erp-text-muted">{t("subtitle")}</p>
        </div>
        <div>
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            capture="environment"
            multiple
            disabled={disabled}
            onChange={handleFilesSelected}
            className="hidden"
          />
          <button
            type="button"
            disabled={disabled}
            onClick={handleUploadClick}
            className="inline-flex min-h-[44px] items-center gap-2 border border-erp-navy bg-erp-navy px-4 py-2 text-xs font-semibold text-white transition-colors hover:bg-erp-navy-hover active:bg-erp-navy-active disabled:cursor-not-allowed disabled:opacity-50"
          >
            <IconCamera size={16} />
            {t("uploadBtn")}
          </button>
        </div>
      </div>

      {photos.length === 0 ? (
        <div className="border border-dashed border-erp-border p-6 text-center">
          <p className="text-xs text-erp-text-muted">{t("empty")}</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          {photos.map((photo) => (
            <div
              key={photo.id}
              className="flex flex-col border border-erp-border bg-erp-surface-subtle p-3"
            >
              {/* Photo Preview & Delete */}
              <div className="relative mb-2 aspect-video w-full overflow-hidden border border-erp-border bg-black/5">
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img
                  src={photo.url}
                  alt={photo.caption || photo.fileName}
                  className="h-full w-full object-cover"
                />
                <button
                  type="button"
                  disabled={disabled}
                  onClick={() => handleRemovePhoto(photo.id)}
                  aria-label={t("deletePhoto")}
                  className="absolute right-1 top-1 inline-flex min-h-[36px] min-w-[36px] items-center justify-center border border-erp-border bg-white/90 text-erp-danger transition-colors hover:bg-white hover:text-red-700"
                >
                  <IconTrash size={16} />
                </button>
              </div>

              {/* Tag Selector Buttons */}
              <div className="mb-2">
                <div className="grid grid-cols-3 gap-1">
                  <button
                    type="button"
                    disabled={disabled}
                    onClick={() => handleTagChange(photo.id, "site_location")}
                    className={`min-h-[32px] px-1 text-[10px] font-semibold transition-colors ${
                      photo.tag === "site_location"
                        ? "border border-erp-navy bg-erp-navy text-white"
                        : "border border-erp-border bg-white text-erp-text-main hover:bg-erp-surface"
                    }`}
                  >
                    {t("tagSiteLocation")}
                  </button>
                  <button
                    type="button"
                    disabled={disabled}
                    onClick={() => handleTagChange(photo.id, "sketch_laser")}
                    className={`min-h-[32px] px-1 text-[10px] font-semibold transition-colors ${
                      photo.tag === "sketch_laser"
                        ? "border border-erp-navy bg-erp-navy text-white"
                        : "border border-erp-border bg-white text-erp-text-main hover:bg-erp-surface"
                    }`}
                  >
                    {t("tagSketchLaser")}
                  </button>
                  <button
                    type="button"
                    disabled={disabled}
                    onClick={() => handleTagChange(photo.id, "material_swatch")}
                    className={`min-h-[32px] px-1 text-[10px] font-semibold transition-colors ${
                      photo.tag === "material_swatch"
                        ? "border border-erp-navy bg-erp-navy text-white"
                        : "border border-erp-border bg-white text-erp-text-main hover:bg-erp-surface"
                    }`}
                  >
                    {t("tagMaterialSwatch")}
                  </button>
                </div>
              </div>

              {/* Caption Input */}
              <input
                type="text"
                disabled={disabled}
                value={photo.caption ?? ""}
                onChange={(e) => handleCaptionChange(photo.id, e.target.value)}
                placeholder={t("captionPlaceholder")}
                className="min-h-[36px] w-full border border-erp-border bg-white px-2.5 py-1 text-xs text-erp-text-main focus:border-erp-navy focus:outline-none"
              />
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
