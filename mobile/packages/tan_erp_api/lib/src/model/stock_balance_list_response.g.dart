// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_balance_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockBalanceListResponse extends StockBalanceListResponse {
  @override
  final BuiltList<StockBalanceResponse>? items;
  @override
  final InventoryPaginationResponse? pagination;
  @override
  final double? totalValue;

  factory _$StockBalanceListResponse(
          [void Function(StockBalanceListResponseBuilder)? updates]) =>
      (StockBalanceListResponseBuilder()..update(updates))._build();

  _$StockBalanceListResponse._({this.items, this.pagination, this.totalValue})
      : super._();
  @override
  StockBalanceListResponse rebuild(
          void Function(StockBalanceListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockBalanceListResponseBuilder toBuilder() =>
      StockBalanceListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockBalanceListResponse &&
        items == other.items &&
        pagination == other.pagination &&
        totalValue == other.totalValue;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, pagination.hashCode);
    _$hash = $jc(_$hash, totalValue.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockBalanceListResponse')
          ..add('items', items)
          ..add('pagination', pagination)
          ..add('totalValue', totalValue))
        .toString();
  }
}

class StockBalanceListResponseBuilder
    implements
        Builder<StockBalanceListResponse, StockBalanceListResponseBuilder> {
  _$StockBalanceListResponse? _$v;

  ListBuilder<StockBalanceResponse>? _items;
  ListBuilder<StockBalanceResponse> get items =>
      _$this._items ??= ListBuilder<StockBalanceResponse>();
  set items(ListBuilder<StockBalanceResponse>? items) => _$this._items = items;

  InventoryPaginationResponseBuilder? _pagination;
  InventoryPaginationResponseBuilder get pagination =>
      _$this._pagination ??= InventoryPaginationResponseBuilder();
  set pagination(InventoryPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  double? _totalValue;
  double? get totalValue => _$this._totalValue;
  set totalValue(double? totalValue) => _$this._totalValue = totalValue;

  StockBalanceListResponseBuilder() {
    StockBalanceListResponse._defaults(this);
  }

  StockBalanceListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _totalValue = $v.totalValue;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockBalanceListResponse other) {
    _$v = other as _$StockBalanceListResponse;
  }

  @override
  void update(void Function(StockBalanceListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockBalanceListResponse build() => _build();

  _$StockBalanceListResponse _build() {
    _$StockBalanceListResponse _$result;
    try {
      _$result = _$v ??
          _$StockBalanceListResponse._(
            items: _items?.build(),
            pagination: _pagination?.build(),
            totalValue: totalValue,
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
            r'StockBalanceListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
