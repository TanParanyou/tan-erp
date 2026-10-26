// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_unit_options_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateUnitOptionsResponse extends EstimateUnitOptionsResponse {
  @override
  final bool? conversionsEnabled;
  @override
  final String? baseUnitCode;
  @override
  final BuiltList<EstimateUnitOptionResponse>? units;

  factory _$EstimateUnitOptionsResponse(
          [void Function(EstimateUnitOptionsResponseBuilder)? updates]) =>
      (EstimateUnitOptionsResponseBuilder()..update(updates))._build();

  _$EstimateUnitOptionsResponse._(
      {this.conversionsEnabled, this.baseUnitCode, this.units})
      : super._();
  @override
  EstimateUnitOptionsResponse rebuild(
          void Function(EstimateUnitOptionsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateUnitOptionsResponseBuilder toBuilder() =>
      EstimateUnitOptionsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateUnitOptionsResponse &&
        conversionsEnabled == other.conversionsEnabled &&
        baseUnitCode == other.baseUnitCode &&
        units == other.units;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, conversionsEnabled.hashCode);
    _$hash = $jc(_$hash, baseUnitCode.hashCode);
    _$hash = $jc(_$hash, units.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateUnitOptionsResponse')
          ..add('conversionsEnabled', conversionsEnabled)
          ..add('baseUnitCode', baseUnitCode)
          ..add('units', units))
        .toString();
  }
}

class EstimateUnitOptionsResponseBuilder
    implements
        Builder<EstimateUnitOptionsResponse,
            EstimateUnitOptionsResponseBuilder> {
  _$EstimateUnitOptionsResponse? _$v;

  bool? _conversionsEnabled;
  bool? get conversionsEnabled => _$this._conversionsEnabled;
  set conversionsEnabled(bool? conversionsEnabled) =>
      _$this._conversionsEnabled = conversionsEnabled;

  String? _baseUnitCode;
  String? get baseUnitCode => _$this._baseUnitCode;
  set baseUnitCode(String? baseUnitCode) => _$this._baseUnitCode = baseUnitCode;

  ListBuilder<EstimateUnitOptionResponse>? _units;
  ListBuilder<EstimateUnitOptionResponse> get units =>
      _$this._units ??= ListBuilder<EstimateUnitOptionResponse>();
  set units(ListBuilder<EstimateUnitOptionResponse>? units) =>
      _$this._units = units;

  EstimateUnitOptionsResponseBuilder() {
    EstimateUnitOptionsResponse._defaults(this);
  }

  EstimateUnitOptionsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _conversionsEnabled = $v.conversionsEnabled;
      _baseUnitCode = $v.baseUnitCode;
      _units = $v.units?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateUnitOptionsResponse other) {
    _$v = other as _$EstimateUnitOptionsResponse;
  }

  @override
  void update(void Function(EstimateUnitOptionsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateUnitOptionsResponse build() => _build();

  _$EstimateUnitOptionsResponse _build() {
    _$EstimateUnitOptionsResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateUnitOptionsResponse._(
            conversionsEnabled: conversionsEnabled,
            baseUnitCode: baseUnitCode,
            units: _units?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'units';
        _units?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateUnitOptionsResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
