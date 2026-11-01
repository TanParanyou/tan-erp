// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_planning_parameter_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpPlanningParameterRequest extends MrpPlanningParameterRequest {
  @override
  final String? itemId;
  @override
  final String? warehouseId;
  @override
  final double? safetyStock;
  @override
  final double? minOrderQuantity;
  @override
  final double? lotMultiple;
  @override
  final int? leadTimeDays;

  factory _$MrpPlanningParameterRequest(
          [void Function(MrpPlanningParameterRequestBuilder)? updates]) =>
      (MrpPlanningParameterRequestBuilder()..update(updates))._build();

  _$MrpPlanningParameterRequest._(
      {this.itemId,
      this.warehouseId,
      this.safetyStock,
      this.minOrderQuantity,
      this.lotMultiple,
      this.leadTimeDays})
      : super._();
  @override
  MrpPlanningParameterRequest rebuild(
          void Function(MrpPlanningParameterRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpPlanningParameterRequestBuilder toBuilder() =>
      MrpPlanningParameterRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpPlanningParameterRequest &&
        itemId == other.itemId &&
        warehouseId == other.warehouseId &&
        safetyStock == other.safetyStock &&
        minOrderQuantity == other.minOrderQuantity &&
        lotMultiple == other.lotMultiple &&
        leadTimeDays == other.leadTimeDays;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jc(_$hash, safetyStock.hashCode);
    _$hash = $jc(_$hash, minOrderQuantity.hashCode);
    _$hash = $jc(_$hash, lotMultiple.hashCode);
    _$hash = $jc(_$hash, leadTimeDays.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpPlanningParameterRequest')
          ..add('itemId', itemId)
          ..add('warehouseId', warehouseId)
          ..add('safetyStock', safetyStock)
          ..add('minOrderQuantity', minOrderQuantity)
          ..add('lotMultiple', lotMultiple)
          ..add('leadTimeDays', leadTimeDays))
        .toString();
  }
}

class MrpPlanningParameterRequestBuilder
    implements
        Builder<MrpPlanningParameterRequest,
            MrpPlanningParameterRequestBuilder> {
  _$MrpPlanningParameterRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  double? _safetyStock;
  double? get safetyStock => _$this._safetyStock;
  set safetyStock(double? safetyStock) => _$this._safetyStock = safetyStock;

  double? _minOrderQuantity;
  double? get minOrderQuantity => _$this._minOrderQuantity;
  set minOrderQuantity(double? minOrderQuantity) =>
      _$this._minOrderQuantity = minOrderQuantity;

  double? _lotMultiple;
  double? get lotMultiple => _$this._lotMultiple;
  set lotMultiple(double? lotMultiple) => _$this._lotMultiple = lotMultiple;

  int? _leadTimeDays;
  int? get leadTimeDays => _$this._leadTimeDays;
  set leadTimeDays(int? leadTimeDays) => _$this._leadTimeDays = leadTimeDays;

  MrpPlanningParameterRequestBuilder() {
    MrpPlanningParameterRequest._defaults(this);
  }

  MrpPlanningParameterRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _warehouseId = $v.warehouseId;
      _safetyStock = $v.safetyStock;
      _minOrderQuantity = $v.minOrderQuantity;
      _lotMultiple = $v.lotMultiple;
      _leadTimeDays = $v.leadTimeDays;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpPlanningParameterRequest other) {
    _$v = other as _$MrpPlanningParameterRequest;
  }

  @override
  void update(void Function(MrpPlanningParameterRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpPlanningParameterRequest build() => _build();

  _$MrpPlanningParameterRequest _build() {
    final _$result = _$v ??
        _$MrpPlanningParameterRequest._(
          itemId: itemId,
          warehouseId: warehouseId,
          safetyStock: safetyStock,
          minOrderQuantity: minOrderQuantity,
          lotMultiple: lotMultiple,
          leadTimeDays: leadTimeDays,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
