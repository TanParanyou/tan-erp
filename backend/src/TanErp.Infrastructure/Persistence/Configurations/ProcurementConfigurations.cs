using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Inventory;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Domain.Procurement;
using TanErp.Domain.Projects;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers", "procurement", t =>
        {
            t.HasCheckConstraint("ck_suppliers_status", "status IN ('active', 'inactive')");
            t.HasCheckConstraint("ck_suppliers_payment_term_days", "payment_term_days BETWEEN 0 AND 365");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.NormalizedCode).HasColumnName("normalized_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.NameTh).HasColumnName("name_th").HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameEn).HasColumnName("name_en").HasMaxLength(200);
        builder.Property(x => x.NormalizedName).HasColumnName("normalized_name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.TaxId).HasColumnName("tax_id").HasMaxLength(20);
        builder.Property(x => x.ContactName).HasColumnName("contact_name").HasMaxLength(200);
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(50);
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(200);
        builder.Property(x => x.PaymentTermDays).HasColumnName("payment_term_days").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(16).IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.NormalizedCode }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.NormalizedName });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders", "procurement", t =>
        {
            t.HasCheckConstraint("ck_purchase_orders_status", "status IN ('draft', 'submitted', 'approved', 'rejected', 'partially_received', 'received', 'cancelled')");
            t.HasCheckConstraint("ck_purchase_orders_total_amount", "total_amount >= 0");
            t.HasCheckConstraint("ck_purchase_orders_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> created_by_user_id");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id");
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(x => x.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ExpectedDeliveryDate).HasColumnName("expected_delivery_date");
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.SubmittedAtUtc).HasColumnName("submitted_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecisionNote).HasColumnName("decision_note").HasMaxLength(500);
        builder.Property(x => x.CancelReason).HasColumnName("cancel_reason").HasMaxLength(500);
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.Status });
        builder.HasIndex(x => new { x.OrganizationId, x.SupplierId });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(l => l.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.ToTable("purchase_order_lines", "procurement", t =>
        {
            t.HasCheckConstraint("ck_purchase_order_lines_quantity", "quantity > 0");
            t.HasCheckConstraint("ck_purchase_order_lines_unit_price", "unit_price >= 0");
            t.HasCheckConstraint("ck_purchase_order_lines_received", "received_quantity >= 0 AND received_quantity <= quantity");
            t.HasCheckConstraint("ck_purchase_order_lines_invoiced", "invoiced_quantity >= 0 AND invoiced_quantity <= received_quantity");
        });
        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.RemainingQuantity);
        builder.Ignore(x => x.UninvoicedReceivedQuantity);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.PurchaseOrderId).HasColumnName("purchase_order_id").IsRequired();
        builder.Property(x => x.LineNo).HasColumnName("line_no").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.ItemCode).HasColumnName("item_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ItemNameTh).HasColumnName("item_name_th").HasMaxLength(250).IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.UnitCode).HasColumnName("unit_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.LineTotal).HasColumnName("line_total").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.ReceivedQuantity).HasColumnName("received_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.InvoicedQuantity).HasColumnName("invoiced_quantity").HasPrecision(18, 4).HasDefaultValue(0m).IsRequired();
        builder.HasIndex(x => new { x.PurchaseOrderId, x.LineNo });
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class GoodsReceiptConfiguration : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.ToTable("goods_receipts", "procurement");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.PurchaseOrderId).HasColumnName("purchase_order_id").IsRequired();
        builder.Property(x => x.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id");
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ReceivedAtUtc).HasColumnName("received_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.ReceivedByUserId).HasColumnName("received_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.PurchaseOrderId, x.ReceivedAtUtc });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ReceivedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(l => l.GoodsReceiptId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class GoodsReceiptLineConfiguration : IEntityTypeConfiguration<GoodsReceiptLine>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptLine> builder)
    {
        builder.ToTable("goods_receipt_lines", "procurement", t =>
        {
            t.HasCheckConstraint("ck_goods_receipt_lines_quantity", "quantity > 0");
            t.HasCheckConstraint("ck_goods_receipt_lines_returned", "returned_quantity >= 0 AND returned_quantity <= quantity");
        });
        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.RemainingReturnableQuantity);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.GoodsReceiptId).HasColumnName("goods_receipt_id").IsRequired();
        builder.Property(x => x.PurchaseOrderLineId).HasColumnName("purchase_order_line_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.ReturnedQuantity).HasColumnName("returned_quantity").HasPrecision(18, 4).HasDefaultValue(0m).IsRequired();
        builder.HasIndex(x => x.PurchaseOrderLineId);
        builder.HasOne<PurchaseOrderLine>().WithMany().HasForeignKey(x => x.PurchaseOrderLineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
    {
        builder.ToTable("purchase_requests", "procurement", t =>
        {
            t.HasCheckConstraint("ck_purchase_requests_status", "status IN ('draft', 'submitted', 'approved', 'rejected', 'cancelled', 'converted')");
            t.HasCheckConstraint("ck_purchase_requests_total_amount", "total_estimated_amount >= 0");
            t.HasCheckConstraint("ck_purchase_requests_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> created_by_user_id");
            t.HasCheckConstraint("ck_purchase_requests_converted", "(status = 'converted') = (converted_purchase_order_id IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id");
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(x => x.TotalEstimatedAmount).HasColumnName("total_estimated_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.RequiredByDate).HasColumnName("required_by_date");
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.SubmittedAtUtc).HasColumnName("submitted_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecisionNote).HasColumnName("decision_note").HasMaxLength(500);
        builder.Property(x => x.CancelReason).HasColumnName("cancel_reason").HasMaxLength(500);
        builder.Property(x => x.ConvertedPurchaseOrderId).HasColumnName("converted_purchase_order_id");
        builder.Property(x => x.ConvertedAtUtc).HasColumnName("converted_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.ProjectId, x.Status });
        builder.HasIndex(x => new { x.OrganizationId, x.BranchId, x.Status });
        builder.HasIndex(x => new { x.OrganizationId, x.ConvertedPurchaseOrderId });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.ConvertedPurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(l => l.PurchaseRequestId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class PurchaseRequestLineConfiguration : IEntityTypeConfiguration<PurchaseRequestLine>
{
    public void Configure(EntityTypeBuilder<PurchaseRequestLine> builder)
    {
        builder.ToTable("purchase_request_lines", "procurement", t =>
        {
            t.HasCheckConstraint("ck_purchase_request_lines_quantity", "quantity > 0");
            t.HasCheckConstraint("ck_purchase_request_lines_unit_price", "estimated_unit_price >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.PurchaseRequestId).HasColumnName("purchase_request_id").IsRequired();
        builder.Property(x => x.LineNo).HasColumnName("line_no").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.ItemCode).HasColumnName("item_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ItemNameTh).HasColumnName("item_name_th").HasMaxLength(250).IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.UnitCode).HasColumnName("unit_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.EstimatedUnitPrice).HasColumnName("estimated_unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.EstimatedLineTotal).HasColumnName("estimated_line_total").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.HasIndex(x => new { x.PurchaseRequestId, x.ItemId }).IsUnique();
        builder.HasIndex(x => new { x.PurchaseRequestId, x.LineNo });
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class SupplierReturnConfiguration : IEntityTypeConfiguration<SupplierReturn>
{
    public void Configure(EntityTypeBuilder<SupplierReturn> builder)
    {
        builder.ToTable("supplier_returns", "procurement", t =>
        {
            t.HasCheckConstraint("ck_supplier_returns_total_amount", "total_return_amount >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.PurchaseOrderId).HasColumnName("purchase_order_id").IsRequired();
        builder.Property(x => x.GoodsReceiptId).HasColumnName("goods_receipt_id").IsRequired();
        builder.Property(x => x.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id");
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ReturnedAtUtc).HasColumnName("returned_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(500).IsRequired();
        builder.Property(x => x.TotalReturnAmount).HasColumnName("total_return_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.StockDocumentId).HasColumnName("stock_document_id");
        builder.Property(x => x.StockDocumentNumber).HasColumnName("stock_document_number").HasMaxLength(64);
        builder.Property(x => x.ReturnedByUserId).HasColumnName("returned_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.PurchaseOrderId, ReturnedAtUtc = x.ReturnedAtUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.GoodsReceiptId });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<GoodsReceipt>().WithMany().HasForeignKey(x => x.GoodsReceiptId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<StockDocument>().WithMany().HasForeignKey(x => x.StockDocumentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ReturnedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(l => l.SupplierReturnId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class SupplierReturnLineConfiguration : IEntityTypeConfiguration<SupplierReturnLine>
{
    public void Configure(EntityTypeBuilder<SupplierReturnLine> builder)
    {
        builder.ToTable("supplier_return_lines", "procurement", t =>
        {
            t.HasCheckConstraint("ck_supplier_return_lines_quantity", "quantity > 0");
            t.HasCheckConstraint("ck_supplier_return_lines_unit_price", "unit_price >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.SupplierReturnId).HasColumnName("supplier_return_id").IsRequired();
        builder.Property(x => x.GoodsReceiptLineId).HasColumnName("goods_receipt_line_id").IsRequired();
        builder.Property(x => x.PurchaseOrderLineId).HasColumnName("purchase_order_line_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.LineReturnAmount).HasColumnName("line_return_amount").HasPrecision(18, 2).IsRequired();
        builder.HasIndex(x => x.GoodsReceiptLineId);
        builder.HasIndex(x => x.PurchaseOrderLineId);
        builder.HasOne<GoodsReceiptLine>().WithMany().HasForeignKey(x => x.GoodsReceiptLineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PurchaseOrderLine>().WithMany().HasForeignKey(x => x.PurchaseOrderLineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class SupplierInvoiceConfiguration : IEntityTypeConfiguration<SupplierInvoice>
{
    public void Configure(EntityTypeBuilder<SupplierInvoice> builder)
    {
        builder.ToTable("supplier_invoices", "procurement", t =>
        {
            t.HasCheckConstraint("ck_supplier_invoices_status", "status IN ('draft', 'on_hold', 'matched', 'approved', 'rejected', 'cancelled', 'voided')");
            t.HasCheckConstraint("ck_supplier_invoices_match_status", "match_status IN ('pending', 'matched', 'price_variance')");
            t.HasCheckConstraint("ck_supplier_invoices_amounts", "subtotal_amount >= 0 AND tax_amount >= 0 AND total_amount >= 0 AND total_po_amount >= 0");
            t.HasCheckConstraint("ck_supplier_invoices_decider", "decided_by_user_id IS NULL OR decided_by_user_id <> created_by_user_id");
            t.HasCheckConstraint("ck_supplier_invoices_void", "(status = 'voided') = (void_reason IS NOT NULL)");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.BranchId).HasColumnName("branch_id").IsRequired();
        builder.Property(x => x.PurchaseOrderId).HasColumnName("purchase_order_id").IsRequired();
        builder.Property(x => x.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id");
        builder.Property(x => x.Number).HasColumnName("number").HasMaxLength(64).IsRequired();
        builder.Property(x => x.SupplierInvoiceNumber).HasColumnName("supplier_invoice_number").HasMaxLength(100).IsRequired();
        builder.Property(x => x.NormalizedSupplierInvoiceNumber).HasColumnName("normalized_supplier_invoice_number").HasMaxLength(100).IsRequired();
        builder.Property(x => x.InvoiceDate).HasColumnName("invoice_date").IsRequired();
        builder.Property(x => x.DueDate).HasColumnName("due_date");
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(x => x.SubtotalAmount).HasColumnName("subtotal_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxAmount).HasColumnName("tax_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TotalPoAmount).HasColumnName("total_po_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TotalVarianceAmount).HasColumnName("total_variance_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.MatchStatus).HasColumnName("match_status").HasMaxLength(32).IsRequired();
        builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(500);
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.SubmittedAtUtc).HasColumnName("submitted_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.PostedAtUtc).HasColumnName("posted_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.DecidedAtUtc).HasColumnName("decided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.DecisionNote).HasColumnName("decision_note").HasMaxLength(500);
        builder.Property(x => x.CancelReason).HasColumnName("cancel_reason").HasMaxLength(500);
        builder.Property(x => x.VoidReason).HasColumnName("void_reason").HasMaxLength(500);
        builder.Property(x => x.VoidedByUserId).HasColumnName("voided_by_user_id");
        builder.Property(x => x.VoidedAtUtc).HasColumnName("voided_at_utc").HasColumnType("timestamptz");
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsConcurrencyToken().IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.SupplierId, x.NormalizedSupplierInvoiceNumber })
            .IsUnique()
            .HasFilter("status NOT IN ('cancelled', 'voided', 'rejected')");
        builder.HasIndex(x => new { x.OrganizationId, x.Status, x.CreatedAtUtc });
        builder.HasIndex(x => new { x.OrganizationId, x.PurchaseOrderId });
        builder.HasIndex(x => new { x.OrganizationId, x.SupplierId, x.InvoiceDate });
        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.VoidedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(l => l.SupplierInvoiceId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class SupplierInvoiceLineConfiguration : IEntityTypeConfiguration<SupplierInvoiceLine>
{
    public void Configure(EntityTypeBuilder<SupplierInvoiceLine> builder)
    {
        builder.ToTable("supplier_invoice_lines", "procurement", t =>
        {
            t.HasCheckConstraint("ck_supplier_invoice_lines_quantity", "quantity > 0");
            t.HasCheckConstraint("ck_supplier_invoice_lines_prices", "unit_price >= 0 AND po_unit_price >= 0");
        });
        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.PoLineTotal);
        builder.Ignore(x => x.ExceedsTolerance);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.SupplierInvoiceId).HasColumnName("supplier_invoice_id").IsRequired();
        builder.Property(x => x.LineNo).HasColumnName("line_no").IsRequired();
        builder.Property(x => x.PurchaseOrderLineId).HasColumnName("purchase_order_line_id").IsRequired();
        builder.Property(x => x.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(x => x.ItemCode).HasColumnName("item_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ItemNameTh).HasColumnName("item_name_th").HasMaxLength(250).IsRequired();
        builder.Property(x => x.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(x => x.UnitCode).HasColumnName("unit_code").HasMaxLength(64).IsRequired();
        builder.Property(x => x.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.PoUnitPrice).HasColumnName("po_unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(x => x.LineTotal).HasColumnName("line_total").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.PriceVarianceAmount).HasColumnName("price_variance_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.PriceVariancePercent).HasColumnName("price_variance_percent").HasPrecision(18, 4).IsRequired();
        builder.HasIndex(x => new { x.SupplierInvoiceId, x.PurchaseOrderLineId }).IsUnique();
        builder.HasIndex(x => new { x.SupplierInvoiceId, x.LineNo });
        builder.HasIndex(x => x.PurchaseOrderLineId);
        builder.HasOne<PurchaseOrderLine>().WithMany().HasForeignKey(x => x.PurchaseOrderLineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
    }
}
