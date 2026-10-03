import { getAuthToken } from "@/lib/auth/auth-session";
import { useSafeLocale } from "@/lib/i18n/i18n-context";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import type { RequestOptions } from "./api-client";
import { AuthenticationRequiredError, MembershipRequiredError } from "./api-error";

export type ApiLocale = "th" | "en";

export interface ApiRequestContext {
  /** Selected membership id; queries should stay disabled while it is missing. */
  membershipId: string | undefined;
  /** UI locale normalized to a supported API locale. */
  locale: ApiLocale;
  /**
   * Builds the authenticated request options (token, membership, locale) for one API call.
   * Rejects with the shared auth errors when there is no session or no selected membership.
   */
  buildOptions: (extra?: Partial<RequestOptions>) => Promise<RequestOptions>;
}

/**
 * Shared request context for TanStack Query hooks, so each feature does not re-implement
 * token/membership/locale plumbing before calling the central ApiClient.
 */
export function useApiRequestContext(): ApiRequestContext {
  const rawLocale = useSafeLocale();
  const locale: ApiLocale = rawLocale === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const membershipId = selectedMembership?.id;

  return {
    membershipId,
    locale,
    buildOptions: async (extra) => {
      const token = await getAuthToken();
      if (!token) throw new AuthenticationRequiredError();
      if (!membershipId) throw new MembershipRequiredError();
      return { token, membershipId, locale, ...extra };
    },
  };
}
