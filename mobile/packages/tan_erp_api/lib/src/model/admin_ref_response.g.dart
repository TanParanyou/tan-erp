// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminRefResponse extends AdminRefResponse {
  @override
  final String? id;
  @override
  final String? name;

  factory _$AdminRefResponse(
          [void Function(AdminRefResponseBuilder)? updates]) =>
      (AdminRefResponseBuilder()..update(updates))._build();

  _$AdminRefResponse._({this.id, this.name}) : super._();
  @override
  AdminRefResponse rebuild(void Function(AdminRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminRefResponseBuilder toBuilder() =>
      AdminRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminRefResponse && id == other.id && name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminRefResponse')
          ..add('id', id)
          ..add('name', name))
        .toString();
  }
}

class AdminRefResponseBuilder
    implements Builder<AdminRefResponse, AdminRefResponseBuilder> {
  _$AdminRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  AdminRefResponseBuilder() {
    AdminRefResponse._defaults(this);
  }

  AdminRefResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminRefResponse other) {
    _$v = other as _$AdminRefResponse;
  }

  @override
  void update(void Function(AdminRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminRefResponse build() => _build();

  _$AdminRefResponse _build() {
    final _$result = _$v ??
        _$AdminRefResponse._(
          id: id,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
