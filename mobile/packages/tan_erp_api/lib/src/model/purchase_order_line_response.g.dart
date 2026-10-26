// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderLineResponse extends PurchaseOrderLineResponse {
  @override
  final String? id;
  @override
  final int? lineNo;
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
  final double? unitPrice;
  @override
  final double? lineTotal;
  @override
  final double? receivedQuantity;
  @override
  final double? remainingQuantity;

  factory _$PurchaseOrderLineResponse(
          [void Function(PurchaseOrderLineResponseBuilder)? updates]) =>
      (PurchaseOrderLineResponseBuilder()..update(updates))._build();

  _$PurchaseOrderLineResponse._(
      {this.id,
      this.lineNo,
      this.itemId,
      this.itemCode,
      this.itemNameTh,
      this.unitId,
      this.unitCode,
      this.quantity,
      this.unitPrice,
      this.lineTotal,
      this.receivedQuantity,
      this.remainingQuantity})
      : super._();
  @override
  PurchaseOrderLineResponse rebuild(
          void Function(PurchaseOrderLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderLineResponseBuilder toBuilder() =>
      PurchaseOrderLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderLineResponse &&
        id == other.id &&
        lineNo == other.lineNo &&
        itemId == other.itemId &&
        itemCode == other.itemCode &&
        itemNameTh == other.itemNameTh &&
        unitId == other.unitId &&
        unitCode == other.unitCode &&
        quantity == other.quantity &&
        unitPrice == other.unitPrice &&
        lineTotal == other.lineTotal &&
        receivedQuantity == other.receivedQuantity &&
        remainingQuantity == other.remainingQuantity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, lineNo.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, itemNameTh.hashCode);
    _$hash = $jc(_$hash, unitId.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jc(_$hash, lineTotal.hashCode);
    _$hash = $jc(_$hash, receivedQuantity.hashCode);
    _$hash = $jc(_$hash, remainingQuantity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseOrderLineResponse')
          ..add('id', id)
          ..add('lineNo', lineNo)
          ..add('itemId', itemId)
          ..add('itemCode', itemCode)
          ..add('itemNameTh', itemNameTh)
          ..add('unitId', unitId)
          ..add('unitCode', unitCode)
          ..add('quantity', quantity)
          ..add('unitPrice', unitPrice)
          ..add('lineTotal', lineTotal)
          ..add('receivedQuantity', receivedQuantity)
          ..add('remainingQuantity', remainingQuantity))
        .toString();
  }
}

class PurchaseOrderLineResponseBuilder
    implements
        Builder<PurchaseOrderLineResponse, PurchaseOrderLineResponseBuilder> {
  _$PurchaseOrderLineResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _lineNo;
  int? get lineNo => _$this._lineNo;
  set lineNo(int? lineNo) => _$this._lineNo = lineNo;

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

  double? _unitPrice;
  double? get unitPrice => _$this._unitPrice;
  set unitPrice(double? unitPrice) => _$this._unitPrice = unitPrice;

  double? _lineTotal;
  double? get lineTotal => _$this._lineTotal;
  set lineTotal(double? lineTotal) => _$this._lineTotal = lineTotal;

  double? _receivedQuantity;
  double? get receivedQuantity => _$this._receivedQuantity;
  set receivedQuantity(double? receivedQuantity) =>
      _$this._receivedQuantity = receivedQuantity;

  double? _remainingQuantity;
  double? get remainingQuantity => _$this._remainingQuantity;
  set remainingQuantity(double? remainingQuantity) =>
      _$this._remainingQuantity = remainingQuantity;

  PurchaseOrderLineResponseBuilder() {
    PurchaseOrderLineResponse._defaults(this);
  }

  PurchaseOrderLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _lineNo = $v.lineNo;
      _itemId = $v.itemId;
      _itemCode = $v.itemCode;
      _itemNameTh = $v.itemNameTh;
      _unitId = $v.unitId;
      _unitCode = $v.unitCode;
      _quantity = $v.quantity;
      _unitPrice = $v.unitPrice;
      _lineTotal = $v.lineTotal;
      _receivedQuantity = $v.receivedQuantity;
      _remainingQuantity = $v.remainingQuantity;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderLineResponse other) {
    _$v = other as _$PurchaseOrderLineResponse;
  }

  @override
  void update(void Function(PurchaseOrderLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderLineResponse build() => _build();

  _$PurchaseOrderLineResponse _build() {
    final _$result = _$v ??
        _$PurchaseOrderLineResponse._(
          id: id,
          lineNo: lineNo,
          itemId: itemId,
          itemCode: itemCode,
          itemNameTh: itemNameTh,
          unitId: unitId,
          unitCode: unitCode,
          quantity: quantity,
          unitPrice: unitPrice,
          lineTotal: lineTotal,
          receivedQuantity: receivedQuantity,
          remainingQuantity: remainingQuantity,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
