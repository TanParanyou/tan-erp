// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_unit_of_measure_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateUnitOfMeasureRequest extends UpdateUnitOfMeasureRequest {
  @override
  final String code;
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

  factory _$UpdateUnitOfMeasureRequest(
          [void Function(UpdateUnitOfMeasureRequestBuilder)? updates]) =>
      (UpdateUnitOfMeasureRequestBuilder()..update(updates))._build();

  _$UpdateUnitOfMeasureRequest._(
      {required this.code,
      required this.name,
      required this.symbol,
      required this.dimension,
      this.decimalScale,
      this.roundingMode})
      : super._();
  @override
  UpdateUnitOfMeasureRequest rebuild(
          void Function(UpdateUnitOfMeasureRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateUnitOfMeasureRequestBuilder toBuilder() =>
      UpdateUnitOfMeasureRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateUnitOfMeasureRequest &&
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
    return (newBuiltValueToStringHelper(r'UpdateUnitOfMeasureRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('symbol', symbol)
          ..add('dimension', dimension)
          ..add('decimalScale', decimalScale)
          ..add('roundingMode', roundingMode))
        .toString();
  }
}

class UpdateUnitOfMeasureRequestBuilder
    implements
        Builder<UpdateUnitOfMeasureRequest, UpdateUnitOfMeasureRequestBuilder> {
  _$UpdateUnitOfMeasureRequest? _$v;

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

  UpdateUnitOfMeasureRequestBuilder() {
    UpdateUnitOfMeasureRequest._defaults(this);
  }

  UpdateUnitOfMeasureRequestBuilder get _$this {
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
  void replace(UpdateUnitOfMeasureRequest other) {
    _$v = other as _$UpdateUnitOfMeasureRequest;
  }

  @override
  void update(void Function(UpdateUnitOfMeasureRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateUnitOfMeasureRequest build() => _build();

  _$UpdateUnitOfMeasureRequest _build() {
    _$UpdateUnitOfMeasureRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateUnitOfMeasureRequest._(
            code: BuiltValueNullFieldError.checkNotNull(
                code, r'UpdateUnitOfMeasureRequest', 'code'),
            name: name.build(),
            symbol: BuiltValueNullFieldError.checkNotNull(
                symbol, r'UpdateUnitOfMeasureRequest', 'symbol'),
            dimension: BuiltValueNullFieldError.checkNotNull(
                dimension, r'UpdateUnitOfMeasureRequest', 'dimension'),
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
            r'UpdateUnitOfMeasureRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
