"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { MultiLangInput } from "@/components/forms/MultiLangInput";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can } from "@/lib/permissions/can";
import type { ItemResponse } from "@/lib/api/api-client";
import { useItemAliasMutations } from "@/features/item-master/api/item-master-queries";

export function ItemAliasMaintenance({ itemId, item }: { itemId: string; item: ItemResponse }) {
  const t = useTranslations("itemMaster");
  const common = useTranslations("common");
  const locale = useLocale();
  const { selectedMembership } = useSelectedMembership();
  const mutations = useItemAliasMutations(itemId);
  const [alias, setAlias] = useState({ th: "", en: "" });
  const [removeTarget, setRemoveTarget] = useState<{ id: string; rowVersion: string } | null>(null);
  const editable = can(selectedMembership, "items.update");
  const add = async () => {
    try {
      await mutations.add.mutateAsync({ alias: { thai: alias.th.trim(), english: alias.en.trim() || null }, rowVersion: item.rowVersion });
      setAlias({ th: "", en: "" });
    } catch {
      // Keep the entered value so the user can correct or retry.
    }
  };
  const remove = async () => {
    if (!removeTarget) return;
    try {
      await mutations.remove.mutateAsync({ aliasId: removeTarget.id, rowVersion: removeTarget.rowVersion });
      setRemoveTarget(null);
    } catch {
      // Keep the confirmation open when the server rejects the change.
    }
  };
  const busy = mutations.add.isPending || mutations.remove.isPending;
  return <section className="space-y-4 border border-erp-border bg-erp-surface p-4 sm:p-6">
    <h2 className="text-lg font-semibold text-erp-ink">{t("aliases")}</h2>
    {item.aliases?.length ? <ul className="divide-y divide-erp-border">{item.aliases.filter((entry) => entry.status === "active").map((entry) => <li key={entry.id} className="flex min-h-12 items-center justify-between gap-3 py-2"><span>{locale === "en" ? entry.alias?.english ?? entry.alias?.thai ?? "-" : entry.alias?.thai ?? "-"}</span>{editable && <Button type="button" variant="outline" disabled={busy} onClick={() => { if (entry.id && item.rowVersion) setRemoveTarget({ id: entry.id, rowVersion: item.rowVersion }); }}>{t("removeAlias")}</Button>}</li>)}</ul> : <p className="text-sm text-erp-muted">{t("noAliases")}</p>}
    {editable && <div className="space-y-3 border-t border-erp-border pt-4"><MultiLangInput label={t("alias")} value={alias} onChange={(value) => setAlias({ th: value.th ?? "", en: value.en ?? "" })} /><Button type="button" disabled={busy || !alias.th.trim()} isLoading={mutations.add.isPending} onClick={() => { void add(); }}>{t("addAlias")}</Button></div>}
    {(mutations.add.isError || mutations.remove.isError) && <p role="alert" className="text-sm text-erp-danger">{t("aliasSaveFailed")}</p>}
    <ConfirmationModal isOpen={removeTarget !== null} onClose={() => { if (!busy) setRemoveTarget(null); }} onConfirm={() => { void remove(); }} title={t("removeAliasTitle")} message={t("removeAliasMessage")} confirmText={t("removeAlias")} cancelText={common("actions.cancel")} isLoading={mutations.remove.isPending} />
  </section>;
}
