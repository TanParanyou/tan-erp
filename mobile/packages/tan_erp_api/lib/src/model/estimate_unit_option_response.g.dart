// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_unit_option_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateUnitOptionResponse extends EstimateUnitOptionResponse {
  @override
  final String? unitId;
  @override
  final String? unitCode;
  @override
  final bool? isBase;
  @override
  final String? conversionId;
  @override
  final String? conversionScope;
  @override
  final String? factor;

  factory _$EstimateUnitOptionResponse(
          [void Function(EstimateUnitOptionResponseBuilder)? updates]) =>
      (EstimateUnitOptionResponseBuilder()..update(updates))._build();

  _$EstimateUnitOptionResponse._(
      {this.unitId,
      this.unitCode,
      this.isBase,
      this.conversionId,
      this.conversionScope,
      this.factor})
      : super._();
  @override
  EstimateUnitOptionResponse rebuild(
          void Function(EstimateUnitOptionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateUnitOptionResponseBuilder toBuilder() =>
      EstimateUnitOptionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateUnitOptionResponse &&
        unitId == other.unitId &&
        unitCode == other.unitCode &&
        isBase == other.isBase &&
        conversionId == other.conversionId &&
        conversionScope == other.conversionScope &&
        factor == other.factor;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, unitId.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, isBase.hashCode);
    _$hash = $jc(_$hash, conversionId.hashCode);
    _$hash = $jc(_$hash, conversionScope.hashCode);
    _$hash = $jc(_$hash, factor.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateUnitOptionResponse')
          ..add('unitId', unitId)
          ..add('unitCode', unitCode)
          ..add('isBase', isBase)
          ..add('conversionId', conversionId)
          ..add('conversionScope', conversionScope)
          ..add('factor', factor))
        .toString();
  }
}

class EstimateUnitOptionResponseBuilder
    implements
        Builder<EstimateUnitOptionResponse, EstimateUnitOptionResponseBuilder> {
  _$EstimateUnitOptionResponse? _$v;

  String? _unitId;
  String? get unitId => _$this._unitId;
  set unitId(String? unitId) => _$this._unitId = unitId;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  bool? _isBase;
  bool? get isBase => _$this._isBase;
  set isBase(bool? isBase) => _$this._isBase = isBase;

  String? _conversionId;
  String? get conversionId => _$this._conversionId;
  set conversionId(String? conversionId) => _$this._conversionId = conversionId;

  String? _conversionScope;
  String? get conversionScope => _$this._conversionScope;
  set conversionScope(String? conversionScope) =>
      _$this._conversionScope = conversionScope;

  String? _factor;
  String? get factor => _$this._factor;
  set factor(String? factor) => _$this._factor = factor;

  EstimateUnitOptionResponseBuilder() {
    EstimateUnitOptionResponse._defaults(this);
  }

  EstimateUnitOptionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _unitId = $v.unitId;
      _unitCode = $v.unitCode;
      _isBase = $v.isBase;
      _conversionId = $v.conversionId;
      _conversionScope = $v.conversionScope;
      _factor = $v.factor;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateUnitOptionResponse other) {
    _$v = other as _$EstimateUnitOptionResponse;
  }

  @override
  void update(void Function(EstimateUnitOptionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateUnitOptionResponse build() => _build();

  _$EstimateUnitOptionResponse _build() {
    final _$result = _$v ??
        _$EstimateUnitOptionResponse._(
          unitId: unitId,
          unitCode: unitCode,
          isBase: isBase,
          conversionId: conversionId,
          conversionScope: conversionScope,
          factor: factor,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
