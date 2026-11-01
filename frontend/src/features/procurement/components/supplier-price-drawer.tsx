"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Checkbox } from "@/components/ui/Checkbox";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Drawer } from "@/components/ui/Drawer";
import { EmptyState } from "@/components/ui/EmptyState";
import { Input } from "@/components/ui/Input";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/Table";
import { ItemAutocomplete } from "@/features/item-master/components/item-autocomplete";
import { useToast } from "@/hooks/useToast";
import { ApiError } from "@/lib/api/api-error";
import type { SupplierItemPriceRequest, SupplierItemPriceResponse, SupplierResponse } from "@/lib/api/api-client";
import { useSupplierItemPriceMutations, useSupplierItemPrices } from "../api/procurement-queries";
import { procurementErrorCode } from "../procurement-status";

interface PriceForm {
  itemId: string;
  unitPrice: string;
  minOrderQuantity: string;
  leadTimeDays: string;
  effectiveFrom: string;
  effectiveTo: string;
  isPreferred: boolean;
}

const EMPTY_FORM: PriceForm = { itemId: "", unitPrice: "", minOrderQuantity: "0", leadTimeDays: "0", effectiveFrom: "", effectiveTo: "", isPreferred: false };

function toForm(price: SupplierItemPriceResponse): PriceForm {
  return {
    itemId: price.itemId ?? "",
    unitPrice: String(price.unitPrice ?? ""),
    minOrderQuantity: String(price.minOrderQuantity ?? 0),
    leadTimeDays: String(price.leadTimeDays ?? 0),
    effectiveFrom: price.effectiveFrom ?? "",
    effectiveTo: price.effectiveTo ?? "",
    isPreferred: price.isPreferred ?? false,
  };
}

interface SupplierPriceDrawerProps {
  supplier: SupplierResponse | null;
  canManage: boolean;
  onClose: () => void;
}

export function SupplierPriceDrawer({ supplier, canManage, onClose }: SupplierPriceDrawerProps) {
  const t = useTranslations("procurement.suppliers");
  const tErrors = useTranslations("procurement.errors");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const supplierId = supplier?.id ?? "";
  const prices = useSupplierItemPrices(supplier?.id, { page: 1, pageSize: 100 });
  const { create, update, remove } = useSupplierItemPriceMutations(supplierId);

  const [editing, setEditing] = useState<SupplierItemPriceResponse | "new" | null>(null);
  const [form, setForm] = useState<PriceForm>(EMPTY_FORM);
  const [error, setError] = useState<string | null>(null);
  const [pickerKey, setPickerKey] = useState(0);
  const [deleting, setDeleting] = useState<SupplierItemPriceResponse | null>(null);

  const isBusy = create.isPending || update.isPending;
  const rows = prices.data?.items ?? [];
  const supplierInactive = supplier?.status !== "active";

  function describe(err: unknown): string {
    const code = err instanceof ApiError ? procurementErrorCode(err.code) : null;
    return code ? tErrors(code) : tErrors("failed");
  }

  function openNew(): void {
    setForm(EMPTY_FORM);
    setError(null);
    setPickerKey((k) => k + 1);
    setEditing("new");
  }

  function openEdit(price: SupplierItemPriceResponse): void {
    setForm(toForm(price));
    setError(null);
    setEditing(price);
  }

  async function save(): Promise<void> {
    const unitPrice = Number(form.unitPrice);
    const moq = Number(form.minOrderQuantity);
    const lead = Number(form.leadTimeDays);
    if (!form.itemId || form.unitPrice === "" || !(unitPrice >= 0) || !(moq >= 0) || !Number.isInteger(lead) || lead < 0 || lead > 365 || !form.effectiveFrom) {
      setError(t("priceInvalid"));
      return;
    }

    const payload: SupplierItemPriceRequest = {
      itemId: form.itemId,
      unitPrice,
      minOrderQuantity: moq,
      leadTimeDays: lead,
      effectiveFrom: form.effectiveFrom,
      effectiveTo: form.effectiveTo || null,
      isPreferred: form.isPreferred,
    };

    try {
      if (editing === "new") {
        await create.mutateAsync({ payload });
      } else if (editing) {
        await update.mutateAsync({ price: editing, payload });
      }
      toast.success(t("priceSaved"));
      setEditing(null);
    } catch (err: unknown) {
      setError(describe(err));
    }
  }

  async function confirmDelete(): Promise<void> {
    if (!deleting) return;
    try {
      await remove.mutateAsync({ price: deleting });
      toast.success(t("priceDeleted"));
    } catch (err: unknown) {
      toast.error(describe(err));
    } finally {
      setDeleting(null);
    }
  }

  const supplierName = supplier ? (locale === "en" ? supplier.nameEn ?? supplier.nameTh : supplier.nameTh) : null;

  return (
    <>
      <Drawer
        isOpen={supplier !== null}
        onClose={() => { if (!isBusy) { setEditing(null); onClose(); } }}
        title={`${t("pricesTitle")}${supplierName ? ` · ${supplierName}` : ""}`}
        size="xl"
        closeDisabled={isBusy}
        closeOnOverlayClick={!isBusy}
      >
        <div className="space-y-4 py-2">
          <p className="text-sm text-erp-text-muted">{t("pricesHint")}</p>
          {supplierInactive && <Alert variant="warning">{t("priceInactiveSupplier")}</Alert>}
          {prices.isError && <Alert variant="danger">{describe(prices.error)}</Alert>}
          {canManage && !supplierInactive && editing === null && (
            <Button type="button" variant="primary" className="min-h-11" onClick={openNew}>{t("priceAdd")}</Button>
          )}

          {editing !== null && (
            <div className="space-y-3 border border-erp-border p-4">
              {error && <Alert variant="danger" onClose={() => setError(null)}>{error}</Alert>}
              {editing === "new" ? (
                <ItemAutocomplete
                  key={pickerKey}
                  label={t("priceItem")}
                  purchasableOnly
                  required
                  disabled={isBusy}
                  onChange={(itemId) => setForm((current) => ({ ...current, itemId }))}
                />
              ) : (
                <p className="text-sm font-medium">{`${editing.itemCode ?? "-"} · ${editing.itemNameTh ?? "-"}`}</p>
              )}
              <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
                <Input type="number" min={0} step="0.0001" label={t("priceUnit")} placeholder={t("priceUnitPlaceholder")} value={form.unitPrice} disabled={isBusy}
                  onChange={(event) => setForm((current) => ({ ...current, unitPrice: event.target.value }))} />
                <Input type="number" min={0} step="0.0001" label={t("priceMoq")} placeholder={t("priceMoqPlaceholder")} value={form.minOrderQuantity} disabled={isBusy}
                  onChange={(event) => setForm((current) => ({ ...current, minOrderQuantity: event.target.value }))} />
                <Input type="number" min={0} max={365} label={t("priceLead")} placeholder={t("priceLeadPlaceholder")} value={form.leadTimeDays} disabled={isBusy}
                  onChange={(event) => setForm((current) => ({ ...current, leadTimeDays: event.target.value }))} />
                <Input type="date" label={t("priceFrom")} placeholder={t("priceFromPlaceholder")} value={form.effectiveFrom} disabled={isBusy}
                  onChange={(event) => setForm((current) => ({ ...current, effectiveFrom: event.target.value }))} />
                <Input type="date" label={t("priceTo")} placeholder={t("priceToPlaceholder")} value={form.effectiveTo} disabled={isBusy}
                  onChange={(event) => setForm((current) => ({ ...current, effectiveTo: event.target.value }))} />
              </div>
              <Checkbox label={t("pricePreferred")} checked={form.isPreferred} disabled={isBusy}
                onCheckedChange={(checked) => setForm((current) => ({ ...current, isPreferred: checked }))} />
              <div className="flex flex-col-reverse gap-2 sm:flex-row sm:justify-end">
                <Button type="button" variant="outline" className="min-h-11" disabled={isBusy} onClick={() => setEditing(null)}>{tCommon("actions.cancel")}</Button>
                <Button type="button" variant="primary" className="min-h-11" isLoading={isBusy} disabled={isBusy} onClick={() => void save()}>{t("priceSave")}</Button>
              </div>
            </div>
          )}

          {!prices.isLoading && rows.length === 0 ? (
            <EmptyState icon="empty" title={t("priceEmpty")} />
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{t("priceItem")}</TableHead>
                  <TableHead>{t("priceUnit")}</TableHead>
                  <TableHead>{t("priceMoq")}</TableHead>
                  <TableHead>{t("priceLead")}</TableHead>
                  <TableHead>{t("priceFrom")}</TableHead>
                  <TableHead>{t("priceTo")}</TableHead>
                  <TableHead>{t("pricePreferred")}</TableHead>
                  <TableHead><span className="sr-only">{t("actions")}</span></TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {rows.map((row) => (
                  <TableRow key={row.id}>
                    <TableCell className="align-middle">{`${row.itemCode ?? "-"} · ${row.itemNameTh ?? "-"}`}</TableCell>
                    <TableCell className="align-middle">{row.unitPrice?.toLocaleString(locale, { minimumFractionDigits: 2, maximumFractionDigits: 4 }) ?? "-"}</TableCell>
                    <TableCell className="align-middle">{row.minOrderQuantity ?? "-"}</TableCell>
                    <TableCell className="align-middle">{row.leadTimeDays ?? "-"}</TableCell>
                    <TableCell className="align-middle">{row.effectiveFrom ?? "-"}</TableCell>
                    <TableCell className="align-middle">{row.effectiveTo ?? t("priceNoEnd")}</TableCell>
                    <TableCell className="align-middle">{row.isPreferred ? t("priceYes") : "-"}</TableCell>
                    <TableCell className="align-middle">
                      {canManage && !supplierInactive && (
                        <div className="flex gap-2">
                          <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={isBusy} onClick={() => openEdit(row)}>{t("priceEdit")}</Button>
                          <Button type="button" variant="outline" size="sm" className="min-h-11" disabled={remove.isPending} onClick={() => setDeleting(row)}>{t("priceDelete")}</Button>
                        </div>
                      )}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </div>
      </Drawer>
      <ConfirmationModal
        isOpen={deleting !== null}
        onClose={() => setDeleting(null)}
        onConfirm={() => confirmDelete()}
        title={t("priceDeleteTitle")}
        message={t("priceDeleteMessage")}
        variant="danger"
        isLoading={remove.isPending}
      />
    </>
  );
}
