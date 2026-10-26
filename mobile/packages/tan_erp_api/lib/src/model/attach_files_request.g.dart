// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'attach_files_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AttachFilesRequest extends AttachFilesRequest {
  @override
  final String? purpose;
  @override
  final BuiltList<String>? fileIds;
  @override
  final String? tag;
  @override
  final String? caption;

  factory _$AttachFilesRequest(
          [void Function(AttachFilesRequestBuilder)? updates]) =>
      (AttachFilesRequestBuilder()..update(updates))._build();

  _$AttachFilesRequest._({this.purpose, this.fileIds, this.tag, this.caption})
      : super._();
  @override
  AttachFilesRequest rebuild(
          void Function(AttachFilesRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AttachFilesRequestBuilder toBuilder() =>
      AttachFilesRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AttachFilesRequest &&
        purpose == other.purpose &&
        fileIds == other.fileIds &&
        tag == other.tag &&
        caption == other.caption;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, purpose.hashCode);
    _$hash = $jc(_$hash, fileIds.hashCode);
    _$hash = $jc(_$hash, tag.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AttachFilesRequest')
          ..add('purpose', purpose)
          ..add('fileIds', fileIds)
          ..add('tag', tag)
          ..add('caption', caption))
        .toString();
  }
}

class AttachFilesRequestBuilder
    implements Builder<AttachFilesRequest, AttachFilesRequestBuilder> {
  _$AttachFilesRequest? _$v;

  String? _purpose;
  String? get purpose => _$this._purpose;
  set purpose(String? purpose) => _$this._purpose = purpose;

  ListBuilder<String>? _fileIds;
  ListBuilder<String> get fileIds => _$this._fileIds ??= ListBuilder<String>();
  set fileIds(ListBuilder<String>? fileIds) => _$this._fileIds = fileIds;

  String? _tag;
  String? get tag => _$this._tag;
  set tag(String? tag) => _$this._tag = tag;

  String? _caption;
  String? get caption => _$this._caption;
  set caption(String? caption) => _$this._caption = caption;

  AttachFilesRequestBuilder() {
    AttachFilesRequest._defaults(this);
  }

  AttachFilesRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _purpose = $v.purpose;
      _fileIds = $v.fileIds?.toBuilder();
      _tag = $v.tag;
      _caption = $v.caption;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AttachFilesRequest other) {
    _$v = other as _$AttachFilesRequest;
  }

  @override
  void update(void Function(AttachFilesRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AttachFilesRequest build() => _build();

  _$AttachFilesRequest _build() {
    _$AttachFilesRequest _$result;
    try {
      _$result = _$v ??
          _$AttachFilesRequest._(
            purpose: purpose,
            fileIds: _fileIds?.build(),
            tag: tag,
            caption: caption,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'fileIds';
        _fileIds?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AttachFilesRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
