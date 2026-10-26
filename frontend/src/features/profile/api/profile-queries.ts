import { useMutation, useQueryClient, type UseMutationResult } from "@tanstack/react-query";
import { useTranslations } from "next-intl";
import {
  apiClient,
  type UpdateMyPreferencesRequest,
  type UpdateMyProfileRequest,
  type UserDto,
} from "@/lib/api/api-client";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useToast } from "@/hooks/useToast";
import { useTheme, type ThemePreferences } from "@/providers/theme-provider";
import { profileErrorKey } from "../profile-errors";
import { readServerPreferences } from "../preferences";

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

/** Saves the caller's own theme preferences, then refreshes /me. */
export function useUpdateMyPreferences(): UseMutationResult<UserDto, Error, UpdateMyPreferencesRequest> {
  const context = useApiRequestContext();
  const queryClient = useQueryClient();

  return useMutation<UserDto, Error, UpdateMyPreferencesRequest>({
    mutationFn: async (payload) => apiClient.updateMyPreferences(payload, await context.buildOptions()),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["current-user"] });
    },
  });
}

/**
 * Applies a preference change on screen immediately, then saves it to the server.
 * On failure the screen reverts to the last server values and an error toast is shown.
 */
export function useSaveThemePreferences(): {
  save: (change: Partial<ThemePreferences> & { preferredLocale?: "th" | "en" }) => Promise<boolean>;
  isPending: boolean;
} {
  const t = useTranslations("myProfile");
  const { toast } = useToast();
  const { theme, density, textScale, applyPreferences } = useTheme();
  const { currentUser } = useSelectedMembership();
  const mutation = useUpdateMyPreferences();
  const serverPreferences = readServerPreferences(currentUser.user?.preferences);

  const save = async ({ preferredLocale, ...change }: Partial<ThemePreferences> & { preferredLocale?: "th" | "en" }): Promise<boolean> => {
    if (mutation.isPending) return false;
    const next: ThemePreferences = { themeMode: theme, density, textScale, ...change };
    applyPreferences(next);
    try {
      await mutation.mutateAsync(preferredLocale ? { ...next, preferredLocale } : next);
      return true;
    } catch (error: unknown) {
      if (serverPreferences) applyPreferences(serverPreferences);
      toast.error(t(`errors.${profileErrorKey(error)}`));
      return false;
    }
  };

  return { save, isPending: mutation.isPending };
}
