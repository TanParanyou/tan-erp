// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'bom_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BomListResponse extends BomListResponse {
  @override
  final BuiltList<BomListItemResponse>? items;
  @override
  final ProductionPaginationResponse? pagination;

  factory _$BomListResponse([void Function(BomListResponseBuilder)? updates]) =>
      (BomListResponseBuilder()..update(updates))._build();

  _$BomListResponse._({this.items, this.pagination}) : super._();
  @override
  BomListResponse rebuild(void Function(BomListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BomListResponseBuilder toBuilder() => BomListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BomListResponse &&
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
    return (newBuiltValueToStringHelper(r'BomListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class BomListResponseBuilder
    implements Builder<BomListResponse, BomListResponseBuilder> {
  _$BomListResponse? _$v;

  ListBuilder<BomListItemResponse>? _items;
  ListBuilder<BomListItemResponse> get items =>
      _$this._items ??= ListBuilder<BomListItemResponse>();
  set items(ListBuilder<BomListItemResponse>? items) => _$this._items = items;

  ProductionPaginationResponseBuilder? _pagination;
  ProductionPaginationResponseBuilder get pagination =>
      _$this._pagination ??= ProductionPaginationResponseBuilder();
  set pagination(ProductionPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  BomListResponseBuilder() {
    BomListResponse._defaults(this);
  }

  BomListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BomListResponse other) {
    _$v = other as _$BomListResponse;
  }

  @override
  void update(void Function(BomListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BomListResponse build() => _build();

  _$BomListResponse _build() {
    _$BomListResponse _$result;
    try {
      _$result = _$v ??
          _$BomListResponse._(
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
            r'BomListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
