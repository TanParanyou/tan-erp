// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_calculation_policy_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateCalculationPolicyRequest extends CreateCalculationPolicyRequest {
  @override
  final String? branchId;
  @override
  final String policyCode;
  @override
  final int? version;
  @override
  final String overheadMethod;
  @override
  final double? overheadValue;
  @override
  final String roundingMode;
  @override
  final String unitConversionMode;
  @override
  final DateTime? effectiveFromUtc;
  @override
  final DateTime? effectiveToUtc;

  factory _$CreateCalculationPolicyRequest(
          [void Function(CreateCalculationPolicyRequestBuilder)? updates]) =>
      (CreateCalculationPolicyRequestBuilder()..update(updates))._build();

  _$CreateCalculationPolicyRequest._(
      {this.branchId,
      required this.policyCode,
      this.version,
      required this.overheadMethod,
      this.overheadValue,
      required this.roundingMode,
      required this.unitConversionMode,
      this.effectiveFromUtc,
      this.effectiveToUtc})
      : super._();
  @override
  CreateCalculationPolicyRequest rebuild(
          void Function(CreateCalculationPolicyRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateCalculationPolicyRequestBuilder toBuilder() =>
      CreateCalculationPolicyRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateCalculationPolicyRequest &&
        branchId == other.branchId &&
        policyCode == other.policyCode &&
        version == other.version &&
        overheadMethod == other.overheadMethod &&
        overheadValue == other.overheadValue &&
        roundingMode == other.roundingMode &&
        unitConversionMode == other.unitConversionMode &&
        effectiveFromUtc == other.effectiveFromUtc &&
        effectiveToUtc == other.effectiveToUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, policyCode.hashCode);
    _$hash = $jc(_$hash, version.hashCode);
    _$hash = $jc(_$hash, overheadMethod.hashCode);
    _$hash = $jc(_$hash, overheadValue.hashCode);
    _$hash = $jc(_$hash, roundingMode.hashCode);
    _$hash = $jc(_$hash, unitConversionMode.hashCode);
    _$hash = $jc(_$hash, effectiveFromUtc.hashCode);
    _$hash = $jc(_$hash, effectiveToUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateCalculationPolicyRequest')
          ..add('branchId', branchId)
          ..add('policyCode', policyCode)
          ..add('version', version)
          ..add('overheadMethod', overheadMethod)
          ..add('overheadValue', overheadValue)
          ..add('roundingMode', roundingMode)
          ..add('unitConversionMode', unitConversionMode)
          ..add('effectiveFromUtc', effectiveFromUtc)
          ..add('effectiveToUtc', effectiveToUtc))
        .toString();
  }
}

class CreateCalculationPolicyRequestBuilder
    implements
        Builder<CreateCalculationPolicyRequest,
            CreateCalculationPolicyRequestBuilder> {
  _$CreateCalculationPolicyRequest? _$v;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _policyCode;
  String? get policyCode => _$this._policyCode;
  set policyCode(String? policyCode) => _$this._policyCode = policyCode;

  int? _version;
  int? get version => _$this._version;
  set version(int? version) => _$this._version = version;

  String? _overheadMethod;
  String? get overheadMethod => _$this._overheadMethod;
  set overheadMethod(String? overheadMethod) =>
      _$this._overheadMethod = overheadMethod;

  double? _overheadValue;
  double? get overheadValue => _$this._overheadValue;
  set overheadValue(double? overheadValue) =>
      _$this._overheadValue = overheadValue;

  String? _roundingMode;
  String? get roundingMode => _$this._roundingMode;
  set roundingMode(String? roundingMode) => _$this._roundingMode = roundingMode;

  String? _unitConversionMode;
  String? get unitConversionMode => _$this._unitConversionMode;
  set unitConversionMode(String? unitConversionMode) =>
      _$this._unitConversionMode = unitConversionMode;

  DateTime? _effectiveFromUtc;
  DateTime? get effectiveFromUtc => _$this._effectiveFromUtc;
  set effectiveFromUtc(DateTime? effectiveFromUtc) =>
      _$this._effectiveFromUtc = effectiveFromUtc;

  DateTime? _effectiveToUtc;
  DateTime? get effectiveToUtc => _$this._effectiveToUtc;
  set effectiveToUtc(DateTime? effectiveToUtc) =>
      _$this._effectiveToUtc = effectiveToUtc;

  CreateCalculationPolicyRequestBuilder() {
    CreateCalculationPolicyRequest._defaults(this);
  }

  CreateCalculationPolicyRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _branchId = $v.branchId;
      _policyCode = $v.policyCode;
      _version = $v.version;
      _overheadMethod = $v.overheadMethod;
      _overheadValue = $v.overheadValue;
      _roundingMode = $v.roundingMode;
      _unitConversionMode = $v.unitConversionMode;
      _effectiveFromUtc = $v.effectiveFromUtc;
      _effectiveToUtc = $v.effectiveToUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateCalculationPolicyRequest other) {
    _$v = other as _$CreateCalculationPolicyRequest;
  }

  @override
  void update(void Function(CreateCalculationPolicyRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateCalculationPolicyRequest build() => _build();

  _$CreateCalculationPolicyRequest _build() {
    final _$result = _$v ??
        _$CreateCalculationPolicyRequest._(
          branchId: branchId,
          policyCode: BuiltValueNullFieldError.checkNotNull(
              policyCode, r'CreateCalculationPolicyRequest', 'policyCode'),
          version: version,
          overheadMethod: BuiltValueNullFieldError.checkNotNull(overheadMethod,
              r'CreateCalculationPolicyRequest', 'overheadMethod'),
          overheadValue: overheadValue,
          roundingMode: BuiltValueNullFieldError.checkNotNull(
              roundingMode, r'CreateCalculationPolicyRequest', 'roundingMode'),
          unitConversionMode: BuiltValueNullFieldError.checkNotNull(
              unitConversionMode,
              r'CreateCalculationPolicyRequest',
              'unitConversionMode'),
          effectiveFromUtc: effectiveFromUtc,
          effectiveToUtc: effectiveToUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
