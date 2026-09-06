"use client";

import React from "react";
import Link from "next/link";
import { cn } from "@/lib/utils/cn";
import { IconSpinner } from "@/components/common/Icons";

export type ButtonVariant = "primary" | "secondary" | "danger" | "outline" | "ghost";
export type ButtonSize = "sm" | "md" | "lg" | "icon";

export interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  isLoading?: boolean;
  icon?: React.ReactNode;
  href?: string;
  target?: string;
  rel?: string;
}

export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      className,
      variant = "primary",
      size = "md",
      isLoading = false,
      icon,
      disabled,
      href,
      target,
      rel,
      children,
      ...props
    },
    ref
  ) => {
    const isActuallyDisabled = disabled || isLoading;

    const baseClass = cn(
      "erp-btn",
      `erp-btn-${variant}`,
      `erp-btn-${size}`,
      className
    );

    const content = (
      <>
        {isLoading ? (
          <IconSpinner size={size === "sm" ? 14 : 18} />
        ) : icon ? (
          icon
        ) : null}
        {children}
      </>
    );

    if (href && !isActuallyDisabled) {
      return (
        <Link
          href={href}
          target={target}
          rel={rel}
          className={baseClass}
          aria-disabled={isActuallyDisabled}
          {...(props as unknown as React.AnchorHTMLAttributes<HTMLAnchorElement>)}
        >
          {content}
        </Link>
      );
    }

    return (
      <button
        ref={ref}
        disabled={isActuallyDisabled}
        aria-busy={isLoading}
        className={baseClass}
        {...props}
      >
        {content}
      </button>
    );
  }
);

Button.displayName = "Button";
