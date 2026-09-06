import type { components } from "@/generated/api/tan-erp.v1";
import { ApiError } from "./api-error";
import type { ProblemDetails } from "./problem-details";

export type CurrentUserResponse = components["schemas"]["CurrentUserResponse"];

export class ApiClient {
  private readonly baseUrl: string;

  constructor(baseUrl?: string) {
    this.baseUrl = (baseUrl || process.env.NEXT_PUBLIC_API_BASE_URL || "").replace(/\/$/, "");
  }

  async getCurrentUser(
    token: string,
    locale: "th" | "en" = "th",
    signal?: AbortSignal
  ): Promise<CurrentUserResponse> {
    if (!token || token.trim() === "") {
      throw new ApiError({
        status: 401,
        code: "AUTHENTICATION_REQUIRED",
        message: "Token is required.",
      });
    }

    const url = `${this.baseUrl}/api/v1/me`;

    try {
      const response = await fetch(url, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token.trim()}`,
          "Accept-Language": locale,
          Accept: "application/json",
        },
        signal,
      });

      if (!response.ok) {
        let problem: ProblemDetails | null = null;
        try {
          problem = await response.json();
        } catch {
          // Response body was not JSON
        }

        if (problem && (problem.code || problem.status || problem.title)) {
          throw ApiError.fromProblemDetails(response.status, problem);
        }

        throw ApiError.fromUnknown(response.status);
      }

      const data: CurrentUserResponse = await response.json();
      return data;
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        throw err;
      }

      if (err instanceof Error && err.name === "AbortError") {
        throw ApiError.fromAbort();
      }

      throw ApiError.fromUnknown(500);
    }
  }
}

export const apiClient = new ApiClient();
