"use client";

import { useMemo, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { IconPlus, IconTrash } from "@/components/common/Icons";
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

  const isDirty = useMemo(() => {
    return Boolean(projectId || start || end || crew || note || lines.length > 0 || newTitle);
  }, [projectId, start, end, crew, note, lines, newTitle]);

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
    <section className="space-y-6 pb-20">
      <PageHeader
        title={t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/service/installations` }, { label: t("create") }]}
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <FormSection title={t("title")}>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <Select
            label={t("project")}
            required
            value={projectId}
            placeholder={t("selectProject")}
            disabled={isBusy}
            options={(projects.data?.items ?? []).map((p) => ({ value: p.id ?? "", label: `${p.code ?? "-"} · ${p.name ?? "-"}` }))}
            onChange={(event) => setProjectId(event.target.value)}
          />
          <Input label={t("crew")} value={crew} maxLength={200} disabled={isBusy} placeholder={t("crewPlaceholder")} onChange={(event) => setCrew(event.target.value)} />
          <Input type="date" label={t("scheduledStart")} placeholder={t("datePlaceholder")} required value={start} disabled={isBusy} onChange={(event) => setStart(event.target.value)} />
          <Input type="date" label={t("scheduledEnd")} placeholder={t("datePlaceholder")} required value={end} disabled={isBusy} onChange={(event) => setEnd(event.target.value)} />
          <div className="md:col-span-2">
            <Input label={t("note")} value={note} maxLength={500} disabled={isBusy} placeholder={t("notePlaceholder")} onChange={(event) => setNote(event.target.value)} />
          </div>
        </div>
      </FormSection>

      <FormSection title={t("checklistTitle")}>
        <div className="space-y-4">
          <div className="erp-form-group !mb-0">
            <label className="erp-label !mb-1.5">{t("newChecklistItem")}</label>
            <div className="flex items-center gap-2">
              <div className="flex-1">
                <Input
                  wrapperClassName="!mb-0"
                  className="h-11 min-h-[44px]"
                  value={newTitle}
                  maxLength={200}
                  disabled={isBusy}
                  placeholder={t("newChecklistItemPlaceholder")}
                  onChange={(event) => setNewTitle(event.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      e.preventDefault();
                      addLine();
                    }
                  }}
                />
              </div>
              <Button
                type="button"
                variant="primary"
                size="md"
                className="h-11 min-h-[44px] shrink-0"
                icon={<IconPlus size={16} />}
                disabled={isBusy || !newTitle.trim()}
                onClick={addLine}
              >
                {t("addChecklistItem")}
              </Button>
            </div>
          </div>

          <div className="overflow-x-auto border border-erp-border bg-erp-surface">
            <table className="w-full text-left text-xs">
              <thead>
                <tr className="border-b border-erp-border bg-erp-surface-subtle text-erp-navy font-bold">
                  <th className="py-2.5 px-3 w-12 text-center">#</th>
                  <th className="py-2.5 px-3 min-w-[200px]">{t("checklistTitle")}</th>
                  <th className="py-2.5 px-3 w-28 text-center">{t("required")}</th>
                  <th className="py-2.5 px-3 w-14 text-center">
                    <span className="sr-only">{t("removeLine")}</span>
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-erp-border/60">
                {lines.map((line, index) => (
                  <tr key={line.key} className="align-middle hover:bg-erp-surface-subtle/50 transition-colors">
                    <td className="py-2 px-3 text-center font-mono text-erp-text-muted">
                      {index + 1}
                    </td>
                    <td className="py-2 px-3 text-sm font-medium text-erp-text-main">
                      {line.title}
                    </td>
                    <td className="py-2 px-3 text-center">
                      <Checkbox
                        checked={line.required}
                        disabled={isBusy}
                        onCheckedChange={(checked) =>
                          setLines((current) =>
                            current.map((l) => (l.key === line.key ? { ...l, required: checked === true } : l))
                          )
                        }
                        label=""
                        aria-label={t("required")}
                      />
                    </td>
                    <td className="py-2 px-3 text-center">
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        className="h-9 w-9 text-erp-danger hover:text-erp-danger hover:bg-erp-danger/10"
                        disabled={isBusy}
                        title={t("removeLine")}
                        aria-label={t("removeLine")}
                        icon={<IconTrash size={16} />}
                        onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}
                      />
                    </td>
                  </tr>
                ))}
                {lines.length === 0 && (
                  <tr>
                    <td colSpan={4} className="py-8 text-center text-sm text-erp-text-muted">
                      {t("checklistEmpty")}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </FormSection>

      <FormActionBar
        isEditMode={false}
        isDirty={isDirty}
        isLoading={isBusy}
        sticky
        saveButtonType="button"
        onSave={() => void save()}
        onCancel={() => router.back()}
      />
    </section>
  );
}
