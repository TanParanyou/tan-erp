"use client";

import { useTranslations } from "next-intl";
import { Collapsible } from "@/components/ui/Collapsible";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/Table";
import { ITEM_IMPORT_COLUMNS, ITEM_IMPORT_REQUIRED_COLUMNS, type ItemImportKind } from "../item-import-constants";

interface ItemImportColumnGuideProps {
  kind: ItemImportKind;
}

/** Collapsible help: the file rules and, for item files, what each column means and whether it is required. */
export function ItemImportColumnGuide({ kind }: ItemImportColumnGuideProps) {
  const t = useTranslations("itemMaster.import");
  return (
    <div className="border border-erp-border px-3 py-2">
      <Collapsible title={t("guideTitle")} defaultOpen={false}>
        <div className="space-y-3 pt-2">
          <p className="text-xs text-erp-text-muted">{t("rules")}</p>
          {kind === "items" && (
            <>
              <p className="text-xs text-erp-text-muted">{t("guideNote")}</p>
              <Table wrapperClassName="max-h-64 overflow-auto border border-erp-border">
                <TableHeader className="sticky top-0 bg-erp-surface-subtle">
                  <TableRow>
                    <TableHead>{t("guideColumn")}</TableHead>
                    <TableHead>{t("guideRequired")}</TableHead>
                    <TableHead>{t("guideFormat")}</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {ITEM_IMPORT_COLUMNS.map((column) => (
                    <TableRow key={column} className="align-top">
                      <TableCell className="whitespace-nowrap font-mono text-xs">{column}</TableCell>
                      <TableCell className="text-xs">{ITEM_IMPORT_REQUIRED_COLUMNS.includes(column) ? t("guideYes") : t("guideNo")}</TableCell>
                      <TableCell className="text-xs">{t(`guide.${column}`)}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </>
          )}
        </div>
      </Collapsible>
    </div>
  );
}
