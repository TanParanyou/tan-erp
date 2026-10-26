// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'disable_cost_record_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DisableCostRecordRequest extends DisableCostRecordRequest {
  @override
  final String reason;

  factory _$DisableCostRecordRequest(
          [void Function(DisableCostRecordRequestBuilder)? updates]) =>
      (DisableCostRecordRequestBuilder()..update(updates))._build();

  _$DisableCostRecordRequest._({required this.reason}) : super._();
  @override
  DisableCostRecordRequest rebuild(
          void Function(DisableCostRecordRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DisableCostRecordRequestBuilder toBuilder() =>
      DisableCostRecordRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DisableCostRecordRequest && reason == other.reason;
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
    return (newBuiltValueToStringHelper(r'DisableCostRecordRequest')
          ..add('reason', reason))
        .toString();
  }
}

class DisableCostRecordRequestBuilder
    implements
        Builder<DisableCostRecordRequest, DisableCostRecordRequestBuilder> {
  _$DisableCostRecordRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  DisableCostRecordRequestBuilder() {
    DisableCostRecordRequest._defaults(this);
  }

  DisableCostRecordRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DisableCostRecordRequest other) {
    _$v = other as _$DisableCostRecordRequest;
  }

  @override
  void update(void Function(DisableCostRecordRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DisableCostRecordRequest build() => _build();

  _$DisableCostRecordRequest _build() {
    final _$result = _$v ??
        _$DisableCostRecordRequest._(
          reason: BuiltValueNullFieldError.checkNotNull(
              reason, r'DisableCostRecordRequest', 'reason'),
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
