import { useMemo } from "react";
import {
  useFieldArray,
  useWatch,
  type ArrayPath,
  type Control,
  type FieldValues,
  type Path,
} from "react-hook-form";
import {
  validateAttributePairs,
  type AttributeValidationResult,
} from "@/lib/utils/item-attributes";
import {
  CONTROLLED_ATTRIBUTE_REGISTRY,
  type AttributeDefinition,
  type CategoryAttributeOptionDto,
  type CategoryAttributeTemplateDto,
  type CategoryAttributeTemplateResponse,
  type ItemAttributePair,
} from "@/types/item-attributes";

export interface UseItemAttributesOptions<
  TFieldValues extends FieldValues,
  TArrayPath extends ArrayPath<TFieldValues> = ArrayPath<TFieldValues>,
> {
  control: Control<TFieldValues>;
  name: TArrayPath;
  disabled?: boolean;
  itemType?: string;
  categoryTemplates?: CategoryAttributeTemplateResponse | null;
}

export interface UseItemAttributesReturn {
  fields: Array<ItemAttributePair & { id: string }>;
  appendAttribute: (key?: string, value?: string) => void;
  removeAttribute: (index: number) => void;
  updateAttribute: (index: number, pair: ItemAttributePair) => void;
  addPreset: (key: string, defaultValue?: string) => boolean;
  hasAttributeKey: (key: string) => boolean;
  validation: AttributeValidationResult;
  definitions: AttributeDefinition[];
  suggestedDefinitions: AttributeDefinition[];
  categoryDefinitions: AttributeDefinition[];
  disabled: boolean;
}

export function useItemAttributes<
  TFieldValues extends FieldValues,
  TArrayPath extends ArrayPath<TFieldValues> = ArrayPath<TFieldValues>,
>({
  control,
  name,
  disabled = false,
  itemType,
  categoryTemplates,
}: UseItemAttributesOptions<TFieldValues, TArrayPath>): UseItemAttributesReturn {
  const { fields, append, remove, update } = useFieldArray({
    control,
    name,
  });

  const watchedValues = useWatch({
    control,
    name: name as unknown as Path<TFieldValues>,
  }) as ItemAttributePair[] | undefined;

  const validation = useMemo<AttributeValidationResult>(() => {
    return validateAttributePairs(watchedValues ?? []);
  }, [watchedValues]);

  const hasAttributeKey = (key: string): boolean => {
    const target = key.trim().toLowerCase();
    if (!watchedValues) return false;
    return watchedValues.some((p) => p.key?.trim().toLowerCase() === target);
  };

  const categoryDefinitions = useMemo<AttributeDefinition[]>(() => {
    if (!categoryTemplates?.templates || categoryTemplates.templates.length === 0) {
      return [];
    }

    return categoryTemplates.templates.map((tmpl: CategoryAttributeTemplateDto) => {
      const rawDataType = (tmpl.dataType ?? "text").toLowerCase();
      const dataType =
        rawDataType === "number" || rawDataType === "select" || rawDataType === "boolean"
          ? rawDataType
          : "text";

      const options = tmpl.options?.map((opt: CategoryAttributeOptionDto) => ({
        value: opt.value ?? "",
        labelTh: opt.label?.thai ?? opt.value ?? "",
        labelEn: opt.label?.english ?? opt.label?.thai ?? opt.value ?? "",
      }));

      return {
        key: tmpl.key ?? "",
        labelTh: tmpl.name?.thai ?? tmpl.key ?? "",
        labelEn: tmpl.name?.english ?? tmpl.name?.thai ?? tmpl.key ?? "",
        dataType,
        unit: tmpl.unit ?? undefined,
        isRequired: tmpl.isRequired ?? false,
        defaultValue: tmpl.defaultValue ?? undefined,
        options,
      };
    });
  }, [categoryTemplates]);

  const allDefinitions = useMemo<AttributeDefinition[]>(() => {
    if (categoryDefinitions.length === 0) {
      return CONTROLLED_ATTRIBUTE_REGISTRY;
    }

    const merged = [...categoryDefinitions];
    for (const reg of CONTROLLED_ATTRIBUTE_REGISTRY) {
      if (!merged.some((d) => d.key.toLowerCase() === reg.key.toLowerCase())) {
        merged.push(reg);
      }
    }
    return merged;
  }, [categoryDefinitions]);

  const suggestedDefinitions = useMemo(() => {
    if (categoryDefinitions.length > 0) {
      return categoryDefinitions;
    }
    if (!itemType) return CONTROLLED_ATTRIBUTE_REGISTRY.slice(0, 5);
    const normalized = itemType.toLowerCase();
    return CONTROLLED_ATTRIBUTE_REGISTRY.filter((def) =>
      def.suggestedCategories?.some((cat) => cat.toLowerCase() === normalized)
    );
  }, [categoryDefinitions, itemType]);

  const appendAttribute = (key = "", value = "") => {
    if (disabled) return;
    const item = { key, value };
    append(item as unknown as Parameters<typeof append>[0]);
  };

  const removeAttribute = (index: number) => {
    if (disabled) return;
    remove(index);
  };

  const updateAttribute = (index: number, pair: ItemAttributePair) => {
    if (disabled) return;
    update(index, pair as unknown as Parameters<typeof update>[1]);
  };

  const addPreset = (key: string, defaultValue = ""): boolean => {
    if (disabled) return false;
    if (hasAttributeKey(key)) return false;
    const item = { key, value: defaultValue };
    append(item as unknown as Parameters<typeof append>[0]);
    return true;
  };

  return {
    fields: fields as unknown as Array<ItemAttributePair & { id: string }>,
    appendAttribute,
    removeAttribute,
    updateAttribute,
    addPreset,
    hasAttributeKey,
    validation,
    definitions: allDefinitions,
    suggestedDefinitions:
      suggestedDefinitions.length > 0
        ? suggestedDefinitions
        : CONTROLLED_ATTRIBUTE_REGISTRY.slice(0, 5),
    categoryDefinitions,
    disabled,
  };
}
