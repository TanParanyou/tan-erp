// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockLineRequest extends StockLineRequest {
  @override
  final String? itemId;
  @override
  final double? quantity;
  @override
  final String? locationId;
  @override
  final String? toLocationId;
  @override
  final String? lotNumber;
  @override
  final BuiltList<String>? serialNumbers;

  factory _$StockLineRequest(
          [void Function(StockLineRequestBuilder)? updates]) =>
      (StockLineRequestBuilder()..update(updates))._build();

  _$StockLineRequest._(
      {this.itemId,
      this.quantity,
      this.locationId,
      this.toLocationId,
      this.lotNumber,
      this.serialNumbers})
      : super._();
  @override
  StockLineRequest rebuild(void Function(StockLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockLineRequestBuilder toBuilder() =>
      StockLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockLineRequest &&
        itemId == other.itemId &&
        quantity == other.quantity &&
        locationId == other.locationId &&
        toLocationId == other.toLocationId &&
        lotNumber == other.lotNumber &&
        serialNumbers == other.serialNumbers;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, locationId.hashCode);
    _$hash = $jc(_$hash, toLocationId.hashCode);
    _$hash = $jc(_$hash, lotNumber.hashCode);
    _$hash = $jc(_$hash, serialNumbers.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockLineRequest')
          ..add('itemId', itemId)
          ..add('quantity', quantity)
          ..add('locationId', locationId)
          ..add('toLocationId', toLocationId)
          ..add('lotNumber', lotNumber)
          ..add('serialNumbers', serialNumbers))
        .toString();
  }
}

class StockLineRequestBuilder
    implements Builder<StockLineRequest, StockLineRequestBuilder> {
  _$StockLineRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  String? _locationId;
  String? get locationId => _$this._locationId;
  set locationId(String? locationId) => _$this._locationId = locationId;

  String? _toLocationId;
  String? get toLocationId => _$this._toLocationId;
  set toLocationId(String? toLocationId) => _$this._toLocationId = toLocationId;

  String? _lotNumber;
  String? get lotNumber => _$this._lotNumber;
  set lotNumber(String? lotNumber) => _$this._lotNumber = lotNumber;

  ListBuilder<String>? _serialNumbers;
  ListBuilder<String> get serialNumbers =>
      _$this._serialNumbers ??= ListBuilder<String>();
  set serialNumbers(ListBuilder<String>? serialNumbers) =>
      _$this._serialNumbers = serialNumbers;

  StockLineRequestBuilder() {
    StockLineRequest._defaults(this);
  }

  StockLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _quantity = $v.quantity;
      _locationId = $v.locationId;
      _toLocationId = $v.toLocationId;
      _lotNumber = $v.lotNumber;
      _serialNumbers = $v.serialNumbers?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockLineRequest other) {
    _$v = other as _$StockLineRequest;
  }

  @override
  void update(void Function(StockLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockLineRequest build() => _build();

  _$StockLineRequest _build() {
    _$StockLineRequest _$result;
    try {
      _$result = _$v ??
          _$StockLineRequest._(
            itemId: itemId,
            quantity: quantity,
            locationId: locationId,
            toLocationId: toLocationId,
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
            r'StockLineRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
