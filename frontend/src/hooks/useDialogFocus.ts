"use client";

import { useEffect, useRef } from "react";

const focusableSelector = 'button:not([disabled]), [href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])';

/** Share keyboard containment and focus restoration across modal surfaces. */
export function useDialogFocus(isOpen: boolean, onClose: () => void, closeDisabled = false, closeOnEscape = true) {
  const dialogRef = useRef<HTMLDivElement>(null);
  const behaviorRef = useRef({ onClose, closeDisabled, closeOnEscape });
  behaviorRef.current = { onClose, closeDisabled, closeOnEscape };

  useEffect(() => {
    if (!isOpen) return;
    const previousFocus = document.activeElement;
    const dialog = dialogRef.current;
    if (!dialog) return;

    const getFocusable = () => Array.from(dialog.querySelectorAll<HTMLElement>(focusableSelector))
      .filter((element) => !element.closest('[hidden], [aria-hidden="true"]'));
    const isTopDialog = () => {
      const dialogs = document.querySelectorAll('[role="dialog"][aria-modal="true"]');
      return dialogs[dialogs.length - 1] === dialog;
    };
    const focusFirst = () => (getFocusable()[0] ?? dialog).focus();
    const timer = setTimeout(focusFirst, 0);
    const handleKeyDown = (event: KeyboardEvent) => {
      if (!isTopDialog()) return;
      const behavior = behaviorRef.current;
      if (event.key === "Escape" && behavior.closeOnEscape && !behavior.closeDisabled) {
        event.preventDefault();
        behavior.onClose();
      }
      if (event.key !== "Tab") return;
      const elements = getFocusable();
      const first = elements[0];
      const last = elements[elements.length - 1];
      if (!first || !last) {
        event.preventDefault();
        dialog.focus();
      } else if (event.shiftKey && (document.activeElement === first || document.activeElement === dialog)) {
        event.preventDefault();
        last.focus();
      } else if (!event.shiftKey && document.activeElement === last) {
        event.preventDefault();
        first.focus();
      }
    };
    const handleFocus = (event: FocusEvent) => {
      if (isTopDialog() && event.target instanceof Node && !dialog.contains(event.target)) focusFirst();
    };
    document.addEventListener("keydown", handleKeyDown);
    document.addEventListener("focusin", handleFocus);
    return () => {
      clearTimeout(timer);
      document.removeEventListener("keydown", handleKeyDown);
      document.removeEventListener("focusin", handleFocus);
      if (previousFocus instanceof HTMLElement && previousFocus.isConnected) previousFocus.focus();
    };
  }, [isOpen]);

  return dialogRef;
}
