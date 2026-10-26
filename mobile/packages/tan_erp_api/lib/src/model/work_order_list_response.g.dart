// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderListResponse extends WorkOrderListResponse {
  @override
  final BuiltList<WorkOrderListItemResponse>? items;
  @override
  final ProductionPaginationResponse? pagination;

  factory _$WorkOrderListResponse(
          [void Function(WorkOrderListResponseBuilder)? updates]) =>
      (WorkOrderListResponseBuilder()..update(updates))._build();

  _$WorkOrderListResponse._({this.items, this.pagination}) : super._();
  @override
  WorkOrderListResponse rebuild(
          void Function(WorkOrderListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderListResponseBuilder toBuilder() =>
      WorkOrderListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderListResponse &&
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
    return (newBuiltValueToStringHelper(r'WorkOrderListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class WorkOrderListResponseBuilder
    implements Builder<WorkOrderListResponse, WorkOrderListResponseBuilder> {
  _$WorkOrderListResponse? _$v;

  ListBuilder<WorkOrderListItemResponse>? _items;
  ListBuilder<WorkOrderListItemResponse> get items =>
      _$this._items ??= ListBuilder<WorkOrderListItemResponse>();
  set items(ListBuilder<WorkOrderListItemResponse>? items) =>
      _$this._items = items;

  ProductionPaginationResponseBuilder? _pagination;
  ProductionPaginationResponseBuilder get pagination =>
      _$this._pagination ??= ProductionPaginationResponseBuilder();
  set pagination(ProductionPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  WorkOrderListResponseBuilder() {
    WorkOrderListResponse._defaults(this);
  }

  WorkOrderListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderListResponse other) {
    _$v = other as _$WorkOrderListResponse;
  }

  @override
  void update(void Function(WorkOrderListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderListResponse build() => _build();

  _$WorkOrderListResponse _build() {
    _$WorkOrderListResponse _$result;
    try {
      _$result = _$v ??
          _$WorkOrderListResponse._(
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
            r'WorkOrderListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
