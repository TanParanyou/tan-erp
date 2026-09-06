"use client";

import React from "react";
import { Button } from "@/components/ui/Button";
import { IconFileText } from "@/components/common/Icons";
import { cn } from "@/lib/utils/cn";
import { useTranslations } from "next-intl";

export interface EmptyStateProps {
  title?: string;
  description?: string;
  icon?: React.ReactNode;
  action?: {
    label: string;
    onClick?: () => void;
    href?: string;
    icon?: React.ReactNode;
  };
  className?: string;
}

export function EmptyState({
  title,
  description,
  icon,
  action,
  className,
}: EmptyStateProps) {
  const tFeedback = useTranslations("common.feedback");
  const tTable = useTranslations("common.table");
  const resolvedTitle = title || tFeedback("noData");
  const resolvedDescription = description || tTable("emptyTable");

  return (
    <div
      className={cn("erp-card", className)}
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        textAlign: "center",
        padding: "3rem 1.5rem",
        backgroundColor: "var(--erp-surface)",
        border: "1px dashed var(--erp-border)",
      }}
    >
      <div
        style={{
          width: "48px",
          height: "48px",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          backgroundColor: "var(--erp-surface-muted)",
          color: "var(--erp-text-muted)",
          marginBottom: "1rem",
        }}
      >
        {icon || <IconFileText size={24} />}
      </div>

      <h3
        style={{
          fontSize: "1rem",
          fontWeight: 600,
          color: "var(--erp-text-main)",
          marginBottom: "0.25rem",
        }}
      >
        {resolvedTitle}
      </h3>

      <p
        style={{
          fontSize: "0.875rem",
          color: "var(--erp-text-muted)",
          maxWidth: "400px",
          marginBottom: action ? "1.25rem" : 0,
        }}
      >
        {resolvedDescription}
      </p>

      {action && (
        <Button
          variant="primary"
          size="sm"
          onClick={action.onClick}
          href={action.href}
          icon={action.icon}
        >
          {action.label}
        </Button>
      )}
    </div>
  );
}
