// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'outbox_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OutboxListResponse extends OutboxListResponse {
  @override
  final BuiltList<OutboxMessageResponse>? items;
  @override
  final FinancePaginationResponse? pagination;

  factory _$OutboxListResponse(
          [void Function(OutboxListResponseBuilder)? updates]) =>
      (OutboxListResponseBuilder()..update(updates))._build();

  _$OutboxListResponse._({this.items, this.pagination}) : super._();
  @override
  OutboxListResponse rebuild(
          void Function(OutboxListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OutboxListResponseBuilder toBuilder() =>
      OutboxListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OutboxListResponse &&
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
    return (newBuiltValueToStringHelper(r'OutboxListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class OutboxListResponseBuilder
    implements Builder<OutboxListResponse, OutboxListResponseBuilder> {
  _$OutboxListResponse? _$v;

  ListBuilder<OutboxMessageResponse>? _items;
  ListBuilder<OutboxMessageResponse> get items =>
      _$this._items ??= ListBuilder<OutboxMessageResponse>();
  set items(ListBuilder<OutboxMessageResponse>? items) => _$this._items = items;

  FinancePaginationResponseBuilder? _pagination;
  FinancePaginationResponseBuilder get pagination =>
      _$this._pagination ??= FinancePaginationResponseBuilder();
  set pagination(FinancePaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  OutboxListResponseBuilder() {
    OutboxListResponse._defaults(this);
  }

  OutboxListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OutboxListResponse other) {
    _$v = other as _$OutboxListResponse;
  }

  @override
  void update(void Function(OutboxListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OutboxListResponse build() => _build();

  _$OutboxListResponse _build() {
    _$OutboxListResponse _$result;
    try {
      _$result = _$v ??
          _$OutboxListResponse._(
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
            r'OutboxListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
