"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { AccessGate } from "@/features/auth";
import { ErpShell } from "@/components/layout/erp-shell";
import { SelectedMembershipProvider } from "@/lib/membership/selected-membership-context";
import { isSupportedLocale } from "@/lib/i18n/locales";

interface ErpLayoutProps {
  children: React.ReactNode;
  params: Promise<{ locale: string }>;
}

export default function ErpLayout({ children, params }: ErpLayoutProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return (
    <AccessGate>
      {(currentUser) => (
        <SelectedMembershipProvider currentUser={currentUser}>
          <ErpShell currentUser={currentUser}>
            {children}
          </ErpShell>
        </SelectedMembershipProvider>
      )}
    </AccessGate>
  );
}
