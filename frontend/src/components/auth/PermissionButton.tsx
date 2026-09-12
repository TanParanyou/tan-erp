"use client";

import React from "react";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, type PermissionKey } from "@/lib/permissions/can";
import { Button, type ButtonProps } from "@/components/ui/Button";

export interface PermissionButtonProps extends ButtonProps {
  permission: PermissionKey | PermissionKey[];
  requireAll?: boolean;
}

export function PermissionButton({
  permission,
  requireAll = true,
  ...buttonProps
}: PermissionButtonProps) {
  const { selectedMembership } = useSelectedMembership();
  const permissions = Array.isArray(permission) ? permission : [permission];
  const hasAccess = requireAll
    ? permissions.every((p) => can(selectedMembership, p))
    : permissions.some((p) => can(selectedMembership, p));

  if (!hasAccess) return null;

  return <Button {...buttonProps} />;
}

PermissionButton.displayName = "PermissionButton";
