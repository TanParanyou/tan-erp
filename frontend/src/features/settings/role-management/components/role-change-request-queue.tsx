"use client";

import React, { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { formatDateTime } from "@/lib/formatters/formatters";
import type { RoleChangeRequestResponse } from "@/lib/api/api-client";
import { useDecideRoleChangeRequest, useRoleChangeRequests } from "../api/role-management-queries";
import { roleAdminErrorKey } from "../role-management-errors";

interface PendingDecision {
  request: RoleChangeRequestResponse;
  decision: "approve" | "reject";
}

function KeyList({ label, keys }: { label: string; keys: ReadonlyArray<string> | null | undefined }) {
  if (!keys || keys.length === 0) return null;
  return (
    <div className="text-xs">
      <span className="font-medium">{label}: </span>
      {keys.map((key) => (
        <code key={key} className="mr-2">{key}</code>
      ))}
    </div>
  );
}

export function RoleChangeRequestQueue() {
  const t = useTranslations("roleAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const { data, isLoading, isError, refetch } = useRoleChangeRequests("pending");
  const decideMutation = useDecideRoleChangeRequest();
  const [pending, setPending] = useState<PendingDecision | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const confirm = async () => {
    if (!pending?.request.id || !pending.request.rowVersion) return;
    setErrorMessage(null);
    try {
      await decideMutation.mutateAsync({ requestId: pending.request.id, decision: pending.decision, ifMatch: pending.request.rowVersion });
      toast.success(t(pending.decision === "approve" ? "queue.approved" : "queue.rejected"));
    } catch (error: unknown) {
      setErrorMessage(t(`errors.${roleAdminErrorKey(error)}`));
    } finally {
      setPending(null);
    }
  };

  const columns = useMemo<Column<RoleChangeRequestResponse>[]>(
    () => [
      { id: "role", header: t("queue.role"), className: "min-w-[180px]", cell: (_v, r) => <span className="font-medium text-erp-text-main">{r.role?.name ?? "-"}</span> },
      { id: "requestedBy", header: t("queue.requestedBy"), className: "min-w-[140px]", cell: (_v, r) => <span>{r.requestedBy?.name ?? "-"}</span> },
      { id: "requestedAt", header: t("queue.requestedAt"), className: "min-w-[160px]", cell: (_v, r) => <span>{formatDateTime(r.requestedAtUtc, locale)}</span> },
      {
        id: "changes",
        header: t("queue.changes"),
        className: "min-w-[260px]",
        cell: (_v, r) => (
          <div className="space-y-1">
            <KeyList label={t("queue.added")} keys={r.addedPermissionKeys} />
            <KeyList label={t("queue.removed")} keys={r.removedPermissionKeys} />
          </div>
        ),
      },
      {
        id: "actions",
        header: tCommon("actions.manage"),
        isAction: true,
        className: "min-w-[200px] text-right",
        cell: (_v, r) => (
          <div className="flex justify-end gap-2">
            <Button size="sm" variant="primary" disabled={decideMutation.isPending} onClick={() => setPending({ request: r, decision: "approve" })}>
              {t("queue.approve")}
            </Button>
            <Button size="sm" variant="outline" disabled={decideMutation.isPending} onClick={() => setPending({ request: r, decision: "reject" })}>
              {t("queue.reject")}
            </Button>
          </div>
        ),
      },
    ],
    [decideMutation.isPending, locale, t, tCommon]
  );

  const roleName = pending?.request.role?.name ?? "-";
  return (
    <div className="space-y-6">
      <PageHeader title={t("queue.title")} subtitle={t("queue.subtitle")} />
      {errorMessage ? <Alert variant="danger">{errorMessage}</Alert> : null}
      <DataTable<RoleChangeRequestResponse>
        columns={columns}
        data={data?.items ?? []}
        isLoading={isLoading}
        isError={isError}
        error={isError ? t("errors.GENERIC") : null}
        onRetry={() => refetch()}
        emptyTitle={t("queue.emptyTitle")}
        emptyDescription={t("queue.emptyDescription")}
        hidePagination={true}
        stickyActionColumn={true}
      />
      <ConfirmationModal
        isOpen={pending !== null}
        onClose={() => setPending(null)}
        onConfirm={confirm}
        title={pending?.decision === "approve" ? t("queue.confirmApproveTitle") : t("queue.confirmRejectTitle")}
        message={pending?.decision === "approve" ? t("queue.confirmApproveMessage", { role: roleName }) : t("queue.confirmRejectMessage", { role: roleName })}
        confirmText={tCommon("actions.confirm")}
        cancelText={tCommon("actions.cancel")}
        variant={pending?.decision === "approve" ? "info" : "danger"}
        isLoading={decideMutation.isPending}
      />
    </div>
  );
}
