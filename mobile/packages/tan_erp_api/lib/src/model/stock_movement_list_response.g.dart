// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_movement_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockMovementListResponse extends StockMovementListResponse {
  @override
  final BuiltList<StockMovementResponse>? items;
  @override
  final InventoryPaginationResponse? pagination;

  factory _$StockMovementListResponse(
          [void Function(StockMovementListResponseBuilder)? updates]) =>
      (StockMovementListResponseBuilder()..update(updates))._build();

  _$StockMovementListResponse._({this.items, this.pagination}) : super._();
  @override
  StockMovementListResponse rebuild(
          void Function(StockMovementListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockMovementListResponseBuilder toBuilder() =>
      StockMovementListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockMovementListResponse &&
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
    return (newBuiltValueToStringHelper(r'StockMovementListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class StockMovementListResponseBuilder
    implements
        Builder<StockMovementListResponse, StockMovementListResponseBuilder> {
  _$StockMovementListResponse? _$v;

  ListBuilder<StockMovementResponse>? _items;
  ListBuilder<StockMovementResponse> get items =>
      _$this._items ??= ListBuilder<StockMovementResponse>();
  set items(ListBuilder<StockMovementResponse>? items) => _$this._items = items;

  InventoryPaginationResponseBuilder? _pagination;
  InventoryPaginationResponseBuilder get pagination =>
      _$this._pagination ??= InventoryPaginationResponseBuilder();
  set pagination(InventoryPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  StockMovementListResponseBuilder() {
    StockMovementListResponse._defaults(this);
  }

  StockMovementListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockMovementListResponse other) {
    _$v = other as _$StockMovementListResponse;
  }

  @override
  void update(void Function(StockMovementListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockMovementListResponse build() => _build();

  _$StockMovementListResponse _build() {
    _$StockMovementListResponse _$result;
    try {
      _$result = _$v ??
          _$StockMovementListResponse._(
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
            r'StockMovementListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
