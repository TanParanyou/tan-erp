// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'attach_work_image_item_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AttachWorkImageItemRequest extends AttachWorkImageItemRequest {
  @override
  final String? fileId;
  @override
  final String? caption;

  factory _$AttachWorkImageItemRequest(
          [void Function(AttachWorkImageItemRequestBuilder)? updates]) =>
      (AttachWorkImageItemRequestBuilder()..update(updates))._build();

  _$AttachWorkImageItemRequest._({this.fileId, this.caption}) : super._();
  @override
  AttachWorkImageItemRequest rebuild(
          void Function(AttachWorkImageItemRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AttachWorkImageItemRequestBuilder toBuilder() =>
      AttachWorkImageItemRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AttachWorkImageItemRequest &&
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
    return (newBuiltValueToStringHelper(r'AttachWorkImageItemRequest')
          ..add('fileId', fileId)
          ..add('caption', caption))
        .toString();
  }
}

class AttachWorkImageItemRequestBuilder
    implements
        Builder<AttachWorkImageItemRequest, AttachWorkImageItemRequestBuilder> {
  _$AttachWorkImageItemRequest? _$v;

  String? _fileId;
  String? get fileId => _$this._fileId;
  set fileId(String? fileId) => _$this._fileId = fileId;

  String? _caption;
  String? get caption => _$this._caption;
  set caption(String? caption) => _$this._caption = caption;

  AttachWorkImageItemRequestBuilder() {
    AttachWorkImageItemRequest._defaults(this);
  }

  AttachWorkImageItemRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fileId = $v.fileId;
      _caption = $v.caption;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AttachWorkImageItemRequest other) {
    _$v = other as _$AttachWorkImageItemRequest;
  }

  @override
  void update(void Function(AttachWorkImageItemRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AttachWorkImageItemRequest build() => _build();

  _$AttachWorkImageItemRequest _build() {
    final _$result = _$v ??
        _$AttachWorkImageItemRequest._(
          fileId: fileId,
          caption: caption,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
