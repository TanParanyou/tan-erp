import { useQuery, useMutation, useQueryClient, type UseQueryResult, type UseMutationResult } from "@tanstack/react-query";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import type {
  DocumentTemplateItem,
  CreateDocumentTemplatePayload,
  UpdateDocumentTemplatePayload
} from "../types";

export function documentTemplatesQueryKey(
  membershipId: string | null | undefined,
  locale: "th" | "en",
  documentType?: string,
  branchId?: string
) {
  return ["business", membershipId, locale, "settings", "document-templates", documentType ?? "all", branchId ?? "all"] as const;
}

export function useDocumentTemplates(
  documentType?: string,
  branchId?: string
): UseQueryResult<DocumentTemplateItem[], Error> {
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useQuery({
    queryKey: documentTemplatesQueryKey(membershipId, normalizedLocale, documentType, branchId),
    queryFn: async ({ signal }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.listDocumentTemplates(
        { documentType, branchId },
        {
          token,
          membershipId,
          locale: normalizedLocale,
          signal,
        }
      );
    },
    enabled: Boolean(membershipId),
  });
}

export function useCreateDocumentTemplate(): UseMutationResult<
  DocumentTemplateItem,
  Error,
  CreateDocumentTemplatePayload
> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async (payload) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.createDocumentTemplate(payload, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["business", membershipId, normalizedLocale, "settings", "document-templates"],
      });
    },
  });
}

export function useUpdateDocumentTemplate(): UseMutationResult<
  DocumentTemplateItem,
  Error,
  { id: string; payload: UpdateDocumentTemplatePayload; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async ({ id, payload, ifMatch }) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.updateDocumentTemplate(id, payload, ifMatch, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["business", membershipId, normalizedLocale, "settings", "document-templates"],
      });
    },
  });
}

export function useDeleteDocumentTemplate(): UseMutationResult<
  void,
  Error,
  string
> {
  const queryClient = useQueryClient();
  const locale = useSafeLocale();
  const normalizedLocale = locale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return useMutation({
    mutationFn: async (id) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();

      return apiClient.deleteDocumentTemplate(id, {
        token,
        membershipId,
        locale: normalizedLocale,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["business", membershipId, normalizedLocale, "settings", "document-templates"],
      });
    },
  });
}
