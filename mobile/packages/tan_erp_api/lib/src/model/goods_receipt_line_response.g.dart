// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'goods_receipt_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$GoodsReceiptLineResponse extends GoodsReceiptLineResponse {
  @override
  final String? id;
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
  final double? returnedQuantity;
  @override
  final double? remainingReturnableQuantity;

  factory _$GoodsReceiptLineResponse(
          [void Function(GoodsReceiptLineResponseBuilder)? updates]) =>
      (GoodsReceiptLineResponseBuilder()..update(updates))._build();

  _$GoodsReceiptLineResponse._(
      {this.id,
      this.purchaseOrderLineId,
      this.itemId,
      this.itemCode,
      this.itemNameTh,
      this.unitCode,
      this.quantity,
      this.unitPrice,
      this.returnedQuantity,
      this.remainingReturnableQuantity})
      : super._();
  @override
  GoodsReceiptLineResponse rebuild(
          void Function(GoodsReceiptLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  GoodsReceiptLineResponseBuilder toBuilder() =>
      GoodsReceiptLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is GoodsReceiptLineResponse &&
        id == other.id &&
        purchaseOrderLineId == other.purchaseOrderLineId &&
        itemId == other.itemId &&
        itemCode == other.itemCode &&
        itemNameTh == other.itemNameTh &&
        unitCode == other.unitCode &&
        quantity == other.quantity &&
        unitPrice == other.unitPrice &&
        returnedQuantity == other.returnedQuantity &&
        remainingReturnableQuantity == other.remainingReturnableQuantity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, purchaseOrderLineId.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, itemNameTh.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jc(_$hash, returnedQuantity.hashCode);
    _$hash = $jc(_$hash, remainingReturnableQuantity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'GoodsReceiptLineResponse')
          ..add('id', id)
          ..add('purchaseOrderLineId', purchaseOrderLineId)
          ..add('itemId', itemId)
          ..add('itemCode', itemCode)
          ..add('itemNameTh', itemNameTh)
          ..add('unitCode', unitCode)
          ..add('quantity', quantity)
          ..add('unitPrice', unitPrice)
          ..add('returnedQuantity', returnedQuantity)
          ..add('remainingReturnableQuantity', remainingReturnableQuantity))
        .toString();
  }
}

class GoodsReceiptLineResponseBuilder
    implements
        Builder<GoodsReceiptLineResponse, GoodsReceiptLineResponseBuilder> {
  _$GoodsReceiptLineResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  double? _returnedQuantity;
  double? get returnedQuantity => _$this._returnedQuantity;
  set returnedQuantity(double? returnedQuantity) =>
      _$this._returnedQuantity = returnedQuantity;

  double? _remainingReturnableQuantity;
  double? get remainingReturnableQuantity =>
      _$this._remainingReturnableQuantity;
  set remainingReturnableQuantity(double? remainingReturnableQuantity) =>
      _$this._remainingReturnableQuantity = remainingReturnableQuantity;

  GoodsReceiptLineResponseBuilder() {
    GoodsReceiptLineResponse._defaults(this);
  }

  GoodsReceiptLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _purchaseOrderLineId = $v.purchaseOrderLineId;
      _itemId = $v.itemId;
      _itemCode = $v.itemCode;
      _itemNameTh = $v.itemNameTh;
      _unitCode = $v.unitCode;
      _quantity = $v.quantity;
      _unitPrice = $v.unitPrice;
      _returnedQuantity = $v.returnedQuantity;
      _remainingReturnableQuantity = $v.remainingReturnableQuantity;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(GoodsReceiptLineResponse other) {
    _$v = other as _$GoodsReceiptLineResponse;
  }

  @override
  void update(void Function(GoodsReceiptLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  GoodsReceiptLineResponse build() => _build();

  _$GoodsReceiptLineResponse _build() {
    final _$result = _$v ??
        _$GoodsReceiptLineResponse._(
          id: id,
          purchaseOrderLineId: purchaseOrderLineId,
          itemId: itemId,
          itemCode: itemCode,
          itemNameTh: itemNameTh,
          unitCode: unitCode,
          quantity: quantity,
          unitPrice: unitPrice,
          returnedQuantity: returnedQuantity,
          remainingReturnableQuantity: remainingReturnableQuantity,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
