// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'void_survey_revision_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$VoidSurveyRevisionRequest extends VoidSurveyRevisionRequest {
  @override
  final String? expectedRevisionVersion;
  @override
  final String? reason;

  factory _$VoidSurveyRevisionRequest(
          [void Function(VoidSurveyRevisionRequestBuilder)? updates]) =>
      (VoidSurveyRevisionRequestBuilder()..update(updates))._build();

  _$VoidSurveyRevisionRequest._({this.expectedRevisionVersion, this.reason})
      : super._();
  @override
  VoidSurveyRevisionRequest rebuild(
          void Function(VoidSurveyRevisionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  VoidSurveyRevisionRequestBuilder toBuilder() =>
      VoidSurveyRevisionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is VoidSurveyRevisionRequest &&
        expectedRevisionVersion == other.expectedRevisionVersion &&
        reason == other.reason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedRevisionVersion.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'VoidSurveyRevisionRequest')
          ..add('expectedRevisionVersion', expectedRevisionVersion)
          ..add('reason', reason))
        .toString();
  }
}

class VoidSurveyRevisionRequestBuilder
    implements
        Builder<VoidSurveyRevisionRequest, VoidSurveyRevisionRequestBuilder> {
  _$VoidSurveyRevisionRequest? _$v;

  String? _expectedRevisionVersion;
  String? get expectedRevisionVersion => _$this._expectedRevisionVersion;
  set expectedRevisionVersion(String? expectedRevisionVersion) =>
      _$this._expectedRevisionVersion = expectedRevisionVersion;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  VoidSurveyRevisionRequestBuilder() {
    VoidSurveyRevisionRequest._defaults(this);
  }

  VoidSurveyRevisionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedRevisionVersion = $v.expectedRevisionVersion;
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(VoidSurveyRevisionRequest other) {
    _$v = other as _$VoidSurveyRevisionRequest;
  }

  @override
  void update(void Function(VoidSurveyRevisionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  VoidSurveyRevisionRequest build() => _build();

  _$VoidSurveyRevisionRequest _build() {
    final _$result = _$v ??
        _$VoidSurveyRevisionRequest._(
          expectedRevisionVersion: expectedRevisionVersion,
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
