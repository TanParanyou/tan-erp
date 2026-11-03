// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_item_price_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierItemPriceRequest extends SupplierItemPriceRequest {
  @override
  final String? itemId;
  @override
  final double? unitPrice;
  @override
  final double? minOrderQuantity;
  @override
  final int? leadTimeDays;
  @override
  final Date? effectiveFrom;
  @override
  final Date? effectiveTo;
  @override
  final bool? isPreferred;

  factory _$SupplierItemPriceRequest(
          [void Function(SupplierItemPriceRequestBuilder)? updates]) =>
      (SupplierItemPriceRequestBuilder()..update(updates))._build();

  _$SupplierItemPriceRequest._(
      {this.itemId,
      this.unitPrice,
      this.minOrderQuantity,
      this.leadTimeDays,
      this.effectiveFrom,
      this.effectiveTo,
      this.isPreferred})
      : super._();
  @override
  SupplierItemPriceRequest rebuild(
          void Function(SupplierItemPriceRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierItemPriceRequestBuilder toBuilder() =>
      SupplierItemPriceRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierItemPriceRequest &&
        itemId == other.itemId &&
        unitPrice == other.unitPrice &&
        minOrderQuantity == other.minOrderQuantity &&
        leadTimeDays == other.leadTimeDays &&
        effectiveFrom == other.effectiveFrom &&
        effectiveTo == other.effectiveTo &&
        isPreferred == other.isPreferred;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jc(_$hash, minOrderQuantity.hashCode);
    _$hash = $jc(_$hash, leadTimeDays.hashCode);
    _$hash = $jc(_$hash, effectiveFrom.hashCode);
    _$hash = $jc(_$hash, effectiveTo.hashCode);
    _$hash = $jc(_$hash, isPreferred.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierItemPriceRequest')
          ..add('itemId', itemId)
          ..add('unitPrice', unitPrice)
          ..add('minOrderQuantity', minOrderQuantity)
          ..add('leadTimeDays', leadTimeDays)
          ..add('effectiveFrom', effectiveFrom)
          ..add('effectiveTo', effectiveTo)
          ..add('isPreferred', isPreferred))
        .toString();
  }
}

class SupplierItemPriceRequestBuilder
    implements
        Builder<SupplierItemPriceRequest, SupplierItemPriceRequestBuilder> {
  _$SupplierItemPriceRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _unitPrice;
  double? get unitPrice => _$this._unitPrice;
  set unitPrice(double? unitPrice) => _$this._unitPrice = unitPrice;

  double? _minOrderQuantity;
  double? get minOrderQuantity => _$this._minOrderQuantity;
  set minOrderQuantity(double? minOrderQuantity) =>
      _$this._minOrderQuantity = minOrderQuantity;

  int? _leadTimeDays;
  int? get leadTimeDays => _$this._leadTimeDays;
  set leadTimeDays(int? leadTimeDays) => _$this._leadTimeDays = leadTimeDays;

  Date? _effectiveFrom;
  Date? get effectiveFrom => _$this._effectiveFrom;
  set effectiveFrom(Date? effectiveFrom) =>
      _$this._effectiveFrom = effectiveFrom;

  Date? _effectiveTo;
  Date? get effectiveTo => _$this._effectiveTo;
  set effectiveTo(Date? effectiveTo) => _$this._effectiveTo = effectiveTo;

  bool? _isPreferred;
  bool? get isPreferred => _$this._isPreferred;
  set isPreferred(bool? isPreferred) => _$this._isPreferred = isPreferred;

  SupplierItemPriceRequestBuilder() {
    SupplierItemPriceRequest._defaults(this);
  }

  SupplierItemPriceRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _unitPrice = $v.unitPrice;
      _minOrderQuantity = $v.minOrderQuantity;
      _leadTimeDays = $v.leadTimeDays;
      _effectiveFrom = $v.effectiveFrom;
      _effectiveTo = $v.effectiveTo;
      _isPreferred = $v.isPreferred;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierItemPriceRequest other) {
    _$v = other as _$SupplierItemPriceRequest;
  }

  @override
  void update(void Function(SupplierItemPriceRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierItemPriceRequest build() => _build();

  _$SupplierItemPriceRequest _build() {
    final _$result = _$v ??
        _$SupplierItemPriceRequest._(
          itemId: itemId,
          unitPrice: unitPrice,
          minOrderQuantity: minOrderQuantity,
          leadTimeDays: leadTimeDays,
          effectiveFrom: effectiveFrom,
          effectiveTo: effectiveTo,
          isPreferred: isPreferred,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
