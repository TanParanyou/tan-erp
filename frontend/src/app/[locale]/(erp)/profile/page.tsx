"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { ProfilePageContent } from "@/features/profile/components/profile-page-content";

interface ProfilePageProps {
  params: Promise<{ locale: string }>;
}

export default function ProfilePage({ params }: ProfilePageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return <ProfilePageContent />;
}
