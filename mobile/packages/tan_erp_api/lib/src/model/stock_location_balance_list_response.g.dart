// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_location_balance_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockLocationBalanceListResponse
    extends StockLocationBalanceListResponse {
  @override
  final BuiltList<StockLocationBalanceResponse>? items;
  @override
  final InventoryPaginationResponse? pagination;

  factory _$StockLocationBalanceListResponse(
          [void Function(StockLocationBalanceListResponseBuilder)? updates]) =>
      (StockLocationBalanceListResponseBuilder()..update(updates))._build();

  _$StockLocationBalanceListResponse._({this.items, this.pagination})
      : super._();
  @override
  StockLocationBalanceListResponse rebuild(
          void Function(StockLocationBalanceListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockLocationBalanceListResponseBuilder toBuilder() =>
      StockLocationBalanceListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockLocationBalanceListResponse &&
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
    return (newBuiltValueToStringHelper(r'StockLocationBalanceListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class StockLocationBalanceListResponseBuilder
    implements
        Builder<StockLocationBalanceListResponse,
            StockLocationBalanceListResponseBuilder> {
  _$StockLocationBalanceListResponse? _$v;

  ListBuilder<StockLocationBalanceResponse>? _items;
  ListBuilder<StockLocationBalanceResponse> get items =>
      _$this._items ??= ListBuilder<StockLocationBalanceResponse>();
  set items(ListBuilder<StockLocationBalanceResponse>? items) =>
      _$this._items = items;

  InventoryPaginationResponseBuilder? _pagination;
  InventoryPaginationResponseBuilder get pagination =>
      _$this._pagination ??= InventoryPaginationResponseBuilder();
  set pagination(InventoryPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  StockLocationBalanceListResponseBuilder() {
    StockLocationBalanceListResponse._defaults(this);
  }

  StockLocationBalanceListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockLocationBalanceListResponse other) {
    _$v = other as _$StockLocationBalanceListResponse;
  }

  @override
  void update(void Function(StockLocationBalanceListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockLocationBalanceListResponse build() => _build();

  _$StockLocationBalanceListResponse _build() {
    _$StockLocationBalanceListResponse _$result;
    try {
      _$result = _$v ??
          _$StockLocationBalanceListResponse._(
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
            r'StockLocationBalanceListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
