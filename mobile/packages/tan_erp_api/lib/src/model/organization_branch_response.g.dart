// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'organization_branch_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OrganizationBranchResponse extends OrganizationBranchResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$OrganizationBranchResponse(
          [void Function(OrganizationBranchResponseBuilder)? updates]) =>
      (OrganizationBranchResponseBuilder()..update(updates))._build();

  _$OrganizationBranchResponse._({this.id, this.code, this.name}) : super._();
  @override
  OrganizationBranchResponse rebuild(
          void Function(OrganizationBranchResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OrganizationBranchResponseBuilder toBuilder() =>
      OrganizationBranchResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OrganizationBranchResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OrganizationBranchResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class OrganizationBranchResponseBuilder
    implements
        Builder<OrganizationBranchResponse, OrganizationBranchResponseBuilder> {
  _$OrganizationBranchResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  OrganizationBranchResponseBuilder() {
    OrganizationBranchResponse._defaults(this);
  }

  OrganizationBranchResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OrganizationBranchResponse other) {
    _$v = other as _$OrganizationBranchResponse;
  }

  @override
  void update(void Function(OrganizationBranchResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OrganizationBranchResponse build() => _build();

  _$OrganizationBranchResponse _build() {
    final _$result = _$v ??
        _$OrganizationBranchResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
