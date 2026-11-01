// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'receive_goods_receipt_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReceiveGoodsReceiptLineRequest extends ReceiveGoodsReceiptLineRequest {
  @override
  final String? itemId;
  @override
  final double? quantity;
  @override
  final String? locationId;
  @override
  final String? lotNumber;
  @override
  final BuiltList<String>? serialNumbers;

  factory _$ReceiveGoodsReceiptLineRequest(
          [void Function(ReceiveGoodsReceiptLineRequestBuilder)? updates]) =>
      (ReceiveGoodsReceiptLineRequestBuilder()..update(updates))._build();

  _$ReceiveGoodsReceiptLineRequest._(
      {this.itemId,
      this.quantity,
      this.locationId,
      this.lotNumber,
      this.serialNumbers})
      : super._();
  @override
  ReceiveGoodsReceiptLineRequest rebuild(
          void Function(ReceiveGoodsReceiptLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReceiveGoodsReceiptLineRequestBuilder toBuilder() =>
      ReceiveGoodsReceiptLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReceiveGoodsReceiptLineRequest &&
        itemId == other.itemId &&
        quantity == other.quantity &&
        locationId == other.locationId &&
        lotNumber == other.lotNumber &&
        serialNumbers == other.serialNumbers;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, locationId.hashCode);
    _$hash = $jc(_$hash, lotNumber.hashCode);
    _$hash = $jc(_$hash, serialNumbers.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReceiveGoodsReceiptLineRequest')
          ..add('itemId', itemId)
          ..add('quantity', quantity)
          ..add('locationId', locationId)
          ..add('lotNumber', lotNumber)
          ..add('serialNumbers', serialNumbers))
        .toString();
  }
}

class ReceiveGoodsReceiptLineRequestBuilder
    implements
        Builder<ReceiveGoodsReceiptLineRequest,
            ReceiveGoodsReceiptLineRequestBuilder> {
  _$ReceiveGoodsReceiptLineRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

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

  ReceiveGoodsReceiptLineRequestBuilder() {
    ReceiveGoodsReceiptLineRequest._defaults(this);
  }

  ReceiveGoodsReceiptLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _quantity = $v.quantity;
      _locationId = $v.locationId;
      _lotNumber = $v.lotNumber;
      _serialNumbers = $v.serialNumbers?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReceiveGoodsReceiptLineRequest other) {
    _$v = other as _$ReceiveGoodsReceiptLineRequest;
  }

  @override
  void update(void Function(ReceiveGoodsReceiptLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReceiveGoodsReceiptLineRequest build() => _build();

  _$ReceiveGoodsReceiptLineRequest _build() {
    _$ReceiveGoodsReceiptLineRequest _$result;
    try {
      _$result = _$v ??
          _$ReceiveGoodsReceiptLineRequest._(
            itemId: itemId,
            quantity: quantity,
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
            r'ReceiveGoodsReceiptLineRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
