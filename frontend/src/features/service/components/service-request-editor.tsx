"use client";

import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useProjectList } from "@/features/projects/api/project-queries";
import { useServiceRequestMutations } from "../api/service-queries";
import { SERVICE_PRIORITIES, serviceErrorCode } from "../service-status";

const PAGE_LIMIT = 100;

export function ServiceRequestEditor() {
  const t = useTranslations("service.requests");
  const tErrors = useTranslations("service.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const mutations = useServiceRequestMutations();
  const projects = useProjectList({ page: 1, pageSize: PAGE_LIMIT });

  const [projectId, setProjectId] = useState("");
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [priority, setPriority] = useState("normal");
  const [error, setError] = useState<string | null>(null);
  // One key per form session so a retried create replays instead of creating a second request.
  const createKeyRef = useRef(crypto.randomUUID());
  const isBusy = mutations.create.isPending;

  async function save(): Promise<void> {
    setError(null);
    if (!projectId || !title.trim() || !description.trim()) {
      setError(t("fieldsInvalid"));
      return;
    }

    try {
      const saved = await mutations.create.mutateAsync({
        payload: { projectId, title: title.trim(), description: description.trim(), priority },
        idempotencyKey: createKeyRef.current,
      });
      toast.success(t("created"));
      router.push(`/${locale}/service/requests/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? serviceErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader title={t("createTitle")} subtitle={t("editorSubtitle")} breadcrumbs={[{ label: t("title"), href: `/${locale}/service/requests` }, { label: t("create") }]} />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
      <div className="erp-card grid grid-cols-1 gap-4 p-5 md:grid-cols-2">
        <Select
          label={t("project")}
          required
          value={projectId}
          placeholder={t("selectProject")}
          disabled={isBusy}
          options={(projects.data?.items ?? []).map((p) => ({ value: p.id ?? "", label: `${p.code ?? "-"} · ${p.name ?? "-"}` }))}
          onChange={(event) => setProjectId(event.target.value)}
        />
        <Select label={t("priority")} value={priority} disabled={isBusy} options={SERVICE_PRIORITIES.map((value) => ({ value, label: t(`priorities.${value}`) }))} onChange={(event) => setPriority(event.target.value)} />
        <Input label={t("titleField")} required value={title} maxLength={200} disabled={isBusy} onChange={(event) => setTitle(event.target.value)} />
        <Input label={t("description")} required value={description} maxLength={1000} disabled={isBusy} onChange={(event) => setDescription(event.target.value)} />
      </div>
      <div className="flex justify-end gap-3">
        <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => router.back()}>{tCommon("actions.cancel")}</Button>
        <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{tCommon("actions.save")}</Button>
      </div>
    </section>
  );
}
