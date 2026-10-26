// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'completed_file_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CompletedFileResponse extends CompletedFileResponse {
  @override
  final String? fileId;
  @override
  final String? filename;
  @override
  final String? mediaType;
  @override
  final int? fileSizeBytes;
  @override
  final String? servingUrl;

  factory _$CompletedFileResponse(
          [void Function(CompletedFileResponseBuilder)? updates]) =>
      (CompletedFileResponseBuilder()..update(updates))._build();

  _$CompletedFileResponse._(
      {this.fileId,
      this.filename,
      this.mediaType,
      this.fileSizeBytes,
      this.servingUrl})
      : super._();
  @override
  CompletedFileResponse rebuild(
          void Function(CompletedFileResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CompletedFileResponseBuilder toBuilder() =>
      CompletedFileResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CompletedFileResponse &&
        fileId == other.fileId &&
        filename == other.filename &&
        mediaType == other.mediaType &&
        fileSizeBytes == other.fileSizeBytes &&
        servingUrl == other.servingUrl;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, filename.hashCode);
    _$hash = $jc(_$hash, mediaType.hashCode);
    _$hash = $jc(_$hash, fileSizeBytes.hashCode);
    _$hash = $jc(_$hash, servingUrl.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CompletedFileResponse')
          ..add('fileId', fileId)
          ..add('filename', filename)
          ..add('mediaType', mediaType)
          ..add('fileSizeBytes', fileSizeBytes)
          ..add('servingUrl', servingUrl))
        .toString();
  }
}

class CompletedFileResponseBuilder
    implements Builder<CompletedFileResponse, CompletedFileResponseBuilder> {
  _$CompletedFileResponse? _$v;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  String? _filename;
  String? get filename => _$this._filename;
  set filename(String? filename) => _$this._filename = filename;

  String? _mediaType;
  String? get mediaType => _$this._mediaType;
  set mediaType(String? mediaType) => _$this._mediaType = mediaType;

  int? _fileSizeBytes;
  int? get fileSizeBytes => _$this._fileSizeBytes;
  set fileSizeBytes(int? fileSizeBytes) =>
      _$this._fileSizeBytes = fileSizeBytes;

  String? _servingUrl;
  String? get servingUrl => _$this._servingUrl;
  set servingUrl(String? servingUrl) => _$this._servingUrl = servingUrl;

  CompletedFileResponseBuilder() {
    CompletedFileResponse._defaults(this);
  }

  CompletedFileResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fileId = $v.fileId;
      _filename = $v.filename;
      _mediaType = $v.mediaType;
      _fileSizeBytes = $v.fileSizeBytes;
      _servingUrl = $v.servingUrl;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CompletedFileResponse other) {
    _$v = other as _$CompletedFileResponse;
  }

  @override
  void update(void Function(CompletedFileResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CompletedFileResponse build() => _build();

  _$CompletedFileResponse _build() {
    final _$result = _$v ??
        _$CompletedFileResponse._(
          fileId: fileId,
          filename: filename,
          mediaType: mediaType,
          fileSizeBytes: fileSizeBytes,
          servingUrl: servingUrl,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
