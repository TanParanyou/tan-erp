"use client";

import React, { useState, useEffect, useCallback, useRef } from "react";
import { ConfirmationModal, type ConfirmationVariant } from "@/components/ui/ConfirmationModal";

export interface UseConfirmOptions {
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  variant?: ConfirmationVariant;
  onConfirm?: () => Promise<void> | void;
}

export const useConfirm = () => {
  const [state, setState] = useState<{
    isOpen: boolean;
    options: UseConfirmOptions | null;
    isLoading: boolean;
  }>({ isOpen: false, options: null, isLoading: false });

  const resolverRef = useRef<((accepted: boolean) => void) | null>(null);

  const settle = useCallback((accepted: boolean) => {
    const resolve = resolverRef.current;
    resolverRef.current = null;
    if (resolve) {
      resolve(accepted);
    }
    setState({ isOpen: false, options: null, isLoading: false });
  }, []);

  const confirm = useCallback((options: UseConfirmOptions): Promise<boolean> => {
    if (resolverRef.current) {
      resolverRef.current(false);
      resolverRef.current = null;
    }
    return new Promise<boolean>((resolve) => {
      resolverRef.current = resolve;
      setState({ isOpen: true, options, isLoading: false });
    });
  }, []);

  const handleConfirm = useCallback(async () => {
    if (!state.options) return;

    if (state.options.onConfirm) {
      setState((prev) => ({ ...prev, isLoading: true }));
      try {
        await state.options.onConfirm();
        settle(true);
      } catch {
        setState((prev) => ({ ...prev, isLoading: false }));
      }
    } else {
      settle(true);
    }
  }, [state.options, settle]);

  const handleClose = useCallback(() => {
    if (state.isLoading) return;
    settle(false);
  }, [state.isLoading, settle]);

  useEffect(() => {
    return () => {
      if (resolverRef.current) {
        resolverRef.current(false);
        resolverRef.current = null;
      }
    };
  }, []);

  const ConfirmDialog = useCallback(() => {
    if (!state.options) return null;
    return (
      <ConfirmationModal
        isOpen={state.isOpen}
        onClose={handleClose}
        onConfirm={handleConfirm}
        title={state.options.title}
        message={state.options.message}
        confirmText={state.options.confirmText}
        cancelText={state.options.cancelText}
        variant={state.options.variant}
        isLoading={state.isLoading}
      />
    );
  }, [state.isOpen, state.options, state.isLoading, handleClose, handleConfirm]);

  return { confirm, ConfirmDialog };
};

export default useConfirm;
