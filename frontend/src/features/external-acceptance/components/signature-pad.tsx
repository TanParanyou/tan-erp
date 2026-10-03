"use client";

import { useEffect, useRef } from "react";
import { Button } from "@/components/ui/Button";

interface SignaturePadProps {
  label: string;
  clearLabel: string;
  disabled?: boolean;
  /** Called with a PNG data URL after each stroke, or null when the pad is empty. */
  onChange: (dataUrl: string | null) => void;
}

const WIDTH = 480;
const HEIGHT = 160;

/** A small pointer-driven drawing area. The image is optional evidence; the typed name and consent remain the signature of record. */
export function SignaturePad({ label, clearLabel, disabled = false, onChange }: SignaturePadProps) {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const drawingRef = useRef(false);
  const drawnRef = useRef(false);

  useEffect(() => {
    const context = canvasRef.current?.getContext("2d");
    if (!context) return;
    context.lineWidth = 2;
    context.lineCap = "round";
    context.strokeStyle = "#0B3056";
  }, []);

  function position(event: React.PointerEvent<HTMLCanvasElement>): { x: number; y: number } {
    const rect = event.currentTarget.getBoundingClientRect();
    return { x: ((event.clientX - rect.left) / rect.width) * WIDTH, y: ((event.clientY - rect.top) / rect.height) * HEIGHT };
  }

  function start(event: React.PointerEvent<HTMLCanvasElement>): void {
    if (disabled) return;
    const context = event.currentTarget.getContext("2d");
    if (!context) return;
    drawingRef.current = true;
    event.currentTarget.setPointerCapture(event.pointerId);
    const { x, y } = position(event);
    context.beginPath();
    context.moveTo(x, y);
  }

  function move(event: React.PointerEvent<HTMLCanvasElement>): void {
    if (!drawingRef.current) return;
    const context = event.currentTarget.getContext("2d");
    if (!context) return;
    const { x, y } = position(event);
    context.lineTo(x, y);
    context.stroke();
    drawnRef.current = true;
  }

  function end(event: React.PointerEvent<HTMLCanvasElement>): void {
    if (!drawingRef.current) return;
    drawingRef.current = false;
    onChange(drawnRef.current ? event.currentTarget.toDataURL("image/png") : null);
  }

  function clear(): void {
    const canvas = canvasRef.current;
    canvas?.getContext("2d")?.clearRect(0, 0, WIDTH, HEIGHT);
    drawnRef.current = false;
    onChange(null);
  }

  return (
    <div className="space-y-2">
      <p className="text-sm font-medium">{label}</p>
      <canvas
        ref={canvasRef}
        width={WIDTH}
        height={HEIGHT}
        aria-label={label}
        className="w-full max-w-md touch-none border border-erp-border bg-white"
        onPointerDown={start}
        onPointerMove={move}
        onPointerUp={end}
        onPointerCancel={end}
      />
      <Button type="button" size="sm" variant="outline" className="min-h-11" disabled={disabled} onClick={clear}>{clearLabel}</Button>
    </div>
  );
}
