// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_survey_draft_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateSurveyDraftRequest extends UpdateSurveyDraftRequest {
  @override
  final String? expectedRevisionVersion;
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
  final BuiltList<UpdateSurveyAreaRequest>? areas;
  @override
  final BuiltList<UpdateSurveyChecklistRequest>? checklist;
  @override
  final BuiltList<UpdateSurveyEvidenceRequest>? evidence;

  factory _$UpdateSurveyDraftRequest(
          [void Function(UpdateSurveyDraftRequestBuilder)? updates]) =>
      (UpdateSurveyDraftRequestBuilder()..update(updates))._build();

  _$UpdateSurveyDraftRequest._(
      {this.expectedRevisionVersion,
      this.visitedAtUtc,
      this.scopeSummary,
      this.assumptions,
      this.constraints,
      this.missingDetails,
      this.areas,
      this.checklist,
      this.evidence})
      : super._();
  @override
  UpdateSurveyDraftRequest rebuild(
          void Function(UpdateSurveyDraftRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateSurveyDraftRequestBuilder toBuilder() =>
      UpdateSurveyDraftRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateSurveyDraftRequest &&
        expectedRevisionVersion == other.expectedRevisionVersion &&
        visitedAtUtc == other.visitedAtUtc &&
        scopeSummary == other.scopeSummary &&
        assumptions == other.assumptions &&
        constraints == other.constraints &&
        missingDetails == other.missingDetails &&
        areas == other.areas &&
        checklist == other.checklist &&
        evidence == other.evidence;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedRevisionVersion.hashCode);
    _$hash = $jc(_$hash, visitedAtUtc.hashCode);
    _$hash = $jc(_$hash, scopeSummary.hashCode);
    _$hash = $jc(_$hash, assumptions.hashCode);
    _$hash = $jc(_$hash, constraints.hashCode);
    _$hash = $jc(_$hash, missingDetails.hashCode);
    _$hash = $jc(_$hash, areas.hashCode);
    _$hash = $jc(_$hash, checklist.hashCode);
    _$hash = $jc(_$hash, evidence.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateSurveyDraftRequest')
          ..add('expectedRevisionVersion', expectedRevisionVersion)
          ..add('visitedAtUtc', visitedAtUtc)
          ..add('scopeSummary', scopeSummary)
          ..add('assumptions', assumptions)
          ..add('constraints', constraints)
          ..add('missingDetails', missingDetails)
          ..add('areas', areas)
          ..add('checklist', checklist)
          ..add('evidence', evidence))
        .toString();
  }
}

class UpdateSurveyDraftRequestBuilder
    implements
        Builder<UpdateSurveyDraftRequest, UpdateSurveyDraftRequestBuilder> {
  _$UpdateSurveyDraftRequest? _$v;

  String? _expectedRevisionVersion;
  String? get expectedRevisionVersion => _$this._expectedRevisionVersion;
  set expectedRevisionVersion(String? expectedRevisionVersion) =>
      _$this._expectedRevisionVersion = expectedRevisionVersion;

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

  ListBuilder<UpdateSurveyAreaRequest>? _areas;
  ListBuilder<UpdateSurveyAreaRequest> get areas =>
      _$this._areas ??= ListBuilder<UpdateSurveyAreaRequest>();
  set areas(ListBuilder<UpdateSurveyAreaRequest>? areas) =>
      _$this._areas = areas;

  ListBuilder<UpdateSurveyChecklistRequest>? _checklist;
  ListBuilder<UpdateSurveyChecklistRequest> get checklist =>
      _$this._checklist ??= ListBuilder<UpdateSurveyChecklistRequest>();
  set checklist(ListBuilder<UpdateSurveyChecklistRequest>? checklist) =>
      _$this._checklist = checklist;

  ListBuilder<UpdateSurveyEvidenceRequest>? _evidence;
  ListBuilder<UpdateSurveyEvidenceRequest> get evidence =>
      _$this._evidence ??= ListBuilder<UpdateSurveyEvidenceRequest>();
  set evidence(ListBuilder<UpdateSurveyEvidenceRequest>? evidence) =>
      _$this._evidence = evidence;

  UpdateSurveyDraftRequestBuilder() {
    UpdateSurveyDraftRequest._defaults(this);
  }

  UpdateSurveyDraftRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedRevisionVersion = $v.expectedRevisionVersion;
      _visitedAtUtc = $v.visitedAtUtc;
      _scopeSummary = $v.scopeSummary;
      _assumptions = $v.assumptions?.toBuilder();
      _constraints = $v.constraints?.toBuilder();
      _missingDetails = $v.missingDetails?.toBuilder();
      _areas = $v.areas?.toBuilder();
      _checklist = $v.checklist?.toBuilder();
      _evidence = $v.evidence?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateSurveyDraftRequest other) {
    _$v = other as _$UpdateSurveyDraftRequest;
  }

  @override
  void update(void Function(UpdateSurveyDraftRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateSurveyDraftRequest build() => _build();

  _$UpdateSurveyDraftRequest _build() {
    _$UpdateSurveyDraftRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateSurveyDraftRequest._(
            expectedRevisionVersion: expectedRevisionVersion,
            visitedAtUtc: visitedAtUtc,
            scopeSummary: scopeSummary,
            assumptions: _assumptions?.build(),
            constraints: _constraints?.build(),
            missingDetails: _missingDetails?.build(),
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
            r'UpdateSurveyDraftRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
