// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'job_title_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$JobTitleResponse extends JobTitleResponse {
  @override
  final String? id;
  @override
  final String? departmentId;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? nameEn;
  @override
  final bool? isActive;

  factory _$JobTitleResponse(
          [void Function(JobTitleResponseBuilder)? updates]) =>
      (JobTitleResponseBuilder()..update(updates))._build();

  _$JobTitleResponse._(
      {this.id,
      this.departmentId,
      this.code,
      this.name,
      this.nameEn,
      this.isActive})
      : super._();
  @override
  JobTitleResponse rebuild(void Function(JobTitleResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  JobTitleResponseBuilder toBuilder() =>
      JobTitleResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is JobTitleResponse &&
        id == other.id &&
        departmentId == other.departmentId &&
        code == other.code &&
        name == other.name &&
        nameEn == other.nameEn &&
        isActive == other.isActive;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, departmentId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'JobTitleResponse')
          ..add('id', id)
          ..add('departmentId', departmentId)
          ..add('code', code)
          ..add('name', name)
          ..add('nameEn', nameEn)
          ..add('isActive', isActive))
        .toString();
  }
}

class JobTitleResponseBuilder
    implements Builder<JobTitleResponse, JobTitleResponseBuilder> {
  _$JobTitleResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _departmentId;
  String? get departmentId => _$this._departmentId;
  set departmentId(String? departmentId) => _$this._departmentId = departmentId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _nameEn;
  String? get nameEn => _$this._nameEn;
  set nameEn(String? nameEn) => _$this._nameEn = nameEn;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  JobTitleResponseBuilder() {
    JobTitleResponse._defaults(this);
  }

  JobTitleResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _departmentId = $v.departmentId;
      _code = $v.code;
      _name = $v.name;
      _nameEn = $v.nameEn;
      _isActive = $v.isActive;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(JobTitleResponse other) {
    _$v = other as _$JobTitleResponse;
  }

  @override
  void update(void Function(JobTitleResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  JobTitleResponse build() => _build();

  _$JobTitleResponse _build() {
    final _$result = _$v ??
        _$JobTitleResponse._(
          id: id,
          departmentId: departmentId,
          code: code,
          name: name,
          nameEn: nameEn,
          isActive: isActive,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
