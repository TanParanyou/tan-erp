"use client";

import React, { useMemo, useState } from "react";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { DataTable, type Column } from "@/components/ui/DataTable";
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

  const columns = useMemo<Column<DocumentSequenceItem>[]>(
    () => [
      {
        id: "documentType",
        header: t("table.documentType"),
        className: "min-w-[200px]",
        cell: (_val, seq) => (
          <div className="flex items-center gap-2">
            <span className="font-medium text-erp-text-main">
              {t(`types.${seq.documentType}`)}
            </span>
            {seq.isBranchSpecific && (
              <Badge variant="outline" size="sm">
                {t("badgeBranchSpecific")}
              </Badge>
            )}
          </div>
        ),
      },
      {
        id: "prefix",
        header: t("table.prefix"),
        className: "min-w-[120px]",
        cell: (_val, seq) => (
          <span className="font-mono font-semibold text-erp-navy">
            {seq.prefix}
          </span>
        ),
      },
      {
        id: "pattern",
        header: t("table.pattern"),
        className: "min-w-[180px]",
        cell: (_val, seq) => (
          <span className="font-mono text-xs text-erp-text-muted">
            {seq.formatPattern}
          </span>
        ),
      },
      {
        id: "resetPeriod",
        header: t("table.resetPeriod"),
        className: "min-w-[140px]",
        cell: (_val, seq) => (
          <span className="text-erp-text-body">
            {t(`resetOptions.${seq.resetPeriod.toLowerCase()}`)}
          </span>
        ),
      },
      {
        id: "preview",
        header: t("table.preview"),
        className: "min-w-[160px]",
        cell: (_val, seq) => (
          <span className="font-mono text-xs px-2 py-1 bg-erp-surface-subtle border border-erp-border text-erp-navy font-bold">
            {seq.samplePreview}
          </span>
        ),
      },
      {
        id: "actions",
        header: tCommon("actions.manage"),
        isAction: true,
        className: "text-right min-w-[100px]",
        cell: (_val, seq) => (
          <Button
            variant="outline"
            size="sm"
            onClick={() => handleEdit(seq)}
            className="inline-flex items-center gap-1.5"
            icon={<IconEdit size={14} />}
          >
            <span>{tCommon("actions.edit")}</span>
          </Button>
        ),
      },
    ],
    [t, tCommon]
  );

  return (
    <div className="flex flex-col gap-6">
      {/* 2-Tier Architectural Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-erp-border pb-4">
        <div>
          <h1 className="text-2xl font-bold text-erp-text-main tracking-tight">
            {t("title")}
          </h1>
          <p className="text-sm text-erp-text-muted mt-1">
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

      {/* Table Preserved Layout via Reusable DataTable */}
      <DataTable<DocumentSequenceItem>
        columns={columns}
        data={sequences ?? []}
        isLoading={isLoading}
        isError={isError}
        error={isError ? t("loadError") : null}
        onRetry={() => refetch()}
        hidePagination={true}
      />

      {/* Edit Drawer */}
      <DocumentSequenceDrawer
        item={selectedItem}
        isOpen={isDrawerOpen}
        onClose={handleCloseDrawer}
      />
    </div>
  );
}
