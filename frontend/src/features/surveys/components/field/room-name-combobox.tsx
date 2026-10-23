"use client";

import React, { useState, useEffect } from "react";
import { useTranslations } from "next-intl";

export interface RoomNameComboboxProps {
  value: string;
  onChange: (roomName: string) => void;
  disabled?: boolean;
  required?: boolean;
  className?: string;
}

const CUSTOM_OPTION_VALUE = "__custom__";

export function RoomNameCombobox({
  value,
  onChange,
  disabled = false,
  required = false,
  className,
}: RoomNameComboboxProps) {
  const t = useTranslations("fieldSurvey");

  const standardRooms: readonly { key: string; label: string }[] = [
    { key: "masterBedroom", label: t("rooms.masterBedroom") },
    { key: "livingRoom", label: t("rooms.livingRoom") },
    { key: "kitchen", label: t("rooms.kitchen") },
    { key: "walkInCloset", label: t("rooms.walkInCloset") },
    { key: "homeOffice", label: t("rooms.homeOffice") },
    { key: "bedroom2", label: t("rooms.bedroom2") },
    { key: "bedroom3", label: t("rooms.bedroom3") },
    { key: "masterBathroom", label: t("rooms.masterBathroom") },
    { key: "hallway", label: t("rooms.hallway") },
    { key: "balcony", label: t("rooms.balcony") },
  ];

  const isPredefined = standardRooms.some((r) => r.label === value);
  const [isCustomMode, setIsCustomMode] = useState<boolean>(!isPredefined && value.trim().length > 0);

  useEffect(() => {
    if (!isPredefined && value.trim().length > 0) {
      setIsCustomMode(true);
    }
  }, [value, isPredefined]);

  const handleSelectChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const selected = e.target.value;
    if (selected === CUSTOM_OPTION_VALUE) {
      setIsCustomMode(true);
      onChange("");
    } else if (selected === "") {
      setIsCustomMode(false);
      onChange("");
    } else {
      setIsCustomMode(false);
      onChange(selected);
    }
  };

  const handleCustomInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChange(e.target.value);
  };

  return (
    <div className={className}>
      <label className="mb-1 block text-xs font-semibold text-erp-text-main">
        {t("roomName")} {required && <span className="text-erp-danger">*</span>}
      </label>

      <div className="space-y-2">
        {/* Dropdown Selector */}
        <select
          value={isCustomMode ? CUSTOM_OPTION_VALUE : value}
          onChange={handleSelectChange}
          disabled={disabled}
          className="min-h-[44px] w-full border border-erp-border bg-white px-3 py-2 text-xs font-medium text-erp-text-main focus:border-erp-navy focus:outline-none"
        >
          <option value="">{t("selectOrTypeRoom")}</option>
          {/* First option: Custom typing option */}
          <option value={CUSTOM_OPTION_VALUE}>
            {t("customRoomOption")}
          </option>
          {/* Standard Popular Rooms */}
          {standardRooms.map((r) => (
            <option key={r.key} value={r.label}>
              {r.label}
            </option>
          ))}
        </select>

        {/* Custom Input shown when custom option is selected or text is custom */}
        {isCustomMode && (
          <div className="flex items-center gap-2">
            <input
              type="text"
              autoFocus
              value={value}
              onChange={handleCustomInputChange}
              disabled={disabled}
              required={required}
              placeholder={t("roomPlaceholder")}
              className="min-h-[44px] flex-1 border border-erp-navy bg-white px-3 py-2 text-xs font-semibold text-erp-navy focus:outline-none focus:ring-1 focus:ring-erp-navy"
            />
            <button
              type="button"
              disabled={disabled}
              onClick={() => {
                setIsCustomMode(false);
                onChange("");
              }}
              className="inline-flex min-h-[44px] items-center border border-erp-border bg-erp-surface-subtle px-3 text-xs font-medium text-erp-text-muted hover:text-erp-navy"
            >
              รีเซ็ต
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
