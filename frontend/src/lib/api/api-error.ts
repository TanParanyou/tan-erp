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
