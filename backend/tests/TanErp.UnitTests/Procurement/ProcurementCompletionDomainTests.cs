using TanErp.Domain.Procurement;
using Xunit;

namespace TanErp.UnitTests.Procurement;

public class ProcurementCompletionDomainTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();
    private static readonly Guid SupplierId = Guid.NewGuid();
    private static readonly Guid CreatorId = Guid.NewGuid();
    private static readonly Guid ApproverId = Guid.NewGuid();
    private static readonly Guid ItemA = Guid.NewGuid();
    private static readonly Guid ItemB = Guid.NewGuid();
    private static readonly Guid UnitId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 10, 29, 10, 0, 0, TimeSpan.Zero);

    private static PurchaseRequest BuildDraftPurchaseRequest()
    {
        var pr = new PurchaseRequest(Guid.NewGuid(), OrgId, BranchId, null, "PR-2026-0001", new DateOnly(2026, 11, 15), "ขอซื้อไม้", CreatorId, Now);
        pr.AddLine(new PurchaseRequestLine(Guid.NewGuid(), OrgId, pr.Id, 1, ItemA, "MAT-001", "ไม้อัด", UnitId, "PCS", 10m, 1250m, "ล็อตแรก"));
        return pr;
    }

    private static (PurchaseOrder Order, PurchaseOrderLine LineA, PurchaseOrderLine LineB) BuildApprovedOrder()
    {
        var order = new PurchaseOrder(Guid.NewGuid(), OrgId, BranchId, SupplierId, null, "PO-2026-0001", new DateOnly(2026, 11, 20), null, CreatorId, Now);
        var lineA = new PurchaseOrderLine(Guid.NewGuid(), OrgId, order.Id, 1, ItemA, "MAT-001", "ไม้อัด", UnitId, "PCS", 10m, 100m);
        var lineB = new PurchaseOrderLine(Guid.NewGuid(), OrgId, order.Id, 2, ItemB, "MAT-002", "ลามิเนต", UnitId, "PCS", 5m, 200m);
        order.AddLine(lineA);
        order.AddLine(lineB);
        order.Submit(Now);
        order.Decide(true, ApproverId, "อนุมัติ", Now);
        return (order, lineA, lineB);
    }

    [Fact]
    public void PurchaseRequest_EnforcesMakerChecker_RejectionReason_AndSingleConversion()
    {
        var pr = BuildDraftPurchaseRequest();
        Assert.Equal(PurchaseRequestStatus.Draft, pr.Status);
        Assert.Equal(12500m, pr.TotalEstimatedAmount);

        var dupEx = Assert.Throws<ProcurementDomainException>(() =>
            pr.AddLine(new PurchaseRequestLine(Guid.NewGuid(), OrgId, pr.Id, 2, ItemA, "MAT-001", "ไม้อัด", UnitId, "PCS", 1m, 100m)));
        Assert.Equal("PURCHASE_REQUEST_LINE_INVALID", dupEx.Code);

        pr.Submit(Now.AddMinutes(5));
        Assert.Equal(PurchaseRequestStatus.Submitted, pr.Status);

        var selfEx = Assert.Throws<ProcurementDomainException>(() => pr.Decide(true, CreatorId, null, Now.AddMinutes(10)));
        Assert.Equal("PURCHASE_REQUEST_SELF_APPROVAL", selfEx.Code);

        var noReasonReject = Assert.Throws<ProcurementDomainException>(() => pr.Decide(false, ApproverId, "  ", Now.AddMinutes(10)));
        Assert.Equal("PROCUREMENT_REASON_REQUIRED", noReasonReject.Code);

        pr.Decide(true, ApproverId, "อนุมัติจัดซื้อ", Now.AddMinutes(15));
        Assert.Equal(PurchaseRequestStatus.Approved, pr.Status);

        var poId = Guid.NewGuid();
        pr.MarkConverted(poId, Now.AddMinutes(20));
        Assert.Equal(PurchaseRequestStatus.Converted, pr.Status);
        Assert.Equal(poId, pr.ConvertedPurchaseOrderId);

        var reconvertEx = Assert.Throws<ProcurementDomainException>(() => pr.MarkConverted(Guid.NewGuid(), Now.AddMinutes(25)));
        Assert.Equal("PURCHASE_REQUEST_INVALID_STATE", reconvertEx.Code);

        var cancelConvertedEx = Assert.Throws<ProcurementDomainException>(() => pr.Cancel("ขอยกเลิก", Now.AddMinutes(30)));
        Assert.Equal("PURCHASE_REQUEST_INVALID_STATE", cancelConvertedEx.Code);
    }

    [Fact]
    public void SupplierReturn_UpdatesGoodsReceiptAndOrderStatus_AndBlocksOverReturnOrAlreadyInvoiced()
    {
        var (order, lineA, lineB) = BuildApprovedOrder();

        var receipt = new GoodsReceipt(Guid.NewGuid(), OrgId, BranchId, order.Id, SupplierId, null, "GR-2026-0001", Now, null, CreatorId, Now);
        var grLineA = new GoodsReceiptLine(Guid.NewGuid(), OrgId, receipt.Id, lineA.Id, ItemA, UnitId, 10m, 100m);
        var grLineB = new GoodsReceiptLine(Guid.NewGuid(), OrgId, receipt.Id, lineB.Id, ItemB, UnitId, 5m, 200m);
        receipt.AddLine(grLineA);
        receipt.AddLine(grLineB);

        order.ApplyReceipt([(lineA.Id, 10m), (lineB.Id, 5m)], Now);
        Assert.Equal(PurchaseOrderStatus.Received, order.Status);

        // Returning 4 of lineA moves PO from received -> partially_received
        receipt.ApplyReturn([(grLineA.Id, 4m)]);
        order.ApplyReturn([(lineA.Id, 4m)], Now.AddMinutes(5));
        Assert.Equal(4m, grLineA.ReturnedQuantity);
        Assert.Equal(6m, grLineA.RemainingReturnableQuantity);
        Assert.Equal(6m, lineA.ReceivedQuantity);
        Assert.Equal(4m, lineA.RemainingQuantity);
        Assert.Equal(PurchaseOrderStatus.PartiallyReceived, order.Status);

        // Over-returning beyond remaining returnable on GR fails
        var overReturnEx = Assert.Throws<ProcurementDomainException>(() => receipt.ApplyReturn([(grLineA.Id, 6.0001m)]));
        Assert.Equal("SUPPLIER_RETURN_OVER_RETURNED", overReturnEx.Code);

        // Reserve 5 invoiced units on lineA; returning 2 (6 - 2 = 4 < 5 invoiced) is blocked
        order.ReserveInvoiced([(lineA.Id, 5m)], Now.AddMinutes(10));
        Assert.Equal(5m, lineA.InvoicedQuantity);
        Assert.Equal(1m, lineA.UninvoicedReceivedQuantity);

        var invoicedGuardEx = Assert.Throws<ProcurementDomainException>(() => order.ApplyReturn([(lineA.Id, 2m)], Now.AddMinutes(12)));
        Assert.Equal("SUPPLIER_RETURN_ALREADY_INVOICED", invoicedGuardEx.Code);

        // Releasing invoiced units allows full return of remaining received goods -> status returns to approved
        order.ReleaseInvoiced([(lineA.Id, 5m)], Now.AddMinutes(15));
        receipt.ApplyReturn([(grLineA.Id, 6m), (grLineB.Id, 5m)]);
        order.ApplyReturn([(lineA.Id, 6m), (lineB.Id, 5m)], Now.AddMinutes(20));
        Assert.Equal(PurchaseOrderStatus.Approved, order.Status);
        Assert.Equal(0m, lineA.ReceivedQuantity);
        Assert.Equal(0m, lineB.ReceivedQuantity);
    }

    [Fact]
    public void PurchaseOrder_ReserveInvoiced_EnforcesZeroPercentOverReceivedTolerance()
    {
        var (order, lineA, _) = BuildApprovedOrder();
        order.ApplyReceipt([(lineA.Id, 6m)], Now);

        var overInvoiced = Assert.Throws<ProcurementDomainException>(() =>
            order.ReserveInvoiced([(lineA.Id, 6.0001m)], Now.AddMinutes(5)));
        Assert.Equal("SUPPLIER_INVOICE_OVER_RECEIVED", overInvoiced.Code);

        order.ReserveInvoiced([(lineA.Id, 4m)], Now.AddMinutes(6));
        Assert.Equal(2m, lineA.UninvoicedReceivedQuantity);

        var secondOver = Assert.Throws<ProcurementDomainException>(() =>
            order.ReserveInvoiced([(lineA.Id, 2.01m)], Now.AddMinutes(7)));
        Assert.Equal("SUPPLIER_INVOICE_OVER_RECEIVED", secondOver.Code);

        order.ReserveInvoiced([(lineA.Id, 2m)], Now.AddMinutes(8));
        Assert.Equal(0m, lineA.UninvoicedReceivedQuantity);
    }

    [Theory]
    [InlineData(100.00, 100.00, false, SupplierInvoiceStatus.Matched, SupplierInvoiceMatchStatus.Matched)]
    [InlineData(100.00, 102.00, false, SupplierInvoiceStatus.Matched, SupplierInvoiceMatchStatus.Matched)]
    [InlineData(100.00, 98.00, false, SupplierInvoiceStatus.Matched, SupplierInvoiceMatchStatus.Matched)]
    [InlineData(100.00, 102.01, true, SupplierInvoiceStatus.OnHold, SupplierInvoiceMatchStatus.PriceVariance)]
    [InlineData(100.00, 97.99, true, SupplierInvoiceStatus.OnHold, SupplierInvoiceMatchStatus.PriceVariance)]
    [InlineData(0.00, 0.00, false, SupplierInvoiceStatus.Matched, SupplierInvoiceMatchStatus.Matched)]
    [InlineData(0.00, 0.01, true, SupplierInvoiceStatus.OnHold, SupplierInvoiceMatchStatus.PriceVariance)]
    public void SupplierInvoice_Submit_EvaluatesTwoPercentPriceVarianceBoundary(
        decimal poUnitPrice,
        decimal invoiceUnitPrice,
        bool expectedExceeds,
        string expectedStatus,
        string expectedMatchStatus)
    {
        var poLineId = Guid.NewGuid();
        var invoice = new SupplierInvoice(
            Guid.NewGuid(), OrgId, BranchId, Guid.NewGuid(), SupplierId, null,
            "SINV-2026-0001", "INV-SUP-001", new DateOnly(2026, 10, 29), new DateOnly(2026, 11, 28), 7m, null, CreatorId, Now);
        var line = new SupplierInvoiceLine(
            Guid.NewGuid(), OrgId, invoice.Id, 1, poLineId, ItemA, "MAT-001", "ไม้อัด", UnitId, "PCS", 10m, poUnitPrice, invoiceUnitPrice);
        invoice.AddLine(line);

        invoice.Submit(new Dictionary<Guid, decimal> { [poLineId] = poUnitPrice }, Now.AddMinutes(5));

        Assert.Equal(expectedExceeds, line.ExceedsTolerance);
        Assert.Equal(expectedStatus, invoice.Status);
        Assert.Equal(expectedMatchStatus, invoice.MatchStatus);
        if (expectedStatus == SupplierInvoiceStatus.Matched)
        {
            Assert.NotNull(invoice.PostedAtUtc);
        }
        else
        {
            Assert.Null(invoice.PostedAtUtc);
        }
    }

    [Fact]
    public void SupplierInvoice_HoldApproval_AndVoidLifecycle_EnforceMakerCheckerAndReasons()
    {
        var poLineId = Guid.NewGuid();
        var invoice = new SupplierInvoice(
            Guid.NewGuid(), OrgId, BranchId, Guid.NewGuid(), SupplierId, null,
            "SINV-2026-0002", "INV-SUP-002", new DateOnly(2026, 10, 29), null, 73.50m, null, CreatorId, Now);
        invoice.AddLine(new SupplierInvoiceLine(
            Guid.NewGuid(), OrgId, invoice.Id, 1, poLineId, ItemA, "MAT-001", "ไม้อัด", UnitId, "PCS", 10m, 100m, 105m));

        invoice.Submit(new Dictionary<Guid, decimal> { [poLineId] = 100m }, Now.AddMinutes(5));
        Assert.Equal(SupplierInvoiceStatus.OnHold, invoice.Status);
        Assert.Equal(50m, invoice.TotalVarianceAmount);
        Assert.Equal(1050m, invoice.SubtotalAmount);
        Assert.Equal(1123.50m, invoice.TotalAmount);

        var selfEx = Assert.Throws<ProcurementDomainException>(() => invoice.DecideHold(true, CreatorId, null, Now.AddMinutes(10)));
        Assert.Equal("SUPPLIER_INVOICE_SELF_APPROVAL", selfEx.Code);

        var rejectNoNote = Assert.Throws<ProcurementDomainException>(() => invoice.DecideHold(false, ApproverId, "", Now.AddMinutes(10)));
        Assert.Equal("PROCUREMENT_REASON_REQUIRED", rejectNoNote.Code);

        invoice.DecideHold(true, ApproverId, "ผู้ขายปรับราคาเหล็กตามสัญญา", Now.AddMinutes(15));
        Assert.Equal(SupplierInvoiceStatus.Approved, invoice.Status);
        Assert.NotNull(invoice.PostedAtUtc);

        var voidNoReason = Assert.Throws<ProcurementDomainException>(() => invoice.Void(" ", CreatorId, Now.AddMinutes(20)));
        Assert.Equal("PROCUREMENT_REASON_REQUIRED", voidNoReason.Code);

        invoice.Void("ผู้ขายออกใบลดหนี้และวางบิลใหม่", CreatorId, Now.AddMinutes(25));
        Assert.Equal(SupplierInvoiceStatus.Voided, invoice.Status);
        Assert.Equal("ผู้ขายออกใบลดหนี้และวางบิลใหม่", invoice.VoidReason);
    }
}
