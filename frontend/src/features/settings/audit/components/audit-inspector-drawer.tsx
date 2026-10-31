"use client";

import React, { useMemo } from "react";
import { useLocale, useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { StatusBadge, type BadgeVariant } from "@/components/ui/StatusBadge";
import { Button } from "@/components/ui/Button";
import { formatDateTime } from "@/lib/formatters/formatters";
import type { AuditEventResponse } from "@/lib/api/api-client";
import { IconAlertCircle } from "@/components/common/Icons";

export interface AuditInspectorDrawerProps {
  event: AuditEventResponse | null;
  isOpen: boolean;
  onClose: () => void;
}

export function resolveActionBadgeVariant(action?: string | null): BadgeVariant {
  if (!action) return "neutral";
  const lower = action.toLowerCase();
  if (lower.includes("create") || lower.includes("insert") || lower.includes("register") || lower.includes("linked") || lower.includes("posted")) return "success";
  if (lower.includes("approve") || lower.includes("accept") || lower.includes("confirm") || lower.includes("activate") || lower.includes("assigned")) return "success";
  if (lower.includes("delete") || lower.includes("void") || lower.includes("reject") || lower.includes("cancel") || lower.includes("deactivate") || lower.includes("unlinked") || lower.includes("reversed") || lower.includes("revoked")) return "danger";
  if (lower.includes("update") || lower.includes("edit") || lower.includes("modify") || lower.includes("change") || lower.includes("renamed") || lower.includes("upserted")) return "info";
  if (lower.includes("draft") || lower.includes("pending") || lower.includes("submit") || lower.includes("review") || lower.includes("requested")) return "warning";
  return "neutral";
}

export function AuditInspectorDrawer({ event, isOpen, onClose }: AuditInspectorDrawerProps) {
  const t = useTranslations("auditLog.inspector");
  const locale = useLocale();

  const formattedChanges = useMemo(() => {
    if (!event?.changesJson || event.changesJson.trim() === "" || event.changesJson.trim() === "{}") {
      return null;
    }
    try {
      const parsed = JSON.parse(event.changesJson);
      return JSON.stringify(parsed, null, 2);
    } catch {
      return event.changesJson;
    }
  }, [event?.changesJson]);

  const hasRedaction = useMemo(() => {
    if (!event) return false;
    const json = event.changesJson || "";
    const reason = event.reason || "";
    const actorName = event.actor?.name || "";
    return json.includes("[REDACTED") || reason.includes("[REDACTED") || actorName.includes("[REDACTED");
  }, [event]);

  if (!event) return null;

  return (
    <Drawer
      isOpen={isOpen}
      onClose={onClose}
      title={t("title")}
      description={t("subtitle")}
      size="lg"
      footer={
        <div className="flex justify-end">
          <Button variant="secondary" size="md" onClick={onClose}>
            {t("close")}
          </Button>
        </div>
      }
    >
      <div className="space-y-6">
        {/* Redacted Security Notice Banner */}
        {hasRedaction && (
          <div className="flex items-start gap-3 border border-amber-500/30 bg-amber-500/10 p-3.5 text-xs text-amber-900 dark:text-amber-200">
            <IconAlertCircle size={18} className="shrink-0 text-amber-600 dark:text-amber-400 mt-0.5" />
            <span className="leading-relaxed">{t("redactedNotice")}</span>
          </div>
        )}

        {/* Section 1: Event Info Card */}
        <div className="border border-erp-border bg-erp-surface">
          <div className="border-b border-erp-border bg-erp-surface-muted px-4 py-2.5">
            <h3 className="text-xs font-bold uppercase tracking-wider text-erp-navy">
              {t("eventInfo")}
            </h3>
          </div>
          <div className="p-4 grid grid-cols-1 sm:grid-cols-2 gap-4 text-xs">
            <div>
              <span className="text-erp-text-muted block mb-1 font-medium">{t("occurredAt")}</span>
              <span className="font-mono font-medium text-erp-text-main">
                {formatDateTime(event.occurredAtUtc, locale)}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block mb-1 font-medium">{t("action")}</span>
              <StatusBadge
                label={event.action || "-"}
                variant={resolveActionBadgeVariant(event.action)}
              />
            </div>
            <div>
              <span className="text-erp-text-muted block mb-1 font-medium">{t("resourceType")}</span>
              <span className="inline-block bg-erp-surface-muted px-2 py-0.5 font-mono font-medium text-erp-navy border border-erp-border">
                {event.resourceType || "-"}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block mb-1 font-medium">{t("resourceId")}</span>
              <span className="font-mono text-erp-text-main break-all font-semibold">
                {event.resourceId || "-"}
              </span>
            </div>
            {event.reason && (
              <div className="sm:col-span-2 border-t border-erp-border-subtle pt-3">
                <span className="text-erp-text-muted block mb-1 font-medium">{t("reason")}</span>
                <p className="text-erp-text-main bg-erp-surface-muted p-2.5 border border-erp-border leading-relaxed">
                  {event.reason}
                </p>
              </div>
            )}
          </div>
        </div>

        {/* Section 2: Actor & Context Card */}
        <div className="border border-erp-border bg-erp-surface">
          <div className="border-b border-erp-border bg-erp-surface-muted px-4 py-2.5">
            <h3 className="text-xs font-bold uppercase tracking-wider text-erp-navy">
              {t("actorInfo")}
            </h3>
          </div>
          <div className="p-4 grid grid-cols-1 sm:grid-cols-2 gap-4 text-xs">
            <div>
              <span className="text-erp-text-muted block mb-1 font-medium">{t("actor")}</span>
              <span className="font-semibold text-erp-text-main">
                {event.actor?.name || "-"}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block mb-1 font-medium">{t("actorId")}</span>
              <span className="font-mono text-erp-text-muted break-all">
                {event.actor?.id || "-"}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block mb-1 font-medium">{t("branchId")}</span>
              <span className="font-mono text-erp-text-muted break-all">
                {event.branchId || "-"}
              </span>
            </div>
            <div>
              <span className="text-erp-text-muted block mb-1 font-medium">{t("traceId")}</span>
              <span className="font-mono text-erp-text-muted break-all">
                {event.traceId || "-"}
              </span>
            </div>
            {event.requestId && (
              <div className="sm:col-span-2">
                <span className="text-erp-text-muted block mb-1 font-medium">{t("requestId")}</span>
                <span className="font-mono text-erp-text-muted break-all">
                  {event.requestId}
                </span>
              </div>
            )}
          </div>
        </div>

        {/* Section 3: Changes Payload (Forensic JSON Inspection) */}
        <div className="border border-erp-border bg-erp-surface">
          <div className="border-b border-erp-border bg-erp-surface-muted px-4 py-2.5 flex items-center justify-between">
            <h3 className="text-xs font-bold uppercase tracking-wider text-erp-navy">
              {t("changesPayload")}
            </h3>
            {formattedChanges && (
              <span className="text-[11px] font-mono text-erp-text-muted">JSON</span>
            )}
          </div>
          <div className="p-4">
            {formattedChanges ? (
              <pre className="max-h-80 overflow-auto bg-slate-950 p-3.5 text-xs font-mono text-emerald-400 border border-slate-800 leading-relaxed select-text">
                {formattedChanges}
              </pre>
            ) : (
              <p className="text-xs text-erp-text-muted italic py-2 text-center">
                {t("noChanges")}
              </p>
            )}
          </div>
        </div>
      </div>
    </Drawer>
  );
}
