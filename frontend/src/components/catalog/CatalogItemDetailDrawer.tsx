"use client";

import React, { useMemo } from "react";
import { useTranslations, useLocale } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Badge, type BadgeVariant } from "@/components/ui/Badge";
import { CopyButton } from "@/components/common/CopyButton";
import { ImageGallery } from "@/components/common/ImageGallery";
import { useLocalizedText } from "@/hooks/useLocalizedText";
import { isSupportedLocale, defaultLocale } from "@/lib/i18n/locales";
import { IconMaximize, IconMinimize } from "@/components/common/Icons";
import { formatFinancialNumber } from "@/features/estimates/utils/estimate-formatters";
import type { CatalogItem } from "@/features/estimates/constants/estimate-catalog-items";
import { useAuthenticatedFileUrl } from "@/hooks/useAuthenticatedFileUrl";

export interface CatalogItemDetailDrawerProps {
  item: CatalogItem | null;
  isOpen: boolean;
  onClose: () => void;
  currency?: string;
}

export function CatalogItemDetailDrawer({
  item,
  isOpen,
  onClose,
  currency = "THB",
}: CatalogItemDetailDrawerProps) {
  const t = useTranslations("estimates");
  const tCommon = useTranslations("common");
  const rawLocale = useLocale();
  const currentLocale = isSupportedLocale(rawLocale) ? rawLocale : defaultLocale;
  const getLocalized = useLocalizedText();
  const [isExpanded, setIsExpanded] = React.useState(false);

  const { objectUrl: authImageUrl } = useAuthenticatedFileUrl(item?.primaryImageFileId || "");

  React.useEffect(() => {
    setIsExpanded(false);
  }, [item?.id]);

  const itemData = useMemo(() => {
    if (!item) return null;

    const itemName = getLocalized(item.name) || item.name.th;
    const itemAltName = currentLocale === "th" ? item.name.en || "-" : item.name.th || "-";
    const brandName = getLocalized(item.brand.name) || item.brand.name.th;
    const categoryName = getLocalized(item.category.name) || item.category.name.th;
    const subCategoryName = item.subCategory
      ? getLocalized(item.subCategory.name) || item.subCategory.name.th
      : "-";
    const supplierName = item.supplier
      ? getLocalized(item.supplier.name) || item.supplier.name.th
      : "-";
    const itemDesc = item.description ? getLocalized(item.description) : null;

    // Resolve all images (single or multiple)
    const images: string[] =
      item.images && item.images.length > 0
        ? item.images
        : authImageUrl
        ? [authImageUrl]
        : item.imageUrl
        ? [item.imageUrl]
        : [];

    // Formulate all specs text for bulk copy
    const specsParts: string[] = [];
    if (item.specs) {
      Object.entries(item.specs).forEach(([k, v]) => specsParts.push(`${k}: ${v}`));
    }
    if (item.attributes) {
      Object.entries(item.attributes).forEach(([k, v]) => specsParts.push(`${k}: ${v}`));
    }
    const allSpecsText = specsParts.join("\n");

    const statusVariant: BadgeVariant =
      item.status === "active"
        ? "success"
        : item.status === "phase_out"
        ? "warning"
        : "neutral";

    return {
      itemName,
      itemAltName,
      brandName,
      categoryName,
      subCategoryName,
      supplierName,
      itemDesc,
      images,
      allSpecsText,
      statusVariant,
    };
  }, [item, currentLocale, getLocalized, authImageUrl]);

  if (!item || !itemData) return null;

  return (
    <Drawer
      isOpen={isOpen}
      onClose={onClose}
      title={t("catalogDetail.drawerTitle")}
      description={t("catalogDetail.drawerDesc")}
      size={isExpanded ? "full" : "lg"}
      overlayClassName="!z-[1200]"
      headerActions={
        <button
          type="button"
          onClick={() => setIsExpanded((prev) => !prev)}
          className="p-1.5 text-erp-text-muted hover:text-erp-text-main hover:bg-erp-surface-muted rounded-none transition-colors focus-visible:outline-2 focus-visible:outline-erp-navy cursor-pointer"
          title={isExpanded ? t("catalogDetail.collapse") : t("catalogDetail.expand")}
          aria-label={isExpanded ? t("catalogDetail.collapse") : t("catalogDetail.expand")}
        >
          {isExpanded ? <IconMinimize size={18} /> : <IconMaximize size={18} />}
        </button>
      }
    >
      <div className="space-y-5 text-xs">
        {/* 1. Multi-Image Preview Gallery with Global Lightbox */}
        <ImageGallery
          images={itemData.images}
          alt={itemData.itemName}
          title={`${item.code} - ${itemData.itemName}`}
          aspectRatio="video"
          enableLightbox={true}
        />

        {/* 2. Header & Code with Central CopyButtons */}
        <div className="border-b border-erp-border pb-3">
          <div className="flex items-center gap-2 mb-1">
            <span className="font-mono font-bold text-erp-navy text-sm">
              {item.code}
            </span>
            <CopyButton
              text={item.code}
              variant="icon"
              size="sm"
              className="h-6 w-6 border-0 bg-transparent p-0 text-erp-text-muted hover:text-erp-navy"
              label={t("catalogDetail.copyCode")}
              copiedLabel={t("catalogDetail.copied")}
            />
            <Badge variant={itemData.statusVariant} className="text-[10px]">
              {tCommon(`status.${item.status}`)}
            </Badge>
            <span className="ml-auto px-1.5 py-0.5 text-[10px] font-mono border border-erp-border uppercase text-erp-text-secondary">
              {t(`costTypes.${item.itemType}`)}
            </span>
          </div>

          <div className="flex items-start justify-between gap-2">
            <div>
              <h3 className="text-sm font-bold text-erp-text-main">
                {itemData.itemName}
              </h3>
              <p className="text-xs font-mono text-erp-text-muted mt-0.5">
                {itemData.itemAltName}
              </p>
            </div>
            <CopyButton
              text={`${itemData.itemName} (${itemData.itemAltName})`}
              variant="icon"
              size="sm"
              className="h-6 w-6 border-0 bg-transparent p-0 text-erp-text-muted hover:text-erp-navy shrink-0"
              label={t("catalogDetail.copyName")}
              copiedLabel={t("catalogDetail.copied")}
            />
          </div>

          {/* Aliases */}
          {item.aliases && item.aliases.length > 0 && (
            <div className="flex items-center gap-1.5 mt-2 flex-wrap text-[11px]">
              <span className="text-erp-text-muted">{t("catalogDetail.aliases")}</span>
              {item.aliases.map((al, idx) => (
                <span
                  key={`${getLocalized(al)}-${idx}`}
                  className="px-1.5 py-0.5 bg-erp-surface-subtle border border-erp-border text-erp-text-main italic"
                >
                  &quot;{getLocalized(al)}&quot;
                </span>
              ))}
            </div>
          )}

          {/* Description */}
          {itemData.itemDesc && (
            <div className="mt-2.5 bg-erp-surface-subtle p-2 border border-erp-border/60 flex items-start justify-between gap-2">
              <p className="text-xs text-erp-text-secondary leading-relaxed">
                {itemData.itemDesc}
              </p>
              <CopyButton
                text={itemData.itemDesc}
                variant="icon"
                size="sm"
                className="h-6 w-6 border-0 bg-transparent p-0 text-erp-text-muted hover:text-erp-navy shrink-0"
                label={t("catalogDetail.copyDesc")}
                copiedLabel={t("catalogDetail.copied")}
              />
            </div>
          )}
        </div>

        {/* 3. General Information Grid */}
        <div className="space-y-2">
          <span className="font-bold text-erp-navy text-[11px] uppercase tracking-wider block">
            {t("catalogDetail.generalInfo")}
          </span>
          <div className="grid grid-cols-2 gap-2 bg-erp-surface border border-erp-border p-2.5">
            <div>
              <span className="text-erp-text-muted block text-[11px]">
                {t("catalogDetail.category")}
              </span>
              <span className="font-medium text-erp-text-main">
                {itemData.categoryName}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block text-[11px]">
                {t("catalogDetail.subCategory")}
              </span>
              <span className="font-medium text-erp-text-main">
                {itemData.subCategoryName}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block text-[11px]">
                {t("catalogDetail.brand")}
              </span>
              <span className="font-medium text-erp-text-main">
                {itemData.brandName}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block text-[11px]">
                {t("catalogDetail.unitCode")}
              </span>
              <span className="font-mono font-medium text-erp-text-main">
                {item.pricing.baseUnitCode}
              </span>
            </div>
          </div>
        </div>

        {/* 4. Specifications & Dynamic Attributes */}
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <span className="font-bold text-erp-navy text-[11px] uppercase tracking-wider block">
              {t("catalogDetail.specsAndAttrs")}
            </span>
            {itemData.allSpecsText && (
              <CopyButton
                text={itemData.allSpecsText}
                variant="inline"
                label={t("catalogDetail.copySpecs")}
                copiedLabel={t("catalogDetail.copied")}
                className="text-[11px] py-0.5 px-2 bg-transparent border-0 text-erp-navy hover:underline"
              />
            )}
          </div>
          <div className="border border-erp-border divide-y divide-erp-border/60 bg-erp-surface">
            {/* Standard Specs */}
            {item.specs &&
              Object.entries(item.specs).map(([k, v]) => (
                <div
                  key={`spec-${k}`}
                  className="flex justify-between items-center px-3 py-1.5 group"
                >
                  <span className="text-erp-text-muted capitalize">{k}</span>
                  <div className="flex items-center gap-1.5">
                    <span className="font-mono font-medium text-erp-text-main">{v}</span>
                    <CopyButton
                      text={v}
                      variant="icon"
                      size="sm"
                      className="opacity-0 group-hover:opacity-100 transition-opacity h-5 w-5 border-0 bg-transparent p-0 text-erp-text-muted hover:text-erp-navy"
                      label={t("catalogDetail.copy")}
                      copiedLabel={t("catalogDetail.copied")}
                    />
                  </div>
                </div>
              ))}

            {/* Dynamic Attributes */}
            {item.attributes &&
              Object.entries(item.attributes).map(([k, v]) => (
                <div
                  key={`attr-${k}`}
                  className="flex justify-between items-center px-3 py-1.5 bg-erp-surface-subtle/30 group"
                >
                  <span className="text-erp-text-secondary font-medium uppercase text-[10px]">
                    {k}
                  </span>
                  <div className="flex items-center gap-1.5">
                    <span className="font-mono font-bold text-erp-navy">{v}</span>
                    <CopyButton
                      text={v}
                      variant="icon"
                      size="sm"
                      className="opacity-0 group-hover:opacity-100 transition-opacity h-5 w-5 border-0 bg-transparent p-0 text-erp-text-muted hover:text-erp-navy"
                      label={t("catalogDetail.copy")}
                      copiedLabel={t("catalogDetail.copied")}
                    />
                  </div>
                </div>
              ))}

            {!item.specs && !item.attributes && (
              <div className="p-3 text-center text-erp-text-muted italic">-</div>
            )}
          </div>
        </div>

        {/* 5. Procurement & Pricing */}
        <div className="space-y-2">
          <span className="font-bold text-erp-navy text-[11px] uppercase tracking-wider block">
            {t("catalogDetail.procurementAndPricing")}
          </span>
          <div className="grid grid-cols-2 gap-2 bg-erp-surface border border-erp-border p-2.5">
            <div>
              <span className="text-erp-text-muted block text-[11px]">
                {t("catalogDetail.supplier")}
              </span>
              <span className="font-medium text-erp-text-main">
                {itemData.supplierName}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block text-[11px]">
                {t("catalogDetail.supplierCode")}
              </span>
              <div className="flex items-center gap-1.5">
                <span className="font-mono text-erp-text-main">
                  {item.supplier?.code || "-"}
                </span>
                {item.supplier?.code && (
                  <CopyButton
                    text={item.supplier.code}
                    variant="icon"
                    size="sm"
                    className="h-5 w-5 border-0 bg-transparent p-0 text-erp-text-muted hover:text-erp-navy"
                    label={t("catalogDetail.copy")}
                    copiedLabel={t("catalogDetail.copied")}
                  />
                )}
              </div>
            </div>
            <div className="col-span-2 pt-2 border-t border-erp-border/60 flex items-center justify-between">
              <div>
                <span className="text-erp-text-muted block text-[11px]">
                  {t("catalogDetail.unitCost")}
                </span>
                <span className="text-xs text-erp-text-secondary font-mono">
                  1 {item.pricing.baseUnitCode}
                </span>
              </div>
              <div className="flex items-center gap-2">
                <span className="font-mono font-bold text-sm text-erp-navy">
                  {formatFinancialNumber(item.pricing.defaultUnitCost)} {currency}
                </span>
                <CopyButton
                  text={`${item.pricing.defaultUnitCost}`}
                  variant="icon"
                  size="sm"
                  className="h-6 w-6 border-0 bg-transparent p-0 text-erp-text-muted hover:text-erp-navy"
                  label={t("catalogDetail.copyCost")}
                  copiedLabel={t("catalogDetail.copied")}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </Drawer>
  );
}

export default CatalogItemDetailDrawer;
