// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_job_title_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateJobTitleRequest extends CreateJobTitleRequest {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? departmentId;
  @override
  final String? nameEn;

  factory _$CreateJobTitleRequest(
          [void Function(CreateJobTitleRequestBuilder)? updates]) =>
      (CreateJobTitleRequestBuilder()..update(updates))._build();

  _$CreateJobTitleRequest._(
      {this.code, this.name, this.departmentId, this.nameEn})
      : super._();
  @override
  CreateJobTitleRequest rebuild(
          void Function(CreateJobTitleRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateJobTitleRequestBuilder toBuilder() =>
      CreateJobTitleRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateJobTitleRequest &&
        code == other.code &&
        name == other.name &&
        departmentId == other.departmentId &&
        nameEn == other.nameEn;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, departmentId.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateJobTitleRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('departmentId', departmentId)
          ..add('nameEn', nameEn))
        .toString();
  }
}

class CreateJobTitleRequestBuilder
    implements Builder<CreateJobTitleRequest, CreateJobTitleRequestBuilder> {
  _$CreateJobTitleRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _departmentId;
  String? get departmentId => _$this._departmentId;
  set departmentId(String? departmentId) => _$this._departmentId = departmentId;

  String? _nameEn;
  String? get nameEn => _$this._nameEn;
  set nameEn(String? nameEn) => _$this._nameEn = nameEn;

  CreateJobTitleRequestBuilder() {
    CreateJobTitleRequest._defaults(this);
  }

  CreateJobTitleRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _departmentId = $v.departmentId;
      _nameEn = $v.nameEn;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateJobTitleRequest other) {
    _$v = other as _$CreateJobTitleRequest;
  }

  @override
  void update(void Function(CreateJobTitleRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateJobTitleRequest build() => _build();

  _$CreateJobTitleRequest _build() {
    final _$result = _$v ??
        _$CreateJobTitleRequest._(
          code: code,
          name: name,
          departmentId: departmentId,
          nameEn: nameEn,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
