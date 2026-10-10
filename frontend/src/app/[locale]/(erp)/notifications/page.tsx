"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { NotificationListPage } from "@/features/notifications/components/notification-list-page";

interface NotificationsPageProps {
  params: Promise<{ locale: string }>;
}

export default function NotificationsPage({ params }: NotificationsPageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  // No PermissionGuard: notifications have no permission key; the backend enforces membership and own-only access.
  return <NotificationListPage />;
}
