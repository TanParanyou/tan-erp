// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_import_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemImportRowResponse extends ItemImportRowResponse {
  @override
  final int? rowNumber;
  @override
  final String? code;
  @override
  final String? nameTh;
  @override
  final bool? isValid;
  @override
  final BuiltList<ItemImportRowErrorResponse>? errors;
  @override
  final String? outcome;

  factory _$ItemImportRowResponse(
          [void Function(ItemImportRowResponseBuilder)? updates]) =>
      (ItemImportRowResponseBuilder()..update(updates))._build();

  _$ItemImportRowResponse._(
      {this.rowNumber,
      this.code,
      this.nameTh,
      this.isValid,
      this.errors,
      this.outcome})
      : super._();
  @override
  ItemImportRowResponse rebuild(
          void Function(ItemImportRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemImportRowResponseBuilder toBuilder() =>
      ItemImportRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemImportRowResponse &&
        rowNumber == other.rowNumber &&
        code == other.code &&
        nameTh == other.nameTh &&
        isValid == other.isValid &&
        errors == other.errors &&
        outcome == other.outcome;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, rowNumber.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, nameTh.hashCode);
    _$hash = $jc(_$hash, isValid.hashCode);
    _$hash = $jc(_$hash, errors.hashCode);
    _$hash = $jc(_$hash, outcome.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemImportRowResponse')
          ..add('rowNumber', rowNumber)
          ..add('code', code)
          ..add('nameTh', nameTh)
          ..add('isValid', isValid)
          ..add('errors', errors)
          ..add('outcome', outcome))
        .toString();
  }
}

class ItemImportRowResponseBuilder
    implements Builder<ItemImportRowResponse, ItemImportRowResponseBuilder> {
  _$ItemImportRowResponse? _$v;

  int? _rowNumber;
  int? get rowNumber => _$this._rowNumber;
  set rowNumber(int? rowNumber) => _$this._rowNumber = rowNumber;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _nameTh;
  String? get nameTh => _$this._nameTh;
  set nameTh(String? nameTh) => _$this._nameTh = nameTh;

  bool? _isValid;
  bool? get isValid => _$this._isValid;
  set isValid(bool? isValid) => _$this._isValid = isValid;

  ListBuilder<ItemImportRowErrorResponse>? _errors;
  ListBuilder<ItemImportRowErrorResponse> get errors =>
      _$this._errors ??= ListBuilder<ItemImportRowErrorResponse>();
  set errors(ListBuilder<ItemImportRowErrorResponse>? errors) =>
      _$this._errors = errors;

  String? _outcome;
  String? get outcome => _$this._outcome;
  set outcome(String? outcome) => _$this._outcome = outcome;

  ItemImportRowResponseBuilder() {
    ItemImportRowResponse._defaults(this);
  }

  ItemImportRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _rowNumber = $v.rowNumber;
      _code = $v.code;
      _nameTh = $v.nameTh;
      _isValid = $v.isValid;
      _errors = $v.errors?.toBuilder();
      _outcome = $v.outcome;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemImportRowResponse other) {
    _$v = other as _$ItemImportRowResponse;
  }

  @override
  void update(void Function(ItemImportRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemImportRowResponse build() => _build();

  _$ItemImportRowResponse _build() {
    _$ItemImportRowResponse _$result;
    try {
      _$result = _$v ??
          _$ItemImportRowResponse._(
            rowNumber: rowNumber,
            code: code,
            nameTh: nameTh,
            isValid: isValid,
            errors: _errors?.build(),
            outcome: outcome,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'errors';
        _errors?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemImportRowResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
