// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_site_survey_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateSiteSurveyRequest extends CreateSiteSurveyRequest {
  @override
  final String? siteId;
  @override
  final String? assignedSurveyorId;
  @override
  final DateTime? scheduledStartUtc;
  @override
  final DateTime? scheduledEndUtc;
  @override
  final String? expectedOpportunityVersion;

  factory _$CreateSiteSurveyRequest(
          [void Function(CreateSiteSurveyRequestBuilder)? updates]) =>
      (CreateSiteSurveyRequestBuilder()..update(updates))._build();

  _$CreateSiteSurveyRequest._(
      {this.siteId,
      this.assignedSurveyorId,
      this.scheduledStartUtc,
      this.scheduledEndUtc,
      this.expectedOpportunityVersion})
      : super._();
  @override
  CreateSiteSurveyRequest rebuild(
          void Function(CreateSiteSurveyRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateSiteSurveyRequestBuilder toBuilder() =>
      CreateSiteSurveyRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateSiteSurveyRequest &&
        siteId == other.siteId &&
        assignedSurveyorId == other.assignedSurveyorId &&
        scheduledStartUtc == other.scheduledStartUtc &&
        scheduledEndUtc == other.scheduledEndUtc &&
        expectedOpportunityVersion == other.expectedOpportunityVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, siteId.hashCode);
    _$hash = $jc(_$hash, assignedSurveyorId.hashCode);
    _$hash = $jc(_$hash, scheduledStartUtc.hashCode);
    _$hash = $jc(_$hash, scheduledEndUtc.hashCode);
    _$hash = $jc(_$hash, expectedOpportunityVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateSiteSurveyRequest')
          ..add('siteId', siteId)
          ..add('assignedSurveyorId', assignedSurveyorId)
          ..add('scheduledStartUtc', scheduledStartUtc)
          ..add('scheduledEndUtc', scheduledEndUtc)
          ..add('expectedOpportunityVersion', expectedOpportunityVersion))
        .toString();
  }
}

class CreateSiteSurveyRequestBuilder
    implements
        Builder<CreateSiteSurveyRequest, CreateSiteSurveyRequestBuilder> {
  _$CreateSiteSurveyRequest? _$v;

  String? _siteId;
  String? get siteId => _$this._siteId;
  set siteId(String? siteId) => _$this._siteId = siteId;

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

  String? _expectedOpportunityVersion;
  String? get expectedOpportunityVersion => _$this._expectedOpportunityVersion;
  set expectedOpportunityVersion(String? expectedOpportunityVersion) =>
      _$this._expectedOpportunityVersion = expectedOpportunityVersion;

  CreateSiteSurveyRequestBuilder() {
    CreateSiteSurveyRequest._defaults(this);
  }

  CreateSiteSurveyRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _siteId = $v.siteId;
      _assignedSurveyorId = $v.assignedSurveyorId;
      _scheduledStartUtc = $v.scheduledStartUtc;
      _scheduledEndUtc = $v.scheduledEndUtc;
      _expectedOpportunityVersion = $v.expectedOpportunityVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateSiteSurveyRequest other) {
    _$v = other as _$CreateSiteSurveyRequest;
  }

  @override
  void update(void Function(CreateSiteSurveyRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateSiteSurveyRequest build() => _build();

  _$CreateSiteSurveyRequest _build() {
    final _$result = _$v ??
        _$CreateSiteSurveyRequest._(
          siteId: siteId,
          assignedSurveyorId: assignedSurveyorId,
          scheduledStartUtc: scheduledStartUtc,
          scheduledEndUtc: scheduledEndUtc,
          expectedOpportunityVersion: expectedOpportunityVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
