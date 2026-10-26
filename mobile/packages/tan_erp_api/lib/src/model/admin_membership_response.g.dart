// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_membership_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminMembershipResponse extends AdminMembershipResponse {
  @override
  final String? id;
  @override
  final bool? isActive;
  @override
  final String? rowVersion;
  @override
  final AdminRefResponse? branch;
  @override
  final DateTime? startsAtUtc;
  @override
  final DateTime? expiresAtUtc;
  @override
  final BuiltList<AdminRefResponse>? roles;
  @override
  final BuiltList<AdminPendingRoleRequestResponse>? pendingRoleRequests;

  factory _$AdminMembershipResponse(
          [void Function(AdminMembershipResponseBuilder)? updates]) =>
      (AdminMembershipResponseBuilder()..update(updates))._build();

  _$AdminMembershipResponse._(
      {this.id,
      this.isActive,
      this.rowVersion,
      this.branch,
      this.startsAtUtc,
      this.expiresAtUtc,
      this.roles,
      this.pendingRoleRequests})
      : super._();
  @override
  AdminMembershipResponse rebuild(
          void Function(AdminMembershipResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminMembershipResponseBuilder toBuilder() =>
      AdminMembershipResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminMembershipResponse &&
        id == other.id &&
        isActive == other.isActive &&
        rowVersion == other.rowVersion &&
        branch == other.branch &&
        startsAtUtc == other.startsAtUtc &&
        expiresAtUtc == other.expiresAtUtc &&
        roles == other.roles &&
        pendingRoleRequests == other.pendingRoleRequests;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, branch.hashCode);
    _$hash = $jc(_$hash, startsAtUtc.hashCode);
    _$hash = $jc(_$hash, expiresAtUtc.hashCode);
    _$hash = $jc(_$hash, roles.hashCode);
    _$hash = $jc(_$hash, pendingRoleRequests.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminMembershipResponse')
          ..add('id', id)
          ..add('isActive', isActive)
          ..add('rowVersion', rowVersion)
          ..add('branch', branch)
          ..add('startsAtUtc', startsAtUtc)
          ..add('expiresAtUtc', expiresAtUtc)
          ..add('roles', roles)
          ..add('pendingRoleRequests', pendingRoleRequests))
        .toString();
  }
}

class AdminMembershipResponseBuilder
    implements
        Builder<AdminMembershipResponse, AdminMembershipResponseBuilder> {
  _$AdminMembershipResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  AdminRefResponseBuilder? _branch;
  AdminRefResponseBuilder get branch =>
      _$this._branch ??= AdminRefResponseBuilder();
  set branch(AdminRefResponseBuilder? branch) => _$this._branch = branch;

  DateTime? _startsAtUtc;
  DateTime? get startsAtUtc => _$this._startsAtUtc;
  set startsAtUtc(DateTime? startsAtUtc) => _$this._startsAtUtc = startsAtUtc;

  DateTime? _expiresAtUtc;
  DateTime? get expiresAtUtc => _$this._expiresAtUtc;
  set expiresAtUtc(DateTime? expiresAtUtc) =>
      _$this._expiresAtUtc = expiresAtUtc;

  ListBuilder<AdminRefResponse>? _roles;
  ListBuilder<AdminRefResponse> get roles =>
      _$this._roles ??= ListBuilder<AdminRefResponse>();
  set roles(ListBuilder<AdminRefResponse>? roles) => _$this._roles = roles;

  ListBuilder<AdminPendingRoleRequestResponse>? _pendingRoleRequests;
  ListBuilder<AdminPendingRoleRequestResponse> get pendingRoleRequests =>
      _$this._pendingRoleRequests ??=
          ListBuilder<AdminPendingRoleRequestResponse>();
  set pendingRoleRequests(
          ListBuilder<AdminPendingRoleRequestResponse>? pendingRoleRequests) =>
      _$this._pendingRoleRequests = pendingRoleRequests;

  AdminMembershipResponseBuilder() {
    AdminMembershipResponse._defaults(this);
  }

  AdminMembershipResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _isActive = $v.isActive;
      _rowVersion = $v.rowVersion;
      _branch = $v.branch?.toBuilder();
      _startsAtUtc = $v.startsAtUtc;
      _expiresAtUtc = $v.expiresAtUtc;
      _roles = $v.roles?.toBuilder();
      _pendingRoleRequests = $v.pendingRoleRequests?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminMembershipResponse other) {
    _$v = other as _$AdminMembershipResponse;
  }

  @override
  void update(void Function(AdminMembershipResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminMembershipResponse build() => _build();

  _$AdminMembershipResponse _build() {
    _$AdminMembershipResponse _$result;
    try {
      _$result = _$v ??
          _$AdminMembershipResponse._(
            id: id,
            isActive: isActive,
            rowVersion: rowVersion,
            branch: _branch?.build(),
            startsAtUtc: startsAtUtc,
            expiresAtUtc: expiresAtUtc,
            roles: _roles?.build(),
            pendingRoleRequests: _pendingRoleRequests?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'branch';
        _branch?.build();

        _$failedField = 'roles';
        _roles?.build();
        _$failedField = 'pendingRoleRequests';
        _pendingRoleRequests?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AdminMembershipResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
