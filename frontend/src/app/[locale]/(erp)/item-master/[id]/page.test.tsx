import React from "react";
import { describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messagesTh from "@/messages/th.json";
import ItemEditorPage from "./page";

const state = vi.hoisted(() => ({ permissions: ["items.read"] as string[] }));

vi.mock("@/features/item-master/components/item-editor", () => ({
  ItemEditor: ({ id }: { id: string }) => <div data-testid="item-editor">{id}</div>,
}));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: { permissions: state.permissions } }),
}));
vi.mock("@/lib/permissions/can", () => ({
  can: (_membership: unknown, permission: string) => state.permissions.includes(permission),
}));

function renderPage(content: React.ReactNode) {
  return render(
    <NextIntlClientProvider locale="th" messages={messagesTh}>
      {content}
    </NextIntlClientProvider>,
  );
}

describe("ItemEditorPage permission guard", () => {
  it("requires item creation permission for the create route", async () => {
    state.permissions = ["items.read"];
    renderPage(await ItemEditorPage({ params: Promise.resolve({ id: "create" }) }));

    expect(screen.queryByTestId("item-editor")).toBeNull();
    expect(screen.getByRole("alert")).toBeInTheDocument();
  });

  it("allows item readers to access an existing item route", async () => {
    state.permissions = ["items.read"];
    renderPage(await ItemEditorPage({ params: Promise.resolve({ id: "item-1" }) }));

    expect(screen.getByTestId("item-editor")).toHaveTextContent("item-1");
  });
});
