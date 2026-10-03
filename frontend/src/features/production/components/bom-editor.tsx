"use client";

import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { PageHeader } from "@/components/layout/PageHeader";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import type { BomRevisionResponse } from "@/lib/api/api-client";
import { useBom, useBomMutations } from "../api/production-queries";
import { productionErrorCode } from "../production-status";

interface DraftLine {
  key: string;
  itemId: string;
  label: string;
  quantity: string;
  scrapPercent: string;
}

interface BomEditorProps {
  /** Existing BOM when editing a draft revision; omit to create a new BOM. */
  bomId?: string;
  revisionId?: string;
}

function fromRevision(revision: BomRevisionResponse): DraftLine[] {
  return (revision.lines ?? []).map((line) => ({
    key: line.id ?? crypto.randomUUID(),
    itemId: line.component?.id ?? "",
    label: `${line.component?.code ?? "-"} · ${line.component?.nameTh ?? "-"} (${line.component?.unitCode ?? "-"})`,
    quantity: String(line.quantity ?? 1),
    scrapPercent: String(line.scrapPercent ?? 0),
  }));
}

export function BomEditor({ bomId, revisionId }: BomEditorProps) {
  const query = useBom(bomId);
  const t = useTranslations("production.boms");
  if (bomId && query.isLoading) {
    return <div className="py-16 flex justify-center" role="status"><MonoSpinner size="md" /></div>;
  }

  if (bomId && (query.isError || !query.data)) {
    return <Alert variant="danger">{t("loadError")}</Alert>;
  }

  const revision = query.data?.revisions?.find((r) => r.id === revisionId);
  if (bomId && (!revision || revision.status !== "draft")) {
    return <Alert variant="warning">{t("onlyDraftEditable")}</Alert>;
  }

  // Keyed by the loaded version so the form is rebuilt from server data, never patched in an effect.
  return <EditorForm key={revision?.rowVersion ?? "new"} bomId={bomId} bomCode={query.data?.code ?? undefined} fixedItemLabel={query.data?.item ? `${query.data.item.code ?? "-"} · ${query.data.item.nameTh ?? "-"}` : undefined} revision={revision} />;
}

interface EditorFormProps {
  bomId?: string;
  bomCode?: string;
  fixedItemLabel?: string;
  revision?: BomRevisionResponse;
}

function EditorForm({ bomId, bomCode, fixedItemLabel, revision }: EditorFormProps) {
  const t = useTranslations("production.boms");
  const tErrors = useTranslations("production.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const mutations = useBomMutations();

  const [itemId, setItemId] = useState("");
  const [outputQuantity, setOutputQuantity] = useState(String(revision?.outputQuantity ?? 1));
  const [note, setNote] = useState(revision?.note ?? "");
  const [lines, setLines] = useState<DraftLine[]>(revision ? fromRevision(revision) : []);
  const [pickerKey, setPickerKey] = useState(0);
  const [error, setError] = useState<string | null>(null);
  // One key per form session so a retried create replays instead of creating a second BOM.
  const createKeyRef = useRef(crypto.randomUUID());

  const isBusy = mutations.create.isPending || mutations.updateDraft.isPending;

  function updateLine(key: string, patch: Partial<DraftLine>): void {
    setLines((current) => current.map((line) => (line.key === key ? { ...line, ...patch } : line)));
  }

  async function save(): Promise<void> {
    setError(null);
    if (!bomId && !itemId) {
      setError(t("itemRequired"));
      return;
    }

    const output = Number(outputQuantity);
    const parsed = lines.map((line) => ({ componentItemId: line.itemId, quantity: Number(line.quantity), scrapPercent: Number(line.scrapPercent) }));
    if (!(output > 0) || parsed.length === 0 || parsed.some((l) => !(l.quantity > 0) || !(l.scrapPercent >= 0 && l.scrapPercent <= 50))) {
      setError(t("linesInvalid"));
      return;
    }

    try {
      const saved = bomId && revision
        ? await mutations.updateDraft.mutateAsync({ id: bomId, revisionId: revision.id ?? "", rowVersion: revision.rowVersion ?? "", payload: { outputQuantity: output, note: note.trim() || null, lines: parsed } })
        : await mutations.create.mutateAsync({ payload: { itemId, outputQuantity: output, note: note.trim() || null, lines: parsed }, idempotencyKey: createKeyRef.current });
      toast.success(bomId ? t("updated") : t("created"));
      router.push(`/${locale}/production/boms/${saved.id}`);
    } catch (err: unknown) {
      const code = err instanceof ApiError ? productionErrorCode(err.code) : null;
      setError(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <section className="space-y-5">
      <PageHeader
        title={bomId ? t("editTitle", { code: bomCode ?? "-", revision: revision?.revisionNo ?? 0 }) : t("createTitle")}
        subtitle={t("editorSubtitle")}
        breadcrumbs={[{ label: t("title"), href: `/${locale}/production/boms` }, { label: bomCode ?? t("create") }]}
      />
      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      <div className="erp-card p-5 grid grid-cols-1 gap-4 md:grid-cols-2">
        {bomId ? (
          <div><div className="text-xs text-erp-text-muted">{t("item")}</div><div className="text-sm font-medium">{fixedItemLabel ?? "-"}</div></div>
        ) : (
          <ItemAutocomplete label={t("item")} required producibleOnly stockableOnly disabled={isBusy} value={itemId} onChange={(id) => setItemId(id)} />
        )}
        <Input type="number" min={0} step="0.0001" label={t("outputQuantity")} value={outputQuantity} disabled={isBusy} onChange={(event) => setOutputQuantity(event.target.value)} />
        <Input label={t("note")} value={note} maxLength={500} disabled={isBusy} onChange={(event) => setNote(event.target.value)} />
      </div>

      <div className="erp-card p-5 space-y-4">
        <h3 className="text-sm font-bold text-erp-navy">{t("componentsTitle")}</h3>
        <ul className="space-y-2">
          {lines.map((line) => (
            <li key={line.key} className="grid grid-cols-1 gap-2 border border-erp-border bg-erp-surface-subtle p-3 sm:grid-cols-[1fr_130px_130px_auto] sm:items-end">
              <div className="text-sm font-medium text-erp-text-main">{line.label}</div>
              <Input type="number" min={0} step="0.0001" aria-label={t("quantity")} placeholder={t("quantity")} value={line.quantity} disabled={isBusy} onChange={(event) => updateLine(line.key, { quantity: event.target.value })} />
              <Input type="number" min={0} max={50} step="0.01" aria-label={t("scrapPercent")} placeholder={t("scrapPercent")} value={line.scrapPercent} disabled={isBusy} onChange={(event) => updateLine(line.key, { scrapPercent: event.target.value })} />
              <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={isBusy} onClick={() => setLines((current) => current.filter((l) => l.key !== line.key))}>{t("removeLine")}</Button>
            </li>
          ))}
          {lines.length === 0 && <li className="text-xs text-erp-text-muted">{t("linesEmpty")}</li>}
        </ul>

        <ItemAutocomplete
          key={pickerKey}
          label={t("addComponent")}
          stockableOnly
          disabled={isBusy}
          onChange={(componentId, item) => {
            if (!componentId || lines.some((l) => l.itemId === componentId)) {
              setPickerKey((k) => k + 1);
              return;
            }

            const name = (locale === "en" ? item?.name?.english : item?.name?.thai) ?? "-";
            setLines((current) => [...current, {
              key: crypto.randomUUID(),
              itemId: componentId,
              label: `${item?.code ?? "-"} · ${name} (${item?.baseUnit?.code ?? "-"})`,
              quantity: "1",
              scrapPercent: "0",
            }]);
            setPickerKey((k) => k + 1);
          }}
        />
      </div>

      <div className="flex justify-end gap-3">
        <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => router.back()}>{tCommon("actions.cancel")}</Button>
        <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{tCommon("actions.save")}</Button>
      </div>
    </section>
  );
}
