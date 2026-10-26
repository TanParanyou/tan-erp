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

  factory _$StockLineRequest(
          [void Function(StockLineRequestBuilder)? updates]) =>
      (StockLineRequestBuilder()..update(updates))._build();

  _$StockLineRequest._({this.itemId, this.quantity}) : super._();
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
        quantity == other.quantity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockLineRequest')
          ..add('itemId', itemId)
          ..add('quantity', quantity))
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

  StockLineRequestBuilder() {
    StockLineRequest._defaults(this);
  }

  StockLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _quantity = $v.quantity;
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
    final _$result = _$v ??
        _$StockLineRequest._(
          itemId: itemId,
          quantity: quantity,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
