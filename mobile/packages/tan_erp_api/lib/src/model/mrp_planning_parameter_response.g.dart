// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_planning_parameter_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpPlanningParameterResponse extends MrpPlanningParameterResponse {
  @override
  final String? id;
  @override
  final MrpItemResponse? item;
  @override
  final MrpWarehouseResponse? warehouse;
  @override
  final double? safetyStock;
  @override
  final double? minOrderQuantity;
  @override
  final double? lotMultiple;
  @override
  final int? leadTimeDays;
  @override
  final DateTime? updatedAtUtc;
  @override
  final String? rowVersion;

  factory _$MrpPlanningParameterResponse(
          [void Function(MrpPlanningParameterResponseBuilder)? updates]) =>
      (MrpPlanningParameterResponseBuilder()..update(updates))._build();

  _$MrpPlanningParameterResponse._(
      {this.id,
      this.item,
      this.warehouse,
      this.safetyStock,
      this.minOrderQuantity,
      this.lotMultiple,
      this.leadTimeDays,
      this.updatedAtUtc,
      this.rowVersion})
      : super._();
  @override
  MrpPlanningParameterResponse rebuild(
          void Function(MrpPlanningParameterResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpPlanningParameterResponseBuilder toBuilder() =>
      MrpPlanningParameterResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpPlanningParameterResponse &&
        id == other.id &&
        item == other.item &&
        warehouse == other.warehouse &&
        safetyStock == other.safetyStock &&
        minOrderQuantity == other.minOrderQuantity &&
        lotMultiple == other.lotMultiple &&
        leadTimeDays == other.leadTimeDays &&
        updatedAtUtc == other.updatedAtUtc &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, safetyStock.hashCode);
    _$hash = $jc(_$hash, minOrderQuantity.hashCode);
    _$hash = $jc(_$hash, lotMultiple.hashCode);
    _$hash = $jc(_$hash, leadTimeDays.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpPlanningParameterResponse')
          ..add('id', id)
          ..add('item', item)
          ..add('warehouse', warehouse)
          ..add('safetyStock', safetyStock)
          ..add('minOrderQuantity', minOrderQuantity)
          ..add('lotMultiple', lotMultiple)
          ..add('leadTimeDays', leadTimeDays)
          ..add('updatedAtUtc', updatedAtUtc)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class MrpPlanningParameterResponseBuilder
    implements
        Builder<MrpPlanningParameterResponse,
            MrpPlanningParameterResponseBuilder> {
  _$MrpPlanningParameterResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  MrpItemResponseBuilder? _item;
  MrpItemResponseBuilder get item => _$this._item ??= MrpItemResponseBuilder();
  set item(MrpItemResponseBuilder? item) => _$this._item = item;

  MrpWarehouseResponseBuilder? _warehouse;
  MrpWarehouseResponseBuilder get warehouse =>
      _$this._warehouse ??= MrpWarehouseResponseBuilder();
  set warehouse(MrpWarehouseResponseBuilder? warehouse) =>
      _$this._warehouse = warehouse;

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

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  MrpPlanningParameterResponseBuilder() {
    MrpPlanningParameterResponse._defaults(this);
  }

  MrpPlanningParameterResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _item = $v.item?.toBuilder();
      _warehouse = $v.warehouse?.toBuilder();
      _safetyStock = $v.safetyStock;
      _minOrderQuantity = $v.minOrderQuantity;
      _lotMultiple = $v.lotMultiple;
      _leadTimeDays = $v.leadTimeDays;
      _updatedAtUtc = $v.updatedAtUtc;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpPlanningParameterResponse other) {
    _$v = other as _$MrpPlanningParameterResponse;
  }

  @override
  void update(void Function(MrpPlanningParameterResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpPlanningParameterResponse build() => _build();

  _$MrpPlanningParameterResponse _build() {
    _$MrpPlanningParameterResponse _$result;
    try {
      _$result = _$v ??
          _$MrpPlanningParameterResponse._(
            id: id,
            item: _item?.build(),
            warehouse: _warehouse?.build(),
            safetyStock: safetyStock,
            minOrderQuantity: minOrderQuantity,
            lotMultiple: lotMultiple,
            leadTimeDays: leadTimeDays,
            updatedAtUtc: updatedAtUtc,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'item';
        _item?.build();
        _$failedField = 'warehouse';
        _warehouse?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'MrpPlanningParameterResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
