// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'unit_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UnitSummaryResponse extends UnitSummaryResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? symbol;
  @override
  final LocalizedTextResponse? name;

  factory _$UnitSummaryResponse(
          [void Function(UnitSummaryResponseBuilder)? updates]) =>
      (UnitSummaryResponseBuilder()..update(updates))._build();

  _$UnitSummaryResponse._({this.id, this.code, this.symbol, this.name})
      : super._();
  @override
  UnitSummaryResponse rebuild(
          void Function(UnitSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UnitSummaryResponseBuilder toBuilder() =>
      UnitSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UnitSummaryResponse &&
        id == other.id &&
        code == other.code &&
        symbol == other.symbol &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, symbol.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UnitSummaryResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('symbol', symbol)
          ..add('name', name))
        .toString();
  }
}

class UnitSummaryResponseBuilder
    implements Builder<UnitSummaryResponse, UnitSummaryResponseBuilder> {
  _$UnitSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _symbol;
  String? get symbol => _$this._symbol;
  set symbol(String? symbol) => _$this._symbol = symbol;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  UnitSummaryResponseBuilder() {
    UnitSummaryResponse._defaults(this);
  }

  UnitSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _symbol = $v.symbol;
      _name = $v.name?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UnitSummaryResponse other) {
    _$v = other as _$UnitSummaryResponse;
  }

  @override
  void update(void Function(UnitSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UnitSummaryResponse build() => _build();

  _$UnitSummaryResponse _build() {
    _$UnitSummaryResponse _$result;
    try {
      _$result = _$v ??
          _$UnitSummaryResponse._(
            id: id,
            code: code,
            symbol: symbol,
            name: _name?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UnitSummaryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
