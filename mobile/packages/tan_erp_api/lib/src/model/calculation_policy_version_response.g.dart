// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calculation_policy_version_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CalculationPolicyVersionResponse
    extends CalculationPolicyVersionResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? branchId;
  @override
  final String? policyCode;
  @override
  final int? version;
  @override
  final String? status;
  @override
  final String? overheadMethod;
  @override
  final double? overheadValue;
  @override
  final String? roundingMode;
  @override
  final String? unitConversionMode;
  @override
  final DateTime? effectiveFromUtc;
  @override
  final DateTime? effectiveToUtc;
  @override
  final String? createdByUserId;
  @override
  final String? publishedByUserId;
  @override
  final DateTime? publishedAtUtc;
  @override
  final String? contentHash;

  factory _$CalculationPolicyVersionResponse(
          [void Function(CalculationPolicyVersionResponseBuilder)? updates]) =>
      (CalculationPolicyVersionResponseBuilder()..update(updates))._build();

  _$CalculationPolicyVersionResponse._(
      {this.id,
      this.organizationId,
      this.branchId,
      this.policyCode,
      this.version,
      this.status,
      this.overheadMethod,
      this.overheadValue,
      this.roundingMode,
      this.unitConversionMode,
      this.effectiveFromUtc,
      this.effectiveToUtc,
      this.createdByUserId,
      this.publishedByUserId,
      this.publishedAtUtc,
      this.contentHash})
      : super._();
  @override
  CalculationPolicyVersionResponse rebuild(
          void Function(CalculationPolicyVersionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CalculationPolicyVersionResponseBuilder toBuilder() =>
      CalculationPolicyVersionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CalculationPolicyVersionResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        branchId == other.branchId &&
        policyCode == other.policyCode &&
        version == other.version &&
        status == other.status &&
        overheadMethod == other.overheadMethod &&
        overheadValue == other.overheadValue &&
        roundingMode == other.roundingMode &&
        unitConversionMode == other.unitConversionMode &&
        effectiveFromUtc == other.effectiveFromUtc &&
        effectiveToUtc == other.effectiveToUtc &&
        createdByUserId == other.createdByUserId &&
        publishedByUserId == other.publishedByUserId &&
        publishedAtUtc == other.publishedAtUtc &&
        contentHash == other.contentHash;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, policyCode.hashCode);
    _$hash = $jc(_$hash, version.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, overheadMethod.hashCode);
    _$hash = $jc(_$hash, overheadValue.hashCode);
    _$hash = $jc(_$hash, roundingMode.hashCode);
    _$hash = $jc(_$hash, unitConversionMode.hashCode);
    _$hash = $jc(_$hash, effectiveFromUtc.hashCode);
    _$hash = $jc(_$hash, effectiveToUtc.hashCode);
    _$hash = $jc(_$hash, createdByUserId.hashCode);
    _$hash = $jc(_$hash, publishedByUserId.hashCode);
    _$hash = $jc(_$hash, publishedAtUtc.hashCode);
    _$hash = $jc(_$hash, contentHash.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CalculationPolicyVersionResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('branchId', branchId)
          ..add('policyCode', policyCode)
          ..add('version', version)
          ..add('status', status)
          ..add('overheadMethod', overheadMethod)
          ..add('overheadValue', overheadValue)
          ..add('roundingMode', roundingMode)
          ..add('unitConversionMode', unitConversionMode)
          ..add('effectiveFromUtc', effectiveFromUtc)
          ..add('effectiveToUtc', effectiveToUtc)
          ..add('createdByUserId', createdByUserId)
          ..add('publishedByUserId', publishedByUserId)
          ..add('publishedAtUtc', publishedAtUtc)
          ..add('contentHash', contentHash))
        .toString();
  }
}

class CalculationPolicyVersionResponseBuilder
    implements
        Builder<CalculationPolicyVersionResponse,
            CalculationPolicyVersionResponseBuilder> {
  _$CalculationPolicyVersionResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _organizationId;
  String? get organizationId => _$this._organizationId;
  set organizationId(String? organizationId) =>
      _$this._organizationId = organizationId;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _policyCode;
  String? get policyCode => _$this._policyCode;
  set policyCode(String? policyCode) => _$this._policyCode = policyCode;

  int? _version;
  int? get version => _$this._version;
  set version(int? version) => _$this._version = version;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

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

  String? _createdByUserId;
  String? get createdByUserId => _$this._createdByUserId;
  set createdByUserId(String? createdByUserId) =>
      _$this._createdByUserId = createdByUserId;

  String? _publishedByUserId;
  String? get publishedByUserId => _$this._publishedByUserId;
  set publishedByUserId(String? publishedByUserId) =>
      _$this._publishedByUserId = publishedByUserId;

  DateTime? _publishedAtUtc;
  DateTime? get publishedAtUtc => _$this._publishedAtUtc;
  set publishedAtUtc(DateTime? publishedAtUtc) =>
      _$this._publishedAtUtc = publishedAtUtc;

  String? _contentHash;
  String? get contentHash => _$this._contentHash;
  set contentHash(String? contentHash) => _$this._contentHash = contentHash;

  CalculationPolicyVersionResponseBuilder() {
    CalculationPolicyVersionResponse._defaults(this);
  }

  CalculationPolicyVersionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _branchId = $v.branchId;
      _policyCode = $v.policyCode;
      _version = $v.version;
      _status = $v.status;
      _overheadMethod = $v.overheadMethod;
      _overheadValue = $v.overheadValue;
      _roundingMode = $v.roundingMode;
      _unitConversionMode = $v.unitConversionMode;
      _effectiveFromUtc = $v.effectiveFromUtc;
      _effectiveToUtc = $v.effectiveToUtc;
      _createdByUserId = $v.createdByUserId;
      _publishedByUserId = $v.publishedByUserId;
      _publishedAtUtc = $v.publishedAtUtc;
      _contentHash = $v.contentHash;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CalculationPolicyVersionResponse other) {
    _$v = other as _$CalculationPolicyVersionResponse;
  }

  @override
  void update(void Function(CalculationPolicyVersionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CalculationPolicyVersionResponse build() => _build();

  _$CalculationPolicyVersionResponse _build() {
    final _$result = _$v ??
        _$CalculationPolicyVersionResponse._(
          id: id,
          organizationId: organizationId,
          branchId: branchId,
          policyCode: policyCode,
          version: version,
          status: status,
          overheadMethod: overheadMethod,
          overheadValue: overheadValue,
          roundingMode: roundingMode,
          unitConversionMode: unitConversionMode,
          effectiveFromUtc: effectiveFromUtc,
          effectiveToUtc: effectiveToUtc,
          createdByUserId: createdByUserId,
          publishedByUserId: publishedByUserId,
          publishedAtUtc: publishedAtUtc,
          contentHash: contentHash,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
