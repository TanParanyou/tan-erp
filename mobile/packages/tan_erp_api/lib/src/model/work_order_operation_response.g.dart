// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_operation_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderOperationResponse extends WorkOrderOperationResponse {
  @override
  final String? id;
  @override
  final int? sequence;
  @override
  final String? operationCode;
  @override
  final String? operationName;
  @override
  final WorkCenterRefResponse? workCenter;
  @override
  final double? plannedSetupHours;
  @override
  final double? plannedRunHours;
  @override
  final double? actualHours;
  @override
  final bool? isSubcontracted;
  @override
  final WorkOrderRefResponse? subcontractPurchaseOrder;
  @override
  final double? subcontractCost;
  @override
  final String? status;
  @override
  final String? note;
  @override
  final DateTime? completedAtUtc;

  factory _$WorkOrderOperationResponse(
          [void Function(WorkOrderOperationResponseBuilder)? updates]) =>
      (WorkOrderOperationResponseBuilder()..update(updates))._build();

  _$WorkOrderOperationResponse._(
      {this.id,
      this.sequence,
      this.operationCode,
      this.operationName,
      this.workCenter,
      this.plannedSetupHours,
      this.plannedRunHours,
      this.actualHours,
      this.isSubcontracted,
      this.subcontractPurchaseOrder,
      this.subcontractCost,
      this.status,
      this.note,
      this.completedAtUtc})
      : super._();
  @override
  WorkOrderOperationResponse rebuild(
          void Function(WorkOrderOperationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderOperationResponseBuilder toBuilder() =>
      WorkOrderOperationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderOperationResponse &&
        id == other.id &&
        sequence == other.sequence &&
        operationCode == other.operationCode &&
        operationName == other.operationName &&
        workCenter == other.workCenter &&
        plannedSetupHours == other.plannedSetupHours &&
        plannedRunHours == other.plannedRunHours &&
        actualHours == other.actualHours &&
        isSubcontracted == other.isSubcontracted &&
        subcontractPurchaseOrder == other.subcontractPurchaseOrder &&
        subcontractCost == other.subcontractCost &&
        status == other.status &&
        note == other.note &&
        completedAtUtc == other.completedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, sequence.hashCode);
    _$hash = $jc(_$hash, operationCode.hashCode);
    _$hash = $jc(_$hash, operationName.hashCode);
    _$hash = $jc(_$hash, workCenter.hashCode);
    _$hash = $jc(_$hash, plannedSetupHours.hashCode);
    _$hash = $jc(_$hash, plannedRunHours.hashCode);
    _$hash = $jc(_$hash, actualHours.hashCode);
    _$hash = $jc(_$hash, isSubcontracted.hashCode);
    _$hash = $jc(_$hash, subcontractPurchaseOrder.hashCode);
    _$hash = $jc(_$hash, subcontractCost.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, completedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderOperationResponse')
          ..add('id', id)
          ..add('sequence', sequence)
          ..add('operationCode', operationCode)
          ..add('operationName', operationName)
          ..add('workCenter', workCenter)
          ..add('plannedSetupHours', plannedSetupHours)
          ..add('plannedRunHours', plannedRunHours)
          ..add('actualHours', actualHours)
          ..add('isSubcontracted', isSubcontracted)
          ..add('subcontractPurchaseOrder', subcontractPurchaseOrder)
          ..add('subcontractCost', subcontractCost)
          ..add('status', status)
          ..add('note', note)
          ..add('completedAtUtc', completedAtUtc))
        .toString();
  }
}

class WorkOrderOperationResponseBuilder
    implements
        Builder<WorkOrderOperationResponse, WorkOrderOperationResponseBuilder> {
  _$WorkOrderOperationResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _sequence;
  int? get sequence => _$this._sequence;
  set sequence(int? sequence) => _$this._sequence = sequence;

  String? _operationCode;
  String? get operationCode => _$this._operationCode;
  set operationCode(String? operationCode) =>
      _$this._operationCode = operationCode;

  String? _operationName;
  String? get operationName => _$this._operationName;
  set operationName(String? operationName) =>
      _$this._operationName = operationName;

  WorkCenterRefResponseBuilder? _workCenter;
  WorkCenterRefResponseBuilder get workCenter =>
      _$this._workCenter ??= WorkCenterRefResponseBuilder();
  set workCenter(WorkCenterRefResponseBuilder? workCenter) =>
      _$this._workCenter = workCenter;

  double? _plannedSetupHours;
  double? get plannedSetupHours => _$this._plannedSetupHours;
  set plannedSetupHours(double? plannedSetupHours) =>
      _$this._plannedSetupHours = plannedSetupHours;

  double? _plannedRunHours;
  double? get plannedRunHours => _$this._plannedRunHours;
  set plannedRunHours(double? plannedRunHours) =>
      _$this._plannedRunHours = plannedRunHours;

  double? _actualHours;
  double? get actualHours => _$this._actualHours;
  set actualHours(double? actualHours) => _$this._actualHours = actualHours;

  bool? _isSubcontracted;
  bool? get isSubcontracted => _$this._isSubcontracted;
  set isSubcontracted(bool? isSubcontracted) =>
      _$this._isSubcontracted = isSubcontracted;

  WorkOrderRefResponseBuilder? _subcontractPurchaseOrder;
  WorkOrderRefResponseBuilder get subcontractPurchaseOrder =>
      _$this._subcontractPurchaseOrder ??= WorkOrderRefResponseBuilder();
  set subcontractPurchaseOrder(
          WorkOrderRefResponseBuilder? subcontractPurchaseOrder) =>
      _$this._subcontractPurchaseOrder = subcontractPurchaseOrder;

  double? _subcontractCost;
  double? get subcontractCost => _$this._subcontractCost;
  set subcontractCost(double? subcontractCost) =>
      _$this._subcontractCost = subcontractCost;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  DateTime? _completedAtUtc;
  DateTime? get completedAtUtc => _$this._completedAtUtc;
  set completedAtUtc(DateTime? completedAtUtc) =>
      _$this._completedAtUtc = completedAtUtc;

  WorkOrderOperationResponseBuilder() {
    WorkOrderOperationResponse._defaults(this);
  }

  WorkOrderOperationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _sequence = $v.sequence;
      _operationCode = $v.operationCode;
      _operationName = $v.operationName;
      _workCenter = $v.workCenter?.toBuilder();
      _plannedSetupHours = $v.plannedSetupHours;
      _plannedRunHours = $v.plannedRunHours;
      _actualHours = $v.actualHours;
      _isSubcontracted = $v.isSubcontracted;
      _subcontractPurchaseOrder = $v.subcontractPurchaseOrder?.toBuilder();
      _subcontractCost = $v.subcontractCost;
      _status = $v.status;
      _note = $v.note;
      _completedAtUtc = $v.completedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderOperationResponse other) {
    _$v = other as _$WorkOrderOperationResponse;
  }

  @override
  void update(void Function(WorkOrderOperationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderOperationResponse build() => _build();

  _$WorkOrderOperationResponse _build() {
    _$WorkOrderOperationResponse _$result;
    try {
      _$result = _$v ??
          _$WorkOrderOperationResponse._(
            id: id,
            sequence: sequence,
            operationCode: operationCode,
            operationName: operationName,
            workCenter: _workCenter?.build(),
            plannedSetupHours: plannedSetupHours,
            plannedRunHours: plannedRunHours,
            actualHours: actualHours,
            isSubcontracted: isSubcontracted,
            subcontractPurchaseOrder: _subcontractPurchaseOrder?.build(),
            subcontractCost: subcontractCost,
            status: status,
            note: note,
            completedAtUtc: completedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'workCenter';
        _workCenter?.build();

        _$failedField = 'subcontractPurchaseOrder';
        _subcontractPurchaseOrder?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WorkOrderOperationResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
