// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_serial_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockSerialListResponse extends StockSerialListResponse {
  @override
  final BuiltList<StockSerialResponse>? items;
  @override
  final InventoryPaginationResponse? pagination;

  factory _$StockSerialListResponse(
          [void Function(StockSerialListResponseBuilder)? updates]) =>
      (StockSerialListResponseBuilder()..update(updates))._build();

  _$StockSerialListResponse._({this.items, this.pagination}) : super._();
  @override
  StockSerialListResponse rebuild(
          void Function(StockSerialListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockSerialListResponseBuilder toBuilder() =>
      StockSerialListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockSerialListResponse &&
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
    return (newBuiltValueToStringHelper(r'StockSerialListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class StockSerialListResponseBuilder
    implements
        Builder<StockSerialListResponse, StockSerialListResponseBuilder> {
  _$StockSerialListResponse? _$v;

  ListBuilder<StockSerialResponse>? _items;
  ListBuilder<StockSerialResponse> get items =>
      _$this._items ??= ListBuilder<StockSerialResponse>();
  set items(ListBuilder<StockSerialResponse>? items) => _$this._items = items;

  InventoryPaginationResponseBuilder? _pagination;
  InventoryPaginationResponseBuilder get pagination =>
      _$this._pagination ??= InventoryPaginationResponseBuilder();
  set pagination(InventoryPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  StockSerialListResponseBuilder() {
    StockSerialListResponse._defaults(this);
  }

  StockSerialListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockSerialListResponse other) {
    _$v = other as _$StockSerialListResponse;
  }

  @override
  void update(void Function(StockSerialListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockSerialListResponse build() => _build();

  _$StockSerialListResponse _build() {
    _$StockSerialListResponse _$result;
    try {
      _$result = _$v ??
          _$StockSerialListResponse._(
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
            r'StockSerialListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
