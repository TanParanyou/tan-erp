// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'finance_reason_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$FinanceReasonRequest extends FinanceReasonRequest {
  @override
  final String? reason;

  factory _$FinanceReasonRequest(
          [void Function(FinanceReasonRequestBuilder)? updates]) =>
      (FinanceReasonRequestBuilder()..update(updates))._build();

  _$FinanceReasonRequest._({this.reason}) : super._();
  @override
  FinanceReasonRequest rebuild(
          void Function(FinanceReasonRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  FinanceReasonRequestBuilder toBuilder() =>
      FinanceReasonRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is FinanceReasonRequest && reason == other.reason;
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
    return (newBuiltValueToStringHelper(r'FinanceReasonRequest')
          ..add('reason', reason))
        .toString();
  }
}

class FinanceReasonRequestBuilder
    implements Builder<FinanceReasonRequest, FinanceReasonRequestBuilder> {
  _$FinanceReasonRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  FinanceReasonRequestBuilder() {
    FinanceReasonRequest._defaults(this);
  }

  FinanceReasonRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(FinanceReasonRequest other) {
    _$v = other as _$FinanceReasonRequest;
  }

  @override
  void update(void Function(FinanceReasonRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  FinanceReasonRequest build() => _build();

  _$FinanceReasonRequest _build() {
    final _$result = _$v ??
        _$FinanceReasonRequest._(
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
