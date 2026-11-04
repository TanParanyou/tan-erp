"use client";

import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Alert } from "@/components/ui/Alert";
import type { DeactivateUnitConversionRequest, UpdateUnitConversionRequest } from "@/lib/api/api-client";

export interface ConversionItemTarget {
  id: string;
  fromUnitCode?: string | null;
  toUnitCode?: string | null;
  factor?: string | null;
  effectiveFrom?: string | null;
  effectiveTo?: string | null;
  reason?: string | null;
  status?: string | null;
  rowVersion: string;
}

export interface UnitConversionEditModalProps {
  isOpen: boolean;
  onClose: () => void;
  conversion: ConversionItemTarget | null;
  onSave: (id: string, payload: UpdateUnitConversionRequest, rowVersion: string) => Promise<void>;
  isLoading?: boolean;
}

export function UnitConversionEditModal({
  isOpen,
  onClose,
  conversion,
  onSave,
  isLoading = false,
}: UnitConversionEditModalProps) {
  const t = useTranslations("itemMaster");
  const tCommon = useTranslations("common");

  const [factor, setFactor] = useState("");
  const [effectiveFrom, setEffectiveFrom] = useState("");
  const [effectiveTo, setEffectiveTo] = useState("");
  const [reason, setReason] = useState("");
  const [errorAlert, setErrorAlert] = useState<string | null>(null);

  useEffect(() => {
    if (conversion) {
      setFactor(conversion.factor ?? "");
      setEffectiveFrom(conversion.effectiveFrom ?? "");
      setEffectiveTo(conversion.effectiveTo ?? "");
      setReason(conversion.reason ?? "");
      setErrorAlert(null);
    }
  }, [conversion]);

  if (!conversion) return null;

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (!conversion) return;
    if (!factor || Number(factor) <= 0 || !effectiveFrom || !reason.trim()) {
      return;
    }
    if (effectiveTo && effectiveTo < effectiveFrom) {
      setErrorAlert(t("conversionSaveFailed"));
      return;
    }

    try {
      setErrorAlert(null);
      await onSave(
        conversion.id,
        {
          factor,
          effectiveFrom,
          effectiveTo: effectiveTo || null,
          reason: reason.trim(),
        },
        conversion.rowVersion,
      );
      onClose();
    } catch {
      setErrorAlert(t("conversionUpdateFailed"));
    }
  }

  const fromCode = conversion.fromUnitCode ?? "-";
  const toCode = conversion.toUnitCode ?? "-";

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={t("editConversion")}
      size="md"
    >
      <form onSubmit={(event) => void handleSubmit(event)} className="space-y-4">
        <div className="border border-erp-border bg-erp-surface-subtle p-3">
          <p className="text-xs font-semibold uppercase tracking-wider text-erp-text-muted">
            {t("unitConversions")}
          </p>
          <p className="mt-1 font-mono text-sm font-bold text-erp-navy">
            {fromCode} → {toCode}
          </p>
        </div>

        {errorAlert && <Alert variant="danger">{errorAlert}</Alert>}

        <div>
          <Input
            label={t("conversionFactor")}
            type="number"
            min="0.000001"
            max="999999999999"
            step="0.000001"
            value={factor}
            onChange={(event) => setFactor(event.target.value)}
            required
            disabled={isLoading}
            placeholder={t("conversionFactorPlaceholder")}
          />
        </div>

        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          <Input
            label={t("effectiveFrom")}
            type="date"
            value={effectiveFrom}
            onChange={(event) => setEffectiveFrom(event.target.value)}
            required
            disabled={isLoading}
            placeholder={t("datePlaceholder")}
          />
          <Input
            label={t("effectiveTo")}
            type="date"
            value={effectiveTo}
            onChange={(event) => setEffectiveTo(event.target.value)}
            disabled={isLoading}
            placeholder={t("datePlaceholder")}
          />
        </div>

        <div>
          <Input
            label={t("reason")}
            value={reason}
            onChange={(event) => setReason(event.target.value)}
            required
            disabled={isLoading}
            maxLength={500}
            placeholder={t("reasonPlaceholder")}
          />
        </div>

        <div className="flex justify-end gap-2 border-t border-erp-border pt-3">
          <Button
            type="button"
            variant="outline"
            onClick={onClose}
            disabled={isLoading}
          >
            {tCommon("actions.cancel")}
          </Button>
          <Button
            type="submit"
            variant="primary"
            isLoading={isLoading}
            disabled={isLoading || !factor || Number(factor) <= 0 || !effectiveFrom || !reason.trim()}
          >
            {tCommon("actions.save")}
          </Button>
        </div>
      </form>
    </Modal>
  );
}

export interface UnitConversionDeactivateModalProps {
  isOpen: boolean;
  onClose: () => void;
  conversion: ConversionItemTarget | null;
  onConfirm: (id: string, payload: DeactivateUnitConversionRequest, rowVersion: string) => Promise<void>;
  isLoading?: boolean;
}

export function UnitConversionDeactivateModal({
  isOpen,
  onClose,
  conversion,
  onConfirm,
  isLoading = false,
}: UnitConversionDeactivateModalProps) {
  const t = useTranslations("itemMaster");
  const tCommon = useTranslations("common");

  const [reason, setReason] = useState("");
  const [errorAlert, setErrorAlert] = useState<string | null>(null);

  useEffect(() => {
    if (conversion) {
      setReason("");
      setErrorAlert(null);
    }
  }, [conversion]);

  if (!conversion) return null;

  async function handleConfirm(event: FormEvent) {
    event.preventDefault();
    if (!conversion || !reason.trim()) return;

    try {
      setErrorAlert(null);
      await onConfirm(
        conversion.id,
        {
          reason: reason.trim(),
        },
        conversion.rowVersion,
      );
      onClose();
    } catch {
      setErrorAlert(t("conversionDeactivateFailed"));
    }
  }

  const fromCode = conversion.fromUnitCode ?? "-";
  const toCode = conversion.toUnitCode ?? "-";

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={t("deactivateConversionTitle")}
      size="sm"
    >
      <form onSubmit={(event) => void handleConfirm(event)} className="space-y-4">
        <p className="text-sm text-erp-text-muted leading-relaxed">
          {t("deactivateConversionMessage", { from: fromCode, to: toCode })}
        </p>

        {errorAlert && <Alert variant="danger">{errorAlert}</Alert>}

        <div>
          <Input
            label={t("reason")}
            value={reason}
            onChange={(event) => setReason(event.target.value)}
            required
            disabled={isLoading}
            maxLength={500}
            placeholder={t("reasonPlaceholder")}
          />
        </div>

        <div className="flex justify-end gap-2 border-t border-erp-border pt-3">
          <Button
            type="button"
            variant="outline"
            onClick={onClose}
            disabled={isLoading}
          >
            {tCommon("actions.cancel")}
          </Button>
          <Button
            type="submit"
            variant="danger"
            isLoading={isLoading}
            disabled={isLoading || !reason.trim()}
          >
            {t("deactivateConversionConfirm")}
          </Button>
        </div>
      </form>
    </Modal>
  );
}
