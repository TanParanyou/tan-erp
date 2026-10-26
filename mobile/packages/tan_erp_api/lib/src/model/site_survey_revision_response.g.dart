// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_survey_revision_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteSurveyRevisionResponse extends SiteSurveyRevisionResponse {
  @override
  final String? id;
  @override
  final String? siteSurveyId;
  @override
  final int? revisionNumber;
  @override
  final String? surveyTemplateVersion;
  @override
  final DateTime? visitedAtUtc;
  @override
  final String? scopeSummary;
  @override
  final BuiltList<String>? assumptions;
  @override
  final BuiltList<String>? constraints;
  @override
  final BuiltList<String>? missingDetails;
  @override
  final String? readiness;
  @override
  final String? status;
  @override
  final DateTime? readyAtUtc;
  @override
  final String? readyByUserId;
  @override
  final String? snapshotHash;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? createdByUserId;
  @override
  final BuiltList<SiteSurveyAreaResponse>? areas;
  @override
  final BuiltList<SiteSurveyChecklistResultResponse>? checklist;
  @override
  final BuiltList<SiteSurveyEvidenceResponse>? evidence;

  factory _$SiteSurveyRevisionResponse(
          [void Function(SiteSurveyRevisionResponseBuilder)? updates]) =>
      (SiteSurveyRevisionResponseBuilder()..update(updates))._build();

  _$SiteSurveyRevisionResponse._(
      {this.id,
      this.siteSurveyId,
      this.revisionNumber,
      this.surveyTemplateVersion,
      this.visitedAtUtc,
      this.scopeSummary,
      this.assumptions,
      this.constraints,
      this.missingDetails,
      this.readiness,
      this.status,
      this.readyAtUtc,
      this.readyByUserId,
      this.snapshotHash,
      this.rowVersion,
      this.createdAtUtc,
      this.createdByUserId,
      this.areas,
      this.checklist,
      this.evidence})
      : super._();
  @override
  SiteSurveyRevisionResponse rebuild(
          void Function(SiteSurveyRevisionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteSurveyRevisionResponseBuilder toBuilder() =>
      SiteSurveyRevisionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteSurveyRevisionResponse &&
        id == other.id &&
        siteSurveyId == other.siteSurveyId &&
        revisionNumber == other.revisionNumber &&
        surveyTemplateVersion == other.surveyTemplateVersion &&
        visitedAtUtc == other.visitedAtUtc &&
        scopeSummary == other.scopeSummary &&
        assumptions == other.assumptions &&
        constraints == other.constraints &&
        missingDetails == other.missingDetails &&
        readiness == other.readiness &&
        status == other.status &&
        readyAtUtc == other.readyAtUtc &&
        readyByUserId == other.readyByUserId &&
        snapshotHash == other.snapshotHash &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        createdByUserId == other.createdByUserId &&
        areas == other.areas &&
        checklist == other.checklist &&
        evidence == other.evidence;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, siteSurveyId.hashCode);
    _$hash = $jc(_$hash, revisionNumber.hashCode);
    _$hash = $jc(_$hash, surveyTemplateVersion.hashCode);
    _$hash = $jc(_$hash, visitedAtUtc.hashCode);
    _$hash = $jc(_$hash, scopeSummary.hashCode);
    _$hash = $jc(_$hash, assumptions.hashCode);
    _$hash = $jc(_$hash, constraints.hashCode);
    _$hash = $jc(_$hash, missingDetails.hashCode);
    _$hash = $jc(_$hash, readiness.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, readyAtUtc.hashCode);
    _$hash = $jc(_$hash, readyByUserId.hashCode);
    _$hash = $jc(_$hash, snapshotHash.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, createdByUserId.hashCode);
    _$hash = $jc(_$hash, areas.hashCode);
    _$hash = $jc(_$hash, checklist.hashCode);
    _$hash = $jc(_$hash, evidence.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteSurveyRevisionResponse')
          ..add('id', id)
          ..add('siteSurveyId', siteSurveyId)
          ..add('revisionNumber', revisionNumber)
          ..add('surveyTemplateVersion', surveyTemplateVersion)
          ..add('visitedAtUtc', visitedAtUtc)
          ..add('scopeSummary', scopeSummary)
          ..add('assumptions', assumptions)
          ..add('constraints', constraints)
          ..add('missingDetails', missingDetails)
          ..add('readiness', readiness)
          ..add('status', status)
          ..add('readyAtUtc', readyAtUtc)
          ..add('readyByUserId', readyByUserId)
          ..add('snapshotHash', snapshotHash)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('createdByUserId', createdByUserId)
          ..add('areas', areas)
          ..add('checklist', checklist)
          ..add('evidence', evidence))
        .toString();
  }
}

class SiteSurveyRevisionResponseBuilder
    implements
        Builder<SiteSurveyRevisionResponse, SiteSurveyRevisionResponseBuilder> {
  _$SiteSurveyRevisionResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _siteSurveyId;
  String? get siteSurveyId => _$this._siteSurveyId;
  set siteSurveyId(String? siteSurveyId) => _$this._siteSurveyId = siteSurveyId;

  int? _revisionNumber;
  int? get revisionNumber => _$this._revisionNumber;
  set revisionNumber(int? revisionNumber) =>
      _$this._revisionNumber = revisionNumber;

  String? _surveyTemplateVersion;
  String? get surveyTemplateVersion => _$this._surveyTemplateVersion;
  set surveyTemplateVersion(String? surveyTemplateVersion) =>
      _$this._surveyTemplateVersion = surveyTemplateVersion;

  DateTime? _visitedAtUtc;
  DateTime? get visitedAtUtc => _$this._visitedAtUtc;
  set visitedAtUtc(DateTime? visitedAtUtc) =>
      _$this._visitedAtUtc = visitedAtUtc;

  String? _scopeSummary;
  String? get scopeSummary => _$this._scopeSummary;
  set scopeSummary(String? scopeSummary) => _$this._scopeSummary = scopeSummary;

  ListBuilder<String>? _assumptions;
  ListBuilder<String> get assumptions =>
      _$this._assumptions ??= ListBuilder<String>();
  set assumptions(ListBuilder<String>? assumptions) =>
      _$this._assumptions = assumptions;

  ListBuilder<String>? _constraints;
  ListBuilder<String> get constraints =>
      _$this._constraints ??= ListBuilder<String>();
  set constraints(ListBuilder<String>? constraints) =>
      _$this._constraints = constraints;

  ListBuilder<String>? _missingDetails;
  ListBuilder<String> get missingDetails =>
      _$this._missingDetails ??= ListBuilder<String>();
  set missingDetails(ListBuilder<String>? missingDetails) =>
      _$this._missingDetails = missingDetails;

  String? _readiness;
  String? get readiness => _$this._readiness;
  set readiness(String? readiness) => _$this._readiness = readiness;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  DateTime? _readyAtUtc;
  DateTime? get readyAtUtc => _$this._readyAtUtc;
  set readyAtUtc(DateTime? readyAtUtc) => _$this._readyAtUtc = readyAtUtc;

  String? _readyByUserId;
  String? get readyByUserId => _$this._readyByUserId;
  set readyByUserId(String? readyByUserId) =>
      _$this._readyByUserId = readyByUserId;

  String? _snapshotHash;
  String? get snapshotHash => _$this._snapshotHash;
  set snapshotHash(String? snapshotHash) => _$this._snapshotHash = snapshotHash;

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

  ListBuilder<SiteSurveyAreaResponse>? _areas;
  ListBuilder<SiteSurveyAreaResponse> get areas =>
      _$this._areas ??= ListBuilder<SiteSurveyAreaResponse>();
  set areas(ListBuilder<SiteSurveyAreaResponse>? areas) =>
      _$this._areas = areas;

  ListBuilder<SiteSurveyChecklistResultResponse>? _checklist;
  ListBuilder<SiteSurveyChecklistResultResponse> get checklist =>
      _$this._checklist ??= ListBuilder<SiteSurveyChecklistResultResponse>();
  set checklist(ListBuilder<SiteSurveyChecklistResultResponse>? checklist) =>
      _$this._checklist = checklist;

  ListBuilder<SiteSurveyEvidenceResponse>? _evidence;
  ListBuilder<SiteSurveyEvidenceResponse> get evidence =>
      _$this._evidence ??= ListBuilder<SiteSurveyEvidenceResponse>();
  set evidence(ListBuilder<SiteSurveyEvidenceResponse>? evidence) =>
      _$this._evidence = evidence;

  SiteSurveyRevisionResponseBuilder() {
    SiteSurveyRevisionResponse._defaults(this);
  }

  SiteSurveyRevisionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _siteSurveyId = $v.siteSurveyId;
      _revisionNumber = $v.revisionNumber;
      _surveyTemplateVersion = $v.surveyTemplateVersion;
      _visitedAtUtc = $v.visitedAtUtc;
      _scopeSummary = $v.scopeSummary;
      _assumptions = $v.assumptions?.toBuilder();
      _constraints = $v.constraints?.toBuilder();
      _missingDetails = $v.missingDetails?.toBuilder();
      _readiness = $v.readiness;
      _status = $v.status;
      _readyAtUtc = $v.readyAtUtc;
      _readyByUserId = $v.readyByUserId;
      _snapshotHash = $v.snapshotHash;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _createdByUserId = $v.createdByUserId;
      _areas = $v.areas?.toBuilder();
      _checklist = $v.checklist?.toBuilder();
      _evidence = $v.evidence?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteSurveyRevisionResponse other) {
    _$v = other as _$SiteSurveyRevisionResponse;
  }

  @override
  void update(void Function(SiteSurveyRevisionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteSurveyRevisionResponse build() => _build();

  _$SiteSurveyRevisionResponse _build() {
    _$SiteSurveyRevisionResponse _$result;
    try {
      _$result = _$v ??
          _$SiteSurveyRevisionResponse._(
            id: id,
            siteSurveyId: siteSurveyId,
            revisionNumber: revisionNumber,
            surveyTemplateVersion: surveyTemplateVersion,
            visitedAtUtc: visitedAtUtc,
            scopeSummary: scopeSummary,
            assumptions: _assumptions?.build(),
            constraints: _constraints?.build(),
            missingDetails: _missingDetails?.build(),
            readiness: readiness,
            status: status,
            readyAtUtc: readyAtUtc,
            readyByUserId: readyByUserId,
            snapshotHash: snapshotHash,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            createdByUserId: createdByUserId,
            areas: _areas?.build(),
            checklist: _checklist?.build(),
            evidence: _evidence?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'assumptions';
        _assumptions?.build();
        _$failedField = 'constraints';
        _constraints?.build();
        _$failedField = 'missingDetails';
        _missingDetails?.build();

        _$failedField = 'areas';
        _areas?.build();
        _$failedField = 'checklist';
        _checklist?.build();
        _$failedField = 'evidence';
        _evidence?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SiteSurveyRevisionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
