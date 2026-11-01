// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'return_stock_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReturnStockLineRequest extends ReturnStockLineRequest {
  @override
  final String? itemId;
  @override
  final double? quantity;
  @override
  final double? unitCost;
  @override
  final String? locationId;
  @override
  final String? lotNumber;
  @override
  final BuiltList<String>? serialNumbers;

  factory _$ReturnStockLineRequest(
          [void Function(ReturnStockLineRequestBuilder)? updates]) =>
      (ReturnStockLineRequestBuilder()..update(updates))._build();

  _$ReturnStockLineRequest._(
      {this.itemId,
      this.quantity,
      this.unitCost,
      this.locationId,
      this.lotNumber,
      this.serialNumbers})
      : super._();
  @override
  ReturnStockLineRequest rebuild(
          void Function(ReturnStockLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReturnStockLineRequestBuilder toBuilder() =>
      ReturnStockLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReturnStockLineRequest &&
        itemId == other.itemId &&
        quantity == other.quantity &&
        unitCost == other.unitCost &&
        locationId == other.locationId &&
        lotNumber == other.lotNumber &&
        serialNumbers == other.serialNumbers;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitCost.hashCode);
    _$hash = $jc(_$hash, locationId.hashCode);
    _$hash = $jc(_$hash, lotNumber.hashCode);
    _$hash = $jc(_$hash, serialNumbers.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReturnStockLineRequest')
          ..add('itemId', itemId)
          ..add('quantity', quantity)
          ..add('unitCost', unitCost)
          ..add('locationId', locationId)
          ..add('lotNumber', lotNumber)
          ..add('serialNumbers', serialNumbers))
        .toString();
  }
}

class ReturnStockLineRequestBuilder
    implements Builder<ReturnStockLineRequest, ReturnStockLineRequestBuilder> {
  _$ReturnStockLineRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  double? _unitCost;
  double? get unitCost => _$this._unitCost;
  set unitCost(double? unitCost) => _$this._unitCost = unitCost;

  String? _locationId;
  String? get locationId => _$this._locationId;
  set locationId(String? locationId) => _$this._locationId = locationId;

  String? _lotNumber;
  String? get lotNumber => _$this._lotNumber;
  set lotNumber(String? lotNumber) => _$this._lotNumber = lotNumber;

  ListBuilder<String>? _serialNumbers;
  ListBuilder<String> get serialNumbers =>
      _$this._serialNumbers ??= ListBuilder<String>();
  set serialNumbers(ListBuilder<String>? serialNumbers) =>
      _$this._serialNumbers = serialNumbers;

  ReturnStockLineRequestBuilder() {
    ReturnStockLineRequest._defaults(this);
  }

  ReturnStockLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _quantity = $v.quantity;
      _unitCost = $v.unitCost;
      _locationId = $v.locationId;
      _lotNumber = $v.lotNumber;
      _serialNumbers = $v.serialNumbers?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReturnStockLineRequest other) {
    _$v = other as _$ReturnStockLineRequest;
  }

  @override
  void update(void Function(ReturnStockLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReturnStockLineRequest build() => _build();

  _$ReturnStockLineRequest _build() {
    _$ReturnStockLineRequest _$result;
    try {
      _$result = _$v ??
          _$ReturnStockLineRequest._(
            itemId: itemId,
            quantity: quantity,
            unitCost: unitCost,
            locationId: locationId,
            lotNumber: lotNumber,
            serialNumbers: _serialNumbers?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'serialNumbers';
        _serialNumbers?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ReturnStockLineRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
