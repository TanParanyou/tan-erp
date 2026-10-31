"use client";

import React, { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { useToast } from "@/hooks/useToast";
import type { TechnicianAssignmentResponse } from "@/lib/api/api-client";
import { ApiError } from "@/lib/api/api-error";
import { formatDate } from "@/lib/formatters/formatters";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useUserList } from "@/features/users/api/user-queries";
import { useTechnicianScheduleMutations, useTechnicianSchedules } from "../api/service-queries";
import { serviceErrorCode } from "../service-status";

export function TechnicianSchedulePanel() {
  const t = useTranslations("service.schedules");
  const tErrors = useTranslations("service.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.SERVICE_REQUESTS_MANAGE);

  // Filters
  const [filterDate, setFilterDate] = useState<string>("");
  const [filterTechnician, setFilterTechnician] = useState<string>("");

  const schedulesQuery = useTechnicianSchedules({
    fromDate: filterDate || undefined,
    toDate: filterDate || undefined,
    technicianUserId: filterTechnician || undefined,
  });
  const usersQuery = useUserList({ limit: 100 });
  const mutations = useTechnicianScheduleMutations();

  // Create form state
  const [selectedTech, setSelectedTech] = useState<string>("");
  const [workDate, setWorkDate] = useState<string>("");
  const [assignmentKind, setAssignmentKind] = useState<string>("service_request");
  const [serviceReqId, setServiceReqId] = useState<string>("");
  const [installationJobId, setInstallationJobId] = useState<string>("");
  const [note, setNote] = useState<string>("");
  const [allowConflict, setAllowConflict] = useState<boolean>(false);
  const [formError, setFormError] = useState<string | null>(null);

  // Delete modal state
  const [deletingAssignment, setDeletingAssignment] = useState<TechnicianAssignmentResponse | null>(null);

  const isBusy = mutations.create.isPending || mutations.remove.isPending;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? serviceErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  async function handleCreate(e: React.FormEvent): Promise<void> {
    e.preventDefault();
    setFormError(null);
    if (!selectedTech || !workDate) {
      setFormError(tErrors("SERVICE_FIELD_REQUIRED"));
      return;
    }

    try {
      await mutations.create.mutateAsync({
        technicianUserId: selectedTech,
        workDate,
        assignmentKind,
        serviceRequestId: serviceReqId.trim() || undefined,
        installationJobId: installationJobId.trim() || undefined,
        note: note.trim() || undefined,
        allowConflict,
      });
      toast.success(t("createSuccess"));
      setSelectedTech("");
      setWorkDate("");
      setServiceReqId("");
      setInstallationJobId("");
      setNote("");
      setAllowConflict(false);
    } catch (err: unknown) {
      setFormError(describe(err));
    }
  }

  async function handleDeleteConfirm(): Promise<void> {
    if (!deletingAssignment?.id) return;
    try {
      await mutations.remove.mutateAsync(deletingAssignment.id);
      toast.success(t("deleteSuccess"));
      setDeletingAssignment(null);
    } catch (err: unknown) {
      toast.error(describe(err));
    }
  }

  const items = schedulesQuery.data?.items ?? [];
  const techOptions = (usersQuery.data?.items ?? []).map((u) => ({
    value: u.id ?? "",
    label: `${u.displayName ?? u.email ?? "-"} (${u.email ?? "-"})`,
  }));

  return (
    <div className="space-y-6">
      {/* Filter toolbar */}
      <div className="erp-card p-4 flex flex-wrap items-center justify-between gap-4">
        <div>
          <h3 className="text-base font-bold text-erp-navy">{t("title")}</h3>
          <p className="text-xs text-erp-text-muted">{t("subtitle")}</p>
        </div>
        <div className="flex flex-wrap items-center gap-3">
          <Input
            id="schedule-filter-date"
            type="date"
            label={t("workDate")}
            placeholder={t("datePlaceholder")}
            value={filterDate}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            onChange={(e) => setFilterDate(e.target.value)}
          />
          <Select
            id="schedule-filter-tech"
            label={t("technician")}
            placeholder={t("selectTechnician")}
            value={filterTechnician}
            wrapperClassName="!mb-0"
            className="h-11 min-h-[44px]"
            options={[{ value: "", label: tCommon("filters.all") }, ...techOptions]}
            onChange={(e) => setFilterTechnician(e.target.value)}
          />
        </div>
      </div>

      {/* Schedule Table */}
      <div className="erp-card p-5 space-y-3">
        {schedulesQuery.isLoading ? (
          <MonoSpinner size="sm" />
        ) : items.length === 0 ? (
          <p className="text-xs text-erp-text-muted">{t("empty")}</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border">
                  <th className="py-2 pr-2">{t("workDate")}</th>
                  <th className="py-2 pr-2">{t("technician")}</th>
                  <th className="py-2 pr-2">{t("assignmentKind")}</th>
                  <th className="py-2 pr-2">{t("document")}</th>
                  <th className="py-2 pr-2">{t("note")}</th>
                  <th className="py-2 pr-2">{tCommon("fields.status")}</th>
                  {canManage && <th className="py-2 text-right">{tCommon("fields.actions")}</th>}
                </tr>
              </thead>
              <tbody>
                {items.map((row) => (
                  <tr key={row.id} className="border-b border-erp-border/60 align-middle">
                    <td className="py-2 pr-2 font-mono">{row.workDate ? formatDate(row.workDate, locale) : "-"}</td>
                    <td className="py-2 pr-2 font-medium">{row.technician?.displayName ?? "-"}</td>
                    <td className="py-2 pr-2">
                      {row.assignmentKind === "installation" ? t("kindInstallation") : t("kindService")}
                    </td>
                    <td className="py-2 pr-2 font-mono">
                      {row.installationJobNumber ?? row.serviceRequestNumber ?? "-"}
                    </td>
                    <td className="py-2 pr-2 max-w-xs truncate">{row.note ?? "-"}</td>
                    <td className="py-2 pr-2">
                      {row.hasConflict ? (
                        <StatusBadge label={t("hasConflict")} variant="danger" />
                      ) : (
                        <StatusBadge label={t("noConflict")} variant="success" />
                      )}
                    </td>
                    {canManage && (
                      <td className="py-2 text-right">
                        <Button
                          type="button"
                          size="sm"
                          variant="ghost"
                          className="min-h-11 text-erp-danger hover:bg-erp-danger/10"
                          disabled={isBusy}
                          onClick={() => setDeletingAssignment(row)}
                        >
                          {t("delete")}
                        </Button>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Assignment Form */}
      {canManage && (
        <form
          onSubmit={(e) => void handleCreate(e)}
          className="erp-card p-5 space-y-4 border border-erp-border bg-erp-surface-subtle"
        >
          <h4 className="text-sm font-bold text-erp-navy">{t("create")}</h4>
          {formError && (
            <Alert variant="danger" onClose={() => setFormError(null)}>
              {formError}
            </Alert>
          )}

          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-3">
            <Select
              id="schedule-create-tech"
              label={t("technician")}
              required
              value={selectedTech}
              placeholder={t("selectTechnician")}
              disabled={isBusy}
              className="h-11 min-h-[44px]"
              options={techOptions}
              onChange={(e) => setSelectedTech(e.target.value)}
            />
            <Input
              id="schedule-create-date"
              type="date"
              label={t("workDate")}
              placeholder={t("datePlaceholder")}
              required
              value={workDate}
              disabled={isBusy}
              className="h-11 min-h-[44px]"
              onChange={(e) => setWorkDate(e.target.value)}
            />
            <Select
              id="schedule-create-kind"
              label={t("assignmentKind")}
              required
              value={assignmentKind}
              placeholder={t("assignmentKind")}
              disabled={isBusy}
              className="h-11 min-h-[44px]"
              options={[
                { value: "service_request", label: t("kindService") },
                { value: "installation", label: t("kindInstallation") },
              ]}
              onChange={(e) => setAssignmentKind(e.target.value)}
            />
            <Input
              id="schedule-create-service-id"
              label={t("serviceRequestId")}
              placeholder={t("serviceRequestIdPlaceholder")}
              value={serviceReqId}
              disabled={isBusy}
              className="h-11 min-h-[44px]"
              onChange={(e) => setServiceReqId(e.target.value)}
            />
            <Input
              id="schedule-create-installation-id"
              label={t("installationJobId")}
              placeholder={t("installationJobIdPlaceholder")}
              value={installationJobId}
              disabled={isBusy}
              className="h-11 min-h-[44px]"
              onChange={(e) => setInstallationJobId(e.target.value)}
            />
            <Input
              id="schedule-create-note"
              label={t("note")}
              placeholder={t("notePlaceholder")}
              value={note}
              disabled={isBusy}
              className="h-11 min-h-[44px]"
              onChange={(e) => setNote(e.target.value)}
            />
          </div>

          <div className="flex flex-wrap items-center justify-between gap-3 pt-2">
            <label className="flex items-center gap-2 text-xs text-erp-text-body cursor-pointer">
              <input
                id="schedule-create-allow-conflict"
                type="checkbox"
                checked={allowConflict}
                disabled={isBusy}
                className="h-4 w-4 rounded-none border-erp-border text-erp-navy focus:ring-erp-navy"
                onChange={(e) => setAllowConflict(e.target.checked)}
              />
              <span>{t("allowConflict")}</span>
            </label>

            <Button
              type="submit"
              variant="primary"
              className="min-h-11"
              isLoading={mutations.create.isPending}
              disabled={isBusy}
            >
              {t("create")}
            </Button>
          </div>
        </form>
      )}

      {/* Delete confirmation modal */}
      <ConfirmationModal
        isOpen={deletingAssignment !== null}
        onClose={() => setDeletingAssignment(null)}
        onConfirm={() => void handleDeleteConfirm()}
        title={t("deleteTitle")}
        message={t("deleteDescription")}
        confirmText={t("delete")}
        variant="danger"
        isLoading={mutations.remove.isPending}
      />
    </div>
  );
}
