// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_invoice_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierInvoiceLineResponse extends SupplierInvoiceLineResponse {
  @override
  final String? id;
  @override
  final int? lineNo;
  @override
  final String? purchaseOrderLineId;
  @override
  final String? itemId;
  @override
  final String? itemCode;
  @override
  final String? itemNameTh;
  @override
  final String? unitId;
  @override
  final String? unitCode;
  @override
  final double? quantity;
  @override
  final double? poUnitPrice;
  @override
  final double? unitPrice;
  @override
  final double? lineTotal;
  @override
  final double? priceVarianceAmount;
  @override
  final double? priceVariancePercent;
  @override
  final bool? exceedsTolerance;

  factory _$SupplierInvoiceLineResponse(
          [void Function(SupplierInvoiceLineResponseBuilder)? updates]) =>
      (SupplierInvoiceLineResponseBuilder()..update(updates))._build();

  _$SupplierInvoiceLineResponse._(
      {this.id,
      this.lineNo,
      this.purchaseOrderLineId,
      this.itemId,
      this.itemCode,
      this.itemNameTh,
      this.unitId,
      this.unitCode,
      this.quantity,
      this.poUnitPrice,
      this.unitPrice,
      this.lineTotal,
      this.priceVarianceAmount,
      this.priceVariancePercent,
      this.exceedsTolerance})
      : super._();
  @override
  SupplierInvoiceLineResponse rebuild(
          void Function(SupplierInvoiceLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierInvoiceLineResponseBuilder toBuilder() =>
      SupplierInvoiceLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierInvoiceLineResponse &&
        id == other.id &&
        lineNo == other.lineNo &&
        purchaseOrderLineId == other.purchaseOrderLineId &&
        itemId == other.itemId &&
        itemCode == other.itemCode &&
        itemNameTh == other.itemNameTh &&
        unitId == other.unitId &&
        unitCode == other.unitCode &&
        quantity == other.quantity &&
        poUnitPrice == other.poUnitPrice &&
        unitPrice == other.unitPrice &&
        lineTotal == other.lineTotal &&
        priceVarianceAmount == other.priceVarianceAmount &&
        priceVariancePercent == other.priceVariancePercent &&
        exceedsTolerance == other.exceedsTolerance;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, lineNo.hashCode);
    _$hash = $jc(_$hash, purchaseOrderLineId.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, itemNameTh.hashCode);
    _$hash = $jc(_$hash, unitId.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, poUnitPrice.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jc(_$hash, lineTotal.hashCode);
    _$hash = $jc(_$hash, priceVarianceAmount.hashCode);
    _$hash = $jc(_$hash, priceVariancePercent.hashCode);
    _$hash = $jc(_$hash, exceedsTolerance.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierInvoiceLineResponse')
          ..add('id', id)
          ..add('lineNo', lineNo)
          ..add('purchaseOrderLineId', purchaseOrderLineId)
          ..add('itemId', itemId)
          ..add('itemCode', itemCode)
          ..add('itemNameTh', itemNameTh)
          ..add('unitId', unitId)
          ..add('unitCode', unitCode)
          ..add('quantity', quantity)
          ..add('poUnitPrice', poUnitPrice)
          ..add('unitPrice', unitPrice)
          ..add('lineTotal', lineTotal)
          ..add('priceVarianceAmount', priceVarianceAmount)
          ..add('priceVariancePercent', priceVariancePercent)
          ..add('exceedsTolerance', exceedsTolerance))
        .toString();
  }
}

class SupplierInvoiceLineResponseBuilder
    implements
        Builder<SupplierInvoiceLineResponse,
            SupplierInvoiceLineResponseBuilder> {
  _$SupplierInvoiceLineResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _lineNo;
  int? get lineNo => _$this._lineNo;
  set lineNo(int? lineNo) => _$this._lineNo = lineNo;

  String? _purchaseOrderLineId;
  String? get purchaseOrderLineId => _$this._purchaseOrderLineId;
  set purchaseOrderLineId(String? purchaseOrderLineId) =>
      _$this._purchaseOrderLineId = purchaseOrderLineId;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _itemCode;
  String? get itemCode => _$this._itemCode;
  set itemCode(String? itemCode) => _$this._itemCode = itemCode;

  String? _itemNameTh;
  String? get itemNameTh => _$this._itemNameTh;
  set itemNameTh(String? itemNameTh) => _$this._itemNameTh = itemNameTh;

  String? _unitId;
  String? get unitId => _$this._unitId;
  set unitId(String? unitId) => _$this._unitId = unitId;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  double? _poUnitPrice;
  double? get poUnitPrice => _$this._poUnitPrice;
  set poUnitPrice(double? poUnitPrice) => _$this._poUnitPrice = poUnitPrice;

  double? _unitPrice;
  double? get unitPrice => _$this._unitPrice;
  set unitPrice(double? unitPrice) => _$this._unitPrice = unitPrice;

  double? _lineTotal;
  double? get lineTotal => _$this._lineTotal;
  set lineTotal(double? lineTotal) => _$this._lineTotal = lineTotal;

  double? _priceVarianceAmount;
  double? get priceVarianceAmount => _$this._priceVarianceAmount;
  set priceVarianceAmount(double? priceVarianceAmount) =>
      _$this._priceVarianceAmount = priceVarianceAmount;

  double? _priceVariancePercent;
  double? get priceVariancePercent => _$this._priceVariancePercent;
  set priceVariancePercent(double? priceVariancePercent) =>
      _$this._priceVariancePercent = priceVariancePercent;

  bool? _exceedsTolerance;
  bool? get exceedsTolerance => _$this._exceedsTolerance;
  set exceedsTolerance(bool? exceedsTolerance) =>
      _$this._exceedsTolerance = exceedsTolerance;

  SupplierInvoiceLineResponseBuilder() {
    SupplierInvoiceLineResponse._defaults(this);
  }

  SupplierInvoiceLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _lineNo = $v.lineNo;
      _purchaseOrderLineId = $v.purchaseOrderLineId;
      _itemId = $v.itemId;
      _itemCode = $v.itemCode;
      _itemNameTh = $v.itemNameTh;
      _unitId = $v.unitId;
      _unitCode = $v.unitCode;
      _quantity = $v.quantity;
      _poUnitPrice = $v.poUnitPrice;
      _unitPrice = $v.unitPrice;
      _lineTotal = $v.lineTotal;
      _priceVarianceAmount = $v.priceVarianceAmount;
      _priceVariancePercent = $v.priceVariancePercent;
      _exceedsTolerance = $v.exceedsTolerance;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierInvoiceLineResponse other) {
    _$v = other as _$SupplierInvoiceLineResponse;
  }

  @override
  void update(void Function(SupplierInvoiceLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierInvoiceLineResponse build() => _build();

  _$SupplierInvoiceLineResponse _build() {
    final _$result = _$v ??
        _$SupplierInvoiceLineResponse._(
          id: id,
          lineNo: lineNo,
          purchaseOrderLineId: purchaseOrderLineId,
          itemId: itemId,
          itemCode: itemCode,
          itemNameTh: itemNameTh,
          unitId: unitId,
          unitCode: unitCode,
          quantity: quantity,
          poUnitPrice: poUnitPrice,
          unitPrice: unitPrice,
          lineTotal: lineTotal,
          priceVarianceAmount: priceVarianceAmount,
          priceVariancePercent: priceVariancePercent,
          exceedsTolerance: exceedsTolerance,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
