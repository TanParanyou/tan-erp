// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_job_title_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminJobTitleResponse extends AdminJobTitleResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$AdminJobTitleResponse(
          [void Function(AdminJobTitleResponseBuilder)? updates]) =>
      (AdminJobTitleResponseBuilder()..update(updates))._build();

  _$AdminJobTitleResponse._({this.id, this.code, this.name}) : super._();
  @override
  AdminJobTitleResponse rebuild(
          void Function(AdminJobTitleResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminJobTitleResponseBuilder toBuilder() =>
      AdminJobTitleResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminJobTitleResponse &&
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
    return (newBuiltValueToStringHelper(r'AdminJobTitleResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class AdminJobTitleResponseBuilder
    implements Builder<AdminJobTitleResponse, AdminJobTitleResponseBuilder> {
  _$AdminJobTitleResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  AdminJobTitleResponseBuilder() {
    AdminJobTitleResponse._defaults(this);
  }

  AdminJobTitleResponseBuilder get _$this {
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
  void replace(AdminJobTitleResponse other) {
    _$v = other as _$AdminJobTitleResponse;
  }

  @override
  void update(void Function(AdminJobTitleResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminJobTitleResponse build() => _build();

  _$AdminJobTitleResponse _build() {
    final _$result = _$v ??
        _$AdminJobTitleResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
