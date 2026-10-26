// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderListResponse extends PurchaseOrderListResponse {
  @override
  final BuiltList<PurchaseOrderListItemResponse>? items;
  @override
  final ProcurementPaginationResponse? pagination;

  factory _$PurchaseOrderListResponse(
          [void Function(PurchaseOrderListResponseBuilder)? updates]) =>
      (PurchaseOrderListResponseBuilder()..update(updates))._build();

  _$PurchaseOrderListResponse._({this.items, this.pagination}) : super._();
  @override
  PurchaseOrderListResponse rebuild(
          void Function(PurchaseOrderListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderListResponseBuilder toBuilder() =>
      PurchaseOrderListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderListResponse &&
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
    return (newBuiltValueToStringHelper(r'PurchaseOrderListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class PurchaseOrderListResponseBuilder
    implements
        Builder<PurchaseOrderListResponse, PurchaseOrderListResponseBuilder> {
  _$PurchaseOrderListResponse? _$v;

  ListBuilder<PurchaseOrderListItemResponse>? _items;
  ListBuilder<PurchaseOrderListItemResponse> get items =>
      _$this._items ??= ListBuilder<PurchaseOrderListItemResponse>();
  set items(ListBuilder<PurchaseOrderListItemResponse>? items) =>
      _$this._items = items;

  ProcurementPaginationResponseBuilder? _pagination;
  ProcurementPaginationResponseBuilder get pagination =>
      _$this._pagination ??= ProcurementPaginationResponseBuilder();
  set pagination(ProcurementPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  PurchaseOrderListResponseBuilder() {
    PurchaseOrderListResponse._defaults(this);
  }

  PurchaseOrderListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseOrderListResponse other) {
    _$v = other as _$PurchaseOrderListResponse;
  }

  @override
  void update(void Function(PurchaseOrderListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderListResponse build() => _build();

  _$PurchaseOrderListResponse _build() {
    _$PurchaseOrderListResponse _$result;
    try {
      _$result = _$v ??
          _$PurchaseOrderListResponse._(
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
            r'PurchaseOrderListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
