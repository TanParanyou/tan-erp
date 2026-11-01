// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_center_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkCenterListResponse extends WorkCenterListResponse {
  @override
  final BuiltList<WorkCenterResponse>? items;
  @override
  final ProductionPaginationResponse? pagination;

  factory _$WorkCenterListResponse(
          [void Function(WorkCenterListResponseBuilder)? updates]) =>
      (WorkCenterListResponseBuilder()..update(updates))._build();

  _$WorkCenterListResponse._({this.items, this.pagination}) : super._();
  @override
  WorkCenterListResponse rebuild(
          void Function(WorkCenterListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkCenterListResponseBuilder toBuilder() =>
      WorkCenterListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkCenterListResponse &&
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
    return (newBuiltValueToStringHelper(r'WorkCenterListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class WorkCenterListResponseBuilder
    implements Builder<WorkCenterListResponse, WorkCenterListResponseBuilder> {
  _$WorkCenterListResponse? _$v;

  ListBuilder<WorkCenterResponse>? _items;
  ListBuilder<WorkCenterResponse> get items =>
      _$this._items ??= ListBuilder<WorkCenterResponse>();
  set items(ListBuilder<WorkCenterResponse>? items) => _$this._items = items;

  ProductionPaginationResponseBuilder? _pagination;
  ProductionPaginationResponseBuilder get pagination =>
      _$this._pagination ??= ProductionPaginationResponseBuilder();
  set pagination(ProductionPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  WorkCenterListResponseBuilder() {
    WorkCenterListResponse._defaults(this);
  }

  WorkCenterListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkCenterListResponse other) {
    _$v = other as _$WorkCenterListResponse;
  }

  @override
  void update(void Function(WorkCenterListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkCenterListResponse build() => _build();

  _$WorkCenterListResponse _build() {
    _$WorkCenterListResponse _$result;
    try {
      _$result = _$v ??
          _$WorkCenterListResponse._(
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
            r'WorkCenterListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
