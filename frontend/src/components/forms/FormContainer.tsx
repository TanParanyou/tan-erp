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
  /** Maximum width of the form body */
  maxWidth?: "sm" | "md" | "lg" | "xl" | "full" | string;
  className?: string;
  contentClassName?: string;
  /** Whether to render the root container as a <form> element. Defaults to false (renders <div>) */
  asForm?: boolean;
}

const maxWidthMap: Record<string, string> = {
  sm: "max-w-xl",
  md: "max-w-[800px]",
  lg: "max-w-4xl",
  xl: "max-w-6xl",
  full: "max-w-full",
};

export function FormContainer({
  header,
  errorBanner,
  topAlert,
  children,
  actionBar,
  maxWidth = "md",
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
          "w-full mx-auto pb-12 flex flex-col gap-6",
          resolvedMaxWidth,
          contentClassName
        )}
      >
        {header}
        {errorBanner}
        {topAlert}
        {children}
      </div>

      {/* Action Bar Slot (Flush to edges and bottom) */}
      {actionBar && (
        <div className="-mx-4 sm:-mx-8">
          {actionBar}
        </div>
      )}
    </>
  );

  const containerClassName = cn(
    "flex-1 flex flex-col justify-between w-full min-h-[calc(100vh-64px)]",
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
