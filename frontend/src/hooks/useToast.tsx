"use client";

import React, {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useRef,
  useState,
} from "react";

export type ToastType = "success" | "error" | "info" | "warning";

export interface Toast {
  id: string;
  type: ToastType;
  message: string;
  description?: string;
  duration?: number;
}

export type ToastInput = Omit<Toast, "id"> & { id?: string };

export interface ToastContextValue {
  toasts: Toast[];
  showToast: (toast: ToastInput) => string;
  removeToast: (id: string) => void;
  clearToasts: () => void;
  toast: {
    success: (message: string, description?: string) => string;
    error: (message: string, description?: string) => string;
    info: (message: string, description?: string) => string;
    warning: (message: string, description?: string) => string;
  };
}

const ToastContext = createContext<ToastContextValue | null>(null);

const DEFAULT_DURATION: Record<ToastType, number> = {
  success: 4000,
  error: 5000,
  info: 4000,
  warning: 4500,
};

const createId = () => {
  if (typeof crypto !== "undefined" && "randomUUID" in crypto) {
    return crypto.randomUUID();
  }
  return `${Date.now()}-${Math.random().toString(16).slice(2)}`;
};

export function ToastProvider({ children }: { children: React.ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([]);
  const timersRef = useRef<Map<string, ReturnType<typeof setTimeout>>>(new Map());

  const removeToast = useCallback((id: string) => {
    const timer = timersRef.current.get(id);
    if (timer) {
      clearTimeout(timer);
      timersRef.current.delete(id);
    }
    setToasts((prev) => prev.filter((t) => t.id !== id));
  }, []);

  const clearToasts = useCallback(() => {
    timersRef.current.forEach((timer) => clearTimeout(timer));
    timersRef.current.clear();
    setToasts([]);
  }, []);

  const showToast = useCallback(
    (input: ToastInput) => {
      const id = input.id ?? createId();
      const duration = input.duration ?? DEFAULT_DURATION[input.type];

      setToasts((prev) => {
        const next = prev.filter((t) => t.id !== id);
        return [...next, { ...input, id, duration }];
      });

      if (duration > 0) {
        const timer = setTimeout(() => removeToast(id), duration);
        timersRef.current.set(id, timer);
      }

      return id;
    },
    [removeToast]
  );

  const toastMethods = useMemo(
    () => ({
      success: (message: string, description?: string) =>
        showToast({ type: "success", message, description }),
      error: (message: string, description?: string) =>
        showToast({ type: "error", message, description }),
      info: (message: string, description?: string) =>
        showToast({ type: "info", message, description }),
      warning: (message: string, description?: string) =>
        showToast({ type: "warning", message, description }),
    }),
    [showToast]
  );

  const value = useMemo(
    () => ({
      toasts,
      showToast,
      removeToast,
      clearToasts,
      toast: toastMethods,
    }),
    [toasts, showToast, removeToast, clearToasts, toastMethods]
  );

  return (
    <ToastContext.Provider value={value}>{children}</ToastContext.Provider>
  );
}

export function useToast() {
  const context = useContext(ToastContext);
  if (!context) {
    throw new Error("useToast must be used within ToastProvider");
  }
  return context;
}
