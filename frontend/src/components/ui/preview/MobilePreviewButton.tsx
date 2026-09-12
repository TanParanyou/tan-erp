"use client";

import React from "react";
import { Button, type ButtonProps } from "@/components/ui/Button";
import { IconEye } from "@/components/common/Icons";

export interface MobilePreviewButtonProps extends ButtonProps {
  label?: string;
}

export function MobilePreviewButton({
  label = "ดูพรีวิว Mobile",
  className,
  ...props
}: MobilePreviewButtonProps) {
  return (
    <Button
      variant="secondary"
      size="sm"
      className={className}
      {...props}
    >
      <IconEye size={14} className="mr-1.5" />
      <span>{label}</span>
    </Button>
  );
}

MobilePreviewButton.displayName = "MobilePreviewButton";
