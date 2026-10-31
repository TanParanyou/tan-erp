"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { DashboardPage } from "@/features/dashboard/components/dashboard-page";

interface PageProps {
  params: Promise<{ locale: string }>;
}

export default function ReportsPage({ params }: PageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return <DashboardPage />;
}
