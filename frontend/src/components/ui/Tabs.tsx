"use client";

import React from "react";
import { Button } from "@/components/ui/Button";

export interface TabConfig<T extends string> {
  id: T;
  label: string;
  icon?: React.ReactNode;
  hasError?: boolean;
}

export interface TabsProps<T extends string> {
  tabs: TabConfig<T>[];
  activeTab: T;
  setActiveTab: (tab: T) => void;
  className?: string;
}

export function Tabs<T extends string>({
  tabs,
  activeTab,
  setActiveTab,
  className = "",
}: TabsProps<T>) {
  return (
    <div
      role="tablist"
      className={`flex flex-wrap gap-2 border-b border-erp-border pb-3 ${className}`}
    >
      {tabs.map((tab) => {
        const isActive = activeTab === tab.id;
        return (
          <Button
            key={tab.id}
            role="tab"
            aria-selected={isActive}
            type="button"
            size="sm"
            variant={isActive ? "primary" : "outline"}
            icon={tab.icon}
            onClick={() => setActiveTab(tab.id)}
          >
            <span className="flex items-center gap-1.5">
              {tab.label}
              {tab.hasError && (
                <span className="w-2 h-2 rounded-none bg-erp-danger animate-pulse" />
              )}
            </span>
          </Button>
        );
      })}
    </div>
  );
}

export default Tabs;
