// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_subcontract_operation_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderSubcontractOperationRequest
    extends WorkOrderSubcontractOperationRequest {
  @override
  final String? purchaseOrderId;
  @override
  final double? costAmount;
  @override
  final String? note;

  factory _$WorkOrderSubcontractOperationRequest(
          [void Function(WorkOrderSubcontractOperationRequestBuilder)?
              updates]) =>
      (WorkOrderSubcontractOperationRequestBuilder()..update(updates))._build();

  _$WorkOrderSubcontractOperationRequest._(
      {this.purchaseOrderId, this.costAmount, this.note})
      : super._();
  @override
  WorkOrderSubcontractOperationRequest rebuild(
          void Function(WorkOrderSubcontractOperationRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderSubcontractOperationRequestBuilder toBuilder() =>
      WorkOrderSubcontractOperationRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderSubcontractOperationRequest &&
        purchaseOrderId == other.purchaseOrderId &&
        costAmount == other.costAmount &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, purchaseOrderId.hashCode);
    _$hash = $jc(_$hash, costAmount.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderSubcontractOperationRequest')
          ..add('purchaseOrderId', purchaseOrderId)
          ..add('costAmount', costAmount)
          ..add('note', note))
        .toString();
  }
}

class WorkOrderSubcontractOperationRequestBuilder
    implements
        Builder<WorkOrderSubcontractOperationRequest,
            WorkOrderSubcontractOperationRequestBuilder> {
  _$WorkOrderSubcontractOperationRequest? _$v;

  String? _purchaseOrderId;
  String? get purchaseOrderId => _$this._purchaseOrderId;
  set purchaseOrderId(String? purchaseOrderId) =>
      _$this._purchaseOrderId = purchaseOrderId;

  double? _costAmount;
  double? get costAmount => _$this._costAmount;
  set costAmount(double? costAmount) => _$this._costAmount = costAmount;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  WorkOrderSubcontractOperationRequestBuilder() {
    WorkOrderSubcontractOperationRequest._defaults(this);
  }

  WorkOrderSubcontractOperationRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _purchaseOrderId = $v.purchaseOrderId;
      _costAmount = $v.costAmount;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderSubcontractOperationRequest other) {
    _$v = other as _$WorkOrderSubcontractOperationRequest;
  }

  @override
  void update(
      void Function(WorkOrderSubcontractOperationRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderSubcontractOperationRequest build() => _build();

  _$WorkOrderSubcontractOperationRequest _build() {
    final _$result = _$v ??
        _$WorkOrderSubcontractOperationRequest._(
          purchaseOrderId: purchaseOrderId,
          costAmount: costAmount,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
