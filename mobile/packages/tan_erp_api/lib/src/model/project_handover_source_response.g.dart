// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_handover_source_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectHandoverSourceResponse extends ProjectHandoverSourceResponse {
  @override
  final String? quotationId;
  @override
  final String? quotationNumber;
  @override
  final String? quotationStatus;
  @override
  final String? quotationRowVersion;
  @override
  final double? contractAmount;
  @override
  final String? opportunityStage;
  @override
  final String? existingProjectId;
  @override
  final String? existingProjectCode;

  factory _$ProjectHandoverSourceResponse(
          [void Function(ProjectHandoverSourceResponseBuilder)? updates]) =>
      (ProjectHandoverSourceResponseBuilder()..update(updates))._build();

  _$ProjectHandoverSourceResponse._(
      {this.quotationId,
      this.quotationNumber,
      this.quotationStatus,
      this.quotationRowVersion,
      this.contractAmount,
      this.opportunityStage,
      this.existingProjectId,
      this.existingProjectCode})
      : super._();
  @override
  ProjectHandoverSourceResponse rebuild(
          void Function(ProjectHandoverSourceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectHandoverSourceResponseBuilder toBuilder() =>
      ProjectHandoverSourceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectHandoverSourceResponse &&
        quotationId == other.quotationId &&
        quotationNumber == other.quotationNumber &&
        quotationStatus == other.quotationStatus &&
        quotationRowVersion == other.quotationRowVersion &&
        contractAmount == other.contractAmount &&
        opportunityStage == other.opportunityStage &&
        existingProjectId == other.existingProjectId &&
        existingProjectCode == other.existingProjectCode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, quotationId.hashCode);
    _$hash = $jc(_$hash, quotationNumber.hashCode);
    _$hash = $jc(_$hash, quotationStatus.hashCode);
    _$hash = $jc(_$hash, quotationRowVersion.hashCode);
    _$hash = $jc(_$hash, contractAmount.hashCode);
    _$hash = $jc(_$hash, opportunityStage.hashCode);
    _$hash = $jc(_$hash, existingProjectId.hashCode);
    _$hash = $jc(_$hash, existingProjectCode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectHandoverSourceResponse')
          ..add('quotationId', quotationId)
          ..add('quotationNumber', quotationNumber)
          ..add('quotationStatus', quotationStatus)
          ..add('quotationRowVersion', quotationRowVersion)
          ..add('contractAmount', contractAmount)
          ..add('opportunityStage', opportunityStage)
          ..add('existingProjectId', existingProjectId)
          ..add('existingProjectCode', existingProjectCode))
        .toString();
  }
}

class ProjectHandoverSourceResponseBuilder
    implements
        Builder<ProjectHandoverSourceResponse,
            ProjectHandoverSourceResponseBuilder> {
  _$ProjectHandoverSourceResponse? _$v;

  String? _quotationId;
  String? get quotationId => _$this._quotationId;
  set quotationId(String? quotationId) => _$this._quotationId = quotationId;

  String? _quotationNumber;
  String? get quotationNumber => _$this._quotationNumber;
  set quotationNumber(String? quotationNumber) =>
      _$this._quotationNumber = quotationNumber;

  String? _quotationStatus;
  String? get quotationStatus => _$this._quotationStatus;
  set quotationStatus(String? quotationStatus) =>
      _$this._quotationStatus = quotationStatus;

  String? _quotationRowVersion;
  String? get quotationRowVersion => _$this._quotationRowVersion;
  set quotationRowVersion(String? quotationRowVersion) =>
      _$this._quotationRowVersion = quotationRowVersion;

  double? _contractAmount;
  double? get contractAmount => _$this._contractAmount;
  set contractAmount(double? contractAmount) =>
      _$this._contractAmount = contractAmount;

  String? _opportunityStage;
  String? get opportunityStage => _$this._opportunityStage;
  set opportunityStage(String? opportunityStage) =>
      _$this._opportunityStage = opportunityStage;

  String? _existingProjectId;
  String? get existingProjectId => _$this._existingProjectId;
  set existingProjectId(String? existingProjectId) =>
      _$this._existingProjectId = existingProjectId;

  String? _existingProjectCode;
  String? get existingProjectCode => _$this._existingProjectCode;
  set existingProjectCode(String? existingProjectCode) =>
      _$this._existingProjectCode = existingProjectCode;

  ProjectHandoverSourceResponseBuilder() {
    ProjectHandoverSourceResponse._defaults(this);
  }

  ProjectHandoverSourceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _quotationId = $v.quotationId;
      _quotationNumber = $v.quotationNumber;
      _quotationStatus = $v.quotationStatus;
      _quotationRowVersion = $v.quotationRowVersion;
      _contractAmount = $v.contractAmount;
      _opportunityStage = $v.opportunityStage;
      _existingProjectId = $v.existingProjectId;
      _existingProjectCode = $v.existingProjectCode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectHandoverSourceResponse other) {
    _$v = other as _$ProjectHandoverSourceResponse;
  }

  @override
  void update(void Function(ProjectHandoverSourceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectHandoverSourceResponse build() => _build();

  _$ProjectHandoverSourceResponse _build() {
    final _$result = _$v ??
        _$ProjectHandoverSourceResponse._(
          quotationId: quotationId,
          quotationNumber: quotationNumber,
          quotationStatus: quotationStatus,
          quotationRowVersion: quotationRowVersion,
          contractAmount: contractAmount,
          opportunityStage: opportunityStage,
          existingProjectId: existingProjectId,
          existingProjectCode: existingProjectCode,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
