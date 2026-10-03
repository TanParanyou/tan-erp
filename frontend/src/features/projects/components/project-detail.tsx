"use client";

import Link from "next/link";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { Alert } from "@/components/ui/Alert";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { formatCurrency, formatDate, formatDateTime } from "@/lib/formatters/formatters";
import { useProject } from "../api/project-queries";
import { isProjectStatus, projectStatusVariant } from "../project-status";

interface ProjectDetailProps {
  projectId: string;
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="flex flex-col gap-0.5">
      <dt className="text-xs text-erp-text-muted">{label}</dt>
      <dd className="text-sm font-medium text-erp-text-main break-words">{children}</dd>
    </div>
  );
}

export function ProjectDetail({ projectId }: ProjectDetailProps) {
  const t = useTranslations("projects");
  const locale = useLocale();
  const { data: project, isLoading, isError } = useProject(projectId);

  if (isLoading) {
    return (
      <div className="py-16 flex justify-center" role="status">
        <MonoSpinner size="md" />
      </div>
    );
  }

  if (isError || !project) {
    return <Alert variant="danger">{t("loadError")}</Alert>;
  }

  const customerName = locale === "en" ? project.customer?.displayNameEn ?? project.customer?.displayNameTh : project.customer?.displayNameTh;
  const status = project.status;

  return (
    <section className="space-y-5">
      <PageHeader
        title={`${project.code ?? "-"} — ${project.name ?? "-"}`}
        subtitle={t("detailSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/projects` }, { label: project.code ?? "-" }]}
        actions={<StatusBadge label={isProjectStatus(status) ? t(`statuses.${status}`) : "-"} variant={projectStatusVariant(status)} />}
      />

      <div className="erp-card p-5">
        <h3 className="mb-4 text-sm font-bold text-erp-navy">{t("overview")}</h3>
        <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <Field label={t("customer")}>{customerName ?? "-"}</Field>
          <Field label={t("site")}>{project.site?.label ?? "-"}</Field>
          <Field label={t("owner")}>{project.owner?.displayName ?? "-"}</Field>
          <Field label={t("plannedStartDate")}>{project.plannedStartDate ? formatDate(project.plannedStartDate, locale) : "-"}</Field>
          <Field label={t("opportunity")}>
            {project.opportunity?.id ? (
              <Link href={`/${locale}/opportunities/${project.opportunity.id}`} className="font-mono text-erp-navy underline">
                {project.opportunity.code ?? "-"}
              </Link>
            ) : "-"}
          </Field>
          <Field label={t("createdAt")}>{project.createdAtUtc ? formatDateTime(project.createdAtUtc, locale) : "-"}</Field>
        </dl>
      </div>

      <div className="erp-card p-5">
        <h3 className="text-sm font-bold text-erp-navy">{t("baselineTitle")}</h3>
        <p className="mb-4 mt-1 text-xs text-erp-text-muted">{t("baselineDesc")}</p>
        <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <Field label={t("quotationNumber")}>
            <span className="font-mono">{project.baseline?.quotationNumber ?? "-"}</span>
          </Field>
          <Field label={t("contractAmount")}>
            <span className="font-mono">{formatCurrency(project.baseline?.contractAmount, "THB", locale)}</span>
          </Field>
          <Field label={t("baselineHash")}>
            <span className="font-mono text-xs">{project.baseline?.baselineHash?.slice(0, 16) ?? "-"}</span>
          </Field>
          <Field label={t("quotationHash")}>
            <span className="font-mono text-xs">{project.baseline?.quotationSnapshotHash ?? "-"}</span>
          </Field>
          <Field label={t("surveyHash")}>
            <span className="font-mono text-xs">{project.baseline?.siteSurveySnapshotHash ?? "-"}</span>
          </Field>
        </dl>
      </div>
    </section>
  );
}
