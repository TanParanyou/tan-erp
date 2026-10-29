"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Drawer } from "@/components/ui/Drawer";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { useToast } from "@/hooks/useToast";
import { formatCurrency, formatDate } from "@/lib/formatters/formatters";
import type {
  LaborRateResponse,
  LaborTimesheetResponse,
  ProjectControlResponse,
  ProjectTimesheetAction,
} from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import type { useProjectControlMutations } from "../api/project-queries";
import { isTimesheetStatus, timesheetStatusVariant } from "../project-status";

interface ProjectTimesheetsCardProps {
  control: ProjectControlResponse;
  canEdit: boolean;
  canApprove: boolean;
  mutations: ReturnType<typeof useProjectControlMutations>;
  describeError: (error: unknown) => string;
}

interface PendingTimesheetDecision {
  timesheet: LaborTimesheetResponse;
  action: "approve" | "reject";
}

const RATE_EDITABLE_STATUSES = ["planned", "active", "on_hold"];

export function ProjectTimesheetsCard({
  control,
  canEdit,
  canApprove,
  mutations,
  describeError,
}: ProjectTimesheetsCardProps) {
  const t = useTranslations("projects.control");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const membershipCtx = useSelectedMembership();
  const defaultWorkerId = membershipCtx?.currentUser?.user?.id ?? "";

  const canRecord = canEdit && control.status === "active";
  const canManageRates = canEdit && RATE_EDITABLE_STATUSES.includes(control.status ?? "");

  // Labor Rate state
  const [rateWorkerId, setRateWorkerId] = useState("");
  const [rateHourly, setRateHourly] = useState("150");
  const [rateError, setRateError] = useState<string | null>(null);

  // New Timesheet drawer state
  const [isCreating, setIsCreating] = useState(false);
  const [workerUserId, setWorkerUserId] = useState(defaultWorkerId);
  const [workDate, setWorkDate] = useState(() => new Date().toISOString().slice(0, 10));
  const [hours, setHours] = useState("8");
  const [wbsNodeId, setWbsNodeId] = useState("");
  const [taskDescription, setTaskDescription] = useState("");
  const [createError, setCreateError] = useState<string | null>(null);
  const createKeyRef = useRef(crypto.randomUUID());

  // Decision Modal state
  const [pendingDecision, setPendingDecision] = useState<PendingTimesheetDecision | null>(null);
  const [decisionNote, setDecisionNote] = useState("");
  const [decisionError, setDecisionError] = useState<string | null>(null);

  const [cardError, setCardError] = useState<string | null>(null);

  const laborRates: LaborRateResponse[] = control.laborRates ?? [];
  const timesheets: LaborTimesheetResponse[] = control.timesheets ?? [];
  const busy = Boolean(
    mutations.setLaborRate?.isPending ||
    mutations.createTimesheet?.isPending ||
    mutations.timesheetAction?.isPending
  );

  function openCreate(): void {
    createKeyRef.current = crypto.randomUUID();
    setWorkerUserId(defaultWorkerId);
    setWorkDate(new Date().toISOString().slice(0, 10));
    setHours("8");
    setWbsNodeId("");
    setTaskDescription("");
    setCreateError(null);
    setIsCreating(true);
  }

  async function handleSaveRate(): Promise<void> {
    setRateError(null);
    const hourly = Number(rateHourly);
    if (!rateWorkerId.trim() || !Number.isFinite(hourly) || hourly <= 0) {
      setRateError(t("laborRateInvalid"));
      return;
    }

    try {
      await mutations.setLaborRate.mutateAsync({
        workerUserId: rateWorkerId.trim(),
        hourlyRate: hourly,
      });
      toast.success(t("laborRateSaved"));
      setRateWorkerId("");
      setRateHourly("150");
    } catch (err: unknown) {
      setRateError(describeError(err));
    }
  }

  async function handleCreateTimesheet(): Promise<void> {
    setCreateError(null);
    const parsedHours = Number(hours);
    if (
      !workerUserId.trim() ||
      !workDate ||
      !taskDescription.trim() ||
      !Number.isFinite(parsedHours) ||
      parsedHours < 0.25 ||
      parsedHours > 24
    ) {
      setCreateError(t("timesheetInvalid"));
      return;
    }

    try {
      await mutations.createTimesheet.mutateAsync({
        payload: {
          workerUserId: workerUserId.trim(),
          workDate,
          hours: parsedHours,
          wbsNodeId: wbsNodeId || null,
          workOrderId: null,
          taskDescription: taskDescription.trim(),
        },
        idempotencyKey: createKeyRef.current,
      });
      toast.success(t("timesheetCreated"));
      setIsCreating(false);
    } catch (err: unknown) {
      setCreateError(describeError(err));
    }
  }

  async function runTimesheetAction(
    ts: LaborTimesheetResponse,
    action: ProjectTimesheetAction,
    note: string | null
  ): Promise<void> {
    setCardError(null);
    try {
      await mutations.timesheetAction.mutateAsync({
        timesheetId: ts.id ?? "",
        action,
        expectedVersion: ts.rowVersion ?? "",
        note,
      });
      toast.success(t(`timesheetActionDone.${action}`));
    } catch (err: unknown) {
      setCardError(describeError(err));
    }
  }

  function openDecision(ts: LaborTimesheetResponse, action: "approve" | "reject"): void {
    setPendingDecision({ timesheet: ts, action });
    setDecisionNote("");
    setDecisionError(null);
  }

  async function confirmDecision(): Promise<void> {
    if (!pendingDecision) return;
    setDecisionError(null);
    if (pendingDecision.action === "reject" && !decisionNote.trim()) {
      setDecisionError(t("timesheetReasonRequired"));
      return;
    }

    try {
      await mutations.timesheetAction.mutateAsync({
        timesheetId: pendingDecision.timesheet.id ?? "",
        action: pendingDecision.action,
        expectedVersion: pendingDecision.timesheet.rowVersion ?? "",
        note: decisionNote.trim() || null,
      });
      toast.success(t(`timesheetActionDone.${pendingDecision.action}`));
      setPendingDecision(null);
    } catch (err: unknown) {
      setDecisionError(describeError(err));
    }
  }

  return (
    <div className="erp-card p-5 space-y-6" role="region" aria-label={t("timesheetsTitle")}>
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h3 className="text-sm font-bold text-erp-navy">{t("timesheetsTitle")}</h3>
          <p className="mt-1 text-xs text-erp-text-muted">{t("timesheetsHint")}</p>
        </div>
        {canRecord && (
          <Button
            type="button"
            variant="primary"
            className="min-h-11"
            disabled={busy}
            onClick={openCreate}
          >
            {t("newTimesheet")}
          </Button>
        )}
      </div>

      {cardError && <p className="text-xs text-erp-danger" role="alert">{cardError}</p>}

      {/* Labor Rates Section */}
      <div className="border border-erp-border bg-erp-surface-subtle p-4 space-y-3">
        <div className="flex flex-wrap items-center justify-between gap-2">
          <h4 className="text-xs font-bold text-erp-navy">{t("laborRatesTitle")}</h4>
          <span className="text-[11px] text-erp-text-muted">{t("laborRateFallbackHint")}</span>
        </div>

        {laborRates.length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("laborRatesEmpty")}</p>
        ) : (
          <div className="flex flex-wrap gap-2">
            {laborRates.map((rate) => (
              <div
                key={rate.id}
                className="flex items-center gap-2 border border-erp-border bg-white px-3 py-1.5 text-xs"
              >
                <span className="font-medium text-erp-text-main">
                  {rate.worker?.displayName ?? rate.worker?.id ?? "-"}
                </span>
                <span className="font-mono text-erp-navy font-semibold">
                  {formatCurrency(rate.hourlyRate ?? 0, "THB", locale)}/hr
                </span>
              </div>
            ))}
          </div>
        )}

        {canManageRates && (
          <div className="grid grid-cols-1 gap-3 pt-2 sm:grid-cols-[1fr_160px_auto] sm:items-end">
            <Input
              label={t("laborRateWorker")}
              placeholder={t("workerUserIdPlaceholder")}
              value={rateWorkerId}
              disabled={busy}
              onChange={(e) => setRateWorkerId(e.target.value)}
            />
            <Input
              type="number"
              step="0.01"
              min={1}
              label={t("hourlyRate")}
              placeholder={t("hourlyRatePlaceholder")}
              value={rateHourly}
              disabled={busy}
              onChange={(e) => setRateHourly(e.target.value)}
            />
            <Button
              type="button"
              variant="primary"
              className="min-h-11"
              isLoading={mutations.setLaborRate.isPending}
              disabled={busy}
              onClick={() => void handleSaveRate()}
            >
              {t("saveLaborRate")}
            </Button>
          </div>
        )}
        {rateError && <p className="text-xs text-erp-danger" role="alert">{rateError}</p>}
      </div>

      {/* Timesheets List */}
      <div className="space-y-3">
        {timesheets.length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("timesheetsEmpty")}</p>
        ) : (
          <ul className="space-y-2">
            {timesheets.map((ts) => {
              const statusVal = ts.status ?? "";
              const canSubmit = canEdit && statusVal === "draft";
              const canCancel = canEdit && (statusVal === "draft" || statusVal === "submitted");
              const canDecide = canApprove && statusVal === "submitted";

              return (
                <li
                  key={ts.id}
                  className="flex flex-col gap-2 border border-erp-border bg-erp-surface-subtle p-3"
                >
                  <div className="flex flex-wrap items-center justify-between gap-2">
                    <div className="flex items-center gap-2">
                      <span className="font-mono font-bold text-erp-navy text-xs">{ts.number}</span>
                      <span className="text-xs text-erp-text-main font-medium">
                        {ts.worker?.displayName ?? ts.worker?.id ?? "-"}
                      </span>
                      <span className="text-xs text-erp-text-muted">
                        • {ts.workDate ? formatDate(ts.workDate, locale) : "-"}
                      </span>
                    </div>
                    <div className="flex items-center gap-2">
                      {ts.isOvertimeWarning && (
                        <StatusBadge
                          label={t("timesheetOvertimeWarning")}
                          variant="warning"
                        />
                      )}
                      <StatusBadge
                        label={isTimesheetStatus(ts.status) ? t(`timesheetStatuses.${ts.status}`) : "-"}
                        variant={timesheetStatusVariant(ts.status)}
                      />
                    </div>
                  </div>

                  <p className="text-xs text-erp-text-body">{ts.taskDescription}</p>

                  <div className="flex flex-wrap items-center justify-between gap-2 border-t border-erp-border pt-2 text-xs">
                    <div className="flex flex-wrap items-center gap-3 font-mono text-erp-text-muted">
                      <span>{ts.hours ?? 0} hrs</span>
                      <span>@ {formatCurrency(ts.hourlyRate ?? 0, "THB", locale)}/hr</span>
                      <span className="font-bold text-erp-navy">
                        = {formatCurrency(ts.costAmount ?? 0, "THB", locale)}
                      </span>
                      {ts.wbsCode && (
                        <span className="border border-erp-border px-1.5 py-0.5 text-[11px] bg-white">
                          WBS: {ts.wbsCode}
                        </span>
                      )}
                    </div>

                    <div className="flex items-center gap-2">
                      {canSubmit && (
                        <Button
                          type="button"
                          variant="outline"
                          size="sm"
                          className="min-h-8 text-xs"
                          disabled={busy}
                          onClick={() => void runTimesheetAction(ts, "submit", null)}
                        >
                          {t("submitTimesheet")}
                        </Button>
                      )}
                      {canCancel && (
                        <Button
                          type="button"
                          variant="outline"
                          size="sm"
                          className="min-h-8 text-xs"
                          disabled={busy}
                          onClick={() => void runTimesheetAction(ts, "cancel", null)}
                        >
                          {t("cancelTimesheet")}
                        </Button>
                      )}
                      {canDecide && (
                        <>
                          <Button
                            type="button"
                            variant="primary"
                            size="sm"
                            className="min-h-8 text-xs"
                            disabled={busy}
                            onClick={() => openDecision(ts, "approve")}
                          >
                            {t("approveTimesheet")}
                          </Button>
                          <Button
                            type="button"
                            variant="outline"
                            size="sm"
                            className="min-h-8 text-xs text-erp-danger"
                            disabled={busy}
                            onClick={() => openDecision(ts, "reject")}
                          >
                            {t("rejectTimesheet")}
                          </Button>
                        </>
                      )}
                    </div>
                  </div>
                </li>
              );
            })}
          </ul>
        )}
      </div>

      {/* New Timesheet Drawer */}
      <Drawer
        isOpen={isCreating}
        onClose={() => { if (!busy) setIsCreating(false); }}
        title={t("newTimesheet")}
        description={t("timesheetCreateDesc")}
        size="md"
        closeDisabled={busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              className="min-h-11"
              disabled={busy}
              onClick={() => setIsCreating(false)}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              className="min-h-11"
              isLoading={mutations.createTimesheet.isPending}
              disabled={busy}
              onClick={() => void handleCreateTimesheet()}
            >
              {t("createTimesheet")}
            </Button>
          </div>
        )}
      >
        <div className="space-y-4">
          <Input
            label={t("timesheetWorker")}
            required
            placeholder={t("workerUserIdPlaceholder")}
            value={workerUserId}
            disabled={busy}
            onChange={(e) => setWorkerUserId(e.target.value)}
          />
          <Input
            type="date"
            label={t("timesheetDate")}
            required
            placeholder={t("datePlaceholder")}
            value={workDate}
            disabled={busy}
            onChange={(e) => setWorkDate(e.target.value)}
          />
          <Input
            type="number"
            step="0.25"
            min={0.25}
            max={24}
            label={t("timesheetHours")}
            required
            placeholder={t("timesheetHoursPlaceholder")}
            value={hours}
            disabled={busy}
            onChange={(e) => setHours(e.target.value)}
          />
          <Select
            label={t("timesheetWbsNode")}
            value={wbsNodeId}
            disabled={busy}
            options={[
              { value: "", label: t("timesheetSelectWbs") },
              ...(control.wbsNodes ?? []).map((n) => ({
                value: n.id ?? "",
                label: `${n.code} — ${n.name}`,
              })),
            ]}
            onChange={(e) => setWbsNodeId(e.target.value)}
          />
          <Input
            label={t("timesheetTask")}
            required
            placeholder={t("timesheetTaskPlaceholder")}
            value={taskDescription}
            maxLength={500}
            disabled={busy}
            onChange={(e) => setTaskDescription(e.target.value)}
          />
          {createError && <p className="text-xs text-erp-danger" role="alert">{createError}</p>}
        </div>
      </Drawer>

      {/* Decision Modal */}
      <Modal
        isOpen={pendingDecision !== null}
        onClose={() => { if (!busy) setPendingDecision(null); }}
        title={pendingDecision?.action === "approve" ? t("approveTimesheetTitle") : t("rejectTimesheetTitle")}
        description={pendingDecision?.action === "approve" ? t("approveTimesheetDesc") : t("rejectTimesheetDesc")}
        size="sm"
        closeDisabled={busy}
        footer={(
          <div className="flex justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              className="min-h-11"
              disabled={busy}
              onClick={() => setPendingDecision(null)}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant={pendingDecision?.action === "reject" ? "danger" : "primary"}
              className="min-h-11"
              isLoading={mutations.timesheetAction.isPending}
              disabled={busy}
              onClick={() => void confirmDecision()}
            >
              {pendingDecision?.action === "reject" ? t("rejectTimesheet") : t("approveTimesheet")}
            </Button>
          </div>
        )}
      >
        <div className="space-y-3">
          <Input
            label={t("decisionNoteLabel")}
            placeholder={t("decisionNotePlaceholder")}
            value={decisionNote}
            maxLength={500}
            disabled={busy}
            required={pendingDecision?.action === "reject"}
            error={decisionError ?? undefined}
            onChange={(e) => {
              setDecisionNote(e.target.value);
              setDecisionError(null);
            }}
          />
        </div>
      </Modal>
    </div>
  );
}
