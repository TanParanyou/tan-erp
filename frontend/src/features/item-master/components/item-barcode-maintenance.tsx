"use client";

import { useState } from "react";
import type { FormEvent } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Input } from "@/components/ui/Input";
import { Table, TableHeader, TableBody, TableRow, TableHead, TableCell } from "@/components/ui/Table";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import type { ItemResponse } from "@/lib/api/api-client";
import { useItemBarcodeMutations, useItemBarcodes, useItemMasterLookups, useItemUnitConversionMutations, useItemUnitConversions, useSharedUnitConversions } from "@/features/item-master/api/item-master-queries";

interface ItemBarcodeMaintenanceProps {
  itemId: string;
  item: ItemResponse;
}

export function ItemBarcodeMaintenance({ itemId, item }: ItemBarcodeMaintenanceProps) {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const query = useItemBarcodes(itemId);
  const mutations = useItemBarcodeMutations(itemId);
  const lookups = useItemMasterLookups();
  const conversions = useItemUnitConversions(itemId);
  const sharedConversions = useSharedUnitConversions();
  const conversionMutations = useItemUnitConversionMutations(itemId);
  const [identifierType, setIdentifierType] = useState("gtin");
  const [value, setValue] = useState("");
  const [unitId, setUnitId] = useState(item.baseUnit?.id ?? "");
  const [quantity, setQuantity] = useState("1");
  const [packagingLevel, setPackagingLevel] = useState("each");
  const [isPrimary, setIsPrimary] = useState(false);
  const [conversionUnitId, setConversionUnitId] = useState("");
  const [conversionFactor, setConversionFactor] = useState("");
  const [conversionEffectiveFrom, setConversionEffectiveFrom] = useState(() => new Date().toISOString().slice(0, 10));
  const [conversionEffectiveTo, setConversionEffectiveTo] = useState("");
  const [conversionReason, setConversionReason] = useState("");
  const [deactivateTarget, setDeactivateTarget] = useState<{ id: string; rowVersion: string; value: string } | null>(null);
  const canManage = can(selectedMembership, "items.manage-barcodes");
  const busy = mutations.create.isPending || mutations.setPrimary.isPending || mutations.deactivate.isPending || conversionMutations.create.isPending;
  const baseUnit = item.baseUnit;

  const create = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      await mutations.create.mutateAsync({ identifierType, value: value.trim(), unitId, quantityInBaseUnit: quantity, packagingLevel, isPrimary });
      setValue("");
    } catch {
      // The mutation state renders the API error next to this form.
    }
  };

  const createConversion = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!conversionUnitId || !baseUnit?.id) return;
    try {
      await conversionMutations.create.mutateAsync({
        fromUnitId: conversionUnitId,
        toUnitId: baseUnit.id,
        factor: conversionFactor,
        effectiveFrom: conversionEffectiveFrom,
        effectiveTo: conversionEffectiveTo || null,
        reason: conversionReason.trim(),
      });
      setUnitId(conversionUnitId);
      setQuantity(conversionFactor);
      setConversionEffectiveTo("");
      setConversionReason("");
    } catch {
      // Keep the draft available so the user can correct and retry.
    }
  };

  const activeConversions = (conversions.data ?? []).filter((conversion) => conversion.status === "active");
  const today = new Date().toISOString().slice(0, 10);
  const effectiveConversions = activeConversions.filter((conversion) => Boolean(conversion.effectiveFrom && conversion.effectiveFrom <= today && (!conversion.effectiveTo || conversion.effectiveTo >= today)));
  const effectiveSharedConversions = (sharedConversions.data ?? []).filter((conversion) => conversion.status === "active" && Boolean(conversion.effectiveFrom && conversion.effectiveFrom <= today && (!conversion.effectiveTo || conversion.effectiveTo >= today)));

  const confirmDeactivate = async () => {
    if (!deactivateTarget) return;
    try {
      await mutations.deactivate.mutateAsync({ barcodeId: deactivateTarget.id, rowVersion: deactivateTarget.rowVersion });
      setDeactivateTarget(null);
    } catch {
      // Keep the confirmation open so the user can review the failure and retry.
    }
  };

  return (
    <section className="space-y-4 border border-erp-border bg-erp-surface p-4 sm:p-6" aria-busy={query.isLoading || busy}>
      <div>
        <h2 className="text-lg font-semibold text-erp-ink">{t("barcodes")}</h2>
        <p className="mt-1 text-sm text-erp-muted">{t("barcodeHelp")}</p>
      </div>

      {query.isError && <p role="alert" className="border border-erp-danger p-3 text-sm text-erp-danger">{t("barcodeLoadFailed")}</p>}
      {mutations.create.isError && <p role="alert" className="border border-erp-danger p-3 text-sm text-erp-danger">{t("barcodeSaveFailed")}</p>}
      {(mutations.setPrimary.isError || mutations.deactivate.isError) && <p role="alert" className="border border-erp-danger p-3 text-sm text-erp-danger">{t("versionConflict")}</p>}

      {canManage && item.status !== "inactive" && <form onSubmit={(event) => { void createConversion(event); }} className="grid grid-cols-1 gap-3 border-y border-erp-border py-4 sm:grid-cols-2 lg:grid-cols-6">
        <div className="sm:col-span-2 lg:col-span-6">
          <h3 className="text-sm font-semibold text-erp-ink">{t("unitConversions")}</h3>
          <p className="mt-1 text-xs text-erp-muted">{t("unitConversionHelp", { baseUnit: baseUnit?.code ?? "-" })}</p>
        </div>
        <div className="erp-form-group">
          <label className="erp-label" htmlFor="conversion-unit">{t("fromUnit")}</label>
          <select id="conversion-unit" className="erp-input" value={conversionUnitId} onChange={(event) => setConversionUnitId(event.target.value)} required disabled={busy || lookups.isLoading}>
            <option value="">{t("selectUnit")}</option>
            {lookups.data?.units.filter((unit) => unit.status === "active" && unit.id !== baseUnit?.id).map((unit) => <option key={unit.id} value={unit.id}>{unit.code} · {locale === "en" ? unit.name?.english ?? "-" : unit.name?.thai ?? "-"}</option>)}
          </select>
        </div>
        <Input label={t("conversionFactor")} type="number" min="0.000001" step="0.000001" value={conversionFactor} onChange={(event) => setConversionFactor(event.target.value)} required disabled={busy} />
        <Input label={t("effectiveFrom")} type="date" value={conversionEffectiveFrom} onChange={(event) => setConversionEffectiveFrom(event.target.value)} required disabled={busy} />
        <Input label={t("effectiveTo")} type="date" value={conversionEffectiveTo} onChange={(event) => setConversionEffectiveTo(event.target.value)} disabled={busy} />
        <div className="sm:col-span-2 lg:col-span-2">
          <Input label={t("reason")} value={conversionReason} onChange={(event) => setConversionReason(event.target.value)} required disabled={busy} maxLength={500} />
        </div>
        <div className="flex items-end justify-end lg:col-span-6"><Button type="submit" disabled={busy || !conversionUnitId || !conversionFactor || !conversionReason.trim()} isLoading={conversionMutations.create.isPending}>{t("addConversion")}</Button></div>
        {conversionMutations.create.isError && <p role="alert" className="sm:col-span-2 lg:col-span-6 border border-erp-danger p-3 text-sm text-erp-danger">{t("conversionSaveFailed")}</p>}
        {(conversions.isError || sharedConversions.isError) && <p role="alert" className="sm:col-span-2 lg:col-span-6 border border-erp-danger p-3 text-sm text-erp-danger">{t("conversionLoadFailed")}</p>}
        {activeConversions.length > 0 && (
          <div className="sm:col-span-2 lg:col-span-6">
            <Table wrapperClassName="min-w-[680px]">
              <TableHeader>
                <TableRow>
                  <TableHead className="p-2 text-xs">{t("fromUnit")}</TableHead>
                  <TableHead className="p-2 text-xs">{t("conversionFactor")}</TableHead>
                  <TableHead className="p-2 text-xs">{t("effectiveFrom")}</TableHead>
                  <TableHead className="p-2 text-xs">{t("effectiveTo")}</TableHead>
                  <TableHead className="p-2 text-xs">{t("reason")}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {activeConversions.map((conversion) => (
                  <TableRow key={conversion.id}>
                    <TableCell className="p-2 text-xs">{conversion.fromUnitCode ?? "-"} → {conversion.toUnitCode ?? "-"}</TableCell>
                    <TableCell className="p-2 text-xs">{conversion.factor ?? "-"}</TableCell>
                    <TableCell className="p-2 text-xs">{conversion.effectiveFrom ?? "-"}</TableCell>
                    <TableCell className="p-2 text-xs">{conversion.effectiveTo ?? "-"}</TableCell>
                    <TableCell className="p-2 text-xs">{conversion.reason ?? "-"}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </form>}

      {canManage && item.status !== "inactive" && <form onSubmit={(event) => { void create(event); }} className="grid grid-cols-1 gap-3 border-y border-erp-border py-4 sm:grid-cols-2 lg:grid-cols-6">
        <div className="erp-form-group">
          <label className="erp-label" htmlFor="barcode-type">{t("identifierType")}</label>
          <select id="barcode-type" className="erp-input" value={identifierType} onChange={(event) => setIdentifierType(event.target.value)} disabled={busy}>
            <option value="gtin">GTIN</option><option value="internal">{t("internalBarcode")}</option>
          </select>
        </div>
        <div className="sm:col-span-1 lg:col-span-2">
          <Input label={t("barcodeValue")} value={value} onChange={(event) => setValue(event.target.value)} required disabled={busy} maxLength={64} />
        </div>
        <div className="erp-form-group">
          <label className="erp-label" htmlFor="barcode-unit">{t("unit")}</label>
          <select id="barcode-unit" className="erp-input" value={unitId} onChange={(event) => {
            setUnitId(event.target.value);
            const conversion = effectiveConversions.find((row) => row.fromUnitId === event.target.value && row.toUnitId === baseUnit?.id)
              ?? effectiveSharedConversions.find((row) => row.fromUnitId === event.target.value && row.toUnitId === baseUnit?.id);
            if (conversion?.factor) setQuantity(conversion.factor);
            else if (event.target.value === baseUnit?.id) setQuantity("1");
          }} disabled={busy || lookups.isLoading} required>
            {lookups.data?.units.filter((unit) => unit.status === "active" && (unit.id === baseUnit?.id || effectiveConversions.some((conversion) => conversion.fromUnitId === unit.id) || effectiveSharedConversions.some((conversion) => conversion.fromUnitId === unit.id && conversion.toUnitId === baseUnit?.id))).map((unit) => <option key={unit.id} value={unit.id}>{unit.code} · {locale === "en" ? unit.name?.english ?? "-" : unit.name?.thai ?? "-"}</option>)}
          </select>
        </div>
        <Input label={t("quantityInBaseUnit")} type="number" min="0.0001" step="0.0001" value={quantity} onChange={(event) => setQuantity(event.target.value)} required disabled={busy} />
        <div className="erp-form-group">
          <label className="erp-label" htmlFor="barcode-packaging">{t("packagingLevel")}</label>
          <select id="barcode-packaging" className="erp-input" value={packagingLevel} onChange={(event) => setPackagingLevel(event.target.value)} disabled={busy}>
            <option value="each">{t("packagingEach")}</option><option value="inner">{t("packagingInner")}</option><option value="case">{t("packagingCase")}</option><option value="pallet">{t("packagingPallet")}</option>
          </select>
        </div>
        <label className="flex min-h-[44px] items-center gap-2 text-sm text-erp-ink sm:col-span-2 lg:col-span-3">
          <input type="checkbox" checked={isPrimary} onChange={(event) => setIsPrimary(event.target.checked)} disabled={busy} className="accent-erp-navy" />
          {t("primaryBarcode")}
        </label>
        <div className="flex items-end justify-end lg:col-span-3"><Button type="submit" disabled={busy || !unitId} isLoading={mutations.create.isPending}>{t("addBarcode")}</Button></div>
      </form>}

      {query.isLoading ? (
        <p className="py-4 text-sm text-erp-muted">{t("loading")}</p>
      ) : (query.data?.length ?? 0) === 0 ? (
        <p className="py-4 text-sm text-erp-muted">{t("noBarcodes")}</p>
      ) : (
        <Table wrapperClassName="min-w-[760px]">
          <TableHeader>
            <TableRow>
              <TableHead>{t("barcodeValue")}</TableHead>
              <TableHead>{t("identifierType")}</TableHead>
              <TableHead>{t("packagingLevel")}</TableHead>
              <TableHead>{t("barcodeQuantity")}</TableHead>
              <TableHead>{t("status")}</TableHead>
              <TableHead>{t("rowActions")}</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {query.data?.map((barcode) => (
              <TableRow key={barcode.id}>
                <TableCell className="font-mono">
                  {barcode.value ?? "-"}
                  {barcode.isPrimary && (
                    <span className="ml-2 border border-erp-border px-1.5 py-0.5 font-sans text-xs">
                      {t("primaryBarcode")}
                    </span>
                  )}
                </TableCell>
                <TableCell>{barcode.identifierType === "gtin" ? "GTIN" : t("internalBarcode")}</TableCell>
                <TableCell>
                  {barcode.packagingLevel === "each"
                    ? t("packagingEach")
                    : barcode.packagingLevel === "inner"
                    ? t("packagingInner")
                    : barcode.packagingLevel === "case"
                    ? t("packagingCase")
                    : barcode.packagingLevel === "pallet"
                    ? t("packagingPallet")
                    : "-"}
                </TableCell>
                <TableCell>
                  {barcode.quantityInBaseUnit ?? "-"} {barcode.unit?.code ?? "-"}
                </TableCell>
                <TableCell>{barcode.status === "active" ? t("statusActive") : t("statusInactive")}</TableCell>
                <TableCell>
                  <div className="flex flex-wrap gap-2">
                    {canManage && barcode.status === "active" && !barcode.isPrimary && barcode.id && barcode.rowVersion && (
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        disabled={busy}
                        onClick={() => {
                          mutations.setPrimary.mutate({ barcodeId: barcode.id!, rowVersion: barcode.rowVersion! });
                        }}
                      >
                        {t("makePrimary")}
                      </Button>
                    )}
                    {canManage && barcode.status === "active" && barcode.id && barcode.rowVersion && (
                      <Button
                        type="button"
                        variant="danger"
                        size="sm"
                        disabled={busy}
                        onClick={() =>
                          setDeactivateTarget({
                            id: barcode.id!,
                            rowVersion: barcode.rowVersion!,
                            value: barcode.value ?? "",
                          })
                        }
                      >
                        {t("deactivate")}
                      </Button>
                    )}
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
      <ConfirmationModal isOpen={Boolean(deactivateTarget)} onClose={() => { if (!mutations.deactivate.isPending) setDeactivateTarget(null); }} onConfirm={() => { void confirmDeactivate(); }} title={t("deactivateBarcodeTitle")} message={t("deactivateBarcodeMessage", { value: deactivateTarget?.value ?? "" })} confirmText={t("deactivate")} cancelText={common("actions.cancel")} isLoading={mutations.deactivate.isPending} />
    </section>
  );
}
