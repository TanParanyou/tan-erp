"use client";

import React from "react";
import { useFormContext, useWatch } from "react-hook-form";
import { useTranslations } from "next-intl";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { Checkbox } from "@/components/ui/Checkbox";
import { Select } from "@/components/ui/Select";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useInfiniteOpportunityWorkImages } from "@/features/opportunities/api/opportunity-queries";
import type { OpportunityWorkImageResponse } from "@/lib/api/api-client";
import {
  SURVEY_EVIDENCE_KINDS,
  type SurveyEvidenceKindValue,
  type SurveyEvidenceItemFormData,
  type SurveyWorkspaceFormData,
} from "../schemas/survey-workspace-schema";

interface SurveyEvidenceSectionProps {
  opportunityId: string;
  minimumEvidenceCount: number;
  isReady: boolean;
}

const MAX_CAPTION_LENGTH = 500;

/**
 * Evidence picker. Files are the opportunity's already-uploaded work images (File Service, parent = opportunity),
 * so nothing is uploaded here; new photos are added from the opportunity's work image gallery.
 */
export function SurveyEvidenceSection({ opportunityId, minimumEvidenceCount, isReady }: SurveyEvidenceSectionProps) {
  const t = useTranslations("surveys");
  const { control, setValue } = useFormContext<SurveyWorkspaceFormData>();
  const evidence = useWatch({ control, name: "evidence" }) ?? [];
  const { data, isLoading, isError, hasNextPage, fetchNextPage, isFetchingNextPage } =
    useInfiniteOpportunityWorkImages(opportunityId);

  const images = (data?.pages.flatMap((page) => page.items ?? []) ?? []).filter(
    (img): img is OpportunityWorkImageResponse & { fileId: string } => Boolean(img?.fileId)
  );
  // Evidence already saved may point at a photo that was later detached from the gallery; keep it visible.
  const knownFileIds = new Set(images.map((img) => img.fileId));
  const orphanEvidence = evidence.filter((e) => !knownFileIds.has(e.fileId));

  const kindOptions = SURVEY_EVIDENCE_KINDS.map((value) => ({ value, label: t(`evidenceKinds.${value}`) }));

  const toggle = (fileId: string, checked: boolean) => {
    if (checked) {
      const next: SurveyEvidenceItemFormData = {
        fileId,
        kind: "site_photo",
        caption: null,
        sortOrder: evidence.length + 1,
      };
      setValue("evidence", [...evidence, next], { shouldDirty: true });
    } else {
      setValue(
        "evidence",
        evidence.filter((e) => e.fileId !== fileId),
        { shouldDirty: true }
      );
    }
  };

  const update = (fileId: string, patch: Partial<SurveyEvidenceItemFormData>) => {
    setValue(
      "evidence",
      evidence.map((e) => (e.fileId === fileId ? { ...e, ...patch } : e)),
      { shouldDirty: true }
    );
  };

  const renderRow = (fileId: string, label: string) => {
    const entry = evidence.find((e) => e.fileId === fileId);
    return (
      <div key={fileId} className="border border-erp-border bg-erp-surface-subtle p-2 space-y-2">
        <div className="relative aspect-video bg-erp-surface-muted overflow-hidden">
          <AuthenticatedFileImage fileId={fileId} alt={label} loading="lazy" className="absolute inset-0 h-full w-full object-cover" />
        </div>
        <Checkbox
          id={`survey-evidence-use-${fileId}`}
          label={t("evidenceUseAction")}
          checked={Boolean(entry)}
          disabled={isReady}
          onChange={(e) => toggle(fileId, e.target.checked)}
        />
        {entry && (
          <div className="space-y-2">
            <Select
              aria-label={t("evidenceKindLabel")}
              value={entry.kind}
              disabled={isReady}
              options={kindOptions}
              onChange={(e) => update(fileId, { kind: e.target.value as SurveyEvidenceKindValue })}
            />
            <Input
              aria-label={t("evidenceCaptionLabel")}
              placeholder={t("evidenceCaptionLabel")}
              value={entry.caption ?? ""}
              maxLength={MAX_CAPTION_LENGTH}
              disabled={isReady}
              onChange={(e) => update(fileId, { caption: e.target.value })}
            />
          </div>
        )}
      </div>
    );
  };

  return (
    <section className="space-y-3" aria-label={t("evidenceTitle")}>
      <div>
        <h4 className="text-sm font-bold text-erp-navy">
          {t("evidenceTitle")}
          <span className="ml-2 font-mono text-[11px] text-erp-text-muted">
            {evidence.length}/{Math.max(minimumEvidenceCount, evidence.length)}
          </span>
        </h4>
        <p className="text-xs text-erp-text-muted mt-0.5">{t("evidenceDesc", { count: minimumEvidenceCount })}</p>
      </div>

      {isLoading ? (
        <div className="py-6 flex justify-center text-xs font-mono text-erp-text-muted">
          <MonoSpinner size="md" />
        </div>
      ) : isError ? (
        <p className="text-xs text-erp-danger">{t("evidenceLoadError")}</p>
      ) : images.length === 0 && orphanEvidence.length === 0 ? (
        <p className="text-xs text-erp-text-muted border border-dashed border-erp-border p-4">{t("evidenceEmptyHint")}</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          {images.map((img) => renderRow(img.fileId, img.caption ?? t("evidenceUseAction")))}
          {orphanEvidence.map((e) => renderRow(e.fileId, t("evidenceUseAction")))}
        </div>
      )}

      {hasNextPage && (
        <button
          type="button"
          className="text-xs font-semibold text-erp-navy underline min-h-11"
          disabled={isFetchingNextPage}
          onClick={() => void fetchNextPage()}
        >
          {t("evidenceLoadMore")}
        </button>
      )}
    </section>
  );
}
