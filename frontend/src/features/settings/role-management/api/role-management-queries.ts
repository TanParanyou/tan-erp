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
  type PermissionCatalogResponse,
  type RoleChangeRequestListResponse,
  type RoleChangeRequestResponse,
  type RoleDefinitionListParams,
  type RoleDefinitionListResponse,
  type RoleDefinitionResponse,
  type SaveRoleDefinitionRequest,
  type UpdateRoleDefinitionResponse,
} from "@/lib/api/api-client";
import { useApiRequestContext, type ApiLocale } from "@/lib/api/use-api-request-context";

export const roleAdminKeys = {
  all: (membershipId: string | null | undefined) => ["business", membershipId] as const,
  catalog: (membershipId: string | null | undefined, locale: ApiLocale) =>
    ["business", membershipId, locale, "role-admin", "catalog"] as const,
  list: (membershipId: string | null | undefined, locale: ApiLocale, params: RoleDefinitionListParams) =>
    ["business", membershipId, locale, "role-admin", "list", params] as const,
  detail: (membershipId: string | null | undefined, locale: ApiLocale, roleId: string) =>
    ["business", membershipId, locale, "role-admin", "detail", roleId] as const,
  requests: (membershipId: string | null | undefined, locale: ApiLocale, status: string) =>
    ["business", membershipId, locale, "role-admin", "change-requests", status] as const,
};

/** A role change alters what every permission-gated screen allows, so refresh all business queries (as user-admin does). */
function invalidateRoles(queryClient: QueryClient, membershipId: string | undefined): void {
  void queryClient.invalidateQueries({ queryKey: roleAdminKeys.all(membershipId) });
}

export function useAdminPermissionCatalog(): UseQueryResult<PermissionCatalogResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: roleAdminKeys.catalog(context.membershipId, context.locale),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.listAdminPermissions(await context.buildOptions({ signal })),
  });
}

export function useRoleDefinitions(params: RoleDefinitionListParams): UseQueryResult<RoleDefinitionListResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: roleAdminKeys.list(context.membershipId, context.locale, params),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.listRoleDefinitions(params, await context.buildOptions({ signal })),
  });
}

export function useRoleDefinition(roleId: string, enabled: boolean): UseQueryResult<RoleDefinitionResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: roleAdminKeys.detail(context.membershipId, context.locale, roleId),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.getRoleDefinition(roleId, await context.buildOptions({ signal })),
  });
}

export function useRoleChangeRequests(status: string, enabled = true): UseQueryResult<RoleChangeRequestListResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: roleAdminKeys.requests(context.membershipId, context.locale, status),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.listRoleChangeRequests(status, await context.buildOptions({ signal })),
  });
}

export function useCreateRoleDefinition(): UseMutationResult<
  RoleDefinitionResponse, Error, { payload: SaveRoleDefinitionRequest; idempotencyKey: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) =>
      apiClient.createRoleDefinition(payload, await context.buildOptions({ idempotencyKey })),
    onSuccess: () => invalidateRoles(queryClient, context.membershipId),
  });
}

export function useUpdateRoleDefinition(): UseMutationResult<
  UpdateRoleDefinitionResponse, Error, { roleId: string; payload: SaveRoleDefinitionRequest; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ roleId, payload, ifMatch }) =>
      apiClient.updateRoleDefinition(roleId, payload, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateRoles(queryClient, context.membershipId),
  });
}

export function useSetRoleDefinitionActive(): UseMutationResult<
  RoleDefinitionResponse, Error, { roleId: string; active: boolean; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ roleId, active, ifMatch }) =>
      apiClient.setRoleDefinitionActive(roleId, active, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateRoles(queryClient, context.membershipId),
  });
}

export function useDecideRoleChangeRequest(): UseMutationResult<
  RoleChangeRequestResponse, Error, { requestId: string; decision: "approve" | "reject" | "cancel"; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ requestId, decision, ifMatch }) =>
      apiClient.decideRoleChangeRequest(requestId, decision, await context.buildOptions({ ifMatch })),
    onSuccess: () => invalidateRoles(queryClient, context.membershipId),
  });
}
