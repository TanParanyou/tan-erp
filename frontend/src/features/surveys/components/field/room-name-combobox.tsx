"use client";

import React, { useState, useEffect } from "react";
import { useTranslations } from "next-intl";
import { cn } from "@/lib/utils/cn";

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

  const handleSwitchToSelect = () => {
    setIsCustomMode(false);
    onChange("");
  };

  return (
    <div className={cn("flex flex-col gap-1.5", className)}>
      <div className="flex items-center justify-between min-h-[20px]">
        <label className="erp-label !mb-0 font-medium text-erp-text-main">
          {t("roomName")} {required && <span className="erp-label-required">*</span>}
        </label>
      </div>

      {/* Single Unified 44px Row: Swaps cleanly without stacked 2-tier clutter */}
      {!isCustomMode ? (
        <select
          value={value}
          onChange={handleSelectChange}
          disabled={disabled}
          className="erp-select h-11 min-h-[44px] w-full text-sm font-medium text-erp-text-main focus:border-erp-navy focus:outline-none"
        >
          <option value="">{t("selectOrTypeRoom")}</option>
          {/* First option: Custom typing mode */}
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
      ) : (
        <div className="relative flex h-11 min-h-[44px] items-center">
          <input
            type="text"
            autoFocus
            value={value}
            onChange={handleCustomInputChange}
            disabled={disabled}
            required={required}
            placeholder={t("roomPlaceholder")}
            className="erp-input h-11 min-h-[44px] w-full pl-3 pr-28 text-sm font-medium text-erp-navy focus:outline-none focus:ring-1 focus:ring-erp-navy"
          />
          <button
            type="button"
            disabled={disabled}
            onClick={handleSwitchToSelect}
            className="absolute right-1 inline-flex h-8 min-h-[32px] items-center border border-erp-border bg-erp-surface-subtle px-2.5 text-[11px] font-medium text-erp-text-muted transition-colors hover:bg-erp-surface hover:text-erp-navy active:bg-erp-surface-subtle"
          >
            {t("selectFromList")}
          </button>
        </div>
      )}
    </div>
  );
}
