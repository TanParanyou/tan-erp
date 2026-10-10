"use client";

import { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import type { ItemResponse } from "@/lib/api/api-client";
import { useItemImageMutations, useItemImages } from "@/features/item-master/api/item-master-queries";
import { uploadVerifiedItemImage } from "@/features/item-master/api/upload-item-image";

export function ItemImageMaintenance({ itemId, item }: { itemId: string; item: ItemResponse }) {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const locale = useLocale() === "en" ? "en" : "th";
  const { selectedMembership } = useSelectedMembership();
  const images = useItemImages(itemId);
  const mutations = useItemImageMutations(itemId);
  const [file, setFile] = useState<File | null>(null);
  const [altText, setAltText] = useState(item.name?.thai ?? "");
  const [removeId, setRemoveId] = useState<string | null>(null);
  const [verifiedFileId, setVerifiedFileId] = useState<string | null>(null);
  const uploadKeyRef = useRef<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const editable = can(selectedMembership, "items.manage-images");
  const busy = uploading || mutations.attach.isPending || mutations.setPrimary.isPending || mutations.detach.isPending;

  const uploadAndAttach = async () => {
    if (!file || !selectedMembership?.id || busy) return;
    setUploading(true);
    try {
      const token = await getAuthToken();
      if (!token) throw new Error("Authentication required.");
      let fileId = verifiedFileId;
      if (!fileId) {
        uploadKeyRef.current ??= crypto.randomUUID();
        fileId = await uploadVerifiedItemImage({ file, itemId, token, membershipId: selectedMembership.id, locale, idempotencyKey: uploadKeyRef.current });
        setVerifiedFileId(fileId);
      }
      if (!fileId) throw new Error("Verified upload did not return a file identifier.");
      await mutations.attach.mutateAsync({ fileId, role: "gallery", isPrimary: (images.data?.length ?? 0) === 0, altText: { thai: altText.trim(), english: item.name?.english ?? null } });
      setFile(null);
      setVerifiedFileId(null);
      uploadKeyRef.current = null;
    } catch {
      // Keep the selected file so the user can retry after reviewing the error.
    } finally {
      setUploading(false);
    }
  };

  const remove = async () => {
    if (!removeId) return;
    try {
      await mutations.detach.mutateAsync(removeId);
      setRemoveId(null);
    } catch {
      // Keep the confirmation open when the server rejects the request.
    }
  };

  return <section className="space-y-4 border border-erp-border bg-erp-surface p-4 sm:p-6">
    <div><h2 className="text-lg font-semibold text-erp-ink">{t("images")}</h2><p className="text-sm text-erp-muted">{t("imageDeferredHelp")}</p></div>
    {images.isError && <p role="alert" className="text-sm text-erp-danger">{t("imagesLoadFailed")}</p>}
    {mutations.attach.isError && <p role="alert" className="text-sm text-erp-danger">{t("imageSaveFailed")}</p>}
    {images.isError ? null : images.data?.length ? <ul className="divide-y divide-erp-border">{images.data.map((image) => <li key={image.id} className="flex min-h-12 flex-wrap items-center justify-between gap-2 py-2"><span>{image.fileName ?? image.fileId}{image.isPrimary ? ` · ${t("primaryImage")}` : ""}</span><span className="flex gap-2">{editable && !image.isPrimary && image.id && <Button type="button" variant="outline" disabled={busy} onClick={() => { void mutations.setPrimary.mutateAsync(image.id!); }}>{t("makeImagePrimary")}</Button>}{editable && image.id && <Button type="button" variant="outline" disabled={busy} onClick={() => setRemoveId(image.id!)}>{t("removeImage")}</Button>}</span></li>)}</ul> : <p className="text-sm text-erp-muted">{t("noImages")}</p>}
    {editable && <div className="grid gap-3 border-t border-erp-border pt-4 sm:grid-cols-2"><Input type="file" label={t("imageFile")} accept="image/jpeg,image/png,image/webp" disabled={busy} onChange={(event) => { setFile(event.target.files?.[0] ?? null); setVerifiedFileId(null); uploadKeyRef.current = null; }} /><Input label={t("imageAltText")} value={altText} disabled={busy} onChange={(event) => setAltText(event.target.value)} placeholder={t("imageAltTextPlaceholder")} /><div className="sm:col-span-2"><Button type="button" disabled={busy || !file || !altText.trim()} isLoading={busy} onClick={() => { void uploadAndAttach(); }}>{t("uploadAndAttachImage")}</Button></div></div>}
    <ConfirmationModal isOpen={removeId !== null} onClose={() => { if (!busy) setRemoveId(null); }} onConfirm={() => { void remove(); }} title={t("removeImageTitle")} message={t("removeImageMessage")} confirmText={t("removeImage")} cancelText={common("actions.cancel")} isLoading={mutations.detach.isPending} />
  </section>;
}
