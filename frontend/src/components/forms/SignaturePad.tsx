"use client";

import React, { useEffect, useRef, useState, useCallback } from "react";
import { IconRefresh, IconSave, IconCheck } from "@/components/common/Icons";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { cn } from "@/lib/utils/cn";
import { useTranslations } from "next-intl";

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
  label,
  clearButtonText,
  helperText,
  height = 160,
  onSaveToPresets,
  className,
}: SignaturePadProps) {
  const t = useTranslations("common.signaturePad");
  const effectiveLabel = label ?? t("defaultLabel");
  const effectiveClearText = clearButtonText ?? t("clear");
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const isDrawingRef = useRef(false);
  const [hasDrawn, setHasDrawn] = useState(Boolean(value));
  const [presetName, setPresetName] = useState("");
  const [showPresetInput, setShowPresetInput] = useState(false);

  const valueRef = useRef<string | null | undefined>(value);
  valueRef.current = value;

  const paintValue = useCallback((ctx: CanvasRenderingContext2D, width: number, height: number) => {
    const current = valueRef.current;
    ctx.clearRect(0, 0, width, height);
    if (!current) return;
    const img = new Image();
    img.crossOrigin = "anonymous";
    img.onload = () => {
      if (valueRef.current !== current) return; // value changed while the image was loading
      ctx.drawImage(img, 0, 0, width, height);
      setHasDrawn(true);
    };
    img.src = current;
  }, []);

  // Sizing resets the canvas, so it only runs on mount and resize; it must not depend on `value`,
  // otherwise every stroke (which emits a new value) would reinitialise and flicker the canvas.
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

    paintValue(ctx, rect.width, rect.height);
  }, [paintValue]);

  useEffect(() => {
    setupCanvas();
    const handleResize = () => setupCanvas();
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, [setupCanvas]);

  // A parent-driven reset (value becomes empty) must clear the canvas and the "drawn" state.
  useEffect(() => {
    if (value) return;
    const canvas = canvasRef.current;
    const ctx = canvas?.getContext("2d");
    if (canvas && ctx) {
      const rect = canvas.getBoundingClientRect();
      ctx.clearRect(0, 0, rect.width, rect.height);
    }
    setHasDrawn(false);
  }, [value]);

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
          {effectiveLabel}
        </span>
        {hasDrawn && (
          <button
            type="button"
            onClick={handleClear}
            className="flex items-center gap-1 text-xs text-red-600 hover:underline"
          >
            <IconRefresh size={12} />
            <span>{effectiveClearText}</span>
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
              {t("signHere")}
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
            {t("saveToPresets")}
          </button>
          {showPresetInput && (
            <div className="flex items-center gap-1.5 border border-erp-border bg-erp-surface-subtle p-2">
              <Input
                type="text"
                value={presetName}
                onChange={(e) => setPresetName(e.target.value)}
                placeholder={t("presetNamePlaceholder")}
                className="flex-1 p-1 text-xs"
                wrapperClassName="flex-1 mb-0"
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
                {t("save")}
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
