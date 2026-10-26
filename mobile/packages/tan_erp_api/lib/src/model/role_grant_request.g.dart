// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'role_grant_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RoleGrantRequest extends RoleGrantRequest {
  @override
  final String? permissionKey;
  @override
  final String? scope;
  @override
  final String? branchId;

  factory _$RoleGrantRequest(
          [void Function(RoleGrantRequestBuilder)? updates]) =>
      (RoleGrantRequestBuilder()..update(updates))._build();

  _$RoleGrantRequest._({this.permissionKey, this.scope, this.branchId})
      : super._();
  @override
  RoleGrantRequest rebuild(void Function(RoleGrantRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RoleGrantRequestBuilder toBuilder() =>
      RoleGrantRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RoleGrantRequest &&
        permissionKey == other.permissionKey &&
        scope == other.scope &&
        branchId == other.branchId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, permissionKey.hashCode);
    _$hash = $jc(_$hash, scope.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RoleGrantRequest')
          ..add('permissionKey', permissionKey)
          ..add('scope', scope)
          ..add('branchId', branchId))
        .toString();
  }
}

class RoleGrantRequestBuilder
    implements Builder<RoleGrantRequest, RoleGrantRequestBuilder> {
  _$RoleGrantRequest? _$v;

  String? _permissionKey;
  String? get permissionKey => _$this._permissionKey;
  set permissionKey(String? permissionKey) =>
      _$this._permissionKey = permissionKey;

  String? _scope;
  String? get scope => _$this._scope;
  set scope(String? scope) => _$this._scope = scope;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  RoleGrantRequestBuilder() {
    RoleGrantRequest._defaults(this);
  }

  RoleGrantRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _permissionKey = $v.permissionKey;
      _scope = $v.scope;
      _branchId = $v.branchId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RoleGrantRequest other) {
    _$v = other as _$RoleGrantRequest;
  }

  @override
  void update(void Function(RoleGrantRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RoleGrantRequest build() => _build();

  _$RoleGrantRequest _build() {
    final _$result = _$v ??
        _$RoleGrantRequest._(
          permissionKey: permissionKey,
          scope: scope,
          branchId: branchId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
