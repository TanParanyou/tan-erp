"use client";

import { useState, useCallback } from "react";
import { useTranslations } from "next-intl";
import { useToast } from "@/hooks/useToast";

export interface Coordinates {
  latitude: number;
  longitude: number;
}

export interface UseCurrentLocationOptions {
  enableHighAccuracy?: boolean;
  timeout?: number;
  maximumAge?: number;
  onSuccess?: (coords: Coordinates) => void;
  onError?: (error: GeolocationPositionError | Error) => void;
}

export function useCurrentLocation(options: UseCurrentLocationOptions = {}) {
  const {
    enableHighAccuracy = true,
    timeout = 10000,
    maximumAge = 0,
    onSuccess,
    onError,
  } = options;

  const t = useTranslations("common.geolocation");
  const { toast } = useToast();
  const [isLoading, setIsLoading] = useState(false);

  const getCurrentLocation = useCallback(async (): Promise<Coordinates | null> => {
    if (typeof window === "undefined" || !navigator.geolocation) {
      const errorMsg = t("notSupported");
      toast.error(errorMsg);
      onError?.(new Error(errorMsg));
      return null;
    }

    setIsLoading(true);

    return new Promise((resolve) => {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setIsLoading(false);
          const coords: Coordinates = {
            latitude: Number(position.coords.latitude.toFixed(7)),
            longitude: Number(position.coords.longitude.toFixed(7)),
          };
          toast.success(t("locationSuccess"));
          onSuccess?.(coords);
          resolve(coords);
        },
        (error) => {
          setIsLoading(false);
          let errorMsg = t("unavailable");
          if (error.code === error.PERMISSION_DENIED) {
            errorMsg = t("permissionDenied");
          } else if (error.code === error.TIMEOUT) {
            errorMsg = t("timeout");
          }
          toast.error(errorMsg);
          onError?.(error);
          resolve(null);
        },
        {
          enableHighAccuracy,
          timeout,
          maximumAge,
        }
      );
    });
  }, [enableHighAccuracy, timeout, maximumAge, onSuccess, onError, t, toast]);

  return {
    getCurrentLocation,
    isLoading,
  };
}
