// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_pending_role_request_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminPendingRoleRequestResponse
    extends AdminPendingRoleRequestResponse {
  @override
  final String? id;
  @override
  final String? rowVersion;
  @override
  final AdminRefResponse? role;
  @override
  final AdminRefResponse? requestedBy;
  @override
  final DateTime? requestedAtUtc;

  factory _$AdminPendingRoleRequestResponse(
          [void Function(AdminPendingRoleRequestResponseBuilder)? updates]) =>
      (AdminPendingRoleRequestResponseBuilder()..update(updates))._build();

  _$AdminPendingRoleRequestResponse._(
      {this.id,
      this.rowVersion,
      this.role,
      this.requestedBy,
      this.requestedAtUtc})
      : super._();
  @override
  AdminPendingRoleRequestResponse rebuild(
          void Function(AdminPendingRoleRequestResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminPendingRoleRequestResponseBuilder toBuilder() =>
      AdminPendingRoleRequestResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminPendingRoleRequestResponse &&
        id == other.id &&
        rowVersion == other.rowVersion &&
        role == other.role &&
        requestedBy == other.requestedBy &&
        requestedAtUtc == other.requestedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, role.hashCode);
    _$hash = $jc(_$hash, requestedBy.hashCode);
    _$hash = $jc(_$hash, requestedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminPendingRoleRequestResponse')
          ..add('id', id)
          ..add('rowVersion', rowVersion)
          ..add('role', role)
          ..add('requestedBy', requestedBy)
          ..add('requestedAtUtc', requestedAtUtc))
        .toString();
  }
}

class AdminPendingRoleRequestResponseBuilder
    implements
        Builder<AdminPendingRoleRequestResponse,
            AdminPendingRoleRequestResponseBuilder> {
  _$AdminPendingRoleRequestResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

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

  AdminPendingRoleRequestResponseBuilder() {
    AdminPendingRoleRequestResponse._defaults(this);
  }

  AdminPendingRoleRequestResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _rowVersion = $v.rowVersion;
      _role = $v.role?.toBuilder();
      _requestedBy = $v.requestedBy?.toBuilder();
      _requestedAtUtc = $v.requestedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminPendingRoleRequestResponse other) {
    _$v = other as _$AdminPendingRoleRequestResponse;
  }

  @override
  void update(void Function(AdminPendingRoleRequestResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminPendingRoleRequestResponse build() => _build();

  _$AdminPendingRoleRequestResponse _build() {
    _$AdminPendingRoleRequestResponse _$result;
    try {
      _$result = _$v ??
          _$AdminPendingRoleRequestResponse._(
            id: id,
            rowVersion: rowVersion,
            role: _role?.build(),
            requestedBy: _requestedBy?.build(),
            requestedAtUtc: requestedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'role';
        _role?.build();
        _$failedField = 'requestedBy';
        _requestedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AdminPendingRoleRequestResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
