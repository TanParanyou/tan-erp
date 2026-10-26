// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reorder_item_images_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReorderItemImagesRequest extends ReorderItemImagesRequest {
  @override
  final BuiltList<String> orderedImageIds;

  factory _$ReorderItemImagesRequest(
          [void Function(ReorderItemImagesRequestBuilder)? updates]) =>
      (ReorderItemImagesRequestBuilder()..update(updates))._build();

  _$ReorderItemImagesRequest._({required this.orderedImageIds}) : super._();
  @override
  ReorderItemImagesRequest rebuild(
          void Function(ReorderItemImagesRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReorderItemImagesRequestBuilder toBuilder() =>
      ReorderItemImagesRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReorderItemImagesRequest &&
        orderedImageIds == other.orderedImageIds;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, orderedImageIds.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReorderItemImagesRequest')
          ..add('orderedImageIds', orderedImageIds))
        .toString();
  }
}

class ReorderItemImagesRequestBuilder
    implements
        Builder<ReorderItemImagesRequest, ReorderItemImagesRequestBuilder> {
  _$ReorderItemImagesRequest? _$v;

  ListBuilder<String>? _orderedImageIds;
  ListBuilder<String> get orderedImageIds =>
      _$this._orderedImageIds ??= ListBuilder<String>();
  set orderedImageIds(ListBuilder<String>? orderedImageIds) =>
      _$this._orderedImageIds = orderedImageIds;

  ReorderItemImagesRequestBuilder() {
    ReorderItemImagesRequest._defaults(this);
  }

  ReorderItemImagesRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _orderedImageIds = $v.orderedImageIds.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReorderItemImagesRequest other) {
    _$v = other as _$ReorderItemImagesRequest;
  }

  @override
  void update(void Function(ReorderItemImagesRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReorderItemImagesRequest build() => _build();

  _$ReorderItemImagesRequest _build() {
    _$ReorderItemImagesRequest _$result;
    try {
      _$result = _$v ??
          _$ReorderItemImagesRequest._(
            orderedImageIds: orderedImageIds.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'orderedImageIds';
        orderedImageIds.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ReorderItemImagesRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
