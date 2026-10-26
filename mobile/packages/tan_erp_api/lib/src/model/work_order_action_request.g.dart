// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_action_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderActionRequest extends WorkOrderActionRequest {
  @override
  final String? reason;

  factory _$WorkOrderActionRequest(
          [void Function(WorkOrderActionRequestBuilder)? updates]) =>
      (WorkOrderActionRequestBuilder()..update(updates))._build();

  _$WorkOrderActionRequest._({this.reason}) : super._();
  @override
  WorkOrderActionRequest rebuild(
          void Function(WorkOrderActionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderActionRequestBuilder toBuilder() =>
      WorkOrderActionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderActionRequest && reason == other.reason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderActionRequest')
          ..add('reason', reason))
        .toString();
  }
}

class WorkOrderActionRequestBuilder
    implements Builder<WorkOrderActionRequest, WorkOrderActionRequestBuilder> {
  _$WorkOrderActionRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  WorkOrderActionRequestBuilder() {
    WorkOrderActionRequest._defaults(this);
  }

  WorkOrderActionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderActionRequest other) {
    _$v = other as _$WorkOrderActionRequest;
  }

  @override
  void update(void Function(WorkOrderActionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderActionRequest build() => _build();

  _$WorkOrderActionRequest _build() {
    final _$result = _$v ??
        _$WorkOrderActionRequest._(
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
