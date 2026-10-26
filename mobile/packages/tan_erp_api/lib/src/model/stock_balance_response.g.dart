// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_balance_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockBalanceResponse extends StockBalanceResponse {
  @override
  final String? id;
  @override
  final InventoryWarehouseRefResponse? warehouse;
  @override
  final InventoryItemRefResponse? item;
  @override
  final double? onHand;
  @override
  final double? reserved;
  @override
  final double? available;
  @override
  final double? averageCost;
  @override
  final double? totalValue;
  @override
  final DateTime? updatedAtUtc;

  factory _$StockBalanceResponse(
          [void Function(StockBalanceResponseBuilder)? updates]) =>
      (StockBalanceResponseBuilder()..update(updates))._build();

  _$StockBalanceResponse._(
      {this.id,
      this.warehouse,
      this.item,
      this.onHand,
      this.reserved,
      this.available,
      this.averageCost,
      this.totalValue,
      this.updatedAtUtc})
      : super._();
  @override
  StockBalanceResponse rebuild(
          void Function(StockBalanceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockBalanceResponseBuilder toBuilder() =>
      StockBalanceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockBalanceResponse &&
        id == other.id &&
        warehouse == other.warehouse &&
        item == other.item &&
        onHand == other.onHand &&
        reserved == other.reserved &&
        available == other.available &&
        averageCost == other.averageCost &&
        totalValue == other.totalValue &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, onHand.hashCode);
    _$hash = $jc(_$hash, reserved.hashCode);
    _$hash = $jc(_$hash, available.hashCode);
    _$hash = $jc(_$hash, averageCost.hashCode);
    _$hash = $jc(_$hash, totalValue.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockBalanceResponse')
          ..add('id', id)
          ..add('warehouse', warehouse)
          ..add('item', item)
          ..add('onHand', onHand)
          ..add('reserved', reserved)
          ..add('available', available)
          ..add('averageCost', averageCost)
          ..add('totalValue', totalValue)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class StockBalanceResponseBuilder
    implements Builder<StockBalanceResponse, StockBalanceResponseBuilder> {
  _$StockBalanceResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  InventoryWarehouseRefResponseBuilder? _warehouse;
  InventoryWarehouseRefResponseBuilder get warehouse =>
      _$this._warehouse ??= InventoryWarehouseRefResponseBuilder();
  set warehouse(InventoryWarehouseRefResponseBuilder? warehouse) =>
      _$this._warehouse = warehouse;

  InventoryItemRefResponseBuilder? _item;
  InventoryItemRefResponseBuilder get item =>
      _$this._item ??= InventoryItemRefResponseBuilder();
  set item(InventoryItemRefResponseBuilder? item) => _$this._item = item;

  double? _onHand;
  double? get onHand => _$this._onHand;
  set onHand(double? onHand) => _$this._onHand = onHand;

  double? _reserved;
  double? get reserved => _$this._reserved;
  set reserved(double? reserved) => _$this._reserved = reserved;

  double? _available;
  double? get available => _$this._available;
  set available(double? available) => _$this._available = available;

  double? _averageCost;
  double? get averageCost => _$this._averageCost;
  set averageCost(double? averageCost) => _$this._averageCost = averageCost;

  double? _totalValue;
  double? get totalValue => _$this._totalValue;
  set totalValue(double? totalValue) => _$this._totalValue = totalValue;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  StockBalanceResponseBuilder() {
    StockBalanceResponse._defaults(this);
  }

  StockBalanceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _warehouse = $v.warehouse?.toBuilder();
      _item = $v.item?.toBuilder();
      _onHand = $v.onHand;
      _reserved = $v.reserved;
      _available = $v.available;
      _averageCost = $v.averageCost;
      _totalValue = $v.totalValue;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockBalanceResponse other) {
    _$v = other as _$StockBalanceResponse;
  }

  @override
  void update(void Function(StockBalanceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockBalanceResponse build() => _build();

  _$StockBalanceResponse _build() {
    _$StockBalanceResponse _$result;
    try {
      _$result = _$v ??
          _$StockBalanceResponse._(
            id: id,
            warehouse: _warehouse?.build(),
            item: _item?.build(),
            onHand: onHand,
            reserved: reserved,
            available: available,
            averageCost: averageCost,
            totalValue: totalValue,
            updatedAtUtc: updatedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'warehouse';
        _warehouse?.build();
        _$failedField = 'item';
        _item?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'StockBalanceResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
