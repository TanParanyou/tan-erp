"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { FormSection } from "@/components/forms/FormSection";
import { useToast } from "@/hooks/useToast";
import type { RoleDefinitionResponse } from "@/lib/api/api-client";
import { useSetRoleDefinitionActive } from "../api/role-management-queries";
import { roleAdminErrorKey } from "../role-management-errors";

interface RoleActivationPanelProps {
  role: RoleDefinitionResponse;
  canManage: boolean;
}

export function RoleActivationPanel({ role, canManage }: RoleActivationPanelProps) {
  const t = useTranslations("roleAdmin");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const setActive = useSetRoleDefinitionActive();

  // System roles are immutable; there is nothing to show.
  if (!canManage || role.isSystem === true) return null;
  const { id: roleId, rowVersion } = role;
  if (!roleId || !rowVersion) return <Alert variant="danger">{t("errors.GENERIC")}</Alert>;

  const isActive = role.isActive === true;
  const members = role.memberCount ?? 0;
  const pendingAssignments = role.pendingAssignmentRequestCount ?? 0;
  const inUse = members > 0 || pendingAssignments > 0;
  // The server decides; this only mirrors its rules so the button is not offered when it would be refused.
  const blocked = isActive && (inUse || role.pendingChange != null);
  const key = isActive ? "deactivate" : "activate";

  const confirm = async () => {
    setErrorMessage(null);
    try {
      await setActive.mutateAsync({ roleId, active: !isActive, ifMatch: rowVersion });
      toast.success(t(`activation.${key}.success`));
    } catch (error: unknown) {
      setErrorMessage(t(`errors.${roleAdminErrorKey(error)}`));
    } finally {
      setConfirmOpen(false);
    }
  };

  return (
    <FormSection title={t("activation.title")}>
      {errorMessage ? <Alert variant="danger">{errorMessage}</Alert> : null}
      {isActive && inUse ? (
        <Alert variant="warning">
          {members > 0 ? <p>{t("activation.membersCount", { count: members })}</p> : null}
          {pendingAssignments > 0 ? <p>{t("activation.pendingAssignments", { count: pendingAssignments })}</p> : null}
          <p>{t("activation.inUseHint")}</p>
        </Alert>
      ) : null}
      <Button variant={isActive ? "danger" : "primary"} size="md" disabled={blocked} onClick={() => setConfirmOpen(true)}>
        {t(`activation.${key}.action`)}
      </Button>
      <ConfirmationModal
        isOpen={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        onConfirm={confirm}
        title={t(`activation.${key}.action`)}
        message={t(`activation.${key}.message`)}
        confirmText={t(`activation.${key}.confirm`)}
        cancelText={tCommon("actions.cancel")}
        variant={isActive ? "danger" : "info"}
        isLoading={setActive.isPending}
      />
    </FormSection>
  );
}
