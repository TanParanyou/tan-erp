"use client";

import { useAuthenticatedFileUrl } from "@/hooks/useAuthenticatedFileUrl";

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
  const { objectUrl, isLoading, isError } = useAuthenticatedFileUrl(enabled ? fileId : undefined);
  return { imageUrl: objectUrl, isLoading, isError };
}
