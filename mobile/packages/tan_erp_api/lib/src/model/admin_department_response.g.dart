// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_department_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminDepartmentResponse extends AdminDepartmentResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$AdminDepartmentResponse(
          [void Function(AdminDepartmentResponseBuilder)? updates]) =>
      (AdminDepartmentResponseBuilder()..update(updates))._build();

  _$AdminDepartmentResponse._({this.id, this.code, this.name}) : super._();
  @override
  AdminDepartmentResponse rebuild(
          void Function(AdminDepartmentResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminDepartmentResponseBuilder toBuilder() =>
      AdminDepartmentResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminDepartmentResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminDepartmentResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class AdminDepartmentResponseBuilder
    implements
        Builder<AdminDepartmentResponse, AdminDepartmentResponseBuilder> {
  _$AdminDepartmentResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  AdminDepartmentResponseBuilder() {
    AdminDepartmentResponse._defaults(this);
  }

  AdminDepartmentResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminDepartmentResponse other) {
    _$v = other as _$AdminDepartmentResponse;
  }

  @override
  void update(void Function(AdminDepartmentResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminDepartmentResponse build() => _build();

  _$AdminDepartmentResponse _build() {
    final _$result = _$v ??
        _$AdminDepartmentResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
