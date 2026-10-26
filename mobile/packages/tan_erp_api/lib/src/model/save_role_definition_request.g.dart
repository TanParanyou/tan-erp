// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'save_role_definition_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SaveRoleDefinitionRequest extends SaveRoleDefinitionRequest {
  @override
  final String? name;
  @override
  final String? description;
  @override
  final BuiltList<RoleGrantRequest>? permissions;

  factory _$SaveRoleDefinitionRequest(
          [void Function(SaveRoleDefinitionRequestBuilder)? updates]) =>
      (SaveRoleDefinitionRequestBuilder()..update(updates))._build();

  _$SaveRoleDefinitionRequest._({this.name, this.description, this.permissions})
      : super._();
  @override
  SaveRoleDefinitionRequest rebuild(
          void Function(SaveRoleDefinitionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SaveRoleDefinitionRequestBuilder toBuilder() =>
      SaveRoleDefinitionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SaveRoleDefinitionRequest &&
        name == other.name &&
        description == other.description &&
        permissions == other.permissions;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, permissions.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SaveRoleDefinitionRequest')
          ..add('name', name)
          ..add('description', description)
          ..add('permissions', permissions))
        .toString();
  }
}

class SaveRoleDefinitionRequestBuilder
    implements
        Builder<SaveRoleDefinitionRequest, SaveRoleDefinitionRequestBuilder> {
  _$SaveRoleDefinitionRequest? _$v;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  ListBuilder<RoleGrantRequest>? _permissions;
  ListBuilder<RoleGrantRequest> get permissions =>
      _$this._permissions ??= ListBuilder<RoleGrantRequest>();
  set permissions(ListBuilder<RoleGrantRequest>? permissions) =>
      _$this._permissions = permissions;

  SaveRoleDefinitionRequestBuilder() {
    SaveRoleDefinitionRequest._defaults(this);
  }

  SaveRoleDefinitionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _description = $v.description;
      _permissions = $v.permissions?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SaveRoleDefinitionRequest other) {
    _$v = other as _$SaveRoleDefinitionRequest;
  }

  @override
  void update(void Function(SaveRoleDefinitionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SaveRoleDefinitionRequest build() => _build();

  _$SaveRoleDefinitionRequest _build() {
    _$SaveRoleDefinitionRequest _$result;
    try {
      _$result = _$v ??
          _$SaveRoleDefinitionRequest._(
            name: name,
            description: description,
            permissions: _permissions?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'permissions';
        _permissions?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SaveRoleDefinitionRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
