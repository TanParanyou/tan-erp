// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'clone_survey_revision_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CloneSurveyRevisionRequest extends CloneSurveyRevisionRequest {
  @override
  final String? sourceRevisionId;
  @override
  final String? reason;

  factory _$CloneSurveyRevisionRequest(
          [void Function(CloneSurveyRevisionRequestBuilder)? updates]) =>
      (CloneSurveyRevisionRequestBuilder()..update(updates))._build();

  _$CloneSurveyRevisionRequest._({this.sourceRevisionId, this.reason})
      : super._();
  @override
  CloneSurveyRevisionRequest rebuild(
          void Function(CloneSurveyRevisionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CloneSurveyRevisionRequestBuilder toBuilder() =>
      CloneSurveyRevisionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CloneSurveyRevisionRequest &&
        sourceRevisionId == other.sourceRevisionId &&
        reason == other.reason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sourceRevisionId.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CloneSurveyRevisionRequest')
          ..add('sourceRevisionId', sourceRevisionId)
          ..add('reason', reason))
        .toString();
  }
}

class CloneSurveyRevisionRequestBuilder
    implements
        Builder<CloneSurveyRevisionRequest, CloneSurveyRevisionRequestBuilder> {
  _$CloneSurveyRevisionRequest? _$v;

  String? _sourceRevisionId;
  String? get sourceRevisionId => _$this._sourceRevisionId;
  set sourceRevisionId(String? sourceRevisionId) =>
      _$this._sourceRevisionId = sourceRevisionId;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  CloneSurveyRevisionRequestBuilder() {
    CloneSurveyRevisionRequest._defaults(this);
  }

  CloneSurveyRevisionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sourceRevisionId = $v.sourceRevisionId;
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CloneSurveyRevisionRequest other) {
    _$v = other as _$CloneSurveyRevisionRequest;
  }

  @override
  void update(void Function(CloneSurveyRevisionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CloneSurveyRevisionRequest build() => _build();

  _$CloneSurveyRevisionRequest _build() {
    final _$result = _$v ??
        _$CloneSurveyRevisionRequest._(
          sourceRevisionId: sourceRevisionId,
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
