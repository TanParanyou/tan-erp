// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_department_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateDepartmentRequest extends CreateDepartmentRequest {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? nameEn;

  factory _$CreateDepartmentRequest(
          [void Function(CreateDepartmentRequestBuilder)? updates]) =>
      (CreateDepartmentRequestBuilder()..update(updates))._build();

  _$CreateDepartmentRequest._({this.code, this.name, this.nameEn}) : super._();
  @override
  CreateDepartmentRequest rebuild(
          void Function(CreateDepartmentRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateDepartmentRequestBuilder toBuilder() =>
      CreateDepartmentRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateDepartmentRequest &&
        code == other.code &&
        name == other.name &&
        nameEn == other.nameEn;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateDepartmentRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('nameEn', nameEn))
        .toString();
  }
}

class CreateDepartmentRequestBuilder
    implements
        Builder<CreateDepartmentRequest, CreateDepartmentRequestBuilder> {
  _$CreateDepartmentRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _nameEn;
  String? get nameEn => _$this._nameEn;
  set nameEn(String? nameEn) => _$this._nameEn = nameEn;

  CreateDepartmentRequestBuilder() {
    CreateDepartmentRequest._defaults(this);
  }

  CreateDepartmentRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _nameEn = $v.nameEn;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateDepartmentRequest other) {
    _$v = other as _$CreateDepartmentRequest;
  }

  @override
  void update(void Function(CreateDepartmentRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateDepartmentRequest build() => _build();

  _$CreateDepartmentRequest _build() {
    final _$result = _$v ??
        _$CreateDepartmentRequest._(
          code: code,
          name: name,
          nameEn: nameEn,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
