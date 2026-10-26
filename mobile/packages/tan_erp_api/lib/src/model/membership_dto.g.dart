// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'membership_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MembershipDto extends MembershipDto {
  @override
  final String? id;
  @override
  final OrganizationDto? organization;
  @override
  final BranchDto? branch;
  @override
  final BuiltList<PermissionDto>? permissions;

  factory _$MembershipDto([void Function(MembershipDtoBuilder)? updates]) =>
      (MembershipDtoBuilder()..update(updates))._build();

  _$MembershipDto._({this.id, this.organization, this.branch, this.permissions})
      : super._();
  @override
  MembershipDto rebuild(void Function(MembershipDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MembershipDtoBuilder toBuilder() => MembershipDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MembershipDto &&
        id == other.id &&
        organization == other.organization &&
        branch == other.branch &&
        permissions == other.permissions;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organization.hashCode);
    _$hash = $jc(_$hash, branch.hashCode);
    _$hash = $jc(_$hash, permissions.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MembershipDto')
          ..add('id', id)
          ..add('organization', organization)
          ..add('branch', branch)
          ..add('permissions', permissions))
        .toString();
  }
}

class MembershipDtoBuilder
    implements Builder<MembershipDto, MembershipDtoBuilder> {
  _$MembershipDto? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  OrganizationDtoBuilder? _organization;
  OrganizationDtoBuilder get organization =>
      _$this._organization ??= OrganizationDtoBuilder();
  set organization(OrganizationDtoBuilder? organization) =>
      _$this._organization = organization;

  BranchDtoBuilder? _branch;
  BranchDtoBuilder get branch => _$this._branch ??= BranchDtoBuilder();
  set branch(BranchDtoBuilder? branch) => _$this._branch = branch;

  ListBuilder<PermissionDto>? _permissions;
  ListBuilder<PermissionDto> get permissions =>
      _$this._permissions ??= ListBuilder<PermissionDto>();
  set permissions(ListBuilder<PermissionDto>? permissions) =>
      _$this._permissions = permissions;

  MembershipDtoBuilder() {
    MembershipDto._defaults(this);
  }

  MembershipDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organization = $v.organization?.toBuilder();
      _branch = $v.branch?.toBuilder();
      _permissions = $v.permissions?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MembershipDto other) {
    _$v = other as _$MembershipDto;
  }

  @override
  void update(void Function(MembershipDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MembershipDto build() => _build();

  _$MembershipDto _build() {
    _$MembershipDto _$result;
    try {
      _$result = _$v ??
          _$MembershipDto._(
            id: id,
            organization: _organization?.build(),
            branch: _branch?.build(),
            permissions: _permissions?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'organization';
        _organization?.build();
        _$failedField = 'branch';
        _branch?.build();
        _$failedField = 'permissions';
        _permissions?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'MembershipDto', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
