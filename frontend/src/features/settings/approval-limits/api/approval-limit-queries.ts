import {
  useMutation,
  useQuery,
  useQueryClient,
  type UseMutationResult,
  type UseQueryResult,
} from "@tanstack/react-query";
import {
  apiClient,
  type ApprovalLimitListParams,
  type ApprovalLimitListResponse,
  type ApprovalLimitOptionsResponse,
  type ApprovalLimitResponse,
  type CreateApprovalLimitRequest,
  type UpdateApprovalLimitRequest,
} from "@/lib/api/api-client";
import { useApiRequestContext, type ApiLocale } from "@/lib/api/use-api-request-context";

export const approvalLimitKeys = {
  all: (membershipId: string | null | undefined) => ["business", membershipId] as const,
  options: (membershipId: string | null | undefined, locale: ApiLocale) =>
    ["business", membershipId, locale, "approval-limits", "options"] as const,
  list: (membershipId: string | null | undefined, locale: ApiLocale, params: ApprovalLimitListParams) =>
    ["business", membershipId, locale, "approval-limits", "list", params] as const,
  detail: (membershipId: string | null | undefined, locale: ApiLocale, limitId: string) =>
    ["business", membershipId, locale, "approval-limits", "detail", limitId] as const,
};

export function useApprovalLimits(params: ApprovalLimitListParams): UseQueryResult<ApprovalLimitListResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: approvalLimitKeys.list(context.membershipId, context.locale, params),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.listApprovalLimits(params, await context.buildOptions({ signal })),
  });
}

export function useApprovalLimitOptions(): UseQueryResult<ApprovalLimitOptionsResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: approvalLimitKeys.options(context.membershipId, context.locale),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.getApprovalLimitOptions(await context.buildOptions({ signal })),
  });
}

export function useApprovalLimit(limitId: string, enabled: boolean): UseQueryResult<ApprovalLimitResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: approvalLimitKeys.detail(context.membershipId, context.locale, limitId),
    enabled: Boolean(context.membershipId) && enabled,
    queryFn: async ({ signal }) => apiClient.getApprovalLimit(limitId, await context.buildOptions({ signal })),
  });
}

export function useCreateApprovalLimit(): UseMutationResult<
  ApprovalLimitResponse, Error, { payload: CreateApprovalLimitRequest; idempotencyKey: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ payload, idempotencyKey }) =>
      apiClient.createApprovalLimit(payload, await context.buildOptions({ idempotencyKey })),
    onSuccess: () => void queryClient.invalidateQueries({ queryKey: approvalLimitKeys.all(context.membershipId) }),
  });
}

export function useUpdateApprovalLimit(): UseMutationResult<
  ApprovalLimitResponse, Error, { limitId: string; payload: UpdateApprovalLimitRequest; ifMatch: string }
> {
  const queryClient = useQueryClient();
  const context = useApiRequestContext();
  return useMutation({
    mutationFn: async ({ limitId, payload, ifMatch }) =>
      apiClient.updateApprovalLimit(limitId, payload, await context.buildOptions({ ifMatch })),
    onSuccess: () => void queryClient.invalidateQueries({ queryKey: approvalLimitKeys.all(context.membershipId) }),
  });
}
