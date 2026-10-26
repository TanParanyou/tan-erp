// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'permission_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PermissionDto extends PermissionDto {
  @override
  final String? key;
  @override
  final String? scope;
  @override
  final String? scopeId;

  factory _$PermissionDto([void Function(PermissionDtoBuilder)? updates]) =>
      (PermissionDtoBuilder()..update(updates))._build();

  _$PermissionDto._({this.key, this.scope, this.scopeId}) : super._();
  @override
  PermissionDto rebuild(void Function(PermissionDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PermissionDtoBuilder toBuilder() => PermissionDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PermissionDto &&
        key == other.key &&
        scope == other.scope &&
        scopeId == other.scopeId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, key.hashCode);
    _$hash = $jc(_$hash, scope.hashCode);
    _$hash = $jc(_$hash, scopeId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PermissionDto')
          ..add('key', key)
          ..add('scope', scope)
          ..add('scopeId', scopeId))
        .toString();
  }
}

class PermissionDtoBuilder
    implements Builder<PermissionDto, PermissionDtoBuilder> {
  _$PermissionDto? _$v;

  String? _key;
  String? get key => _$this._key;
  set key(String? key) => _$this._key = key;

  String? _scope;
  String? get scope => _$this._scope;
  set scope(String? scope) => _$this._scope = scope;

  String? _scopeId;
  String? get scopeId => _$this._scopeId;
  set scopeId(String? scopeId) => _$this._scopeId = scopeId;

  PermissionDtoBuilder() {
    PermissionDto._defaults(this);
  }

  PermissionDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _key = $v.key;
      _scope = $v.scope;
      _scopeId = $v.scopeId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PermissionDto other) {
    _$v = other as _$PermissionDto;
  }

  @override
  void update(void Function(PermissionDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PermissionDto build() => _build();

  _$PermissionDto _build() {
    final _$result = _$v ??
        _$PermissionDto._(
          key: key,
          scope: scope,
          scopeId: scopeId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
