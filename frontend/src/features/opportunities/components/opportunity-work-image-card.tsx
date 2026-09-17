"use client";

import React from "react";
import Image from "next/image";
import { IconEye, IconTrash } from "@/components/common/Icons";
import { Badge } from "@/components/ui/Badge";
import { resolveOpportunityStageLabel } from "../opportunity-labels";
import { fileClient } from "@/lib/api/file-client";
import { useTranslations } from "next-intl";
import type { OpportunityWorkImageResponse } from "@/lib/api/api-client";

export interface OpportunityWorkImageCardProps {
  image: OpportunityWorkImageResponse;
  onClick: () => void;
  canManage?: boolean;
  isClosed?: boolean;
  onDetach?: (image: OpportunityWorkImageResponse) => void;
}

/**
 * OpportunityWorkImageCard:
 * - Subcomponent for rendering individual work photo in the gallery
 * - 0px sharp aesthetics complying with Atelier Architectural Navy Sharp
 * - Stage badge with full Thai i18n support
 * - Hover preview overlay
 * - Safe nullability with standard "-" fallback
 */
export function OpportunityWorkImageCard({
  image,
  onClick,
  canManage = false,
  isClosed = false,
  onDetach,
}: OpportunityWorkImageCardProps) {
  const t = useTranslations("opportunities");
  const tCommon = useTranslations("common.actions");

  const stageLabel = resolveOpportunityStageLabel(image.stageAtAttach, t);
  const uploaderName = image.createdBy?.displayName ?? "-";

  return (
    <div className="border border-erp-border bg-erp-surface group relative flex flex-col justify-between overflow-hidden shadow-none rounded-none hover:border-erp-navy transition-colors">
      {/* Thumbnail Container */}
      <div
        className="relative aspect-video w-full bg-erp-surface-muted cursor-pointer overflow-hidden"
        onClick={onClick}
        role="button"
        tabIndex={0}
        onKeyDown={(e) => {
          if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            onClick();
          }
        }}
        aria-label={image.caption || stageLabel}
      >
        <Image
          src={fileClient.getFileUrl(image.fileId ?? "")}
          alt={image.caption || stageLabel}
          fill
          unoptimized
          className="object-cover transition-transform group-hover:scale-105"
        />

        {/* Stage Badge */}
        <div className="absolute top-1.5 left-1.5 z-10">
          <Badge variant="primary" size="sm" className="shadow-sm">
            {stageLabel}
          </Badge>
        </div>

        {/* Hover overlay with Eye icon */}
        <div className="absolute inset-0 bg-erp-navy/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
          <span className="p-2 bg-erp-surface text-erp-navy rounded-none shadow-md">
            <IconEye size={18} strokeWidth={2} />
          </span>
        </div>
      </div>

      {/* Caption & Metadata Footer */}
      <div className="p-3 space-y-2 text-xs bg-erp-surface-subtle border-t border-erp-border flex-1 flex flex-col justify-between">
        <p
          className="text-erp-text-main line-clamp-2 text-xs font-medium leading-relaxed"
          title={image.caption ?? undefined}
        >
          {image.caption || <span className="text-erp-text-muted italic">-</span>}
        </p>

        <div className="flex items-center justify-between text-[11px] font-mono text-erp-text-muted pt-2 border-t border-erp-border/60">
          <span className="truncate max-w-[130px]">{uploaderName}</span>

          {canManage && !isClosed && onDetach && (
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                onDetach(image);
              }}
              className="inline-flex items-center gap-1 text-destructive hover:underline ml-2 uppercase font-semibold text-[10px] cursor-pointer"
              title={tCommon("delete")}
              aria-label={`${tCommon("delete")} ${image.caption || stageLabel}`}
            >
              <IconTrash size={12} />
              <span>{tCommon("delete")}</span>
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

export default OpportunityWorkImageCard;
