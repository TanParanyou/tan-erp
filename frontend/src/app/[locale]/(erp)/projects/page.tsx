"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { ProjectList } from "@/features/projects/components/project-list";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface ProjectsPageProps {
  params: Promise<{ locale: string }>;
}

export default function ProjectsPage({ params }: ProjectsPageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("projects");

  return (
    <PermissionGuard permission={PERMISSIONS.PROJECTS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <ProjectList />
    </PermissionGuard>
  );
}
