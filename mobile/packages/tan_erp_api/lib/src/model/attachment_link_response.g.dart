// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'attachment_link_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AttachmentLinkResponse extends AttachmentLinkResponse {
  @override
  final String? id;
  @override
  final String? ownerType;
  @override
  final String? ownerId;
  @override
  final String? fileId;
  @override
  final String? purpose;
  @override
  final String? filename;
  @override
  final String? mediaType;
  @override
  final int? fileSizeBytes;
  @override
  final String? servingUrl;
  @override
  final AttachmentPersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? tag;
  @override
  final String? caption;

  factory _$AttachmentLinkResponse(
          [void Function(AttachmentLinkResponseBuilder)? updates]) =>
      (AttachmentLinkResponseBuilder()..update(updates))._build();

  _$AttachmentLinkResponse._(
      {this.id,
      this.ownerType,
      this.ownerId,
      this.fileId,
      this.purpose,
      this.filename,
      this.mediaType,
      this.fileSizeBytes,
      this.servingUrl,
      this.createdBy,
      this.createdAtUtc,
      this.tag,
      this.caption})
      : super._();
  @override
  AttachmentLinkResponse rebuild(
          void Function(AttachmentLinkResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AttachmentLinkResponseBuilder toBuilder() =>
      AttachmentLinkResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AttachmentLinkResponse &&
        id == other.id &&
        ownerType == other.ownerType &&
        ownerId == other.ownerId &&
        fileId == other.fileId &&
        purpose == other.purpose &&
        filename == other.filename &&
        mediaType == other.mediaType &&
        fileSizeBytes == other.fileSizeBytes &&
        servingUrl == other.servingUrl &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        tag == other.tag &&
        caption == other.caption;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, ownerType.hashCode);
    _$hash = $jc(_$hash, ownerId.hashCode);
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, purpose.hashCode);
    _$hash = $jc(_$hash, filename.hashCode);
    _$hash = $jc(_$hash, mediaType.hashCode);
    _$hash = $jc(_$hash, fileSizeBytes.hashCode);
    _$hash = $jc(_$hash, servingUrl.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, tag.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AttachmentLinkResponse')
          ..add('id', id)
          ..add('ownerType', ownerType)
          ..add('ownerId', ownerId)
          ..add('fileId', fileId)
          ..add('purpose', purpose)
          ..add('filename', filename)
          ..add('mediaType', mediaType)
          ..add('fileSizeBytes', fileSizeBytes)
          ..add('servingUrl', servingUrl)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('tag', tag)
          ..add('caption', caption))
        .toString();
  }
}

class AttachmentLinkResponseBuilder
    implements Builder<AttachmentLinkResponse, AttachmentLinkResponseBuilder> {
  _$AttachmentLinkResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _ownerType;
  String? get ownerType => _$this._ownerType;
  set ownerType(String? ownerType) => _$this._ownerType = ownerType;

  String? _ownerId;
  String? get ownerId => _$this._ownerId;
  set ownerId(String? ownerId) => _$this._ownerId = ownerId;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  String? _purpose;
  String? get purpose => _$this._purpose;
  set purpose(String? purpose) => _$this._purpose = purpose;

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

  AttachmentPersonResponseBuilder? _createdBy;
  AttachmentPersonResponseBuilder get createdBy =>
      _$this._createdBy ??= AttachmentPersonResponseBuilder();
  set createdBy(AttachmentPersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _tag;
  String? get tag => _$this._tag;
  set tag(String? tag) => _$this._tag = tag;

  String? _caption;
  String? get caption => _$this._caption;
  set caption(String? caption) => _$this._caption = caption;

  AttachmentLinkResponseBuilder() {
    AttachmentLinkResponse._defaults(this);
  }

  AttachmentLinkResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _ownerType = $v.ownerType;
      _ownerId = $v.ownerId;
      _fileId = $v.fileId;
      _purpose = $v.purpose;
      _filename = $v.filename;
      _mediaType = $v.mediaType;
      _fileSizeBytes = $v.fileSizeBytes;
      _servingUrl = $v.servingUrl;
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _tag = $v.tag;
      _caption = $v.caption;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AttachmentLinkResponse other) {
    _$v = other as _$AttachmentLinkResponse;
  }

  @override
  void update(void Function(AttachmentLinkResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AttachmentLinkResponse build() => _build();

  _$AttachmentLinkResponse _build() {
    _$AttachmentLinkResponse _$result;
    try {
      _$result = _$v ??
          _$AttachmentLinkResponse._(
            id: id,
            ownerType: ownerType,
            ownerId: ownerId,
            fileId: fileId,
            purpose: purpose,
            filename: filename,
            mediaType: mediaType,
            fileSizeBytes: fileSizeBytes,
            servingUrl: servingUrl,
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            tag: tag,
            caption: caption,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'createdBy';
        _createdBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AttachmentLinkResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
