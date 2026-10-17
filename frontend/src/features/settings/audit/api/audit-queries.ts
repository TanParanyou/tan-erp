import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import { apiClient, type AuditEventListParams, type AuditEventListResponse } from "@/lib/api/api-client";
import { useApiRequestContext, type ApiLocale } from "@/lib/api/use-api-request-context";

export const auditKeys = {
  list: (membershipId: string | null | undefined, locale: ApiLocale, params: AuditEventListParams) =>
    ["business", membershipId, locale, "audit-events", "list", params] as const,
};

export function useAuditEvents(params: AuditEventListParams): UseQueryResult<AuditEventListResponse, Error> {
  const context = useApiRequestContext();
  return useQuery({
    queryKey: auditKeys.list(context.membershipId, context.locale, params),
    enabled: Boolean(context.membershipId),
    queryFn: async ({ signal }) => apiClient.listAuditEvents(params, await context.buildOptions({ signal })),
  });
}
