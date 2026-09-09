"use client";

import React from "react";

export interface ListToolbarProps {
  children: React.ReactNode;
  activeFilters?: React.ReactNode;
  className?: string;
}

export function ListToolbar({
  children,
  activeFilters,
  className = "",
}: ListToolbarProps) {
  return (
    <div
      className={`flex flex-col gap-3.5 rounded-none border border-erp-border bg-erp-surface p-4 shadow-sm ${className}`}
    >
      <div className="flex flex-col md:flex-row flex-wrap md:items-end justify-between gap-3.5">
        {children}
      </div>
      {activeFilters}
    </div>
  );
}

export default ListToolbar;
