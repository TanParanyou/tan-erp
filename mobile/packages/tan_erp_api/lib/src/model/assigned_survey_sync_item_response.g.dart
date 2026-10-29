// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'assigned_survey_sync_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AssignedSurveySyncItemResponse extends AssignedSurveySyncItemResponse {
  @override
  final String? opportunityId;
  @override
  final String? opportunityCode;
  @override
  final String? opportunityTitle;
  @override
  final String? opportunityStage;
  @override
  final String? opportunityRowVersion;
  @override
  final String? customerDisplayName;
  @override
  final SiteSummaryResponse? site;
  @override
  final SiteSurveyResponse? survey;

  factory _$AssignedSurveySyncItemResponse(
          [void Function(AssignedSurveySyncItemResponseBuilder)? updates]) =>
      (AssignedSurveySyncItemResponseBuilder()..update(updates))._build();

  _$AssignedSurveySyncItemResponse._(
      {this.opportunityId,
      this.opportunityCode,
      this.opportunityTitle,
      this.opportunityStage,
      this.opportunityRowVersion,
      this.customerDisplayName,
      this.site,
      this.survey})
      : super._();
  @override
  AssignedSurveySyncItemResponse rebuild(
          void Function(AssignedSurveySyncItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AssignedSurveySyncItemResponseBuilder toBuilder() =>
      AssignedSurveySyncItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AssignedSurveySyncItemResponse &&
        opportunityId == other.opportunityId &&
        opportunityCode == other.opportunityCode &&
        opportunityTitle == other.opportunityTitle &&
        opportunityStage == other.opportunityStage &&
        opportunityRowVersion == other.opportunityRowVersion &&
        customerDisplayName == other.customerDisplayName &&
        site == other.site &&
        survey == other.survey;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jc(_$hash, opportunityCode.hashCode);
    _$hash = $jc(_$hash, opportunityTitle.hashCode);
    _$hash = $jc(_$hash, opportunityStage.hashCode);
    _$hash = $jc(_$hash, opportunityRowVersion.hashCode);
    _$hash = $jc(_$hash, customerDisplayName.hashCode);
    _$hash = $jc(_$hash, site.hashCode);
    _$hash = $jc(_$hash, survey.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AssignedSurveySyncItemResponse')
          ..add('opportunityId', opportunityId)
          ..add('opportunityCode', opportunityCode)
          ..add('opportunityTitle', opportunityTitle)
          ..add('opportunityStage', opportunityStage)
          ..add('opportunityRowVersion', opportunityRowVersion)
          ..add('customerDisplayName', customerDisplayName)
          ..add('site', site)
          ..add('survey', survey))
        .toString();
  }
}

class AssignedSurveySyncItemResponseBuilder
    implements
        Builder<AssignedSurveySyncItemResponse,
            AssignedSurveySyncItemResponseBuilder> {
  _$AssignedSurveySyncItemResponse? _$v;

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  String? _opportunityCode;
  String? get opportunityCode => _$this._opportunityCode;
  set opportunityCode(String? opportunityCode) =>
      _$this._opportunityCode = opportunityCode;

  String? _opportunityTitle;
  String? get opportunityTitle => _$this._opportunityTitle;
  set opportunityTitle(String? opportunityTitle) =>
      _$this._opportunityTitle = opportunityTitle;

  String? _opportunityStage;
  String? get opportunityStage => _$this._opportunityStage;
  set opportunityStage(String? opportunityStage) =>
      _$this._opportunityStage = opportunityStage;

  String? _opportunityRowVersion;
  String? get opportunityRowVersion => _$this._opportunityRowVersion;
  set opportunityRowVersion(String? opportunityRowVersion) =>
      _$this._opportunityRowVersion = opportunityRowVersion;

  String? _customerDisplayName;
  String? get customerDisplayName => _$this._customerDisplayName;
  set customerDisplayName(String? customerDisplayName) =>
      _$this._customerDisplayName = customerDisplayName;

  SiteSummaryResponseBuilder? _site;
  SiteSummaryResponseBuilder get site =>
      _$this._site ??= SiteSummaryResponseBuilder();
  set site(SiteSummaryResponseBuilder? site) => _$this._site = site;

  SiteSurveyResponseBuilder? _survey;
  SiteSurveyResponseBuilder get survey =>
      _$this._survey ??= SiteSurveyResponseBuilder();
  set survey(SiteSurveyResponseBuilder? survey) => _$this._survey = survey;

  AssignedSurveySyncItemResponseBuilder() {
    AssignedSurveySyncItemResponse._defaults(this);
  }

  AssignedSurveySyncItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _opportunityId = $v.opportunityId;
      _opportunityCode = $v.opportunityCode;
      _opportunityTitle = $v.opportunityTitle;
      _opportunityStage = $v.opportunityStage;
      _opportunityRowVersion = $v.opportunityRowVersion;
      _customerDisplayName = $v.customerDisplayName;
      _site = $v.site?.toBuilder();
      _survey = $v.survey?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AssignedSurveySyncItemResponse other) {
    _$v = other as _$AssignedSurveySyncItemResponse;
  }

  @override
  void update(void Function(AssignedSurveySyncItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AssignedSurveySyncItemResponse build() => _build();

  _$AssignedSurveySyncItemResponse _build() {
    _$AssignedSurveySyncItemResponse _$result;
    try {
      _$result = _$v ??
          _$AssignedSurveySyncItemResponse._(
            opportunityId: opportunityId,
            opportunityCode: opportunityCode,
            opportunityTitle: opportunityTitle,
            opportunityStage: opportunityStage,
            opportunityRowVersion: opportunityRowVersion,
            customerDisplayName: customerDisplayName,
            site: _site?.build(),
            survey: _survey?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'site';
        _site?.build();
        _$failedField = 'survey';
        _survey?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AssignedSurveySyncItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
