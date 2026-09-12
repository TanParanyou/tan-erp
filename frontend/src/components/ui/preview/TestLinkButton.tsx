"use client";

import React, { useState } from "react";
import { Button, type ButtonProps } from "@/components/ui/Button";
import { IconCheck, IconClose } from "@/components/common/Icons";

export interface TestLinkButtonProps extends Omit<ButtonProps, "onClick"> {
  url: string;
  label?: string;
}

export function TestLinkButton({
  url,
  label = "ทดสอบลิงก์",
  className,
  ...props
}: TestLinkButtonProps) {
  const [status, setStatus] = useState<"idle" | "valid" | "invalid">("idle");

  const handleTest = () => {
    if (!url) {
      setStatus("invalid");
      return;
    }
    try {
      new URL(url);
      window.open(url, "_blank", "noopener,noreferrer");
      setStatus("valid");
    } catch {
      setStatus("invalid");
    }
    setTimeout(() => setStatus("idle"), 3000);
  };

  return (
    <Button
      variant="secondary"
      size="sm"
      onClick={handleTest}
      className={className}
      {...props}
    >
      {status === "valid" && <IconCheck size={14} className="mr-1 text-emerald-600" />}
      {status === "invalid" && <IconClose size={14} className="mr-1 text-red-600" />}
      <span>{status === "invalid" ? "ลิงก์ไม่ถูกต้อง" : label}</span>
    </Button>
  );
}

TestLinkButton.displayName = "TestLinkButton";
