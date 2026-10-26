// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierListResponse extends SupplierListResponse {
  @override
  final BuiltList<SupplierResponse>? items;
  @override
  final ProcurementPaginationResponse? pagination;

  factory _$SupplierListResponse(
          [void Function(SupplierListResponseBuilder)? updates]) =>
      (SupplierListResponseBuilder()..update(updates))._build();

  _$SupplierListResponse._({this.items, this.pagination}) : super._();
  @override
  SupplierListResponse rebuild(
          void Function(SupplierListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierListResponseBuilder toBuilder() =>
      SupplierListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierListResponse &&
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
    return (newBuiltValueToStringHelper(r'SupplierListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class SupplierListResponseBuilder
    implements Builder<SupplierListResponse, SupplierListResponseBuilder> {
  _$SupplierListResponse? _$v;

  ListBuilder<SupplierResponse>? _items;
  ListBuilder<SupplierResponse> get items =>
      _$this._items ??= ListBuilder<SupplierResponse>();
  set items(ListBuilder<SupplierResponse>? items) => _$this._items = items;

  ProcurementPaginationResponseBuilder? _pagination;
  ProcurementPaginationResponseBuilder get pagination =>
      _$this._pagination ??= ProcurementPaginationResponseBuilder();
  set pagination(ProcurementPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  SupplierListResponseBuilder() {
    SupplierListResponse._defaults(this);
  }

  SupplierListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierListResponse other) {
    _$v = other as _$SupplierListResponse;
  }

  @override
  void update(void Function(SupplierListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierListResponse build() => _build();

  _$SupplierListResponse _build() {
    _$SupplierListResponse _$result;
    try {
      _$result = _$v ??
          _$SupplierListResponse._(
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
            r'SupplierListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
