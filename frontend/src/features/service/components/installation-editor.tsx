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
import { useInstallationMutations } from "../api/service-queries";
import { serviceErrorCode } from "../service-status";

interface ChecklistLine {
  key: string;
  title: string;
  required: boolean;
}

const PAGE_LIMIT = 100;

export function InstallationEditor() {
  const t = useTranslations("service.installations");
  const tErrors = useTranslations("service.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const mutations = useInstallationMutations();
  const projects = useProjectList({ page: 1, pageSize: PAGE_LIMIT });

  const [projectId, setProjectId] = useState("");
  const [start, setStart] = useState("");
  const [end, setEnd] = useState("");
  const [crew, setCrew] = useState("");
  const [note, setNote] = useState("");
  const [lines, setLines] = useState<ChecklistLine[]>([]);
  const [newTitle, setNewTitle] = useState("");
  const [error, setError] = useState<string | null>(null);
  // One key per form session so a retried create replays instead of creating a second installation.
  const createKeyRef = useRef(crypto.randomUUID());
  const isBusy = mutations.create.isPending;

  function addLine(): void {
    const title = newTitle.trim();
    if (!title) return;
    setLines((current) => [...current, { key: crypto.randomUUID(), title, required: true }]);
    setNewTitle("");
  }

  async function save(): Promise<void> {
    setError(null);
    if (!projectId || !start || !end || end < start) {
      setError(t("fieldsInvalid"));
      return;
    }

    try {
      const saved = await mutations.create.mutateAsync({
        payload: {
          projectId,
          scheduledStart: start,
          scheduledEnd: end,
          crewName: crew.trim() || null,
          note: note.trim() || null,
          checklist: lines.map((line) => ({ title: line.title, required: line.required })),
        },
        idempotencyKey: createKeyRef.current,
      });
      toast.success(t("created"));
      router.push(`/${locale}/service/installations/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? serviceErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader
        title={t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/service/installations` }, { label: t("create") }]}
      />
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
        <Input label={t("crew")} value={crew} maxLength={200} disabled={isBusy} onChange={(event) => setCrew(event.target.value)} />
        <Input type="date" label={t("scheduledStart")} required value={start} disabled={isBusy} onChange={(event) => setStart(event.target.value)} />
        <Input type="date" label={t("scheduledEnd")} required value={end} disabled={isBusy} onChange={(event) => setEnd(event.target.value)} />
        <Input label={t("note")} value={note} maxLength={500} disabled={isBusy} onChange={(event) => setNote(event.target.value)} />
      </div>

      <div className="erp-card space-y-4 p-5">
        <h3 className="text-sm font-bold text-erp-navy">{t("checklistTitle")}</h3>
        <ul className="space-y-2">
          {lines.map((line) => (
            <li key={line.key} className="flex flex-wrap items-center gap-3 border border-erp-border bg-erp-surface-subtle p-3">
              <span className="min-w-48 flex-1 text-sm">{line.title}</span>
              <label className="flex items-center gap-2 text-xs">
                <input type="checkbox" className="h-4 w-4" checked={line.required} disabled={isBusy} onChange={(event) => setLines((current) => current.map((l) => (l.key === line.key ? { ...l, required: event.target.checked } : l)))} />
                {t("required")}
              </label>
              <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={isBusy} onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}>{t("removeLine")}</Button>
            </li>
          ))}
          {lines.length === 0 && <li className="text-xs text-erp-text-muted">{t("checklistEmpty")}</li>}
        </ul>
        <div className="flex flex-wrap items-end gap-3">
          <Input label={t("newChecklistItem")} value={newTitle} maxLength={200} disabled={isBusy} onChange={(event) => setNewTitle(event.target.value)} />
          <Button type="button" variant="outline" className="min-h-11" disabled={isBusy || !newTitle.trim()} onClick={addLine}>{t("addChecklistItem")}</Button>
        </div>
      </div>

      <div className="flex justify-end gap-3">
        <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => router.back()}>{tCommon("actions.cancel")}</Button>
        <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{tCommon("actions.save")}</Button>
      </div>
    </section>
  );
}
