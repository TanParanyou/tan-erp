// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'return_cost_record_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReturnCostRecordRequest extends ReturnCostRecordRequest {
  @override
  final String reason;

  factory _$ReturnCostRecordRequest(
          [void Function(ReturnCostRecordRequestBuilder)? updates]) =>
      (ReturnCostRecordRequestBuilder()..update(updates))._build();

  _$ReturnCostRecordRequest._({required this.reason}) : super._();
  @override
  ReturnCostRecordRequest rebuild(
          void Function(ReturnCostRecordRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReturnCostRecordRequestBuilder toBuilder() =>
      ReturnCostRecordRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReturnCostRecordRequest && reason == other.reason;
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
    return (newBuiltValueToStringHelper(r'ReturnCostRecordRequest')
          ..add('reason', reason))
        .toString();
  }
}

class ReturnCostRecordRequestBuilder
    implements
        Builder<ReturnCostRecordRequest, ReturnCostRecordRequestBuilder> {
  _$ReturnCostRecordRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  ReturnCostRecordRequestBuilder() {
    ReturnCostRecordRequest._defaults(this);
  }

  ReturnCostRecordRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReturnCostRecordRequest other) {
    _$v = other as _$ReturnCostRecordRequest;
  }

  @override
  void update(void Function(ReturnCostRecordRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReturnCostRecordRequest build() => _build();

  _$ReturnCostRecordRequest _build() {
    final _$result = _$v ??
        _$ReturnCostRecordRequest._(
          reason: BuiltValueNullFieldError.checkNotNull(
              reason, r'ReturnCostRecordRequest', 'reason'),
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
