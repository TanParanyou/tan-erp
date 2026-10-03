import { useMutation, useQuery, useQueryClient, type UseMutationResult, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type CreateProjectFromHandoverRequest,
  type ListProjectsParams,
  type ProjectHandoverSourceResponse,
  type ProjectListResponse,
  type ProjectResponse,
} from "@/lib/api/api-client";
import { ApiError, AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { opportunityDetailQueryKey } from "@/features/opportunities/api/opportunity-queries";

type UiLocale = "th" | "en";

export function projectsKey(membershipId: string | null | undefined, locale: UiLocale) {
  return ["business", membershipId, locale, "projects"] as const;
}

function useRequestContext() {
  const locale: UiLocale = useSafeLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;
  const options = async (idempotencyKey?: string, signal?: AbortSignal) => {
    const token = await getAuthToken();
    if (!token) throw new AuthenticationRequiredError();
    if (!membershipId) throw new MembershipRequiredError();
    return { token, membershipId, locale, idempotencyKey, signal };
  };
  return { locale, membershipId, options };
}

export function useProjectList(params: ListProjectsParams): UseQueryResult<ProjectListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...projectsKey(membershipId, locale), "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listProjects(await options(undefined, signal), params),
  });
}

export function useProject(id: string | undefined): UseQueryResult<ProjectResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...projectsKey(membershipId, locale), "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getProject(id ?? "", await options(undefined, signal)),
  });
}

/** The accepted quotation (and existing project) of a Won opportunity; resolves to null when none is accepted yet. */
export function useProjectHandoverSource(
  opportunityId: string | undefined,
  enabled: boolean
): UseQueryResult<ProjectHandoverSourceResponse | null, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...projectsKey(membershipId, locale), "handover-source", opportunityId],
    enabled: Boolean(membershipId && opportunityId && enabled),
    queryFn: async ({ signal }) => {
      try {
        return await apiClient.getProjectHandoverSource(opportunityId ?? "", await options(undefined, signal));
      } catch (error: unknown) {
        if (error instanceof ApiError && error.status === 404) return null;
        throw error;
      }
    },
  });
}

export function useCreateProjectFromHandover(
  opportunityId: string
): UseMutationResult<ProjectResponse, Error, { payload: CreateProjectFromHandoverRequest; idempotencyKey: string }> {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) => apiClient.createProjectFromHandover(payload, await options(idempotencyKey)),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: projectsKey(membershipId, locale) });
      await queryClient.invalidateQueries({ queryKey: opportunityDetailQueryKey(membershipId, locale, opportunityId) });
    },
  });
}
