"use client";

import React, { useState, useMemo } from "react";
import { useTranslations } from "next-intl";
import { useUserList } from "@/features/users/api/user-queries";
import { Button } from "@/components/ui/Button";
import { Avatar } from "@/components/ui/Avatar";
import { cn } from "@/lib/utils/cn";
import { EntityAutocomplete } from "./EntityAutocomplete";
import type { UserListItemResponse } from "@/lib/api/api-client";

export interface UserAutocompleteProps {
  value?: string | null;
  onChange: (userId: string) => void;
  branchId?: string;
  label?: string;
  error?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  className?: string;
}

/**
 * UserAutocomplete - Atelier Architectural Navy Sharp
 * Consumes global EntityAutocomplete with user/staff-specific layout and cards.
 */
export function UserAutocomplete({
  value,
  onChange,
  branchId,
  label,
  error,
  placeholder,
  required,
  disabled = false,
  className,
}: UserAutocompleteProps) {
  const t = useTranslations("opportunities");
  const [searchQuery, setSearchQuery] = useState("");

  // Fetch active users in branch/organization
  const { data: userData, isLoading } = useUserList({
    branchId: branchId || undefined,
    search: searchQuery.trim() || undefined,
    limit: 20,
  });

  const userList: UserListItemResponse[] = useMemo(
    () => userData?.items ?? [],
    [userData?.items]
  );

  // Find currently selected user details if value is present
  const selectedUser = useMemo(() => {
    if (!value) return null;
    return userList.find((u) => u.id === value) ?? null;
  }, [value, userList]);

  const resolvedLabel = label || t("newOwnerLabel");
  const resolvedPlaceholder = placeholder || t("searchUserPlaceholder");

  return (
    <EntityAutocomplete<UserListItemResponse>
      value={value}
      onChange={onChange}
      label={resolvedLabel}
      placeholder={resolvedPlaceholder}
      error={error}
      required={required}
      disabled={disabled}
      className={className}
      items={userList}
      isLoading={isLoading}
      emptyText={t("noUsersFound")}
      loadingText={t("saving")}
      onSearchChange={setSearchQuery}
      getItemKey={(u) => u.id ?? ""}
      renderSelectedCard={(onClear) => (
        <div
          role="region"
          aria-label={resolvedLabel}
          className="flex flex-col gap-3 border border-erp-navy/40 bg-erp-surface p-3.5 shadow-sm"
        >
          <div className="flex flex-col sm:flex-row sm:items-start justify-between gap-3">
            <div className="flex items-start gap-3 min-w-0 flex-1">
              <Avatar
                initial={selectedUser?.displayName || undefined}
                variant="navy"
                size="md"
                className="shrink-0 mt-0.5"
              />

              <div className="flex flex-col min-w-0 flex-1 gap-1">
                <span className="font-bold text-sm text-erp-navy break-words">
                  {selectedUser ? selectedUser.displayName || "-" : `ID: ${value}`}
                </span>
                {selectedUser?.email && (
                  <p className="text-xs text-erp-text-muted break-words m-0 font-mono">
                    {selectedUser.email}
                  </p>
                )}
              </div>
            </div>

            {/* Action Buttons: Change User */}
            <div className="flex items-center gap-2 shrink-0 self-end sm:self-start">
              {!disabled && (
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={onClear}
                  className="shrink-0 text-xs font-semibold"
                >
                  {t("changeUser")}
                </Button>
              )}
            </div>
          </div>
        </div>
      )}
      renderListItem={(user, isHighlighted, isMobile) => (
        <div className="flex flex-col min-w-0">
          <span
            className={cn(
              "font-semibold text-sm truncate",
              !isMobile && isHighlighted ? "text-white" : "text-erp-text-main"
            )}
          >
            {user.displayName || "-"}
          </span>
          {user.email && (
            <span
              className={cn(
                "text-xs font-mono tracking-wider truncate",
                !isMobile && isHighlighted ? "text-white/80" : "text-erp-text-muted"
              )}
            >
              {user.email}
            </span>
          )}
        </div>
      )}
    />
  );
}

export default UserAutocomplete;
