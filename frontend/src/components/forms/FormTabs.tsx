"use client";

import React from "react";
import { cn } from "@/lib/utils/cn";

export interface FormTabItem {
  id: string;
  label: string;
  icon?: React.ReactNode;
  badge?: React.ReactNode;
}

export interface FormTabsProps {
  tabs: FormTabItem[];
  activeTab: string;
  onChange: (tabId: string) => void;
  className?: string;
}

export function FormTabs({
  tabs,
  activeTab,
  onChange,
  className,
}: FormTabsProps) {
  return (
    <div className={cn("flex border-b border-erp-border bg-erp-surface-subtle overflow-x-auto rounded-none", className)}>
      {tabs.map((tab) => {
        const isActive = activeTab === tab.id;
        return (
          <button
            key={tab.id}
            type="button"
            onClick={() => onChange(tab.id)}
            className={cn(
              "relative flex items-center gap-2 px-4 py-2.5 text-xs font-semibold uppercase tracking-wider transition-colors rounded-none whitespace-nowrap",
              isActive
                ? "bg-erp-surface text-erp-navy border-b-2 border-erp-navy font-bold"
                : "text-erp-text-muted hover:bg-erp-surface/60 hover:text-erp-text-main"
            )}
          >
            {tab.icon}
            <span>{tab.label}</span>
            {tab.badge}
          </button>
        );
      })}
    </div>
  );
}

FormTabs.displayName = "FormTabs";
