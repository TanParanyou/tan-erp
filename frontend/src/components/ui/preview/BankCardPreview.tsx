"use client";

import React from "react";
import { CopyButton } from "@/components/common/CopyButton";
import { cn } from "@/lib/utils/cn";

export interface BankCardPreviewProps {
  bankName: string;
  accountName: string;
  accountNumber: string;
  branch?: string;
  swiftCode?: string;
  className?: string;
}

export function BankCardPreview({
  bankName,
  accountName,
  accountNumber,
  branch,
  swiftCode,
  className,
}: BankCardPreviewProps) {
  return (
    <div
      className={cn(
        "relative overflow-hidden border border-erp-border bg-gradient-to-br from-erp-navy-900 to-erp-navy-950 p-5 text-white shadow-md rounded-none text-left",
        className
      )}
    >
      <div className="flex items-center justify-between border-b border-white/10 pb-3 mb-4">
        <span className="text-xs font-semibold tracking-wider uppercase text-white/80">
          บัญชีธนาคารสำหรับชำระเงิน
        </span>
        <span className="font-bold text-sm tracking-wide text-white">{bankName}</span>
      </div>

      <div className="space-y-3">
        <div>
          <span className="text-[10px] text-white/60 uppercase block">เลขที่บัญชี</span>
          <div className="flex items-center gap-2 mt-0.5">
            <span className="font-mono text-lg font-bold tracking-wider">
              {accountNumber}
            </span>
            <CopyButton text={accountNumber} className="text-white/80 hover:text-white" />
          </div>
        </div>

        <div>
          <span className="text-[10px] text-white/60 uppercase block">ชื่อบัญชี</span>
          <span className="text-xs font-medium text-white/90">{accountName}</span>
        </div>

        {(branch || swiftCode) && (
          <div className="flex gap-4 pt-1 text-[11px] text-white/70 border-t border-white/10">
            {branch && <span>สาขา: {branch}</span>}
            {swiftCode && <span>SWIFT: {swiftCode}</span>}
          </div>
        )}
      </div>
    </div>
  );
}

BankCardPreview.displayName = "BankCardPreview";
