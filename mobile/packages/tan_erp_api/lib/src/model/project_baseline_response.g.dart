// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_baseline_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBaselineResponse extends ProjectBaselineResponse {
  @override
  final String? quotationId;
  @override
  final String? quotationNumber;
  @override
  final double? contractAmount;
  @override
  final String? quotationSnapshotHash;
  @override
  final String? estimateId;
  @override
  final String? estimateRevisionId;
  @override
  final String? siteSurveyRevisionId;
  @override
  final String? siteSurveySnapshotHash;
  @override
  final String? baselineHash;

  factory _$ProjectBaselineResponse(
          [void Function(ProjectBaselineResponseBuilder)? updates]) =>
      (ProjectBaselineResponseBuilder()..update(updates))._build();

  _$ProjectBaselineResponse._(
      {this.quotationId,
      this.quotationNumber,
      this.contractAmount,
      this.quotationSnapshotHash,
      this.estimateId,
      this.estimateRevisionId,
      this.siteSurveyRevisionId,
      this.siteSurveySnapshotHash,
      this.baselineHash})
      : super._();
  @override
  ProjectBaselineResponse rebuild(
          void Function(ProjectBaselineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBaselineResponseBuilder toBuilder() =>
      ProjectBaselineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBaselineResponse &&
        quotationId == other.quotationId &&
        quotationNumber == other.quotationNumber &&
        contractAmount == other.contractAmount &&
        quotationSnapshotHash == other.quotationSnapshotHash &&
        estimateId == other.estimateId &&
        estimateRevisionId == other.estimateRevisionId &&
        siteSurveyRevisionId == other.siteSurveyRevisionId &&
        siteSurveySnapshotHash == other.siteSurveySnapshotHash &&
        baselineHash == other.baselineHash;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, quotationId.hashCode);
    _$hash = $jc(_$hash, quotationNumber.hashCode);
    _$hash = $jc(_$hash, contractAmount.hashCode);
    _$hash = $jc(_$hash, quotationSnapshotHash.hashCode);
    _$hash = $jc(_$hash, estimateId.hashCode);
    _$hash = $jc(_$hash, estimateRevisionId.hashCode);
    _$hash = $jc(_$hash, siteSurveyRevisionId.hashCode);
    _$hash = $jc(_$hash, siteSurveySnapshotHash.hashCode);
    _$hash = $jc(_$hash, baselineHash.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectBaselineResponse')
          ..add('quotationId', quotationId)
          ..add('quotationNumber', quotationNumber)
          ..add('contractAmount', contractAmount)
          ..add('quotationSnapshotHash', quotationSnapshotHash)
          ..add('estimateId', estimateId)
          ..add('estimateRevisionId', estimateRevisionId)
          ..add('siteSurveyRevisionId', siteSurveyRevisionId)
          ..add('siteSurveySnapshotHash', siteSurveySnapshotHash)
          ..add('baselineHash', baselineHash))
        .toString();
  }
}

class ProjectBaselineResponseBuilder
    implements
        Builder<ProjectBaselineResponse, ProjectBaselineResponseBuilder> {
  _$ProjectBaselineResponse? _$v;

  String? _quotationId;
  String? get quotationId => _$this._quotationId;
  set quotationId(String? quotationId) => _$this._quotationId = quotationId;

  String? _quotationNumber;
  String? get quotationNumber => _$this._quotationNumber;
  set quotationNumber(String? quotationNumber) =>
      _$this._quotationNumber = quotationNumber;

  double? _contractAmount;
  double? get contractAmount => _$this._contractAmount;
  set contractAmount(double? contractAmount) =>
      _$this._contractAmount = contractAmount;

  String? _quotationSnapshotHash;
  String? get quotationSnapshotHash => _$this._quotationSnapshotHash;
  set quotationSnapshotHash(String? quotationSnapshotHash) =>
      _$this._quotationSnapshotHash = quotationSnapshotHash;

  String? _estimateId;
  String? get estimateId => _$this._estimateId;
  set estimateId(String? estimateId) => _$this._estimateId = estimateId;

  String? _estimateRevisionId;
  String? get estimateRevisionId => _$this._estimateRevisionId;
  set estimateRevisionId(String? estimateRevisionId) =>
      _$this._estimateRevisionId = estimateRevisionId;

  String? _siteSurveyRevisionId;
  String? get siteSurveyRevisionId => _$this._siteSurveyRevisionId;
  set siteSurveyRevisionId(String? siteSurveyRevisionId) =>
      _$this._siteSurveyRevisionId = siteSurveyRevisionId;

  String? _siteSurveySnapshotHash;
  String? get siteSurveySnapshotHash => _$this._siteSurveySnapshotHash;
  set siteSurveySnapshotHash(String? siteSurveySnapshotHash) =>
      _$this._siteSurveySnapshotHash = siteSurveySnapshotHash;

  String? _baselineHash;
  String? get baselineHash => _$this._baselineHash;
  set baselineHash(String? baselineHash) => _$this._baselineHash = baselineHash;

  ProjectBaselineResponseBuilder() {
    ProjectBaselineResponse._defaults(this);
  }

  ProjectBaselineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _quotationId = $v.quotationId;
      _quotationNumber = $v.quotationNumber;
      _contractAmount = $v.contractAmount;
      _quotationSnapshotHash = $v.quotationSnapshotHash;
      _estimateId = $v.estimateId;
      _estimateRevisionId = $v.estimateRevisionId;
      _siteSurveyRevisionId = $v.siteSurveyRevisionId;
      _siteSurveySnapshotHash = $v.siteSurveySnapshotHash;
      _baselineHash = $v.baselineHash;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBaselineResponse other) {
    _$v = other as _$ProjectBaselineResponse;
  }

  @override
  void update(void Function(ProjectBaselineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBaselineResponse build() => _build();

  _$ProjectBaselineResponse _build() {
    final _$result = _$v ??
        _$ProjectBaselineResponse._(
          quotationId: quotationId,
          quotationNumber: quotationNumber,
          contractAmount: contractAmount,
          quotationSnapshotHash: quotationSnapshotHash,
          estimateId: estimateId,
          estimateRevisionId: estimateRevisionId,
          siteSurveyRevisionId: siteSurveyRevisionId,
          siteSurveySnapshotHash: siteSurveySnapshotHash,
          baselineHash: baselineHash,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
