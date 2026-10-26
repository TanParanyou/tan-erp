// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'role_change_request_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RoleChangeRequestResponse extends RoleChangeRequestResponse {
  @override
  final String? id;
  @override
  final String? rowVersion;
  @override
  final String? status;
  @override
  final AdminRefResponse? role;
  @override
  final AdminRefResponse? requestedBy;
  @override
  final DateTime? requestedAtUtc;
  @override
  final String? proposedName;
  @override
  final String? proposedDescription;
  @override
  final BuiltList<String>? addedPermissionKeys;
  @override
  final BuiltList<String>? removedPermissionKeys;

  factory _$RoleChangeRequestResponse(
          [void Function(RoleChangeRequestResponseBuilder)? updates]) =>
      (RoleChangeRequestResponseBuilder()..update(updates))._build();

  _$RoleChangeRequestResponse._(
      {this.id,
      this.rowVersion,
      this.status,
      this.role,
      this.requestedBy,
      this.requestedAtUtc,
      this.proposedName,
      this.proposedDescription,
      this.addedPermissionKeys,
      this.removedPermissionKeys})
      : super._();
  @override
  RoleChangeRequestResponse rebuild(
          void Function(RoleChangeRequestResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RoleChangeRequestResponseBuilder toBuilder() =>
      RoleChangeRequestResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RoleChangeRequestResponse &&
        id == other.id &&
        rowVersion == other.rowVersion &&
        status == other.status &&
        role == other.role &&
        requestedBy == other.requestedBy &&
        requestedAtUtc == other.requestedAtUtc &&
        proposedName == other.proposedName &&
        proposedDescription == other.proposedDescription &&
        addedPermissionKeys == other.addedPermissionKeys &&
        removedPermissionKeys == other.removedPermissionKeys;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, role.hashCode);
    _$hash = $jc(_$hash, requestedBy.hashCode);
    _$hash = $jc(_$hash, requestedAtUtc.hashCode);
    _$hash = $jc(_$hash, proposedName.hashCode);
    _$hash = $jc(_$hash, proposedDescription.hashCode);
    _$hash = $jc(_$hash, addedPermissionKeys.hashCode);
    _$hash = $jc(_$hash, removedPermissionKeys.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RoleChangeRequestResponse')
          ..add('id', id)
          ..add('rowVersion', rowVersion)
          ..add('status', status)
          ..add('role', role)
          ..add('requestedBy', requestedBy)
          ..add('requestedAtUtc', requestedAtUtc)
          ..add('proposedName', proposedName)
          ..add('proposedDescription', proposedDescription)
          ..add('addedPermissionKeys', addedPermissionKeys)
          ..add('removedPermissionKeys', removedPermissionKeys))
        .toString();
  }
}

class RoleChangeRequestResponseBuilder
    implements
        Builder<RoleChangeRequestResponse, RoleChangeRequestResponseBuilder> {
  _$RoleChangeRequestResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  AdminRefResponseBuilder? _role;
  AdminRefResponseBuilder get role =>
      _$this._role ??= AdminRefResponseBuilder();
  set role(AdminRefResponseBuilder? role) => _$this._role = role;

  AdminRefResponseBuilder? _requestedBy;
  AdminRefResponseBuilder get requestedBy =>
      _$this._requestedBy ??= AdminRefResponseBuilder();
  set requestedBy(AdminRefResponseBuilder? requestedBy) =>
      _$this._requestedBy = requestedBy;

  DateTime? _requestedAtUtc;
  DateTime? get requestedAtUtc => _$this._requestedAtUtc;
  set requestedAtUtc(DateTime? requestedAtUtc) =>
      _$this._requestedAtUtc = requestedAtUtc;

  String? _proposedName;
  String? get proposedName => _$this._proposedName;
  set proposedName(String? proposedName) => _$this._proposedName = proposedName;

  String? _proposedDescription;
  String? get proposedDescription => _$this._proposedDescription;
  set proposedDescription(String? proposedDescription) =>
      _$this._proposedDescription = proposedDescription;

  ListBuilder<String>? _addedPermissionKeys;
  ListBuilder<String> get addedPermissionKeys =>
      _$this._addedPermissionKeys ??= ListBuilder<String>();
  set addedPermissionKeys(ListBuilder<String>? addedPermissionKeys) =>
      _$this._addedPermissionKeys = addedPermissionKeys;

  ListBuilder<String>? _removedPermissionKeys;
  ListBuilder<String> get removedPermissionKeys =>
      _$this._removedPermissionKeys ??= ListBuilder<String>();
  set removedPermissionKeys(ListBuilder<String>? removedPermissionKeys) =>
      _$this._removedPermissionKeys = removedPermissionKeys;

  RoleChangeRequestResponseBuilder() {
    RoleChangeRequestResponse._defaults(this);
  }

  RoleChangeRequestResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _rowVersion = $v.rowVersion;
      _status = $v.status;
      _role = $v.role?.toBuilder();
      _requestedBy = $v.requestedBy?.toBuilder();
      _requestedAtUtc = $v.requestedAtUtc;
      _proposedName = $v.proposedName;
      _proposedDescription = $v.proposedDescription;
      _addedPermissionKeys = $v.addedPermissionKeys?.toBuilder();
      _removedPermissionKeys = $v.removedPermissionKeys?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RoleChangeRequestResponse other) {
    _$v = other as _$RoleChangeRequestResponse;
  }

  @override
  void update(void Function(RoleChangeRequestResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RoleChangeRequestResponse build() => _build();

  _$RoleChangeRequestResponse _build() {
    _$RoleChangeRequestResponse _$result;
    try {
      _$result = _$v ??
          _$RoleChangeRequestResponse._(
            id: id,
            rowVersion: rowVersion,
            status: status,
            role: _role?.build(),
            requestedBy: _requestedBy?.build(),
            requestedAtUtc: requestedAtUtc,
            proposedName: proposedName,
            proposedDescription: proposedDescription,
            addedPermissionKeys: _addedPermissionKeys?.build(),
            removedPermissionKeys: _removedPermissionKeys?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'role';
        _role?.build();
        _$failedField = 'requestedBy';
        _requestedBy?.build();

        _$failedField = 'addedPermissionKeys';
        _addedPermissionKeys?.build();
        _$failedField = 'removedPermissionKeys';
        _removedPermissionKeys?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'RoleChangeRequestResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
