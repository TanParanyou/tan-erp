// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'department_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DepartmentResponse extends DepartmentResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? nameEn;
  @override
  final bool? isActive;

  factory _$DepartmentResponse(
          [void Function(DepartmentResponseBuilder)? updates]) =>
      (DepartmentResponseBuilder()..update(updates))._build();

  _$DepartmentResponse._(
      {this.id, this.code, this.name, this.nameEn, this.isActive})
      : super._();
  @override
  DepartmentResponse rebuild(
          void Function(DepartmentResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DepartmentResponseBuilder toBuilder() =>
      DepartmentResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DepartmentResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        nameEn == other.nameEn &&
        isActive == other.isActive;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DepartmentResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('nameEn', nameEn)
          ..add('isActive', isActive))
        .toString();
  }
}

class DepartmentResponseBuilder
    implements Builder<DepartmentResponse, DepartmentResponseBuilder> {
  _$DepartmentResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  DepartmentResponseBuilder() {
    DepartmentResponse._defaults(this);
  }

  DepartmentResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name;
      _nameEn = $v.nameEn;
      _isActive = $v.isActive;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DepartmentResponse other) {
    _$v = other as _$DepartmentResponse;
  }

  @override
  void update(void Function(DepartmentResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DepartmentResponse build() => _build();

  _$DepartmentResponse _build() {
    final _$result = _$v ??
        _$DepartmentResponse._(
          id: id,
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
