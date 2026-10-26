// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'attachment_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AttachmentListResponse extends AttachmentListResponse {
  @override
  final BuiltList<AttachmentLinkResponse>? items;

  factory _$AttachmentListResponse(
          [void Function(AttachmentListResponseBuilder)? updates]) =>
      (AttachmentListResponseBuilder()..update(updates))._build();

  _$AttachmentListResponse._({this.items}) : super._();
  @override
  AttachmentListResponse rebuild(
          void Function(AttachmentListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AttachmentListResponseBuilder toBuilder() =>
      AttachmentListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AttachmentListResponse && items == other.items;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AttachmentListResponse')
          ..add('items', items))
        .toString();
  }
}

class AttachmentListResponseBuilder
    implements Builder<AttachmentListResponse, AttachmentListResponseBuilder> {
  _$AttachmentListResponse? _$v;

  ListBuilder<AttachmentLinkResponse>? _items;
  ListBuilder<AttachmentLinkResponse> get items =>
      _$this._items ??= ListBuilder<AttachmentLinkResponse>();
  set items(ListBuilder<AttachmentLinkResponse>? items) =>
      _$this._items = items;

  AttachmentListResponseBuilder() {
    AttachmentListResponse._defaults(this);
  }

  AttachmentListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AttachmentListResponse other) {
    _$v = other as _$AttachmentListResponse;
  }

  @override
  void update(void Function(AttachmentListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AttachmentListResponse build() => _build();

  _$AttachmentListResponse _build() {
    _$AttachmentListResponse _$result;
    try {
      _$result = _$v ??
          _$AttachmentListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AttachmentListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
