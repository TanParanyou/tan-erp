// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseRequestListResponse extends PurchaseRequestListResponse {
  @override
  final BuiltList<PurchaseRequestListItemResponse>? items;
  @override
  final ProcurementPaginationResponse? pagination;

  factory _$PurchaseRequestListResponse(
          [void Function(PurchaseRequestListResponseBuilder)? updates]) =>
      (PurchaseRequestListResponseBuilder()..update(updates))._build();

  _$PurchaseRequestListResponse._({this.items, this.pagination}) : super._();
  @override
  PurchaseRequestListResponse rebuild(
          void Function(PurchaseRequestListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseRequestListResponseBuilder toBuilder() =>
      PurchaseRequestListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseRequestListResponse &&
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
    return (newBuiltValueToStringHelper(r'PurchaseRequestListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class PurchaseRequestListResponseBuilder
    implements
        Builder<PurchaseRequestListResponse,
            PurchaseRequestListResponseBuilder> {
  _$PurchaseRequestListResponse? _$v;

  ListBuilder<PurchaseRequestListItemResponse>? _items;
  ListBuilder<PurchaseRequestListItemResponse> get items =>
      _$this._items ??= ListBuilder<PurchaseRequestListItemResponse>();
  set items(ListBuilder<PurchaseRequestListItemResponse>? items) =>
      _$this._items = items;

  ProcurementPaginationResponseBuilder? _pagination;
  ProcurementPaginationResponseBuilder get pagination =>
      _$this._pagination ??= ProcurementPaginationResponseBuilder();
  set pagination(ProcurementPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  PurchaseRequestListResponseBuilder() {
    PurchaseRequestListResponse._defaults(this);
  }

  PurchaseRequestListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PurchaseRequestListResponse other) {
    _$v = other as _$PurchaseRequestListResponse;
  }

  @override
  void update(void Function(PurchaseRequestListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseRequestListResponse build() => _build();

  _$PurchaseRequestListResponse _build() {
    _$PurchaseRequestListResponse _$result;
    try {
      _$result = _$v ??
          _$PurchaseRequestListResponse._(
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
            r'PurchaseRequestListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
