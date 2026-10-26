// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_role_definition_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateRoleDefinitionResponse extends UpdateRoleDefinitionResponse {
  @override
  final RoleDefinitionResponse? role;
  @override
  final RoleChangeRequestResponse? pendingRequest;

  factory _$UpdateRoleDefinitionResponse(
          [void Function(UpdateRoleDefinitionResponseBuilder)? updates]) =>
      (UpdateRoleDefinitionResponseBuilder()..update(updates))._build();

  _$UpdateRoleDefinitionResponse._({this.role, this.pendingRequest})
      : super._();
  @override
  UpdateRoleDefinitionResponse rebuild(
          void Function(UpdateRoleDefinitionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateRoleDefinitionResponseBuilder toBuilder() =>
      UpdateRoleDefinitionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateRoleDefinitionResponse &&
        role == other.role &&
        pendingRequest == other.pendingRequest;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, role.hashCode);
    _$hash = $jc(_$hash, pendingRequest.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateRoleDefinitionResponse')
          ..add('role', role)
          ..add('pendingRequest', pendingRequest))
        .toString();
  }
}

class UpdateRoleDefinitionResponseBuilder
    implements
        Builder<UpdateRoleDefinitionResponse,
            UpdateRoleDefinitionResponseBuilder> {
  _$UpdateRoleDefinitionResponse? _$v;

  RoleDefinitionResponseBuilder? _role;
  RoleDefinitionResponseBuilder get role =>
      _$this._role ??= RoleDefinitionResponseBuilder();
  set role(RoleDefinitionResponseBuilder? role) => _$this._role = role;

  RoleChangeRequestResponseBuilder? _pendingRequest;
  RoleChangeRequestResponseBuilder get pendingRequest =>
      _$this._pendingRequest ??= RoleChangeRequestResponseBuilder();
  set pendingRequest(RoleChangeRequestResponseBuilder? pendingRequest) =>
      _$this._pendingRequest = pendingRequest;

  UpdateRoleDefinitionResponseBuilder() {
    UpdateRoleDefinitionResponse._defaults(this);
  }

  UpdateRoleDefinitionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _role = $v.role?.toBuilder();
      _pendingRequest = $v.pendingRequest?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateRoleDefinitionResponse other) {
    _$v = other as _$UpdateRoleDefinitionResponse;
  }

  @override
  void update(void Function(UpdateRoleDefinitionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateRoleDefinitionResponse build() => _build();

  _$UpdateRoleDefinitionResponse _build() {
    _$UpdateRoleDefinitionResponse _$result;
    try {
      _$result = _$v ??
          _$UpdateRoleDefinitionResponse._(
            role: _role?.build(),
            pendingRequest: _pendingRequest?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'role';
        _role?.build();
        _$failedField = 'pendingRequest';
        _pendingRequest?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateRoleDefinitionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
