"use client";

import React, { useEffect, useRef, useState, useCallback } from "react";
import { IconRefresh, IconSave, IconCheck } from "@/components/common/Icons";
import { Button } from "@/components/ui/Button";
import { cn } from "@/lib/utils/cn";

export interface SignaturePadProps {
  value?: string | null;
  onChange: (value: string | null) => void;
  label?: string;
  clearButtonText?: string;
  helperText?: string;
  height?: number;
  onSaveToPresets?: (name: string, url: string) => void;
  className?: string;
}

export function SignaturePad({
  value,
  onChange,
  label = "ลงลายมือชื่อดิจิทัล (Digital Signature)",
  clearButtonText = "ล้างลายเซ็น",
  helperText,
  height = 160,
  onSaveToPresets,
  className,
}: SignaturePadProps) {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const isDrawingRef = useRef(false);
  const [hasDrawn, setHasDrawn] = useState(Boolean(value));
  const [presetName, setPresetName] = useState("");
  const [showPresetInput, setShowPresetInput] = useState(false);

  const setupCanvas = useCallback(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const rect = canvas.getBoundingClientRect();
    const dpr = window.devicePixelRatio || 1;

    canvas.width = rect.width * dpr;
    canvas.height = rect.height * dpr;

    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    ctx.scale(dpr, dpr);
    ctx.strokeStyle = "#0B3056"; // Solid Architectural Navy
    ctx.lineWidth = 2.5;
    ctx.lineCap = "square";
    ctx.lineJoin = "miter";

    if (value) {
      const img = new Image();
      img.crossOrigin = "anonymous";
      img.onload = () => {
        ctx.clearRect(0, 0, rect.width, rect.height);
        ctx.drawImage(img, 0, 0, rect.width, rect.height);
        setHasDrawn(true);
      };
      img.src = value;
    }
  }, [value]);

  useEffect(() => {
    setupCanvas();
    const handleResize = () => setupCanvas();
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, [setupCanvas]);

  const getCoordinates = (
    e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>
  ): { x: number; y: number } | null => {
    const canvas = canvasRef.current;
    if (!canvas) return null;

    const rect = canvas.getBoundingClientRect();
    if ("touches" in e) {
      if (e.touches.length === 0) return null;
      return {
        x: e.touches[0].clientX - rect.left,
        y: e.touches[0].clientY - rect.top,
      };
    }
    return {
      x: e.clientX - rect.left,
      y: e.clientY - rect.top,
    };
  };

  const startDrawing = (
    e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>
  ) => {
    if ("touches" in e) e.stopPropagation();
    const coords = getCoordinates(e);
    if (!coords) return;

    const canvas = canvasRef.current;
    const ctx = canvas?.getContext("2d");
    if (!ctx) return;

    isDrawingRef.current = true;
    ctx.beginPath();
    ctx.moveTo(coords.x, coords.y);
  };

  const draw = (
    e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>
  ) => {
    if (!isDrawingRef.current) return;
    if ("touches" in e) {
      e.preventDefault();
      e.stopPropagation();
    }

    const coords = getCoordinates(e);
    if (!coords) return;

    const canvas = canvasRef.current;
    const ctx = canvas?.getContext("2d");
    if (!ctx) return;

    ctx.lineTo(coords.x, coords.y);
    ctx.stroke();
    setHasDrawn(true);
  };

  const stopDrawing = () => {
    if (!isDrawingRef.current) return;
    isDrawingRef.current = false;

    const canvas = canvasRef.current;
    if (!canvas) return;

    const dataUrl = canvas.toDataURL("image/png");
    onChange(dataUrl);
  };

  const handleClear = () => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    const rect = canvas.getBoundingClientRect();
    ctx.clearRect(0, 0, rect.width, rect.height);
    setHasDrawn(false);
    onChange(null);
  };

  return (
    <div className={cn("space-y-2 text-left", className)}>
      <div className="flex items-center justify-between">
        <span className="text-xs font-semibold uppercase tracking-wider text-erp-text-main">
          {label}
        </span>
        {hasDrawn && (
          <button
            type="button"
            onClick={handleClear}
            className="flex items-center gap-1 text-xs text-red-600 hover:underline"
          >
            <IconRefresh size={12} />
            <span>{clearButtonText}</span>
          </button>
        )}
      </div>

      <div
        className="relative w-full border border-erp-border bg-erp-surface rounded-none overflow-hidden touch-none"
        style={{ height }}
      >
        <canvas
          ref={canvasRef}
          onMouseDown={startDrawing}
          onMouseMove={draw}
          onMouseUp={stopDrawing}
          onMouseLeave={stopDrawing}
          onTouchStart={startDrawing}
          onTouchMove={draw}
          onTouchEnd={stopDrawing}
          className="h-full w-full cursor-crosshair block"
        />
        {!hasDrawn && (
          <div className="pointer-events-none absolute inset-0 flex flex-col justify-end p-3 text-erp-text-muted/40">
            <div className="mb-1 w-full border-b border-dashed border-erp-border" />
            <span className="text-[10px] font-mono">
              ✕ ลงลายมือชื่อที่นี่ (Sign here)
            </span>
          </div>
        )}
      </div>

      {hasDrawn && onSaveToPresets && (
        <div className="pt-1 space-y-2">
          <button
            type="button"
            onClick={() => setShowPresetInput(!showPresetInput)}
            className="text-xs text-erp-navy hover:underline font-medium"
          >
            + บันทึกลงคลังลายเซ็นส่วนบุคคล
          </button>
          {showPresetInput && (
            <div className="flex items-center gap-1.5 border border-erp-border bg-erp-surface-subtle p-2">
              <input
                type="text"
                value={presetName}
                onChange={(e) => setPresetName(e.target.value)}
                placeholder="ระบุชื่อลายเซ็น เช่น ลายเซ็นผู้ตรวจการ"
                className="flex-1 border border-erp-border bg-erp-surface p-1 text-xs text-erp-text-main outline-none focus:border-erp-navy rounded-none"
              />
              <Button
                type="button"
                size="sm"
                disabled={!presetName.trim()}
                onClick={() => {
                  const canvas = canvasRef.current;
                  const dataUrl = value || canvas?.toDataURL("image/png");
                  if (dataUrl && presetName.trim()) {
                    onSaveToPresets(presetName.trim(), dataUrl);
                    setPresetName("");
                    setShowPresetInput(false);
                  }
                }}
              >
                <IconCheck size={12} className="mr-1" />
                บันทึก
              </Button>
            </div>
          )}
        </div>
      )}

      {helperText && <p className="text-xs text-erp-text-muted">{helperText}</p>}
    </div>
  );
}

SignaturePad.displayName = "SignaturePad";
