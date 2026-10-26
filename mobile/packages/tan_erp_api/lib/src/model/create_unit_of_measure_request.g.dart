// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_unit_of_measure_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateUnitOfMeasureRequest extends CreateUnitOfMeasureRequest {
  @override
  final String? code;
  @override
  final LocalizedTextInput name;
  @override
  final String symbol;
  @override
  final String dimension;
  @override
  final int? decimalScale;
  @override
  final String? roundingMode;

  factory _$CreateUnitOfMeasureRequest(
          [void Function(CreateUnitOfMeasureRequestBuilder)? updates]) =>
      (CreateUnitOfMeasureRequestBuilder()..update(updates))._build();

  _$CreateUnitOfMeasureRequest._(
      {this.code,
      required this.name,
      required this.symbol,
      required this.dimension,
      this.decimalScale,
      this.roundingMode})
      : super._();
  @override
  CreateUnitOfMeasureRequest rebuild(
          void Function(CreateUnitOfMeasureRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateUnitOfMeasureRequestBuilder toBuilder() =>
      CreateUnitOfMeasureRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateUnitOfMeasureRequest &&
        code == other.code &&
        name == other.name &&
        symbol == other.symbol &&
        dimension == other.dimension &&
        decimalScale == other.decimalScale &&
        roundingMode == other.roundingMode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, symbol.hashCode);
    _$hash = $jc(_$hash, dimension.hashCode);
    _$hash = $jc(_$hash, decimalScale.hashCode);
    _$hash = $jc(_$hash, roundingMode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateUnitOfMeasureRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('symbol', symbol)
          ..add('dimension', dimension)
          ..add('decimalScale', decimalScale)
          ..add('roundingMode', roundingMode))
        .toString();
  }
}

class CreateUnitOfMeasureRequestBuilder
    implements
        Builder<CreateUnitOfMeasureRequest, CreateUnitOfMeasureRequestBuilder> {
  _$CreateUnitOfMeasureRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextInputBuilder? _name;
  LocalizedTextInputBuilder get name =>
      _$this._name ??= LocalizedTextInputBuilder();
  set name(LocalizedTextInputBuilder? name) => _$this._name = name;

  String? _symbol;
  String? get symbol => _$this._symbol;
  set symbol(String? symbol) => _$this._symbol = symbol;

  String? _dimension;
  String? get dimension => _$this._dimension;
  set dimension(String? dimension) => _$this._dimension = dimension;

  int? _decimalScale;
  int? get decimalScale => _$this._decimalScale;
  set decimalScale(int? decimalScale) => _$this._decimalScale = decimalScale;

  String? _roundingMode;
  String? get roundingMode => _$this._roundingMode;
  set roundingMode(String? roundingMode) => _$this._roundingMode = roundingMode;

  CreateUnitOfMeasureRequestBuilder() {
    CreateUnitOfMeasureRequest._defaults(this);
  }

  CreateUnitOfMeasureRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name.toBuilder();
      _symbol = $v.symbol;
      _dimension = $v.dimension;
      _decimalScale = $v.decimalScale;
      _roundingMode = $v.roundingMode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateUnitOfMeasureRequest other) {
    _$v = other as _$CreateUnitOfMeasureRequest;
  }

  @override
  void update(void Function(CreateUnitOfMeasureRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateUnitOfMeasureRequest build() => _build();

  _$CreateUnitOfMeasureRequest _build() {
    _$CreateUnitOfMeasureRequest _$result;
    try {
      _$result = _$v ??
          _$CreateUnitOfMeasureRequest._(
            code: code,
            name: name.build(),
            symbol: BuiltValueNullFieldError.checkNotNull(
                symbol, r'CreateUnitOfMeasureRequest', 'symbol'),
            dimension: BuiltValueNullFieldError.checkNotNull(
                dimension, r'CreateUnitOfMeasureRequest', 'dimension'),
            decimalScale: decimalScale,
            roundingMode: roundingMode,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        name.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreateUnitOfMeasureRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
