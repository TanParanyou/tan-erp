"use client";

import { useEffect, useState } from "react";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Checkbox } from "@/components/ui/Checkbox";
import { useToast } from "@/hooks/useToast";
import {
  useCategoryAttributeTemplate,
  useCategoryAttributeTemplateMutations,
} from "@/features/item-master/api/item-master-queries";
import type {
  CategoryAttributeTemplateDto,
} from "@/types/item-attributes";
import { RESERVED_ATTRIBUTE_KEYS } from "@/types/item-attributes";

interface Props {
  isOpen: boolean;
  onClose: () => void;
  categoryId: string | null;
  categoryCode?: string | null;
  categoryName?: string | null;
}

interface EditableOption {
  value: string;
  labelTh: string;
  labelEn: string;
}

interface EditableTemplate {
  key: string;
  nameTh: string;
  nameEn: string;
  dataType: "text" | "number" | "select" | "boolean";
  unit: string;
  isRequired: boolean;
  defaultValue: string;
  options: EditableOption[];
}

export function CategoryAttributeTemplateDrawer({
  isOpen,
  onClose,
  categoryId,
  categoryCode,
  categoryName,
}: Props) {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const { toast } = useToast();

  const { data: templateResponse, isLoading, isError, refetch } = useCategoryAttributeTemplate(categoryId ?? undefined);
  const { setTemplates } = useCategoryAttributeTemplateMutations(categoryId ?? undefined);

  // Local state for direct templates being edited
  const [templates, setTemplatesState] = useState<EditableTemplate[]>([]);
  const [inheritedTemplates, setInheritedTemplates] = useState<CategoryAttributeTemplateDto[]>([]);
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Sync state when templateResponse changes or drawer opens
  useEffect(() => {
    if (!templateResponse || !categoryId) {
      setTemplatesState([]);
      setInheritedTemplates([]);
      setErrors({});
      return;
    }

    // In current contract, all templates returned from backend for this category
    // (If backend returns list of templates)
    const direct: EditableTemplate[] = [];
    const inherited: CategoryAttributeTemplateDto[] = [];

    const list = templateResponse.templates ?? [];
    for (const item of list) {
      const options: EditableOption[] = (item.options ?? []).map((opt) => ({
        value: opt.value ?? "",
        labelTh: opt.label?.thai ?? opt.value ?? "",
        labelEn: opt.label?.english ?? "",
      }));

      direct.push({
        key: item.key ?? "",
        nameTh: item.name?.thai ?? "",
        nameEn: item.name?.english ?? "",
        dataType: (item.dataType as EditableTemplate["dataType"]) ?? "text",
        unit: item.unit ?? "",
        isRequired: Boolean(item.isRequired),
        defaultValue: item.defaultValue ?? "",
        options,
      });
    }

    setTemplatesState(direct);
    setInheritedTemplates(inherited);
    setErrors({});
  }, [templateResponse, categoryId, isOpen]);

  const addTemplate = () => {
    setTemplatesState((prev) => [
      ...prev,
      {
        key: "",
        nameTh: "",
        nameEn: "",
        dataType: "text",
        unit: "",
        isRequired: false,
        defaultValue: "",
        options: [],
      },
    ]);
  };

  const removeTemplate = (index: number) => {
    setTemplatesState((prev) => prev.filter((_, idx) => idx !== index));
    setErrors({});
  };

  const updateTemplate = <K extends keyof EditableTemplate>(
    index: number,
    field: K,
    value: EditableTemplate[K]
  ) => {
    setTemplatesState((prev) =>
      prev.map((item, idx) => {
        if (idx !== index) return item;
        const updated = { ...item, [field]: value };
        // If data type changed away from select, clear options
        if (field === "dataType" && value !== "select") {
          updated.options = [];
        }
        return updated;
      })
    );
  };

  const addOption = (templateIndex: number) => {
    setTemplatesState((prev) =>
      prev.map((item, idx) => {
        if (idx !== templateIndex) return item;
        const newOptions: EditableOption[] = [
          ...item.options,
          { value: "", labelTh: "", labelEn: "" },
        ];
        return { ...item, options: newOptions };
      })
    );
  };

  const removeOption = (templateIndex: number, optionIndex: number) => {
    setTemplatesState((prev) =>
      prev.map((item, idx) => {
        if (idx !== templateIndex) return item;
        return {
          ...item,
          options: item.options.filter((_, oIdx) => oIdx !== optionIndex),
        };
      })
    );
  };

  const updateOption = (
    templateIndex: number,
    optionIndex: number,
    field: keyof EditableOption,
    value: string
  ) => {
    setTemplatesState((prev) =>
      prev.map((item, idx) => {
        if (idx !== templateIndex) return item;
        const nextOptions = item.options.map((opt, oIdx) => {
          if (oIdx !== optionIndex) return opt;
          return { ...opt, [field]: value };
        });
        return { ...item, options: nextOptions };
      })
    );
  };

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};
    const seenKeys = new Set<string>();

    for (const inh of inheritedTemplates) {
      if (inh.key) seenKeys.add(inh.key.toLowerCase());
    }

    const keyPattern = /^[a-z0-9_]{2,64}$/;

    templates.forEach((item, index) => {
      const cleanKey = item.key.trim().toLowerCase();
      if (!cleanKey || !keyPattern.test(cleanKey)) {
        newErrors[`key_${index}`] = t("templateKeyPatternError");
      } else if (RESERVED_ATTRIBUTE_KEYS.includes(cleanKey as typeof RESERVED_ATTRIBUTE_KEYS[number])) {
        newErrors[`key_${index}`] = t("templateKeyReservedError");
      } else if (seenKeys.has(cleanKey)) {
        newErrors[`key_${index}`] = t("templateKeyDuplicateError");
      } else {
        seenKeys.add(cleanKey);
      }

      if (!item.nameTh.trim()) {
        newErrors[`nameTh_${index}`] = t("templateNameThRequired");
      }

      if (item.dataType === "select") {
        if (item.options.length === 0) {
          newErrors[`options_${index}`] = t("templateOptionRequired");
        } else {
          const optionVals = new Set<string>();
          item.options.forEach((opt, optIdx) => {
            const v = opt.value.trim().toLowerCase();
            if (!v || optionVals.has(v)) {
              newErrors[`opt_${index}_${optIdx}`] = t("templateOptionDuplicate");
            } else {
              optionVals.add(v);
            }
          });
        }
      }
    });

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!categoryId) return;
    if (!validate()) return;

    try {
      const payloadTemplates: CategoryAttributeTemplateDto[] = templates.map((tmpl) => ({
        key: tmpl.key.trim().toLowerCase(),
        name: {
          thai: tmpl.nameTh.trim(),
          english: tmpl.nameEn.trim() || null,
        },
        dataType: tmpl.dataType,
        unit: tmpl.unit.trim() || null,
        isRequired: tmpl.isRequired,
        defaultValue: tmpl.defaultValue.trim() || null,
        options:
          tmpl.dataType === "select"
            ? tmpl.options.map((opt) => ({
                value: opt.value.trim(),
                label: {
                  thai: opt.labelTh.trim() || opt.value.trim(),
                  english: opt.labelEn.trim() || null,
                },
              }))
            : null,
      }));

      await setTemplates.mutateAsync({ templates: payloadTemplates });
      toast.success(t("templateSaveSuccess"));
      onClose();
    } catch {
      toast.error(t("templateSaveFailed"));
    }
  };

  return (
    <Drawer
      isOpen={isOpen}
      onClose={onClose}
      size="xl"
      title={t("categoryTemplatesTitle", { code: categoryCode || "-" })}
      description={categoryName ? `${categoryName} — ${t("categoryTemplatesDesc")}` : t("categoryTemplatesDesc")}
      footer={
        <div className="flex items-center justify-end gap-3 w-full">
          <Button type="button" variant="outline" onClick={onClose} disabled={setTemplates.isPending}>
            {common("actions.cancel")}
          </Button>
          <Button
            type="button"
            variant="primary"
            onClick={() => void handleSave()}
            isLoading={setTemplates.isPending}
            disabled={setTemplates.isPending || isLoading}
          >
            {t("templateSave")}
          </Button>
        </div>
      }
    >
      <div className="space-y-6">
        {isLoading && (
          <div className="py-8 text-center text-sm text-erp-muted animate-pulse">
            {common("status.loading")}
          </div>
        )}

        {isError && (
          <div className="border border-erp-danger bg-erp-danger/5 p-4 text-sm text-erp-danger flex items-center justify-between">
            <span>{t("templateSaveFailed")}</span>
            <Button size="sm" variant="outline" onClick={() => void refetch()}>
              {common("actions.retry")}
            </Button>
          </div>
        )}

        {!isLoading && (
          <>
            {/* Inherited Templates Section */}
            {inheritedTemplates.length > 0 && (
              <div className="space-y-3">
                <div className="flex items-center justify-between border-b border-erp-border pb-2">
                  <h3 className="text-sm font-semibold text-erp-text-main flex items-center gap-2">
                    <span>{t("inheritedCategoryTemplates")}</span>
                    <span className="bg-erp-muted/20 text-erp-text-muted text-xs px-2 py-0.5 font-mono">
                      {inheritedTemplates.length}
                    </span>
                  </h3>
                  <span className="text-xs text-erp-muted">{t("templateInheritedBadge")}</span>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                  {inheritedTemplates.map((inh) => (
                    <div
                      key={inh.key ?? ""}
                      className="border border-erp-border bg-erp-surface-secondary/50 p-3 space-y-1 text-xs"
                    >
                      <div className="flex items-center justify-between">
                        <span className="font-mono font-bold text-erp-navy">{inh.key}</span>
                        <span className="text-erp-muted uppercase font-mono">{inh.dataType}</span>
                      </div>
                      <div className="text-erp-text-main font-medium">
                        {inh.name?.thai} {inh.name?.english ? `(${inh.name.english})` : ""}
                      </div>
                      {inh.unit && <div className="text-erp-muted">{t("templateUnit")}: {inh.unit}</div>}
                      {inh.isRequired && (
                        <span className="inline-block bg-erp-danger/10 text-erp-danger px-1.5 py-0.2 font-semibold">
                          {t("templateRequired")}
                        </span>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Direct Templates Section */}
            <div className="space-y-4">
              <div className="flex items-center justify-between border-b border-erp-border pb-2">
                <h3 className="text-sm font-semibold text-erp-text-main flex items-center gap-2">
                  <span>{t("directCategoryTemplates")}</span>
                  <span className="bg-erp-navy/10 text-erp-navy text-xs px-2 py-0.5 font-mono font-bold">
                    {templates.length}
                  </span>
                </h3>
                <Button type="button" size="sm" variant="outline" onClick={addTemplate}>
                  + {t("templateAdd")}
                </Button>
              </div>

              {templates.length === 0 ? (
                <div className="py-8 text-center border border-dashed border-erp-border p-6 text-sm text-erp-muted">
                  <p>{t("templateEmpty")}</p>
                  <Button type="button" size="sm" variant="outline" className="mt-3" onClick={addTemplate}>
                    + {t("templateAdd")}
                  </Button>
                </div>
              ) : (
                <div className="space-y-4">
                  {templates.map((item, index) => {
                    const keyError = errors[`key_${index}`];
                    const nameThError = errors[`nameTh_${index}`];
                    const optionsError = errors[`options_${index}`];

                    return (
                      <div
                        key={index}
                        className="border border-erp-border bg-erp-surface p-4 space-y-4 relative"
                      >
                        {/* Header with index and remove */}
                        <div className="flex items-center justify-between border-b border-erp-border/60 pb-2">
                          <span className="text-xs font-mono font-bold text-erp-navy">
                            #{index + 1}
                          </span>
                          <Button
                            type="button"
                            size="sm"
                            variant="danger"
                            onClick={() => removeTemplate(index)}
                          >
                            {t("templateRemove")}
                          </Button>
                        </div>

                        {/* Top inputs: Key, Name TH, Name EN */}
                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                          <Input
                            label={t("templateKey")}
                            placeholder="e.g. thickness_mm"
                            value={item.key}
                            onChange={(e) => updateTemplate(index, "key", e.target.value)}
                            error={keyError}
                            required
                          />
                          <Input
                            label={`${t("templateName")} (TH)`}
                            placeholder="เช่น ความหนา"
                            value={item.nameTh}
                            onChange={(e) => updateTemplate(index, "nameTh", e.target.value)}
                            error={nameThError}
                            required
                          />
                          <Input
                            label={`${t("templateName")} (EN)`}
                            placeholder="e.g. Thickness"
                            value={item.nameEn}
                            onChange={(e) => updateTemplate(index, "nameEn", e.target.value)}
                          />
                        </div>

                        {/* Middle inputs: DataType, Unit, Default Value, Required */}
                        <div className="grid grid-cols-1 sm:grid-cols-4 gap-3 items-end">
                          <Select
                            label={t("templateDataType")}
                            value={item.dataType}
                            onChange={(e) =>
                              updateTemplate(
                                index,
                                "dataType",
                                e.target.value as EditableTemplate["dataType"]
                              )
                            }
                            options={[
                              { value: "text", label: "Text (ข้อความ)" },
                              { value: "number", label: "Number (ตัวเลข)" },
                              { value: "select", label: "Select (ตัวเลือก)" },
                              { value: "boolean", label: "Boolean (ใช่/ไม่ใช่)" },
                            ]}
                          />
                          <Input
                            label={t("templateUnit")}
                            placeholder="เช่น mm, kg, แผ่น"
                            value={item.unit}
                            onChange={(e) => updateTemplate(index, "unit", e.target.value)}
                          />
                          <Input
                            label={t("templateDefaultValue")}
                            placeholder="ค่าเริ่มต้น (ถ้ามี)"
                            value={item.defaultValue}
                            onChange={(e) => updateTemplate(index, "defaultValue", e.target.value)}
                          />
                          <div className="h-10 flex items-center">
                            <Checkbox
                              id={`req_${index}`}
                              label={t("templateRequired")}
                              checked={item.isRequired}
                              onChange={(e) => updateTemplate(index, "isRequired", e.target.checked)}
                            />
                          </div>
                        </div>

                        {/* Options Section for Select type */}
                        {item.dataType === "select" && (
                          <div className="border-t border-erp-border/60 pt-3 space-y-3">
                            <div className="flex items-center justify-between">
                              <span className="text-xs font-semibold text-erp-text-main">
                                {t("templateOptions")}
                              </span>
                              <Button
                                type="button"
                                size="sm"
                                variant="outline"
                                onClick={() => addOption(index)}
                              >
                                + {t("templateAddOption")}
                              </Button>
                            </div>

                            {optionsError && (
                              <p className="text-xs text-erp-danger">{optionsError}</p>
                            )}

                            {item.options.length === 0 ? (
                              <p className="text-xs text-erp-muted italic">
                                {t("templateOptionRequired")}
                              </p>
                            ) : (
                              <div className="space-y-2">
                                {item.options.map((opt, optIdx) => {
                                  const optErr = errors[`opt_${index}_${optIdx}`];
                                  return (
                                    <div
                                      key={optIdx}
                                      className="grid grid-cols-1 sm:grid-cols-4 gap-2 items-center bg-erp-surface-secondary/40 p-2 border border-erp-border/40"
                                    >
                                      <Input
                                        placeholder={t("templateOptionValue")}
                                        value={opt.value}
                                        onChange={(e) =>
                                          updateOption(index, optIdx, "value", e.target.value)
                                        }
                                        error={optErr}
                                      />
                                      <Input
                                        placeholder={t("templateOptionLabelTh")}
                                        value={opt.labelTh}
                                        onChange={(e) =>
                                          updateOption(index, optIdx, "labelTh", e.target.value)
                                        }
                                      />
                                      <Input
                                        placeholder={t("templateOptionLabelEn")}
                                        value={opt.labelEn}
                                        onChange={(e) =>
                                          updateOption(index, optIdx, "labelEn", e.target.value)
                                        }
                                      />
                                      <div className="flex justify-end">
                                        <Button
                                          type="button"
                                          size="sm"
                                          variant="ghost"
                                          className="text-erp-danger hover:bg-erp-danger/10"
                                          onClick={() => removeOption(index, optIdx)}
                                        >
                                          {t("templateRemoveOption")}
                                        </Button>
                                      </div>
                                    </div>
                                  );
                                })}
                              </div>
                            )}
                          </div>
                        )}
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          </>
        )}
      </div>
    </Drawer>
  );
}
