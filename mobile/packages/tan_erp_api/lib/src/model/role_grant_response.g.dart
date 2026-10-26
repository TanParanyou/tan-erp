// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'role_grant_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RoleGrantResponse extends RoleGrantResponse {
  @override
  final String? permissionKey;
  @override
  final String? scope;
  @override
  final AdminRefResponse? branch;

  factory _$RoleGrantResponse(
          [void Function(RoleGrantResponseBuilder)? updates]) =>
      (RoleGrantResponseBuilder()..update(updates))._build();

  _$RoleGrantResponse._({this.permissionKey, this.scope, this.branch})
      : super._();
  @override
  RoleGrantResponse rebuild(void Function(RoleGrantResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RoleGrantResponseBuilder toBuilder() =>
      RoleGrantResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RoleGrantResponse &&
        permissionKey == other.permissionKey &&
        scope == other.scope &&
        branch == other.branch;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, permissionKey.hashCode);
    _$hash = $jc(_$hash, scope.hashCode);
    _$hash = $jc(_$hash, branch.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RoleGrantResponse')
          ..add('permissionKey', permissionKey)
          ..add('scope', scope)
          ..add('branch', branch))
        .toString();
  }
}

class RoleGrantResponseBuilder
    implements Builder<RoleGrantResponse, RoleGrantResponseBuilder> {
  _$RoleGrantResponse? _$v;

  String? _permissionKey;
  String? get permissionKey => _$this._permissionKey;
  set permissionKey(String? permissionKey) =>
      _$this._permissionKey = permissionKey;

  String? _scope;
  String? get scope => _$this._scope;
  set scope(String? scope) => _$this._scope = scope;

  AdminRefResponseBuilder? _branch;
  AdminRefResponseBuilder get branch =>
      _$this._branch ??= AdminRefResponseBuilder();
  set branch(AdminRefResponseBuilder? branch) => _$this._branch = branch;

  RoleGrantResponseBuilder() {
    RoleGrantResponse._defaults(this);
  }

  RoleGrantResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _permissionKey = $v.permissionKey;
      _scope = $v.scope;
      _branch = $v.branch?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RoleGrantResponse other) {
    _$v = other as _$RoleGrantResponse;
  }

  @override
  void update(void Function(RoleGrantResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RoleGrantResponse build() => _build();

  _$RoleGrantResponse _build() {
    _$RoleGrantResponse _$result;
    try {
      _$result = _$v ??
          _$RoleGrantResponse._(
            permissionKey: permissionKey,
            scope: scope,
            branch: _branch?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'branch';
        _branch?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'RoleGrantResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
