// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'assign_admin_role_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AssignAdminRoleRequest extends AssignAdminRoleRequest {
  @override
  final String? roleId;

  factory _$AssignAdminRoleRequest(
          [void Function(AssignAdminRoleRequestBuilder)? updates]) =>
      (AssignAdminRoleRequestBuilder()..update(updates))._build();

  _$AssignAdminRoleRequest._({this.roleId}) : super._();
  @override
  AssignAdminRoleRequest rebuild(
          void Function(AssignAdminRoleRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AssignAdminRoleRequestBuilder toBuilder() =>
      AssignAdminRoleRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AssignAdminRoleRequest && roleId == other.roleId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, roleId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AssignAdminRoleRequest')
          ..add('roleId', roleId))
        .toString();
  }
}

class AssignAdminRoleRequestBuilder
    implements Builder<AssignAdminRoleRequest, AssignAdminRoleRequestBuilder> {
  _$AssignAdminRoleRequest? _$v;

  String? _roleId;
  String? get roleId => _$this._roleId;
  set roleId(String? roleId) => _$this._roleId = roleId;

  AssignAdminRoleRequestBuilder() {
    AssignAdminRoleRequest._defaults(this);
  }

  AssignAdminRoleRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _roleId = $v.roleId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AssignAdminRoleRequest other) {
    _$v = other as _$AssignAdminRoleRequest;
  }

  @override
  void update(void Function(AssignAdminRoleRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AssignAdminRoleRequest build() => _build();

  _$AssignAdminRoleRequest _build() {
    final _$result = _$v ??
        _$AssignAdminRoleRequest._(
          roleId: roleId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
