// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'receive_goods_receipt_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReceiveGoodsReceiptRequest extends ReceiveGoodsReceiptRequest {
  @override
  final String? goodsReceiptId;
  @override
  final String? warehouseId;

  factory _$ReceiveGoodsReceiptRequest(
          [void Function(ReceiveGoodsReceiptRequestBuilder)? updates]) =>
      (ReceiveGoodsReceiptRequestBuilder()..update(updates))._build();

  _$ReceiveGoodsReceiptRequest._({this.goodsReceiptId, this.warehouseId})
      : super._();
  @override
  ReceiveGoodsReceiptRequest rebuild(
          void Function(ReceiveGoodsReceiptRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReceiveGoodsReceiptRequestBuilder toBuilder() =>
      ReceiveGoodsReceiptRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReceiveGoodsReceiptRequest &&
        goodsReceiptId == other.goodsReceiptId &&
        warehouseId == other.warehouseId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, goodsReceiptId.hashCode);
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReceiveGoodsReceiptRequest')
          ..add('goodsReceiptId', goodsReceiptId)
          ..add('warehouseId', warehouseId))
        .toString();
  }
}

class ReceiveGoodsReceiptRequestBuilder
    implements
        Builder<ReceiveGoodsReceiptRequest, ReceiveGoodsReceiptRequestBuilder> {
  _$ReceiveGoodsReceiptRequest? _$v;

  String? _goodsReceiptId;
  String? get goodsReceiptId => _$this._goodsReceiptId;
  set goodsReceiptId(String? goodsReceiptId) =>
      _$this._goodsReceiptId = goodsReceiptId;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  ReceiveGoodsReceiptRequestBuilder() {
    ReceiveGoodsReceiptRequest._defaults(this);
  }

  ReceiveGoodsReceiptRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _goodsReceiptId = $v.goodsReceiptId;
      _warehouseId = $v.warehouseId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReceiveGoodsReceiptRequest other) {
    _$v = other as _$ReceiveGoodsReceiptRequest;
  }

  @override
  void update(void Function(ReceiveGoodsReceiptRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReceiveGoodsReceiptRequest build() => _build();

  _$ReceiveGoodsReceiptRequest _build() {
    final _$result = _$v ??
        _$ReceiveGoodsReceiptRequest._(
          goodsReceiptId: goodsReceiptId,
          warehouseId: warehouseId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
