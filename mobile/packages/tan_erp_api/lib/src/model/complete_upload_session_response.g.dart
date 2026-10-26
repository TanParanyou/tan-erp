// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'complete_upload_session_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CompleteUploadSessionResponse extends CompleteUploadSessionResponse {
  @override
  final String? sessionId;
  @override
  final BuiltList<CompletedFileResponse>? files;

  factory _$CompleteUploadSessionResponse(
          [void Function(CompleteUploadSessionResponseBuilder)? updates]) =>
      (CompleteUploadSessionResponseBuilder()..update(updates))._build();

  _$CompleteUploadSessionResponse._({this.sessionId, this.files}) : super._();
  @override
  CompleteUploadSessionResponse rebuild(
          void Function(CompleteUploadSessionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CompleteUploadSessionResponseBuilder toBuilder() =>
      CompleteUploadSessionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CompleteUploadSessionResponse &&
        sessionId == other.sessionId &&
        files == other.files;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, sessionId.hashCode);
    _$hash = $jc(_$hash, files.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CompleteUploadSessionResponse')
          ..add('sessionId', sessionId)
          ..add('files', files))
        .toString();
  }
}

class CompleteUploadSessionResponseBuilder
    implements
        Builder<CompleteUploadSessionResponse,
            CompleteUploadSessionResponseBuilder> {
  _$CompleteUploadSessionResponse? _$v;

  String? _sessionId;
  String? get sessionId => _$this._sessionId;
  set sessionId(String? sessionId) => _$this._sessionId = sessionId;

  ListBuilder<CompletedFileResponse>? _files;
  ListBuilder<CompletedFileResponse> get files =>
      _$this._files ??= ListBuilder<CompletedFileResponse>();
  set files(ListBuilder<CompletedFileResponse>? files) => _$this._files = files;

  CompleteUploadSessionResponseBuilder() {
    CompleteUploadSessionResponse._defaults(this);
  }

  CompleteUploadSessionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _sessionId = $v.sessionId;
      _files = $v.files?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CompleteUploadSessionResponse other) {
    _$v = other as _$CompleteUploadSessionResponse;
  }

  @override
  void update(void Function(CompleteUploadSessionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CompleteUploadSessionResponse build() => _build();

  _$CompleteUploadSessionResponse _build() {
    _$CompleteUploadSessionResponse _$result;
    try {
      _$result = _$v ??
          _$CompleteUploadSessionResponse._(
            sessionId: sessionId,
            files: _files?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'files';
        _files?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CompleteUploadSessionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
