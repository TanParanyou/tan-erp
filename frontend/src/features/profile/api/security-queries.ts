import {
  useMutation,
  useQuery,
  useQueryClient,
  type UseMutationResult,
  type UseQueryResult,
} from "@tanstack/react-query";
import {
  apiClient,
  type ActiveSessionListResponse,
  type DisableTwoFactorRequest,
  type EnableTwoFactorRequest,
  type MySecurityStatusResponse,
  type RegenerateBackupCodesRequest,
  type RevokeSessionsResponse,
  type TotpSetupPayloadDto,
  type TwoFactorBackupCodesResponse,
} from "@/lib/api/api-client";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";

export const securityKeys = {
  status: (membershipId: string | null | undefined) => ["my-security-status", membershipId] as const,
  sessions: (membershipId: string | null | undefined) => ["my-active-sessions", membershipId] as const,
};

export function useMySecurityStatus(): UseQueryResult<MySecurityStatusResponse, Error> {
  const context = useApiRequestContext();

  return useQuery({
    queryKey: securityKeys.status(context.membershipId),
    queryFn: async ({ signal }) =>
      apiClient.getMySecurityStatus(await context.buildOptions({ signal })),
    enabled: Boolean(context.membershipId),
  });
}

export function useSetupTwoFactor(): UseMutationResult<TotpSetupPayloadDto, Error, void> {
  const context = useApiRequestContext();

  return useMutation({
    mutationFn: async () =>
      apiClient.setupMyTwoFactor(await context.buildOptions()),
  });
}

export function useEnableTwoFactor(): UseMutationResult<
  TwoFactorBackupCodesResponse,
  Error,
  EnableTwoFactorRequest
> {
  const context = useApiRequestContext();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (payload) =>
      apiClient.enableMyTwoFactor(payload, await context.buildOptions()),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: securityKeys.status(context.membershipId) });
    },
  });
}

export function useDisableTwoFactor(): UseMutationResult<
  TwoFactorBackupCodesResponse,
  Error,
  DisableTwoFactorRequest
> {
  const context = useApiRequestContext();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (payload) =>
      apiClient.disableMyTwoFactor(payload, await context.buildOptions()),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: securityKeys.status(context.membershipId) });
    },
  });
}

export function useRegenerateBackupCodes(): UseMutationResult<
  TwoFactorBackupCodesResponse,
  Error,
  RegenerateBackupCodesRequest
> {
  const context = useApiRequestContext();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (payload) =>
      apiClient.regenerateMyBackupCodes(payload, await context.buildOptions()),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: securityKeys.status(context.membershipId) });
    },
  });
}

export function useMyActiveSessions(): UseQueryResult<ActiveSessionListResponse, Error> {
  const context = useApiRequestContext();

  return useQuery({
    queryKey: securityKeys.sessions(context.membershipId),
    queryFn: async ({ signal }) =>
      apiClient.listMySessions(await context.buildOptions({ signal })),
    enabled: Boolean(context.membershipId),
  });
}

export function useRevokeMySession(): UseMutationResult<void, Error, string> {
  const context = useApiRequestContext();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (sessionId: string) =>
      apiClient.revokeMySession(sessionId, await context.buildOptions()),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: securityKeys.sessions(context.membershipId) });
    },
  });
}

export function useRevokeOtherSessions(): UseMutationResult<RevokeSessionsResponse, Error, void> {
  const context = useApiRequestContext();
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async () =>
      apiClient.revokeOtherSessions(await context.buildOptions()),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: securityKeys.sessions(context.membershipId) });
    },
  });
}
