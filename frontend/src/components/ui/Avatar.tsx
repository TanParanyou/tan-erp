"use client";

import React, { useState } from "react";
import { cn } from "@/lib/utils/cn";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";

export type AvatarSize = "sm" | "md" | "lg";
export type AvatarVariant = "navy" | "muted" | "outline";

export interface AvatarProps {
  initial?: string;
  icon?: React.ReactNode;
  src?: string | null;
  fileId?: string | null;
  alt?: string;
  size?: AvatarSize;
  variant?: AvatarVariant;
  className?: string;
  title?: string;
}

const sizeClasses: Record<AvatarSize, string> = {
  sm: "w-7 h-7 text-[11px]",
  md: "w-9 h-9 text-xs",
  lg: "w-12 h-12 text-sm",
};

const variantClasses: Record<AvatarVariant, string> = {
  navy: "bg-erp-navy text-white border-erp-navy",
  muted: "bg-erp-surface-muted text-erp-navy border-erp-border",
  outline: "bg-erp-surface text-erp-text-main border-erp-border",
};

export function Avatar({
  initial,
  icon,
  src,
  fileId,
  alt = "Avatar",
  size = "md",
  variant = "muted",
  className,
  title,
}: AvatarProps) {
  const [imageError, setImageError] = useState(false);

  const resolvedInitial = initial
    ? initial.trim().charAt(0).toUpperCase()
    : undefined;

  return (
    <div
      className={cn(
        "flex items-center justify-center font-bold font-mono border rounded-none shrink-0 select-none overflow-hidden",
        sizeClasses[size],
        variantClasses[variant],
        className
      )}
      title={title ?? alt}
      aria-label={title ?? alt}
    >
      {fileId ? (
        <AuthenticatedFileImage
          fileId={fileId}
          alt={alt}
          className="w-full h-full object-cover rounded-none"
        />
      ) : src && !imageError ? (
        <img
          src={src}
          alt={alt}
          onError={() => setImageError(true)}
          className="w-full h-full object-cover rounded-none"
        />
      ) : icon ? (
        icon
      ) : (
        <span>{resolvedInitial ?? "-"}</span>
      )}
    </div>
  );
}

export default Avatar;
