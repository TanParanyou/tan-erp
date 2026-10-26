// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'warehouse_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WarehouseListResponse extends WarehouseListResponse {
  @override
  final BuiltList<WarehouseResponse>? items;
  @override
  final InventoryPaginationResponse? pagination;

  factory _$WarehouseListResponse(
          [void Function(WarehouseListResponseBuilder)? updates]) =>
      (WarehouseListResponseBuilder()..update(updates))._build();

  _$WarehouseListResponse._({this.items, this.pagination}) : super._();
  @override
  WarehouseListResponse rebuild(
          void Function(WarehouseListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WarehouseListResponseBuilder toBuilder() =>
      WarehouseListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WarehouseListResponse &&
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
    return (newBuiltValueToStringHelper(r'WarehouseListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class WarehouseListResponseBuilder
    implements Builder<WarehouseListResponse, WarehouseListResponseBuilder> {
  _$WarehouseListResponse? _$v;

  ListBuilder<WarehouseResponse>? _items;
  ListBuilder<WarehouseResponse> get items =>
      _$this._items ??= ListBuilder<WarehouseResponse>();
  set items(ListBuilder<WarehouseResponse>? items) => _$this._items = items;

  InventoryPaginationResponseBuilder? _pagination;
  InventoryPaginationResponseBuilder get pagination =>
      _$this._pagination ??= InventoryPaginationResponseBuilder();
  set pagination(InventoryPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  WarehouseListResponseBuilder() {
    WarehouseListResponse._defaults(this);
  }

  WarehouseListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WarehouseListResponse other) {
    _$v = other as _$WarehouseListResponse;
  }

  @override
  void update(void Function(WarehouseListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WarehouseListResponse build() => _build();

  _$WarehouseListResponse _build() {
    _$WarehouseListResponse _$result;
    try {
      _$result = _$v ??
          _$WarehouseListResponse._(
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
            r'WarehouseListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
