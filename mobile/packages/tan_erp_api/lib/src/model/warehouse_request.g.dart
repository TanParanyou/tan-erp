// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'warehouse_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WarehouseRequest extends WarehouseRequest {
  @override
  final String? name;
  @override
  final String? address;

  factory _$WarehouseRequest(
          [void Function(WarehouseRequestBuilder)? updates]) =>
      (WarehouseRequestBuilder()..update(updates))._build();

  _$WarehouseRequest._({this.name, this.address}) : super._();
  @override
  WarehouseRequest rebuild(void Function(WarehouseRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WarehouseRequestBuilder toBuilder() =>
      WarehouseRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WarehouseRequest &&
        name == other.name &&
        address == other.address;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, address.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WarehouseRequest')
          ..add('name', name)
          ..add('address', address))
        .toString();
  }
}

class WarehouseRequestBuilder
    implements Builder<WarehouseRequest, WarehouseRequestBuilder> {
  _$WarehouseRequest? _$v;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _address;
  String? get address => _$this._address;
  set address(String? address) => _$this._address = address;

  WarehouseRequestBuilder() {
    WarehouseRequest._defaults(this);
  }

  WarehouseRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _address = $v.address;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WarehouseRequest other) {
    _$v = other as _$WarehouseRequest;
  }

  @override
  void update(void Function(WarehouseRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WarehouseRequest build() => _build();

  _$WarehouseRequest _build() {
    final _$result = _$v ??
        _$WarehouseRequest._(
          name: name,
          address: address,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
