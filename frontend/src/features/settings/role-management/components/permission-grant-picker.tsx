"use client";

import React, { useMemo, useState } from "react";
import { useTranslations } from "next-intl";
import { Checkbox } from "@/components/ui/Checkbox";
import { Input } from "@/components/ui/Input";
import { StatusBadge } from "@/components/ui/StatusBadge";

export interface CatalogItem {
  key: string;
  description: string | null;
  isApprovalPermission: boolean;
}

interface PermissionGrantPickerProps {
  catalog: ReadonlyArray<CatalogItem>;
  selected: ReadonlySet<string>;
  disabled: boolean;
  onToggle: (key: string) => void;
}

/** Organization-scope permission picker, grouped by module (the part of the key before the first dot). */
export function PermissionGrantPicker({ catalog, selected, disabled, onToggle }: PermissionGrantPickerProps) {
  const t = useTranslations("roleAdmin.editor.permissions");
  const [query, setQuery] = useState("");

  const groups = useMemo(() => {
    const needle = query.trim().toLowerCase();
    const matches = catalog.filter(
      (item) => needle === "" || item.key.toLowerCase().includes(needle) || (item.description ?? "").toLowerCase().includes(needle)
    );
    const byModule = new Map<string, CatalogItem[]>();
    for (const item of matches) {
      const moduleName = item.key.split(".")[0];
      byModule.set(moduleName, [...(byModule.get(moduleName) ?? []), item]);
    }
    return [...byModule.entries()];
  }, [catalog, query]);

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <Input
          label={t("searchPlaceholder")}
          placeholder={t("searchPlaceholder")}
          value={query}
          onChange={(event) => setQuery(event.target.value)}
        />
        <span className="text-sm text-erp-text-muted">{t("selectedCount", { count: selected.size })}</span>
      </div>
      {groups.length === 0 ? <p className="text-sm text-erp-text-muted">{t("noMatch")}</p> : null}
      {groups.map(([moduleName, items]) => (
        <fieldset key={moduleName} className="space-y-1 border border-erp-border p-3">
          <legend className="px-1 text-sm font-semibold text-erp-text-main">{moduleName}</legend>
          {items.map((item) => (
            <div key={item.key} className="flex flex-wrap items-center gap-x-3">
              <Checkbox
                label={item.key}
                checked={selected.has(item.key)}
                disabled={disabled}
                onCheckedChange={() => onToggle(item.key)}
                wrapperClassName="mb-0"
              />
              {item.isApprovalPermission ? <StatusBadge label={t("approvalTag")} variant="warning" /> : null}
              {item.description ? <span className="text-xs text-erp-text-muted">{item.description}</span> : null}
            </div>
          ))}
        </fieldset>
      ))}
    </div>
  );
}
