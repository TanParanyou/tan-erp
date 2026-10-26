// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderListItemResponse extends WorkOrderListItemResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final String? itemCode;
  @override
  final String? itemNameTh;
  @override
  final double? plannedQuantity;
  @override
  final double? completedQuantity;
  @override
  final String? projectCode;
  @override
  final DateTime? createdAtUtc;

  factory _$WorkOrderListItemResponse(
          [void Function(WorkOrderListItemResponseBuilder)? updates]) =>
      (WorkOrderListItemResponseBuilder()..update(updates))._build();

  _$WorkOrderListItemResponse._(
      {this.id,
      this.number,
      this.status,
      this.itemCode,
      this.itemNameTh,
      this.plannedQuantity,
      this.completedQuantity,
      this.projectCode,
      this.createdAtUtc})
      : super._();
  @override
  WorkOrderListItemResponse rebuild(
          void Function(WorkOrderListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderListItemResponseBuilder toBuilder() =>
      WorkOrderListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderListItemResponse &&
        id == other.id &&
        number == other.number &&
        status == other.status &&
        itemCode == other.itemCode &&
        itemNameTh == other.itemNameTh &&
        plannedQuantity == other.plannedQuantity &&
        completedQuantity == other.completedQuantity &&
        projectCode == other.projectCode &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, itemNameTh.hashCode);
    _$hash = $jc(_$hash, plannedQuantity.hashCode);
    _$hash = $jc(_$hash, completedQuantity.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderListItemResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('status', status)
          ..add('itemCode', itemCode)
          ..add('itemNameTh', itemNameTh)
          ..add('plannedQuantity', plannedQuantity)
          ..add('completedQuantity', completedQuantity)
          ..add('projectCode', projectCode)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class WorkOrderListItemResponseBuilder
    implements
        Builder<WorkOrderListItemResponse, WorkOrderListItemResponseBuilder> {
  _$WorkOrderListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _itemCode;
  String? get itemCode => _$this._itemCode;
  set itemCode(String? itemCode) => _$this._itemCode = itemCode;

  String? _itemNameTh;
  String? get itemNameTh => _$this._itemNameTh;
  set itemNameTh(String? itemNameTh) => _$this._itemNameTh = itemNameTh;

  double? _plannedQuantity;
  double? get plannedQuantity => _$this._plannedQuantity;
  set plannedQuantity(double? plannedQuantity) =>
      _$this._plannedQuantity = plannedQuantity;

  double? _completedQuantity;
  double? get completedQuantity => _$this._completedQuantity;
  set completedQuantity(double? completedQuantity) =>
      _$this._completedQuantity = completedQuantity;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  WorkOrderListItemResponseBuilder() {
    WorkOrderListItemResponse._defaults(this);
  }

  WorkOrderListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _status = $v.status;
      _itemCode = $v.itemCode;
      _itemNameTh = $v.itemNameTh;
      _plannedQuantity = $v.plannedQuantity;
      _completedQuantity = $v.completedQuantity;
      _projectCode = $v.projectCode;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderListItemResponse other) {
    _$v = other as _$WorkOrderListItemResponse;
  }

  @override
  void update(void Function(WorkOrderListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderListItemResponse build() => _build();

  _$WorkOrderListItemResponse _build() {
    final _$result = _$v ??
        _$WorkOrderListItemResponse._(
          id: id,
          number: number,
          status: status,
          itemCode: itemCode,
          itemNameTh: itemNameTh,
          plannedQuantity: plannedQuantity,
          completedQuantity: completedQuantity,
          projectCode: projectCode,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
