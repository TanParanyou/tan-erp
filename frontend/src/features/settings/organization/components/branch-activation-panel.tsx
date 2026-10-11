"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Textarea } from "@/components/ui/Textarea";
import { FormSection } from "@/components/forms/FormSection";
import { useToast } from "@/hooks/useToast";
import type { AdminBranchResponse } from "@/lib/api/api-client";
import { useBranchDeactivationCheck, useSetAdminBranchActive } from "../api/organization-admin-queries";
import { organizationAdminErrorKey } from "../organization-admin-errors";

const MAX_REASON_LENGTH = 500;

interface BranchActivationPanelProps {
  branch: AdminBranchResponse;
  canManage: boolean;
}

export function BranchActivationPanel({ branch, canManage }: BranchActivationPanelProps) {
  const t = useTranslations("organizationAdmin");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const [reason, setReason] = useState("");
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const isActive = branch.isActive === true;
  const branchId = branch.id ?? "";
  const checkQuery = useBranchDeactivationCheck(branchId, canManage && isActive && branchId !== "");
  const setActive = useSetAdminBranchActive();

  if (!canManage) return null;
  // Narrowed so the confirm closure keeps non-optional identifiers (id + concurrency token).
  const rowVersion = branch.rowVersion;
  if (!branch.id || !rowVersion) return <Alert variant="danger">{t("errors.GENERIC")}</Alert>;
  const targetBranchId = branch.id;

  // Blockers come straight from the server; the server also decides `canDeactivate`, so the UI never recomputes it.
  const blockers = checkQuery.data?.blockers ?? [];
  const trimmedReason = reason.trim();
  const canOpenDeactivate = isActive && checkQuery.data?.canDeactivate === true && trimmedReason !== "";

  const confirm = async () => {
    setErrorMessage(null);
    try {
      if (isActive) {
        await setActive.mutateAsync({ branchId: targetBranchId, active: false, reason: trimmedReason, ifMatch: rowVersion });
        toast.success(t("branches.deactivate.success"));
        setReason("");
      } else {
        await setActive.mutateAsync({ branchId: targetBranchId, active: true, ifMatch: rowVersion });
        toast.success(t("branches.activate.success"));
      }
    } catch (error: unknown) {
      setErrorMessage(t(`errors.${organizationAdminErrorKey(error)}`));
    } finally {
      setConfirmOpen(false);
    }
  };

  const title = isActive ? t("branches.deactivate.title") : t("branches.activate.title");
  const message = isActive ? t("branches.deactivate.message") : t("branches.activate.message");

  return (
    <FormSection title={title}>
      {errorMessage ? <Alert variant="danger">{errorMessage}</Alert> : null}
      {isActive && blockers.length > 0 ? (
        <Alert variant="warning">
          <p className="font-medium">{t("branches.deactivate.blockedTitle")}</p>
          <p>{t("branches.deactivate.blockedIntro")}</p>
          <ul className="list-disc pl-5">
            {blockers.map((blocker, index) => (
              <li key={`${blocker.type ?? "unknown"}-${index}`}>
                {blocker.type ? t(`branches.blockers.${blocker.type}`) : "-"}
                {blocker.type === "last_active_branch" ? "" : ` (${blocker.count ?? "-"})`}
              </li>
            ))}
          </ul>
        </Alert>
      ) : null}
      {isActive ? (
        <Textarea
          label={t("branches.deactivate.reasonLabel")}
          rows={2}
          maxLength={MAX_REASON_LENGTH}
          value={reason}
          onChange={(event) => setReason(event.target.value)}
        />
      ) : null}
      <Button
        variant={isActive ? "danger" : "primary"}
        size="md"
        disabled={isActive ? !canOpenDeactivate : false}
        onClick={() => setConfirmOpen(true)}
      >
        {isActive ? t("branches.deactivate.action") : t("branches.activate.action")}
      </Button>
      <ConfirmationModal
        isOpen={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        onConfirm={confirm}
        title={title}
        message={message}
        confirmText={isActive ? t("branches.deactivate.confirm") : t("branches.activate.confirm")}
        cancelText={tCommon("actions.cancel")}
        variant={isActive ? "danger" : "info"}
        isLoading={setActive.isPending}
      />
    </FormSection>
  );
}
