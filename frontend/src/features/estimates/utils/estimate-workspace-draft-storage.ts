import type { EstimateWorkspaceFormData } from "../schemas/estimate-workspace-schema";

interface StoredDraft {
  /** Revision row version the edits started from; a draft on any other version is stale. */
  baseVersion: string;
  savedAtUtc: string;
  values: EstimateWorkspaceFormData;
}

// sessionStorage, not localStorage: pricing data should not outlive the browser tab on a shared device.
function storage(): Storage | null {
  try {
    return typeof window === "undefined" ? null : window.sessionStorage;
  } catch {
    return null;
  }
}

export const draftStorageKey = (membershipId: string, estimateId: string, revisionId: string) =>
  `estimate-workspace-draft:${membershipId}:${estimateId}:${revisionId}`;

export function loadWorkspaceDraft(key: string, baseVersion: string | undefined): StoredDraft | null {
  const store = storage();
  if (!store) return null;
  try {
    const raw = store.getItem(key);
    if (!raw) return null;
    const parsed: unknown = JSON.parse(raw);
    if (typeof parsed !== "object" || parsed === null || !("baseVersion" in parsed) || !("values" in parsed) || !("savedAtUtc" in parsed)) {
      store.removeItem(key);
      return null;
    }
    const draft = parsed as StoredDraft;
    if (draft.baseVersion !== baseVersion) { store.removeItem(key); return null; }
    return draft;
  } catch {
    store.removeItem(key);
    return null;
  }
}

export function saveWorkspaceDraft(key: string, baseVersion: string, values: EstimateWorkspaceFormData): void {
  try {
    storage()?.setItem(key, JSON.stringify({ baseVersion, savedAtUtc: new Date().toISOString(), values } satisfies StoredDraft));
  } catch {
    // Quota or privacy mode: recovery is best-effort and must never block editing.
  }
}

export function clearWorkspaceDraft(key: string): void {
  try {
    storage()?.removeItem(key);
  } catch {
    // ignore
  }
}
