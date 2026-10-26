// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'attach_work_images_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AttachWorkImagesRequest extends AttachWorkImagesRequest {
  @override
  final BuiltList<AttachWorkImageItemRequest>? images;

  factory _$AttachWorkImagesRequest(
          [void Function(AttachWorkImagesRequestBuilder)? updates]) =>
      (AttachWorkImagesRequestBuilder()..update(updates))._build();

  _$AttachWorkImagesRequest._({this.images}) : super._();
  @override
  AttachWorkImagesRequest rebuild(
          void Function(AttachWorkImagesRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AttachWorkImagesRequestBuilder toBuilder() =>
      AttachWorkImagesRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AttachWorkImagesRequest && images == other.images;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, images.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AttachWorkImagesRequest')
          ..add('images', images))
        .toString();
  }
}

class AttachWorkImagesRequestBuilder
    implements
        Builder<AttachWorkImagesRequest, AttachWorkImagesRequestBuilder> {
  _$AttachWorkImagesRequest? _$v;

  ListBuilder<AttachWorkImageItemRequest>? _images;
  ListBuilder<AttachWorkImageItemRequest> get images =>
      _$this._images ??= ListBuilder<AttachWorkImageItemRequest>();
  set images(ListBuilder<AttachWorkImageItemRequest>? images) =>
      _$this._images = images;

  AttachWorkImagesRequestBuilder() {
    AttachWorkImagesRequest._defaults(this);
  }

  AttachWorkImagesRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _images = $v.images?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AttachWorkImagesRequest other) {
    _$v = other as _$AttachWorkImagesRequest;
  }

  @override
  void update(void Function(AttachWorkImagesRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AttachWorkImagesRequest build() => _build();

  _$AttachWorkImagesRequest _build() {
    _$AttachWorkImagesRequest _$result;
    try {
      _$result = _$v ??
          _$AttachWorkImagesRequest._(
            images: _images?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'images';
        _images?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AttachWorkImagesRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
