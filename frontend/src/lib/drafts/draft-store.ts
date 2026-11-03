/**
 * Browser-local draft storage (IndexedDB). Best-effort cache for unsaved form state:
 * callers must tolerate failure (private mode, quota, blocked storage).
 * Values may contain File/Blob objects, which IndexedDB stores natively.
 */

const DB_NAME = "tan-erp-drafts";
const STORE_NAME = "drafts";
const DB_VERSION = 1;

export const DRAFT_MAX_AGE_MS = 7 * 24 * 60 * 60 * 1000;

export interface StoredDraft<T> {
  savedAt: number;
  payload: T;
}

function openDraftDb(): Promise<IDBDatabase> {
  return new Promise((resolve, reject) => {
    if (typeof indexedDB === "undefined") {
      reject(new Error("IndexedDB is not available"));
      return;
    }
    const request = indexedDB.open(DB_NAME, DB_VERSION);
    request.onupgradeneeded = () => {
      request.result.createObjectStore(STORE_NAME);
    };
    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error ?? new Error("Failed to open draft database"));
  });
}

async function runRequest<R>(
  mode: IDBTransactionMode,
  run: (store: IDBObjectStore) => IDBRequest<R>
): Promise<R> {
  const db = await openDraftDb();
  return new Promise<R>((resolve, reject) => {
    const transaction = db.transaction(STORE_NAME, mode);
    const request = run(transaction.objectStore(STORE_NAME));
    transaction.oncomplete = () => {
      db.close();
      resolve(request.result);
    };
    const fail = () => {
      db.close();
      reject(transaction.error ?? new Error("Draft transaction failed"));
    };
    transaction.onerror = fail;
    transaction.onabort = fail;
  });
}

export async function deleteDraft(key: string): Promise<void> {
  await runRequest("readwrite", (store) => store.delete(key));
}

export async function writeDraft<T>(key: string, payload: T): Promise<void> {
  const stored: StoredDraft<T> = { savedAt: Date.now(), payload };
  await runRequest("readwrite", (store) => store.put(stored, key));
}

/** Returns null when absent or older than DRAFT_MAX_AGE_MS (expired drafts are removed). */
export async function readDraft<T>(key: string): Promise<StoredDraft<T> | null> {
  const stored = await runRequest<StoredDraft<T> | undefined>("readonly", (store) => store.get(key));
  if (!stored) return null;
  if (Date.now() - stored.savedAt > DRAFT_MAX_AGE_MS) {
    await deleteDraft(key);
    return null;
  }
  return stored;
}

/** Removes every draft; called on sign-out so a shared device keeps nothing. */
export async function clearAllDrafts(): Promise<void> {
  await runRequest("readwrite", (store) => store.clear());
}
