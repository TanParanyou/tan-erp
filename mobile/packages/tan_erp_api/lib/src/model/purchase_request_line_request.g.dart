// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseRequestLineRequest extends PurchaseRequestLineRequest {
  @override
  final String? itemId;
  @override
  final double? quantity;
  @override
  final double? estimatedUnitPrice;
  @override
  final String? note;

  factory _$PurchaseRequestLineRequest(
          [void Function(PurchaseRequestLineRequestBuilder)? updates]) =>
      (PurchaseRequestLineRequestBuilder()..update(updates))._build();

  _$PurchaseRequestLineRequest._(
      {this.itemId, this.quantity, this.estimatedUnitPrice, this.note})
      : super._();
  @override
  PurchaseRequestLineRequest rebuild(
          void Function(PurchaseRequestLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseRequestLineRequestBuilder toBuilder() =>
      PurchaseRequestLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseRequestLineRequest &&
        itemId == other.itemId &&
        quantity == other.quantity &&
        estimatedUnitPrice == other.estimatedUnitPrice &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, estimatedUnitPrice.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseRequestLineRequest')
          ..add('itemId', itemId)
          ..add('quantity', quantity)
          ..add('estimatedUnitPrice', estimatedUnitPrice)
          ..add('note', note))
        .toString();
  }
}

class PurchaseRequestLineRequestBuilder
    implements
        Builder<PurchaseRequestLineRequest, PurchaseRequestLineRequestBuilder> {
  _$PurchaseRequestLineRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  double? _estimatedUnitPrice;
  double? get estimatedUnitPrice => _$this._estimatedUnitPrice;
  set estimatedUnitPrice(double? estimatedUnitPrice) =>
      _$this._estimatedUnitPrice = estimatedUnitPrice;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  PurchaseRequestLineRequestBuilder() {
    PurchaseRequestLineRequest._defaults(this);
  }

  PurchaseRequestLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _quantity = $v.quantity;
      _estimatedUnitPrice = $v.estimatedUnitPrice;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseRequestLineRequest other) {
    _$v = other as _$PurchaseRequestLineRequest;
  }

  @override
  void update(void Function(PurchaseRequestLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseRequestLineRequest build() => _build();

  _$PurchaseRequestLineRequest _build() {
    final _$result = _$v ??
        _$PurchaseRequestLineRequest._(
          itemId: itemId,
          quantity: quantity,
          estimatedUnitPrice: estimatedUnitPrice,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
