"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { IconEdit, IconRefresh } from "@/components/common/Icons";
import { useDocumentSequences } from "../api/document-sequence-queries";
import { DocumentSequenceDrawer } from "./document-sequence-drawer";
import type { DocumentSequenceItem } from "../types";

export function DocumentSequenceList() {
  const t = useTranslations("documentNumbering");
  const tCommon = useTranslations("common");
  const { data: sequences, isLoading, isError, refetch } = useDocumentSequences();

  const [selectedItem, setSelectedItem] = useState<DocumentSequenceItem | null>(null);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);

  const handleEdit = (item: DocumentSequenceItem) => {
    setSelectedItem(item);
    setIsDrawerOpen(true);
  };

  const handleCloseDrawer = () => {
    setIsDrawerOpen(false);
    setSelectedItem(null);
  };

  return (
    <div className="flex flex-col gap-6">
      {/* 2-Tier Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-erp-border pb-4">
        <div>
          <h1 className="text-2xl font-bold text-slate-900 dark:text-white tracking-tight">
            {t("title")}
          </h1>
          <p className="text-sm text-slate-500 dark:text-slate-400 mt-1">
            {t("description")}
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="sm" onClick={() => refetch()} title={tCommon("actions.refresh")}>
            <IconRefresh size={16} />
            <span>{tCommon("actions.refresh")}</span>
          </Button>
        </div>
      </div>

      {/* Loading State: Minimal Mono per Atelier Architectural standards */}
      {isLoading && (
        <div className="flex items-center justify-center p-12 border border-erp-border bg-slate-50 dark:bg-slate-900">
          <span className="font-mono text-xs uppercase tracking-widest text-slate-500 animate-pulse">
            {t("loading")}
          </span>
        </div>
      )}

      {/* Error State */}
      {isError && !isLoading && (
        <div className="p-4 border border-red-500 bg-red-50 dark:bg-red-950/30 text-red-700 dark:text-red-300 text-sm">
          {t("loadError")}
        </div>
      )}

      {/* Table Preserved Layout */}
      {!isLoading && !isError && (
        <div className="border border-erp-border bg-white dark:bg-slate-950 overflow-x-auto">
          <table className="w-full text-left text-sm border-collapse">
            <thead>
              <tr className="border-b border-erp-border bg-slate-100 dark:bg-slate-900 text-xs font-semibold uppercase tracking-wider text-slate-600 dark:text-slate-400">
                <th className="p-3">{t("table.documentType")}</th>
                <th className="p-3">{t("table.prefix")}</th>
                <th className="p-3">{t("table.pattern")}</th>
                <th className="p-3">{t("table.resetPeriod")}</th>
                <th className="p-3">{t("table.preview")}</th>
                <th className="p-3 text-right">{tCommon("actions.manage")}</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-erp-border">
              {(sequences ?? []).map((seq) => (
                <tr
                  key={seq.documentType}
                  className="hover:bg-slate-50 dark:hover:bg-slate-900/50 transition-colors"
                >
                  <td className="p-3 font-medium text-slate-900 dark:text-white">
                    <div className="flex items-center gap-2">
                      <span>{t(`types.${seq.documentType}`)}</span>
                      {seq.isBranchSpecific && (
                        <Badge variant="outline" size="sm">
                          {t("badgeBranchSpecific")}
                        </Badge>
                      )}
                    </div>
                  </td>
                  <td className="p-3 font-mono font-semibold text-erp-navy dark:text-sky-400">
                    {seq.prefix}
                  </td>
                  <td className="p-3 font-mono text-xs text-slate-600 dark:text-slate-300">
                    {seq.formatPattern}
                  </td>
                  <td className="p-3 text-slate-600 dark:text-slate-300">
                    {t(`resetOptions.${seq.resetPeriod.toLowerCase()}`)}
                  </td>
                  <td className="p-3">
                    <span className="font-mono text-xs px-2 py-1 bg-slate-100 dark:bg-slate-800 border border-slate-300 dark:border-slate-700 text-erp-navy dark:text-sky-300 font-bold">
                      {seq.samplePreview}
                    </span>
                  </td>
                  <td className="p-3 text-right">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => handleEdit(seq)}
                      className="inline-flex items-center gap-1.5"
                    >
                      <IconEdit size={14} />
                      <span>{tCommon("actions.edit")}</span>
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Edit Drawer */}
      <DocumentSequenceDrawer
        item={selectedItem}
        isOpen={isDrawerOpen}
        onClose={handleCloseDrawer}
      />
    </div>
  );
}
