// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_barcode_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemBarcodeItemResponse extends ItemBarcodeItemResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final LocalizedTextResponse? name;
  @override
  final String? status;

  factory _$ItemBarcodeItemResponse(
          [void Function(ItemBarcodeItemResponseBuilder)? updates]) =>
      (ItemBarcodeItemResponseBuilder()..update(updates))._build();

  _$ItemBarcodeItemResponse._({this.id, this.code, this.name, this.status})
      : super._();
  @override
  ItemBarcodeItemResponse rebuild(
          void Function(ItemBarcodeItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemBarcodeItemResponseBuilder toBuilder() =>
      ItemBarcodeItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemBarcodeItemResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        status == other.status;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemBarcodeItemResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('status', status))
        .toString();
  }
}

class ItemBarcodeItemResponseBuilder
    implements
        Builder<ItemBarcodeItemResponse, ItemBarcodeItemResponseBuilder> {
  _$ItemBarcodeItemResponse? _$v;

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

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  ItemBarcodeItemResponseBuilder() {
    ItemBarcodeItemResponse._defaults(this);
  }

  ItemBarcodeItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _status = $v.status;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemBarcodeItemResponse other) {
    _$v = other as _$ItemBarcodeItemResponse;
  }

  @override
  void update(void Function(ItemBarcodeItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemBarcodeItemResponse build() => _build();

  _$ItemBarcodeItemResponse _build() {
    _$ItemBarcodeItemResponse _$result;
    try {
      _$result = _$v ??
          _$ItemBarcodeItemResponse._(
            id: id,
            code: code,
            name: _name?.build(),
            status: status,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemBarcodeItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
