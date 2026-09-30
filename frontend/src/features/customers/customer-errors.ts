import { isAuthenticationRequiredError, isMembershipRequiredError } from "@/lib/api/api-error";

export interface CustomerQueryErrorMessages {
  authenticationRequired: string;
  membershipRequired: string;
  fallback: string;
}

export function getCustomerQueryErrorMessage(
  error: unknown,
  messages: CustomerQueryErrorMessages,
): string {
  if (isAuthenticationRequiredError(error)) return messages.authenticationRequired;
  if (isMembershipRequiredError(error)) return messages.membershipRequired;
  return messages.fallback;
}
