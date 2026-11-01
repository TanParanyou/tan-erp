"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { IconGlobe, IconLogOut } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import { cn } from "@/lib/utils/cn";
import {
  useMyActiveSessions,
  useRevokeMySession,
  useRevokeOtherSessions,
} from "@/features/profile/api/security-queries";
import { formatDateTime } from "@/lib/formatters/formatters";
import type { ActiveSessionItemDto } from "@/lib/api/api-client";

export interface ActiveSessionsCardProps {
  sessions?: ActiveSessionItemDto[];
  className?: string;
}

export function ActiveSessionsCard({
  sessions: initialSessions,
  className,
}: ActiveSessionsCardProps) {
  const t = useTranslations("security");
  const { toast } = useToast();

  const { data: serverSessions, isLoading } = useMyActiveSessions();
  const revokeMySessionMutation = useRevokeMySession();
  const revokeOtherSessionsMutation = useRevokeOtherSessions();

  const sessions = serverSessions?.items ?? initialSessions ?? [];
  const otherSessionsCount = sessions.filter((s) => !s.isCurrent).length;

  const [sessionToRevoke, setSessionToRevoke] = useState<ActiveSessionItemDto | null>(null);
  const [showRevokeOthersModal, setShowRevokeOthersModal] = useState(false);

  const handleRevokeSingle = async () => {
    if (!sessionToRevoke?.id) return;
    try {
      await revokeMySessionMutation.mutateAsync(sessionToRevoke.id);
      toast.success(t("toastSessionRevoked"));
      setSessionToRevoke(null);
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : t("toastSessionRevoked"));
    }
  };

  const handleRevokeOthers = async () => {
    try {
      await revokeOtherSessionsMutation.mutateAsync();
      toast.success(t("toastAllOtherSessionsRevoked"));
      setShowRevokeOthersModal(false);
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : t("toastSessionRevoked"));
    }
  };

  return (
    <div className={cn("border border-erp-border bg-erp-surface p-5 rounded-none shadow-2xs text-left", className)}>
      <div className="flex flex-wrap items-center justify-between gap-3 border-b border-erp-border pb-3">
        <div>
          <h3 className="text-sm font-bold text-erp-text-main">
            {t("activeSessionsTitle")}
          </h3>
          <p className="text-xs text-erp-text-muted mt-0.5">
            {t("activeSessionsDescription")}
          </p>
        </div>

        {otherSessionsCount > 0 && (
          <Button
            variant="outline"
            size="sm"
            onClick={() => setShowRevokeOthersModal(true)}
            icon={<IconLogOut size={14} />}
          >
            {t("revokeAllOtherSessions")}
          </Button>
        )}
      </div>

      <div className="pt-2">
        {isLoading && sessions.length === 0 ? (
          <div className="py-6 flex justify-center">
            <MonoSpinner size="md" />
          </div>
        ) : sessions.length === 0 ? (
          <p className="text-xs text-erp-text-muted py-4 text-center">
            {t("noOtherSessions")}
          </p>
        ) : (
          <div className="divide-y divide-erp-border">
            {sessions.map((sess) => (
              <div key={sess.id ?? sess.ipAddress} className="flex flex-wrap items-center justify-between gap-3 py-3">
                <div className="flex items-center gap-3">
                  <div className="flex h-9 w-9 shrink-0 items-center justify-center border border-erp-border bg-erp-surface-subtle text-erp-text-muted">
                    <IconGlobe size={18} />
                  </div>
                  <div>
                    <div className="flex flex-wrap items-center gap-2">
                      <span className="text-xs font-bold text-erp-text-main">
                        {sess.deviceLabel || sess.userAgent || t("currentDevice")}
                      </span>
                      {sess.isCurrent && (
                        <span className="bg-erp-navy px-1.5 py-0.5 text-[10px] font-bold text-white tracking-wide">
                          {t("thisDevice")}
                        </span>
                      )}
                    </div>

                    <div className="flex flex-wrap items-center gap-x-3 gap-y-0.5 text-[11px] text-erp-text-muted font-mono mt-0.5">
                      <span>IP: {sess.ipAddress || "127.0.0.1"}</span>
                      {sess.clientPlatform && <span>• {sess.clientPlatform}</span>}
                      {sess.lastUsedAtUtc && (
                        <span>• {t("lastActive")}: {formatDateTime(sess.lastUsedAtUtc)}</span>
                      )}
                    </div>
                  </div>
                </div>

                {!sess.isCurrent && sess.id && (
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setSessionToRevoke(sess)}
                    className="text-xs text-erp-danger hover:border-erp-danger"
                  >
                    {t("revokeSession")}
                  </Button>
                )}
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Confirmation Modal: Revoke single session */}
      {sessionToRevoke && (
        <ConfirmationModal
          isOpen={sessionToRevoke !== null}
          onClose={() => setSessionToRevoke(null)}
          onConfirm={handleRevokeSingle}
          title={t("revokeSessionConfirmTitle")}
          message={t("revokeSessionConfirmDesc")}
          variant="danger"
          confirmText={t("revokeSession")}
          isLoading={revokeMySessionMutation.isPending}
        />
      )}

      {/* Confirmation Modal: Revoke all other sessions */}
      {showRevokeOthersModal && (
        <ConfirmationModal
          isOpen={showRevokeOthersModal}
          onClose={() => setShowRevokeOthersModal(false)}
          onConfirm={handleRevokeOthers}
          title={t("revokeOtherSessionsConfirmTitle")}
          message={t("revokeOtherSessionsConfirmDesc")}
          variant="danger"
          confirmText={t("revokeAllOtherSessions")}
          isLoading={revokeOtherSessionsMutation.isPending}
        />
      )}
    </div>
  );
}

ActiveSessionsCard.displayName = "ActiveSessionsCard";
