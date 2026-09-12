"use client";

import React from "react";
import { SignaturePad } from "./SignaturePad";
import type { SignatureManagerProps } from "@/types/signatures";
import { cn } from "@/lib/utils/cn";

export function SignatureManager({
  signatureMode,
  onModeChange,
  liveSignature,
  onLiveSignatureChange,
  savedSignatureUrl,
  selectedPresetId,
  onSelectPresetId,
  presets,
  onSaveToPresets,
  onDeletePreset,
}: SignatureManagerProps) {
  return (
    <div className="flex flex-col gap-3 border border-erp-border bg-erp-surface p-3 sm:p-4 rounded-none text-left">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between border-b border-erp-border pb-2.5 gap-2">
        <span className="text-xs font-semibold uppercase tracking-wider text-erp-text-main">
          โหมดการลงลายมือชื่อ (Signature Mode)
        </span>
        <div className="flex items-center border border-erp-border bg-erp-surface-subtle p-0.5 rounded-none">
          <button
            type="button"
            onClick={() => onModeChange("saved")}
            className={cn(
              "px-2.5 py-1 text-xs font-medium transition-colors rounded-none",
              signatureMode === "saved"
                ? "bg-erp-navy text-white shadow-xs font-bold"
                : "text-erp-text-muted hover:text-erp-text-main"
            )}
          >
            จากคลัง ({presets.length})
          </button>
          <button
            type="button"
            onClick={() => onModeChange("pad")}
            className={cn(
              "px-2.5 py-1 text-xs font-medium transition-colors rounded-none",
              signatureMode === "pad"
                ? "bg-erp-navy text-white shadow-xs font-bold"
                : "text-erp-text-muted hover:text-erp-text-main"
            )}
          >
            เซ็นสด (Live Pad)
          </button>
          <button
            type="button"
            onClick={() => onModeChange("none")}
            className={cn(
              "px-2.5 py-1 text-xs font-medium transition-colors rounded-none",
              signatureMode === "none"
                ? "bg-erp-navy text-white shadow-xs font-bold"
                : "text-erp-text-muted hover:text-erp-text-main"
            )}
          >
            ไม่ระบุ
          </button>
        </div>
      </div>

      {signatureMode === "pad" && (
        <SignaturePad
          value={liveSignature}
          onChange={onLiveSignatureChange}
          onSaveToPresets={onSaveToPresets}
        />
      )}

      {signatureMode === "saved" && (
        <div className="space-y-3">
          {presets.length === 0 ? (
            <p className="py-4 text-center text-xs text-erp-text-muted">
              ยังไม่มีลายเซ็นในคลัง กรุณาเลือกโหมด "เซ็นสด" เพื่อบันทึกใหม่
            </p>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-2.5">
              {presets.map((preset) => {
                const isSelected = selectedPresetId === preset.id;
                return (
                  <div
                    key={preset.id}
                    onClick={() => onSelectPresetId(preset.id)}
                    className={cn(
                      "cursor-pointer border p-3 rounded-none transition-all flex flex-col justify-between",
                      isSelected
                        ? "border-erp-navy bg-erp-surface shadow-xs ring-1 ring-erp-navy"
                        : "border-erp-border bg-erp-surface hover:bg-erp-surface-subtle"
                    )}
                  >
                    <div className="h-16 w-full flex items-center justify-center bg-white border border-erp-border/40 p-1 mb-2">
                      <img src={preset.url} alt={preset.name} className="max-h-full max-w-full object-contain" />
                    </div>
                    <div className="flex items-center justify-between text-xs">
                      <span className="font-medium text-erp-text-main truncate">{preset.name}</span>
                      <button
                        type="button"
                        onClick={(e) => {
                          e.stopPropagation();
                          onDeletePreset(preset.id, preset.name);
                        }}
                        className="text-[10px] text-red-600 hover:underline ml-1"
                      >
                        ลบ
                      </button>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      )}
    </div>
  );
}

SignatureManager.displayName = "SignatureManager";
