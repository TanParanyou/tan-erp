// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_location_balance_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockLocationBalanceResponse extends StockLocationBalanceResponse {
  @override
  final String? id;
  @override
  final InventoryWarehouseRefResponse? warehouse;
  @override
  final InventoryLocationRefResponse? location;
  @override
  final InventoryItemRefResponse? item;
  @override
  final String? lotNumber;
  @override
  final double? onHand;
  @override
  final DateTime? updatedAtUtc;

  factory _$StockLocationBalanceResponse(
          [void Function(StockLocationBalanceResponseBuilder)? updates]) =>
      (StockLocationBalanceResponseBuilder()..update(updates))._build();

  _$StockLocationBalanceResponse._(
      {this.id,
      this.warehouse,
      this.location,
      this.item,
      this.lotNumber,
      this.onHand,
      this.updatedAtUtc})
      : super._();
  @override
  StockLocationBalanceResponse rebuild(
          void Function(StockLocationBalanceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockLocationBalanceResponseBuilder toBuilder() =>
      StockLocationBalanceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockLocationBalanceResponse &&
        id == other.id &&
        warehouse == other.warehouse &&
        location == other.location &&
        item == other.item &&
        lotNumber == other.lotNumber &&
        onHand == other.onHand &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, location.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, lotNumber.hashCode);
    _$hash = $jc(_$hash, onHand.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockLocationBalanceResponse')
          ..add('id', id)
          ..add('warehouse', warehouse)
          ..add('location', location)
          ..add('item', item)
          ..add('lotNumber', lotNumber)
          ..add('onHand', onHand)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class StockLocationBalanceResponseBuilder
    implements
        Builder<StockLocationBalanceResponse,
            StockLocationBalanceResponseBuilder> {
  _$StockLocationBalanceResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  InventoryWarehouseRefResponseBuilder? _warehouse;
  InventoryWarehouseRefResponseBuilder get warehouse =>
      _$this._warehouse ??= InventoryWarehouseRefResponseBuilder();
  set warehouse(InventoryWarehouseRefResponseBuilder? warehouse) =>
      _$this._warehouse = warehouse;

  InventoryLocationRefResponseBuilder? _location;
  InventoryLocationRefResponseBuilder get location =>
      _$this._location ??= InventoryLocationRefResponseBuilder();
  set location(InventoryLocationRefResponseBuilder? location) =>
      _$this._location = location;

  InventoryItemRefResponseBuilder? _item;
  InventoryItemRefResponseBuilder get item =>
      _$this._item ??= InventoryItemRefResponseBuilder();
  set item(InventoryItemRefResponseBuilder? item) => _$this._item = item;

  String? _lotNumber;
  String? get lotNumber => _$this._lotNumber;
  set lotNumber(String? lotNumber) => _$this._lotNumber = lotNumber;

  double? _onHand;
  double? get onHand => _$this._onHand;
  set onHand(double? onHand) => _$this._onHand = onHand;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  StockLocationBalanceResponseBuilder() {
    StockLocationBalanceResponse._defaults(this);
  }

  StockLocationBalanceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _warehouse = $v.warehouse?.toBuilder();
      _location = $v.location?.toBuilder();
      _item = $v.item?.toBuilder();
      _lotNumber = $v.lotNumber;
      _onHand = $v.onHand;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockLocationBalanceResponse other) {
    _$v = other as _$StockLocationBalanceResponse;
  }

  @override
  void update(void Function(StockLocationBalanceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockLocationBalanceResponse build() => _build();

  _$StockLocationBalanceResponse _build() {
    _$StockLocationBalanceResponse _$result;
    try {
      _$result = _$v ??
          _$StockLocationBalanceResponse._(
            id: id,
            warehouse: _warehouse?.build(),
            location: _location?.build(),
            item: _item?.build(),
            lotNumber: lotNumber,
            onHand: onHand,
            updatedAtUtc: updatedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'warehouse';
        _warehouse?.build();
        _$failedField = 'location';
        _location?.build();
        _$failedField = 'item';
        _item?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'StockLocationBalanceResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
