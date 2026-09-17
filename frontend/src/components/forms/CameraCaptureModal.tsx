"use client";

import React, { useState, useEffect } from "react";
import Image from "next/image";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import {
  IconCamera,
  IconCheck,
  IconRefresh,
  IconAlertCircle,
  IconSpinner,
} from "@/components/common/Icons";
import { useCamera, type FacingMode, type CameraError } from "@/hooks/useCamera";
import { useTranslations } from "next-intl";

export interface CameraCaptureModalProps {
  isOpen: boolean;
  onClose: () => void;
  onCapture: (file: File) => void;
  title?: string;
  preferredFacingMode?: FacingMode;
  onTriggerNativeCamera?: () => void;
}

export function CameraCaptureModal({
  isOpen,
  onClose,
  onCapture,
  title,
  preferredFacingMode = "environment",
  onTriggerNativeCamera,
}: CameraCaptureModalProps) {
  const t = useTranslations("common.camera");
  const tActions = useTranslations("common.actions");

  const [snapshot, setSnapshot] = useState<{ file: File; url: string } | null>(null);
  const [isCapturing, setIsCapturing] = useState(false);

  const {
    videoRef,
    isActive,
    isLoading,
    error,
    facingMode,
    startCamera,
    stopCamera,
    switchCamera,
    capturePhoto,
  } = useCamera({
    initialFacingMode: preferredFacingMode,
    preferredResolution: { width: 1920, height: 1080 },
    autoStart: false,
  });

  // Start camera when modal opens, stop when closes
  useEffect(() => {
    if (isOpen) {
      setSnapshot(null);
      void startCamera();
    } else {
      stopCamera();
      if (snapshot?.url) {
        URL.revokeObjectURL(snapshot.url);
        setSnapshot(null);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isOpen]);

  // Clean up object URL on unmount
  useEffect(() => {
    return () => {
      if (snapshot?.url) {
        URL.revokeObjectURL(snapshot.url);
      }
    };
  }, [snapshot]);

  const handleClose = () => {
    stopCamera();
    if (snapshot?.url) {
      URL.revokeObjectURL(snapshot.url);
      setSnapshot(null);
    }
    onClose();
  };

  const handleShutter = async () => {
    if (!isActive || isLoading || isCapturing) return;
    setIsCapturing(true);

    try {
      const capturedFile = await capturePhoto({
        fileNamePrefix: "site_photo",
        format: "image/webp",
        quality: 0.92,
      });

      if (capturedFile) {
        const objectUrl = URL.createObjectURL(capturedFile);
        setSnapshot({ file: capturedFile, url: objectUrl });
        // Pause active stream while reviewing
        stopCamera();
      }
    } finally {
      setIsCapturing(false);
    }
  };

  const handleRetake = async () => {
    if (snapshot?.url) {
      URL.revokeObjectURL(snapshot.url);
    }
    setSnapshot(null);
    await startCamera();
  };

  const handleConfirmPhoto = () => {
    if (!snapshot) return;
    const fileToUse = snapshot.file;
    handleClose();
    onCapture(fileToUse);
  };

  const getLocalizedErrorMessage = (camError: CameraError | null): string => {
    if (!camError) return "";
    switch (camError.code) {
      case "PERMISSION_DENIED":
        return t("permissionDenied");
      case "NOT_FOUND":
        return t("notFound");
      case "NOT_READABLE":
        return t("notReadable");
      case "OVERCONSTRAINED":
        return t("overconstrained");
      case "UNSUPPORTED":
        return t("unsupported");
      default:
        return t("unknownError");
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={handleClose}
      title={title || t("modalTitle")}
      size="full"
      className="!w-screen !h-screen sm:!w-screen sm:!h-screen !max-w-none !max-h-none !m-0 !p-0 !border-0 bg-black flex flex-col"
      contentClassName="!p-0 !overflow-hidden flex-1 flex flex-col bg-black relative"
    >
      <div className="flex-1 flex flex-col relative w-full h-full overflow-hidden bg-black select-none">
        {/* Fullscreen Viewfinder / Video Canvas Container */}
        <div className="flex-1 relative w-full h-full bg-black overflow-hidden flex items-center justify-center">
          {/* Live Video element */}
          <video
            ref={videoRef}
            playsInline
            muted
            autoPlay
            className={`w-full h-full object-contain ${snapshot ? "hidden" : "block"}`}
          />

          {/* Active Camera Indicator Badge (Front / Back) */}
          {!snapshot && isActive && (
            <div
              className="absolute top-4 right-4 px-3 py-1.5 bg-black/85 border border-white/30 text-white text-xs font-mono tracking-wider z-20"
              style={{ borderRadius: "0px" }}
            >
              {facingMode === "environment" ? t("backCamera") : t("frontCamera")}
            </div>
          )}

          {/* Frozen Snapshot Review Mode */}
          {snapshot && (
            <div className="relative w-full h-full">
              <Image
                src={snapshot.url}
                alt="Captured Snapshot"
                fill
                unoptimized
                className="object-contain"
              />
              <div
                className="absolute top-4 left-4 px-3 py-1.5 bg-black/85 border border-white/30 text-white text-xs font-mono tracking-wider uppercase z-20"
                style={{ borderRadius: "0px" }}
              >
                {t("useThisPhoto")}?
              </div>
            </div>
          )}

          {/* Architectural Viewfinder Overlays (When in Live Mode) */}
          {!snapshot && isActive && (
            <div className="absolute inset-0 pointer-events-none flex flex-col justify-between p-6">
              {/* Top Hairline Crosshair corners */}
              <div className="flex justify-between w-full">
                <div className="w-8 h-8 border-t-2 border-l-2 border-white/80" />
                <div className="w-8 h-8 border-t-2 border-r-2 border-white/80" />
              </div>

              {/* Center Crosshair / Level Guide */}
              <div className="self-center flex items-center justify-center">
                <div className="relative w-16 h-16">
                  <div className="absolute inset-x-0 top-1/2 -translate-y-1/2 h-[1px] bg-white/50" />
                  <div className="absolute inset-y-0 left-1/2 -translate-x-1/2 w-[1px] bg-white/50" />
                  <div className="absolute inset-3 border border-white/40" />
                </div>
              </div>

              {/* Bottom Hairline Crosshair corners */}
              <div className="flex justify-between w-full">
                <div className="w-8 h-8 border-b-2 border-l-2 border-white/80" />
                <div className="w-8 h-8 border-b-2 border-r-2 border-white/80" />
              </div>
            </div>
          )}

          {/* Shutter flash animation overlay */}
          {isCapturing && (
            <div className="absolute inset-0 bg-white opacity-80 animate-pulse pointer-events-none z-30" />
          )}

          {/* Loading state */}
          {isLoading && !snapshot && (
            <div className="absolute inset-0 bg-black/80 flex flex-col items-center justify-center text-white gap-3 z-30">
              <IconSpinner className="w-10 h-10 animate-spin text-white" />
              <span className="text-sm font-mono">{t("cameraLoading")}</span>
            </div>
          )}

          {/* Error display */}
          {error && !snapshot && (
            <div className="absolute inset-0 bg-erp-slate-950 p-6 flex flex-col items-center justify-center text-center text-white gap-4 z-30">
              <IconAlertCircle className="w-12 h-12 text-destructive" />
              <div className="space-y-1.5 max-w-sm">
                <p className="text-base font-semibold text-destructive">{getLocalizedErrorMessage(error)}</p>
                <p className="text-xs text-erp-slate-400 font-mono">CODE: {error.code}</p>
              </div>

              {onTriggerNativeCamera && (
                <Button
                  type="button"
                  variant="primary"
                  size="md"
                  onClick={() => {
                    handleClose();
                    onTriggerNativeCamera();
                  }}
                  icon={<IconCamera size={18} />}
                >
                  {t("useNativeFallback")}
                </Button>
              )}
            </div>
          )}
        </div>

        {/* Viewfinder Instruction / Hint */}
        <div className="bg-erp-slate-950/90 py-1.5 px-4 text-center border-t border-white/10 shrink-0">
          <p className="text-xs text-erp-slate-300 font-mono">
            {!snapshot ? t("viewfinderHint") : `${snapshot.file.name} (${(snapshot.file.size / 1024).toFixed(1)} KB)`}
          </p>
        </div>

        {/* Action Controls Bar (Single Switch Button + Shutter + Cancel) */}
        <div className="shrink-0 bg-black px-6 py-4 border-t border-white/10 flex items-center justify-between gap-4">
          {/* Left Action: Cancel or Retake */}
          {snapshot ? (
            <Button
              type="button"
              variant="secondary"
              size="md"
              onClick={() => void handleRetake()}
              icon={<IconRefresh size={18} />}
            >
              {t("retakePhoto")}
            </Button>
          ) : (
            <Button
              type="button"
              variant="secondary"
              size="md"
              onClick={handleClose}
              className="border-white/20 text-white bg-white/10 hover:bg-white/20"
            >
              {tActions("cancel")}
            </Button>
          )}

          {/* Center Action: Shutter in live mode or Confirm in review mode */}
          {snapshot ? (
            <Button
              type="button"
              variant="primary"
              size="md"
              onClick={handleConfirmPhoto}
              icon={<IconCheck size={18} />}
              className="bg-emerald-600 hover:bg-emerald-700 font-semibold px-6"
            >
              {t("useThisPhoto")}
            </Button>
          ) : (
            <div className="flex items-center gap-3">
              <Button
                type="button"
                variant="primary"
                size="md"
                onClick={() => void handleShutter()}
                disabled={!isActive || isLoading || isCapturing}
                isLoading={isCapturing}
                icon={<IconCamera size={20} />}
                className="bg-erp-navy hover:bg-erp-navy-950 px-8 py-3 text-base font-bold shadow-lg"
              >
                {t("takePhoto")}
              </Button>

              {/* Single Switch Camera Button (Front / Back) */}
              <Button
                type="button"
                variant="secondary"
                size="md"
                onClick={() => void switchCamera()}
                disabled={!isActive || isLoading || isCapturing}
                icon={<IconRefresh size={18} />}
                title={t("switchCamera")}
                className="border-white/20 text-white bg-white/10 hover:bg-white/20"
              >
                {facingMode === "environment" ? t("frontCamera") : t("backCamera")}
              </Button>
            </div>
          )}
        </div>
      </div>
    </Modal>
  );
}

export default CameraCaptureModal;
