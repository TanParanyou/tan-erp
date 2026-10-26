// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_conversion_snapshot_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateConversionSnapshotResponse
    extends EstimateConversionSnapshotResponse {
  @override
  final String? conversionId;
  @override
  final String? conversionScope;
  @override
  final String? fromUnit;
  @override
  final String? toUnit;
  @override
  final String? factor;

  factory _$EstimateConversionSnapshotResponse(
          [void Function(EstimateConversionSnapshotResponseBuilder)?
              updates]) =>
      (EstimateConversionSnapshotResponseBuilder()..update(updates))._build();

  _$EstimateConversionSnapshotResponse._(
      {this.conversionId,
      this.conversionScope,
      this.fromUnit,
      this.toUnit,
      this.factor})
      : super._();
  @override
  EstimateConversionSnapshotResponse rebuild(
          void Function(EstimateConversionSnapshotResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateConversionSnapshotResponseBuilder toBuilder() =>
      EstimateConversionSnapshotResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateConversionSnapshotResponse &&
        conversionId == other.conversionId &&
        conversionScope == other.conversionScope &&
        fromUnit == other.fromUnit &&
        toUnit == other.toUnit &&
        factor == other.factor;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, conversionId.hashCode);
    _$hash = $jc(_$hash, conversionScope.hashCode);
    _$hash = $jc(_$hash, fromUnit.hashCode);
    _$hash = $jc(_$hash, toUnit.hashCode);
    _$hash = $jc(_$hash, factor.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateConversionSnapshotResponse')
          ..add('conversionId', conversionId)
          ..add('conversionScope', conversionScope)
          ..add('fromUnit', fromUnit)
          ..add('toUnit', toUnit)
          ..add('factor', factor))
        .toString();
  }
}

class EstimateConversionSnapshotResponseBuilder
    implements
        Builder<EstimateConversionSnapshotResponse,
            EstimateConversionSnapshotResponseBuilder> {
  _$EstimateConversionSnapshotResponse? _$v;

  String? _conversionId;
  String? get conversionId => _$this._conversionId;
  set conversionId(String? conversionId) => _$this._conversionId = conversionId;

  String? _conversionScope;
  String? get conversionScope => _$this._conversionScope;
  set conversionScope(String? conversionScope) =>
      _$this._conversionScope = conversionScope;

  String? _fromUnit;
  String? get fromUnit => _$this._fromUnit;
  set fromUnit(String? fromUnit) => _$this._fromUnit = fromUnit;

  String? _toUnit;
  String? get toUnit => _$this._toUnit;
  set toUnit(String? toUnit) => _$this._toUnit = toUnit;

  String? _factor;
  String? get factor => _$this._factor;
  set factor(String? factor) => _$this._factor = factor;

  EstimateConversionSnapshotResponseBuilder() {
    EstimateConversionSnapshotResponse._defaults(this);
  }

  EstimateConversionSnapshotResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _conversionId = $v.conversionId;
      _conversionScope = $v.conversionScope;
      _fromUnit = $v.fromUnit;
      _toUnit = $v.toUnit;
      _factor = $v.factor;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateConversionSnapshotResponse other) {
    _$v = other as _$EstimateConversionSnapshotResponse;
  }

  @override
  void update(
      void Function(EstimateConversionSnapshotResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateConversionSnapshotResponse build() => _build();

  _$EstimateConversionSnapshotResponse _build() {
    final _$result = _$v ??
        _$EstimateConversionSnapshotResponse._(
          conversionId: conversionId,
          conversionScope: conversionScope,
          fromUnit: fromUnit,
          toUnit: toUnit,
          factor: factor,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
