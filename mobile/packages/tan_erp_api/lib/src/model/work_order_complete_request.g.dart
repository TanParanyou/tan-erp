// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_complete_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderCompleteRequest extends WorkOrderCompleteRequest {
  @override
  final double? quantity;

  factory _$WorkOrderCompleteRequest(
          [void Function(WorkOrderCompleteRequestBuilder)? updates]) =>
      (WorkOrderCompleteRequestBuilder()..update(updates))._build();

  _$WorkOrderCompleteRequest._({this.quantity}) : super._();
  @override
  WorkOrderCompleteRequest rebuild(
          void Function(WorkOrderCompleteRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderCompleteRequestBuilder toBuilder() =>
      WorkOrderCompleteRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderCompleteRequest && quantity == other.quantity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderCompleteRequest')
          ..add('quantity', quantity))
        .toString();
  }
}

class WorkOrderCompleteRequestBuilder
    implements
        Builder<WorkOrderCompleteRequest, WorkOrderCompleteRequestBuilder> {
  _$WorkOrderCompleteRequest? _$v;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  WorkOrderCompleteRequestBuilder() {
    WorkOrderCompleteRequest._defaults(this);
  }

  WorkOrderCompleteRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _quantity = $v.quantity;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderCompleteRequest other) {
    _$v = other as _$WorkOrderCompleteRequest;
  }

  @override
  void update(void Function(WorkOrderCompleteRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderCompleteRequest build() => _build();

  _$WorkOrderCompleteRequest _build() {
    final _$result = _$v ??
        _$WorkOrderCompleteRequest._(
          quantity: quantity,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
