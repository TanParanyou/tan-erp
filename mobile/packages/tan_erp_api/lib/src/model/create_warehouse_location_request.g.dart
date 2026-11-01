// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_warehouse_location_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateWarehouseLocationRequest extends CreateWarehouseLocationRequest {
  @override
  final String? code;
  @override
  final String? name;

  factory _$CreateWarehouseLocationRequest(
          [void Function(CreateWarehouseLocationRequestBuilder)? updates]) =>
      (CreateWarehouseLocationRequestBuilder()..update(updates))._build();

  _$CreateWarehouseLocationRequest._({this.code, this.name}) : super._();
  @override
  CreateWarehouseLocationRequest rebuild(
          void Function(CreateWarehouseLocationRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateWarehouseLocationRequestBuilder toBuilder() =>
      CreateWarehouseLocationRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateWarehouseLocationRequest &&
        code == other.code &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateWarehouseLocationRequest')
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class CreateWarehouseLocationRequestBuilder
    implements
        Builder<CreateWarehouseLocationRequest,
            CreateWarehouseLocationRequestBuilder> {
  _$CreateWarehouseLocationRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  CreateWarehouseLocationRequestBuilder() {
    CreateWarehouseLocationRequest._defaults(this);
  }

  CreateWarehouseLocationRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateWarehouseLocationRequest other) {
    _$v = other as _$CreateWarehouseLocationRequest;
  }

  @override
  void update(void Function(CreateWarehouseLocationRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateWarehouseLocationRequest build() => _build();

  _$CreateWarehouseLocationRequest _build() {
    final _$result = _$v ??
        _$CreateWarehouseLocationRequest._(
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
