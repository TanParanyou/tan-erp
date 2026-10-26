// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'publish_survey_template_version_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PublishSurveyTemplateVersionRequest
    extends PublishSurveyTemplateVersionRequest {
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

  factory _$PublishSurveyTemplateVersionRequest(
          [void Function(PublishSurveyTemplateVersionRequestBuilder)?
              updates]) =>
      (PublishSurveyTemplateVersionRequestBuilder()..update(updates))._build();

  _$PublishSurveyTemplateVersionRequest._(
      {this.code,
      this.requiredChecklistItems,
      this.minimumEvidenceCount,
      this.minimumAreaEvidenceCount,
      this.snapshotHashVersion,
      this.effectiveFromUtc})
      : super._();
  @override
  PublishSurveyTemplateVersionRequest rebuild(
          void Function(PublishSurveyTemplateVersionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PublishSurveyTemplateVersionRequestBuilder toBuilder() =>
      PublishSurveyTemplateVersionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PublishSurveyTemplateVersionRequest &&
        code == other.code &&
        requiredChecklistItems == other.requiredChecklistItems &&
        minimumEvidenceCount == other.minimumEvidenceCount &&
        minimumAreaEvidenceCount == other.minimumAreaEvidenceCount &&
        snapshotHashVersion == other.snapshotHashVersion &&
        effectiveFromUtc == other.effectiveFromUtc;
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
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PublishSurveyTemplateVersionRequest')
          ..add('code', code)
          ..add('requiredChecklistItems', requiredChecklistItems)
          ..add('minimumEvidenceCount', minimumEvidenceCount)
          ..add('minimumAreaEvidenceCount', minimumAreaEvidenceCount)
          ..add('snapshotHashVersion', snapshotHashVersion)
          ..add('effectiveFromUtc', effectiveFromUtc))
        .toString();
  }
}

class PublishSurveyTemplateVersionRequestBuilder
    implements
        Builder<PublishSurveyTemplateVersionRequest,
            PublishSurveyTemplateVersionRequestBuilder> {
  _$PublishSurveyTemplateVersionRequest? _$v;

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

  PublishSurveyTemplateVersionRequestBuilder() {
    PublishSurveyTemplateVersionRequest._defaults(this);
  }

  PublishSurveyTemplateVersionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _requiredChecklistItems = $v.requiredChecklistItems?.toBuilder();
      _minimumEvidenceCount = $v.minimumEvidenceCount;
      _minimumAreaEvidenceCount = $v.minimumAreaEvidenceCount;
      _snapshotHashVersion = $v.snapshotHashVersion;
      _effectiveFromUtc = $v.effectiveFromUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PublishSurveyTemplateVersionRequest other) {
    _$v = other as _$PublishSurveyTemplateVersionRequest;
  }

  @override
  void update(
      void Function(PublishSurveyTemplateVersionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PublishSurveyTemplateVersionRequest build() => _build();

  _$PublishSurveyTemplateVersionRequest _build() {
    _$PublishSurveyTemplateVersionRequest _$result;
    try {
      _$result = _$v ??
          _$PublishSurveyTemplateVersionRequest._(
            code: code,
            requiredChecklistItems: _requiredChecklistItems?.build(),
            minimumEvidenceCount: minimumEvidenceCount,
            minimumAreaEvidenceCount: minimumAreaEvidenceCount,
            snapshotHashVersion: snapshotHashVersion,
            effectiveFromUtc: effectiveFromUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'requiredChecklistItems';
        _requiredChecklistItems?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(r'PublishSurveyTemplateVersionRequest',
            _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
