// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderLineRequest extends PurchaseOrderLineRequest {
  @override
  final String? itemId;
  @override
  final double? quantity;
  @override
  final double? unitPrice;

  factory _$PurchaseOrderLineRequest(
          [void Function(PurchaseOrderLineRequestBuilder)? updates]) =>
      (PurchaseOrderLineRequestBuilder()..update(updates))._build();

  _$PurchaseOrderLineRequest._({this.itemId, this.quantity, this.unitPrice})
      : super._();
  @override
  PurchaseOrderLineRequest rebuild(
          void Function(PurchaseOrderLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderLineRequestBuilder toBuilder() =>
      PurchaseOrderLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderLineRequest &&
        itemId == other.itemId &&
        quantity == other.quantity &&
        unitPrice == other.unitPrice;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PurchaseOrderLineRequest')
          ..add('itemId', itemId)
          ..add('quantity', quantity)
          ..add('unitPrice', unitPrice))
        .toString();
  }
}

class PurchaseOrderLineRequestBuilder
    implements
        Builder<PurchaseOrderLineRequest, PurchaseOrderLineRequestBuilder> {
  _$PurchaseOrderLineRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  double? _unitPrice;
  double? get unitPrice => _$this._unitPrice;
  set unitPrice(double? unitPrice) => _$this._unitPrice = unitPrice;

  PurchaseOrderLineRequestBuilder() {
    PurchaseOrderLineRequest._defaults(this);
  }

  PurchaseOrderLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _quantity = $v.quantity;
      _unitPrice = $v.unitPrice;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderLineRequest other) {
    _$v = other as _$PurchaseOrderLineRequest;
  }

  @override
  void update(void Function(PurchaseOrderLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderLineRequest build() => _build();

  _$PurchaseOrderLineRequest _build() {
    final _$result = _$v ??
        _$PurchaseOrderLineRequest._(
          itemId: itemId,
          quantity: quantity,
          unitPrice: unitPrice,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
