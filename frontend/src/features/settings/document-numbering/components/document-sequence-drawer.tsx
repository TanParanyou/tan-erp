"use client";

import React, { useState, useEffect, useMemo } from "react";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { useUpdateDocumentSequence } from "../api/document-sequence-queries";
import type { DocumentSequenceItem, ResetPeriod } from "../types";

export interface DocumentSequenceDrawerProps {
  item: DocumentSequenceItem | null;
  isOpen: boolean;
  onClose: () => void;
}

const AVAILABLE_TOKENS = [
  { token: "{PREFIX}", label: "Prefix" },
  { token: "{BRANCH}", label: "Branch" },
  { token: "{BBBB}", label: "พ.ศ. (4 หลัก)" },
  { token: "{BB}", label: "พ.ศ. (2 หลัก)" },
  { token: "{YYYY}", label: "ค.ศ. (4 หลัก)" },
  { token: "{YY}", label: "ค.ศ. (2 หลัก)" },
  { token: "{MM}", label: "เดือน (01-12)" },
  { token: "{DD}", label: "วัน (01-31)" },
  { token: "{SEQ:4}", label: "Running 4 หลัก" },
  { token: "{SEQ:5}", label: "Running 5 หลัก" },
];

export function DocumentSequenceDrawer({
  item,
  isOpen,
  onClose,
}: DocumentSequenceDrawerProps) {
  const t = useTranslations("documentNumbering");
  const tCommon = useTranslations("common");

  const [prefix, setPrefix] = useState("");
  const [formatPattern, setFormatPattern] = useState("");
  const [resetPeriod, setResetPeriod] = useState<ResetPeriod>("Yearly");
  const [padding, setPadding] = useState<number>(4);
  const [isBranchSpecific, setIsBranchSpecific] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const updateMutation = useUpdateDocumentSequence();

  useEffect(() => {
    if (item) {
      setPrefix(item.prefix);
      setFormatPattern(item.formatPattern);
      setResetPeriod(item.resetPeriod);
      setPadding(item.padding);
      setIsBranchSpecific(item.isBranchSpecific);
      setError(null);
    }
  }, [item]);

  const livePreview = useMemo(() => {
    if (!formatPattern) return "";
    const now = new Date();
    const yearAd = now.getFullYear();
    const yearBe = yearAd + 543;
    const shortYearAd = String(yearAd % 100).padStart(2, "0");
    const shortYearBe = String(yearBe % 100).padStart(2, "0");
    const month = String(now.getMonth() + 1).padStart(2, "0");
    const day = String(now.getDate()).padStart(2, "0");

    let res = formatPattern
      .replace(/\{PREFIX\}/gi, prefix || "")
      .replace(/\{BRANCH\}/gi, isBranchSpecific ? "BKK" : "HQ")
      .replace(/\{YYYY\}/gi, String(yearAd))
      .replace(/\{YY\}/gi, shortYearAd)
      .replace(/\{BBBB\}/gi, String(yearBe))
      .replace(/\{BB\}/gi, shortYearBe)
      .replace(/\{MM\}/gi, month)
      .replace(/\{DD\}/gi, day);

    res = res.replace(/\{SEQ(?::(\d+))?\}/gi, (_, p) => {
      const padLen = p ? parseInt(p, 10) : padding || 4;
      return String(1).padStart(padLen, "0");
    });

    return res;
  }, [formatPattern, prefix, isBranchSpecific, padding]);

  if (!item) return null;

  const handleInsertToken = (token: string) => {
    setFormatPattern((prev) => `${prev}${token}`);
  };

  const handleSave = async () => {
    if (!formatPattern.trim()) {
      setError(t("patternRequired"));
      return;
    }

    try {
      await updateMutation.mutateAsync({
        documentType: item.documentType,
        payload: {
          prefix: prefix.trim(),
          formatPattern: formatPattern.trim(),
          resetPeriod,
          padding: Number(padding) || 4,
          isBranchSpecific,
        },
      });
      onClose();
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : t("updateFailed"));
    }
  };

  return (
    <Drawer
      isOpen={isOpen}
      onClose={onClose}
      title={`${t("editTitle")}: ${t(`types.${item.documentType}`)}`}
      description={t("editDescription")}
      size="lg"
      footer={
        <div className="flex justify-end gap-3 w-full">
          <Button variant="outline" size="sm" onClick={onClose} disabled={updateMutation.isPending}>
            {tCommon("actions.cancel")}
          </Button>
          <Button
            variant="primary"
            size="sm"
            onClick={handleSave}
            isLoading={updateMutation.isPending}
            disabled={updateMutation.isPending}
          >
            {tCommon("actions.save")}
          </Button>
        </div>
      }
    >
      <div className="flex flex-col gap-6 py-2">
        {/* Live Preview Card */}
        <div className="p-4 border border-erp-border bg-slate-50 dark:bg-slate-900">
          <div className="text-xs uppercase tracking-wider font-semibold text-slate-500 mb-1">
            {t("livePreview")}
          </div>
          <div className="flex items-center gap-2">
            <span className="font-mono text-xl font-bold text-erp-navy dark:text-sky-400">
              {livePreview || "-"}
            </span>
            <Badge variant="outline" size="sm">
              {t("previewBadge")}
            </Badge>
          </div>
        </div>

        {error && (
          <div className="p-3 border border-red-500 bg-red-50 dark:bg-red-950/40 text-red-700 dark:text-red-300 text-sm">
            {error}
          </div>
        )}

        {/* Prefix Input */}
        <div className="flex flex-col gap-1.5">
          <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
            {t("prefix")}
          </label>
          <input
            type="text"
            className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 font-mono text-sm"
            value={prefix}
            onChange={(e) => setPrefix(e.target.value.toUpperCase())}
            placeholder="EST"
          />
        </div>

        {/* Format Pattern Input */}
        <div className="flex flex-col gap-2">
          <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
            {t("formatPattern")}
          </label>
          <input
            type="text"
            className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 font-mono text-sm"
            value={formatPattern}
            onChange={(e) => setFormatPattern(e.target.value)}
            placeholder="{PREFIX}-{BRANCH}-{BB}{MM}-{SEQ:4}"
          />

          {/* Token Insertion Pills */}
          <div className="flex flex-col gap-1 mt-1">
            <span className="text-xs text-slate-500">{t("clickToInsertToken")}:</span>
            <div className="flex flex-wrap gap-1.5">
              {AVAILABLE_TOKENS.map((item) => (
                <button
                  key={item.token}
                  type="button"
                  onClick={() => handleInsertToken(item.token)}
                  className="px-2 py-1 text-xs font-mono bg-white dark:bg-slate-800 border border-slate-300 dark:border-slate-700 hover:border-erp-navy hover:text-erp-navy transition-colors cursor-pointer"
                  title={item.label}
                >
                  {item.token}
                </button>
              ))}
            </div>
          </div>
        </div>

        {/* Reset Period & Padding */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div className="flex flex-col gap-1.5">
            <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
              {t("resetPeriod")}
            </label>
            <select
              className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 text-sm"
              value={resetPeriod}
              onChange={(e) => setResetPeriod(e.target.value as ResetPeriod)}
            >
              <option value="Never">{t("resetOptions.never")}</option>
              <option value="Yearly">{t("resetOptions.yearly")}</option>
              <option value="Monthly">{t("resetOptions.monthly")}</option>
              <option value="Daily">{t("resetOptions.daily")}</option>
            </select>
          </div>

          <div className="flex flex-col gap-1.5">
            <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
              {t("padding")}
            </label>
            <input
              type="number"
              min={1}
              max={10}
              className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 text-sm"
              value={padding}
              onChange={(e) => setPadding(parseInt(e.target.value, 10) || 4)}
            />
          </div>
        </div>

        {/* Branch Specific Checkbox */}
        <label className="flex items-center gap-2 cursor-pointer pt-2">
          <input
            type="checkbox"
            className="w-4 h-4 border border-erp-border text-erp-navy focus:ring-0"
            checked={isBranchSpecific}
            onChange={(e) => setIsBranchSpecific(e.target.checked)}
          />
          <span className="text-sm text-slate-800 dark:text-slate-200">{t("isBranchSpecific")}</span>
        </label>
      </div>
    </Drawer>
  );
}
