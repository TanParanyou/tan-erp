// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_invoice_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierInvoiceListResponse extends SupplierInvoiceListResponse {
  @override
  final BuiltList<SupplierInvoiceListItemResponse>? items;
  @override
  final ProcurementPaginationResponse? pagination;

  factory _$SupplierInvoiceListResponse(
          [void Function(SupplierInvoiceListResponseBuilder)? updates]) =>
      (SupplierInvoiceListResponseBuilder()..update(updates))._build();

  _$SupplierInvoiceListResponse._({this.items, this.pagination}) : super._();
  @override
  SupplierInvoiceListResponse rebuild(
          void Function(SupplierInvoiceListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierInvoiceListResponseBuilder toBuilder() =>
      SupplierInvoiceListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierInvoiceListResponse &&
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
    return (newBuiltValueToStringHelper(r'SupplierInvoiceListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class SupplierInvoiceListResponseBuilder
    implements
        Builder<SupplierInvoiceListResponse,
            SupplierInvoiceListResponseBuilder> {
  _$SupplierInvoiceListResponse? _$v;

  ListBuilder<SupplierInvoiceListItemResponse>? _items;
  ListBuilder<SupplierInvoiceListItemResponse> get items =>
      _$this._items ??= ListBuilder<SupplierInvoiceListItemResponse>();
  set items(ListBuilder<SupplierInvoiceListItemResponse>? items) =>
      _$this._items = items;

  ProcurementPaginationResponseBuilder? _pagination;
  ProcurementPaginationResponseBuilder get pagination =>
      _$this._pagination ??= ProcurementPaginationResponseBuilder();
  set pagination(ProcurementPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  SupplierInvoiceListResponseBuilder() {
    SupplierInvoiceListResponse._defaults(this);
  }

  SupplierInvoiceListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierInvoiceListResponse other) {
    _$v = other as _$SupplierInvoiceListResponse;
  }

  @override
  void update(void Function(SupplierInvoiceListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierInvoiceListResponse build() => _build();

  _$SupplierInvoiceListResponse _build() {
    _$SupplierInvoiceListResponse _$result;
    try {
      _$result = _$v ??
          _$SupplierInvoiceListResponse._(
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
            r'SupplierInvoiceListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
