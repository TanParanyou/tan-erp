// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_capabilities_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemCapabilitiesResponse extends ItemCapabilitiesResponse {
  @override
  final bool? canSell;
  @override
  final bool? canCost;
  @override
  final bool? canPurchase;
  @override
  final bool? canStock;
  @override
  final bool? canProduce;

  factory _$ItemCapabilitiesResponse(
          [void Function(ItemCapabilitiesResponseBuilder)? updates]) =>
      (ItemCapabilitiesResponseBuilder()..update(updates))._build();

  _$ItemCapabilitiesResponse._(
      {this.canSell,
      this.canCost,
      this.canPurchase,
      this.canStock,
      this.canProduce})
      : super._();
  @override
  ItemCapabilitiesResponse rebuild(
          void Function(ItemCapabilitiesResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemCapabilitiesResponseBuilder toBuilder() =>
      ItemCapabilitiesResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemCapabilitiesResponse &&
        canSell == other.canSell &&
        canCost == other.canCost &&
        canPurchase == other.canPurchase &&
        canStock == other.canStock &&
        canProduce == other.canProduce;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, canSell.hashCode);
    _$hash = $jc(_$hash, canCost.hashCode);
    _$hash = $jc(_$hash, canPurchase.hashCode);
    _$hash = $jc(_$hash, canStock.hashCode);
    _$hash = $jc(_$hash, canProduce.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemCapabilitiesResponse')
          ..add('canSell', canSell)
          ..add('canCost', canCost)
          ..add('canPurchase', canPurchase)
          ..add('canStock', canStock)
          ..add('canProduce', canProduce))
        .toString();
  }
}

class ItemCapabilitiesResponseBuilder
    implements
        Builder<ItemCapabilitiesResponse, ItemCapabilitiesResponseBuilder> {
  _$ItemCapabilitiesResponse? _$v;

  bool? _canSell;
  bool? get canSell => _$this._canSell;
  set canSell(bool? canSell) => _$this._canSell = canSell;

  bool? _canCost;
  bool? get canCost => _$this._canCost;
  set canCost(bool? canCost) => _$this._canCost = canCost;

  bool? _canPurchase;
  bool? get canPurchase => _$this._canPurchase;
  set canPurchase(bool? canPurchase) => _$this._canPurchase = canPurchase;

  bool? _canStock;
  bool? get canStock => _$this._canStock;
  set canStock(bool? canStock) => _$this._canStock = canStock;

  bool? _canProduce;
  bool? get canProduce => _$this._canProduce;
  set canProduce(bool? canProduce) => _$this._canProduce = canProduce;

  ItemCapabilitiesResponseBuilder() {
    ItemCapabilitiesResponse._defaults(this);
  }

  ItemCapabilitiesResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _canSell = $v.canSell;
      _canCost = $v.canCost;
      _canPurchase = $v.canPurchase;
      _canStock = $v.canStock;
      _canProduce = $v.canProduce;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemCapabilitiesResponse other) {
    _$v = other as _$ItemCapabilitiesResponse;
  }

  @override
  void update(void Function(ItemCapabilitiesResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemCapabilitiesResponse build() => _build();

  _$ItemCapabilitiesResponse _build() {
    final _$result = _$v ??
        _$ItemCapabilitiesResponse._(
          canSell: canSell,
          canCost: canCost,
          canPurchase: canPurchase,
          canStock: canStock,
          canProduce: canProduce,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
