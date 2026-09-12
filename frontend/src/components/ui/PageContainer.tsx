import React, { type ReactNode } from "react";
import { cn } from "@/lib/utils/cn";

export type PageContainerWidth = "wide" | "content" | "reading";

export interface PageContainerProps {
  children: ReactNode;
  className?: string;
  width?: PageContainerWidth;
  overlap?: boolean;
}

const widths: Record<PageContainerWidth, string> = {
  wide: "max-w-7xl",
  content: "max-w-6xl",
  reading: "max-w-3xl",
};

export function PageContainer({
  children,
  className = "",
  width = "wide",
  overlap = false,
}: PageContainerProps) {
  return (
    <div
      className={cn(
        "relative z-20 mx-auto w-full bg-erp-surface px-4 pb-16 pt-8 text-erp-text-main sm:px-6 md:pb-24 md:pt-12 rounded-none",
        widths[width],
        overlap && "-mt-8 md:-mt-10",
        className
      )}
    >
      {children}
    </div>
  );
}

PageContainer.displayName = "PageContainer";
