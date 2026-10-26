// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_requests_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceRequestsListResponse extends ServiceRequestsListResponse {
  @override
  final BuiltList<ServiceRequestListItemResponse>? items;
  @override
  final ServicePaginationResponse? pagination;

  factory _$ServiceRequestsListResponse(
          [void Function(ServiceRequestsListResponseBuilder)? updates]) =>
      (ServiceRequestsListResponseBuilder()..update(updates))._build();

  _$ServiceRequestsListResponse._({this.items, this.pagination}) : super._();
  @override
  ServiceRequestsListResponse rebuild(
          void Function(ServiceRequestsListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceRequestsListResponseBuilder toBuilder() =>
      ServiceRequestsListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceRequestsListResponse &&
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
    return (newBuiltValueToStringHelper(r'ServiceRequestsListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class ServiceRequestsListResponseBuilder
    implements
        Builder<ServiceRequestsListResponse,
            ServiceRequestsListResponseBuilder> {
  _$ServiceRequestsListResponse? _$v;

  ListBuilder<ServiceRequestListItemResponse>? _items;
  ListBuilder<ServiceRequestListItemResponse> get items =>
      _$this._items ??= ListBuilder<ServiceRequestListItemResponse>();
  set items(ListBuilder<ServiceRequestListItemResponse>? items) =>
      _$this._items = items;

  ServicePaginationResponseBuilder? _pagination;
  ServicePaginationResponseBuilder get pagination =>
      _$this._pagination ??= ServicePaginationResponseBuilder();
  set pagination(ServicePaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  ServiceRequestsListResponseBuilder() {
    ServiceRequestsListResponse._defaults(this);
  }

  ServiceRequestsListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceRequestsListResponse other) {
    _$v = other as _$ServiceRequestsListResponse;
  }

  @override
  void update(void Function(ServiceRequestsListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceRequestsListResponse build() => _build();

  _$ServiceRequestsListResponse _build() {
    _$ServiceRequestsListResponse _$result;
    try {
      _$result = _$v ??
          _$ServiceRequestsListResponse._(
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
            r'ServiceRequestsListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
