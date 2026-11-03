// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_item_price_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierItemPriceResponse extends SupplierItemPriceResponse {
  @override
  final String? id;
  @override
  final String? supplierId;
  @override
  final String? itemId;
  @override
  final String? itemCode;
  @override
  final String? itemNameTh;
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
  @override
  final String? rowVersion;

  factory _$SupplierItemPriceResponse(
          [void Function(SupplierItemPriceResponseBuilder)? updates]) =>
      (SupplierItemPriceResponseBuilder()..update(updates))._build();

  _$SupplierItemPriceResponse._(
      {this.id,
      this.supplierId,
      this.itemId,
      this.itemCode,
      this.itemNameTh,
      this.unitPrice,
      this.minOrderQuantity,
      this.leadTimeDays,
      this.effectiveFrom,
      this.effectiveTo,
      this.isPreferred,
      this.rowVersion})
      : super._();
  @override
  SupplierItemPriceResponse rebuild(
          void Function(SupplierItemPriceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierItemPriceResponseBuilder toBuilder() =>
      SupplierItemPriceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierItemPriceResponse &&
        id == other.id &&
        supplierId == other.supplierId &&
        itemId == other.itemId &&
        itemCode == other.itemCode &&
        itemNameTh == other.itemNameTh &&
        unitPrice == other.unitPrice &&
        minOrderQuantity == other.minOrderQuantity &&
        leadTimeDays == other.leadTimeDays &&
        effectiveFrom == other.effectiveFrom &&
        effectiveTo == other.effectiveTo &&
        isPreferred == other.isPreferred &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, supplierId.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, itemNameTh.hashCode);
    _$hash = $jc(_$hash, unitPrice.hashCode);
    _$hash = $jc(_$hash, minOrderQuantity.hashCode);
    _$hash = $jc(_$hash, leadTimeDays.hashCode);
    _$hash = $jc(_$hash, effectiveFrom.hashCode);
    _$hash = $jc(_$hash, effectiveTo.hashCode);
    _$hash = $jc(_$hash, isPreferred.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierItemPriceResponse')
          ..add('id', id)
          ..add('supplierId', supplierId)
          ..add('itemId', itemId)
          ..add('itemCode', itemCode)
          ..add('itemNameTh', itemNameTh)
          ..add('unitPrice', unitPrice)
          ..add('minOrderQuantity', minOrderQuantity)
          ..add('leadTimeDays', leadTimeDays)
          ..add('effectiveFrom', effectiveFrom)
          ..add('effectiveTo', effectiveTo)
          ..add('isPreferred', isPreferred)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class SupplierItemPriceResponseBuilder
    implements
        Builder<SupplierItemPriceResponse, SupplierItemPriceResponseBuilder> {
  _$SupplierItemPriceResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _supplierId;
  String? get supplierId => _$this._supplierId;
  set supplierId(String? supplierId) => _$this._supplierId = supplierId;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _itemCode;
  String? get itemCode => _$this._itemCode;
  set itemCode(String? itemCode) => _$this._itemCode = itemCode;

  String? _itemNameTh;
  String? get itemNameTh => _$this._itemNameTh;
  set itemNameTh(String? itemNameTh) => _$this._itemNameTh = itemNameTh;

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

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  SupplierItemPriceResponseBuilder() {
    SupplierItemPriceResponse._defaults(this);
  }

  SupplierItemPriceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _supplierId = $v.supplierId;
      _itemId = $v.itemId;
      _itemCode = $v.itemCode;
      _itemNameTh = $v.itemNameTh;
      _unitPrice = $v.unitPrice;
      _minOrderQuantity = $v.minOrderQuantity;
      _leadTimeDays = $v.leadTimeDays;
      _effectiveFrom = $v.effectiveFrom;
      _effectiveTo = $v.effectiveTo;
      _isPreferred = $v.isPreferred;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierItemPriceResponse other) {
    _$v = other as _$SupplierItemPriceResponse;
  }

  @override
  void update(void Function(SupplierItemPriceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierItemPriceResponse build() => _build();

  _$SupplierItemPriceResponse _build() {
    final _$result = _$v ??
        _$SupplierItemPriceResponse._(
          id: id,
          supplierId: supplierId,
          itemId: itemId,
          itemCode: itemCode,
          itemNameTh: itemNameTh,
          unitPrice: unitPrice,
          minOrderQuantity: minOrderQuantity,
          leadTimeDays: leadTimeDays,
          effectiveFrom: effectiveFrom,
          effectiveTo: effectiveTo,
          isPreferred: isPreferred,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
