import { ApiError } from "./api-error";
import type { RequestOptions } from "./api-client";
import type { ProblemDetails } from "./problem-details";
import type { components } from "@/generated/api/tan-erp.v1";

function isProblemDetails(value: unknown): value is ProblemDetails {
  if (typeof value !== "object" || value === null) return false;
  if ("code" in value && value.code !== null && typeof value.code !== "string") return false;
  if ("status" in value && value.status !== null && typeof value.status !== "number") return false;
  if ("title" in value && value.title !== null && typeof value.title !== "string") return false;
  return "code" in value || "status" in value || "title" in value;
}

export type CreateUploadSessionRequest = components["schemas"]["CreateUploadSessionRequest"];
export type CreateUploadSessionResponse = components["schemas"]["CreateUploadSessionResponse"];
export type CompleteUploadSessionResponse = components["schemas"]["CompleteUploadSessionResponse"];
export type CompletedFileResponse = components["schemas"]["CompletedFileResponse"];

/**
 * Global reusable client for the verified file upload service.
 * Follows the 3-step deferred upload protocol:
 * 1. Create upload session with expected files
 * 2. Upload actual file binaries to slots
 * 3. Complete upload session, verifying magic numbers and receiving persistent file IDs.
 */
export class FileClient {
  private readonly baseUrl: string;

  constructor(baseUrl?: string) {
    this.baseUrl = (baseUrl || process.env.NEXT_PUBLIC_API_BASE_URL || "").replace(/\/$/, "");
  }

  getFileUrl(fileId?: string | null): string {
    if (!fileId) return "";
    return `${this.baseUrl}/api/v1/files/${encodeURIComponent(fileId)}/content`;
  }

  async getFileBlob(fileId: string, options: RequestOptions): Promise<Blob> {
    const { token, membershipId, locale = "th", signal } = options;
    const headers: Record<string, string> = {
      Authorization: `Bearer ${token.trim()}`,
      "Accept-Language": locale,
      Accept: "image/webp,image/png,image/jpeg",
    };

    if (membershipId) headers["X-Membership-Id"] = membershipId;

    const response = await fetch(this.getFileUrl(fileId), {
      method: "GET",
      headers,
      signal,
    });

    if (!response.ok) {
      let problem: unknown = null;
      try {
        problem = await response.json();
      } catch {
        // The response may not contain a JSON problem body.
      }
      if (isProblemDetails(problem)) {
        throw ApiError.fromProblemDetails(response.status, problem);
      }
      throw ApiError.fromUnknown(response.status);
    }

    return response.blob();
  }

  async createSession(
    payload: CreateUploadSessionRequest,
    options: RequestOptions
  ): Promise<CreateUploadSessionResponse> {
    const { token, membershipId, idempotencyKey, locale = "th", signal } = options;

    const headers: Record<string, string> = {
      Authorization: `Bearer ${token.trim()}`,
      "Accept-Language": locale,
      Accept: "application/json",
      "Content-Type": "application/json",
    };

    if (membershipId) headers["X-Membership-Id"] = membershipId;
    if (idempotencyKey) headers["Idempotency-Key"] = idempotencyKey;

    const response = await fetch(`${this.baseUrl}/api/v1/files/upload-sessions`, {
      method: "POST",
      headers,
      body: JSON.stringify(payload),
      signal,
    });

    if (!response.ok) {
      let problem = null;
      try {
        problem = await response.json();
      } catch {
        // non-JSON problem
      }
      if (problem && (problem.code || problem.status || problem.title)) {
        throw ApiError.fromProblemDetails(response.status, problem);
      }
      throw ApiError.fromUnknown(response.status);
    }

    return response.json();
  }

  /**
   * Completes an upload session with multiple files via multipart/form-data.
   */
  async completeSession(
    sessionId: string,
    files: (File | { slotId: string; file: File })[],
    options: RequestOptions
  ): Promise<CompleteUploadSessionResponse> {
    const { token, membershipId, locale = "th", signal } = options;

    const formData = new FormData();
    for (const item of files) {
      if ("slotId" in item && "file" in item) {
        formData.append(item.slotId, item.file, item.file.name);
      } else {
        formData.append(item.name, item, item.name);
      }
    }

    const headers: Record<string, string> = {
      Authorization: `Bearer ${token.trim()}`,
      "Accept-Language": locale,
      Accept: "application/json",
    };

    if (membershipId) headers["X-Membership-Id"] = membershipId;

    const response = await fetch(
      `${this.baseUrl}/api/v1/files/upload-sessions/${encodeURIComponent(sessionId)}/complete`,
      {
        method: "POST",
        headers,
        body: formData,
        signal,
      }
    );

    if (!response.ok) {
      let problem = null;
      try {
        problem = await response.json();
      } catch {
        // non-JSON problem
      }
      if (problem && (problem.code || problem.status || problem.title)) {
        throw ApiError.fromProblemDetails(response.status, problem);
      }
      throw ApiError.fromUnknown(response.status);
    }

    return response.json();
  }
}

export const fileClient = new FileClient();
