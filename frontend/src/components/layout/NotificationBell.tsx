"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale } from "next-intl";
import {
  useMarkAllNotificationsRead,
  useMarkNotificationRead,
  useNotificationList,
  useUnreadNotificationCount,
} from "@/hooks/useNotifications";
import { formatDateTime } from "@/lib/formatters/formatters";
import { localizedNotificationHref, useNotificationText } from "@/lib/notifications/notification-view";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { NotificationCenter, type NotificationItem } from "./NotificationCenter";

const DROPDOWN_PAGE_SIZE = 10;

/** Header bell: only mounted (and only polling) while a membership is selected. */
export function NotificationBell() {
  const { selectedMembership } = useSelectedMembership();
  if (!selectedMembership) return null;
  return <NotificationBellContent />;
}

function NotificationBellContent() {
  const router = useRouter();
  const locale = useLocale();
  const textOf = useNotificationText();
  const [isOpen, setIsOpen] = useState(false);

  const unread = useUnreadNotificationCount();
  const list = useNotificationList({ unreadOnly: false, page: 1, pageSize: DROPDOWN_PAGE_SIZE }, isOpen);
  const markRead = useMarkNotificationRead();
  const markAll = useMarkAllNotificationsRead();

  // Rows without an id cannot be marked read, so they are skipped instead of guessed at.
  const items: NotificationItem[] = (list.data?.items ?? []).flatMap((n) =>
    n.id === undefined
      ? []
      : [
          {
            id: n.id,
            title: textOf(n),
            timeText: formatDateTime(n.createdAtUtc, locale),
            isRead: Boolean(n.readAtUtc),
            href: localizedNotificationHref(locale, n.deepLink ?? null),
          },
        ],
  );

  const handleItemClick = async (item: NotificationItem) => {
    try {
      if (item.isRead !== true) {
        await markRead.mutateAsync(item.id);
      }
    } catch {
      // Marking read is best-effort: the item stays unread and the next poll shows it; opening the document must not depend on it.
    }
    if (item.href) {
      router.push(item.href);
    }
  };

  return (
    <NotificationCenter
      notifications={items}
      unreadCount={unread.data?.unreadCount ?? 0}
      isLoading={isOpen && list.isLoading}
      isError={isOpen && list.isError}
      onOpenChange={setIsOpen}
      onItemClick={handleItemClick}
      onMarkAllRead={() => markAll.mutate()}
      onViewAll={() => router.push(`/${locale}/notifications`)}
    />
  );
}
