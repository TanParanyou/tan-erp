// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderRequest extends WorkOrderRequest {
  @override
  final String? itemId;
  @override
  final String? warehouseId;
  @override
  final String? projectId;
  @override
  final double? plannedQuantity;
  @override
  final String? note;

  factory _$WorkOrderRequest(
          [void Function(WorkOrderRequestBuilder)? updates]) =>
      (WorkOrderRequestBuilder()..update(updates))._build();

  _$WorkOrderRequest._(
      {this.itemId,
      this.warehouseId,
      this.projectId,
      this.plannedQuantity,
      this.note})
      : super._();
  @override
  WorkOrderRequest rebuild(void Function(WorkOrderRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderRequestBuilder toBuilder() =>
      WorkOrderRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderRequest &&
        itemId == other.itemId &&
        warehouseId == other.warehouseId &&
        projectId == other.projectId &&
        plannedQuantity == other.plannedQuantity &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, plannedQuantity.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderRequest')
          ..add('itemId', itemId)
          ..add('warehouseId', warehouseId)
          ..add('projectId', projectId)
          ..add('plannedQuantity', plannedQuantity)
          ..add('note', note))
        .toString();
  }
}

class WorkOrderRequestBuilder
    implements Builder<WorkOrderRequest, WorkOrderRequestBuilder> {
  _$WorkOrderRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  double? _plannedQuantity;
  double? get plannedQuantity => _$this._plannedQuantity;
  set plannedQuantity(double? plannedQuantity) =>
      _$this._plannedQuantity = plannedQuantity;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  WorkOrderRequestBuilder() {
    WorkOrderRequest._defaults(this);
  }

  WorkOrderRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _warehouseId = $v.warehouseId;
      _projectId = $v.projectId;
      _plannedQuantity = $v.plannedQuantity;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderRequest other) {
    _$v = other as _$WorkOrderRequest;
  }

  @override
  void update(void Function(WorkOrderRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderRequest build() => _build();

  _$WorkOrderRequest _build() {
    final _$result = _$v ??
        _$WorkOrderRequest._(
          itemId: itemId,
          warehouseId: warehouseId,
          projectId: projectId,
          plannedQuantity: plannedQuantity,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
