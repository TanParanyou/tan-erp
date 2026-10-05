import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type AttachFilesRequest,
  type AttachmentListResponse,
  type CaptureSignatureRequest,
  type SignatureCaptureListResponse,
  type SignatureCaptureResponse,
} from "@/lib/api/api-client";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import type { AttachmentOwnerType } from "@/lib/attachments/attachment-owner-types";

type UiLocale = "th" | "en";

export function attachmentsKey(membershipId: string | undefined, locale: UiLocale, ownerType: AttachmentOwnerType, ownerId: string) {
  return ["business", membershipId, locale, "attachments", ownerType, ownerId] as const;
}

export function useAttachmentLinks(ownerType: AttachmentOwnerType, ownerId: string): UseQueryResult<AttachmentListResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: [...attachmentsKey(membershipId, locale, ownerType, ownerId), "links"],
    enabled: Boolean(membershipId && ownerId),
    queryFn: async ({ signal }) => apiClient.listAttachments(ownerType, ownerId, await buildOptions({ signal })),
  });
}

/** Attach (idempotent) and unlink. Both refresh the owner's attachment list. */
export function useAttachmentMutations(ownerType: AttachmentOwnerType, ownerId: string) {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  const refresh = async () => {
    await queryClient.invalidateQueries({ queryKey: attachmentsKey(membershipId, locale, ownerType, ownerId) });
  };

  const attach = useMutation({
    mutationFn: async (input: { payload: AttachFilesRequest; idempotencyKey: string }) =>
      apiClient.attachFiles(ownerType, ownerId, input.payload, await buildOptions({ idempotencyKey: input.idempotencyKey })),
    onSuccess: refresh,
  });

  const unlink = useMutation({
    mutationFn: async (linkId: string) => apiClient.unlinkAttachment(ownerType, ownerId, linkId, await buildOptions()),
    onSuccess: refresh,
  });

  return { attach, unlink };
}

export function useSignatureCaptures(ownerType: AttachmentOwnerType, ownerId: string): UseQueryResult<SignatureCaptureListResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: [...attachmentsKey(membershipId, locale, ownerType, ownerId), "signatures"],
    enabled: Boolean(membershipId && ownerId),
    queryFn: async ({ signal }) => apiClient.listSignatures(ownerType, ownerId, await buildOptions({ signal })),
  });
}

export function useCaptureSignature(ownerType: AttachmentOwnerType, ownerId: string) {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  return useMutation<SignatureCaptureResponse, Error, { payload: CaptureSignatureRequest; idempotencyKey: string }>({
    mutationFn: async (input) => apiClient.captureSignature(ownerType, ownerId, input.payload, await buildOptions({ idempotencyKey: input.idempotencyKey })),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: attachmentsKey(membershipId, locale, ownerType, ownerId) });
    },
  });
}
