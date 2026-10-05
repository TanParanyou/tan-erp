"use client";

import React, { useId } from "react";
import {
  Controller,
  type ArrayPath,
  type Control,
  type FieldValues,
  type Path,
} from "react-hook-form";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { useItemAttributes } from "@/hooks/useItemAttributes";
import { getMergedAttributeDefinition } from "@/lib/utils/item-attributes";
import { cn } from "@/lib/utils/cn";
import { type AttributeDefinition, type CategoryAttributeTemplateResponse } from "@/types/item-attributes";
import { useCategoryAttributeTemplate } from "@/features/item-master/api/item-master-queries";

export interface ItemAttributesFieldProps<
  TFieldValues extends FieldValues,
  TArrayPath extends ArrayPath<TFieldValues> = ArrayPath<TFieldValues>,
> {
  control: Control<TFieldValues>;
  name: TArrayPath;
  disabled?: boolean;
  itemType?: string;
  categoryId?: string;
  categoryTemplates?: CategoryAttributeTemplateResponse | null;
  className?: string;
}

export function ItemAttributesField<
  TFieldValues extends FieldValues,
  TArrayPath extends ArrayPath<TFieldValues> = ArrayPath<TFieldValues>,
>({
  control,
  name,
  disabled = false,
  itemType,
  categoryId,
  categoryTemplates: passedCategoryTemplates,
  className,
}: ItemAttributesFieldProps<TFieldValues, TArrayPath>) {
  const currentLocale = useLocale() as "th" | "en";
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");

  // Query templates if categoryId is provided and no templates are explicitly passed
  const categoryTemplateQuery = useCategoryAttributeTemplate(categoryId);
  const activeCategoryTemplates = passedCategoryTemplates ?? categoryTemplateQuery.data ?? null;

  const {
    fields,
    appendAttribute,
    removeAttribute,
    addPreset,
    hasAttributeKey,
    validation,
    definitions,
    suggestedDefinitions,
    categoryDefinitions,
  } = useItemAttributes({
    control,
    name,
    disabled,
    itemType,
    categoryTemplates: activeCategoryTemplates,
  });

  const getDefLabel = (def: AttributeDefinition) => {
    return currentLocale === "th" ? def.labelTh : def.labelEn;
  };

  return (
    <div className={cn("space-y-4", className)}>
      {/* Quick Suggestion Chips (Category-driven) */}
      {!disabled && (
        <div className="flex flex-wrap items-center justify-between gap-3 bg-erp-surface-subtle p-3 border border-erp-border">
          <div className="flex flex-wrap items-center gap-1.5 text-xs text-erp-text-secondary">
            <span className="font-bold text-erp-navy text-[11px] uppercase tracking-wide">
              {t("attributeQuickPresets")}:
            </span>
            {suggestedDefinitions.map((def) => {
              const isAdded = hasAttributeKey(def.key);
              return (
                <button
                  key={def.key}
                  type="button"
                  disabled={isAdded || disabled}
                  onClick={() => addPreset(def.key)}
                  className={cn(
                    "px-2.5 py-1 border text-[11px] transition-colors rounded-none flex items-center gap-1",
                    isAdded
                      ? "bg-erp-surface-muted text-erp-text-muted border-erp-border/60 cursor-not-allowed opacity-60"
                      : "bg-erp-surface hover:bg-erp-surface-muted border-erp-border text-erp-text-main hover:border-erp-navy cursor-pointer"
                  )}
                  title={isAdded ? t("attributeAlreadyAdded") : undefined}
                >
                  <span className="font-bold">+{getDefLabel(def)}</span>
                  <span className="font-mono text-[10px] text-erp-text-muted">({def.key})</span>
                </button>
              );
            })}
          </div>

          <Button
            type="button"
            variant="outline"
            size="sm"
            disabled={disabled}
            onClick={() => appendAttribute()}
            className="text-xs"
          >
            <svg
              className="w-3.5 h-3.5 mr-1"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden="true"
            >
              <path
                strokeLinecap="square"
                strokeLinejoin="miter"
                strokeWidth="2.5"
                d="M12 4v16m8-8H4"
              />
            </svg>
            {t("addAttribute")}
          </Button>
        </div>
      )}

      {/* Controlled Attributes Table */}
      <div className="border border-erp-border overflow-hidden bg-erp-surface">
        <table className="w-full text-left text-xs border-collapse">
          <thead>
            <tr className="bg-erp-surface-muted border-b border-erp-border text-erp-text-secondary">
              <th className="p-3 font-bold w-12 text-center">#</th>
              <th className="p-3 font-bold w-5/12">
                {t("attributeKey")} <span className="text-erp-danger">*</span>
              </th>
              <th className="p-3 font-bold w-5/12">{t("attributeValue")}</th>
              <th className="p-3 font-bold w-20 text-center">{common("actions.actions")}</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-erp-border/60 bg-erp-surface">
            {fields.map((fieldItem, index) => {
              const keyPath = `${name}.${index}.key` as Path<TFieldValues>;
              const valuePath = `${name}.${index}.value` as Path<TFieldValues>;

              return (
                <tr
                  key={fieldItem.id}
                  className="hover:bg-erp-surface-subtle transition-colors"
                >
                  <td className="p-3 text-center text-erp-text-muted font-mono">
                    {index + 1}
                  </td>
                  <td className="p-2">
                    <Controller
                      control={control}
                      name={keyPath}
                      render={({ field, fieldState }) => {
                        const currentKey = (field.value as string) ?? "";
                        const def = getMergedAttributeDefinition(currentKey, categoryDefinitions);

                        return (
                          <div className="space-y-1">
                            <Select
                              value={currentKey}
                              disabled={disabled}
                              placeholder={t("attributeSelectPlaceholder")}
                              className="text-xs py-1.5 h-8 font-medium"
                              options={definitions.map((d) => ({
                                value: d.key,
                                label: `${getDefLabel(d)} (${d.key})`,
                              }))}
                              onChange={(e) => {
                                field.onChange(e.target.value);
                              }}
                              error={
                                fieldState.error?.message === "KEY_REQUIRED"
                                  ? t("attributeKeyRequired")
                                  : undefined
                              }
                            />
                            <div className="flex items-center gap-2 pl-1">
                              {def?.unit && (
                                <span className="text-[10px] text-erp-text-muted font-mono">
                                  {t("attributeUnit")}: {def.unit}
                                </span>
                              )}
                              {def?.isRequired && (
                                <span className="text-[10px] font-bold text-erp-navy bg-erp-surface-muted px-1.5 py-0.5 border border-erp-border">
                                  {t("attributeTemplateRequired")}
                                </span>
                              )}
                            </div>
                          </div>
                        );
                      }}
                    />
                  </td>
                  <td className="p-2">
                    <Controller
                      control={control}
                      name={keyPath}
                      render={({ field: keyField }) => {
                        const currentKey = (keyField.value as string) ?? "";
                        const def = getMergedAttributeDefinition(currentKey, categoryDefinitions);

                        return (
                          <Controller
                            control={control}
                            name={valuePath}
                            render={({ field: valField }) => {
                              const val = (valField.value as string) ?? "";

                              // Controlled Select Option Input if registry or template has options
                              if (def?.options && def.options.length > 0) {
                                return (
                                  <div className="space-y-1">
                                    <Select
                                      value={val}
                                      disabled={disabled}
                                      placeholder={
                                        currentLocale === "th"
                                          ? def.placeholderTh || common("actions.select")
                                          : def.placeholderEn || common("actions.select")
                                      }
                                      className="text-xs py-1.5 h-8"
                                      options={def.options.map((opt) => ({
                                        value: opt.value,
                                        label: currentLocale === "th" ? opt.labelTh : opt.labelEn,
                                      }))}
                                      onChange={(e) => valField.onChange(e.target.value)}
                                    />
                                  </div>
                                );
                              }

                              // Boolean Input
                              if (def?.dataType === "boolean") {
                                return (
                                  <div className="space-y-1">
                                    <Select
                                      value={val}
                                      disabled={disabled}
                                      placeholder={common("actions.select")}
                                      className="text-xs py-1.5 h-8"
                                      options={[
                                        { value: "true", label: t("attributeBooleanTrue") },
                                        { value: "false", label: t("attributeBooleanFalse") },
                                      ]}
                                      onChange={(e) => valField.onChange(e.target.value)}
                                    />
                                  </div>
                                );
                              }

                              // Number Input with Unit Suffix
                              if (def?.dataType === "number") {
                                return (
                                  <div className="relative">
                                    <Input
                                      type="number"
                                      value={val}
                                      disabled={disabled}
                                      placeholder={
                                        currentLocale === "th"
                                          ? def.placeholderTh || t("attributeValuePlaceholder")
                                          : def.placeholderEn || t("attributeValuePlaceholder")
                                      }
                                      className="text-xs py-1.5 h-8 font-mono pr-14"
                                      onChange={(e) => valField.onChange(e.target.value)}
                                    />
                                    {def.unit && (
                                      <span className="absolute right-2 top-2 text-[10px] text-erp-text-muted font-mono pointer-events-none">
                                        {def.unit.split(" ")[0]}
                                      </span>
                                    )}
                                  </div>
                                );
                              }

                              // Default Text Input
                              return (
                                <Input
                                  value={val}
                                  disabled={disabled}
                                  placeholder={
                                    def
                                      ? currentLocale === "th"
                                        ? def.placeholderTh
                                        : def.placeholderEn
                                      : t("attributeValuePlaceholder")
                                  }
                                  className="text-xs py-1.5 h-8"
                                  onChange={(e) => valField.onChange(e.target.value)}
                                />
                              );
                            }}
                          />
                        );
                      }}
                    />
                  </td>
                  <td className="p-2 text-center">
                    <button
                      type="button"
                      disabled={disabled}
                      onClick={() => removeAttribute(index)}
                      className="p-1.5 text-erp-text-muted hover:text-erp-danger hover:bg-erp-danger-bg border border-transparent hover:border-erp-danger/20 transition-colors disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
                      title={t("removeAttribute")}
                      aria-label={t("removeAttribute")}
                    >
                      <svg
                        className="w-4 h-4"
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                        aria-hidden="true"
                      >
                        <path
                          strokeLinecap="square"
                          strokeLinejoin="miter"
                          strokeWidth="2"
                          d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                        />
                      </svg>
                    </button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>

        {/* Empty State */}
        {fields.length === 0 && (
          <div className="p-8 text-center bg-erp-surface-subtle">
            <svg
              className="w-8 h-8 text-erp-text-muted/60 mx-auto mb-2"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              aria-hidden="true"
            >
              <path
                strokeLinecap="square"
                strokeLinejoin="miter"
                strokeWidth="1.5"
                d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
              />
            </svg>
            <p className="text-xs font-semibold text-erp-text-main">
              {t("noAttributes")}
            </p>
            <p className="text-[11px] text-erp-text-muted mt-0.5">
              {t("noAttributesHelp")}
            </p>
          </div>
        )}
      </div>

      {/* Validation alerts */}
      {validation.duplicateKeys.length > 0 && (
        <div
          role="alert"
          className="border border-erp-danger bg-erp-danger-bg p-3 text-xs text-erp-danger flex items-start gap-2"
        >
          <svg
            className="w-4 h-4 text-erp-danger mt-0.5 shrink-0"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <path
              strokeLinecap="square"
              strokeLinejoin="miter"
              strokeWidth="2"
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <div>
            <p className="font-bold">{t("errorDuplicateAttributeKey")}:</p>
            <p className="font-mono text-[11px] mt-0.5">
              {validation.duplicateKeys.join(", ")}
            </p>
          </div>
        </div>
      )}

      {validation.reservedKeys.length > 0 && (
        <div
          role="alert"
          className="border border-erp-danger bg-erp-danger-bg p-3 text-xs text-erp-danger flex items-start gap-2"
        >
          <svg
            className="w-4 h-4 text-erp-danger mt-0.5 shrink-0"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <path
              strokeLinecap="square"
              strokeLinejoin="miter"
              strokeWidth="2"
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <div>
            <p className="font-bold">{t("errorReservedAttributeKey")}:</p>
            <p className="font-mono text-[11px] mt-0.5">
              {validation.reservedKeys.join(", ")}
            </p>
          </div>
        </div>
      )}
    </div>
  );
}
