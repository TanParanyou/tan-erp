"use client";

import { useRef, useState } from "react";
import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { Alert } from "@/components/ui/Alert";
import { UserAutocomplete } from "@/components/forms/UserAutocomplete";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { formatCurrency } from "@/lib/formatters/formatters";
import { useCreateProjectFromHandover, useProjectHandoverSource } from "../api/project-queries";

interface ProjectHandoverSectionProps {
  opportunityId: string;
  opportunityName: string;
  branchId?: string;
  isWon: boolean;
  canRead: boolean;
  canCreate: boolean;
}

/** Shows the project created from a Won opportunity, or lets an authorised user hand the accepted quotation over. */
export function ProjectHandoverSection({
  opportunityId,
  opportunityName,
  branchId,
  isWon,
  canRead,
  canCreate,
}: ProjectHandoverSectionProps) {
  const t = useTranslations("projects");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const sourceQuery = useProjectHandoverSource(opportunityId, isWon && canRead);
  const createMutation = useCreateProjectFromHandover(opportunityId);

  const [isOpen, setIsOpen] = useState(false);
  const [ownerUserId, setOwnerUserId] = useState("");
  const [plannedStart, setPlannedStart] = useState("");
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);
  // One key per modal session so a retried submit replays instead of creating a second project.
  const keyRef = useRef(crypto.randomUUID());

  if (!isWon || !canRead) return null;

  const source = sourceQuery.data;
  if (sourceQuery.isLoading || !source) return null;

  function describeError(err: unknown): string {
    if (err instanceof ApiError) {
      if (err.code === "PROJECT_ALREADY_EXISTS") return t("errors.alreadyExists");
      if (err.code === "PROJECT_HANDOVER_NOT_ALLOWED") return t("errors.notAllowed");
      if (err.code === "QUOTATION_VERSION_CONFLICT") return t("errors.versionConflict");
    }
    return t("errors.failed");
  }

  async function handleSubmit(event: React.FormEvent): Promise<void> {
    event.preventDefault();
    if (!source) return;
    if (!ownerUserId) {
      setError(t("ownerRequired"));
      return;
    }

    try {
      const project = await createMutation.mutateAsync({
        payload: {
          quotationId: source.quotationId,
          expectedQuotationVersion: source.quotationRowVersion,
          ownerUserId,
          plannedStartDate: plannedStart || null,
          name: name.trim() || null,
        },
        idempotencyKey: keyRef.current,
      });
      toast.success(t("handoverSuccess", { code: project.code ?? "-" }));
      setIsOpen(false);
    } catch (err: unknown) {
      setError(describeError(err));
    }
  }

  return (
    <div className="erp-card p-5 flex flex-col gap-3" role="region" aria-label={t("handoverTitle")}>
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h3 className="text-sm font-bold text-erp-navy">{t("handoverTitle")}</h3>
          <p className="mt-1 text-xs text-erp-text-muted">
            {t("handoverSource", { number: source.quotationNumber ?? "-", amount: formatCurrency(source.contractAmount, "THB", locale) })}
          </p>
        </div>
        {source.existingProjectId ? (
          <Link href={`/${locale}/projects/${source.existingProjectId}`} className="font-mono text-sm font-semibold text-erp-navy underline">
            {t("openProject", { code: source.existingProjectCode ?? "-" })}
          </Link>
        ) : canCreate ? (
          <Button type="button" variant="primary" className="min-h-11" onClick={() => { setError(null); setIsOpen(true); }}>
            {t("handoverAction")}
          </Button>
        ) : (
          <span className="text-xs text-erp-text-muted">{t("handoverPermissionRequired")}</span>
        )}
      </div>

      <Modal
        isOpen={isOpen}
        onClose={() => { if (!createMutation.isPending) setIsOpen(false); }}
        title={t("handoverModalTitle")}
        description={t("handoverModalDesc")}
        size="md"
        closeDisabled={createMutation.isPending}
        closeOnOverlayClick={!createMutation.isPending}
        closeOnEscape={!createMutation.isPending}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={createMutation.isPending} onClick={() => setIsOpen(false)}>
              {tCommon("actions.cancel")}
            </Button>
            <Button type="submit" form="project-handover-form" variant="primary" className="min-h-11" isLoading={createMutation.isPending} disabled={createMutation.isPending}>
              {t("handoverConfirm")}
            </Button>
          </div>
        )}
      >
        <form id="project-handover-form" onSubmit={(event) => void handleSubmit(event)} className="space-y-4">
          {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
          <Input
            label={t("projectName")}
            value={name}
            maxLength={200}
            placeholder={opportunityName}
            helperText={t("projectNameHelp")}
            disabled={createMutation.isPending}
            onChange={(event) => setName(event.target.value)}
          />
          <UserAutocomplete
            value={ownerUserId}
            onChange={(userId) => { setOwnerUserId(userId); setError(null); }}
            branchId={branchId}
            required
            disabled={createMutation.isPending}
            label={t("owner")}
            placeholder={t("ownerPlaceholder")}
          />
          <Input
            type="date"
            label={t("plannedStartDate")}
            value={plannedStart}
            disabled={createMutation.isPending}
            onChange={(event) => setPlannedStart(event.target.value)}
          />
        </form>
      </Modal>
    </div>
  );
}
