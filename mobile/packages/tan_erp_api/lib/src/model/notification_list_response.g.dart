// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'notification_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$NotificationListResponse extends NotificationListResponse {
  @override
  final BuiltList<NotificationResponse>? items;
  @override
  final PaginationMetadataResponse? pagination;

  factory _$NotificationListResponse(
          [void Function(NotificationListResponseBuilder)? updates]) =>
      (NotificationListResponseBuilder()..update(updates))._build();

  _$NotificationListResponse._({this.items, this.pagination}) : super._();
  @override
  NotificationListResponse rebuild(
          void Function(NotificationListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  NotificationListResponseBuilder toBuilder() =>
      NotificationListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is NotificationListResponse &&
        items == other.items &&
        pagination == other.pagination;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, pagination.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'NotificationListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class NotificationListResponseBuilder
    implements
        Builder<NotificationListResponse, NotificationListResponseBuilder> {
  _$NotificationListResponse? _$v;

  ListBuilder<NotificationResponse>? _items;
  ListBuilder<NotificationResponse> get items =>
      _$this._items ??= ListBuilder<NotificationResponse>();
  set items(ListBuilder<NotificationResponse>? items) => _$this._items = items;

  PaginationMetadataResponseBuilder? _pagination;
  PaginationMetadataResponseBuilder get pagination =>
      _$this._pagination ??= PaginationMetadataResponseBuilder();
  set pagination(PaginationMetadataResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  NotificationListResponseBuilder() {
    NotificationListResponse._defaults(this);
  }

  NotificationListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(NotificationListResponse other) {
    _$v = other as _$NotificationListResponse;
  }

  @override
  void update(void Function(NotificationListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  NotificationListResponse build() => _build();

  _$NotificationListResponse _build() {
    _$NotificationListResponse _$result;
    try {
      _$result = _$v ??
          _$NotificationListResponse._(
            items: _items?.build(),
            pagination: _pagination?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
        _$failedField = 'pagination';
        _pagination?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'NotificationListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
