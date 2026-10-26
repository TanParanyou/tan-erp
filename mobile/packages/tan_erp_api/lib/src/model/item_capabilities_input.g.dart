// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_capabilities_input.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemCapabilitiesInput extends ItemCapabilitiesInput {
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

  factory _$ItemCapabilitiesInput(
          [void Function(ItemCapabilitiesInputBuilder)? updates]) =>
      (ItemCapabilitiesInputBuilder()..update(updates))._build();

  _$ItemCapabilitiesInput._(
      {this.canSell,
      this.canCost,
      this.canPurchase,
      this.canStock,
      this.canProduce})
      : super._();
  @override
  ItemCapabilitiesInput rebuild(
          void Function(ItemCapabilitiesInputBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemCapabilitiesInputBuilder toBuilder() =>
      ItemCapabilitiesInputBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemCapabilitiesInput &&
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
    return (newBuiltValueToStringHelper(r'ItemCapabilitiesInput')
          ..add('canSell', canSell)
          ..add('canCost', canCost)
          ..add('canPurchase', canPurchase)
          ..add('canStock', canStock)
          ..add('canProduce', canProduce))
        .toString();
  }
}

class ItemCapabilitiesInputBuilder
    implements Builder<ItemCapabilitiesInput, ItemCapabilitiesInputBuilder> {
  _$ItemCapabilitiesInput? _$v;

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

  ItemCapabilitiesInputBuilder() {
    ItemCapabilitiesInput._defaults(this);
  }

  ItemCapabilitiesInputBuilder get _$this {
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
  void replace(ItemCapabilitiesInput other) {
    _$v = other as _$ItemCapabilitiesInput;
  }

  @override
  void update(void Function(ItemCapabilitiesInputBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemCapabilitiesInput build() => _build();

  _$ItemCapabilitiesInput _build() {
    final _$result = _$v ??
        _$ItemCapabilitiesInput._(
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
