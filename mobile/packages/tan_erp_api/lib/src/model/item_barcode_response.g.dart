// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_barcode_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemBarcodeResponse extends ItemBarcodeResponse {
  @override
  final String? id;
  @override
  final ItemBarcodeItemResponse? item;
  @override
  final String? identifierType;
  @override
  final String? value;
  @override
  final ItemBarcodeUnitResponse? unit;
  @override
  final String? quantityInBaseUnit;
  @override
  final String? packagingLevel;
  @override
  final bool? isPrimary;
  @override
  final String? status;
  @override
  final String? rowVersion;

  factory _$ItemBarcodeResponse(
          [void Function(ItemBarcodeResponseBuilder)? updates]) =>
      (ItemBarcodeResponseBuilder()..update(updates))._build();

  _$ItemBarcodeResponse._(
      {this.id,
      this.item,
      this.identifierType,
      this.value,
      this.unit,
      this.quantityInBaseUnit,
      this.packagingLevel,
      this.isPrimary,
      this.status,
      this.rowVersion})
      : super._();
  @override
  ItemBarcodeResponse rebuild(
          void Function(ItemBarcodeResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemBarcodeResponseBuilder toBuilder() =>
      ItemBarcodeResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemBarcodeResponse &&
        id == other.id &&
        item == other.item &&
        identifierType == other.identifierType &&
        value == other.value &&
        unit == other.unit &&
        quantityInBaseUnit == other.quantityInBaseUnit &&
        packagingLevel == other.packagingLevel &&
        isPrimary == other.isPrimary &&
        status == other.status &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, identifierType.hashCode);
    _$hash = $jc(_$hash, value.hashCode);
    _$hash = $jc(_$hash, unit.hashCode);
    _$hash = $jc(_$hash, quantityInBaseUnit.hashCode);
    _$hash = $jc(_$hash, packagingLevel.hashCode);
    _$hash = $jc(_$hash, isPrimary.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemBarcodeResponse')
          ..add('id', id)
          ..add('item', item)
          ..add('identifierType', identifierType)
          ..add('value', value)
          ..add('unit', unit)
          ..add('quantityInBaseUnit', quantityInBaseUnit)
          ..add('packagingLevel', packagingLevel)
          ..add('isPrimary', isPrimary)
          ..add('status', status)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class ItemBarcodeResponseBuilder
    implements Builder<ItemBarcodeResponse, ItemBarcodeResponseBuilder> {
  _$ItemBarcodeResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  ItemBarcodeItemResponseBuilder? _item;
  ItemBarcodeItemResponseBuilder get item =>
      _$this._item ??= ItemBarcodeItemResponseBuilder();
  set item(ItemBarcodeItemResponseBuilder? item) => _$this._item = item;

  String? _identifierType;
  String? get identifierType => _$this._identifierType;
  set identifierType(String? identifierType) =>
      _$this._identifierType = identifierType;

  String? _value;
  String? get value => _$this._value;
  set value(String? value) => _$this._value = value;

  ItemBarcodeUnitResponseBuilder? _unit;
  ItemBarcodeUnitResponseBuilder get unit =>
      _$this._unit ??= ItemBarcodeUnitResponseBuilder();
  set unit(ItemBarcodeUnitResponseBuilder? unit) => _$this._unit = unit;

  String? _quantityInBaseUnit;
  String? get quantityInBaseUnit => _$this._quantityInBaseUnit;
  set quantityInBaseUnit(String? quantityInBaseUnit) =>
      _$this._quantityInBaseUnit = quantityInBaseUnit;

  String? _packagingLevel;
  String? get packagingLevel => _$this._packagingLevel;
  set packagingLevel(String? packagingLevel) =>
      _$this._packagingLevel = packagingLevel;

  bool? _isPrimary;
  bool? get isPrimary => _$this._isPrimary;
  set isPrimary(bool? isPrimary) => _$this._isPrimary = isPrimary;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ItemBarcodeResponseBuilder() {
    ItemBarcodeResponse._defaults(this);
  }

  ItemBarcodeResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _item = $v.item?.toBuilder();
      _identifierType = $v.identifierType;
      _value = $v.value;
      _unit = $v.unit?.toBuilder();
      _quantityInBaseUnit = $v.quantityInBaseUnit;
      _packagingLevel = $v.packagingLevel;
      _isPrimary = $v.isPrimary;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemBarcodeResponse other) {
    _$v = other as _$ItemBarcodeResponse;
  }

  @override
  void update(void Function(ItemBarcodeResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemBarcodeResponse build() => _build();

  _$ItemBarcodeResponse _build() {
    _$ItemBarcodeResponse _$result;
    try {
      _$result = _$v ??
          _$ItemBarcodeResponse._(
            id: id,
            item: _item?.build(),
            identifierType: identifierType,
            value: value,
            unit: _unit?.build(),
            quantityInBaseUnit: quantityInBaseUnit,
            packagingLevel: packagingLevel,
            isPrimary: isPrimary,
            status: status,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'item';
        _item?.build();

        _$failedField = 'unit';
        _unit?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemBarcodeResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
