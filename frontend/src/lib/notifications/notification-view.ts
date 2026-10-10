import { useCallback } from "react";
import { useTranslations } from "next-intl";
import type { NotificationResponse } from "@/lib/api/api-client";
import { isNotificationType, notificationMessageKey, notificationMessageValues } from "./notification-types";

/** Returns a function that renders a notification as one display line; an unregistered type gets a neutral line. */
export function useNotificationText(): (notification: NotificationResponse) => string {
  const t = useTranslations("notifications");
  return useCallback(
    (notification) => {
      // The generated contract marks type and payload optional; a missing type is unregistered and a missing payload has no fields ("-").
      const type = notification.type ?? "";
      const messageKey = notificationMessageKey(type);
      if (messageKey === null || !isNotificationType(type)) {
        return t("unknownType");
      }
      return t(`types.${messageKey}`, notificationMessageValues(type, notification.payload ?? {}));
    },
    [t],
  );
}

/**
 * The API returns locale-less paths; accept only an absolute in-app path so a link can never leave the app.
 * Backslashes are rejected too, because browsers treat "/\\host" like the protocol-relative "//host".
 */
export function localizedNotificationHref(locale: string, deepLink: string | null): string | null {
  if (deepLink === null || !deepLink.startsWith("/") || deepLink.startsWith("//") || deepLink.includes("\\")) {
    return null;
  }
  return `/${locale}${deepLink}`;
}
