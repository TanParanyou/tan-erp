"use client";

import React from "react";
import type { DocumentTemplateLayoutConfig } from "../types";
import { cn } from "@/lib/utils/cn";

export interface A4DocumentPreviewProps {
  layout: DocumentTemplateLayoutConfig;
  logoPreviewUrl?: string | null;
  className?: string;
  isPrintMode?: boolean;
}

export function A4DocumentPreview({
  layout,
  logoPreviewUrl,
  className,
  isPrintMode = false,
}: A4DocumentPreviewProps) {
  const { header, columns, summary, terms, signatures } = layout;

  const sampleItems = [
    {
      no: 1,
      sku: "WIN-ALU-001",
      name: "ชุดหน้าต่างบานเลื่อนอลูมิเนียม พร้อมมุ้งลวด",
      specs: "กรอบอลูมิเนียมสีดำ Powder Coat หนา 1.5 มม. กระจกตัดแสงสีเขียว 6 มม.",
      qty: 4,
      unit: "ชุด",
      unitPrice: 4500,
      discount: 400,
      total: 17600,
    },
    {
      no: 2,
      sku: "DOOR-SW-002",
      name: "ประตูบานเปิดเดี่ยวอลูมิเนียม ลายไม้สัก",
      specs: "อุปกรณ์โช้คฝังพื้น Dorma กุญแจมอร์ทิสล็อก Hafele ชนิดความปลอดภัยสูง",
      qty: 2,
      unit: "บาน",
      unitPrice: 8500,
      discount: 0,
      total: 17000,
    },
    {
      no: 3,
      sku: "INST-SRV-001",
      name: "ค่าบริการติดตั้งและซีลกันน้ำภายนอก",
      specs: "ยาแนวด้วย PU Sealant ทนแดดฝน รับประกันงานติดตั้ง 1 ปี",
      qty: 1,
      unit: "งาน",
      unitPrice: 3500,
      discount: 0,
      total: 3500,
    },
  ];

  const subtotal = 38100;
  const totalDiscount = columns.showLineDiscount ? 400 : 0;
  const netBeforeTax = subtotal;
  const vatAmount = summary.showVat ? netBeforeTax * 0.07 : 0;
  const grandTotal = netBeforeTax + vatAmount;

  return (
    <div
      className={cn(
        "a4-print-sheet relative bg-white text-slate-800 font-sans shadow-xl border border-slate-300 select-none",
        "w-full max-w-[794px] min-h-[1100px] p-8 sm:p-10 text-[11px] leading-relaxed flex flex-col justify-between",
        "print:w-full print:max-w-none print:min-h-0 print:p-0 print:border-none print:shadow-none print:text-[10pt]",
        isPrintMode && "border-none shadow-none p-0",
        className
      )}
    >
      {/* Upper Content Section */}
      <div className="space-y-4">
        {/* Header Section */}
        <div
          className={cn(
            "flex items-start justify-between border-b-2 border-[#0B3056] pb-4 gap-4",
            header.logoPosition === "right" && "flex-row-reverse"
          )}
        >
          {/* Logo */}
          {header.showLogo && (
            <div className="flex-shrink-0 flex items-center justify-center">
              {logoPreviewUrl ? (
                // eslint-disable-next-line @next/next/no-img-element
                <img
                  src={logoPreviewUrl}
                  alt="Company Logo"
                  style={{ maxHeight: header.logoHeight || "48px" }}
                  className="object-contain"
                />
              ) : (
                <div
                  style={{ height: header.logoHeight || "48px" }}
                  className="w-24 bg-slate-100 border border-dashed border-slate-300 flex items-center justify-center text-[10px] text-slate-400 font-medium"
                >
                  LOGO
                </div>
              )}
            </div>
          )}

          {/* Company Details */}
          <div className="flex-1 text-left">
            <div className="font-bold text-sm sm:text-base text-[#0B3056] tracking-tight">
              {header.companyNameTh || "บริษัท แทน คอนสตรัคชั่น แอนด์ ดีไซน์ จำกัด"}
            </div>
            {header.companyNameEn && (
              <div className="text-[10px] text-slate-500 font-medium">
                {header.companyNameEn}
              </div>
            )}
            <div className="text-[10px] text-slate-600 mt-1 leading-snug">
              {header.address || "123/45 ถนนสุขุมวิท แขวงคลองเตย เขตคลองเตย กรุงเทพมหานคร 10110"}
            </div>
            <div className="text-[10px] text-slate-600 flex flex-wrap gap-x-3 mt-1">
              <span>เลขประจำตัวผู้เสียภาษี: {header.taxId || "0105560000000"}</span>
              {header.phone && <span>โทร: {header.phone}</span>}
              {header.email && <span>อีเมล: {header.email}</span>}
            </div>
          </div>

          {/* Document Title & Number */}
          <div className="text-right flex-shrink-0 min-w-[150px]">
            <h1 className="text-base sm:text-lg font-black text-[#0B3056] uppercase tracking-wider">
              {header.documentTitle || "ใบเสนอราคา"}
            </h1>
            <div className="mt-1 text-[10px] space-y-0.5 text-slate-600">
              <div className="flex justify-between gap-2">
                <span className="text-slate-400">เลขที่:</span>
                <span className="font-mono font-semibold text-slate-900">QT-202610-0012</span>
              </div>
              <div className="flex justify-between gap-2">
                <span className="text-slate-400">วันที่:</span>
                <span className="font-medium">24/10/2026</span>
              </div>
              <div className="flex justify-between gap-2">
                <span className="text-slate-400">ครบกำหนด:</span>
                <span className="font-medium">23/11/2026</span>
              </div>
            </div>
          </div>
        </div>

        {/* Customer Info Card */}
        <div className="grid grid-cols-2 gap-4 border border-slate-200 bg-slate-50/60 p-3 text-[10px]">
          <div>
            <div className="font-bold text-[#0B3056] mb-1">ข้อมูลลูกค้า / Customer Details</div>
            <div className="font-semibold text-slate-900">บริษัท นวัตกรรมก่อสร้าง จำกัด (มหาชน)</div>
            <div className="text-slate-600 mt-0.5">88 อาคารเพรสทีจทาวเวอร์ ชั้น 12 ถนนสาทรใต้ กรุงเทพฯ 10120</div>
            <div className="text-slate-500 mt-0.5">เลขประจำตัวผู้เสียภาษี: 0107558000123 (สำนักงานใหญ่)</div>
          </div>
          <div>
            <div className="font-bold text-[#0B3056] mb-1">สถานที่ติดตั้ง / Project Site</div>
            <div className="font-medium text-slate-800">โครงการ ออริจิ้น เรสซิเดนซ์ สุขุมวิท 42</div>
            <div className="text-slate-600 mt-0.5">ผู้ติดต่อ: คุณสมชาย สายชล (081-234-5678)</div>
            <div className="text-slate-500 mt-0.5">พนักงานขาย: นายพิชิต ชัยชนะ (ฝ่ายขายโครงการ)</div>
          </div>
        </div>

        {/* Line Items Table */}
        <div className="border border-slate-200 overflow-hidden">
          <table className="w-full border-collapse text-[10px]">
            <thead>
              <tr className="bg-[#0B3056] text-white font-semibold">
                <th className="py-1.5 px-2 text-center w-8">#</th>
                {columns.showThumbnail && <th className="py-1.5 px-1.5 text-center w-12">รูป</th>}
                {columns.showSku && <th className="py-1.5 px-2 text-left w-24">รหัสสินค้า</th>}
                <th className="py-1.5 px-2 text-left">รายการ / รายละเอียด</th>
                <th className="py-1.5 px-2 text-right w-12">จำนวน</th>
                <th className="py-1.5 px-1.5 text-center w-12">หน่วย</th>
                <th className="py-1.5 px-2 text-right w-18">ราคา/หน่วย</th>
                {columns.showLineDiscount && <th className="py-1.5 px-2 text-right w-16">ส่วนลด</th>}
                <th className="py-1.5 px-2 text-right w-20">จำนวนเงิน</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-200 text-slate-700">
              {sampleItems.map((item) => (
                <tr key={item.no} className="hover:bg-slate-50/50">
                  <td className="py-2 px-2 text-center font-mono text-slate-400">{item.no}</td>
                  {columns.showThumbnail && (
                    <td className="py-2 px-1.5 text-center">
                      <div className="w-8 h-8 bg-slate-100 border border-slate-200 mx-auto flex items-center justify-center text-[8px] text-slate-400">
                        IMG
                      </div>
                    </td>
                  )}
                  {columns.showSku && (
                    <td className="py-2 px-2 font-mono text-[9.5px] text-slate-600 font-medium">
                      {item.sku}
                    </td>
                  )}
                  <td className="py-2 px-2">
                    <div className="font-semibold text-slate-900">{item.name}</div>
                    {columns.showSpecs && (
                      <div className="text-[9px] text-slate-500 mt-0.5 leading-tight">{item.specs}</div>
                    )}
                  </td>
                  <td className="py-2 px-2 text-right font-mono font-medium">{item.qty}</td>
                  <td className="py-2 px-1.5 text-center text-slate-600">{item.unit}</td>
                  <td className="py-2 px-2 text-right font-mono">
                    {item.unitPrice.toLocaleString("th-TH", { minimumFractionDigits: 2 })}
                  </td>
                  {columns.showLineDiscount && (
                    <td className="py-2 px-2 text-right font-mono text-red-600">
                      {item.discount > 0 ? `-${item.discount.toLocaleString("th-TH", { minimumFractionDigits: 2 })}` : "-"}
                    </td>
                  )}
                  <td className="py-2 px-2 text-right font-mono font-semibold text-slate-900">
                    {item.total.toLocaleString("th-TH", { minimumFractionDigits: 2 })}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Summary & Terms Section */}
        <div className="grid grid-cols-12 gap-4 pt-2">
          {/* Left: Terms and Bank Info */}
          <div className="col-span-7 space-y-2 text-[9.5px]">
            {terms.validityPeriodText && (
              <div className="flex items-start gap-1.5 text-slate-600">
                <span className="font-semibold text-slate-800 flex-shrink-0">กำหนดยืนราคา:</span>
                <span>{terms.validityPeriodText}</span>
              </div>
            )}
            {terms.commercialTermsText && (
              <div className="flex items-start gap-1.5 text-slate-600">
                <span className="font-semibold text-slate-800 flex-shrink-0">เงื่อนไขการชำระ:</span>
                <span className="whitespace-pre-line">{terms.commercialTermsText}</span>
              </div>
            )}
            {terms.showBankDetails && (
              <div className="border border-slate-200 bg-slate-50 p-2.5 text-slate-700">
                <div className="font-bold text-[#0B3056] text-[10px] mb-1">การโอนชำระเงินผ่านบัญชีธนาคาร</div>
                <div className="space-y-0.5">
                  <div>ธนาคาร: <span className="font-semibold">{terms.bankName || "กสิกรไทย (KBANK)"}</span></div>
                  <div>เลขที่บัญชี: <span className="font-mono font-bold text-slate-900">{terms.accountNo || "123-2-34567-8"}</span></div>
                  <div>ชื่อบัญชี: <span className="font-medium">{terms.accountName || "บจก. แทน คอนสตรัคชั่น แอนด์ ดีไซน์"}</span></div>
                </div>
              </div>
            )}
            {terms.footerNote && (
              <div className="text-slate-500 italic text-[9px] pt-1 leading-snug">
                * {terms.footerNote}
              </div>
            )}
          </div>

          {/* Right: Financial Totals */}
          <div className="col-span-5 border border-slate-200 divide-y divide-slate-200 text-[10px] self-start">
            <div className="flex justify-between py-1.5 px-2.5 text-slate-600">
              <span>รวมเป็นเงิน (Subtotal)</span>
              <span className="font-mono">{subtotal.toLocaleString("th-TH", { minimumFractionDigits: 2 })}</span>
            </div>
            {columns.showLineDiscount && (
              <div className="flex justify-between py-1.5 px-2.5 text-red-600">
                <span>ส่วนลดรวม (Discount)</span>
                <span className="font-mono">-{totalDiscount.toLocaleString("th-TH", { minimumFractionDigits: 2 })}</span>
              </div>
            )}
            {summary.showVat && (
              <div className="flex justify-between py-1.5 px-2.5 text-slate-600">
                <span>ภาษีมูลค่าเพิ่ม (VAT 7%)</span>
                <span className="font-mono">{vatAmount.toLocaleString("th-TH", { minimumFractionDigits: 2 })}</span>
              </div>
            )}
            {summary.showWithholdingTax && (
              <div className="flex justify-between py-1.5 px-2.5 text-slate-500">
                <span>หัก ณ ที่จ่าย 3% (WHT)</span>
                <span className="font-mono">-1,143.00</span>
              </div>
            )}
            <div className="flex justify-between py-2 px-2.5 bg-[#0B3056] text-white font-bold text-xs">
              <span>จำนวนเงินรวมทั้งสิ้น</span>
              <span className="font-mono">{grandTotal.toLocaleString("th-TH", { minimumFractionDigits: 2 })} ฿</span>
            </div>
          </div>
        </div>

        {/* Baht Text Display */}
        {summary.showBahtText && (
          <div className="bg-slate-100 border border-slate-200 px-3 py-1.5 text-center font-medium text-[10px] text-slate-700">
            ( สี่หมื่นเจ็ดร้อยหกสิบเจ็ดบาทถ้วน )
          </div>
        )}
      </div>

      {/* Signatures Section (Anchored cleanly at bottom) */}
      <div className="pt-8 mt-6 border-t border-slate-200">
        <div
          className={cn(
            "grid gap-4 text-center text-[10px]",
            signatures.signatureCount === 2 ? "grid-cols-2" : "grid-cols-3"
          )}
        >
          {/* Box 1 */}
          <div className="border border-slate-200 p-2.5 space-y-6 bg-slate-50/40">
            <div className="font-semibold text-slate-800">{signatures.box1Label || "ผู้สั่งซื้อ / ลูกค้า"}</div>
            <div className="border-b border-dashed border-slate-400 mx-4 h-6"></div>
            <div className="text-slate-500 text-[9px] space-y-0.5">
              <div>( ............................................................ )</div>
              <div>วันที่ ...... / ...... / ............</div>
            </div>
          </div>

          {/* Box 2 */}
          <div className="border border-slate-200 p-2.5 space-y-6 bg-slate-50/40">
            <div className="font-semibold text-slate-800">{signatures.box2Label || "พนักงานขาย / ผู้เสนอราคา"}</div>
            <div className="border-b border-dashed border-slate-400 mx-4 h-6"></div>
            <div className="text-slate-500 text-[9px] space-y-0.5">
              <div>( ............................................................ )</div>
              <div>วันที่ ...... / ...... / ............</div>
            </div>
          </div>

          {/* Box 3 (Optional) */}
          {signatures.signatureCount >= 3 && (
            <div className="border border-slate-200 p-2.5 space-y-6 bg-slate-50/40">
              <div className="font-semibold text-slate-800">{signatures.box3Label || "ผู้อนุมัติ / ผู้มีอำนาจลงนาม"}</div>
              <div className="border-b border-dashed border-slate-400 mx-4 h-6"></div>
              <div className="text-slate-500 text-[9px] space-y-0.5">
                <div>( ............................................................ )</div>
                <div>วันที่ ...... / ...... / ............</div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
