"use client";

import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { fileClient } from "@/lib/api/file-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useOptionalSelectedMembership } from "@/lib/membership/selected-membership-context";

export function useAuthenticatedFileUrl(fileId?: string | null) {
  const membershipContext = useOptionalSelectedMembership();
  const locale = useSafeLocale();
  const membershipId = membershipContext?.selectedMembership?.id;

  const query = useQuery({
    queryKey: ["business", "file-content", membershipId, fileId],
    queryFn: async () => {
      const token = await getAuthToken();
      if (!token || !membershipId || !fileId) {
        throw new Error("AUTHENTICATION_REQUIRED");
      }
      return fileClient.getFileBlob(fileId, {
        token,
        membershipId,
        locale: locale === "en" ? "en" : "th",
      });
    },
    enabled: Boolean(fileId && membershipId),
    staleTime: 5 * 60 * 1000,
  });

  const [objectUrl, setObjectUrl] = useState<string | null>(null);
  useEffect(() => {
    if (!query.data) {
      setObjectUrl(null);
      return;
    }

    const nextUrl = URL.createObjectURL(query.data);
    setObjectUrl(nextUrl);
    return () => URL.revokeObjectURL(nextUrl);
  }, [query.data]);

  return {
    objectUrl,
    isLoading: query.isLoading,
    isError: query.isError,
  };
}
