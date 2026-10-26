// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_rate_override_revoke_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchRateOverrideRevokeRequest
    extends BranchRateOverrideRevokeRequest {
  @override
  final String? reason;

  factory _$BranchRateOverrideRevokeRequest(
          [void Function(BranchRateOverrideRevokeRequestBuilder)? updates]) =>
      (BranchRateOverrideRevokeRequestBuilder()..update(updates))._build();

  _$BranchRateOverrideRevokeRequest._({this.reason}) : super._();
  @override
  BranchRateOverrideRevokeRequest rebuild(
          void Function(BranchRateOverrideRevokeRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchRateOverrideRevokeRequestBuilder toBuilder() =>
      BranchRateOverrideRevokeRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchRateOverrideRevokeRequest && reason == other.reason;
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
    return (newBuiltValueToStringHelper(r'BranchRateOverrideRevokeRequest')
          ..add('reason', reason))
        .toString();
  }
}

class BranchRateOverrideRevokeRequestBuilder
    implements
        Builder<BranchRateOverrideRevokeRequest,
            BranchRateOverrideRevokeRequestBuilder> {
  _$BranchRateOverrideRevokeRequest? _$v;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  BranchRateOverrideRevokeRequestBuilder() {
    BranchRateOverrideRevokeRequest._defaults(this);
  }

  BranchRateOverrideRevokeRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _reason = $v.reason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchRateOverrideRevokeRequest other) {
    _$v = other as _$BranchRateOverrideRevokeRequest;
  }

  @override
  void update(void Function(BranchRateOverrideRevokeRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchRateOverrideRevokeRequest build() => _build();

  _$BranchRateOverrideRevokeRequest _build() {
    final _$result = _$v ??
        _$BranchRateOverrideRevokeRequest._(
          reason: reason,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
