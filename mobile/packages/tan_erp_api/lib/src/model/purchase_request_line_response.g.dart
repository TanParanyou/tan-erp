// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseRequestLineResponse extends PurchaseRequestLineResponse {
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
  final double? estimatedUnitPrice;
  @override
  final double? estimatedLineTotal;
  @override
  final String? note;

  factory _$PurchaseRequestLineResponse(
          [void Function(PurchaseRequestLineResponseBuilder)? updates]) =>
      (PurchaseRequestLineResponseBuilder()..update(updates))._build();

  _$PurchaseRequestLineResponse._(
      {this.id,
      this.lineNo,
      this.itemId,
      this.itemCode,
      this.itemNameTh,
      this.unitId,
      this.unitCode,
      this.quantity,
      this.estimatedUnitPrice,
      this.estimatedLineTotal,
      this.note})
      : super._();
  @override
  PurchaseRequestLineResponse rebuild(
          void Function(PurchaseRequestLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseRequestLineResponseBuilder toBuilder() =>
      PurchaseRequestLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseRequestLineResponse &&
        id == other.id &&
        lineNo == other.lineNo &&
        itemId == other.itemId &&
        itemCode == other.itemCode &&
        itemNameTh == other.itemNameTh &&
        unitId == other.unitId &&
        unitCode == other.unitCode &&
        quantity == other.quantity &&
        estimatedUnitPrice == other.estimatedUnitPrice &&
        estimatedLineTotal == other.estimatedLineTotal &&
        note == other.note;
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
    _$hash = $jc(_$hash, estimatedUnitPrice.hashCode);
    _$hash = $jc(_$hash, estimatedLineTotal.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseRequestLineResponse')
          ..add('id', id)
          ..add('lineNo', lineNo)
          ..add('itemId', itemId)
          ..add('itemCode', itemCode)
          ..add('itemNameTh', itemNameTh)
          ..add('unitId', unitId)
          ..add('unitCode', unitCode)
          ..add('quantity', quantity)
          ..add('estimatedUnitPrice', estimatedUnitPrice)
          ..add('estimatedLineTotal', estimatedLineTotal)
          ..add('note', note))
        .toString();
  }
}

class PurchaseRequestLineResponseBuilder
    implements
        Builder<PurchaseRequestLineResponse,
            PurchaseRequestLineResponseBuilder> {
  _$PurchaseRequestLineResponse? _$v;

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

  double? _estimatedUnitPrice;
  double? get estimatedUnitPrice => _$this._estimatedUnitPrice;
  set estimatedUnitPrice(double? estimatedUnitPrice) =>
      _$this._estimatedUnitPrice = estimatedUnitPrice;

  double? _estimatedLineTotal;
  double? get estimatedLineTotal => _$this._estimatedLineTotal;
  set estimatedLineTotal(double? estimatedLineTotal) =>
      _$this._estimatedLineTotal = estimatedLineTotal;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  PurchaseRequestLineResponseBuilder() {
    PurchaseRequestLineResponse._defaults(this);
  }

  PurchaseRequestLineResponseBuilder get _$this {
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
      _estimatedUnitPrice = $v.estimatedUnitPrice;
      _estimatedLineTotal = $v.estimatedLineTotal;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseRequestLineResponse other) {
    _$v = other as _$PurchaseRequestLineResponse;
  }

  @override
  void update(void Function(PurchaseRequestLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseRequestLineResponse build() => _build();

  _$PurchaseRequestLineResponse _build() {
    final _$result = _$v ??
        _$PurchaseRequestLineResponse._(
          id: id,
          lineNo: lineNo,
          itemId: itemId,
          itemCode: itemCode,
          itemNameTh: itemNameTh,
          unitId: unitId,
          unitCode: unitCode,
          quantity: quantity,
          estimatedUnitPrice: estimatedUnitPrice,
          estimatedLineTotal: estimatedLineTotal,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
