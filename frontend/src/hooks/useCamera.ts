"use client";

import { useState, useCallback, useRef, useEffect } from "react";

export type CameraErrorCode =
  | "PERMISSION_DENIED"
  | "NOT_FOUND"
  | "NOT_READABLE"
  | "OVERCONSTRAINED"
  | "UNSUPPORTED"
  | "UNKNOWN";

export interface CameraError {
  code: CameraErrorCode;
  originalError?: unknown;
}

export type FacingMode = "user" | "environment";

export interface CapturePhotoOptions {
  videoElement?: HTMLVideoElement | null;
  fileNamePrefix?: string;
  format?: "image/webp" | "image/jpeg" | "image/png";
  quality?: number;
  maxDimension?: number;
}

export interface UseCameraOptions {
  initialFacingMode?: FacingMode;
  preferredResolution?: { width: number; height: number };
  autoStart?: boolean;
  onError?: (error: CameraError) => void;
}

export interface UseCameraReturn {
  videoRef: React.RefObject<HTMLVideoElement | null>;
  stream: MediaStream | null;
  isActive: boolean;
  isLoading: boolean;
  error: CameraError | null;
  devices: MediaDeviceInfo[];
  selectedDeviceId: string | null;
  facingMode: FacingMode;
  hasMultipleCameras: boolean;
  isSupported: boolean;
  startCamera: (constraints?: MediaTrackConstraints) => Promise<boolean>;
  stopCamera: () => void;
  switchCamera: () => Promise<void>;
  selectDevice: (deviceId: string) => Promise<void>;
  capturePhoto: (options?: CapturePhotoOptions) => Promise<File | null>;
}

function parseCameraError(err: unknown): CameraError {
  if (err instanceof DOMException || (typeof err === "object" && err !== null && "name" in err)) {
    const name = String((err as { name: unknown }).name);
    switch (name) {
      case "NotAllowedError":
      case "PermissionDeniedError":
        return { code: "PERMISSION_DENIED", originalError: err };
      case "NotFoundError":
      case "DevicesNotFoundError":
        return { code: "NOT_FOUND", originalError: err };
      case "NotReadableError":
      case "TrackStartError":
        return { code: "NOT_READABLE", originalError: err };
      case "OverconstrainedError":
      case "ConstraintNotSatisfiedError":
        return { code: "OVERCONSTRAINED", originalError: err };
      default:
        return { code: "UNKNOWN", originalError: err };
    }
  }
  return { code: "UNKNOWN", originalError: err };
}

function generateTimestampFilename(prefix: string, extension: string): string {
  const now = new Date();
  const pad = (n: number) => n.toString().padStart(2, "0");
  const yyyy = now.getFullYear();
  const mm = pad(now.getMonth() + 1);
  const dd = pad(now.getDate());
  const hh = pad(now.getHours());
  const min = pad(now.getMinutes());
  const ss = pad(now.getSeconds());
  return `${prefix}_${yyyy}${mm}${dd}_${hh}${min}${ss}.${extension}`;
}

export function useCamera(options: UseCameraOptions = {}): UseCameraReturn {
  const {
    initialFacingMode = "environment",
    preferredResolution = { width: 1920, height: 1080 },
    autoStart = false,
    onError,
  } = options;

  const [stream, setStream] = useState<MediaStream | null>(null);
  const [isActive, setIsActive] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<CameraError | null>(null);
  const [devices, setDevices] = useState<MediaDeviceInfo[]>([]);
  const [selectedDeviceId, setSelectedDeviceId] = useState<string | null>(null);
  const [facingMode, setFacingMode] = useState<FacingMode>(initialFacingMode);

  const videoRef = useRef<HTMLVideoElement | null>(null);
  const streamRef = useRef<MediaStream | null>(null);

  const isSupported =
    typeof navigator !== "undefined" &&
    Boolean(navigator.mediaDevices && navigator.mediaDevices.getUserMedia);

  const stopCamera = useCallback(() => {
    if (streamRef.current) {
      streamRef.current.getTracks().forEach((track) => {
        track.stop();
      });
      streamRef.current = null;
    }
    if (videoRef.current) {
      videoRef.current.srcObject = null;
    }
    setStream(null);
    setIsActive(false);
    setIsLoading(false);
  }, []);

  const refreshDevices = useCallback(async () => {
    if (!navigator?.mediaDevices?.enumerateDevices) return;
    try {
      const allDevices = await navigator.mediaDevices.enumerateDevices();
      const videoDevices = allDevices.filter((d) => d.kind === "videoinput");
      setDevices(videoDevices);
    } catch {
      // Ignored: device enumeration may fail silently if permission not yet granted
    }
  }, []);

  const startCamera = useCallback(
    async (customConstraints?: MediaTrackConstraints): Promise<boolean> => {
      if (!isSupported) {
        const unsupportedErr: CameraError = { code: "UNSUPPORTED" };
        setError(unsupportedErr);
        onError?.(unsupportedErr);
        return false;
      }

      setIsLoading(true);
      setError(null);

      // Stop existing stream first
      if (streamRef.current) {
        streamRef.current.getTracks().forEach((track) => track.stop());
        streamRef.current = null;
      }

      try {
        const videoConstraints: MediaTrackConstraints = {
          width: { ideal: preferredResolution.width },
          height: { ideal: preferredResolution.height },
          ...customConstraints,
        };

        if (selectedDeviceId) {
          videoConstraints.deviceId = { exact: selectedDeviceId };
        } else {
          videoConstraints.facingMode = { ideal: facingMode };
        }

        const newStream = await navigator.mediaDevices.getUserMedia({
          video: videoConstraints,
          audio: false,
        });

        streamRef.current = newStream;
        setStream(newStream);
        setIsActive(true);

        if (videoRef.current) {
          videoRef.current.srcObject = newStream;
          videoRef.current.setAttribute("playsinline", "true");
          videoRef.current.play().catch(() => {
            // Autoplay could be blocked by browser policy until user gesture
          });
        }

        await refreshDevices();
        setIsLoading(false);
        return true;
      } catch (err: unknown) {
        const parsedError = parseCameraError(err);
        setError(parsedError);
        onError?.(parsedError);
        stopCamera();
        return false;
      }
    },
    [
      facingMode,
      isSupported,
      onError,
      preferredResolution.height,
      preferredResolution.width,
      refreshDevices,
      selectedDeviceId,
      stopCamera,
    ]
  );

  const switchCamera = useCallback(async () => {
    // Toggle facingMode strictly between Front ("user") and Back ("environment")
    const nextFacingMode: FacingMode = facingMode === "environment" ? "user" : "environment";
    setFacingMode(nextFacingMode);
    setSelectedDeviceId(null);

    // If devices are enumerated, find an explicit device matching the target facing mode if available
    if (devices.length > 1) {
      const matchDevice = devices.find((d) => {
        const label = (d.label || "").toLowerCase();
        if (nextFacingMode === "user") {
          return (
            label.includes("front") ||
            label.includes("user") ||
            label.includes("face") ||
            label.includes("selfie")
          );
        } else {
          return (
            label.includes("back") ||
            label.includes("rear") ||
            label.includes("environment")
          );
        }
      });

      if (matchDevice) {
        setSelectedDeviceId(matchDevice.deviceId);
        await startCamera({
          deviceId: { exact: matchDevice.deviceId },
          facingMode: { ideal: nextFacingMode },
        });
        return;
      }
    }

    // Standard WebRTC facingMode toggle
    try {
      await startCamera({ facingMode: { exact: nextFacingMode } });
    } catch {
      await startCamera({ facingMode: { ideal: nextFacingMode } });
    }
  }, [devices, facingMode, startCamera]);

  const selectDevice = useCallback(
    async (deviceId: string) => {
      setSelectedDeviceId(deviceId);
      await startCamera({ deviceId: { exact: deviceId } });
    },
    [startCamera]
  );

  const capturePhoto = useCallback(
    async (captureOptions: CapturePhotoOptions = {}): Promise<File | null> => {
      const targetVideo = captureOptions.videoElement ?? videoRef.current;
      if (!targetVideo || targetVideo.videoWidth === 0 || targetVideo.videoHeight === 0) {
        return null;
      }

      const {
        fileNamePrefix = "site_photo",
        format = "image/webp",
        quality = 0.92,
        maxDimension,
      } = captureOptions;

      let targetWidth = targetVideo.videoWidth;
      let targetHeight = targetVideo.videoHeight;

      if (maxDimension && (targetWidth > maxDimension || targetHeight > maxDimension)) {
        if (targetWidth > targetHeight) {
          targetHeight = Math.round((targetHeight * maxDimension) / targetWidth);
          targetWidth = maxDimension;
        } else {
          targetWidth = Math.round((targetWidth * maxDimension) / targetHeight);
          targetHeight = maxDimension;
        }
      }

      const canvas = document.createElement("canvas");
      canvas.width = targetWidth;
      canvas.height = targetHeight;
      const ctx = canvas.getContext("2d");
      if (!ctx) return null;

      // Draw current video frame to canvas
      ctx.drawImage(targetVideo, 0, 0, targetWidth, targetHeight);

      const extension = format === "image/webp" ? "webp" : format === "image/png" ? "png" : "jpg";
      const filename = generateTimestampFilename(fileNamePrefix, extension);

      return new Promise<File | null>((resolve) => {
        canvas.toBlob(
          (blob) => {
            if (!blob) {
              // Fallback to JPEG if browser fails to export to WebP
              if (format === "image/webp") {
                canvas.toBlob(
                  (fallbackBlob) => {
                    if (!fallbackBlob) {
                      resolve(null);
                      return;
                    }
                    const fallbackFilename = generateTimestampFilename(fileNamePrefix, "jpg");
                    const file = new File([fallbackBlob], fallbackFilename, {
                      type: "image/jpeg",
                      lastModified: Date.now(),
                    });
                    resolve(file);
                  },
                  "image/jpeg",
                  quality
                );
                return;
              }
              resolve(null);
              return;
            }

            const file = new File([blob], filename, {
              type: blob.type || format,
              lastModified: Date.now(),
            });
            resolve(file);
          },
          format,
          quality
        );
      });
    },
    []
  );

  // Auto start if requested
  useEffect(() => {
    if (autoStart) {
      void startCamera();
    }
    return () => {
      stopCamera();
    };
  }, [autoStart, startCamera, stopCamera]);

  const hasMultipleCameras = devices.length > 1;

  return {
    videoRef,
    stream,
    isActive,
    isLoading,
    error,
    devices,
    selectedDeviceId,
    facingMode,
    hasMultipleCameras,
    isSupported,
    startCamera,
    stopCamera,
    switchCamera,
    selectDevice,
    capturePhoto,
  };
}

export default useCamera;
