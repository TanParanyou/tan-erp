import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import messages from "@/messages/th.json";
import { useItemMasterLookups } from "@/features/item-master/api/item-master-queries";
import { BrandAutocomplete } from "./brand-autocomplete";

vi.mock("@/features/item-master/api/item-master-queries", () => ({ useItemMasterLookups: vi.fn() }));
vi.mock("@/components/ui/Avatar", () => ({ Avatar: ({ initial }: { initial?: string }) => <span>{initial}</span> }));

describe("BrandAutocomplete", () => {
  beforeEach(() => {
    vi.mocked(useItemMasterLookups).mockReturnValue({
      data: {
        categories: [],
        brands: [{ id: "brand-1", code: "BRD-00001", name: { thai: "วนชัย", english: "Vanachai" }, imageFileId: "file-1", status: "active" }],
        units: [],
        taxCategories: [],
      },
      isLoading: false,
      isError: false,
    } as unknown as ReturnType<typeof useItemMasterLookups>);
  });

  it("renders image-backed option and reports controlled selection", async () => {
    const onChange = vi.fn();
    const onSelectedOptionChange = vi.fn();
    render(
      <NextIntlClientProvider locale="th" messages={messages}>
        <BrandAutocomplete value="" onChange={onChange} onSelectedOptionChange={onSelectedOptionChange} />
      </NextIntlClientProvider>
    );

    fireEvent.focus(screen.getByRole("combobox", { name: "แบรนด์" }));
    await waitFor(() => expect(screen.getByText("BRD-00001 · วนชัย")).toBeInTheDocument());
    fireEvent.click(screen.getByText("BRD-00001 · วนชัย"));

    expect(onChange).toHaveBeenCalledWith("brand-1");
    expect(onSelectedOptionChange).toHaveBeenCalledWith(expect.objectContaining({ imageFileId: "file-1" }));
  });
});
