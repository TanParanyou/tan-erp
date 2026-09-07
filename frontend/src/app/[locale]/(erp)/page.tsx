"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";

interface ErpHomePageProps {
  params: Promise<{ locale: string }>;
}

export default function ErpHomePage({ params }: ErpHomePageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  // Children is null here because default dashboard view is rendered by ErpShell when children is empty/null
  return null;
}
