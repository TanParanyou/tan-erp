// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cancel_estimate_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CancelEstimateRequest extends CancelEstimateRequest {
  @override
  final String? reason;

  factory _$CancelEstimateRequest(
          [void Function(CancelEstimateRequestBuilder)? updates]) =>
      (CancelEstimateRequestBuilder()..update(updates))._build();

  _$CancelEstimateRequest._({this.reason}) : super._();
  @override
  CancelEstimateRequest rebuild(
          void Function(CancelEstimateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CancelEstimateRequestBuilder toBuilder() =>
      CancelEstimateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CancelEstimateRequest && reason == other.reason;
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
    return (newBuiltValueToStringHelper(r'CancelEstimateRequest')
          ..add('reason', reason))
        .toString();
  }
}

class CancelEstimateRequestBuilder
    implements Builder<CancelEstimateRequest, CancelEstimateRequestBuilder> {
  _$CancelEstimateRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  CancelEstimateRequestBuilder() {
    CancelEstimateRequest._defaults(this);
  }

  CancelEstimateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CancelEstimateRequest other) {
    _$v = other as _$CancelEstimateRequest;
  }

  @override
  void update(void Function(CancelEstimateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CancelEstimateRequest build() => _build();

  _$CancelEstimateRequest _build() {
    final _$result = _$v ??
        _$CancelEstimateRequest._(
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
