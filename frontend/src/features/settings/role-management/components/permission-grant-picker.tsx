"use client";

import React, { useMemo, useState } from "react";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { Tooltip } from "@/components/ui/Tooltip";
import { IconCheck, IconClose } from "@/components/common/Icons";

export interface CatalogItem {
  key: string;
  description: string | null;
  module?: string | null;
  requiresMakerChecker?: boolean;
  isDocumentApproval?: boolean;
  isApprovalPermission?: boolean;
}

interface PermissionGrantPickerProps {
  catalog: ReadonlyArray<CatalogItem>;
  selected: ReadonlySet<string>;
  disabled: boolean;
  onToggle: (key: string) => void;
  onBatchToggle?: (keys: string[], select: boolean) => void;
}

const MODULE_ORDER: readonly string[] = ["system", "crm", "survey_estimating", "operations"];

/** Organization-scope permission picker, grouped by business module with Maker-Checker / Approval distinctions. */
export function PermissionGrantPicker({ catalog, selected, disabled, onToggle, onBatchToggle }: PermissionGrantPickerProps) {
  const t = useTranslations("roleAdmin.editor.permissions");
  const [query, setQuery] = useState("");

  const groups = useMemo(() => {
    const needle = query.trim().toLowerCase();
    const matches = catalog.filter(
      (item) => needle === "" || item.key.toLowerCase().includes(needle) || (item.description ?? "").toLowerCase().includes(needle)
    );
    const byModule = new Map<string, CatalogItem[]>();
    for (const item of matches) {
      const moduleName = item.module || item.key.split(".")[0];
      byModule.set(moduleName, [...(byModule.get(moduleName) ?? []), item]);
    }

    // Sort groups according to domain business hierarchy
    return [...byModule.entries()].sort(([a], [b]) => {
      const indexA = MODULE_ORDER.indexOf(a);
      const indexB = MODULE_ORDER.indexOf(b);
      if (indexA !== -1 && indexB !== -1) return indexA - indexB;
      if (indexA !== -1) return -1;
      if (indexB !== -1) return 1;
      return a.localeCompare(b);
    });
  }, [catalog, query]);

  const handleGroupToggle = (items: CatalogItem[], selectAll: boolean) => {
    if (disabled) return;
    if (onBatchToggle) {
      onBatchToggle(items.map((i) => i.key), selectAll);
    } else {
      for (const item of items) {
        const isSelected = selected.has(item.key);
        if (selectAll && !isSelected) onToggle(item.key);
        else if (!selectAll && isSelected) onToggle(item.key);
      }
    }
  };

  const getModuleLabel = (moduleName: string): string => {
    if (t.has(`modules.${moduleName}`)) {
      return t(`modules.${moduleName}`);
    }
    return moduleName;
  };

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <Input
          label={t("searchPlaceholder")}
          placeholder={t("searchPlaceholder")}
          value={query}
          onChange={(event) => setQuery(event.target.value)}
          wrapperClassName="w-full sm:w-80 !mb-0"
        />
        <span className="text-sm text-erp-text-muted">{t("selectedCount", { count: selected.size })}</span>
      </div>
      {groups.length === 0 ? <p className="text-sm text-erp-text-muted">{t("noMatch")}</p> : null}
      {groups.map(([moduleName, items]) => {
        const allSelected = items.length > 0 && items.every((i) => selected.has(i.key));
        return (
          <fieldset key={moduleName} className="space-y-2 border border-erp-border p-3">
            <div className="flex flex-wrap items-center justify-between gap-2 border-b border-erp-border-hairline pb-2">
              <legend className="px-1 text-sm font-semibold text-erp-text-main">{getModuleLabel(moduleName)}</legend>
              {!disabled && (
                <div className="flex items-center gap-1.5">
                  <Tooltip content={allSelected ? t("clearGroup") : t("selectAll")} position="top">
                    <Button
                      type="button"
                      variant="outline"
                      size="icon"
                      className={`h-7 w-7 min-h-[28px] min-w-[28px] p-0 border border-erp-border ${
                        allSelected
                          ? "text-erp-text-muted hover:border-erp-danger hover:text-erp-danger"
                          : "text-erp-navy hover:bg-erp-bg-subtle"
                      }`}
                      onClick={() => handleGroupToggle(items, !allSelected)}
                      aria-label={allSelected ? t("clearGroup") : t("selectAll")}
                      icon={allSelected ? <IconClose size={14} /> : <IconCheck size={14} />}
                    />
                  </Tooltip>
                </div>
              )}
            </div>

            <div className="space-y-2 pt-1">
              {items.map((item) => {
                const isSelected = selected.has(item.key);
                return (
                  <div key={item.key} className="space-y-1">
                    <div className="flex flex-wrap items-center gap-x-3 gap-y-1">
                      <Checkbox
                        label={item.key}
                        checked={isSelected}
                        disabled={disabled}
                        onCheckedChange={() => onToggle(item.key)}
                        wrapperClassName="mb-0"
                      />
                      {item.isDocumentApproval ? (
                        <StatusBadge label={t("documentApprovalTag")} variant="info" />
                      ) : item.requiresMakerChecker ? (
                        <StatusBadge label={t("makerCheckerTag")} variant="warning" />
                      ) : item.isApprovalPermission ? (
                        <StatusBadge label={t("approvalTag")} variant="warning" />
                      ) : null}
                      {item.description ? <span className="text-xs text-erp-text-muted">{item.description}</span> : null}
                    </div>

                    {item.key === "approval-limits.manage" ? (
                      <div className="ml-7 rounded-none border-l-2 border-amber-500 bg-amber-50/70 p-2 text-xs text-amber-900">
                        {t("approvalLimitsManageHint")}
                      </div>
                    ) : null}
                  </div>
                );
              })}
            </div>
          </fieldset>
        );
      })}
    </div>
  );
}
