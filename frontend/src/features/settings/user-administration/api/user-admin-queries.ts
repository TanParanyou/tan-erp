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
  type AdminAssignRoleResponse,
  type AdminRoleListResponse,
  type AdminRoleRequestListResponse,
  type AdminRoleRequestResponse,
  type AdminUserListParams,
  type AdminUserListResponse,
  type AdminUserResponse,
  type CreateAdminUserRequest,
  type UpdateAdminMembershipRequest,
} from "@/lib/api/api-client";
import { useApiRequestContext, type ApiLocale } from "@/lib/api/use-api-request-context";


export const userAdminKeys = {
  all: (membershipId: string | null | undefined) => ["business", membershipId] as const,
  list: (membershipId: string | null | undefined, locale: ApiLocale, params: AdminUserListParams) =>
    ["business", membershipId, locale, "user-admin", "list", params] as const,
  detail: (membershipId: string | null | undefined, locale: ApiLocale, userId: string) =>
    ["business", membershipId, locale, "user-admin", "detail", userId] as const,
  roles: (membershipId: string | null | undefined, locale: ApiLocale) =>
    ["business", membershipId, locale, "user-admin", "roles"] as const,
  requests: (membershipId: string | null | undefined, locale: ApiLocale, status: string) =>
    ["business", membershipId, locale, "user-admin", "role-requests", status] as const,
};

/** Access changes also change what /me and every permission-gated screen allow, so refresh all business queries. */
function invalidateAdmin(queryClient: QueryClient, membershipId: string | undefined): void {
  void queryClient.invalidateQueries({ queryKey: userAdminKeys.all(membershipId) });
}

export function useAdminUsers(params: AdminUserListParams): UseQueryResult<AdminUserListResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: userAdminKeys.list(context.membershipId, context.locale, params),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.listAdminUsers(params, await context.buildOptions({ signal })),
  });
}

export function useAdminUser(userId: string, enabled: boolean): UseQueryResult<AdminUserResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: userAdminKeys.detail(context.membershipId, context.locale, userId),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.getAdminUser(userId, await context.buildOptions({ signal })),
  });
}

export function useAdminRoles(enabled = true): UseQueryResult<AdminRoleListResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: userAdminKeys.roles(context.membershipId, context.locale),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listAdminRoles(await context.buildOptions({ signal })),
  });
}

export function useAdminRoleRequests(status: string, enabled = true): UseQueryResult<AdminRoleRequestListResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: userAdminKeys.requests(context.membershipId, context.locale, status),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listAdminRoleRequests(status, await context.buildOptions({ signal })),
  });
}

export function useCreateAdminUser(): UseMutationResult<
  AdminUserResponse,
  Error,
  { payload: CreateAdminUserRequest; idempotencyKey: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) =>
      apiClient.createAdminUser(payload, await context.buildOptions({ idempotencyKey })),
    onSuccess: () => invalidateAdmin(queryClient, context.membershipId),
  });
}

export function useRenameAdminUser(): UseMutationResult<AdminUserResponse, Error, { userId: string; displayName: string; ifMatch: string }> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ userId, displayName, ifMatch }) =>
      apiClient.renameAdminUser(userId, { displayName }, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateAdmin(queryClient, context.membershipId),
  });
}

export function useSetAdminUserActive(): UseMutationResult<AdminUserResponse, Error, { userId: string; active: boolean; ifMatch: string }> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ userId, active, ifMatch }) =>
      apiClient.setAdminUserActive(userId, active, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateAdmin(queryClient, context.membershipId),
  });
}

export function useUpdateAdminMembership(): UseMutationResult<
  AdminUserResponse,
  Error,
  { membershipId: string; payload: UpdateAdminMembershipRequest; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ membershipId, payload, ifMatch }) =>
      apiClient.updateAdminMembership(membershipId, payload, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateAdmin(queryClient, context.membershipId),
  });
}

export function useSetAdminMembershipActive(): UseMutationResult<
  AdminUserResponse,
  Error,
  { membershipId: string; active: boolean; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ membershipId, active, ifMatch }) =>
      apiClient.setAdminMembershipActive(membershipId, active, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateAdmin(queryClient, context.membershipId),
  });
}

export function useAssignAdminRole(): UseMutationResult<
  AdminAssignRoleResponse,
  Error,
  { membershipId: string; roleId: string; idempotencyKey: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ membershipId, roleId, idempotencyKey }) =>
      apiClient.assignAdminRole(membershipId, { roleId }, await context.buildOptions({ idempotencyKey })),
    onSuccess: () => invalidateAdmin(queryClient, context.membershipId),
  });
}

export function useRevokeAdminRole(): UseMutationResult<void, Error, { membershipId: string; roleId: string }> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ membershipId, roleId }) => apiClient.revokeAdminRole(membershipId, roleId, await context.buildOptions()),
    onSuccess: () => invalidateAdmin(queryClient, context.membershipId),
  });
}

export function useDecideAdminRoleRequest(): UseMutationResult<
  AdminRoleRequestResponse,
  Error,
  { requestId: string; decision: "approve" | "reject" | "cancel"; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ requestId, decision, ifMatch }) =>
      apiClient.decideAdminRoleRequest(requestId, decision, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateAdmin(queryClient, context.membershipId),
  });
}
