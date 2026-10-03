import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type InstallationRequest,
  type InstallationResponse,
  type InstallationStep,
  type InstallationsListResponse,
  type ListInstallationsParams,
  type ListServiceRequestsParams,
  type ListWarrantiesParams,
  type ServiceRequestRequest,
  type ServiceRequestResponse,
  type ServiceRequestStep,
  type ServiceRequestsListResponse,
  type WarrantiesListResponse,
} from "@/lib/api/api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "@/lib/api/api-error";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";

type UiLocale = "th" | "en";

export function serviceKey(membershipId: string | null | undefined, locale: UiLocale) {
  return ["business", membershipId, locale, "service"] as const;
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

export function useInstallationList(params: ListInstallationsParams): UseQueryResult<InstallationsListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...serviceKey(membershipId, locale), "installations", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listInstallations(await options(undefined, signal), params),
  });
}

export function useInstallation(id: string | undefined): UseQueryResult<InstallationResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...serviceKey(membershipId, locale), "installations", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getInstallation(id ?? "", await options(undefined, signal)),
  });
}

/** Installation writes. Each call returns the fresh job, which is cached so the page shows the new row version immediately. */
export function useInstallationMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (job: InstallationResponse) => {
    queryClient.setQueryData([...serviceKey(membershipId, locale), "installations", "detail", job.id], job);
    await queryClient.invalidateQueries({ queryKey: [...serviceKey(membershipId, locale), "installations", "list"] });
    // A handover creates a warranty.
    await queryClient.invalidateQueries({ queryKey: [...serviceKey(membershipId, locale), "warranties"] });
    await queryClient.invalidateQueries({ queryKey: ["business", membershipId, locale, "projects"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: InstallationRequest; idempotencyKey: string }) =>
      apiClient.createInstallation(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const step = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; step: InstallationStep }) =>
      apiClient.installationStep(input.id, input.rowVersion, input.step, await options()),
    onSuccess: store,
  });
  return { create, step };
}

export function useWarrantyList(params: ListWarrantiesParams): UseQueryResult<WarrantiesListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...serviceKey(membershipId, locale), "warranties", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listWarranties(await options(undefined, signal), params),
  });
}

export function useServiceRequestList(params: ListServiceRequestsParams): UseQueryResult<ServiceRequestsListResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...serviceKey(membershipId, locale), "requests", "list", params],
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listServiceRequests(await options(undefined, signal), params),
  });
}

export function useServiceRequest(id: string | undefined): UseQueryResult<ServiceRequestResponse, Error> {
  const { locale, membershipId, options } = useRequestContext();
  return useQuery({
    queryKey: [...serviceKey(membershipId, locale), "requests", "detail", id],
    enabled: Boolean(membershipId && id),
    queryFn: async ({ signal }) => apiClient.getServiceRequest(id ?? "", await options(undefined, signal)),
  });
}

export function useServiceRequestMutations() {
  const { locale, membershipId, options } = useRequestContext();
  const queryClient = useQueryClient();
  const store = async (request: ServiceRequestResponse) => {
    queryClient.setQueryData([...serviceKey(membershipId, locale), "requests", "detail", request.id], request);
    await queryClient.invalidateQueries({ queryKey: [...serviceKey(membershipId, locale), "requests", "list"] });
    await queryClient.invalidateQueries({ queryKey: [...serviceKey(membershipId, locale), "warranties"] });
  };
  const create = useMutation({
    mutationFn: async (input: { payload: ServiceRequestRequest; idempotencyKey: string }) =>
      apiClient.createServiceRequest(input.payload, await options(input.idempotencyKey)),
    onSuccess: store,
  });
  const step = useMutation({
    mutationFn: async (input: { id: string; rowVersion: string; step: ServiceRequestStep }) =>
      apiClient.serviceRequestStep(input.id, input.rowVersion, input.step, await options()),
    onSuccess: store,
  });
  return { create, step };
}
