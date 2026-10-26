// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_site_image_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateSiteImageRequest extends CreateSiteImageRequest {
  @override
  final String? fileId;
  @override
  final String? caption;

  factory _$CreateSiteImageRequest(
          [void Function(CreateSiteImageRequestBuilder)? updates]) =>
      (CreateSiteImageRequestBuilder()..update(updates))._build();

  _$CreateSiteImageRequest._({this.fileId, this.caption}) : super._();
  @override
  CreateSiteImageRequest rebuild(
          void Function(CreateSiteImageRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateSiteImageRequestBuilder toBuilder() =>
      CreateSiteImageRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateSiteImageRequest &&
        fileId == other.fileId &&
        caption == other.caption;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, fileId.hashCode);
    _$hash = $jc(_$hash, caption.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateSiteImageRequest')
          ..add('fileId', fileId)
          ..add('caption', caption))
        .toString();
  }
}

class CreateSiteImageRequestBuilder
    implements Builder<CreateSiteImageRequest, CreateSiteImageRequestBuilder> {
  _$CreateSiteImageRequest? _$v;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  String? _caption;
  String? get caption => _$this._caption;
  set caption(String? caption) => _$this._caption = caption;

  CreateSiteImageRequestBuilder() {
    CreateSiteImageRequest._defaults(this);
  }

  CreateSiteImageRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fileId = $v.fileId;
      _caption = $v.caption;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateSiteImageRequest other) {
    _$v = other as _$CreateSiteImageRequest;
  }

  @override
  void update(void Function(CreateSiteImageRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateSiteImageRequest build() => _build();

  _$CreateSiteImageRequest _build() {
    final _$result = _$v ??
        _$CreateSiteImageRequest._(
          fileId: fileId,
          caption: caption,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
