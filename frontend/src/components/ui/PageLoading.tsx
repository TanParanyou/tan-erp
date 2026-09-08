"use client";

import React from "react";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useTranslations } from "next-intl";

export interface PageLoadingProps {
  text?: string;
  className?: string;
}

export function PageLoading({ text, className = "" }: PageLoadingProps) {
  const t = useTranslations("common.states");
  const displayText = text || t("loading");

  return (
    <div
      className={`flex items-center justify-center min-h-[360px] w-full bg-erp-surface ${className}`}
    >
      <MonoSpinner size="lg" label={displayText} />
    </div>
  );
}

export default PageLoading;
