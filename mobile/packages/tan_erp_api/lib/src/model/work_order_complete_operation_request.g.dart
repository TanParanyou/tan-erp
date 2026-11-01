// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_complete_operation_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderCompleteOperationRequest
    extends WorkOrderCompleteOperationRequest {
  @override
  final double? actualHours;
  @override
  final String? note;

  factory _$WorkOrderCompleteOperationRequest(
          [void Function(WorkOrderCompleteOperationRequestBuilder)? updates]) =>
      (WorkOrderCompleteOperationRequestBuilder()..update(updates))._build();

  _$WorkOrderCompleteOperationRequest._({this.actualHours, this.note})
      : super._();
  @override
  WorkOrderCompleteOperationRequest rebuild(
          void Function(WorkOrderCompleteOperationRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderCompleteOperationRequestBuilder toBuilder() =>
      WorkOrderCompleteOperationRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderCompleteOperationRequest &&
        actualHours == other.actualHours &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, actualHours.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderCompleteOperationRequest')
          ..add('actualHours', actualHours)
          ..add('note', note))
        .toString();
  }
}

class WorkOrderCompleteOperationRequestBuilder
    implements
        Builder<WorkOrderCompleteOperationRequest,
            WorkOrderCompleteOperationRequestBuilder> {
  _$WorkOrderCompleteOperationRequest? _$v;

  double? _actualHours;
  double? get actualHours => _$this._actualHours;
  set actualHours(double? actualHours) => _$this._actualHours = actualHours;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  WorkOrderCompleteOperationRequestBuilder() {
    WorkOrderCompleteOperationRequest._defaults(this);
  }

  WorkOrderCompleteOperationRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _actualHours = $v.actualHours;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderCompleteOperationRequest other) {
    _$v = other as _$WorkOrderCompleteOperationRequest;
  }

  @override
  void update(
      void Function(WorkOrderCompleteOperationRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderCompleteOperationRequest build() => _build();

  _$WorkOrderCompleteOperationRequest _build() {
    final _$result = _$v ??
        _$WorkOrderCompleteOperationRequest._(
          actualHours: actualHours,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
