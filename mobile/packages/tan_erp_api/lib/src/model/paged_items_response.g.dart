// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'paged_items_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PagedItemsResponse extends PagedItemsResponse {
  @override
  final BuiltList<ItemResponse>? items;
  @override
  final int? totalCount;
  @override
  final int? pageNumber;
  @override
  final int? pageSize;

  factory _$PagedItemsResponse(
          [void Function(PagedItemsResponseBuilder)? updates]) =>
      (PagedItemsResponseBuilder()..update(updates))._build();

  _$PagedItemsResponse._(
      {this.items, this.totalCount, this.pageNumber, this.pageSize})
      : super._();
  @override
  PagedItemsResponse rebuild(
          void Function(PagedItemsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PagedItemsResponseBuilder toBuilder() =>
      PagedItemsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PagedItemsResponse &&
        items == other.items &&
        totalCount == other.totalCount &&
        pageNumber == other.pageNumber &&
        pageSize == other.pageSize;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, totalCount.hashCode);
    _$hash = $jc(_$hash, pageNumber.hashCode);
    _$hash = $jc(_$hash, pageSize.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PagedItemsResponse')
          ..add('items', items)
          ..add('totalCount', totalCount)
          ..add('pageNumber', pageNumber)
          ..add('pageSize', pageSize))
        .toString();
  }
}

class PagedItemsResponseBuilder
    implements Builder<PagedItemsResponse, PagedItemsResponseBuilder> {
  _$PagedItemsResponse? _$v;

  ListBuilder<ItemResponse>? _items;
  ListBuilder<ItemResponse> get items =>
      _$this._items ??= ListBuilder<ItemResponse>();
  set items(ListBuilder<ItemResponse>? items) => _$this._items = items;

  int? _totalCount;
  int? get totalCount => _$this._totalCount;
  set totalCount(int? totalCount) => _$this._totalCount = totalCount;

  int? _pageNumber;
  int? get pageNumber => _$this._pageNumber;
  set pageNumber(int? pageNumber) => _$this._pageNumber = pageNumber;

  int? _pageSize;
  int? get pageSize => _$this._pageSize;
  set pageSize(int? pageSize) => _$this._pageSize = pageSize;

  PagedItemsResponseBuilder() {
    PagedItemsResponse._defaults(this);
  }

  PagedItemsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _totalCount = $v.totalCount;
      _pageNumber = $v.pageNumber;
      _pageSize = $v.pageSize;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PagedItemsResponse other) {
    _$v = other as _$PagedItemsResponse;
  }

  @override
  void update(void Function(PagedItemsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PagedItemsResponse build() => _build();

  _$PagedItemsResponse _build() {
    _$PagedItemsResponse _$result;
    try {
      _$result = _$v ??
          _$PagedItemsResponse._(
            items: _items?.build(),
            totalCount: totalCount,
            pageNumber: pageNumber,
            pageSize: pageSize,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PagedItemsResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
