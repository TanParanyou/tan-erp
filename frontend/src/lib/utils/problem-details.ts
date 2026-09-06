import type { ProblemDetails } from "@/lib/api/problem-details";

export interface ExtractedProblemDetails {
  title: string;
  detail?: string;
  fieldErrors: Record<string, string>;
}

/**
 * Extracts human-readable errors and field-level validation errors from an RFC 7807 ProblemDetails object.
 */
export function extractProblemDetails(
  problem: ProblemDetails | null | undefined,
  fallbackMessage = "An error occurred"
): ExtractedProblemDetails {
  if (!problem) {
    return {
      title: fallbackMessage,
      fieldErrors: {},
    };
  }

  const title = problem.title || fallbackMessage;
  const detail = problem.detail || undefined;
  const fieldErrors: Record<string, string> = {};

  if (problem.errors && typeof problem.errors === "object") {
    for (const [field, messages] of Object.entries(problem.errors)) {
      if (Array.isArray(messages) && messages.length > 0) {
        // Lowercase the first letter of field to match frontend camelCase convention if needed
        const normalizedField = field.charAt(0).toLowerCase() + field.slice(1);
        fieldErrors[normalizedField] = messages[0];
      }
    }
  }

  return {
    title,
    detail,
    fieldErrors,
  };
}

/**
 * Directly applies field-level ProblemDetails errors to React Hook Form's setError function.
 */
export function applyProblemDetailsToForm<TFieldValues extends Record<string, unknown> = Record<string, unknown>>(
  problem: ProblemDetails | null | undefined,
  setError: (
    name: keyof TFieldValues | string,
    error: { type?: string; message?: string }
  ) => void,
  fallbackMessage = "An error occurred"
): ExtractedProblemDetails {
  const extracted = extractProblemDetails(problem, fallbackMessage);

  for (const [field, message] of Object.entries(extracted.fieldErrors)) {
    setError(field as keyof TFieldValues, {
      type: "server",
      message,
    });
  }

  return extracted;
}
