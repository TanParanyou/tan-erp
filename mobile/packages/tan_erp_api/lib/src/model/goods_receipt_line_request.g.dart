// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'goods_receipt_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$GoodsReceiptLineRequest extends GoodsReceiptLineRequest {
  @override
  final String? purchaseOrderLineId;
  @override
  final double? quantity;

  factory _$GoodsReceiptLineRequest(
          [void Function(GoodsReceiptLineRequestBuilder)? updates]) =>
      (GoodsReceiptLineRequestBuilder()..update(updates))._build();

  _$GoodsReceiptLineRequest._({this.purchaseOrderLineId, this.quantity})
      : super._();
  @override
  GoodsReceiptLineRequest rebuild(
          void Function(GoodsReceiptLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  GoodsReceiptLineRequestBuilder toBuilder() =>
      GoodsReceiptLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is GoodsReceiptLineRequest &&
        purchaseOrderLineId == other.purchaseOrderLineId &&
        quantity == other.quantity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, purchaseOrderLineId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'GoodsReceiptLineRequest')
          ..add('purchaseOrderLineId', purchaseOrderLineId)
          ..add('quantity', quantity))
        .toString();
  }
}

class GoodsReceiptLineRequestBuilder
    implements
        Builder<GoodsReceiptLineRequest, GoodsReceiptLineRequestBuilder> {
  _$GoodsReceiptLineRequest? _$v;

  String? _purchaseOrderLineId;
  String? get purchaseOrderLineId => _$this._purchaseOrderLineId;
  set purchaseOrderLineId(String? purchaseOrderLineId) =>
      _$this._purchaseOrderLineId = purchaseOrderLineId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  GoodsReceiptLineRequestBuilder() {
    GoodsReceiptLineRequest._defaults(this);
  }

  GoodsReceiptLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _purchaseOrderLineId = $v.purchaseOrderLineId;
      _quantity = $v.quantity;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(GoodsReceiptLineRequest other) {
    _$v = other as _$GoodsReceiptLineRequest;
  }

  @override
  void update(void Function(GoodsReceiptLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  GoodsReceiptLineRequest build() => _build();

  _$GoodsReceiptLineRequest _build() {
    final _$result = _$v ??
        _$GoodsReceiptLineRequest._(
          purchaseOrderLineId: purchaseOrderLineId,
          quantity: quantity,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
