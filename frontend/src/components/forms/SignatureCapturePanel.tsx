"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { useCaptureSignature, useSignatureCaptures } from "@/hooks/useAttachments";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import {
  SIGNATURE_CONSENT_VERSIONS,
  attachmentErrorCode,
  type AttachmentOwnerType,
  type SignaturePurpose,
} from "@/lib/attachments/attachment-owner-types";
import { formatDateTime } from "@/lib/formatters/formatters";
import { dataUrlToPngFile } from "@/lib/media/data-url-to-file";
import { SignaturePad } from "./SignaturePad";

export interface SignatureCapturePanelProps {
  ownerType: AttachmentOwnerType;
  ownerId: string;
  purpose: SignaturePurpose;
  /** Whether the viewer may record a signature (the server still enforces permission and owner state). */
  canCapture: boolean;
}

const HASH_PREVIEW_LENGTH = 12;

/**
 * Captured signatures of an owner record plus a form to record a new one.
 * The signature PNG is uploaded only on submit; a failed capture keeps the uploaded image so a retry does not upload it again.
 * Evidence is an image plus SHA-256, not a legal electronic signature.
 */
export function SignatureCapturePanel({ ownerType, ownerId, purpose, canCapture }: SignatureCapturePanelProps) {
  const t = useTranslations("attachments.signature");
  const tErrors = useTranslations("attachments.errors");
  const locale = useLocale();
  const { toast } = useToast();
  const { buildOptions } = useApiRequestContext();
  const signatures = useSignatureCaptures(ownerType, ownerId);
  const capture = useCaptureSignature(ownerType, ownerId);
  const uploader = useDeferredFileUpload({ parentType: ownerType, parentId: ownerId });

  const consentVersion = SIGNATURE_CONSENT_VERSIONS[purpose];
  const keyRef = useRef<string | null>(null);
  const [signerName, setSignerName] = useState("");
  const [signerRole, setSignerRole] = useState("");
  const [consent, setConsent] = useState(false);
  const [signature, setSignature] = useState<string | null>(null);
  const [uploadedFileId, setUploadedFileId] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSave = signerName.trim().length >= 2 && consent && signature !== null && !isSaving;

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? attachmentErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  function handleSignatureChange(value: string | null): void {
    setSignature(value);
    setUploadedFileId(null); // a different drawing is a different image
    keyRef.current = null;
  }

  async function handleSave(): Promise<void> {
    if (!canSave || signature === null) return;
    setIsSaving(true);
    setError(null);
    try {
      let imageFileId = uploadedFileId;
      if (imageFileId === null) {
        const options = await buildOptions();
        const uploaded = await uploader.uploadSingleFile(dataUrlToPngFile(signature, "signature.png"), {
          token: options.token,
          membershipId: options.membershipId,
          locale: options.locale,
        });
        if (!uploaded.fileId) {
          setError(tErrors("failed"));
          return;
        }

        imageFileId = uploaded.fileId;
        setUploadedFileId(imageFileId);
        keyRef.current = null;
      }

      keyRef.current ??= `signature-${crypto.randomUUID()}`;
      await capture.mutateAsync({
        payload: {
          purpose,
          signerName: signerName.trim(),
          signerRole: signerRole.trim() === "" ? null : signerRole.trim(),
          imageFileId,
          consentAccepted: consent,
          consentTextVersion: consentVersion,
        },
        idempotencyKey: keyRef.current,
      });
      toast.success(t("saved"));
      setSignerName("");
      setSignerRole("");
      setConsent(false);
      setSignature(null);
      setUploadedFileId(null);
      keyRef.current = null;
    } catch (err: unknown) {
      setError(describe(err));
    } finally {
      setIsSaving(false);
    }
  }

  const items = signatures.data?.items ?? [];

  return (
    <section className="erp-card flex flex-col gap-4 p-5" aria-labelledby={`signatures-${ownerId}`}>
      <div className="flex flex-col gap-1 border-b border-erp-border pb-3">
        <h3 id={`signatures-${ownerId}`} className="text-base font-bold text-erp-navy">{t("title")}</h3>
        <p className="text-xs text-erp-text-muted">{t("subtitle")}</p>
      </div>

      {signatures.isLoading ? (
        <div className="flex justify-center py-6" role="status"><MonoSpinner size="md" /></div>
      ) : signatures.isError ? (
        <Alert variant="danger">{t("loadError")}</Alert>
      ) : items.length === 0 ? (
        <p className="text-sm text-erp-text-muted">{t("empty")}</p>
      ) : (
        <ul className="divide-y divide-erp-border border border-erp-border">
          {items.map((item) => (
            <li key={item.id} className="flex items-center gap-3 p-3">
              <AuthenticatedFileImage fileId={item.imageFileId ?? ""} alt={item.signerName ?? ""} className="h-16 w-28 border border-erp-border bg-white object-contain" />
              <div className="min-w-0 flex-1">
                <p className="text-sm font-semibold text-erp-navy">
                  {t("signedBy", { name: item.signerName ?? "-", date: item.signedAtUtc ? formatDateTime(item.signedAtUtc, locale) : "-" })}
                </p>
                {item.signerRole && <p className="text-xs text-erp-text-muted">{item.signerRole}</p>}
                <p className="font-mono text-[11px] text-erp-text-muted">{t("hashLabel", { hash: (item.contentHash ?? "").slice(0, HASH_PREVIEW_LENGTH) })}</p>
              </div>
            </li>
          ))}
        </ul>
      )}

      {canCapture && (
        <div className="flex flex-col gap-4 border-t border-erp-border pt-4">
          {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
          <Input
            label={t("signerName")}
            required
            value={signerName}
            maxLength={200}
            placeholder={t("signerNamePlaceholder")}
            disabled={isSaving}
            onChange={(event) => { setSignerName(event.target.value); keyRef.current = null; }}
          />
          <Input
            label={t("signerRole")}
            value={signerRole}
            maxLength={100}
            placeholder={t("signerRolePlaceholder")}
            disabled={isSaving}
            onChange={(event) => { setSignerRole(event.target.value); keyRef.current = null; }}
          />
          <SignaturePad value={signature} onChange={handleSignatureChange} label={t("signaturePadLabel")} />
          <Checkbox
            checked={consent}
            disabled={isSaving}
            aria-label={t("consentLabel")}
            onChange={(event) => { setConsent(event.target.checked); keyRef.current = null; }}
            label={t("consentLabel")}
            description={t(`consentVersions.${consentVersion}`)}
          />
          <div>
            <Button type="button" variant="primary" className="min-h-11" isLoading={isSaving} disabled={!canSave} onClick={() => void handleSave()}>
              {t("capture")}
            </Button>
          </div>
        </div>
      )}
    </section>
  );
}
