"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface FormContainerProps extends React.FormHTMLAttributes<HTMLFormElement> {
  /** Top header slot, typically PageHeader */
  header?: React.ReactNode;
  /** Primary error banner slot */
  errorBanner?: React.ReactNode;
  /** Secondary alert slot (e.g. duplicate candidate warning) */
  topAlert?: React.ReactNode;
  /** Main form content */
  children: React.ReactNode;
  /** Sticky action bar slot, typically FormActionBar */
  actionBar?: React.ReactNode;
  /** Whether the action bar slot should stick to the bottom of the viewport. Defaults to true */
  stickyActionBar?: boolean;
  /** Maximum width of the form body */
  maxWidth?: "sm" | "md" | "lg" | "xl" | "full" | string;
  className?: string;
  contentClassName?: string;
  /** Whether to render the root container as a <form> element. Defaults to false (renders <div>) */
  asForm?: boolean;
}

const maxWidthMap: Record<string, string> = {
  sm: "max-w-xl",
  md: "max-w-3xl",
  lg: "max-w-5xl",
  xl: "max-w-7xl",
  full: "w-full",
};

export function FormContainer({
  header,
  errorBanner,
  topAlert,
  children,
  actionBar,
  stickyActionBar = true,
  maxWidth = "full",
  className,
  contentClassName,
  asForm = false,
  ...formProps
}: FormContainerProps) {
  const resolvedMaxWidth = maxWidthMap[maxWidth] || maxWidth;

  const body = (
    <>
      {/* Form Body Area */}
      <div
        className={cn(
          "w-full mx-auto flex-1 flex flex-col gap-4 sm:gap-6 mb-6 sm:mb-8 pb-28 sm:pb-24",
          resolvedMaxWidth,
          contentClassName
        )}
      >
        {header}
        {errorBanner}
        {topAlert}
        {children}
      </div>

      {/* Action Bar Slot */}
      {actionBar}
    </>
  );

  const containerClassName = cn(
    "flex-1 flex flex-col justify-between w-full min-h-[calc(100vh-7rem)]",
    className
  );

  if (asForm) {
    return (
      <form {...formProps} className={containerClassName}>
        {body}
      </form>
    );
  }

  return (
    <div className={containerClassName}>
      {body}
    </div>
  );
}

export default FormContainer;
