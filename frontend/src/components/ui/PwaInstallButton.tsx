"use client";

import React, { useState } from "react";
import { usePwaInstall } from "@/hooks/usePwaInstall";
import { Button, type ButtonProps } from "@/components/ui/Button";
import { IconDownload } from "@/components/common/Icons";
import { IosInstallModal } from "./IosInstallModal";

export interface PwaInstallButtonProps extends Omit<ButtonProps, "onClick"> {
  label?: string;
}

export function PwaInstallButton({
  label = "ติดตั้ง Web App",
  className,
  ...props
}: PwaInstallButtonProps) {
  const { canInstall, isInstalled, isIOS, promptInstall } = usePwaInstall();
  const [showIosModal, setShowIosModal] = useState(false);

  if (isInstalled) return null;

  const handleClick = () => {
    if (isIOS) {
      setShowIosModal(true);
    } else if (canInstall) {
      promptInstall();
    }
  };

  return (
    <>
      <Button
        variant="secondary"
        size="sm"
        onClick={handleClick}
        className={className}
        {...props}
      >
        <IconDownload size={14} className="mr-1.5" />
        <span>{label}</span>
      </Button>
      <IosInstallModal
        isOpen={showIosModal}
        onClose={() => setShowIosModal(false)}
      />
    </>
  );
}

PwaInstallButton.displayName = "PwaInstallButton";
