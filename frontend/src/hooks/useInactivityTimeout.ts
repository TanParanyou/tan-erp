"use client";

import { useEffect, useState, useCallback, useRef } from "react";

export interface UseInactivityTimeoutOptions {
  idleTimeoutMs?: number; // default 15 mins
  warningDurationMs?: number; // default 60 secs
  checkIntervalMs?: number; // default 1 sec
  onTimeout?: () => void | Promise<void>;
  enabled?: boolean;
}

export function useInactivityTimeout(options: UseInactivityTimeoutOptions = {}) {
  const {
    idleTimeoutMs = 15 * 60 * 1000,
    warningDurationMs = 60 * 1000,
    checkIntervalMs = 1000,
    onTimeout,
    enabled = true,
  } = options;

  const [isWarningOpen, setIsWarningOpen] = useState(false);
  const [secondsRemaining, setSecondsRemaining] = useState(
    Math.ceil(warningDurationMs / 1000)
  );

  const lastActivityRef = useRef<number>(Date.now());
  const isWarningOpenRef = useRef<boolean>(false);

  const resetActivity = useCallback(() => {
    lastActivityRef.current = Date.now();
    if (isWarningOpenRef.current) {
      isWarningOpenRef.current = false;
      setIsWarningOpen(false);
    }
  }, []);

  const stayLoggedIn = useCallback(() => {
    resetActivity();
  }, [resetActivity]);

  useEffect(() => {
    if (!enabled) {
      setIsWarningOpen(false);
      isWarningOpenRef.current = false;
      return;
    }

    lastActivityRef.current = Date.now();

    const activityEvents = [
      "mousemove",
      "mousedown",
      "keydown",
      "touchstart",
      "scroll",
    ];

    let throttleTimer: ReturnType<typeof setTimeout> | null = null;
    const handleUserActivity = () => {
      if (!throttleTimer && !isWarningOpenRef.current) {
        throttleTimer = setTimeout(() => {
          lastActivityRef.current = Date.now();
          throttleTimer = null;
        }, 1000);
      }
    };

    activityEvents.forEach((evt) =>
      window.addEventListener(evt, handleUserActivity, { passive: true })
    );

    const interval = setInterval(async () => {
      const elapsed = Date.now() - lastActivityRef.current;
      const timeUntilLogout = idleTimeoutMs - elapsed;

      if (timeUntilLogout <= 0) {
        clearInterval(interval);
        setIsWarningOpen(false);
        isWarningOpenRef.current = false;
        await onTimeout?.();
      } else if (timeUntilLogout <= warningDurationMs) {
        if (!isWarningOpenRef.current) {
          isWarningOpenRef.current = true;
          setIsWarningOpen(true);
        }
        setSecondsRemaining(Math.max(1, Math.ceil(timeUntilLogout / 1000)));
      } else {
        if (isWarningOpenRef.current) {
          isWarningOpenRef.current = false;
          setIsWarningOpen(false);
        }
      }
    }, checkIntervalMs);

    return () => {
      if (throttleTimer) clearTimeout(throttleTimer);
      clearInterval(interval);
      activityEvents.forEach((evt) =>
        window.removeEventListener(evt, handleUserActivity)
      );
    };
  }, [enabled, idleTimeoutMs, warningDurationMs, checkIntervalMs, onTimeout]);

  return {
    isWarningOpen,
    secondsRemaining,
    stayLoggedIn,
    resetActivity,
  };
}

export default useInactivityTimeout;
