// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderResponse extends WorkOrderResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final ProductionItemResponse? item;
  @override
  final String? bomRevisionId;
  @override
  final String? bomCode;
  @override
  final int? bomRevisionNo;
  @override
  final WorkOrderRefResponse? warehouse;
  @override
  final WorkOrderRefResponse? project;
  @override
  final double? plannedQuantity;
  @override
  final double? completedQuantity;
  @override
  final double? costAllocated;
  @override
  final String? note;
  @override
  final String? cancelReason;
  @override
  final ProductionPersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? rowVersion;
  @override
  final BuiltList<WorkOrderMaterialResponse>? materials;
  @override
  final BuiltList<WorkOrderTransactionResponse>? transactions;

  factory _$WorkOrderResponse(
          [void Function(WorkOrderResponseBuilder)? updates]) =>
      (WorkOrderResponseBuilder()..update(updates))._build();

  _$WorkOrderResponse._(
      {this.id,
      this.branchId,
      this.number,
      this.status,
      this.item,
      this.bomRevisionId,
      this.bomCode,
      this.bomRevisionNo,
      this.warehouse,
      this.project,
      this.plannedQuantity,
      this.completedQuantity,
      this.costAllocated,
      this.note,
      this.cancelReason,
      this.createdBy,
      this.createdAtUtc,
      this.rowVersion,
      this.materials,
      this.transactions})
      : super._();
  @override
  WorkOrderResponse rebuild(void Function(WorkOrderResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderResponseBuilder toBuilder() =>
      WorkOrderResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderResponse &&
        id == other.id &&
        branchId == other.branchId &&
        number == other.number &&
        status == other.status &&
        item == other.item &&
        bomRevisionId == other.bomRevisionId &&
        bomCode == other.bomCode &&
        bomRevisionNo == other.bomRevisionNo &&
        warehouse == other.warehouse &&
        project == other.project &&
        plannedQuantity == other.plannedQuantity &&
        completedQuantity == other.completedQuantity &&
        costAllocated == other.costAllocated &&
        note == other.note &&
        cancelReason == other.cancelReason &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        rowVersion == other.rowVersion &&
        materials == other.materials &&
        transactions == other.transactions;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, bomRevisionId.hashCode);
    _$hash = $jc(_$hash, bomCode.hashCode);
    _$hash = $jc(_$hash, bomRevisionNo.hashCode);
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, project.hashCode);
    _$hash = $jc(_$hash, plannedQuantity.hashCode);
    _$hash = $jc(_$hash, completedQuantity.hashCode);
    _$hash = $jc(_$hash, costAllocated.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, cancelReason.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, materials.hashCode);
    _$hash = $jc(_$hash, transactions.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('number', number)
          ..add('status', status)
          ..add('item', item)
          ..add('bomRevisionId', bomRevisionId)
          ..add('bomCode', bomCode)
          ..add('bomRevisionNo', bomRevisionNo)
          ..add('warehouse', warehouse)
          ..add('project', project)
          ..add('plannedQuantity', plannedQuantity)
          ..add('completedQuantity', completedQuantity)
          ..add('costAllocated', costAllocated)
          ..add('note', note)
          ..add('cancelReason', cancelReason)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('rowVersion', rowVersion)
          ..add('materials', materials)
          ..add('transactions', transactions))
        .toString();
  }
}

class WorkOrderResponseBuilder
    implements Builder<WorkOrderResponse, WorkOrderResponseBuilder> {
  _$WorkOrderResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  ProductionItemResponseBuilder? _item;
  ProductionItemResponseBuilder get item =>
      _$this._item ??= ProductionItemResponseBuilder();
  set item(ProductionItemResponseBuilder? item) => _$this._item = item;

  String? _bomRevisionId;
  String? get bomRevisionId => _$this._bomRevisionId;
  set bomRevisionId(String? bomRevisionId) =>
      _$this._bomRevisionId = bomRevisionId;

  String? _bomCode;
  String? get bomCode => _$this._bomCode;
  set bomCode(String? bomCode) => _$this._bomCode = bomCode;

  int? _bomRevisionNo;
  int? get bomRevisionNo => _$this._bomRevisionNo;
  set bomRevisionNo(int? bomRevisionNo) =>
      _$this._bomRevisionNo = bomRevisionNo;

  WorkOrderRefResponseBuilder? _warehouse;
  WorkOrderRefResponseBuilder get warehouse =>
      _$this._warehouse ??= WorkOrderRefResponseBuilder();
  set warehouse(WorkOrderRefResponseBuilder? warehouse) =>
      _$this._warehouse = warehouse;

  WorkOrderRefResponseBuilder? _project;
  WorkOrderRefResponseBuilder get project =>
      _$this._project ??= WorkOrderRefResponseBuilder();
  set project(WorkOrderRefResponseBuilder? project) =>
      _$this._project = project;

  double? _plannedQuantity;
  double? get plannedQuantity => _$this._plannedQuantity;
  set plannedQuantity(double? plannedQuantity) =>
      _$this._plannedQuantity = plannedQuantity;

  double? _completedQuantity;
  double? get completedQuantity => _$this._completedQuantity;
  set completedQuantity(double? completedQuantity) =>
      _$this._completedQuantity = completedQuantity;

  double? _costAllocated;
  double? get costAllocated => _$this._costAllocated;
  set costAllocated(double? costAllocated) =>
      _$this._costAllocated = costAllocated;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  String? _cancelReason;
  String? get cancelReason => _$this._cancelReason;
  set cancelReason(String? cancelReason) => _$this._cancelReason = cancelReason;

  ProductionPersonResponseBuilder? _createdBy;
  ProductionPersonResponseBuilder get createdBy =>
      _$this._createdBy ??= ProductionPersonResponseBuilder();
  set createdBy(ProductionPersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ListBuilder<WorkOrderMaterialResponse>? _materials;
  ListBuilder<WorkOrderMaterialResponse> get materials =>
      _$this._materials ??= ListBuilder<WorkOrderMaterialResponse>();
  set materials(ListBuilder<WorkOrderMaterialResponse>? materials) =>
      _$this._materials = materials;

  ListBuilder<WorkOrderTransactionResponse>? _transactions;
  ListBuilder<WorkOrderTransactionResponse> get transactions =>
      _$this._transactions ??= ListBuilder<WorkOrderTransactionResponse>();
  set transactions(ListBuilder<WorkOrderTransactionResponse>? transactions) =>
      _$this._transactions = transactions;

  WorkOrderResponseBuilder() {
    WorkOrderResponse._defaults(this);
  }

  WorkOrderResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _number = $v.number;
      _status = $v.status;
      _item = $v.item?.toBuilder();
      _bomRevisionId = $v.bomRevisionId;
      _bomCode = $v.bomCode;
      _bomRevisionNo = $v.bomRevisionNo;
      _warehouse = $v.warehouse?.toBuilder();
      _project = $v.project?.toBuilder();
      _plannedQuantity = $v.plannedQuantity;
      _completedQuantity = $v.completedQuantity;
      _costAllocated = $v.costAllocated;
      _note = $v.note;
      _cancelReason = $v.cancelReason;
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _rowVersion = $v.rowVersion;
      _materials = $v.materials?.toBuilder();
      _transactions = $v.transactions?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderResponse other) {
    _$v = other as _$WorkOrderResponse;
  }

  @override
  void update(void Function(WorkOrderResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderResponse build() => _build();

  _$WorkOrderResponse _build() {
    _$WorkOrderResponse _$result;
    try {
      _$result = _$v ??
          _$WorkOrderResponse._(
            id: id,
            branchId: branchId,
            number: number,
            status: status,
            item: _item?.build(),
            bomRevisionId: bomRevisionId,
            bomCode: bomCode,
            bomRevisionNo: bomRevisionNo,
            warehouse: _warehouse?.build(),
            project: _project?.build(),
            plannedQuantity: plannedQuantity,
            completedQuantity: completedQuantity,
            costAllocated: costAllocated,
            note: note,
            cancelReason: cancelReason,
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            rowVersion: rowVersion,
            materials: _materials?.build(),
            transactions: _transactions?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'item';
        _item?.build();

        _$failedField = 'warehouse';
        _warehouse?.build();
        _$failedField = 'project';
        _project?.build();

        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'materials';
        _materials?.build();
        _$failedField = 'transactions';
        _transactions?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WorkOrderResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
