// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reservation_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReservationListResponse extends ReservationListResponse {
  @override
  final BuiltList<ReservationResponse>? items;
  @override
  final InventoryPaginationResponse? pagination;

  factory _$ReservationListResponse(
          [void Function(ReservationListResponseBuilder)? updates]) =>
      (ReservationListResponseBuilder()..update(updates))._build();

  _$ReservationListResponse._({this.items, this.pagination}) : super._();
  @override
  ReservationListResponse rebuild(
          void Function(ReservationListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReservationListResponseBuilder toBuilder() =>
      ReservationListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReservationListResponse &&
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
    return (newBuiltValueToStringHelper(r'ReservationListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class ReservationListResponseBuilder
    implements
        Builder<ReservationListResponse, ReservationListResponseBuilder> {
  _$ReservationListResponse? _$v;

  ListBuilder<ReservationResponse>? _items;
  ListBuilder<ReservationResponse> get items =>
      _$this._items ??= ListBuilder<ReservationResponse>();
  set items(ListBuilder<ReservationResponse>? items) => _$this._items = items;

  InventoryPaginationResponseBuilder? _pagination;
  InventoryPaginationResponseBuilder get pagination =>
      _$this._pagination ??= InventoryPaginationResponseBuilder();
  set pagination(InventoryPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  ReservationListResponseBuilder() {
    ReservationListResponse._defaults(this);
  }

  ReservationListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReservationListResponse other) {
    _$v = other as _$ReservationListResponse;
  }

  @override
  void update(void Function(ReservationListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReservationListResponse build() => _build();

  _$ReservationListResponse _build() {
    _$ReservationListResponse _$result;
    try {
      _$result = _$v ??
          _$ReservationListResponse._(
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
            r'ReservationListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
