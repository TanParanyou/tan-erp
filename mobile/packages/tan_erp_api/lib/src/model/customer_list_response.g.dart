// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerListResponse extends CustomerListResponse {
  @override
  final BuiltList<CustomerListItemResponse>? items;
  @override
  final PaginationMetadataResponse? pagination;

  factory _$CustomerListResponse(
          [void Function(CustomerListResponseBuilder)? updates]) =>
      (CustomerListResponseBuilder()..update(updates))._build();

  _$CustomerListResponse._({this.items, this.pagination}) : super._();
  @override
  CustomerListResponse rebuild(
          void Function(CustomerListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerListResponseBuilder toBuilder() =>
      CustomerListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerListResponse &&
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
    return (newBuiltValueToStringHelper(r'CustomerListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class CustomerListResponseBuilder
    implements Builder<CustomerListResponse, CustomerListResponseBuilder> {
  _$CustomerListResponse? _$v;

  ListBuilder<CustomerListItemResponse>? _items;
  ListBuilder<CustomerListItemResponse> get items =>
      _$this._items ??= ListBuilder<CustomerListItemResponse>();
  set items(ListBuilder<CustomerListItemResponse>? items) =>
      _$this._items = items;

  PaginationMetadataResponseBuilder? _pagination;
  PaginationMetadataResponseBuilder get pagination =>
      _$this._pagination ??= PaginationMetadataResponseBuilder();
  set pagination(PaginationMetadataResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  CustomerListResponseBuilder() {
    CustomerListResponse._defaults(this);
  }

  CustomerListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerListResponse other) {
    _$v = other as _$CustomerListResponse;
  }

  @override
  void update(void Function(CustomerListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerListResponse build() => _build();

  _$CustomerListResponse _build() {
    _$CustomerListResponse _$result;
    try {
      _$result = _$v ??
          _$CustomerListResponse._(
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
            r'CustomerListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
