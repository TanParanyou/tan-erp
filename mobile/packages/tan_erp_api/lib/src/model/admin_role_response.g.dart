// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_role_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminRoleResponse extends AdminRoleResponse {
  @override
  final String? id;
  @override
  final String? name;
  @override
  final bool? assignable;
  @override
  final bool? requiresApproval;
  @override
  final BuiltList<String>? permissionKeys;

  factory _$AdminRoleResponse(
          [void Function(AdminRoleResponseBuilder)? updates]) =>
      (AdminRoleResponseBuilder()..update(updates))._build();

  _$AdminRoleResponse._(
      {this.id,
      this.name,
      this.assignable,
      this.requiresApproval,
      this.permissionKeys})
      : super._();
  @override
  AdminRoleResponse rebuild(void Function(AdminRoleResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminRoleResponseBuilder toBuilder() =>
      AdminRoleResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminRoleResponse &&
        id == other.id &&
        name == other.name &&
        assignable == other.assignable &&
        requiresApproval == other.requiresApproval &&
        permissionKeys == other.permissionKeys;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, assignable.hashCode);
    _$hash = $jc(_$hash, requiresApproval.hashCode);
    _$hash = $jc(_$hash, permissionKeys.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminRoleResponse')
          ..add('id', id)
          ..add('name', name)
          ..add('assignable', assignable)
          ..add('requiresApproval', requiresApproval)
          ..add('permissionKeys', permissionKeys))
        .toString();
  }
}

class AdminRoleResponseBuilder
    implements Builder<AdminRoleResponse, AdminRoleResponseBuilder> {
  _$AdminRoleResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  bool? _assignable;
  bool? get assignable => _$this._assignable;
  set assignable(bool? assignable) => _$this._assignable = assignable;

  bool? _requiresApproval;
  bool? get requiresApproval => _$this._requiresApproval;
  set requiresApproval(bool? requiresApproval) =>
      _$this._requiresApproval = requiresApproval;

  ListBuilder<String>? _permissionKeys;
  ListBuilder<String> get permissionKeys =>
      _$this._permissionKeys ??= ListBuilder<String>();
  set permissionKeys(ListBuilder<String>? permissionKeys) =>
      _$this._permissionKeys = permissionKeys;

  AdminRoleResponseBuilder() {
    AdminRoleResponse._defaults(this);
  }

  AdminRoleResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name;
      _assignable = $v.assignable;
      _requiresApproval = $v.requiresApproval;
      _permissionKeys = $v.permissionKeys?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminRoleResponse other) {
    _$v = other as _$AdminRoleResponse;
  }

  @override
  void update(void Function(AdminRoleResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminRoleResponse build() => _build();

  _$AdminRoleResponse _build() {
    _$AdminRoleResponse _$result;
    try {
      _$result = _$v ??
          _$AdminRoleResponse._(
            id: id,
            name: name,
            assignable: assignable,
            requiresApproval: requiresApproval,
            permissionKeys: _permissionKeys?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'permissionKeys';
        _permissionKeys?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AdminRoleResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
