// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_return_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierReturnLineResponse extends SupplierReturnLineResponse {
  @override
  final String? id;
  @override
  final String? goodsReceiptLineId;
  @override
  final String? purchaseOrderLineId;
  @override
  final String? itemId;
  @override
  final String? itemCode;
  @override
  final String? itemNameTh;
  @override
  final String? unitCode;
  @override
  final double? quantity;
  @override
  final double? unitPrice;
  @override
  final double? lineReturnAmount;

  factory _$SupplierReturnLineResponse(
          [void Function(SupplierReturnLineResponseBuilder)? updates]) =>
      (SupplierReturnLineResponseBuilder()..update(updates))._build();

  _$SupplierReturnLineResponse._(
      {this.id,
      this.goodsReceiptLineId,
      this.purchaseOrderLineId,
      this.itemId,
      this.itemCode,
      this.itemNameTh,
      this.unitCode,
      this.quantity,
      this.unitPrice,
      this.lineReturnAmount})
      : super._();
  @override
  SupplierReturnLineResponse rebuild(
          void Function(SupplierReturnLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierReturnLineResponseBuilder toBuilder() =>
      SupplierReturnLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierReturnLineResponse &&
        id == other.id &&
        goodsReceiptLineId == other.goodsReceiptLineId &&
        purchaseOrderLineId == other.purchaseOrderLineId &&
        itemId == other.itemId &&
        itemCode == other.itemCode &&
        itemNameTh == other.itemNameTh &&
        unitCode == other.unitCode &&
        quantity == other.quantity &&
        unitPrice == other.unitPrice &&
        lineReturnAmount == other.lineReturnAmount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, goodsReceiptLineId.hashCode);
    _$hash = $jc(_$hash, purchaseOrderLineId.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, itemNameTh.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jc(_$hash, lineReturnAmount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierReturnLineResponse')
          ..add('id', id)
          ..add('goodsReceiptLineId', goodsReceiptLineId)
          ..add('purchaseOrderLineId', purchaseOrderLineId)
          ..add('itemId', itemId)
          ..add('itemCode', itemCode)
          ..add('itemNameTh', itemNameTh)
          ..add('unitCode', unitCode)
          ..add('quantity', quantity)
          ..add('unitPrice', unitPrice)
          ..add('lineReturnAmount', lineReturnAmount))
        .toString();
  }
}

class SupplierReturnLineResponseBuilder
    implements
        Builder<SupplierReturnLineResponse, SupplierReturnLineResponseBuilder> {
  _$SupplierReturnLineResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _goodsReceiptLineId;
  String? get goodsReceiptLineId => _$this._goodsReceiptLineId;
  set goodsReceiptLineId(String? goodsReceiptLineId) =>
      _$this._goodsReceiptLineId = goodsReceiptLineId;

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

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  double? _unitPrice;
  double? get unitPrice => _$this._unitPrice;
  set unitPrice(double? unitPrice) => _$this._unitPrice = unitPrice;

  double? _lineReturnAmount;
  double? get lineReturnAmount => _$this._lineReturnAmount;
  set lineReturnAmount(double? lineReturnAmount) =>
      _$this._lineReturnAmount = lineReturnAmount;

  SupplierReturnLineResponseBuilder() {
    SupplierReturnLineResponse._defaults(this);
  }

  SupplierReturnLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _goodsReceiptLineId = $v.goodsReceiptLineId;
      _purchaseOrderLineId = $v.purchaseOrderLineId;
      _itemId = $v.itemId;
      _itemCode = $v.itemCode;
      _itemNameTh = $v.itemNameTh;
      _unitCode = $v.unitCode;
      _quantity = $v.quantity;
      _unitPrice = $v.unitPrice;
      _lineReturnAmount = $v.lineReturnAmount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierReturnLineResponse other) {
    _$v = other as _$SupplierReturnLineResponse;
  }

  @override
  void update(void Function(SupplierReturnLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierReturnLineResponse build() => _build();

  _$SupplierReturnLineResponse _build() {
    final _$result = _$v ??
        _$SupplierReturnLineResponse._(
          id: id,
          goodsReceiptLineId: goodsReceiptLineId,
          purchaseOrderLineId: purchaseOrderLineId,
          itemId: itemId,
          itemCode: itemCode,
          itemNameTh: itemNameTh,
          unitCode: unitCode,
          quantity: quantity,
          unitPrice: unitPrice,
          lineReturnAmount: lineReturnAmount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
