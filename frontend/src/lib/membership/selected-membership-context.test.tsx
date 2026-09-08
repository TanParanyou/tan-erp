"use client";

import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import {
  SelectedMembershipProvider,
  useSelectedMembership,
} from "./selected-membership-context";
import type { CurrentUserResponse } from "@/lib/api/api-client";
import { customerListQueryKey } from "@/features/customers/api/customer-queries";

const currentUser = {
  user: { id: "user-1", displayName: "Test User", email: "test@example.test" },
  memberships: [
    { id: "membership-a", organization: { id: "org-a", name: "Org A" }, permissions: [] },
    { id: "membership-b", organization: { id: "org-b", name: "Org B" }, permissions: [] },
  ],
} as unknown as CurrentUserResponse;

function Probe({ queryClient }: { queryClient: QueryClient }): React.JSX.Element {
  const { selectedMembership, setSelectedMembershipId } = useSelectedMembership();
  return (
    <div>
      <span data-testid="selected">{selectedMembership?.id ?? "none"}</span>
      <button type="button" onClick={() => void setSelectedMembershipId("membership-b")}>
        switch-to-b
      </button>
      <button
        type="button"
        onClick={() => void setSelectedMembershipId("membership-unknown")}
      >
        switch-unknown
      </button>
      <span data-testid="client">{queryClient ? "ready" : "missing"}</span>
    </div>
  );
}

describe("SelectedMembershipProvider switch", () => {
  it("cancels then removes business queries before exposing B and keeps auth queries", async () => {
    const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    const order: string[] = [];
    vi.spyOn(queryClient, "cancelQueries").mockImplementation(async () => {
      order.push("cancel");
    });
    const originalRemove = queryClient.removeQueries.bind(queryClient);
    vi.spyOn(queryClient, "removeQueries").mockImplementation((filters) => {
      order.push("remove");
      return originalRemove(filters);
    });

    queryClient.setQueryData(["business", "membership-a"], { marker: "a" });
    queryClient.setQueryData(["business", "membership-b"], { marker: "b" });
    queryClient.setQueryData(["auth", "current-user"], { marker: "auth" });
    queryClient.setQueryData(customerListQueryKey("membership-a", "th", { limit: 10 }), {
      marker: "list-a",
    });

    render(
      <QueryClientProvider client={queryClient}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <SelectedMembershipProvider currentUser={currentUser}>
            <Probe queryClient={queryClient} />
          </SelectedMembershipProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>,
    );

    expect(screen.getByTestId("selected").textContent).toBe("membership-a");

    fireEvent.click(screen.getByText("switch-to-b"));

    await waitFor(() => expect(screen.getByTestId("selected").textContent).toBe("membership-b"));
    expect(order).toEqual(["cancel", "remove"]);
    expect(queryClient.getQueryData(["business", "membership-a"])).toBeUndefined();
    expect(queryClient.getQueryData(["business", "membership-b"])).toBeUndefined();
    expect(queryClient.getQueryData(["auth", "current-user"])).toEqual({ marker: "auth" });
  });

  it("rejects unknown membership ids", async () => {
    const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
    render(
      <QueryClientProvider client={queryClient}>
        <NextIntlClientProvider locale="th" messages={thMessages}>
          <SelectedMembershipProvider currentUser={currentUser}>
            <Probe queryClient={queryClient} />
          </SelectedMembershipProvider>
        </NextIntlClientProvider>
      </QueryClientProvider>,
    );

    fireEvent.click(screen.getByText("switch-unknown"));
    await waitFor(() => expect(screen.getByTestId("selected").textContent).toBe("membership-a"));
  });
});
