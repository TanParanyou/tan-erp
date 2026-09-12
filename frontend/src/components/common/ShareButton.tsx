"use client";

import React, { useState } from "react";
import { Button, type ButtonProps } from "@/components/ui/Button";
import { IconCheck, IconCopy } from "@/components/common/Icons";

export interface ShareButtonProps extends Omit<ButtonProps, "onClick"> {
  url?: string;
  title?: string;
  label?: string;
}

export function ShareButton({
  url,
  label = "แชร์ลิงก์",
  className,
  ...props
}: ShareButtonProps) {
  const [copied, setCopied] = useState(false);

  const handleShare = async () => {
    const targetUrl = url || (typeof window !== "undefined" ? window.location.href : "");
    if (!targetUrl) return;

    if (navigator.clipboard) {
      await navigator.clipboard.writeText(targetUrl);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  return (
    <Button
      variant="secondary"
      size="sm"
      onClick={handleShare}
      className={className}
      {...props}
    >
      {copied ? <IconCheck size={14} className="mr-1 text-emerald-600" /> : <IconCopy size={14} className="mr-1" />}
      <span>{copied ? "คัดลอกแล้ว" : label}</span>
    </Button>
  );
}

ShareButton.displayName = "ShareButton";
