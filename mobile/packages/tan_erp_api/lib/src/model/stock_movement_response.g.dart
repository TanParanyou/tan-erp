// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_movement_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockMovementResponse extends StockMovementResponse {
  @override
  final String? id;
  @override
  final String? stockDocumentId;
  @override
  final String? documentType;
  @override
  final String? documentNumber;
  @override
  final InventoryWarehouseRefResponse? warehouse;
  @override
  final InventoryItemRefResponse? item;
  @override
  final String? kind;
  @override
  final double? quantityDelta;
  @override
  final double? unitCost;
  @override
  final double? valueDelta;
  @override
  final double? onHandAfter;
  @override
  final DateTime? occurredAtUtc;
  @override
  final DateTime? postedAtUtc;

  factory _$StockMovementResponse(
          [void Function(StockMovementResponseBuilder)? updates]) =>
      (StockMovementResponseBuilder()..update(updates))._build();

  _$StockMovementResponse._(
      {this.id,
      this.stockDocumentId,
      this.documentType,
      this.documentNumber,
      this.warehouse,
      this.item,
      this.kind,
      this.quantityDelta,
      this.unitCost,
      this.valueDelta,
      this.onHandAfter,
      this.occurredAtUtc,
      this.postedAtUtc})
      : super._();
  @override
  StockMovementResponse rebuild(
          void Function(StockMovementResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockMovementResponseBuilder toBuilder() =>
      StockMovementResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockMovementResponse &&
        id == other.id &&
        stockDocumentId == other.stockDocumentId &&
        documentType == other.documentType &&
        documentNumber == other.documentNumber &&
        warehouse == other.warehouse &&
        item == other.item &&
        kind == other.kind &&
        quantityDelta == other.quantityDelta &&
        unitCost == other.unitCost &&
        valueDelta == other.valueDelta &&
        onHandAfter == other.onHandAfter &&
        occurredAtUtc == other.occurredAtUtc &&
        postedAtUtc == other.postedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, stockDocumentId.hashCode);
    _$hash = $jc(_$hash, documentType.hashCode);
    _$hash = $jc(_$hash, documentNumber.hashCode);
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, quantityDelta.hashCode);
    _$hash = $jc(_$hash, unitCost.hashCode);
    _$hash = $jc(_$hash, valueDelta.hashCode);
    _$hash = $jc(_$hash, onHandAfter.hashCode);
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jc(_$hash, postedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockMovementResponse')
          ..add('id', id)
          ..add('stockDocumentId', stockDocumentId)
          ..add('documentType', documentType)
          ..add('documentNumber', documentNumber)
          ..add('warehouse', warehouse)
          ..add('item', item)
          ..add('kind', kind)
          ..add('quantityDelta', quantityDelta)
          ..add('unitCost', unitCost)
          ..add('valueDelta', valueDelta)
          ..add('onHandAfter', onHandAfter)
          ..add('occurredAtUtc', occurredAtUtc)
          ..add('postedAtUtc', postedAtUtc))
        .toString();
  }
}

class StockMovementResponseBuilder
    implements Builder<StockMovementResponse, StockMovementResponseBuilder> {
  _$StockMovementResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _stockDocumentId;
  String? get stockDocumentId => _$this._stockDocumentId;
  set stockDocumentId(String? stockDocumentId) =>
      _$this._stockDocumentId = stockDocumentId;

  String? _documentType;
  String? get documentType => _$this._documentType;
  set documentType(String? documentType) => _$this._documentType = documentType;

  String? _documentNumber;
  String? get documentNumber => _$this._documentNumber;
  set documentNumber(String? documentNumber) =>
      _$this._documentNumber = documentNumber;

  InventoryWarehouseRefResponseBuilder? _warehouse;
  InventoryWarehouseRefResponseBuilder get warehouse =>
      _$this._warehouse ??= InventoryWarehouseRefResponseBuilder();
  set warehouse(InventoryWarehouseRefResponseBuilder? warehouse) =>
      _$this._warehouse = warehouse;

  InventoryItemRefResponseBuilder? _item;
  InventoryItemRefResponseBuilder get item =>
      _$this._item ??= InventoryItemRefResponseBuilder();
  set item(InventoryItemRefResponseBuilder? item) => _$this._item = item;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  double? _quantityDelta;
  double? get quantityDelta => _$this._quantityDelta;
  set quantityDelta(double? quantityDelta) =>
      _$this._quantityDelta = quantityDelta;

  double? _unitCost;
  double? get unitCost => _$this._unitCost;
  set unitCost(double? unitCost) => _$this._unitCost = unitCost;

  double? _valueDelta;
  double? get valueDelta => _$this._valueDelta;
  set valueDelta(double? valueDelta) => _$this._valueDelta = valueDelta;

  double? _onHandAfter;
  double? get onHandAfter => _$this._onHandAfter;
  set onHandAfter(double? onHandAfter) => _$this._onHandAfter = onHandAfter;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  DateTime? _postedAtUtc;
  DateTime? get postedAtUtc => _$this._postedAtUtc;
  set postedAtUtc(DateTime? postedAtUtc) => _$this._postedAtUtc = postedAtUtc;

  StockMovementResponseBuilder() {
    StockMovementResponse._defaults(this);
  }

  StockMovementResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _stockDocumentId = $v.stockDocumentId;
      _documentType = $v.documentType;
      _documentNumber = $v.documentNumber;
      _warehouse = $v.warehouse?.toBuilder();
      _item = $v.item?.toBuilder();
      _kind = $v.kind;
      _quantityDelta = $v.quantityDelta;
      _unitCost = $v.unitCost;
      _valueDelta = $v.valueDelta;
      _onHandAfter = $v.onHandAfter;
      _occurredAtUtc = $v.occurredAtUtc;
      _postedAtUtc = $v.postedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockMovementResponse other) {
    _$v = other as _$StockMovementResponse;
  }

  @override
  void update(void Function(StockMovementResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockMovementResponse build() => _build();

  _$StockMovementResponse _build() {
    _$StockMovementResponse _$result;
    try {
      _$result = _$v ??
          _$StockMovementResponse._(
            id: id,
            stockDocumentId: stockDocumentId,
            documentType: documentType,
            documentNumber: documentNumber,
            warehouse: _warehouse?.build(),
            item: _item?.build(),
            kind: kind,
            quantityDelta: quantityDelta,
            unitCost: unitCost,
            valueDelta: valueDelta,
            onHandAfter: onHandAfter,
            occurredAtUtc: occurredAtUtc,
            postedAtUtc: postedAtUtc,
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
            r'StockMovementResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
