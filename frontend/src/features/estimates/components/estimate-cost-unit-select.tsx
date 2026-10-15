"use client";

import { useFormContext, useWatch } from "react-hook-form";
import { useTranslations } from "next-intl";
import { Select } from "@/components/ui/Select";
import { useEstimateUnitOptions } from "../hooks/use-estimate-unit-options";
import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";

interface EstimateCostUnitSelectProps {
  sectionIndex: number;
  itemIndex: number;
  costIndex: number;
  branchId?: string;
}

/** Unit picker for a catalog-linked cost line. Renders nothing unless the item has converted units the effective policy allows. */
export function EstimateCostUnitSelect(props: EstimateCostUnitSelectProps) {
  const { control } = useFormContext<EstimateWorkspaceFormData>();
  const itemId = useWatch({
    control,
    name: `sections.${props.sectionIndex}.workItems.${props.itemIndex}.costComponents.${props.costIndex}.itemId`,
  });
  if (!itemId) return null;
  return <CatalogCostUnitSelect {...props} itemId={itemId} />;
}

function CatalogCostUnitSelect({ sectionIndex, itemIndex, costIndex, branchId, itemId }: EstimateCostUnitSelectProps & { itemId: string }) {
  const t = useTranslations("estimates");
  const path = `sections.${sectionIndex}.workItems.${itemIndex}.costComponents.${costIndex}` as const;
  const { control, setValue } = useFormContext<EstimateWorkspaceFormData>();
  const unitCode = useWatch({ control, name: `${path}.unitCode` });
  const baseUnitCost = useWatch({ control, name: `${path}.baseUnitCost` });
  const { data } = useEstimateUnitOptions({ branchId, itemId });

  const units = data?.units;
  const baseUnitCode = data?.baseUnitCode;
  if (!data?.conversionsEnabled || !units || !baseUnitCode || units.length < 2) return null;

  const unitsByCode = new Map(units.flatMap((unit) => (unit.unitCode ? [[unit.unitCode, unit] as const] : [])));
  const options = Array.from(unitsByCode.values()).map((unit) => ({
    value: unit.unitCode ?? "",
    label: unit.isBase
      ? t("unitOptionBase", { unit: unit.unitCode ?? "-" })
      : t("unitOptionConverted", { unit: unit.unitCode ?? "-", factor: unit.factor ?? "-", base: baseUnitCode }),
  }));

  function handleChange(code: string) {
    const option = unitsByCode.get(code);
    if (!option?.unitCode || baseUnitCost === null || baseUnitCost === undefined) return;
    const frozen = !option.isBase && option.conversionId && option.conversionScope && option.factor
      ? { conversionId: option.conversionId, scope: option.conversionScope, fromUnit: option.unitCode, toUnit: baseUnitCode!, factor: option.factor }
      : null;
    const factor = frozen ? Number(frozen.factor) : 1;
    setValue(`${path}.unitCode`, option.unitCode, { shouldDirty: true });
    setValue(`${path}.conversion`, frozen, { shouldDirty: true });
    // Preview only: the server recomputes the line from the frozen factor when the draft is saved.
    setValue(`${path}.unitCost`, Math.round(baseUnitCost * factor * 10000) / 10000, { shouldDirty: true });
  }

  return (
    <Select
      id={`cost-unit-${sectionIndex}-${itemIndex}-${costIndex}`}
      label={t("unitSelectLabel")}
      options={options}
      value={unitCode}
      disabled={baseUnitCost === null || baseUnitCost === undefined}
      onChange={(event) => handleChange(event.target.value)}
      wrapperClassName="col-span-2 mb-0"
    />
  );
}
