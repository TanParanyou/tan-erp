import React from "react";
import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { RoomNameCombobox } from "./room-name-combobox";

vi.mock("next-intl", () => ({
  useTranslations: () => (key: string) => {
    const map: Record<string, string> = {
      roomName: "ชื่อห้อง / บริเวณสำรวจ",
      roomPlaceholder: "เช่น ห้องนอนใหญ่ Master Bedroom, โถงรับแขก",
      customRoomOption: "✍️ กรอกชื่อห้องเอง...",
      selectOrTypeRoom: "— เลือกห้องหรือพิมพ์เอง —",
      selectFromList: "เลือกจากลิสต์",
      "rooms.masterBedroom": "ห้องนอนใหญ่ (Master Bedroom)",
      "rooms.livingRoom": "ห้องรับแขก / นั่งเล่น (Living Room)",
      "rooms.kitchen": "ห้องครัว / แพนทรี (Kitchen & Pantry)",
      "rooms.walkInCloset": "ห้องแต่งตัว (Walk-in Closet)",
      "rooms.homeOffice": "ห้องทำงาน (Home Office / Study)",
      "rooms.bedroom2": "ห้องนอน 2 (Bedroom 2)",
      "rooms.bedroom3": "ห้องนอน 3 (Bedroom 3)",
      "rooms.masterBathroom": "ห้องน้ำใหญ่ (Master Bathroom)",
      "rooms.hallway": "โถงทางเดิน / บันได (Hallway & Foyer)",
      "rooms.balcony": "ระเบียงพักผ่อน (Balcony)",
    };
    return map[key] ?? key;
  },
}));

describe("RoomNameCombobox Component", () => {
  it("renders with label, dropdown selection, and custom room option as first option", () => {
    const onChange = vi.fn();
    render(<RoomNameCombobox value="" onChange={onChange} />);

    expect(screen.getByText(/ชื่อห้อง \/ บริเวณสำรวจ/)).toBeDefined();
    const options = screen.getAllByRole("option");
    expect(options[1].textContent).toContain("✍️ กรอกชื่อห้องเอง...");
  });

  it("selects standard room name directly from dropdown", () => {
    const onChange = vi.fn();
    render(<RoomNameCombobox value="" onChange={onChange} />);

    const select = screen.getByRole("combobox");
    fireEvent.change(select, { target: { value: "ห้องนอนใหญ่ (Master Bedroom)" } });

    expect(onChange).toHaveBeenCalledWith("ห้องนอนใหญ่ (Master Bedroom)");
  });

  it("swaps seamlessly to single-row input when custom option is selected and can switch back", () => {
    const onChange = vi.fn();
    render(<RoomNameCombobox value="" onChange={onChange} />);

    // Select custom option
    const select = screen.getByRole("combobox");
    fireEvent.change(select, { target: { value: "__custom__" } });

    // Custom input should now take over the row
    const customInput = screen.getByPlaceholderText(
      "เช่น ห้องนอนใหญ่ Master Bedroom, โถงรับแขก"
    );
    expect(customInput).toBeDefined();

    fireEvent.change(customInput, { target: { value: "ห้องเก็บไวน์ใต้ดิน" } });
    expect(onChange).toHaveBeenCalledWith("ห้องเก็บไวน์ใต้ดิน");

    // Click switch back to list button
    const switchBackBtn = screen.getByRole("button", { name: "เลือกจากลิสต์" });
    fireEvent.click(switchBackBtn);

    // Dropdown returns
    expect(screen.getByRole("combobox")).toBeDefined();
  });
});
