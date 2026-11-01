// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_unit_price_override_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseRequestUnitPriceOverrideRequest
    extends PurchaseRequestUnitPriceOverrideRequest {
  @override
  final String? itemId;
  @override
  final double? unitPrice;

  factory _$PurchaseRequestUnitPriceOverrideRequest(
          [void Function(PurchaseRequestUnitPriceOverrideRequestBuilder)?
              updates]) =>
      (PurchaseRequestUnitPriceOverrideRequestBuilder()..update(updates))
          ._build();

  _$PurchaseRequestUnitPriceOverrideRequest._({this.itemId, this.unitPrice})
      : super._();
  @override
  PurchaseRequestUnitPriceOverrideRequest rebuild(
          void Function(PurchaseRequestUnitPriceOverrideRequestBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseRequestUnitPriceOverrideRequestBuilder toBuilder() =>
      PurchaseRequestUnitPriceOverrideRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseRequestUnitPriceOverrideRequest &&
        itemId == other.itemId &&
        unitPrice == other.unitPrice;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(
            r'PurchaseRequestUnitPriceOverrideRequest')
          ..add('itemId', itemId)
          ..add('unitPrice', unitPrice))
        .toString();
  }
}

class PurchaseRequestUnitPriceOverrideRequestBuilder
    implements
        Builder<PurchaseRequestUnitPriceOverrideRequest,
            PurchaseRequestUnitPriceOverrideRequestBuilder> {
  _$PurchaseRequestUnitPriceOverrideRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _unitPrice;
  double? get unitPrice => _$this._unitPrice;
  set unitPrice(double? unitPrice) => _$this._unitPrice = unitPrice;

  PurchaseRequestUnitPriceOverrideRequestBuilder() {
    PurchaseRequestUnitPriceOverrideRequest._defaults(this);
  }

  PurchaseRequestUnitPriceOverrideRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _unitPrice = $v.unitPrice;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseRequestUnitPriceOverrideRequest other) {
    _$v = other as _$PurchaseRequestUnitPriceOverrideRequest;
  }

  @override
  void update(
      void Function(PurchaseRequestUnitPriceOverrideRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseRequestUnitPriceOverrideRequest build() => _build();

  _$PurchaseRequestUnitPriceOverrideRequest _build() {
    final _$result = _$v ??
        _$PurchaseRequestUnitPriceOverrideRequest._(
          itemId: itemId,
          unitPrice: unitPrice,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
