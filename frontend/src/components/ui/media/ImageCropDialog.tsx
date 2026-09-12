"use client";

import React, { useState, useCallback } from "react";
import Cropper, { Point, Area } from "react-easy-crop";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { IconSpinner, IconRefresh } from "@/components/common/Icons";
import { getCroppedImg, PixelCrop } from "./cropUtils";

export interface ImageCropDialogProps {
  isOpen: boolean;
  imageSrc: string;
  onClose: () => void;
  onCropComplete: (croppedBlob: Blob) => void;
  aspectRatio?: number;
}

export function ImageCropDialog({
  isOpen,
  imageSrc,
  onClose,
  onCropComplete,
  aspectRatio = 16 / 9,
}: ImageCropDialogProps) {
  const [crop, setCrop] = useState<Point>({ x: 0, y: 0 });
  const [zoom, setZoom] = useState(1);
  const [rotation, setRotation] = useState(0);
  const [croppedAreaPixels, setCroppedAreaPixels] = useState<PixelCrop | null>(null);
  const [isProcessing, setIsProcessing] = useState(false);

  const handleCropCompleteCallback = useCallback(
    (_croppedArea: Area, currentCroppedAreaPixels: Area) => {
      setCroppedAreaPixels(currentCroppedAreaPixels);
    },
    []
  );

  const handleApplyCrop = async () => {
    if (!croppedAreaPixels) return;
    try {
      setIsProcessing(true);
      const croppedBlob = await getCroppedImg(imageSrc, croppedAreaPixels, rotation);
      if (croppedBlob) {
        onCropComplete(croppedBlob);
        onClose();
      }
    } catch (err) {
      console.error("Crop error:", err);
    } finally {
      setIsProcessing(false);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="ตัดแต่งรูปภาพ (Crop Image)"
      size="lg"
    >
      <div className="space-y-4">
        <div className="relative h-80 w-full overflow-hidden bg-black/90 border border-erp-border rounded-none">
          <Cropper
            image={imageSrc}
            crop={crop}
            zoom={zoom}
            rotation={rotation}
            aspect={aspectRatio}
            onCropChange={setCrop}
            onCropComplete={handleCropCompleteCallback}
            onZoomChange={setZoom}
          />
        </div>

        <div className="flex flex-wrap items-center justify-between gap-4 border-t border-erp-border pt-3">
          <div className="flex items-center gap-2">
            <span className="text-xs font-medium text-erp-text-muted">ซูม:</span>
            <input
              type="range"
              min={1}
              max={3}
              step={0.1}
              value={zoom}
              onChange={(e) => setZoom(Number(e.target.value))}
              className="h-1.5 w-32 cursor-pointer bg-erp-border accent-erp-navy"
            />
          </div>

          <div className="flex items-center gap-2">
            <Button
              type="button"
              variant="secondary"
              size="sm"
              onClick={() => setRotation((r) => (r + 90) % 360)}
              className="gap-1 rounded-none"
            >
              <IconRefresh size={14} />
              <span>หมุน 90°</span>
            </Button>
          </div>
        </div>

        <div className="flex justify-end gap-2 border-t border-erp-border pt-3">
          <Button variant="secondary" onClick={onClose} disabled={isProcessing}>
            ยกเลิก
          </Button>
          <Button variant="primary" onClick={handleApplyCrop} disabled={isProcessing}>
            {isProcessing ? <IconSpinner size={14} className="animate-spin mr-1.5" /> : null}
            <span>บันทึกการตัดรูป</span>
          </Button>
        </div>
      </div>
    </Modal>
  );
}

ImageCropDialog.displayName = "ImageCropDialog";
