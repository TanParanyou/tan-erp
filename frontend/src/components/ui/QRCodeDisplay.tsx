"use client";

import React, { useMemo } from "react";
import { generateQRCodeMatrix } from "@/lib/utils/qrcode";
import { cn } from "@/lib/utils/cn";

export interface QRCodeDisplayProps {
  value: string;
  size?: number;
  className?: string;
  label?: string;
}

export function QRCodeDisplay({
  value,
  size = 180,
  className,
  label,
}: QRCodeDisplayProps) {
  const matrix = useMemo(() => {
    if (!value) return [];
    try {
      return generateQRCodeMatrix(value);
    } catch {
      return [];
    }
  }, [value]);

  if (matrix.length === 0) {
    return (
      <div
        style={{ width: size, height: size }}
        className={cn(
          "flex items-center justify-center border border-erp-border bg-erp-surface-subtle p-2 text-center text-xs text-erp-text-muted rounded-none",
          className
        )}
      >
        ไม่สามารถสร้าง QR Code ได้
      </div>
    );
  }

  const moduleCount = matrix.length;
  const cellSize = size / moduleCount;

  return (
    <div className={cn("inline-flex flex-col items-center gap-2", className)}>
      <div
        style={{ width: size, height: size }}
        className="border border-erp-border bg-white p-2 shadow-2xs rounded-none"
      >
        <svg
          viewBox={`0 0 ${size} ${size}`}
          width="100%"
          height="100%"
          shapeRendering="crispEdges"
        >
          {matrix.map((row, r) =>
            row.map((isDark, c) =>
              isDark ? (
                <rect
                  key={`${r}-${c}`}
                  x={c * cellSize}
                  y={r * cellSize}
                  width={cellSize}
                  height={cellSize}
                  fill="#0B3056"
                />
              ) : null
            )
          )}
        </svg>
      </div>
      {label && <span className="text-xs text-erp-text-muted font-mono">{label}</span>}
    </div>
  );
}

QRCodeDisplay.displayName = "QRCodeDisplay";
