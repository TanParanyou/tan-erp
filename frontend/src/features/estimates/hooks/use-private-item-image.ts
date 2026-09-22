"use client";

import { useState, useEffect } from "react";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useOptionalSelectedMembership } from "@/lib/membership/selected-membership-context";

export interface UsePrivateItemImageOptions {
  fileId?: string | null;
  enabled?: boolean;
}

export interface UsePrivateItemImageResult {
  imageUrl: string | null;
  isLoading: boolean;
  isError: boolean;
}

export function usePrivateItemImage({
  fileId,
  enabled = true,
}: UsePrivateItemImageOptions): UsePrivateItemImageResult {
  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isError, setIsError] = useState(false);

  const membershipContext = useOptionalSelectedMembership();
  const membershipId = membershipContext?.selectedMembership?.id;

  useEffect(() => {
    if (!enabled || !fileId || !membershipId) {
      setImageUrl(null);
      setIsLoading(false);
      setIsError(false);
      return;
    }

    let isMounted = true;
    let createdUrl: string | null = null;
    const abortController = new AbortController();

    async function fetchImage() {
      setIsLoading(true);
      setIsError(false);

      try {
        const token = await getAuthToken();
        if (!token) {
          throw new Error("AUTHENTICATION_REQUIRED");
        }

        const response = await fetch(`/api/v1/files/${fileId}/content`, {
          headers: {
            Authorization: `Bearer ${token}`,
            "X-Membership-Id": membershipId!,
          },
          signal: abortController.signal,
        });

        if (!response.ok) {
          throw new Error(`Failed to load image: ${response.status}`);
        }

        const blob = await response.blob();
        if (!isMounted) return;

        createdUrl = URL.createObjectURL(blob);
        setImageUrl(createdUrl);
      } catch (err: unknown) {
        if (!isMounted) return;
        if (err instanceof Error && err.name === "AbortError") return;
        setIsError(true);
        setImageUrl(null);
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    }

    void fetchImage();

    return () => {
      isMounted = false;
      abortController.abort();
      if (createdUrl) {
        URL.revokeObjectURL(createdUrl);
      }
    };
  }, [fileId, membershipId, enabled]);

  return { imageUrl, isLoading, isError };
}
