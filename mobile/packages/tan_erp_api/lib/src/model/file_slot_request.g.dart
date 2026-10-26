// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'file_slot_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$FileSlotRequest extends FileSlotRequest {
  @override
  final String? filename;
  @override
  final String? mediaType;
  @override
  final int? fileSizeBytes;

  factory _$FileSlotRequest([void Function(FileSlotRequestBuilder)? updates]) =>
      (FileSlotRequestBuilder()..update(updates))._build();

  _$FileSlotRequest._({this.filename, this.mediaType, this.fileSizeBytes})
      : super._();
  @override
  FileSlotRequest rebuild(void Function(FileSlotRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  FileSlotRequestBuilder toBuilder() => FileSlotRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is FileSlotRequest &&
        filename == other.filename &&
        mediaType == other.mediaType &&
        fileSizeBytes == other.fileSizeBytes;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, filename.hashCode);
    _$hash = $jc(_$hash, mediaType.hashCode);
    _$hash = $jc(_$hash, fileSizeBytes.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'FileSlotRequest')
          ..add('filename', filename)
          ..add('mediaType', mediaType)
          ..add('fileSizeBytes', fileSizeBytes))
        .toString();
  }
}

class FileSlotRequestBuilder
    implements Builder<FileSlotRequest, FileSlotRequestBuilder> {
  _$FileSlotRequest? _$v;

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

  FileSlotRequestBuilder() {
    FileSlotRequest._defaults(this);
  }

  FileSlotRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _filename = $v.filename;
      _mediaType = $v.mediaType;
      _fileSizeBytes = $v.fileSizeBytes;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(FileSlotRequest other) {
    _$v = other as _$FileSlotRequest;
  }

  @override
  void update(void Function(FileSlotRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  FileSlotRequest build() => _build();

  _$FileSlotRequest _build() {
    final _$result = _$v ??
        _$FileSlotRequest._(
          filename: filename,
          mediaType: mediaType,
          fileSizeBytes: fileSizeBytes,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
