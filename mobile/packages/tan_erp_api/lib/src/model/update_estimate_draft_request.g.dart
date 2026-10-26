// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_estimate_draft_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateEstimateDraftRequest extends UpdateEstimateDraftRequest {
  @override
  final String? expectedRevisionVersion;
  @override
  final BuiltList<UpdateEstimateSectionDto>? sections;

  factory _$UpdateEstimateDraftRequest(
          [void Function(UpdateEstimateDraftRequestBuilder)? updates]) =>
      (UpdateEstimateDraftRequestBuilder()..update(updates))._build();

  _$UpdateEstimateDraftRequest._({this.expectedRevisionVersion, this.sections})
      : super._();
  @override
  UpdateEstimateDraftRequest rebuild(
          void Function(UpdateEstimateDraftRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateEstimateDraftRequestBuilder toBuilder() =>
      UpdateEstimateDraftRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateEstimateDraftRequest &&
        expectedRevisionVersion == other.expectedRevisionVersion &&
        sections == other.sections;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedRevisionVersion.hashCode);
    _$hash = $jc(_$hash, sections.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateEstimateDraftRequest')
          ..add('expectedRevisionVersion', expectedRevisionVersion)
          ..add('sections', sections))
        .toString();
  }
}

class UpdateEstimateDraftRequestBuilder
    implements
        Builder<UpdateEstimateDraftRequest, UpdateEstimateDraftRequestBuilder> {
  _$UpdateEstimateDraftRequest? _$v;

  String? _expectedRevisionVersion;
  String? get expectedRevisionVersion => _$this._expectedRevisionVersion;
  set expectedRevisionVersion(String? expectedRevisionVersion) =>
      _$this._expectedRevisionVersion = expectedRevisionVersion;

  ListBuilder<UpdateEstimateSectionDto>? _sections;
  ListBuilder<UpdateEstimateSectionDto> get sections =>
      _$this._sections ??= ListBuilder<UpdateEstimateSectionDto>();
  set sections(ListBuilder<UpdateEstimateSectionDto>? sections) =>
      _$this._sections = sections;

  UpdateEstimateDraftRequestBuilder() {
    UpdateEstimateDraftRequest._defaults(this);
  }

  UpdateEstimateDraftRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedRevisionVersion = $v.expectedRevisionVersion;
      _sections = $v.sections?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateEstimateDraftRequest other) {
    _$v = other as _$UpdateEstimateDraftRequest;
  }

  @override
  void update(void Function(UpdateEstimateDraftRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateEstimateDraftRequest build() => _build();

  _$UpdateEstimateDraftRequest _build() {
    _$UpdateEstimateDraftRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateEstimateDraftRequest._(
            expectedRevisionVersion: expectedRevisionVersion,
            sections: _sections?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'sections';
        _sections?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateEstimateDraftRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
