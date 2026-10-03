"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { ProjectDetail } from "@/features/projects/components/project-detail";
import { PermissionGuard } from "@/components/auth";
import { PERMISSIONS } from "@/lib/permissions/permissions";
import { useTranslations } from "next-intl";

interface ProjectDetailPageProps {
  params: Promise<{ locale: string; id: string }>;
}

export default function ProjectDetailPage({ params }: ProjectDetailPageProps) {
  const { locale, id } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  const t = useTranslations("projects");

  return (
    <PermissionGuard permission={PERMISSIONS.PROJECTS_READ} title={t("accessDeniedTitle")} detail={t("accessDeniedDetail")}>
      <ProjectDetail projectId={id} />
    </PermissionGuard>
  );
}
