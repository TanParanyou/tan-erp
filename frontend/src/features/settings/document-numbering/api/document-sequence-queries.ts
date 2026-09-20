import { useQuery, useMutation, useQueryClient, type UseQueryResult, type UseMutationResult } from "@tanstack/react-query";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import type { DocumentSequenceItem, UpdateDocumentSequencePayload, PreviewDocumentSequencePayload } from "../types";

export function documentSequencesQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en"
): readonly ["business", string | null | undefined, "th" | "en", "settings", "document-sequences"] {
  return ["business", membershipId, locale, "settings", "document-sequences"] as const;
}

export function useDocumentSequences(): UseQueryResult<DocumentSequenceItem[], Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: documentSequencesQueryKey(membershipId, normalizedLocale),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.listDocumentSequences({
        token,
        membershipId,
        locale: normalizedLocale,
        signal,
      });
    },
    enabled: Boolean(membershipId),
  });
}

export function useUpdateDocumentSequence(): UseMutationResult<
  DocumentSequenceItem,
  Error,
  { documentType: string; payload: UpdateDocumentSequencePayload }
> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async ({ documentType, payload }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.updateDocumentSequence(documentType, payload, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: documentSequencesQueryKey(membershipId, normalizedLocale),
      });
    },
  });
}

export function usePreviewDocumentSequence(): UseMutationResult<
  { preview: string },
  Error,
  PreviewDocumentSequencePayload
> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async (payload) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.previewDocumentSequence(payload, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
  });
}
