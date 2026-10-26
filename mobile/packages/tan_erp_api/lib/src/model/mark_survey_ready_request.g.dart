// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mark_survey_ready_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MarkSurveyReadyRequest extends MarkSurveyReadyRequest {
  @override
  final String? expectedRevisionVersion;
  @override
  final String? expectedOpportunityVersion;

  factory _$MarkSurveyReadyRequest(
          [void Function(MarkSurveyReadyRequestBuilder)? updates]) =>
      (MarkSurveyReadyRequestBuilder()..update(updates))._build();

  _$MarkSurveyReadyRequest._(
      {this.expectedRevisionVersion, this.expectedOpportunityVersion})
      : super._();
  @override
  MarkSurveyReadyRequest rebuild(
          void Function(MarkSurveyReadyRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MarkSurveyReadyRequestBuilder toBuilder() =>
      MarkSurveyReadyRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MarkSurveyReadyRequest &&
        expectedRevisionVersion == other.expectedRevisionVersion &&
        expectedOpportunityVersion == other.expectedOpportunityVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedRevisionVersion.hashCode);
    _$hash = $jc(_$hash, expectedOpportunityVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MarkSurveyReadyRequest')
          ..add('expectedRevisionVersion', expectedRevisionVersion)
          ..add('expectedOpportunityVersion', expectedOpportunityVersion))
        .toString();
  }
}

class MarkSurveyReadyRequestBuilder
    implements Builder<MarkSurveyReadyRequest, MarkSurveyReadyRequestBuilder> {
  _$MarkSurveyReadyRequest? _$v;

  String? _expectedRevisionVersion;
  String? get expectedRevisionVersion => _$this._expectedRevisionVersion;
  set expectedRevisionVersion(String? expectedRevisionVersion) =>
      _$this._expectedRevisionVersion = expectedRevisionVersion;

  String? _expectedOpportunityVersion;
  String? get expectedOpportunityVersion => _$this._expectedOpportunityVersion;
  set expectedOpportunityVersion(String? expectedOpportunityVersion) =>
      _$this._expectedOpportunityVersion = expectedOpportunityVersion;

  MarkSurveyReadyRequestBuilder() {
    MarkSurveyReadyRequest._defaults(this);
  }

  MarkSurveyReadyRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedRevisionVersion = $v.expectedRevisionVersion;
      _expectedOpportunityVersion = $v.expectedOpportunityVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MarkSurveyReadyRequest other) {
    _$v = other as _$MarkSurveyReadyRequest;
  }

  @override
  void update(void Function(MarkSurveyReadyRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MarkSurveyReadyRequest build() => _build();

  _$MarkSurveyReadyRequest _build() {
    final _$result = _$v ??
        _$MarkSurveyReadyRequest._(
          expectedRevisionVersion: expectedRevisionVersion,
          expectedOpportunityVersion: expectedOpportunityVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
