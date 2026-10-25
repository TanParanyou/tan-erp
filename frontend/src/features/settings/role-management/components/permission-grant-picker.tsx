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

function parsePermissionKey(key: string): { resource: string; action: string } {
  const lastDot = key.lastIndexOf(".");
  const resource = lastDot !== -1 ? key.substring(0, lastDot) : key;
  const action = lastDot !== -1 ? key.substring(lastDot + 1) : "";
  return { resource, action };
}

interface FeatureGroup {
  resource: string;
  items: CatalogItem[];
}

interface ModuleGroup {
  moduleName: string;
  items: CatalogItem[];
  features: FeatureGroup[];
}

/** Feature-First Permission Picker: All permissions stay unified within their functional domain. */
export function PermissionGrantPicker({ catalog, selected, disabled, onToggle, onBatchToggle }: PermissionGrantPickerProps) {
  const t = useTranslations("roleAdmin.editor.permissions");
  const [query, setQuery] = useState("");

  const getModuleLabel = (moduleName: string): string => {
    if (t.has(`modules.${moduleName}`)) {
      return t(`modules.${moduleName}`);
    }
    return moduleName;
  };

  const getResourceLabel = (resource: string): string => {
    // Dots would be read by next-intl as nesting, so dotted resources use "_" in the message key.
    const resourceKey = resource.replace(/\./g, "_");
    if (t.has(`resourceNames.${resourceKey}`)) {
      return t(`resourceNames.${resourceKey}`);
    }
    return resource.replace(/-/g, " ");
  };

  const getActionLabel = (item: CatalogItem): string => {
    const actionKey = item.key.replace(/[\.-]/g, "_");
    if (t.has(`actions.${actionKey}`)) {
      return t(`actions.${actionKey}`);
    }
    if (item.description) {
      return item.description;
    }
    const { action } = parsePermissionKey(item.key);
    return action;
  };

  const groups = useMemo<ModuleGroup[]>(() => {
    const needle = query.trim().toLowerCase();
    const matches = catalog.filter((item) => {
      if (needle === "") return true;
      const { resource } = parsePermissionKey(item.key);
      const resLabel = getResourceLabel(resource).toLowerCase();
      const modLabel = (item.module ? getModuleLabel(item.module) : "").toLowerCase();
      const actionLabel = getActionLabel(item).toLowerCase();
      return (
        item.key.toLowerCase().includes(needle) ||
        (item.description ?? "").toLowerCase().includes(needle) ||
        resLabel.includes(needle) ||
        modLabel.includes(needle) ||
        actionLabel.includes(needle)
      );
    });

    const byModule = new Map<string, CatalogItem[]>();
    for (const item of matches) {
      const moduleName = item.module || item.key.split(".")[0];
      byModule.set(moduleName, [...(byModule.get(moduleName) ?? []), item]);
    }

    const sortedModuleEntries = [...byModule.entries()].sort(([a], [b]) => {
      const indexA = MODULE_ORDER.indexOf(a);
      const indexB = MODULE_ORDER.indexOf(b);
      if (indexA !== -1 && indexB !== -1) return indexA - indexB;
      if (indexA !== -1) return -1;
      if (indexB !== -1) return 1;
      return a.localeCompare(b);
    });

    return sortedModuleEntries.map(([moduleName, items]) => {
      const resMap = new Map<string, CatalogItem[]>();
      for (const item of items) {
        const { resource } = parsePermissionKey(item.key);
        resMap.set(resource, [...(resMap.get(resource) ?? []), item]);
      }

      const features: FeatureGroup[] = [...resMap.entries()].map(([resource, resItems]) => ({
        resource,
        items: resItems,
      }));

      return { moduleName, items, features };
    });
  }, [catalog, query]);

  const handleBatchToggle = (items: CatalogItem[], selectAll: boolean) => {
    if (disabled || items.length === 0) return;
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

  const renderSecurityBadges = (item: CatalogItem) => {
    return (
      <div className="flex flex-wrap items-center gap-1">
        {item.isDocumentApproval ? (
          <StatusBadge label={t("documentApprovalTag")} variant="info" className="text-[10px] px-1 py-0" />
        ) : item.requiresMakerChecker ? (
          <StatusBadge label={t("makerCheckerTag")} variant="warning" className="text-[10px] px-1 py-0" />
        ) : item.isApprovalPermission ? (
          <StatusBadge label={t("approvalTag")} variant="warning" className="text-[10px] px-1 py-0" />
        ) : null}
      </div>
    );
  };

  return (
    <div className="space-y-6">
      {/* Search and Global Counter */}
      <div className="flex flex-wrap items-end justify-between gap-3">
        <Input
          label={t("searchPlaceholder")}
          placeholder={t("searchPlaceholder")}
          value={query}
          onChange={(event) => setQuery(event.target.value)}
          wrapperClassName="w-full sm:w-80 !mb-0"
        />
        <span className="text-sm font-medium text-erp-text-main">
          {t("selectedCount", { count: selected.size })}
        </span>
      </div>

      {groups.length === 0 ? <p className="text-sm text-erp-text-muted">{t("noMatch")}</p> : null}

      {/* Modules */}
      {groups.map(({ moduleName, items: moduleItems, features }) => {
        const selectedInModule = moduleItems.filter((i) => selected.has(i.key)).length;
        const allModuleSelected = moduleItems.length > 0 && selectedInModule === moduleItems.length;

        return (
          <div key={moduleName} className="border border-erp-border bg-erp-surface">
            {/* Module Header Bar */}
            <div className="flex flex-wrap items-center justify-between gap-2 border-b border-erp-border bg-erp-surface-subtle px-4 py-3">
              <div className="flex items-center gap-2.5">
                <div>
                  <h3 className="text-sm font-semibold text-erp-navy">{getModuleLabel(moduleName)}</h3>
                  <span className="text-xs text-erp-text-muted font-mono">{moduleName}</span>
                </div>
                <span className="text-xs text-erp-text-muted bg-erp-surface px-2 py-0.5 border border-erp-border">
                  {selectedInModule} / {moduleItems.length}
                </span>
              </div>
              {!disabled && (
                <div className="flex items-center gap-2">
                  <Tooltip content={allModuleSelected ? t("clearGroup") : t("selectAll")} position="top">
                    <Button
                      type="button"
                      variant="outline"
                      size="icon"
                      className={`h-8 w-8 min-h-[32px] min-w-[32px] p-0 border border-erp-border ${
                        allModuleSelected
                          ? "text-erp-text-muted hover:border-erp-danger hover:text-erp-danger"
                          : "text-erp-navy hover:bg-erp-bg-subtle"
                      }`}
                      onClick={() => handleBatchToggle(moduleItems, !allModuleSelected)}
                      aria-label={allModuleSelected ? t("clearGroup") : t("selectAll")}
                      icon={allModuleSelected ? <IconClose size={15} /> : <IconCheck size={15} />}
                    />
                  </Tooltip>
                </div>
              )}
            </div>

            {/* Feature Cards Grid */}
            <div className="p-4 grid grid-cols-1 lg:grid-cols-2 gap-4">
              {features.map(({ resource, items: featureItems }) => {
                const selectedInFeature = featureItems.filter((i) => selected.has(i.key)).length;
                const allFeatureSelected = featureItems.length > 0 && selectedInFeature === featureItems.length;
                const resourceLabel = getResourceLabel(resource);

                return (
                  <div
                    key={resource}
                    className={`border transition-colors flex flex-col justify-between ${
                      selectedInFeature > 0 ? "border-erp-navy/30 bg-erp-surface" : "border-erp-border bg-erp-surface"
                    }`}
                  >
                    {/* Feature Card Header */}
                    <div className="flex items-center justify-between gap-2 border-b border-erp-border bg-erp-surface-subtle/50 px-3.5 py-2.5">
                      <div className="min-w-0">
                        <h4 className="text-xs font-semibold text-erp-text-main truncate">
                          {resourceLabel}
                        </h4>
                        <span className="text-[11px] text-erp-text-muted font-mono block">
                          {resource}
                        </span>
                      </div>
                      <div className="flex items-center gap-2">
                        <span className="text-xs text-erp-text-muted font-mono">
                          {selectedInFeature}/{featureItems.length}
                        </span>
                        {!disabled && (
                          <Tooltip
                            content={
                              allFeatureSelected
                                ? t("clearFeature", { feature: resourceLabel })
                                : t("selectAllFeature", { feature: resourceLabel })
                            }
                            position="top"
                          >
                            <Button
                              type="button"
                              variant="ghost"
                              size="icon"
                              className={`h-7 w-7 min-h-[28px] min-w-[28px] p-0 border border-erp-border ${
                                allFeatureSelected
                                  ? "text-erp-text-muted hover:border-erp-danger hover:text-erp-danger"
                                  : "text-erp-navy hover:bg-erp-bg-subtle"
                              }`}
                              onClick={() => handleBatchToggle(featureItems, !allFeatureSelected)}
                              aria-label={
                                allFeatureSelected
                                  ? t("clearFeature", { feature: resourceLabel })
                                  : t("selectAllFeature", { feature: resourceLabel })
                              }
                              icon={allFeatureSelected ? <IconClose size={13} /> : <IconCheck size={13} />}
                            />
                          </Tooltip>
                        )}
                      </div>
                    </div>

                    {/* Permissions List for Feature */}
                    <div className="divide-y divide-erp-border-hairline flex-1">
                      {featureItems.map((item) => {
                        const isSelected = selected.has(item.key);
                        const actionTitle = getActionLabel(item);

                        return (
                          <div
                            key={item.key}
                            className={`p-3 transition-colors ${
                              isSelected ? "bg-erp-surface-subtle/40" : "hover:bg-erp-surface-subtle/20"
                            }`}
                          >
                            <div className="flex items-start gap-2.5">
                              <Checkbox
                                id={`checkbox-${item.key}`}
                                aria-label={item.key}
                                checked={isSelected}
                                disabled={disabled}
                                onCheckedChange={() => onToggle(item.key)}
                                wrapperClassName="!mb-0 mt-0.5"
                              />
                              <div className="space-y-1 flex-1 min-w-0">
                                <div className="flex flex-wrap items-center gap-1.5">
                                  <label
                                    htmlFor={`checkbox-${item.key}`}
                                    className={`text-xs font-semibold cursor-pointer ${
                                      isSelected ? "text-erp-navy" : "text-erp-text-main"
                                    }`}
                                  >
                                    {actionTitle}
                                  </label>
                                  {renderSecurityBadges(item)}
                                </div>

                                <span className="text-[11px] text-erp-text-muted font-mono block">
                                  {item.key}
                                </span>

                                {item.description && item.description !== actionTitle ? (
                                  <p className="text-[11px] text-erp-text-muted leading-relaxed">
                                    {item.description}
                                  </p>
                                ) : null}

                                {item.key === "approval-limits.manage" ? (
                                  <div className="mt-1.5 rounded-none border-l-2 border-amber-500 bg-amber-50/80 p-2 text-xs text-amber-900">
                                    {t("approvalLimitsManageHint")}
                                  </div>
                                ) : null}
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        );
      })}
    </div>
  );
}
