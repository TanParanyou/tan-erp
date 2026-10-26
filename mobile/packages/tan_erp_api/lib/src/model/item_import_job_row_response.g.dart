// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_import_job_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemImportJobRowResponse extends ItemImportJobRowResponse {
  @override
  final int? rowNumber;
  @override
  final String? code;
  @override
  final String? nameTh;
  @override
  final BuiltList<ItemImportRowErrorResponse>? errors;

  factory _$ItemImportJobRowResponse(
          [void Function(ItemImportJobRowResponseBuilder)? updates]) =>
      (ItemImportJobRowResponseBuilder()..update(updates))._build();

  _$ItemImportJobRowResponse._(
      {this.rowNumber, this.code, this.nameTh, this.errors})
      : super._();
  @override
  ItemImportJobRowResponse rebuild(
          void Function(ItemImportJobRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemImportJobRowResponseBuilder toBuilder() =>
      ItemImportJobRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemImportJobRowResponse &&
        rowNumber == other.rowNumber &&
        code == other.code &&
        nameTh == other.nameTh &&
        errors == other.errors;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, rowNumber.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, nameTh.hashCode);
    _$hash = $jc(_$hash, errors.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemImportJobRowResponse')
          ..add('rowNumber', rowNumber)
          ..add('code', code)
          ..add('nameTh', nameTh)
          ..add('errors', errors))
        .toString();
  }
}

class ItemImportJobRowResponseBuilder
    implements
        Builder<ItemImportJobRowResponse, ItemImportJobRowResponseBuilder> {
  _$ItemImportJobRowResponse? _$v;

  int? _rowNumber;
  int? get rowNumber => _$this._rowNumber;
  set rowNumber(int? rowNumber) => _$this._rowNumber = rowNumber;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _nameTh;
  String? get nameTh => _$this._nameTh;
  set nameTh(String? nameTh) => _$this._nameTh = nameTh;

  ListBuilder<ItemImportRowErrorResponse>? _errors;
  ListBuilder<ItemImportRowErrorResponse> get errors =>
      _$this._errors ??= ListBuilder<ItemImportRowErrorResponse>();
  set errors(ListBuilder<ItemImportRowErrorResponse>? errors) =>
      _$this._errors = errors;

  ItemImportJobRowResponseBuilder() {
    ItemImportJobRowResponse._defaults(this);
  }

  ItemImportJobRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _rowNumber = $v.rowNumber;
      _code = $v.code;
      _nameTh = $v.nameTh;
      _errors = $v.errors?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemImportJobRowResponse other) {
    _$v = other as _$ItemImportJobRowResponse;
  }

  @override
  void update(void Function(ItemImportJobRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemImportJobRowResponse build() => _build();

  _$ItemImportJobRowResponse _build() {
    _$ItemImportJobRowResponse _$result;
    try {
      _$result = _$v ??
          _$ItemImportJobRowResponse._(
            rowNumber: rowNumber,
            code: code,
            nameTh: nameTh,
            errors: _errors?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'errors';
        _errors?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemImportJobRowResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
