// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_invoice_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierInvoiceLineRequest extends SupplierInvoiceLineRequest {
  @override
  final String? purchaseOrderLineId;
  @override
  final double? quantity;
  @override
  final double? unitPrice;

  factory _$SupplierInvoiceLineRequest(
          [void Function(SupplierInvoiceLineRequestBuilder)? updates]) =>
      (SupplierInvoiceLineRequestBuilder()..update(updates))._build();

  _$SupplierInvoiceLineRequest._(
      {this.purchaseOrderLineId, this.quantity, this.unitPrice})
      : super._();
  @override
  SupplierInvoiceLineRequest rebuild(
          void Function(SupplierInvoiceLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierInvoiceLineRequestBuilder toBuilder() =>
      SupplierInvoiceLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierInvoiceLineRequest &&
        purchaseOrderLineId == other.purchaseOrderLineId &&
        quantity == other.quantity &&
        unitPrice == other.unitPrice;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, purchaseOrderLineId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierInvoiceLineRequest')
          ..add('purchaseOrderLineId', purchaseOrderLineId)
          ..add('quantity', quantity)
          ..add('unitPrice', unitPrice))
        .toString();
  }
}

class SupplierInvoiceLineRequestBuilder
    implements
        Builder<SupplierInvoiceLineRequest, SupplierInvoiceLineRequestBuilder> {
  _$SupplierInvoiceLineRequest? _$v;

  String? _purchaseOrderLineId;
  String? get purchaseOrderLineId => _$this._purchaseOrderLineId;
  set purchaseOrderLineId(String? purchaseOrderLineId) =>
      _$this._purchaseOrderLineId = purchaseOrderLineId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  double? _unitPrice;
  double? get unitPrice => _$this._unitPrice;
  set unitPrice(double? unitPrice) => _$this._unitPrice = unitPrice;

  SupplierInvoiceLineRequestBuilder() {
    SupplierInvoiceLineRequest._defaults(this);
  }

  SupplierInvoiceLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _purchaseOrderLineId = $v.purchaseOrderLineId;
      _quantity = $v.quantity;
      _unitPrice = $v.unitPrice;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierInvoiceLineRequest other) {
    _$v = other as _$SupplierInvoiceLineRequest;
  }

  @override
  void update(void Function(SupplierInvoiceLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierInvoiceLineRequest build() => _build();

  _$SupplierInvoiceLineRequest _build() {
    final _$result = _$v ??
        _$SupplierInvoiceLineRequest._(
          purchaseOrderLineId: purchaseOrderLineId,
          quantity: quantity,
          unitPrice: unitPrice,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
