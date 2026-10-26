// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_admin_user_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateAdminUserRequest extends CreateAdminUserRequest {
  @override
  final String? displayName;
  @override
  final String? email;
  @override
  final String? branchId;
  @override
  final BuiltList<String>? roleIds;
  @override
  final String? phoneNumber;
  @override
  final String? employeeCode;
  @override
  final String? departmentId;
  @override
  final String? jobTitleId;
  @override
  final String? avatarFileId;
  @override
  final String? notes;
  @override
  final DateTime? startsAtUtc;
  @override
  final DateTime? expiresAtUtc;

  factory _$CreateAdminUserRequest(
          [void Function(CreateAdminUserRequestBuilder)? updates]) =>
      (CreateAdminUserRequestBuilder()..update(updates))._build();

  _$CreateAdminUserRequest._(
      {this.displayName,
      this.email,
      this.branchId,
      this.roleIds,
      this.phoneNumber,
      this.employeeCode,
      this.departmentId,
      this.jobTitleId,
      this.avatarFileId,
      this.notes,
      this.startsAtUtc,
      this.expiresAtUtc})
      : super._();
  @override
  CreateAdminUserRequest rebuild(
          void Function(CreateAdminUserRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateAdminUserRequestBuilder toBuilder() =>
      CreateAdminUserRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateAdminUserRequest &&
        displayName == other.displayName &&
        email == other.email &&
        branchId == other.branchId &&
        roleIds == other.roleIds &&
        phoneNumber == other.phoneNumber &&
        employeeCode == other.employeeCode &&
        departmentId == other.departmentId &&
        jobTitleId == other.jobTitleId &&
        avatarFileId == other.avatarFileId &&
        notes == other.notes &&
        startsAtUtc == other.startsAtUtc &&
        expiresAtUtc == other.expiresAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, roleIds.hashCode);
    _$hash = $jc(_$hash, phoneNumber.hashCode);
    _$hash = $jc(_$hash, employeeCode.hashCode);
    _$hash = $jc(_$hash, departmentId.hashCode);
    _$hash = $jc(_$hash, jobTitleId.hashCode);
    _$hash = $jc(_$hash, avatarFileId.hashCode);
    _$hash = $jc(_$hash, notes.hashCode);
    _$hash = $jc(_$hash, startsAtUtc.hashCode);
    _$hash = $jc(_$hash, expiresAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateAdminUserRequest')
          ..add('displayName', displayName)
          ..add('email', email)
          ..add('branchId', branchId)
          ..add('roleIds', roleIds)
          ..add('phoneNumber', phoneNumber)
          ..add('employeeCode', employeeCode)
          ..add('departmentId', departmentId)
          ..add('jobTitleId', jobTitleId)
          ..add('avatarFileId', avatarFileId)
          ..add('notes', notes)
          ..add('startsAtUtc', startsAtUtc)
          ..add('expiresAtUtc', expiresAtUtc))
        .toString();
  }
}

class CreateAdminUserRequestBuilder
    implements Builder<CreateAdminUserRequest, CreateAdminUserRequestBuilder> {
  _$CreateAdminUserRequest? _$v;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  ListBuilder<String>? _roleIds;
  ListBuilder<String> get roleIds => _$this._roleIds ??= ListBuilder<String>();
  set roleIds(ListBuilder<String>? roleIds) => _$this._roleIds = roleIds;

  String? _phoneNumber;
  String? get phoneNumber => _$this._phoneNumber;
  set phoneNumber(String? phoneNumber) => _$this._phoneNumber = phoneNumber;

  String? _employeeCode;
  String? get employeeCode => _$this._employeeCode;
  set employeeCode(String? employeeCode) => _$this._employeeCode = employeeCode;

  String? _departmentId;
  String? get departmentId => _$this._departmentId;
  set departmentId(String? departmentId) => _$this._departmentId = departmentId;

  String? _jobTitleId;
  String? get jobTitleId => _$this._jobTitleId;
  set jobTitleId(String? jobTitleId) => _$this._jobTitleId = jobTitleId;

  String? _avatarFileId;
  String? get avatarFileId => _$this._avatarFileId;
  set avatarFileId(String? avatarFileId) => _$this._avatarFileId = avatarFileId;

  String? _notes;
  String? get notes => _$this._notes;
  set notes(String? notes) => _$this._notes = notes;

  DateTime? _startsAtUtc;
  DateTime? get startsAtUtc => _$this._startsAtUtc;
  set startsAtUtc(DateTime? startsAtUtc) => _$this._startsAtUtc = startsAtUtc;

  DateTime? _expiresAtUtc;
  DateTime? get expiresAtUtc => _$this._expiresAtUtc;
  set expiresAtUtc(DateTime? expiresAtUtc) =>
      _$this._expiresAtUtc = expiresAtUtc;

  CreateAdminUserRequestBuilder() {
    CreateAdminUserRequest._defaults(this);
  }

  CreateAdminUserRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _displayName = $v.displayName;
      _email = $v.email;
      _branchId = $v.branchId;
      _roleIds = $v.roleIds?.toBuilder();
      _phoneNumber = $v.phoneNumber;
      _employeeCode = $v.employeeCode;
      _departmentId = $v.departmentId;
      _jobTitleId = $v.jobTitleId;
      _avatarFileId = $v.avatarFileId;
      _notes = $v.notes;
      _startsAtUtc = $v.startsAtUtc;
      _expiresAtUtc = $v.expiresAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateAdminUserRequest other) {
    _$v = other as _$CreateAdminUserRequest;
  }

  @override
  void update(void Function(CreateAdminUserRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateAdminUserRequest build() => _build();

  _$CreateAdminUserRequest _build() {
    _$CreateAdminUserRequest _$result;
    try {
      _$result = _$v ??
          _$CreateAdminUserRequest._(
            displayName: displayName,
            email: email,
            branchId: branchId,
            roleIds: _roleIds?.build(),
            phoneNumber: phoneNumber,
            employeeCode: employeeCode,
            departmentId: departmentId,
            jobTitleId: jobTitleId,
            avatarFileId: avatarFileId,
            notes: notes,
            startsAtUtc: startsAtUtc,
            expiresAtUtc: expiresAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'roleIds';
        _roleIds?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreateAdminUserRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
