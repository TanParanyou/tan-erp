"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { AccessGate } from "@/features/auth";
import { ErpShell } from "@/components/layout/erp-shell";
import { isSupportedLocale } from "@/lib/i18n/locales";

interface ErpHomePageProps {
  params: Promise<{ locale: string }>;
}

export default function ErpHomePage({ params }: ErpHomePageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <AccessGate>
      {(currentUser) => (
        <ErpShell currentUser={currentUser} />
      )}
    </AccessGate>
  );
}
