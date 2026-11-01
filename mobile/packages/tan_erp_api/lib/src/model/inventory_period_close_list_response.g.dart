// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'inventory_period_close_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InventoryPeriodCloseListResponse
    extends InventoryPeriodCloseListResponse {
  @override
  final BuiltList<InventoryPeriodCloseResponse>? items;

  factory _$InventoryPeriodCloseListResponse(
          [void Function(InventoryPeriodCloseListResponseBuilder)? updates]) =>
      (InventoryPeriodCloseListResponseBuilder()..update(updates))._build();

  _$InventoryPeriodCloseListResponse._({this.items}) : super._();
  @override
  InventoryPeriodCloseListResponse rebuild(
          void Function(InventoryPeriodCloseListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InventoryPeriodCloseListResponseBuilder toBuilder() =>
      InventoryPeriodCloseListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InventoryPeriodCloseListResponse && items == other.items;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'InventoryPeriodCloseListResponse')
          ..add('items', items))
        .toString();
  }
}

class InventoryPeriodCloseListResponseBuilder
    implements
        Builder<InventoryPeriodCloseListResponse,
            InventoryPeriodCloseListResponseBuilder> {
  _$InventoryPeriodCloseListResponse? _$v;

  ListBuilder<InventoryPeriodCloseResponse>? _items;
  ListBuilder<InventoryPeriodCloseResponse> get items =>
      _$this._items ??= ListBuilder<InventoryPeriodCloseResponse>();
  set items(ListBuilder<InventoryPeriodCloseResponse>? items) =>
      _$this._items = items;

  InventoryPeriodCloseListResponseBuilder() {
    InventoryPeriodCloseListResponse._defaults(this);
  }

  InventoryPeriodCloseListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(InventoryPeriodCloseListResponse other) {
    _$v = other as _$InventoryPeriodCloseListResponse;
  }

  @override
  void update(void Function(InventoryPeriodCloseListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InventoryPeriodCloseListResponse build() => _build();

  _$InventoryPeriodCloseListResponse _build() {
    _$InventoryPeriodCloseListResponse _$result;
    try {
      _$result = _$v ??
          _$InventoryPeriodCloseListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'InventoryPeriodCloseListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
