// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_role_request_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminRoleRequestResponse extends AdminRoleRequestResponse {
  @override
  final String? id;
  @override
  final String? rowVersion;
  @override
  final String? status;
  @override
  final AdminRefResponse? role;
  @override
  final AdminRefResponse? membership;
  @override
  final AdminRefResponse? requestedBy;
  @override
  final DateTime? requestedAtUtc;

  factory _$AdminRoleRequestResponse(
          [void Function(AdminRoleRequestResponseBuilder)? updates]) =>
      (AdminRoleRequestResponseBuilder()..update(updates))._build();

  _$AdminRoleRequestResponse._(
      {this.id,
      this.rowVersion,
      this.status,
      this.role,
      this.membership,
      this.requestedBy,
      this.requestedAtUtc})
      : super._();
  @override
  AdminRoleRequestResponse rebuild(
          void Function(AdminRoleRequestResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminRoleRequestResponseBuilder toBuilder() =>
      AdminRoleRequestResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminRoleRequestResponse &&
        id == other.id &&
        rowVersion == other.rowVersion &&
        status == other.status &&
        role == other.role &&
        membership == other.membership &&
        requestedBy == other.requestedBy &&
        requestedAtUtc == other.requestedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, role.hashCode);
    _$hash = $jc(_$hash, membership.hashCode);
    _$hash = $jc(_$hash, requestedBy.hashCode);
    _$hash = $jc(_$hash, requestedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminRoleRequestResponse')
          ..add('id', id)
          ..add('rowVersion', rowVersion)
          ..add('status', status)
          ..add('role', role)
          ..add('membership', membership)
          ..add('requestedBy', requestedBy)
          ..add('requestedAtUtc', requestedAtUtc))
        .toString();
  }
}

class AdminRoleRequestResponseBuilder
    implements
        Builder<AdminRoleRequestResponse, AdminRoleRequestResponseBuilder> {
  _$AdminRoleRequestResponse? _$v;

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

  AdminRefResponseBuilder? _membership;
  AdminRefResponseBuilder get membership =>
      _$this._membership ??= AdminRefResponseBuilder();
  set membership(AdminRefResponseBuilder? membership) =>
      _$this._membership = membership;

  AdminRefResponseBuilder? _requestedBy;
  AdminRefResponseBuilder get requestedBy =>
      _$this._requestedBy ??= AdminRefResponseBuilder();
  set requestedBy(AdminRefResponseBuilder? requestedBy) =>
      _$this._requestedBy = requestedBy;

  DateTime? _requestedAtUtc;
  DateTime? get requestedAtUtc => _$this._requestedAtUtc;
  set requestedAtUtc(DateTime? requestedAtUtc) =>
      _$this._requestedAtUtc = requestedAtUtc;

  AdminRoleRequestResponseBuilder() {
    AdminRoleRequestResponse._defaults(this);
  }

  AdminRoleRequestResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _rowVersion = $v.rowVersion;
      _status = $v.status;
      _role = $v.role?.toBuilder();
      _membership = $v.membership?.toBuilder();
      _requestedBy = $v.requestedBy?.toBuilder();
      _requestedAtUtc = $v.requestedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminRoleRequestResponse other) {
    _$v = other as _$AdminRoleRequestResponse;
  }

  @override
  void update(void Function(AdminRoleRequestResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminRoleRequestResponse build() => _build();

  _$AdminRoleRequestResponse _build() {
    _$AdminRoleRequestResponse _$result;
    try {
      _$result = _$v ??
          _$AdminRoleRequestResponse._(
            id: id,
            rowVersion: rowVersion,
            status: status,
            role: _role?.build(),
            membership: _membership?.build(),
            requestedBy: _requestedBy?.build(),
            requestedAtUtc: requestedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'role';
        _role?.build();
        _$failedField = 'membership';
        _membership?.build();
        _$failedField = 'requestedBy';
        _requestedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AdminRoleRequestResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
