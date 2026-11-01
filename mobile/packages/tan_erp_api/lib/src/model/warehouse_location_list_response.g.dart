// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'warehouse_location_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WarehouseLocationListResponse extends WarehouseLocationListResponse {
  @override
  final BuiltList<WarehouseLocationResponse>? items;

  factory _$WarehouseLocationListResponse(
          [void Function(WarehouseLocationListResponseBuilder)? updates]) =>
      (WarehouseLocationListResponseBuilder()..update(updates))._build();

  _$WarehouseLocationListResponse._({this.items}) : super._();
  @override
  WarehouseLocationListResponse rebuild(
          void Function(WarehouseLocationListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WarehouseLocationListResponseBuilder toBuilder() =>
      WarehouseLocationListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WarehouseLocationListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'WarehouseLocationListResponse')
          ..add('items', items))
        .toString();
  }
}

class WarehouseLocationListResponseBuilder
    implements
        Builder<WarehouseLocationListResponse,
            WarehouseLocationListResponseBuilder> {
  _$WarehouseLocationListResponse? _$v;

  ListBuilder<WarehouseLocationResponse>? _items;
  ListBuilder<WarehouseLocationResponse> get items =>
      _$this._items ??= ListBuilder<WarehouseLocationResponse>();
  set items(ListBuilder<WarehouseLocationResponse>? items) =>
      _$this._items = items;

  WarehouseLocationListResponseBuilder() {
    WarehouseLocationListResponse._defaults(this);
  }

  WarehouseLocationListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WarehouseLocationListResponse other) {
    _$v = other as _$WarehouseLocationListResponse;
  }

  @override
  void update(void Function(WarehouseLocationListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WarehouseLocationListResponse build() => _build();

  _$WarehouseLocationListResponse _build() {
    _$WarehouseLocationListResponse _$result;
    try {
      _$result = _$v ??
          _$WarehouseLocationListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WarehouseLocationListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
