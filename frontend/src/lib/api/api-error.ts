import type { ProblemDetails } from "./problem-details";

export class ApiError extends Error {
  readonly status: number;
  readonly code: string;
  readonly traceId?: string;
  readonly errors?: Record<string, string[]>;
  readonly isAbort: boolean;

  constructor(options: {
    status: number;
    code: string;
    message: string;
    traceId?: string;
    errors?: Record<string, string[]>;
    isAbort?: boolean;
  }) {
    super(options.message);
    this.name = "ApiError";
    this.status = options.status;
    this.code = options.code;
    this.traceId = options.traceId;
    this.errors = options.errors;
    this.isAbort = options.isAbort ?? false;
  }

  static fromProblemDetails(status: number, problem: ProblemDetails): ApiError {
    const code = problem.code || (status === 401 ? "AUTHENTICATION_REQUIRED" : "UNKNOWN_ERROR");
    return new ApiError({
      status,
      code,
      message: problem.title || problem.detail || code,
      traceId: problem.traceId || undefined,
      errors: problem.errors || undefined,
    });
  }

  static fromAbort(): ApiError {
    return new ApiError({
      status: 0,
      code: "REQUEST_ABORTED",
      message: "The request was aborted.",
      isAbort: true,
    });
  }

  static fromUnknown(status = 500, traceId?: string): ApiError {
    return new ApiError({
      status,
      code: "UNKNOWN_ERROR",
      message: "An unexpected server error occurred.",
      traceId,
    });
  }
}

export class AuthenticationRequiredError extends ApiError {
  constructor(message = "No authentication token available") {
    super({
      status: 401,
      code: "AUTHENTICATION_REQUIRED",
      message,
    });
    this.name = "AuthenticationRequiredError";
  }
}

export class MembershipRequiredError extends ApiError {
  constructor(message = "No active membership selected") {
    super({
      status: 400,
      code: "MEMBERSHIP_CONTEXT_REQUIRED",
      message,
    });
    this.name = "MembershipRequiredError";
  }
}

export function isAuthenticationRequiredError(error: unknown): boolean {
  if (!error) return false;
  if (error instanceof AuthenticationRequiredError) return true;
  if (error instanceof ApiError && error.code === "AUTHENTICATION_REQUIRED") return true;
  return false;
}

export function isMembershipRequiredError(error: unknown): boolean {
  if (!error) return false;
  if (error instanceof MembershipRequiredError) return true;
  if (
    error instanceof ApiError &&
    (error.code === "MEMBERSHIP_CONTEXT_REQUIRED" || error.code === "ACTIVE_MEMBERSHIP_REQUIRED")
  ) {
    return true;
  }
  return false;
}
