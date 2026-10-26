// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_project_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceProjectRefResponse extends ServiceProjectRefResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$ServiceProjectRefResponse(
          [void Function(ServiceProjectRefResponseBuilder)? updates]) =>
      (ServiceProjectRefResponseBuilder()..update(updates))._build();

  _$ServiceProjectRefResponse._({this.id, this.code, this.name}) : super._();
  @override
  ServiceProjectRefResponse rebuild(
          void Function(ServiceProjectRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceProjectRefResponseBuilder toBuilder() =>
      ServiceProjectRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceProjectRefResponse &&
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
    return (newBuiltValueToStringHelper(r'ServiceProjectRefResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class ServiceProjectRefResponseBuilder
    implements
        Builder<ServiceProjectRefResponse, ServiceProjectRefResponseBuilder> {
  _$ServiceProjectRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  ServiceProjectRefResponseBuilder() {
    ServiceProjectRefResponse._defaults(this);
  }

  ServiceProjectRefResponseBuilder get _$this {
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
  void replace(ServiceProjectRefResponse other) {
    _$v = other as _$ServiceProjectRefResponse;
  }

  @override
  void update(void Function(ServiceProjectRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceProjectRefResponse build() => _build();

  _$ServiceProjectRefResponse _build() {
    final _$result = _$v ??
        _$ServiceProjectRefResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
