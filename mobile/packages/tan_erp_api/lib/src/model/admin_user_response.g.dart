// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_user_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminUserResponse extends AdminUserResponse {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? email;
  @override
  final String? status;
  @override
  final String? rowVersion;
  @override
  final BuiltList<AdminMembershipResponse>? memberships;
  @override
  final String? phoneNumber;
  @override
  final String? employeeCode;
  @override
  final AdminDepartmentResponse? department;
  @override
  final AdminJobTitleResponse? jobTitle;
  @override
  final AdminAvatarResponse? avatar;
  @override
  final String? notes;
  @override
  final DateTime? createdAtUtc;

  factory _$AdminUserResponse(
          [void Function(AdminUserResponseBuilder)? updates]) =>
      (AdminUserResponseBuilder()..update(updates))._build();

  _$AdminUserResponse._(
      {this.id,
      this.displayName,
      this.email,
      this.status,
      this.rowVersion,
      this.memberships,
      this.phoneNumber,
      this.employeeCode,
      this.department,
      this.jobTitle,
      this.avatar,
      this.notes,
      this.createdAtUtc})
      : super._();
  @override
  AdminUserResponse rebuild(void Function(AdminUserResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminUserResponseBuilder toBuilder() =>
      AdminUserResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminUserResponse &&
        id == other.id &&
        displayName == other.displayName &&
        email == other.email &&
        status == other.status &&
        rowVersion == other.rowVersion &&
        memberships == other.memberships &&
        phoneNumber == other.phoneNumber &&
        employeeCode == other.employeeCode &&
        department == other.department &&
        jobTitle == other.jobTitle &&
        avatar == other.avatar &&
        notes == other.notes &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, memberships.hashCode);
    _$hash = $jc(_$hash, phoneNumber.hashCode);
    _$hash = $jc(_$hash, employeeCode.hashCode);
    _$hash = $jc(_$hash, department.hashCode);
    _$hash = $jc(_$hash, jobTitle.hashCode);
    _$hash = $jc(_$hash, avatar.hashCode);
    _$hash = $jc(_$hash, notes.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminUserResponse')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('email', email)
          ..add('status', status)
          ..add('rowVersion', rowVersion)
          ..add('memberships', memberships)
          ..add('phoneNumber', phoneNumber)
          ..add('employeeCode', employeeCode)
          ..add('department', department)
          ..add('jobTitle', jobTitle)
          ..add('avatar', avatar)
          ..add('notes', notes)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class AdminUserResponseBuilder
    implements Builder<AdminUserResponse, AdminUserResponseBuilder> {
  _$AdminUserResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ListBuilder<AdminMembershipResponse>? _memberships;
  ListBuilder<AdminMembershipResponse> get memberships =>
      _$this._memberships ??= ListBuilder<AdminMembershipResponse>();
  set memberships(ListBuilder<AdminMembershipResponse>? memberships) =>
      _$this._memberships = memberships;

  String? _phoneNumber;
  String? get phoneNumber => _$this._phoneNumber;
  set phoneNumber(String? phoneNumber) => _$this._phoneNumber = phoneNumber;

  String? _employeeCode;
  String? get employeeCode => _$this._employeeCode;
  set employeeCode(String? employeeCode) => _$this._employeeCode = employeeCode;

  AdminDepartmentResponseBuilder? _department;
  AdminDepartmentResponseBuilder get department =>
      _$this._department ??= AdminDepartmentResponseBuilder();
  set department(AdminDepartmentResponseBuilder? department) =>
      _$this._department = department;

  AdminJobTitleResponseBuilder? _jobTitle;
  AdminJobTitleResponseBuilder get jobTitle =>
      _$this._jobTitle ??= AdminJobTitleResponseBuilder();
  set jobTitle(AdminJobTitleResponseBuilder? jobTitle) =>
      _$this._jobTitle = jobTitle;

  AdminAvatarResponseBuilder? _avatar;
  AdminAvatarResponseBuilder get avatar =>
      _$this._avatar ??= AdminAvatarResponseBuilder();
  set avatar(AdminAvatarResponseBuilder? avatar) => _$this._avatar = avatar;

  String? _notes;
  String? get notes => _$this._notes;
  set notes(String? notes) => _$this._notes = notes;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  AdminUserResponseBuilder() {
    AdminUserResponse._defaults(this);
  }

  AdminUserResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _email = $v.email;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _memberships = $v.memberships?.toBuilder();
      _phoneNumber = $v.phoneNumber;
      _employeeCode = $v.employeeCode;
      _department = $v.department?.toBuilder();
      _jobTitle = $v.jobTitle?.toBuilder();
      _avatar = $v.avatar?.toBuilder();
      _notes = $v.notes;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminUserResponse other) {
    _$v = other as _$AdminUserResponse;
  }

  @override
  void update(void Function(AdminUserResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminUserResponse build() => _build();

  _$AdminUserResponse _build() {
    _$AdminUserResponse _$result;
    try {
      _$result = _$v ??
          _$AdminUserResponse._(
            id: id,
            displayName: displayName,
            email: email,
            status: status,
            rowVersion: rowVersion,
            memberships: _memberships?.build(),
            phoneNumber: phoneNumber,
            employeeCode: employeeCode,
            department: _department?.build(),
            jobTitle: _jobTitle?.build(),
            avatar: _avatar?.build(),
            notes: notes,
            createdAtUtc: createdAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'memberships';
        _memberships?.build();

        _$failedField = 'department';
        _department?.build();
        _$failedField = 'jobTitle';
        _jobTitle?.build();
        _$failedField = 'avatar';
        _avatar?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AdminUserResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
