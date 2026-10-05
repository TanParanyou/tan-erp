"use client";

import React from "react";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";

import { useTranslations } from "next-intl";

export interface IosInstallModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function IosInstallModal({ isOpen, onClose }: IosInstallModalProps) {
  const t = useTranslations("common.iosInstall");

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={t("title")} size="sm">
      <div className="space-y-3.5 text-xs text-erp-text-main text-left">
        <p className="text-erp-text-muted leading-relaxed">
          {t("description")}
        </p>
        <ol className="list-decimal pl-4 space-y-2 leading-relaxed">
          <li>{t("step1")}</li>
          <li>{t("step2")}</li>
          <li>{t("step3")}</li>
        </ol>
        <div className="pt-3 border-t border-erp-border flex justify-end">
          <Button variant="primary" size="sm" onClick={onClose}>
            {t("gotIt")}
          </Button>
        </div>
      </div>
    </Modal>
  );
}

IosInstallModal.displayName = "IosInstallModal";
