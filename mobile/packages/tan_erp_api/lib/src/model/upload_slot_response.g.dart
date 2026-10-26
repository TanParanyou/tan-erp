// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'upload_slot_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UploadSlotResponse extends UploadSlotResponse {
  @override
  final String? slotId;
  @override
  final String? filename;
  @override
  final String? mediaType;
  @override
  final int? fileSizeBytes;

  factory _$UploadSlotResponse(
          [void Function(UploadSlotResponseBuilder)? updates]) =>
      (UploadSlotResponseBuilder()..update(updates))._build();

  _$UploadSlotResponse._(
      {this.slotId, this.filename, this.mediaType, this.fileSizeBytes})
      : super._();
  @override
  UploadSlotResponse rebuild(
          void Function(UploadSlotResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UploadSlotResponseBuilder toBuilder() =>
      UploadSlotResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UploadSlotResponse &&
        slotId == other.slotId &&
        filename == other.filename &&
        mediaType == other.mediaType &&
        fileSizeBytes == other.fileSizeBytes;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, slotId.hashCode);
    _$hash = $jc(_$hash, filename.hashCode);
    _$hash = $jc(_$hash, mediaType.hashCode);
    _$hash = $jc(_$hash, fileSizeBytes.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UploadSlotResponse')
          ..add('slotId', slotId)
          ..add('filename', filename)
          ..add('mediaType', mediaType)
          ..add('fileSizeBytes', fileSizeBytes))
        .toString();
  }
}

class UploadSlotResponseBuilder
    implements Builder<UploadSlotResponse, UploadSlotResponseBuilder> {
  _$UploadSlotResponse? _$v;

  String? _slotId;
  String? get slotId => _$this._slotId;
  set slotId(String? slotId) => _$this._slotId = slotId;

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

  UploadSlotResponseBuilder() {
    UploadSlotResponse._defaults(this);
  }

  UploadSlotResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _slotId = $v.slotId;
      _filename = $v.filename;
      _mediaType = $v.mediaType;
      _fileSizeBytes = $v.fileSizeBytes;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UploadSlotResponse other) {
    _$v = other as _$UploadSlotResponse;
  }

  @override
  void update(void Function(UploadSlotResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UploadSlotResponse build() => _build();

  _$UploadSlotResponse _build() {
    final _$result = _$v ??
        _$UploadSlotResponse._(
          slotId: slotId,
          filename: filename,
          mediaType: mediaType,
          fileSizeBytes: fileSizeBytes,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
