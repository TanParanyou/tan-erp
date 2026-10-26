// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'survey_template_version_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyTemplateVersionResponse extends SurveyTemplateVersionResponse {
  @override
  final String? code;
  @override
  final BuiltList<String>? requiredChecklistItems;
  @override
  final int? minimumEvidenceCount;
  @override
  final int? minimumAreaEvidenceCount;
  @override
  final String? snapshotHashVersion;
  @override
  final DateTime? effectiveFromUtc;
  @override
  final DateTime? effectiveToUtc;
  @override
  final bool? isCurrent;

  factory _$SurveyTemplateVersionResponse(
          [void Function(SurveyTemplateVersionResponseBuilder)? updates]) =>
      (SurveyTemplateVersionResponseBuilder()..update(updates))._build();

  _$SurveyTemplateVersionResponse._(
      {this.code,
      this.requiredChecklistItems,
      this.minimumEvidenceCount,
      this.minimumAreaEvidenceCount,
      this.snapshotHashVersion,
      this.effectiveFromUtc,
      this.effectiveToUtc,
      this.isCurrent})
      : super._();
  @override
  SurveyTemplateVersionResponse rebuild(
          void Function(SurveyTemplateVersionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyTemplateVersionResponseBuilder toBuilder() =>
      SurveyTemplateVersionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyTemplateVersionResponse &&
        code == other.code &&
        requiredChecklistItems == other.requiredChecklistItems &&
        minimumEvidenceCount == other.minimumEvidenceCount &&
        minimumAreaEvidenceCount == other.minimumAreaEvidenceCount &&
        snapshotHashVersion == other.snapshotHashVersion &&
        effectiveFromUtc == other.effectiveFromUtc &&
        effectiveToUtc == other.effectiveToUtc &&
        isCurrent == other.isCurrent;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, requiredChecklistItems.hashCode);
    _$hash = $jc(_$hash, minimumEvidenceCount.hashCode);
    _$hash = $jc(_$hash, minimumAreaEvidenceCount.hashCode);
    _$hash = $jc(_$hash, snapshotHashVersion.hashCode);
    _$hash = $jc(_$hash, effectiveFromUtc.hashCode);
    _$hash = $jc(_$hash, effectiveToUtc.hashCode);
    _$hash = $jc(_$hash, isCurrent.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyTemplateVersionResponse')
          ..add('code', code)
          ..add('requiredChecklistItems', requiredChecklistItems)
          ..add('minimumEvidenceCount', minimumEvidenceCount)
          ..add('minimumAreaEvidenceCount', minimumAreaEvidenceCount)
          ..add('snapshotHashVersion', snapshotHashVersion)
          ..add('effectiveFromUtc', effectiveFromUtc)
          ..add('effectiveToUtc', effectiveToUtc)
          ..add('isCurrent', isCurrent))
        .toString();
  }
}

class SurveyTemplateVersionResponseBuilder
    implements
        Builder<SurveyTemplateVersionResponse,
            SurveyTemplateVersionResponseBuilder> {
  _$SurveyTemplateVersionResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  ListBuilder<String>? _requiredChecklistItems;
  ListBuilder<String> get requiredChecklistItems =>
      _$this._requiredChecklistItems ??= ListBuilder<String>();
  set requiredChecklistItems(ListBuilder<String>? requiredChecklistItems) =>
      _$this._requiredChecklistItems = requiredChecklistItems;

  int? _minimumEvidenceCount;
  int? get minimumEvidenceCount => _$this._minimumEvidenceCount;
  set minimumEvidenceCount(int? minimumEvidenceCount) =>
      _$this._minimumEvidenceCount = minimumEvidenceCount;

  int? _minimumAreaEvidenceCount;
  int? get minimumAreaEvidenceCount => _$this._minimumAreaEvidenceCount;
  set minimumAreaEvidenceCount(int? minimumAreaEvidenceCount) =>
      _$this._minimumAreaEvidenceCount = minimumAreaEvidenceCount;

  String? _snapshotHashVersion;
  String? get snapshotHashVersion => _$this._snapshotHashVersion;
  set snapshotHashVersion(String? snapshotHashVersion) =>
      _$this._snapshotHashVersion = snapshotHashVersion;

  DateTime? _effectiveFromUtc;
  DateTime? get effectiveFromUtc => _$this._effectiveFromUtc;
  set effectiveFromUtc(DateTime? effectiveFromUtc) =>
      _$this._effectiveFromUtc = effectiveFromUtc;

  DateTime? _effectiveToUtc;
  DateTime? get effectiveToUtc => _$this._effectiveToUtc;
  set effectiveToUtc(DateTime? effectiveToUtc) =>
      _$this._effectiveToUtc = effectiveToUtc;

  bool? _isCurrent;
  bool? get isCurrent => _$this._isCurrent;
  set isCurrent(bool? isCurrent) => _$this._isCurrent = isCurrent;

  SurveyTemplateVersionResponseBuilder() {
    SurveyTemplateVersionResponse._defaults(this);
  }

  SurveyTemplateVersionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _requiredChecklistItems = $v.requiredChecklistItems?.toBuilder();
      _minimumEvidenceCount = $v.minimumEvidenceCount;
      _minimumAreaEvidenceCount = $v.minimumAreaEvidenceCount;
      _snapshotHashVersion = $v.snapshotHashVersion;
      _effectiveFromUtc = $v.effectiveFromUtc;
      _effectiveToUtc = $v.effectiveToUtc;
      _isCurrent = $v.isCurrent;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyTemplateVersionResponse other) {
    _$v = other as _$SurveyTemplateVersionResponse;
  }

  @override
  void update(void Function(SurveyTemplateVersionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyTemplateVersionResponse build() => _build();

  _$SurveyTemplateVersionResponse _build() {
    _$SurveyTemplateVersionResponse _$result;
    try {
      _$result = _$v ??
          _$SurveyTemplateVersionResponse._(
            code: code,
            requiredChecklistItems: _requiredChecklistItems?.build(),
            minimumEvidenceCount: minimumEvidenceCount,
            minimumAreaEvidenceCount: minimumAreaEvidenceCount,
            snapshotHashVersion: snapshotHashVersion,
            effectiveFromUtc: effectiveFromUtc,
            effectiveToUtc: effectiveToUtc,
            isCurrent: isCurrent,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'requiredChecklistItems';
        _requiredChecklistItems?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SurveyTemplateVersionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
