/**
 * Master-Detail / Line Items calculation helpers for ERP documents
 * (Quotations, Sales Orders, Purchase Orders, Invoices).
 */

export interface LineItem {
  id: string;
  name: string;
  quantity: number;
  unitPrice: number;
  discount?: number;
  taxRate?: number;
  total?: number;
  [key: string]: unknown;
}

export interface DocumentTotals {
  subtotal: number;
  totalDiscount: number;
  taxableAmount: number;
  taxAmount: number;
  grandTotal: number;
}

/**
 * Calculates a single line item total (Quantity * UnitPrice - LineDiscount).
 */
export function calculateLineItemTotal(
  quantity: number,
  unitPrice: number,
  discount: number = 0
): number {
  const qty = Number(quantity) || 0;
  const price = Number(unitPrice) || 0;
  const disc = Number(discount) || 0;
  const total = qty * price - disc;
  return Math.max(0, Number(total.toFixed(4)));
}

/**
 * Calculates overall document financial totals with standard Thai VAT (default 7%).
 */
export function calculateDocumentTotals(
  items: LineItem[],
  overallDiscount: number = 0,
  taxRatePercent: number = 7
): DocumentTotals {
  const subtotal = items.reduce((acc, item) => {
    const itemTotal =
      item.total !== undefined
        ? item.total
        : calculateLineItemTotal(item.quantity, item.unitPrice, item.discount);
    return acc + itemTotal;
  }, 0);

  const discount = Math.min(subtotal, Math.max(0, Number(overallDiscount) || 0));
  const taxableAmount = subtotal - discount;
  const taxAmount = Number(((taxableAmount * taxRatePercent) / 100).toFixed(2));
  const grandTotal = Number((taxableAmount + taxAmount).toFixed(2));

  return {
    subtotal: Number(subtotal.toFixed(2)),
    totalDiscount: Number(discount.toFixed(2)),
    taxableAmount: Number(taxableAmount.toFixed(2)),
    taxAmount,
    grandTotal,
  };
}
