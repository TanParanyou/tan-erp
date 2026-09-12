import { describe, expect, it, vi, beforeEach, afterEach } from "vitest";
import { renderHook, waitFor, act } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import React from "react";
import {
  opportunityListQueryKey,
  opportunityDetailQueryKey,
  useQualifyOpportunity,
  useUpdateDraftQGate,
} from "./opportunity-queries";
import { apiClient, type OpportunityResponse } from "@/lib/api/api-client";
import * as authSession from "@/lib/auth/auth-session";
import * as membershipContext from "@/lib/membership/selected-membership-context";

describe("opportunity business query keys", () => {
  it("distinguishes filters", () => {
    expect(opportunityListQueryKey("membership-a", "th", { stage: "draft", limit: 10 })).not.toEqual(
      opportunityListQueryKey("membership-a", "th", { stage: "draft", limit: 25 })
    );
    expect(opportunityListQueryKey("membership-a", "th", { customerId: "c1" })).not.toEqual(
      opportunityListQueryKey("membership-a", "th", { customerId: "c2" })
    );
  });

  it("distinguishes detail opportunities", () => {
    expect(opportunityDetailQueryKey("membership-a", "th", "opp-1")).not.toEqual(
      opportunityDetailQueryKey("membership-a", "th", "opp-2")
    );
  });

  it("distinguishes memberships and locales", () => {
    expect(opportunityDetailQueryKey("membership-a", "th", "opp-1")).not.toEqual(
      opportunityDetailQueryKey("membership-b", "th", "opp-1")
    );
    expect(opportunityDetailQueryKey("membership-a", "th", "opp-1")).not.toEqual(
      opportunityDetailQueryKey("membership-a", "en", "opp-1")
    );
  });

  it("namespaces business keys with membership and locale", () => {
    const key = opportunityListQueryKey("membership-a", "th", { stage: "draft" });
    expect(key[0]).toBe("business");
    expect(key).toContain("membership-a");
    expect(key).toContain("th");
    expect(key).toContain("opportunities");
  });
});

describe("useQualifyOpportunity mutation", () => {
  let queryClient: QueryClient;

  beforeEach(() => {
    queryClient = new QueryClient({
      defaultOptions: {
        queries: { retry: false },
        mutations: { retry: false },
      },
    });
    vi.restoreAllMocks();
  });

  it("useQualifyOpportunity_Success_PostsContractAndRefreshesCaches", async () => {
    const mockToken = "sample-token";
    const mockMembership = {
      id: "membership-123",
      organization: { id: "org-1", name: "Org 1" },
      branch: { id: "branch-1", name: "Branch 1" },
      permissions: [{ key: "opportunities.transition", scope: "organization", scopeId: "org-1" }],
    };

    vi.spyOn(authSession, "getAuthToken").mockResolvedValue(mockToken);
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      selectedMembership: mockMembership,
      currentUser: {
        user: { id: "user-1", displayName: "User", email: "user@example.test" },
        memberships: [mockMembership],
      },
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    const mockResponse: OpportunityResponse = {
      id: "opp-123",
      code: "OPP-001",
      customerId: "cust-1",
      branchId: "branch-1",
      ownerUserId: "user-1",
      title: "โครงการปรับปรุง",
      workTypes: ["built-in"],
      stage: "qualified",
      rowVersion: "00000000-0000-0000-0000-000000000002",
      createdAtUtc: "2026-09-10T10:00:00Z",
    };

    const transitionSpy = vi.spyOn(apiClient, "transitionOpportunityStage").mockResolvedValue(mockResponse);
    const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries");

    const wrapper = ({ children }: { children: React.ReactNode }) => (
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    );

    const { result } = renderHook(() => useQualifyOpportunity(), { wrapper });

    await act(async () => {
      await result.current.mutateAsync({
        opportunityId: "opp-123",
        expectedVersion: "00000000-0000-0000-0000-000000000001",
        idempotencyKey: "idemp-key-1",
      });
    });

    expect(transitionSpy).toHaveBeenCalledTimes(1);
    expect(transitionSpy).toHaveBeenCalledWith(
      "opp-123",
      {
        targetStage: "qualified",
        expectedVersion: "00000000-0000-0000-0000-000000000001",
      },
      {
        token: mockToken,
        membershipId: "membership-123",
        locale: "th",
        idempotencyKey: "idemp-key-1",
      }
    );

    // Verify detail cache was populated with returned resource
    const detailKey = opportunityDetailQueryKey("membership-123", "th", "opp-123");
    const cachedDetail = queryClient.getQueryData(detailKey);
    expect(cachedDetail).toEqual(mockResponse);

    // Verify list queries were invalidated
    expect(invalidateSpy).toHaveBeenCalled();
  });

  it("useUpdateDraftQGate_Success_SendsIfMatchAndRefreshesCaches", async () => {
    const mockToken = "sample-token";
    const mockMembership = {
      id: "membership-123",
      organization: { id: "org-1", name: "Org 1" },
      branch: { id: "branch-1", name: "Branch 1" },
      permissions: [{ key: "opportunities.update", scope: "organization", scopeId: "org-1" }],
    };

    vi.spyOn(authSession, "getAuthToken").mockResolvedValue(mockToken);
    vi.spyOn(membershipContext, "useSelectedMembership").mockReturnValue({
      selectedMembership: mockMembership,
      currentUser: {
        user: { id: "user-1", displayName: "User", email: "user@example.test" },
        memberships: [mockMembership],
      },
      memberships: [mockMembership],
      setSelectedMembershipId: vi.fn(),
    });

    const mockResponse: OpportunityResponse = {
      id: "opp-123",
      code: "OPP-001",
      customerId: "cust-1",
      branchId: "branch-1",
      ownerUserId: "user-1",
      title: "โครงการปรับปรุง",
      scopeSummary: "ขอบเขตใหม่",
      workTypes: ["built-in"],
      nextActionAtUtc: "2026-09-15T10:00:00Z",
      nextActionNote: "โทรติดต่อ",
      stage: "draft",
      rowVersion: "00000000-0000-0000-0000-000000000003",
      createdAtUtc: "2026-09-10T10:00:00Z",
    };

    const updateSpy = vi.spyOn(apiClient, "updateDraftQGate").mockResolvedValue(mockResponse);
    const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries");

    const wrapper = ({ children }: { children: React.ReactNode }) => (
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    );

    const { result } = renderHook(() => useUpdateDraftQGate(), { wrapper });

    const patchPayload = {
      scopeSummary: "ขอบเขตใหม่",
      workTypes: ["built-in"],
      nextActionAtUtc: "2026-09-15T10:00:00Z",
      nextActionNote: "โทรติดต่อ",
    };

    await act(async () => {
      await result.current.mutateAsync({
        opportunityId: "opp-123",
        expectedVersion: "00000000-0000-0000-0000-000000000001",
        payload: patchPayload,
        idempotencyKey: "idemp-patch-1",
      });
    });

    expect(updateSpy).toHaveBeenCalledTimes(1);
    expect(updateSpy).toHaveBeenCalledWith(
      "opp-123",
      patchPayload,
      {
        token: mockToken,
        membershipId: "membership-123",
        locale: "th",
        idempotencyKey: "idemp-patch-1",
        ifMatch: '"00000000-0000-0000-0000-000000000001"',
      }
    );

    // Verify detail cache was populated with returned resource
    const detailKey = opportunityDetailQueryKey("membership-123", "th", "opp-123");
    const cachedDetail = queryClient.getQueryData(detailKey);
    expect(cachedDetail).toEqual(mockResponse);

    // Verify list queries were invalidated
    expect(invalidateSpy).toHaveBeenCalled();
  });
});


