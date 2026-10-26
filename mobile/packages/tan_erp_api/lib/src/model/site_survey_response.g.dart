// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_survey_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteSurveyResponse extends SiteSurveyResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? branchId;
  @override
  final String? opportunityId;
  @override
  final String? siteId;
  @override
  final String? surveyNumber;
  @override
  final String? assignedSurveyorId;
  @override
  final DateTime? scheduledStartUtc;
  @override
  final DateTime? scheduledEndUtc;
  @override
  final String? status;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? createdByUserId;
  @override
  final SiteSurveyRevisionResponse? currentRevision;
  @override
  final SurveyorSummaryResponse? assignedSurveyor;
  @override
  final SiteSummaryResponse? site;
  @override
  final SiteSurveyReadyRevisionResponse? latestReadyRevision;

  factory _$SiteSurveyResponse(
          [void Function(SiteSurveyResponseBuilder)? updates]) =>
      (SiteSurveyResponseBuilder()..update(updates))._build();

  _$SiteSurveyResponse._(
      {this.id,
      this.organizationId,
      this.branchId,
      this.opportunityId,
      this.siteId,
      this.surveyNumber,
      this.assignedSurveyorId,
      this.scheduledStartUtc,
      this.scheduledEndUtc,
      this.status,
      this.rowVersion,
      this.createdAtUtc,
      this.createdByUserId,
      this.currentRevision,
      this.assignedSurveyor,
      this.site,
      this.latestReadyRevision})
      : super._();
  @override
  SiteSurveyResponse rebuild(
          void Function(SiteSurveyResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteSurveyResponseBuilder toBuilder() =>
      SiteSurveyResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteSurveyResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        branchId == other.branchId &&
        opportunityId == other.opportunityId &&
        siteId == other.siteId &&
        surveyNumber == other.surveyNumber &&
        assignedSurveyorId == other.assignedSurveyorId &&
        scheduledStartUtc == other.scheduledStartUtc &&
        scheduledEndUtc == other.scheduledEndUtc &&
        status == other.status &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        createdByUserId == other.createdByUserId &&
        currentRevision == other.currentRevision &&
        assignedSurveyor == other.assignedSurveyor &&
        site == other.site &&
        latestReadyRevision == other.latestReadyRevision;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jc(_$hash, siteId.hashCode);
    _$hash = $jc(_$hash, surveyNumber.hashCode);
    _$hash = $jc(_$hash, assignedSurveyorId.hashCode);
    _$hash = $jc(_$hash, scheduledStartUtc.hashCode);
    _$hash = $jc(_$hash, scheduledEndUtc.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, createdByUserId.hashCode);
    _$hash = $jc(_$hash, currentRevision.hashCode);
    _$hash = $jc(_$hash, assignedSurveyor.hashCode);
    _$hash = $jc(_$hash, site.hashCode);
    _$hash = $jc(_$hash, latestReadyRevision.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteSurveyResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('branchId', branchId)
          ..add('opportunityId', opportunityId)
          ..add('siteId', siteId)
          ..add('surveyNumber', surveyNumber)
          ..add('assignedSurveyorId', assignedSurveyorId)
          ..add('scheduledStartUtc', scheduledStartUtc)
          ..add('scheduledEndUtc', scheduledEndUtc)
          ..add('status', status)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('createdByUserId', createdByUserId)
          ..add('currentRevision', currentRevision)
          ..add('assignedSurveyor', assignedSurveyor)
          ..add('site', site)
          ..add('latestReadyRevision', latestReadyRevision))
        .toString();
  }
}

class SiteSurveyResponseBuilder
    implements Builder<SiteSurveyResponse, SiteSurveyResponseBuilder> {
  _$SiteSurveyResponse? _$v;

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

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  String? _siteId;
  String? get siteId => _$this._siteId;
  set siteId(String? siteId) => _$this._siteId = siteId;

  String? _surveyNumber;
  String? get surveyNumber => _$this._surveyNumber;
  set surveyNumber(String? surveyNumber) => _$this._surveyNumber = surveyNumber;

  String? _assignedSurveyorId;
  String? get assignedSurveyorId => _$this._assignedSurveyorId;
  set assignedSurveyorId(String? assignedSurveyorId) =>
      _$this._assignedSurveyorId = assignedSurveyorId;

  DateTime? _scheduledStartUtc;
  DateTime? get scheduledStartUtc => _$this._scheduledStartUtc;
  set scheduledStartUtc(DateTime? scheduledStartUtc) =>
      _$this._scheduledStartUtc = scheduledStartUtc;

  DateTime? _scheduledEndUtc;
  DateTime? get scheduledEndUtc => _$this._scheduledEndUtc;
  set scheduledEndUtc(DateTime? scheduledEndUtc) =>
      _$this._scheduledEndUtc = scheduledEndUtc;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _createdByUserId;
  String? get createdByUserId => _$this._createdByUserId;
  set createdByUserId(String? createdByUserId) =>
      _$this._createdByUserId = createdByUserId;

  SiteSurveyRevisionResponseBuilder? _currentRevision;
  SiteSurveyRevisionResponseBuilder get currentRevision =>
      _$this._currentRevision ??= SiteSurveyRevisionResponseBuilder();
  set currentRevision(SiteSurveyRevisionResponseBuilder? currentRevision) =>
      _$this._currentRevision = currentRevision;

  SurveyorSummaryResponseBuilder? _assignedSurveyor;
  SurveyorSummaryResponseBuilder get assignedSurveyor =>
      _$this._assignedSurveyor ??= SurveyorSummaryResponseBuilder();
  set assignedSurveyor(SurveyorSummaryResponseBuilder? assignedSurveyor) =>
      _$this._assignedSurveyor = assignedSurveyor;

  SiteSummaryResponseBuilder? _site;
  SiteSummaryResponseBuilder get site =>
      _$this._site ??= SiteSummaryResponseBuilder();
  set site(SiteSummaryResponseBuilder? site) => _$this._site = site;

  SiteSurveyReadyRevisionResponseBuilder? _latestReadyRevision;
  SiteSurveyReadyRevisionResponseBuilder get latestReadyRevision =>
      _$this._latestReadyRevision ??= SiteSurveyReadyRevisionResponseBuilder();
  set latestReadyRevision(
          SiteSurveyReadyRevisionResponseBuilder? latestReadyRevision) =>
      _$this._latestReadyRevision = latestReadyRevision;

  SiteSurveyResponseBuilder() {
    SiteSurveyResponse._defaults(this);
  }

  SiteSurveyResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _branchId = $v.branchId;
      _opportunityId = $v.opportunityId;
      _siteId = $v.siteId;
      _surveyNumber = $v.surveyNumber;
      _assignedSurveyorId = $v.assignedSurveyorId;
      _scheduledStartUtc = $v.scheduledStartUtc;
      _scheduledEndUtc = $v.scheduledEndUtc;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _createdByUserId = $v.createdByUserId;
      _currentRevision = $v.currentRevision?.toBuilder();
      _assignedSurveyor = $v.assignedSurveyor?.toBuilder();
      _site = $v.site?.toBuilder();
      _latestReadyRevision = $v.latestReadyRevision?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteSurveyResponse other) {
    _$v = other as _$SiteSurveyResponse;
  }

  @override
  void update(void Function(SiteSurveyResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteSurveyResponse build() => _build();

  _$SiteSurveyResponse _build() {
    _$SiteSurveyResponse _$result;
    try {
      _$result = _$v ??
          _$SiteSurveyResponse._(
            id: id,
            organizationId: organizationId,
            branchId: branchId,
            opportunityId: opportunityId,
            siteId: siteId,
            surveyNumber: surveyNumber,
            assignedSurveyorId: assignedSurveyorId,
            scheduledStartUtc: scheduledStartUtc,
            scheduledEndUtc: scheduledEndUtc,
            status: status,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            createdByUserId: createdByUserId,
            currentRevision: _currentRevision?.build(),
            assignedSurveyor: _assignedSurveyor?.build(),
            site: _site?.build(),
            latestReadyRevision: _latestReadyRevision?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'currentRevision';
        _currentRevision?.build();
        _$failedField = 'assignedSurveyor';
        _assignedSurveyor?.build();
        _$failedField = 'site';
        _site?.build();
        _$failedField = 'latestReadyRevision';
        _latestReadyRevision?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SiteSurveyResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
