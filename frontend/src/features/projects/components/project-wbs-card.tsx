"use client";

import { useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { useToast } from "@/hooks/useToast";
import { formatCurrency } from "@/lib/formatters/formatters";
import type {
  ProjectControlResponse,
  ProjectWbsCostSummaryResponse,
  ProjectWbsNodeResponse,
} from "@/lib/api/api-client";
import type { useProjectControlMutations } from "../api/project-queries";

interface ProjectWbsCardProps {
  control: ProjectControlResponse;
  canEdit: boolean;
  mutations: ReturnType<typeof useProjectControlMutations>;
  describeError: (error: unknown) => string;
}

const EDITABLE_STATUSES = ["planned", "active", "on_hold"];

export function ProjectWbsCard({ control, canEdit, mutations, describeError }: ProjectWbsCardProps) {
  const t = useTranslations("projects.control");
  const locale = useLocale();
  const { toast } = useToast();

  const [parentId, setParentId] = useState("");
  const [code, setCode] = useState("");
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);

  const status = control.status ?? "";
  const editable = canEdit && EDITABLE_STATUSES.includes(status);
  const nodes: ProjectWbsNodeResponse[] = control.wbsNodes ?? [];
  const summaries: ProjectWbsCostSummaryResponse[] = control.wbsCostSummary ?? [];
  const busy = Boolean(mutations.addWbsNode?.isPending || mutations.deleteWbsNode?.isPending);

  const displayRows: ProjectWbsCostSummaryResponse[] = summaries.length > 0
    ? summaries
    : nodes.map((n) => ({
        wbsNodeId: n.id,
        wbsCode: n.code,
        wbsName: n.name,
        level: n.level,
        baselineBudget: 0,
        committedAmount: 0,
        actualAmount: 0,
        remainingBudget: 0,
        isOverrun: false,
      }));

  async function addNode(): Promise<void> {
    setError(null);
    if (!code.trim() || !name.trim()) {
      setError(t("wbsInvalid"));
      return;
    }

    try {
      await mutations.addWbsNode.mutateAsync({
        parentId: parentId || null,
        code: code.trim(),
        name: name.trim(),
      });
      toast.success(t("wbsNodeAdded"));
      setCode("");
      setName("");
      setParentId("");
    } catch (err: unknown) {
      setError(describeError(err));
    }
  }

  async function deleteNode(node: ProjectWbsNodeResponse): Promise<void> {
    setError(null);
    try {
      await mutations.deleteWbsNode.mutateAsync({
        wbsNodeId: node.id ?? "",
        expectedVersion: node.rowVersion ?? "",
      });
      toast.success(t("wbsNodeRemoved"));
    } catch (err: unknown) {
      setError(describeError(err));
    }
  }

  return (
    <div className="erp-card p-5 space-y-4" role="region" aria-label={t("wbsTitle")}>
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h3 className="text-sm font-bold text-erp-navy">{t("wbsTitle")}</h3>
          <p className="mt-1 text-xs text-erp-text-muted">{t("wbsHint")}</p>
        </div>
      </div>

      {nodes.length === 0 && summaries.length === 0 ? (
        <p className="text-xs text-erp-text-muted">{t("wbsEmpty")}</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead className="border-b border-erp-border bg-erp-surface-subtle font-semibold text-erp-navy">
              <tr>
                <th className="p-2.5">{t("wbsNodeHeader")}</th>
                <th className="p-2.5 text-right">{t("baselineBudget")}</th>
                <th className="p-2.5 text-right">{t("committedAmount")}</th>
                <th className="p-2.5 text-right">{t("actualAmount")}</th>
                <th className="p-2.5 text-right">{t("remainingBudget")}</th>
                <th className="p-2.5 text-center">{t("statusHeader")}</th>
                {editable && (
                  <th className="p-2.5 text-right">
                    <span className="sr-only">{t("actionsHeader")}</span>
                  </th>
                )}
              </tr>
            </thead>
            <tbody className="divide-y divide-erp-border">
              {displayRows.map((row) => {
                const matchingNode = nodes.find((n) => n.id === row.wbsNodeId);
                const level = row.level ?? 1;
                const indentClass = level === 3 ? "pl-8" : level === 2 ? "pl-5" : "pl-2.5";
                return (
                  <tr key={row.wbsNodeId ?? row.wbsCode ?? "unassigned"} className="hover:bg-erp-surface-subtle/50">
                    <td className={`p-2.5 ${indentClass}`}>
                      <div className="flex items-center gap-2">
                        <span className="font-mono text-[10px] px-1 py-0.5 border border-erp-border text-erp-text-muted">
                          L{level}
                        </span>
                        <span className="font-mono font-bold text-erp-navy">{row.wbsCode}</span>
                        <span className="text-erp-text-main">{row.wbsName}</span>
                      </div>
                    </td>
                    <td className="p-2.5 text-right font-mono">{formatCurrency(row.baselineBudget ?? 0, "THB", locale)}</td>
                    <td className="p-2.5 text-right font-mono">{formatCurrency(row.committedAmount ?? 0, "THB", locale)}</td>
                    <td className="p-2.5 text-right font-mono">{formatCurrency(row.actualAmount ?? 0, "THB", locale)}</td>
                    <td className="p-2.5 text-right font-mono">{formatCurrency(row.remainingBudget ?? 0, "THB", locale)}</td>
                    <td className="p-2.5 text-center">
                      <StatusBadge
                        label={row.isOverrun ? t("wbsOverrun") : t("wbsWithinBudget")}
                        variant={row.isOverrun ? "danger" : "success"}
                      />
                    </td>
                    {editable && (
                      <td className="p-2.5 text-right">
                        {matchingNode && (
                          <Button
                            type="button"
                            variant="outline"
                            size="sm"
                            className="min-h-8 text-xs"
                            disabled={busy}
                            onClick={() => void deleteNode(matchingNode)}
                          >
                            {t("removeLine")}
                          </Button>
                        )}
                      </td>
                    )}
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}

      {editable && (
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-[200px_140px_1fr_auto] sm:items-end">
          <Select
            label={t("wbsParent")}
            value={parentId}
            disabled={busy}
            options={[
              { value: "", label: t("wbsRootLevel") },
              ...nodes
                .filter((n) => (n.level ?? 1) < 3)
                .map((n) => ({ value: n.id ?? "", label: `${n.code} — ${n.name}` })),
            ]}
            onChange={(e) => setParentId(e.target.value)}
          />
          <Input
            label={t("wbsCode")}
            placeholder={t("wbsCodePlaceholder")}
            value={code}
            maxLength={50}
            disabled={busy}
            onChange={(e) => setCode(e.target.value)}
          />
          <Input
            label={t("wbsName")}
            placeholder={t("wbsNamePlaceholder")}
            value={name}
            maxLength={200}
            disabled={busy}
            onChange={(e) => setName(e.target.value)}
          />
          <Button
            type="button"
            variant="primary"
            className="min-h-11"
            isLoading={mutations.addWbsNode.isPending}
            disabled={busy}
            onClick={() => void addNode()}
          >
            {t("addWbsNode")}
          </Button>
        </div>
      )}
      {error && <p className="text-xs text-erp-danger" role="alert">{error}</p>}
    </div>
  );
}
