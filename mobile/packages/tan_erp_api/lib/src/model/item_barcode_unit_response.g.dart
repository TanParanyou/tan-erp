// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_barcode_unit_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemBarcodeUnitResponse extends ItemBarcodeUnitResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final LocalizedTextResponse? name;
  @override
  final String? symbol;

  factory _$ItemBarcodeUnitResponse(
          [void Function(ItemBarcodeUnitResponseBuilder)? updates]) =>
      (ItemBarcodeUnitResponseBuilder()..update(updates))._build();

  _$ItemBarcodeUnitResponse._({this.id, this.code, this.name, this.symbol})
      : super._();
  @override
  ItemBarcodeUnitResponse rebuild(
          void Function(ItemBarcodeUnitResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemBarcodeUnitResponseBuilder toBuilder() =>
      ItemBarcodeUnitResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemBarcodeUnitResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        symbol == other.symbol;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, symbol.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemBarcodeUnitResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('symbol', symbol))
        .toString();
  }
}

class ItemBarcodeUnitResponseBuilder
    implements
        Builder<ItemBarcodeUnitResponse, ItemBarcodeUnitResponseBuilder> {
  _$ItemBarcodeUnitResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  String? _symbol;
  String? get symbol => _$this._symbol;
  set symbol(String? symbol) => _$this._symbol = symbol;

  ItemBarcodeUnitResponseBuilder() {
    ItemBarcodeUnitResponse._defaults(this);
  }

  ItemBarcodeUnitResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _symbol = $v.symbol;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemBarcodeUnitResponse other) {
    _$v = other as _$ItemBarcodeUnitResponse;
  }

  @override
  void update(void Function(ItemBarcodeUnitResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemBarcodeUnitResponse build() => _build();

  _$ItemBarcodeUnitResponse _build() {
    _$ItemBarcodeUnitResponse _$result;
    try {
      _$result = _$v ??
          _$ItemBarcodeUnitResponse._(
            id: id,
            code: code,
            name: _name?.build(),
            symbol: symbol,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemBarcodeUnitResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
