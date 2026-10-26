// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_import_row_error_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemImportRowErrorResponse extends ItemImportRowErrorResponse {
  @override
  final String? field;
  @override
  final String? code;

  factory _$ItemImportRowErrorResponse(
          [void Function(ItemImportRowErrorResponseBuilder)? updates]) =>
      (ItemImportRowErrorResponseBuilder()..update(updates))._build();

  _$ItemImportRowErrorResponse._({this.field, this.code}) : super._();
  @override
  ItemImportRowErrorResponse rebuild(
          void Function(ItemImportRowErrorResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemImportRowErrorResponseBuilder toBuilder() =>
      ItemImportRowErrorResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemImportRowErrorResponse &&
        field == other.field &&
        code == other.code;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, field.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemImportRowErrorResponse')
          ..add('field', field)
          ..add('code', code))
        .toString();
  }
}

class ItemImportRowErrorResponseBuilder
    implements
        Builder<ItemImportRowErrorResponse, ItemImportRowErrorResponseBuilder> {
  _$ItemImportRowErrorResponse? _$v;

  String? _field;
  String? get field => _$this._field;
  set field(String? field) => _$this._field = field;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  ItemImportRowErrorResponseBuilder() {
    ItemImportRowErrorResponse._defaults(this);
  }

  ItemImportRowErrorResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _field = $v.field;
      _code = $v.code;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemImportRowErrorResponse other) {
    _$v = other as _$ItemImportRowErrorResponse;
  }

  @override
  void update(void Function(ItemImportRowErrorResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemImportRowErrorResponse build() => _build();

  _$ItemImportRowErrorResponse _build() {
    final _$result = _$v ??
        _$ItemImportRowErrorResponse._(
          field: field,
          code: code,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
