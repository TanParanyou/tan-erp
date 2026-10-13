"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ApiError } from "@/lib/api/api-error";
import type { QuotationHistoryItemResponse } from "@/lib/api/api-client";
import { useQuotationDocumentDownload } from "../api/quotation-lifecycle-queries";
import { quotationLifecycleErrorCode } from "../quotation-lifecycle-status";

const HASH_PREFIX_LENGTH = 16;

/** The stored PDF of one quotation: download it and see the start of its SHA-256. Quotations issued before PDFs were stored have none. */
export function QuotationDocumentActions({ item }: { item: QuotationHistoryItemResponse }) {
  const t = useTranslations("quotationLifecycle.document");
  const tErrors = useTranslations("quotationLifecycle.errors");
  const download = useQuotationDocumentDownload();
  const [message, setMessage] = useState<string | null>(null);

  const document = item.document ?? null;
  if (!item.id || !item.number || !document) return null;
  const quotationId = item.id;
  const fileName = `${item.number}.pdf`;

  async function onDownload(): Promise<void> {
    setMessage(null);
    try {
      await download.mutateAsync({ quotationId, fileName });
    } catch (err: unknown) {
      const code = err instanceof ApiError ? quotationLifecycleErrorCode(err.code) : null;
      setMessage(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <div className="mt-2 space-y-2">
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}
      <div className="flex flex-wrap items-center gap-3">
        <Button type="button" size="sm" variant="outline" isLoading={download.isPending} disabled={download.isPending} onClick={() => void onDownload()}>
          {t("download")}
        </Button>
        <span className="font-mono text-erp-text-muted" title={document.sha256 ?? undefined}>
          {t("hash", { hash: document.sha256 ? document.sha256.slice(0, HASH_PREFIX_LENGTH) : "-" })}
        </span>
      </div>
    </div>
  );
}
