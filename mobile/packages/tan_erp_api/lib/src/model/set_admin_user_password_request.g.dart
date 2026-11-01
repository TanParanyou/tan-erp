// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'set_admin_user_password_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SetAdminUserPasswordRequest extends SetAdminUserPasswordRequest {
  @override
  final String? newPassword;

  factory _$SetAdminUserPasswordRequest(
          [void Function(SetAdminUserPasswordRequestBuilder)? updates]) =>
      (SetAdminUserPasswordRequestBuilder()..update(updates))._build();

  _$SetAdminUserPasswordRequest._({this.newPassword}) : super._();
  @override
  SetAdminUserPasswordRequest rebuild(
          void Function(SetAdminUserPasswordRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SetAdminUserPasswordRequestBuilder toBuilder() =>
      SetAdminUserPasswordRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SetAdminUserPasswordRequest &&
        newPassword == other.newPassword;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, newPassword.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SetAdminUserPasswordRequest')
          ..add('newPassword', newPassword))
        .toString();
  }
}

class SetAdminUserPasswordRequestBuilder
    implements
        Builder<SetAdminUserPasswordRequest,
            SetAdminUserPasswordRequestBuilder> {
  _$SetAdminUserPasswordRequest? _$v;

  String? _newPassword;
  String? get newPassword => _$this._newPassword;
  set newPassword(String? newPassword) => _$this._newPassword = newPassword;

  SetAdminUserPasswordRequestBuilder() {
    SetAdminUserPasswordRequest._defaults(this);
  }

  SetAdminUserPasswordRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _newPassword = $v.newPassword;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SetAdminUserPasswordRequest other) {
    _$v = other as _$SetAdminUserPasswordRequest;
  }

  @override
  void update(void Function(SetAdminUserPasswordRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SetAdminUserPasswordRequest build() => _build();

  _$SetAdminUserPasswordRequest _build() {
    final _$result = _$v ??
        _$SetAdminUserPasswordRequest._(
          newPassword: newPassword,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
