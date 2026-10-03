import { useMutation, useQuery, useQueryClient, type UseMutationResult, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type CreateProjectFromHandoverRequest,
  type AddProjectMilestoneRequest,
  type CreateProjectChangeOrderRequest,
  type ListProjectsParams,
  type ProjectBudgetLineRequest,
  type ProjectChangeOrderAction,
  type ProjectControlResponse,
  type SetProjectPlanRequest,
  type UpdateProjectMilestoneRequest,
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

export function useProjectList(params: ListProjectsParams, enabled = true): UseQueryResult<ProjectListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...projectsKey(membershipId, locale), "list", params],
    enabled: Boolean(membershipId) && enabled,
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

export function useProjectControl(projectId: string | undefined): UseQueryResult<ProjectControlResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...projectsKey(membershipId, locale), "control", projectId],
    enabled: Boolean(membershipId && projectId),
    queryFn: async ({ signal }) => apiClient.getProjectControl(projectId ?? "", await options(undefined, signal)),
  });
}

/**
 * All project-control writes. Every call returns the fresh control read model, which is written straight into
 * the cache so the page reflects the new row version before the next refetch.
 */
export function useProjectControlMutations(projectId: string) {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const controlKey = [...projectsKey(membershipId, locale), "control", projectId];
  const store = async (control: ProjectControlResponse) => {
    queryClient.setQueryData(controlKey, control);
    await queryClient.invalidateQueries({ queryKey: [...projectsKey(membershipId, locale), "detail", projectId] });
    await queryClient.invalidateQueries({ queryKey: [...projectsKey(membershipId, locale), "list"] });
  };

  const setPlan = useMutation({
    mutationFn: async (input: { rowVersion: string; payload: SetProjectPlanRequest }) =>
      apiClient.setProjectPlan(projectId, input.rowVersion, input.payload, await options()),
    onSuccess: store,
  });
  const replaceBudget = useMutation({
    mutationFn: async (input: { rowVersion: string; lines: ProjectBudgetLineRequest[] }) =>
      apiClient.replaceProjectBudget(projectId, input.rowVersion, input.lines, await options()),
    onSuccess: store,
  });
  const transition = useMutation({
    mutationFn: async (input: { rowVersion: string; targetStatus: string; reason: string | null }) =>
      apiClient.transitionProject(projectId, input.rowVersion, input.targetStatus, input.reason, await options()),
    onSuccess: store,
  });
  const addMilestone = useMutation({
    mutationFn: async (payload: AddProjectMilestoneRequest) => apiClient.addProjectMilestone(projectId, payload, await options()),
    onSuccess: store,
  });
  const updateMilestone = useMutation({
    mutationFn: async (input: { milestoneId: string; payload: UpdateProjectMilestoneRequest }) =>
      apiClient.updateProjectMilestone(projectId, input.milestoneId, input.payload, await options()),
    onSuccess: store,
  });
  const completeMilestone = useMutation({
    mutationFn: async (input: { milestoneId: string; expectedVersion: string }) =>
      apiClient.completeProjectMilestone(projectId, input.milestoneId, input.expectedVersion, await options()),
    onSuccess: store,
  });
  const deleteMilestone = useMutation({
    mutationFn: async (input: { milestoneId: string; expectedVersion: string }) =>
      apiClient.deleteProjectMilestone(projectId, input.milestoneId, input.expectedVersion, await options()),
    onSuccess: store,
  });
  const createChangeOrder = useMutation({
    mutationFn: async (input: { payload: CreateProjectChangeOrderRequest; idempotencyKey: string }) =>
      apiClient.createProjectChangeOrder(projectId, input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const changeOrderAction = useMutation({
    mutationFn: async (input: { changeOrderId: string; action: ProjectChangeOrderAction; expectedVersion: string; note: string | null }) =>
      apiClient.projectChangeOrderAction(projectId, input.changeOrderId, input.action, input.expectedVersion, input.note, await options()),
    onSuccess: store,
  });

  return { setPlan, replaceBudget, transition, addMilestone, updateMilestone, completeMilestone, deleteMilestone, createChangeOrder, changeOrderAction };
}

