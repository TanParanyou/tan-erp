import {
  useMutation,
  useQuery,
  useQueryClient,
  type QueryClient,
  type UseMutationResult,
  type UseQueryResult,
} from "@tanstack/react-query";
import {
  apiClient,
  type AdminBranchResponse,
  type BranchDeactivationCheckResponse,
  type CreateAdminBranchRequest,
  type OrganizationProfileResponse,
  type UpdateAdminBranchRequest,
  type UpdateOrganizationProfileRequest,
} from "@/lib/api/api-client";
import { useApiRequestContext, type ApiLocale } from "@/lib/api/use-api-request-context";

export const organizationAdminKeys = {
  all: (membershipId: string | null | undefined) => ["business", membershipId] as const,
  profile: (membershipId: string | null | undefined, locale: ApiLocale) =>
    ["business", membershipId, locale, "org-admin", "profile"] as const,
  branches: (membershipId: string | null | undefined, locale: ApiLocale, status: string) =>
    ["business", membershipId, locale, "org-admin", "branches", status] as const,
  branch: (membershipId: string | null | undefined, locale: ApiLocale, branchId: string) =>
    ["business", membershipId, locale, "org-admin", "branch", branchId] as const,
  deactivationCheck: (membershipId: string | null | undefined, locale: ApiLocale, branchId: string) =>
    ["business", membershipId, locale, "org-admin", "deactivation-check", branchId] as const,
};

/** Branch changes also change every branch picker in the app, so refresh all business queries (as user-admin does). */
function invalidateOrg(queryClient: QueryClient, membershipId: string | undefined): void {
  void queryClient.invalidateQueries({ queryKey: organizationAdminKeys.all(membershipId) });
}

export function useOrganizationProfile(): UseQueryResult<OrganizationProfileResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: organizationAdminKeys.profile(context.membershipId, context.locale),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.getOrganizationProfile(await context.buildOptions({ signal })),
  });
}

export function useUpdateOrganizationProfile(): UseMutationResult<
  OrganizationProfileResponse, Error, { payload: UpdateOrganizationProfileRequest; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ payload, ifMatch }) =>
      apiClient.updateOrganizationProfile(payload, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateOrg(queryClient, context.membershipId),
  });
}

export function useAdminBranches(status: "active" | "inactive" | undefined): UseQueryResult<AdminBranchResponse[], Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: organizationAdminKeys.branches(context.membershipId, context.locale, status ?? "all"),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.listAdminBranches(status, await context.buildOptions({ signal })),
  });
}

export function useAdminBranch(branchId: string, enabled: boolean): UseQueryResult<AdminBranchResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: organizationAdminKeys.branch(context.membershipId, context.locale, branchId),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.getAdminBranch(branchId, await context.buildOptions({ signal })),
  });
}

export function useBranchDeactivationCheck(branchId: string, enabled: boolean): UseQueryResult<BranchDeactivationCheckResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: organizationAdminKeys.deactivationCheck(context.membershipId, context.locale, branchId),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.getBranchDeactivationCheck(branchId, await context.buildOptions({ signal })),
  });
}

export function useCreateAdminBranch(): UseMutationResult<
  AdminBranchResponse, Error, { payload: CreateAdminBranchRequest; idempotencyKey: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) =>
      apiClient.createAdminBranch(payload, await context.buildOptions({ idempotencyKey })),
    onSuccess: () => invalidateOrg(queryClient, context.membershipId),
  });
}

export function useUpdateAdminBranch(): UseMutationResult<
  AdminBranchResponse, Error, { branchId: string; payload: UpdateAdminBranchRequest; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ branchId, payload, ifMatch }) =>
      apiClient.updateAdminBranch(branchId, payload, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateOrg(queryClient, context.membershipId),
  });
}

export function useSetAdminBranchActive(): UseMutationResult<
  AdminBranchResponse, Error, { branchId: string; active: boolean; reason?: string; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ branchId, active, reason, ifMatch }) => {
      const options = await context.buildOptions({ ifMatch });
      return active
        ? apiClient.activateAdminBranch(branchId, options)
        : apiClient.deactivateAdminBranch(branchId, { reason }, options);
    },
    onSuccess: () => invalidateOrg(queryClient, context.membershipId),
  });
}
