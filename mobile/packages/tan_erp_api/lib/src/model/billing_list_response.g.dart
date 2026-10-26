// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'billing_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BillingListResponse extends BillingListResponse {
  @override
  final BuiltList<BillingListItemResponse>? items;
  @override
  final FinancePaginationResponse? pagination;

  factory _$BillingListResponse(
          [void Function(BillingListResponseBuilder)? updates]) =>
      (BillingListResponseBuilder()..update(updates))._build();

  _$BillingListResponse._({this.items, this.pagination}) : super._();
  @override
  BillingListResponse rebuild(
          void Function(BillingListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BillingListResponseBuilder toBuilder() =>
      BillingListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BillingListResponse &&
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
    return (newBuiltValueToStringHelper(r'BillingListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class BillingListResponseBuilder
    implements Builder<BillingListResponse, BillingListResponseBuilder> {
  _$BillingListResponse? _$v;

  ListBuilder<BillingListItemResponse>? _items;
  ListBuilder<BillingListItemResponse> get items =>
      _$this._items ??= ListBuilder<BillingListItemResponse>();
  set items(ListBuilder<BillingListItemResponse>? items) =>
      _$this._items = items;

  FinancePaginationResponseBuilder? _pagination;
  FinancePaginationResponseBuilder get pagination =>
      _$this._pagination ??= FinancePaginationResponseBuilder();
  set pagination(FinancePaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  BillingListResponseBuilder() {
    BillingListResponse._defaults(this);
  }

  BillingListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BillingListResponse other) {
    _$v = other as _$BillingListResponse;
  }

  @override
  void update(void Function(BillingListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BillingListResponse build() => _build();

  _$BillingListResponse _build() {
    _$BillingListResponse _$result;
    try {
      _$result = _$v ??
          _$BillingListResponse._(
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
            r'BillingListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
