// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_admin_user_profile_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateAdminUserProfileRequest extends UpdateAdminUserProfileRequest {
  @override
  final String? displayName;
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

  factory _$UpdateAdminUserProfileRequest(
          [void Function(UpdateAdminUserProfileRequestBuilder)? updates]) =>
      (UpdateAdminUserProfileRequestBuilder()..update(updates))._build();

  _$UpdateAdminUserProfileRequest._(
      {this.displayName,
      this.phoneNumber,
      this.employeeCode,
      this.departmentId,
      this.jobTitleId,
      this.avatarFileId,
      this.notes})
      : super._();
  @override
  UpdateAdminUserProfileRequest rebuild(
          void Function(UpdateAdminUserProfileRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateAdminUserProfileRequestBuilder toBuilder() =>
      UpdateAdminUserProfileRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateAdminUserProfileRequest &&
        displayName == other.displayName &&
        phoneNumber == other.phoneNumber &&
        employeeCode == other.employeeCode &&
        departmentId == other.departmentId &&
        jobTitleId == other.jobTitleId &&
        avatarFileId == other.avatarFileId &&
        notes == other.notes;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, phoneNumber.hashCode);
    _$hash = $jc(_$hash, employeeCode.hashCode);
    _$hash = $jc(_$hash, departmentId.hashCode);
    _$hash = $jc(_$hash, jobTitleId.hashCode);
    _$hash = $jc(_$hash, avatarFileId.hashCode);
    _$hash = $jc(_$hash, notes.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateAdminUserProfileRequest')
          ..add('displayName', displayName)
          ..add('phoneNumber', phoneNumber)
          ..add('employeeCode', employeeCode)
          ..add('departmentId', departmentId)
          ..add('jobTitleId', jobTitleId)
          ..add('avatarFileId', avatarFileId)
          ..add('notes', notes))
        .toString();
  }
}

class UpdateAdminUserProfileRequestBuilder
    implements
        Builder<UpdateAdminUserProfileRequest,
            UpdateAdminUserProfileRequestBuilder> {
  _$UpdateAdminUserProfileRequest? _$v;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

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

  UpdateAdminUserProfileRequestBuilder() {
    UpdateAdminUserProfileRequest._defaults(this);
  }

  UpdateAdminUserProfileRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _displayName = $v.displayName;
      _phoneNumber = $v.phoneNumber;
      _employeeCode = $v.employeeCode;
      _departmentId = $v.departmentId;
      _jobTitleId = $v.jobTitleId;
      _avatarFileId = $v.avatarFileId;
      _notes = $v.notes;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateAdminUserProfileRequest other) {
    _$v = other as _$UpdateAdminUserProfileRequest;
  }

  @override
  void update(void Function(UpdateAdminUserProfileRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateAdminUserProfileRequest build() => _build();

  _$UpdateAdminUserProfileRequest _build() {
    final _$result = _$v ??
        _$UpdateAdminUserProfileRequest._(
          displayName: displayName,
          phoneNumber: phoneNumber,
          employeeCode: employeeCode,
          departmentId: departmentId,
          jobTitleId: jobTitleId,
          avatarFileId: avatarFileId,
          notes: notes,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
