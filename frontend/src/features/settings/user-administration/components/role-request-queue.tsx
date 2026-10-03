"use client";

import React, { useMemo, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { useToast } from "@/hooks/useToast";
import { formatDateTime } from "@/lib/formatters/formatters";
import type { AdminRoleRequestResponse } from "@/lib/api/api-client";
import { useAdminRoleRequests, useDecideAdminRoleRequest } from "../api/user-admin-queries";
import { adminErrorKey } from "../user-admin-errors";

interface PendingDecision {
  request: AdminRoleRequestResponse;
  decision: "approve" | "reject";
}

export function RoleRequestQueue() {
  const t = useTranslations("userAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();

  const { data, isLoading, isError, refetch } = useAdminRoleRequests("pending");
  const decideMutation = useDecideAdminRoleRequest();
  const [pending, setPending] = useState<PendingDecision | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const requests = data?.items ?? [];

  const confirm = async () => {
    if (!pending?.request.id || !pending.request.rowVersion) return;
    setErrorMessage(null);
    try {
      await decideMutation.mutateAsync({
        requestId: pending.request.id,
        decision: pending.decision,
        ifMatch: pending.request.rowVersion,
      });
      toast.success(t(pending.decision === "approve" ? "queue.approved" : "queue.rejected"));
    } catch (error: unknown) {
      setErrorMessage(t(`errors.${adminErrorKey(error)}`));
    } finally {
      setPending(null);
    }
  };

  const columns = useMemo<Column<AdminRoleRequestResponse>[]>(
    () => [
      {
        id: "role",
        header: t("queue.role"),
        className: "min-w-[180px]",
        cell: (_value, request) => <span className="font-medium text-erp-text-main">{request.role?.name ?? "-"}</span>,
      },
      {
        id: "member",
        header: t("queue.member"),
        className: "min-w-[180px]",
        cell: (_value, request) => <span>{request.membership?.name ?? "-"}</span>,
      },
      {
        id: "requestedBy",
        header: t("queue.requestedBy"),
        className: "min-w-[160px]",
        cell: (_value, request) => <span>{request.requestedBy?.name ?? "-"}</span>,
      },
      {
        id: "requestedAt",
        header: t("queue.requestedAt"),
        className: "min-w-[160px]",
        cell: (_value, request) => <span>{formatDateTime(request.requestedAtUtc, locale)}</span>,
      },
      {
        id: "actions",
        header: tCommon("actions.manage"),
        isAction: true,
        className: "min-w-[200px] text-right",
        cell: (_value, request) => (
          <div className="flex justify-end gap-2">
            <Button size="sm" variant="primary" disabled={decideMutation.isPending} onClick={() => setPending({ request, decision: "approve" })}>
              {t("queue.approve")}
            </Button>
            <Button size="sm" variant="outline" disabled={decideMutation.isPending} onClick={() => setPending({ request, decision: "reject" })}>
              {t("queue.reject")}
            </Button>
          </div>
        ),
      },
    ],
    [decideMutation.isPending, locale, t, tCommon]
  );

  return (
    <div className="space-y-6">
      <PageHeader title={t("queue.title")} subtitle={t("queue.subtitle")} />
      {errorMessage ? <Alert variant="danger">{errorMessage}</Alert> : null}
      <DataTable<AdminRoleRequestResponse>
        columns={columns}
        data={requests}
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
        message={
          pending?.decision === "approve"
            ? t("queue.confirmApproveMessage", { role: pending.request.role?.name ?? "-", member: pending.request.membership?.name ?? "-" })
            : t("queue.confirmRejectMessage", { role: pending?.request.role?.name ?? "-", member: pending?.request.membership?.name ?? "-" })
        }
        confirmText={tCommon("actions.confirm")}
        cancelText={tCommon("actions.cancel")}
        variant={pending?.decision === "approve" ? "info" : "danger"}
        isLoading={decideMutation.isPending}
      />
    </div>
  );
}
