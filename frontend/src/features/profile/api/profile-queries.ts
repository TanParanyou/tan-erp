import { useMutation, useQueryClient, type UseMutationResult } from "@tanstack/react-query";
import { apiClient, type UpdateMyProfileRequest, type UserDto } from "@/lib/api/api-client";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";

/** Saves the caller's own profile, then refreshes /me so the header and gates see the new values. */
export function useUpdateMyProfile(): UseMutationResult<UserDto, Error, UpdateMyProfileRequest> {
  const context = useApiRequestContext();
  const queryClient = useQueryClient();

  return useMutation<UserDto, Error, UpdateMyProfileRequest>({
    mutationFn: async (payload) => apiClient.updateMyProfile(payload, await context.buildOptions()),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["current-user"] });
    },
  });
}
