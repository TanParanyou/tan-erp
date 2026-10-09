"use client";

import React from "react";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Button } from "@/components/ui/Button";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { TableAction, TableActionGroup } from "@/components/ui/TableAction";
import { IconCheckCircle, IconEye } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize, type ListUrlSchema } from "@/hooks/useListState";
import {
  useMarkAllNotificationsRead,
  useMarkNotificationRead,
  useNotificationList,
} from "@/hooks/useNotifications";
import type { NotificationResponse } from "@/lib/api/api-client";
import { formatDateTime } from "@/lib/formatters/formatters";
import { localizedNotificationHref, useNotificationText } from "@/lib/notifications/notification-view";

/** The API rejects pageSize above this; useListState also offers 100, so the request is clamped (never guessed). */
const MAX_API_PAGE_SIZE = 50;

interface NotificationFilters extends ListFilterRecord {
  status?: string;
}

const NOTIFICATION_LIST_SCHEMA: ListUrlSchema<NotificationFilters> = { single: ["status"] };

/** A row is unread while the API reports no read time (the generated contract marks the field optional). */
const isUnread = (notification: NotificationResponse): boolean => !notification.readAtUtc;

export function NotificationListPage() {
  const t = useTranslations("notifications");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const textOf = useNotificationText();

  const listState = useListState<NotificationFilters>({ schema: NOTIFICATION_LIST_SCHEMA });
  const pageSize = Math.min(listState.params.limit, MAX_API_PAGE_SIZE);
  const { data, isLoading, isError, refetch } = useNotificationList({
    unreadOnly: listState.params.filters.status === "unread",
    page: listState.params.page,
    pageSize,
  });
  const markRead = useMarkNotificationRead();
  const markAll = useMarkAllNotificationsRead();

  const items = data?.items ?? [];
  const totalItems = data?.pagination?.totalCount ?? 0;
  const totalPages = data?.pagination?.totalPages ?? 0;
  const hasUnread = items.some(isUnread);

  const columns: Column<NotificationResponse>[] = [
    {
      id: "message",
      header: t("list.columnMessage"),
      className: "min-w-[280px]",
      cell: (_value, n) => (
        <span className={isUnread(n) ? "text-xs font-semibold text-erp-text-main" : "text-xs text-erp-text-main"}>
          {textOf(n)}
        </span>
      ),
    },
    {
      id: "receivedAt",
      header: t("list.columnReceivedAt"),
      className: "min-w-[160px]",
      cell: (_value, n) => <span className="font-mono text-[11px] text-erp-text-muted">{formatDateTime(n.createdAtUtc, locale)}</span>,
    },
    {
      id: "status",
      header: t("list.columnStatus"),
      className: "min-w-[110px]",
      cell: (_value, n) => (
        <StatusBadge
          label={isUnread(n) ? t("list.statusUnread") : t("list.statusRead")}
          variant={isUnread(n) ? "info" : "neutral"}
        />
      ),
    },
    {
      id: "actions",
      header: tCommon("fields.actions"),
      className: "w-[100px]",
      sticky: "right",
      isAction: true,
      cell: (_value, n) => {
        const href = localizedNotificationHref(locale, n.deepLink ?? null);
        const notificationId = n.id;
        return (
          <TableActionGroup>
            {isUnread(n) && notificationId !== undefined && (
              <TableAction
                icon={<IconCheckCircle size={14} />}
                label={t("list.markRead")}
                onClick={() => markRead.mutate(notificationId)}
              />
            )}
            {href !== null && <TableAction icon={<IconEye size={14} />} label={t("list.open")} href={href} variant="primary" />}
          </TableActionGroup>
        );
      },
    },
  ];

  return (
    <div className="flex flex-col gap-5">
      <PageHeader
        title={t("list.title")}
        subtitle={t("list.subtitle")}
        actions={
          <Button
            type="button"
            variant="outline"
            size="md"
            disabled={markAll.isPending || !hasUnread}
            isLoading={markAll.isPending}
            onClick={() => markAll.mutate()}
          >
            {t("list.markAllRead")}
          </Button>
        }
      />

      <ListToolbar>
        <ListFilterSelect
          id="filter-notification-status"
          label={t("list.filterLabel")}
          value={listState.params.filters.status ?? ""}
          onChange={(value) => listState.actions.setFilter("status", value === "" ? undefined : value)}
          options={[{ value: "unread", label: t("list.filterUnread") }]}
          widthClassName="w-full sm:w-44"
        />
      </ListToolbar>

      <DataTable<NotificationResponse>
        columns={columns}
        data={items}
        isLoading={isLoading}
        isError={isError}
        error={isError ? t("loadFailed") : null}
        onRetry={() => refetch()}
        emptyTitle={t("list.empty")}
        stickyActionColumn={true}
        pagination={{ page: listState.params.page, limit: pageSize, totalPages, totalItems }}
        onPageChange={(p) => listState.actions.setPage(p)}
        onLimitChange={(limit) => listState.actions.setLimit(limit as ListPageSize)}
      />
    </div>
  );
}
