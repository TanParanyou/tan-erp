// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_assign_role_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminAssignRoleResponse extends AdminAssignRoleResponse {
  @override
  final AdminUserResponse? user;
  @override
  final AdminRoleRequestResponse? pendingRequest;

  factory _$AdminAssignRoleResponse(
          [void Function(AdminAssignRoleResponseBuilder)? updates]) =>
      (AdminAssignRoleResponseBuilder()..update(updates))._build();

  _$AdminAssignRoleResponse._({this.user, this.pendingRequest}) : super._();
  @override
  AdminAssignRoleResponse rebuild(
          void Function(AdminAssignRoleResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminAssignRoleResponseBuilder toBuilder() =>
      AdminAssignRoleResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminAssignRoleResponse &&
        user == other.user &&
        pendingRequest == other.pendingRequest;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, user.hashCode);
    _$hash = $jc(_$hash, pendingRequest.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminAssignRoleResponse')
          ..add('user', user)
          ..add('pendingRequest', pendingRequest))
        .toString();
  }
}

class AdminAssignRoleResponseBuilder
    implements
        Builder<AdminAssignRoleResponse, AdminAssignRoleResponseBuilder> {
  _$AdminAssignRoleResponse? _$v;

  AdminUserResponseBuilder? _user;
  AdminUserResponseBuilder get user =>
      _$this._user ??= AdminUserResponseBuilder();
  set user(AdminUserResponseBuilder? user) => _$this._user = user;

  AdminRoleRequestResponseBuilder? _pendingRequest;
  AdminRoleRequestResponseBuilder get pendingRequest =>
      _$this._pendingRequest ??= AdminRoleRequestResponseBuilder();
  set pendingRequest(AdminRoleRequestResponseBuilder? pendingRequest) =>
      _$this._pendingRequest = pendingRequest;

  AdminAssignRoleResponseBuilder() {
    AdminAssignRoleResponse._defaults(this);
  }

  AdminAssignRoleResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _user = $v.user?.toBuilder();
      _pendingRequest = $v.pendingRequest?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminAssignRoleResponse other) {
    _$v = other as _$AdminAssignRoleResponse;
  }

  @override
  void update(void Function(AdminAssignRoleResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminAssignRoleResponse build() => _build();

  _$AdminAssignRoleResponse _build() {
    _$AdminAssignRoleResponse _$result;
    try {
      _$result = _$v ??
          _$AdminAssignRoleResponse._(
            user: _user?.build(),
            pendingRequest: _pendingRequest?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'user';
        _user?.build();
        _$failedField = 'pendingRequest';
        _pendingRequest?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AdminAssignRoleResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
