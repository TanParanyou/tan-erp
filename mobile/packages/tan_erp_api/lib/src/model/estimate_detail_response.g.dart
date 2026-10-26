// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_detail_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateDetailResponse extends EstimateDetailResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? branchId;
  @override
  final String? customerId;
  @override
  final String? opportunityId;
  @override
  final String? siteSurveyRevisionId;
  @override
  final String? siteSurveySnapshotHash;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final int? currentRevisionNo;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;
  @override
  final EstimateRevisionResponse? currentRevision;

  factory _$EstimateDetailResponse(
          [void Function(EstimateDetailResponseBuilder)? updates]) =>
      (EstimateDetailResponseBuilder()..update(updates))._build();

  _$EstimateDetailResponse._(
      {this.id,
      this.organizationId,
      this.branchId,
      this.customerId,
      this.opportunityId,
      this.siteSurveyRevisionId,
      this.siteSurveySnapshotHash,
      this.number,
      this.status,
      this.currentRevisionNo,
      this.rowVersion,
      this.createdAtUtc,
      this.updatedAtUtc,
      this.currentRevision})
      : super._();
  @override
  EstimateDetailResponse rebuild(
          void Function(EstimateDetailResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateDetailResponseBuilder toBuilder() =>
      EstimateDetailResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateDetailResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        branchId == other.branchId &&
        customerId == other.customerId &&
        opportunityId == other.opportunityId &&
        siteSurveyRevisionId == other.siteSurveyRevisionId &&
        siteSurveySnapshotHash == other.siteSurveySnapshotHash &&
        number == other.number &&
        status == other.status &&
        currentRevisionNo == other.currentRevisionNo &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc &&
        currentRevision == other.currentRevision;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jc(_$hash, siteSurveyRevisionId.hashCode);
    _$hash = $jc(_$hash, siteSurveySnapshotHash.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, currentRevisionNo.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jc(_$hash, currentRevision.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateDetailResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('branchId', branchId)
          ..add('customerId', customerId)
          ..add('opportunityId', opportunityId)
          ..add('siteSurveyRevisionId', siteSurveyRevisionId)
          ..add('siteSurveySnapshotHash', siteSurveySnapshotHash)
          ..add('number', number)
          ..add('status', status)
          ..add('currentRevisionNo', currentRevisionNo)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc)
          ..add('currentRevision', currentRevision))
        .toString();
  }
}

class EstimateDetailResponseBuilder
    implements Builder<EstimateDetailResponse, EstimateDetailResponseBuilder> {
  _$EstimateDetailResponse? _$v;

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

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  String? _siteSurveyRevisionId;
  String? get siteSurveyRevisionId => _$this._siteSurveyRevisionId;
  set siteSurveyRevisionId(String? siteSurveyRevisionId) =>
      _$this._siteSurveyRevisionId = siteSurveyRevisionId;

  String? _siteSurveySnapshotHash;
  String? get siteSurveySnapshotHash => _$this._siteSurveySnapshotHash;
  set siteSurveySnapshotHash(String? siteSurveySnapshotHash) =>
      _$this._siteSurveySnapshotHash = siteSurveySnapshotHash;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  int? _currentRevisionNo;
  int? get currentRevisionNo => _$this._currentRevisionNo;
  set currentRevisionNo(int? currentRevisionNo) =>
      _$this._currentRevisionNo = currentRevisionNo;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  EstimateRevisionResponseBuilder? _currentRevision;
  EstimateRevisionResponseBuilder get currentRevision =>
      _$this._currentRevision ??= EstimateRevisionResponseBuilder();
  set currentRevision(EstimateRevisionResponseBuilder? currentRevision) =>
      _$this._currentRevision = currentRevision;

  EstimateDetailResponseBuilder() {
    EstimateDetailResponse._defaults(this);
  }

  EstimateDetailResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _branchId = $v.branchId;
      _customerId = $v.customerId;
      _opportunityId = $v.opportunityId;
      _siteSurveyRevisionId = $v.siteSurveyRevisionId;
      _siteSurveySnapshotHash = $v.siteSurveySnapshotHash;
      _number = $v.number;
      _status = $v.status;
      _currentRevisionNo = $v.currentRevisionNo;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _currentRevision = $v.currentRevision?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateDetailResponse other) {
    _$v = other as _$EstimateDetailResponse;
  }

  @override
  void update(void Function(EstimateDetailResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateDetailResponse build() => _build();

  _$EstimateDetailResponse _build() {
    _$EstimateDetailResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateDetailResponse._(
            id: id,
            organizationId: organizationId,
            branchId: branchId,
            customerId: customerId,
            opportunityId: opportunityId,
            siteSurveyRevisionId: siteSurveyRevisionId,
            siteSurveySnapshotHash: siteSurveySnapshotHash,
            number: number,
            status: status,
            currentRevisionNo: currentRevisionNo,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            updatedAtUtc: updatedAtUtc,
            currentRevision: _currentRevision?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'currentRevision';
        _currentRevision?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateDetailResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
