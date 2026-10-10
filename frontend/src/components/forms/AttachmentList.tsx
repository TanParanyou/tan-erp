"use client";

import { useRef, useState, type ChangeEvent } from "react";
import { useLocale, useTranslations } from "next-intl";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { IconTrash, IconUpload } from "@/components/common/Icons";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { useAttachmentLinks, useAttachmentMutations } from "@/hooks/useAttachments";
import { useDeferredFileUpload, type UploadedFileResult } from "@/hooks/useDeferredFileUpload";
import { useToast } from "@/hooks/useToast";
import type { AttachmentLinkResponse } from "@/lib/api/api-client";
import { ApiError } from "@/lib/api/api-error";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import {
  ATTACHMENT_ACCEPTED_MEDIA_TYPES,
  ATTACHMENT_MAX_FILE_BYTES,
  attachmentErrorCode,
  isAttachmentPurpose,
  type AttachmentOwnerType,
  type AttachmentPurpose,
} from "@/lib/attachments/attachment-owner-types";
import { formatDateTime } from "@/lib/formatters/formatters";

export interface AttachmentListProps {
  ownerType: AttachmentOwnerType;
  ownerId: string;
  /** Whether the viewer may add or remove attachments (the server still enforces permission and owner state). */
  canManage: boolean;
  /** Purposes the user can choose from; the first one is the default. */
  purposes?: readonly AttachmentPurpose[];
}

const DEFAULT_PURPOSES: readonly AttachmentPurpose[] = ["general"];

function isAcceptedImage(file: File): boolean {
  return ATTACHMENT_ACCEPTED_MEDIA_TYPES.some((type) => type === file.type) && file.size > 0 && file.size <= ATTACHMENT_MAX_FILE_BYTES;
}

/**
 * Shared attachment list for any registered owner type. Files are uploaded only when the user saves
 * (deferred upload); a failed link keeps the already-uploaded files so a retry does not upload them again.
 */
export function AttachmentList({ ownerType, ownerId, canManage, purposes = DEFAULT_PURPOSES }: AttachmentListProps) {
  const t = useTranslations("attachments");
  const tCommon = useTranslations("common.actions");
  const locale = useLocale();
  const { toast } = useToast();
  const { buildOptions } = useApiRequestContext();
  const links = useAttachmentLinks(ownerType, ownerId);
  const { attach, unlink } = useAttachmentMutations(ownerType, ownerId);
  const uploader = useDeferredFileUpload({ parentType: ownerType, parentId: ownerId });

  const inputRef = useRef<HTMLInputElement | null>(null);
  const attachKeyRef = useRef<string | null>(null);
  const [pending, setPending] = useState<File[]>([]);
  const [uploaded, setUploaded] = useState<UploadedFileResult[]>([]);
  const [purpose, setPurpose] = useState<AttachmentPurpose>(purposes[0] ?? "general");
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [toRemove, setToRemove] = useState<AttachmentLinkResponse | null>(null);

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? attachmentErrorCode(err.code) : null;
    return code ? t(`errors.${code}`) : t("errors.failed");
  }

  function handleSelect(event: ChangeEvent<HTMLInputElement>): void {
    const selected = Array.from(event.target.files ?? []);
    event.target.value = "";
    const rejected = selected.find((file) => !isAcceptedImage(file));
    setError(rejected ? t("fileRejected", { name: rejected.name }) : null);
    const accepted = selected.filter(isAcceptedImage);
    if (accepted.length > 0) setPending((current) => [...current, ...accepted]);
  }

  async function handleSave(): Promise<void> {
    if (isSaving || (pending.length === 0 && uploaded.length === 0)) return;
    setIsSaving(true);
    setError(null);
    try {
      let ready = uploaded;
      if (pending.length > 0) {
        const options = await buildOptions();
        const result = await uploader.uploadFiles(pending, { token: options.token, membershipId: options.membershipId, locale: options.locale });
        ready = [...uploaded, ...result.files];
        setUploaded(ready);
        setPending([]);
        attachKeyRef.current = null; // a new file list needs a new idempotency key
      }

      attachKeyRef.current ??= `attach-${crypto.randomUUID()}`;
      await attach.mutateAsync({ payload: { purpose, fileIds: ready.map((file) => file.fileId) }, idempotencyKey: attachKeyRef.current });
      setUploaded([]);
      attachKeyRef.current = null;
      toast.success(t("saved"));
    } catch (err: unknown) {
      setError(describe(err));
    } finally {
      setIsSaving(false);
    }
  }

  async function handleUnlink(): Promise<void> {
    if (!toRemove?.id) return;
    try {
      await unlink.mutateAsync(toRemove.id);
      toast.success(t("unlinked"));
    } catch (err: unknown) {
      toast.error(describe(err));
    } finally {
      setToRemove(null);
    }
  }

  const items = links.data?.items ?? [];
  const waitingCount = pending.length + uploaded.length;

  return (
    <section className="erp-card flex flex-col gap-4 p-5" aria-labelledby={`attachments-${ownerId}`}>
      <div className="flex flex-col gap-1 border-b border-erp-border pb-3">
        <div className="flex items-center gap-2">
          <h3 id={`attachments-${ownerId}`} className="text-base font-bold text-erp-navy">{t("title")}</h3>
          <span className="border border-erp-border bg-erp-surface-muted px-1.5 py-0.5 font-mono text-[11px] font-semibold text-erp-navy">{items.length}</span>
        </div>
        <p className="text-xs text-erp-text-muted">{t("subtitle")}</p>
      </div>

      {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}

      {links.isLoading ? (
        <div className="flex justify-center py-8" role="status" aria-label={t("loading")}><MonoSpinner size="md" /></div>
      ) : links.isError ? (
        <Alert variant="danger">{t("loadError")}</Alert>
      ) : items.length === 0 ? (
        <p className="py-4 text-sm text-erp-text-muted">{t("empty")}</p>
      ) : (
        <ul className="divide-y divide-erp-border border border-erp-border">
          {items.map((item) => (
            <li key={item.id} className="flex items-center gap-3 p-3">
              <AuthenticatedFileImage fileId={item.fileId ?? ""} alt={item.filename ?? ""} className="h-16 w-16 border border-erp-border object-cover" />
              <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-semibold text-erp-navy">{item.filename}</p>
                <p className="text-xs text-erp-text-muted">
                  {item.purpose && isAttachmentPurpose(item.purpose) ? t(`purposes.${item.purpose}`) : "-"}
                </p>
                <p className="flex flex-wrap gap-x-2 text-xs text-erp-text-muted">
                  <span>{t("createdBy", { name: item.createdBy?.displayName ?? "-" })}</span>
                  <span>{item.createdAtUtc ? formatDateTime(item.createdAtUtc, locale) : "-"}</span>
                </p>
              </div>
              {canManage && (
                <Button type="button" size="sm" variant="outline" className="min-h-11 min-w-11" aria-label={t("unlink.action")} onClick={() => setToRemove(item)}>
                  <IconTrash size={16} />
                </Button>
              )}
            </li>
          ))}
        </ul>
      )}

      {canManage && (
        <div className="flex flex-col gap-3 border-t border-erp-border pt-4">
          {purposes.length > 1 && (
            <Select
              label={t("purposeLabel")}
              value={purpose}
              disabled={isSaving}
              options={purposes.map((value) => ({ value, label: t(`purposes.${value}`) }))}
              onChange={(event) => {
                const next = event.target.value;
                if (isAttachmentPurpose(next)) {
                  setPurpose(next);
                  attachKeyRef.current = null; // a different payload needs a new idempotency key
                }
              }}
            />
          )}

          <input
            ref={inputRef}
            type="file"
            multiple
            accept={ATTACHMENT_ACCEPTED_MEDIA_TYPES.join(",")}
            aria-label={t("selectFiles")}
            className="sr-only"
            onChange={handleSelect}
          />
          <div className="flex flex-wrap items-center gap-3">
            <Button type="button" variant="outline" className="min-h-11" icon={<IconUpload size={16} />} disabled={isSaving} onClick={() => inputRef.current?.click()}>
              {t("selectFiles")}
            </Button>
            {waitingCount > 0 && (
              <Button type="button" variant="primary" className="min-h-11" isLoading={isSaving} disabled={isSaving} onClick={() => void handleSave()}>
                {t("save")}
              </Button>
            )}
          </div>

          {pending.length > 0 && (
            <div>
              <p className="text-xs font-semibold text-erp-text-muted">{t("pendingTitle", { count: pending.length })}</p>
              <ul className="mt-1 divide-y divide-erp-border border border-erp-border">
                {pending.map((file, index) => (
                  <li key={`${file.name}-${index}`} className="flex items-center justify-between gap-3 px-3 py-2 text-sm">
                    <span className="truncate">{file.name}</span>
                    <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={isSaving} onClick={() => setPending((current) => current.filter((_, i) => i !== index))}>
                      {t("removePending")}
                    </Button>
                  </li>
                ))}
              </ul>
            </div>
          )}
          {uploaded.length > 0 && <p className="text-xs text-erp-text-muted">{t("uploadedWaiting", { count: uploaded.length })}</p>}
        </div>
      )}

      <ConfirmationModal
        isOpen={toRemove !== null}
        onClose={() => { if (!unlink.isPending) setToRemove(null); }}
        onConfirm={handleUnlink}
        title={t("unlink.title")}
        message={t("unlink.message")}
        confirmText={tCommon("delete")}
        cancelText={tCommon("cancel")}
        variant="danger"
        isLoading={unlink.isPending}
      />
    </section>
  );
}
