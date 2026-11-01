// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_return_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierReturnLineRequest extends SupplierReturnLineRequest {
  @override
  final String? goodsReceiptLineId;
  @override
  final double? quantity;

  factory _$SupplierReturnLineRequest(
          [void Function(SupplierReturnLineRequestBuilder)? updates]) =>
      (SupplierReturnLineRequestBuilder()..update(updates))._build();

  _$SupplierReturnLineRequest._({this.goodsReceiptLineId, this.quantity})
      : super._();
  @override
  SupplierReturnLineRequest rebuild(
          void Function(SupplierReturnLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierReturnLineRequestBuilder toBuilder() =>
      SupplierReturnLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierReturnLineRequest &&
        goodsReceiptLineId == other.goodsReceiptLineId &&
        quantity == other.quantity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, goodsReceiptLineId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierReturnLineRequest')
          ..add('goodsReceiptLineId', goodsReceiptLineId)
          ..add('quantity', quantity))
        .toString();
  }
}

class SupplierReturnLineRequestBuilder
    implements
        Builder<SupplierReturnLineRequest, SupplierReturnLineRequestBuilder> {
  _$SupplierReturnLineRequest? _$v;

  String? _goodsReceiptLineId;
  String? get goodsReceiptLineId => _$this._goodsReceiptLineId;
  set goodsReceiptLineId(String? goodsReceiptLineId) =>
      _$this._goodsReceiptLineId = goodsReceiptLineId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  SupplierReturnLineRequestBuilder() {
    SupplierReturnLineRequest._defaults(this);
  }

  SupplierReturnLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _goodsReceiptLineId = $v.goodsReceiptLineId;
      _quantity = $v.quantity;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierReturnLineRequest other) {
    _$v = other as _$SupplierReturnLineRequest;
  }

  @override
  void update(void Function(SupplierReturnLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierReturnLineRequest build() => _build();

  _$SupplierReturnLineRequest _build() {
    final _$result = _$v ??
        _$SupplierReturnLineRequest._(
          goodsReceiptLineId: goodsReceiptLineId,
          quantity: quantity,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
