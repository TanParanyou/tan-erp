"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Alert } from "@/components/ui/Alert";
import { parseBoqImportRows, type BoqImportError, type BoqImportRow } from "../utils/estimate-boq-import";

export interface EstimateBoqImportModalProps {
  isOpen: boolean;
  onClose: () => void;
  onImport: (rows: BoqImportRow[], reasonCode: string, reason: string) => void;
  sectionTitle?: string;
}

export function EstimateBoqImportModal({ isOpen, onClose, onImport, sectionTitle }: EstimateBoqImportModalProps) {
  const t = useTranslations("estimates");
  const tc = useTranslations("common");
  const [rows, setRows] = useState<BoqImportRow[]>([]);
  const [errors, setErrors] = useState<BoqImportError[]>([]);
  const [readFailed, setReadFailed] = useState(false);
  const [reasonCode, setReasonCode] = useState("");
  const [reason, setReason] = useState("");
  const [isReading, setIsReading] = useState(false);

  function close() {
    setRows([]); setErrors([]); setReadFailed(false); setReasonCode(""); setReason("");
    onClose();
  }

  async function handleFile(file: File | undefined) {
    if (!file) return;
    setIsReading(true); setReadFailed(false);
    try {
      // exceljs is large, so it is only loaded when someone actually imports.
      const { default: ExcelJS } = await import("exceljs");
      const workbook = new ExcelJS.Workbook();
      await workbook.xlsx.load(await file.arrayBuffer());
      const sheet = workbook.worksheets[0];
      const raw: unknown[][] = [];
      sheet?.eachRow({ includeEmpty: false }, (row) => { raw.push(Array.from({ length: 7 }, (_, index) => row.getCell(index + 1).text)); });
      const parsed = parseBoqImportRows(raw);
      setRows(parsed.rows); setErrors(parsed.errors);
    } catch {
      setRows([]); setErrors([]); setReadFailed(true);
    } finally {
      setIsReading(false);
    }
  }

  const canImport = rows.length > 0 && errors.length === 0 && reasonCode.trim() !== "" && reason.trim() !== "";
  return (
    <Modal isOpen={isOpen} onClose={close} title={t("importExcel")} description={sectionTitle ? `${t("importExcelDesc")} · ${sectionTitle}` : t("importExcelDesc")} size="lg"
      footer={<div className="flex justify-end gap-2">
        <Button variant="outline" onClick={close}>{tc("actions.cancel")}</Button>
        <Button variant="primary" disabled={!canImport} onClick={() => { onImport(rows, reasonCode.trim(), reason.trim()); close(); }}>{t("importApply", { count: rows.length })}</Button>
      </div>}>
      <div className="space-y-3">
        <Input id="boq-import-file" type="file" accept=".xlsx" disabled={isReading} label={t("importChooseFile")} placeholder={t("importChooseFile")}
          onChange={(event) => { void handleFile(event.target.files?.[0]); }} />
        {readFailed && <Alert variant="danger">{t("importReadFailed")}</Alert>}
        {errors.length > 0 && (
          <Alert variant="danger">
            <p className="font-semibold">{t("importHasErrors", { count: errors.length })}</p>
            <ul className="mt-1 list-disc pl-5 text-xs">
              {errors.slice(0, 20).map((error) => <li key={`${error.row}-${error.field}`}>{t("importErrorRow", { row: error.row, field: t(`importFields.${error.field}`) })}</li>)}
            </ul>
          </Alert>
        )}
        {rows.length > 0 && errors.length === 0 && <Alert variant="success">{t("importReady", { count: rows.length })}</Alert>}
        <p className="text-xs text-erp-text-muted">{t("importCustomReasonNote")}</p>
        <Input id="boq-import-reason-code" label={t("customWorkItemReasonCode")} placeholder={t("customWorkItemReasonCodePlaceholder")} maxLength={64} value={reasonCode} onChange={(event) => setReasonCode(event.target.value)} />
        <Input id="boq-import-reason" label={t("customWorkItemReason")} placeholder={t("customWorkItemReasonPlaceholder")} maxLength={500} value={reason} onChange={(event) => setReason(event.target.value)} />
      </div>
    </Modal>
  );
}
