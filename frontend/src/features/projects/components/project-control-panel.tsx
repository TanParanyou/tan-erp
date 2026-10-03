"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { useToast } from "@/hooks/useToast";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useProjectControl, useProjectControlMutations } from "../api/project-queries";
import { allowedProjectTransitions, isProjectStatus, projectStatusVariant, transitionNeedsReason, type ProjectStatusValue } from "../project-status";
import { projectControlErrorCode } from "./project-control-errors";
import { ProjectPlanCard } from "./project-plan-card";
import { ProjectBudgetCard } from "./project-budget-card";
import { ProjectMilestonesCard } from "./project-milestones-card";
import { ProjectChangeOrdersCard } from "./project-change-orders-card";
import { ProjectHistoryCard } from "./project-history-card";

interface ProjectControlPanelProps {
  projectId: string;
}

/** Plan, baseline budget, milestones/progress, change orders and status history of one project. */
export function ProjectControlPanel({ projectId }: ProjectControlPanelProps) {
  const t = useTranslations("projects");
  const tCommon = useTranslations("common");
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const { data: control, isLoading, isError } = useProjectControl(projectId);
  const mutations = useProjectControlMutations(projectId);

  const [target, setTarget] = useState<ProjectStatusValue | null>(null);
  const [reason, setReason] = useState("");
  const [reasonError, setReasonError] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  if (isLoading) {
    return <div className="py-10 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  }

  if (isError || !control) {
    return <Alert variant="danger">{t("control.loadError")}</Alert>;
  }

  const canUpdate = can(selectedMembership, PERMISSIONS.PROJECTS_UPDATE);
  const canTransition = can(selectedMembership, PERMISSIONS.PROJECTS_TRANSITION);
  const canManageChangeOrders = can(selectedMembership, PERMISSIONS.PROJECTS_CHANGE_ORDERS_MANAGE);
  const canApproveChangeOrders = can(selectedMembership, PERMISSIONS.PROJECTS_CHANGE_ORDERS_APPROVE);
  const status = control.status ?? "";
  const transitions = canTransition ? allowedProjectTransitions(status) : [];
  const isTransitioning = mutations.transition.isPending;

  function describe(error: unknown): string {
    const code = projectControlErrorCode(error);
    return code ? t(`control.errors.${code}`) : t("control.errors.failed");
  }

  function openTransition(next: ProjectStatusValue): void {
    setTarget(next);
    setReason("");
    setReasonError(false);
    setMessage(null);
  }

  async function confirmTransition(): Promise<void> {
    if (!target || !control) return;
    const needsReason = transitionNeedsReason(status, target);
    if (needsReason && !reason.trim()) {
      setReasonError(true);
      return;
    }

    try {
      await mutations.transition.mutateAsync({ rowVersion: control.rowVersion ?? "", targetStatus: target, reason: reason.trim() || null });
      toast.success(t("control.transitionSuccess", { status: t(`statuses.${target}`) }));
      setTarget(null);
    } catch (error: unknown) {
      setMessage(describe(error));
      setTarget(null);
    }
  }

  return (
    <div className="space-y-5">
      <div className="erp-card p-5 flex flex-wrap items-center justify-between gap-3">
        <div className="flex flex-col gap-1">
          <div className="flex items-center gap-2">
            <h3 className="text-sm font-bold text-erp-navy">{t("control.statusTitle")}</h3>
            <StatusBadge label={isProjectStatus(status) ? t(`statuses.${status}`) : "-"} variant={projectStatusVariant(status)} />
          </div>
          {control.statusReason && <p className="text-xs text-erp-text-muted">{t("control.statusReason", { reason: control.statusReason })}</p>}
        </div>
        {transitions.length > 0 && (
          <div className="flex flex-wrap gap-2">
            {transitions.map((next) => (
              <Button
                key={next}
                type="button"
                variant={next === "cancelled" ? "danger" : "outline"}
                size="sm"
                className="min-h-11"
                onClick={() => openTransition(next)}
              >
                {t(`control.transitions.${next}`)}
              </Button>
            ))}
          </div>
        )}
      </div>

      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}

      <ProjectPlanCard control={control} canEdit={canUpdate} mutation={mutations.setPlan} describeError={describe} />
      <ProjectBudgetCard control={control} canEdit={canUpdate} mutation={mutations.replaceBudget} describeError={describe} />
      <ProjectMilestonesCard control={control} canEdit={canUpdate} mutations={mutations} describeError={describe} />
      <ProjectChangeOrdersCard
        control={control}
        canManage={canManageChangeOrders}
        canApprove={canApproveChangeOrders}
        mutations={mutations}
        describeError={describe}
      />
      <ProjectHistoryCard history={control.history ?? []} />

      <Modal
        isOpen={target !== null}
        onClose={() => { if (!isTransitioning) setTarget(null); }}
        title={target ? t(`control.transitions.${target}`) : undefined}
        description={target ? t(`control.transitionDescriptions.${target}`) : undefined}
        size="sm"
        closeDisabled={isTransitioning}
        closeOnOverlayClick={!isTransitioning}
        closeOnEscape={!isTransitioning}
        footer={(
          <div className="flex justify-end gap-3">
            <Button type="button" variant="outline" className="min-h-11" disabled={isTransitioning} onClick={() => setTarget(null)}>
              {tCommon("actions.cancel")}
            </Button>
            <Button type="button" variant={target === "cancelled" ? "danger" : "primary"} className="min-h-11" isLoading={isTransitioning} disabled={isTransitioning} onClick={() => void confirmTransition()}>
              {t("control.confirm")}
            </Button>
          </div>
        )}
      >
        {target && (
          <Input
            label={t("control.reason")}
            required={transitionNeedsReason(status, target)}
            value={reason}
            maxLength={500}
            disabled={isTransitioning}
            error={reasonError ? t("control.reasonRequired") : undefined}
            onChange={(event) => {
              setReason(event.target.value);
              if (event.target.value.trim()) setReasonError(false);
            }}
          />
        )}
      </Modal>
    </div>
  );
}
