// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_admin_user_two_factor_policy_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateAdminUserTwoFactorPolicyRequest
    extends UpdateAdminUserTwoFactorPolicyRequest {
  @override
  final bool? requireTwoFactor;

  factory _$UpdateAdminUserTwoFactorPolicyRequest(
          [void Function(UpdateAdminUserTwoFactorPolicyRequestBuilder)?
              updates]) =>
      (UpdateAdminUserTwoFactorPolicyRequestBuilder()..update(updates))
          ._build();

  _$UpdateAdminUserTwoFactorPolicyRequest._({this.requireTwoFactor})
      : super._();
  @override
  UpdateAdminUserTwoFactorPolicyRequest rebuild(
          void Function(UpdateAdminUserTwoFactorPolicyRequestBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateAdminUserTwoFactorPolicyRequestBuilder toBuilder() =>
      UpdateAdminUserTwoFactorPolicyRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateAdminUserTwoFactorPolicyRequest &&
        requireTwoFactor == other.requireTwoFactor;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, requireTwoFactor.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(
            r'UpdateAdminUserTwoFactorPolicyRequest')
          ..add('requireTwoFactor', requireTwoFactor))
        .toString();
  }
}

class UpdateAdminUserTwoFactorPolicyRequestBuilder
    implements
        Builder<UpdateAdminUserTwoFactorPolicyRequest,
            UpdateAdminUserTwoFactorPolicyRequestBuilder> {
  _$UpdateAdminUserTwoFactorPolicyRequest? _$v;

  bool? _requireTwoFactor;
  bool? get requireTwoFactor => _$this._requireTwoFactor;
  set requireTwoFactor(bool? requireTwoFactor) =>
      _$this._requireTwoFactor = requireTwoFactor;

  UpdateAdminUserTwoFactorPolicyRequestBuilder() {
    UpdateAdminUserTwoFactorPolicyRequest._defaults(this);
  }

  UpdateAdminUserTwoFactorPolicyRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _requireTwoFactor = $v.requireTwoFactor;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateAdminUserTwoFactorPolicyRequest other) {
    _$v = other as _$UpdateAdminUserTwoFactorPolicyRequest;
  }

  @override
  void update(
      void Function(UpdateAdminUserTwoFactorPolicyRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateAdminUserTwoFactorPolicyRequest build() => _build();

  _$UpdateAdminUserTwoFactorPolicyRequest _build() {
    final _$result = _$v ??
        _$UpdateAdminUserTwoFactorPolicyRequest._(
          requireTwoFactor: requireTwoFactor,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
