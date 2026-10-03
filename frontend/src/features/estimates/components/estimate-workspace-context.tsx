"use client";

import { createContext, useContext } from "react";

export interface WorkspaceTarget { sectionKey: string; workKey: string; mode?: "costs" | "workItem" }
export interface EstimateWorkspaceContextValue {
  selectedKey: string | null;
  selectWork: (key: string) => void;
  inspector: HTMLDivElement | null;
  searchQuery: string;
  filterMode: "all" | "low_margin";
  openCatalog: (target: WorkspaceTarget) => void;
  catalogTarget: WorkspaceTarget | null;
  focusTargetId?: string;
  errorTarget: { key: string; tab: "info" | "cost" | "pricing"; attempt: number } | null;
}

export const EstimateWorkspaceContext = createContext<EstimateWorkspaceContextValue | null>(null);
export const useEstimateWorkspace = () => useContext(EstimateWorkspaceContext);

/** Scroll only a workspace pane; scrollIntoView can also move the clipped drawer window. */
export function scrollEstimateTarget(target: HTMLElement) {
  let pane = target.parentElement;
  while (pane) {
    if (pane.hasAttribute("data-estimate-scroll") && pane.scrollHeight > pane.clientHeight) {
      pane.scrollTop += target.getBoundingClientRect().top - pane.getBoundingClientRect().top;
      return;
    }
    pane = pane.parentElement;
  }
}
