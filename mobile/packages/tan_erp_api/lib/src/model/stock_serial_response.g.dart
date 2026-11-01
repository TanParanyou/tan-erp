// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_serial_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockSerialResponse extends StockSerialResponse {
  @override
  final String? id;
  @override
  final InventoryItemRefResponse? item;
  @override
  final String? serialNumber;
  @override
  final InventoryWarehouseRefResponse? warehouse;
  @override
  final InventoryLocationRefResponse? location;
  @override
  final String? lotNumber;
  @override
  final String? status;
  @override
  final String? lastStockDocumentId;
  @override
  final DateTime? updatedAtUtc;

  factory _$StockSerialResponse(
          [void Function(StockSerialResponseBuilder)? updates]) =>
      (StockSerialResponseBuilder()..update(updates))._build();

  _$StockSerialResponse._(
      {this.id,
      this.item,
      this.serialNumber,
      this.warehouse,
      this.location,
      this.lotNumber,
      this.status,
      this.lastStockDocumentId,
      this.updatedAtUtc})
      : super._();
  @override
  StockSerialResponse rebuild(
          void Function(StockSerialResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockSerialResponseBuilder toBuilder() =>
      StockSerialResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockSerialResponse &&
        id == other.id &&
        item == other.item &&
        serialNumber == other.serialNumber &&
        warehouse == other.warehouse &&
        location == other.location &&
        lotNumber == other.lotNumber &&
        status == other.status &&
        lastStockDocumentId == other.lastStockDocumentId &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, serialNumber.hashCode);
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, location.hashCode);
    _$hash = $jc(_$hash, lotNumber.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, lastStockDocumentId.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockSerialResponse')
          ..add('id', id)
          ..add('item', item)
          ..add('serialNumber', serialNumber)
          ..add('warehouse', warehouse)
          ..add('location', location)
          ..add('lotNumber', lotNumber)
          ..add('status', status)
          ..add('lastStockDocumentId', lastStockDocumentId)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class StockSerialResponseBuilder
    implements Builder<StockSerialResponse, StockSerialResponseBuilder> {
  _$StockSerialResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  InventoryItemRefResponseBuilder? _item;
  InventoryItemRefResponseBuilder get item =>
      _$this._item ??= InventoryItemRefResponseBuilder();
  set item(InventoryItemRefResponseBuilder? item) => _$this._item = item;

  String? _serialNumber;
  String? get serialNumber => _$this._serialNumber;
  set serialNumber(String? serialNumber) => _$this._serialNumber = serialNumber;

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

  String? _lotNumber;
  String? get lotNumber => _$this._lotNumber;
  set lotNumber(String? lotNumber) => _$this._lotNumber = lotNumber;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _lastStockDocumentId;
  String? get lastStockDocumentId => _$this._lastStockDocumentId;
  set lastStockDocumentId(String? lastStockDocumentId) =>
      _$this._lastStockDocumentId = lastStockDocumentId;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  StockSerialResponseBuilder() {
    StockSerialResponse._defaults(this);
  }

  StockSerialResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _item = $v.item?.toBuilder();
      _serialNumber = $v.serialNumber;
      _warehouse = $v.warehouse?.toBuilder();
      _location = $v.location?.toBuilder();
      _lotNumber = $v.lotNumber;
      _status = $v.status;
      _lastStockDocumentId = $v.lastStockDocumentId;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockSerialResponse other) {
    _$v = other as _$StockSerialResponse;
  }

  @override
  void update(void Function(StockSerialResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockSerialResponse build() => _build();

  _$StockSerialResponse _build() {
    _$StockSerialResponse _$result;
    try {
      _$result = _$v ??
          _$StockSerialResponse._(
            id: id,
            item: _item?.build(),
            serialNumber: serialNumber,
            warehouse: _warehouse?.build(),
            location: _location?.build(),
            lotNumber: lotNumber,
            status: status,
            lastStockDocumentId: lastStockDocumentId,
            updatedAtUtc: updatedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'item';
        _item?.build();

        _$failedField = 'warehouse';
        _warehouse?.build();
        _$failedField = 'location';
        _location?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'StockSerialResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
