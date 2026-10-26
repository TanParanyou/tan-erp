// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reconciliation_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReconciliationRowResponse extends ReconciliationRowResponse {
  @override
  final InventoryWarehouseRefResponse? warehouse;
  @override
  final InventoryItemRefResponse? item;
  @override
  final double? balanceOnHand;
  @override
  final double? ledgerQuantity;
  @override
  final double? balanceValue;
  @override
  final double? ledgerValue;
  @override
  final bool? isConsistent;

  factory _$ReconciliationRowResponse(
          [void Function(ReconciliationRowResponseBuilder)? updates]) =>
      (ReconciliationRowResponseBuilder()..update(updates))._build();

  _$ReconciliationRowResponse._(
      {this.warehouse,
      this.item,
      this.balanceOnHand,
      this.ledgerQuantity,
      this.balanceValue,
      this.ledgerValue,
      this.isConsistent})
      : super._();
  @override
  ReconciliationRowResponse rebuild(
          void Function(ReconciliationRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReconciliationRowResponseBuilder toBuilder() =>
      ReconciliationRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReconciliationRowResponse &&
        warehouse == other.warehouse &&
        item == other.item &&
        balanceOnHand == other.balanceOnHand &&
        ledgerQuantity == other.ledgerQuantity &&
        balanceValue == other.balanceValue &&
        ledgerValue == other.ledgerValue &&
        isConsistent == other.isConsistent;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, balanceOnHand.hashCode);
    _$hash = $jc(_$hash, ledgerQuantity.hashCode);
    _$hash = $jc(_$hash, balanceValue.hashCode);
    _$hash = $jc(_$hash, ledgerValue.hashCode);
    _$hash = $jc(_$hash, isConsistent.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReconciliationRowResponse')
          ..add('warehouse', warehouse)
          ..add('item', item)
          ..add('balanceOnHand', balanceOnHand)
          ..add('ledgerQuantity', ledgerQuantity)
          ..add('balanceValue', balanceValue)
          ..add('ledgerValue', ledgerValue)
          ..add('isConsistent', isConsistent))
        .toString();
  }
}

class ReconciliationRowResponseBuilder
    implements
        Builder<ReconciliationRowResponse, ReconciliationRowResponseBuilder> {
  _$ReconciliationRowResponse? _$v;

  InventoryWarehouseRefResponseBuilder? _warehouse;
  InventoryWarehouseRefResponseBuilder get warehouse =>
      _$this._warehouse ??= InventoryWarehouseRefResponseBuilder();
  set warehouse(InventoryWarehouseRefResponseBuilder? warehouse) =>
      _$this._warehouse = warehouse;

  InventoryItemRefResponseBuilder? _item;
  InventoryItemRefResponseBuilder get item =>
      _$this._item ??= InventoryItemRefResponseBuilder();
  set item(InventoryItemRefResponseBuilder? item) => _$this._item = item;

  double? _balanceOnHand;
  double? get balanceOnHand => _$this._balanceOnHand;
  set balanceOnHand(double? balanceOnHand) =>
      _$this._balanceOnHand = balanceOnHand;

  double? _ledgerQuantity;
  double? get ledgerQuantity => _$this._ledgerQuantity;
  set ledgerQuantity(double? ledgerQuantity) =>
      _$this._ledgerQuantity = ledgerQuantity;

  double? _balanceValue;
  double? get balanceValue => _$this._balanceValue;
  set balanceValue(double? balanceValue) => _$this._balanceValue = balanceValue;

  double? _ledgerValue;
  double? get ledgerValue => _$this._ledgerValue;
  set ledgerValue(double? ledgerValue) => _$this._ledgerValue = ledgerValue;

  bool? _isConsistent;
  bool? get isConsistent => _$this._isConsistent;
  set isConsistent(bool? isConsistent) => _$this._isConsistent = isConsistent;

  ReconciliationRowResponseBuilder() {
    ReconciliationRowResponse._defaults(this);
  }

  ReconciliationRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _warehouse = $v.warehouse?.toBuilder();
      _item = $v.item?.toBuilder();
      _balanceOnHand = $v.balanceOnHand;
      _ledgerQuantity = $v.ledgerQuantity;
      _balanceValue = $v.balanceValue;
      _ledgerValue = $v.ledgerValue;
      _isConsistent = $v.isConsistent;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReconciliationRowResponse other) {
    _$v = other as _$ReconciliationRowResponse;
  }

  @override
  void update(void Function(ReconciliationRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReconciliationRowResponse build() => _build();

  _$ReconciliationRowResponse _build() {
    _$ReconciliationRowResponse _$result;
    try {
      _$result = _$v ??
          _$ReconciliationRowResponse._(
            warehouse: _warehouse?.build(),
            item: _item?.build(),
            balanceOnHand: balanceOnHand,
            ledgerQuantity: ledgerQuantity,
            balanceValue: balanceValue,
            ledgerValue: ledgerValue,
            isConsistent: isConsistent,
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
            r'ReconciliationRowResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
