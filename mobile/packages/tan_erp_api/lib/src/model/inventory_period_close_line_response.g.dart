// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'inventory_period_close_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InventoryPeriodCloseLineResponse
    extends InventoryPeriodCloseLineResponse {
  @override
  final String? id;
  @override
  final InventoryWarehouseRefResponse? warehouse;
  @override
  final InventoryItemRefResponse? item;
  @override
  final double? onHandQuantity;
  @override
  final double? averageCost;
  @override
  final double? totalValue;

  factory _$InventoryPeriodCloseLineResponse(
          [void Function(InventoryPeriodCloseLineResponseBuilder)? updates]) =>
      (InventoryPeriodCloseLineResponseBuilder()..update(updates))._build();

  _$InventoryPeriodCloseLineResponse._(
      {this.id,
      this.warehouse,
      this.item,
      this.onHandQuantity,
      this.averageCost,
      this.totalValue})
      : super._();
  @override
  InventoryPeriodCloseLineResponse rebuild(
          void Function(InventoryPeriodCloseLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InventoryPeriodCloseLineResponseBuilder toBuilder() =>
      InventoryPeriodCloseLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InventoryPeriodCloseLineResponse &&
        id == other.id &&
        warehouse == other.warehouse &&
        item == other.item &&
        onHandQuantity == other.onHandQuantity &&
        averageCost == other.averageCost &&
        totalValue == other.totalValue;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, onHandQuantity.hashCode);
    _$hash = $jc(_$hash, averageCost.hashCode);
    _$hash = $jc(_$hash, totalValue.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'InventoryPeriodCloseLineResponse')
          ..add('id', id)
          ..add('warehouse', warehouse)
          ..add('item', item)
          ..add('onHandQuantity', onHandQuantity)
          ..add('averageCost', averageCost)
          ..add('totalValue', totalValue))
        .toString();
  }
}

class InventoryPeriodCloseLineResponseBuilder
    implements
        Builder<InventoryPeriodCloseLineResponse,
            InventoryPeriodCloseLineResponseBuilder> {
  _$InventoryPeriodCloseLineResponse? _$v;

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

  double? _onHandQuantity;
  double? get onHandQuantity => _$this._onHandQuantity;
  set onHandQuantity(double? onHandQuantity) =>
      _$this._onHandQuantity = onHandQuantity;

  double? _averageCost;
  double? get averageCost => _$this._averageCost;
  set averageCost(double? averageCost) => _$this._averageCost = averageCost;

  double? _totalValue;
  double? get totalValue => _$this._totalValue;
  set totalValue(double? totalValue) => _$this._totalValue = totalValue;

  InventoryPeriodCloseLineResponseBuilder() {
    InventoryPeriodCloseLineResponse._defaults(this);
  }

  InventoryPeriodCloseLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _warehouse = $v.warehouse?.toBuilder();
      _item = $v.item?.toBuilder();
      _onHandQuantity = $v.onHandQuantity;
      _averageCost = $v.averageCost;
      _totalValue = $v.totalValue;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(InventoryPeriodCloseLineResponse other) {
    _$v = other as _$InventoryPeriodCloseLineResponse;
  }

  @override
  void update(void Function(InventoryPeriodCloseLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InventoryPeriodCloseLineResponse build() => _build();

  _$InventoryPeriodCloseLineResponse _build() {
    _$InventoryPeriodCloseLineResponse _$result;
    try {
      _$result = _$v ??
          _$InventoryPeriodCloseLineResponse._(
            id: id,
            warehouse: _warehouse?.build(),
            item: _item?.build(),
            onHandQuantity: onHandQuantity,
            averageCost: averageCost,
            totalValue: totalValue,
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
            r'InventoryPeriodCloseLineResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
