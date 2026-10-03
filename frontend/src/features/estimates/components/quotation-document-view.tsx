import React from "react";
import type { QuotationDocumentResponse } from "@/lib/api/api-client";
import { formatDate } from "@/lib/formatters/formatters";
import { formatCurrencyAmount, formatFinancialNumber } from "../utils/estimate-formatters";

export type QuotationDocumentLabel = (key: string) => string;

interface QuotationDocumentViewProps {
  document: QuotationDocumentResponse;
  documentLocale: "th" | "en";
  label: QuotationDocumentLabel;
}

const EMPTY = "-";

function textOrEmpty(value: string | null | undefined): string {
  return value && value.trim().length > 0 ? value : EMPTY;
}

export function QuotationDocumentView({ document, documentLocale, label }: QuotationDocumentViewProps) {
  // The currency comes from the issued revision; never assume one when it is missing.
  const currency = document.currency ?? null;
  const customer = document.customer;
  const address = customer?.address;
  const addressLines = address
    ? [
        address.addressLine1,
        address.subdistrict,
        address.district,
        address.province,
        address.postalCode,
      ].filter((part): part is string => Boolean(part && part.trim().length > 0))
    : [];
  const sections = document.sections ?? [];
  const totals = document.totals;

  return (
    // A quotation is a paper document: it stays white with black text in dark mode and on print.
    <article
      data-testid="quotation-document"
      className="mx-auto w-full max-w-[210mm] bg-white p-8 text-sm text-black shadow-sm print:max-w-none print:p-0 print:shadow-none"
    >
      <header className="flex flex-wrap items-start justify-between gap-4 border-b-2 border-erp-navy pb-4">
        <h1 className="text-2xl font-bold text-erp-navy">{label("title")}</h1>
        <dl className="grid grid-cols-[auto_auto] gap-x-4 gap-y-1 text-right">
          <dt className="font-semibold">{label("number")}</dt>
          <dd className="font-mono">{textOrEmpty(document.number)}</dd>
          <dt className="font-semibold">{label("issuedDate")}</dt>
          <dd>{formatDate(document.issuedAtUtc, documentLocale)}</dd>
          <dt className="font-semibold">{label("currency")}</dt>
          <dd>{textOrEmpty(currency)}</dd>
        </dl>
      </header>

      <section className="mt-6 border border-erp-border p-4" aria-label={label("customer")}>
        <h2 className="mb-2 text-base font-bold text-erp-navy">{label("customer")}</h2>
        <dl className="grid grid-cols-[auto_1fr] gap-x-4 gap-y-1">
          <dt className="font-semibold">{label("customerName")}</dt>
          <dd>{textOrEmpty(customer?.displayName)}</dd>
          <dt className="font-semibold">{label("legalName")}</dt>
          <dd>{textOrEmpty(customer?.legalName)}</dd>
          <dt className="font-semibold">{label("taxId")}</dt>
          <dd className="font-mono">{textOrEmpty(customer?.taxIdentifier)}</dd>
          <dt className="font-semibold">{label("branchCode")}</dt>
          <dd className="font-mono">{textOrEmpty(customer?.branchCode)}</dd>
          <dt className="font-semibold">{label("address")}</dt>
          <dd>{addressLines.length > 0 ? addressLines.join(" ") : EMPTY}</dd>
        </dl>
      </section>

      <section className="mt-6 overflow-x-auto print:overflow-visible" aria-label={label("items")}>
        <table className="w-full min-w-[34rem] border-collapse border border-erp-border print:min-w-0">
          <thead className="bg-erp-surface-subtle">
            <tr>
              <th scope="col" className="border border-erp-border px-2 py-1 text-left">{label("itemCode")}</th>
              <th scope="col" className="border border-erp-border px-2 py-1 text-left">{label("description")}</th>
              <th scope="col" className="border border-erp-border px-2 py-1 text-right">{label("quantity")}</th>
              <th scope="col" className="border border-erp-border px-2 py-1 text-left">{label("unit")}</th>
              <th scope="col" className="border border-erp-border px-2 py-1 text-right">{label("unitPrice")}</th>
              <th scope="col" className="border border-erp-border px-2 py-1 text-right">{label("lineTotal")}</th>
            </tr>
          </thead>
          {sections.map((section) => (
            <tbody key={section.code} className="break-inside-avoid-page">
              <tr className="bg-erp-surface-subtle">
                <th scope="colgroup" colSpan={5} className="border border-erp-border px-2 py-1 text-left">
                  <span className="font-mono">{textOrEmpty(section.code)}</span>
                  {" "}
                  {textOrEmpty(section.name)}
                </th>
                <td className="border border-erp-border px-2 py-1 text-right font-semibold">
                  {formatFinancialNumber(section.subtotal)}
                </td>
              </tr>
              {(section.workItems ?? []).map((item) => (
                <tr key={item.code} className="break-inside-avoid">
                  <td className="border border-erp-border px-2 py-1 align-top font-mono">{textOrEmpty(item.code)}</td>
                  <td className="border border-erp-border px-2 py-1 align-top whitespace-pre-wrap break-words">
                    {textOrEmpty(item.description)}
                  </td>
                  <td className="border border-erp-border px-2 py-1 align-top text-right">
                    {formatFinancialNumber(item.quantity, 0, 4)}
                  </td>
                  <td className="border border-erp-border px-2 py-1 align-top">{textOrEmpty(item.unitCode)}</td>
                  <td className="border border-erp-border px-2 py-1 align-top text-right">
                    {formatFinancialNumber(item.unitPrice)}
                  </td>
                  <td className="border border-erp-border px-2 py-1 align-top text-right">
                    {formatFinancialNumber(item.lineTotal)}
                  </td>
                </tr>
              ))}
            </tbody>
          ))}
        </table>
      </section>

      <section className="mt-6 ml-auto w-full max-w-xs break-inside-avoid" aria-label={label("totals")}>
        <dl className="grid grid-cols-[1fr_auto] gap-x-4 gap-y-1">
          <dt>{label("subtotal")}</dt>
          <dd className="text-right">{formatFinancialNumber(totals?.subtotal)}</dd>
          <dt>{label("discount")}</dt>
          <dd className="text-right">{formatFinancialNumber(totals?.discountAmount)}</dd>
          <dt>{label("netBeforeTax")}</dt>
          <dd className="text-right">{formatFinancialNumber(totals?.netBeforeTax)}</dd>
          <dt>{label("tax")}</dt>
          <dd className="text-right">{formatFinancialNumber(totals?.taxAmount)}</dd>
          <dt className="border-t-2 border-erp-navy pt-1 text-base font-bold">{label("grandTotal")}</dt>
          <dd className="border-t-2 border-erp-navy pt-1 text-right text-base font-bold">
            {currency
              ? formatCurrencyAmount(totals?.grandTotal, currency, documentLocale)
              : formatFinancialNumber(totals?.grandTotal)}
          </dd>
        </dl>
      </section>
    </article>
  );
}
