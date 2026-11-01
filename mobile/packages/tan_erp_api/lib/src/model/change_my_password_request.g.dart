// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'change_my_password_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ChangeMyPasswordRequest extends ChangeMyPasswordRequest {
  @override
  final String? currentPassword;
  @override
  final String? newPassword;

  factory _$ChangeMyPasswordRequest(
          [void Function(ChangeMyPasswordRequestBuilder)? updates]) =>
      (ChangeMyPasswordRequestBuilder()..update(updates))._build();

  _$ChangeMyPasswordRequest._({this.currentPassword, this.newPassword})
      : super._();
  @override
  ChangeMyPasswordRequest rebuild(
          void Function(ChangeMyPasswordRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ChangeMyPasswordRequestBuilder toBuilder() =>
      ChangeMyPasswordRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ChangeMyPasswordRequest &&
        currentPassword == other.currentPassword &&
        newPassword == other.newPassword;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, currentPassword.hashCode);
    _$hash = $jc(_$hash, newPassword.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ChangeMyPasswordRequest')
          ..add('currentPassword', currentPassword)
          ..add('newPassword', newPassword))
        .toString();
  }
}

class ChangeMyPasswordRequestBuilder
    implements
        Builder<ChangeMyPasswordRequest, ChangeMyPasswordRequestBuilder> {
  _$ChangeMyPasswordRequest? _$v;

  String? _currentPassword;
  String? get currentPassword => _$this._currentPassword;
  set currentPassword(String? currentPassword) =>
      _$this._currentPassword = currentPassword;

  String? _newPassword;
  String? get newPassword => _$this._newPassword;
  set newPassword(String? newPassword) => _$this._newPassword = newPassword;

  ChangeMyPasswordRequestBuilder() {
    ChangeMyPasswordRequest._defaults(this);
  }

  ChangeMyPasswordRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _currentPassword = $v.currentPassword;
      _newPassword = $v.newPassword;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ChangeMyPasswordRequest other) {
    _$v = other as _$ChangeMyPasswordRequest;
  }

  @override
  void update(void Function(ChangeMyPasswordRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ChangeMyPasswordRequest build() => _build();

  _$ChangeMyPasswordRequest _build() {
    final _$result = _$v ??
        _$ChangeMyPasswordRequest._(
          currentPassword: currentPassword,
          newPassword: newPassword,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
