// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_warehouse_location_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateWarehouseLocationRequest extends UpdateWarehouseLocationRequest {
  @override
  final String? name;
  @override
  final bool? active;

  factory _$UpdateWarehouseLocationRequest(
          [void Function(UpdateWarehouseLocationRequestBuilder)? updates]) =>
      (UpdateWarehouseLocationRequestBuilder()..update(updates))._build();

  _$UpdateWarehouseLocationRequest._({this.name, this.active}) : super._();
  @override
  UpdateWarehouseLocationRequest rebuild(
          void Function(UpdateWarehouseLocationRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateWarehouseLocationRequestBuilder toBuilder() =>
      UpdateWarehouseLocationRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateWarehouseLocationRequest &&
        name == other.name &&
        active == other.active;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, active.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateWarehouseLocationRequest')
          ..add('name', name)
          ..add('active', active))
        .toString();
  }
}

class UpdateWarehouseLocationRequestBuilder
    implements
        Builder<UpdateWarehouseLocationRequest,
            UpdateWarehouseLocationRequestBuilder> {
  _$UpdateWarehouseLocationRequest? _$v;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  bool? _active;
  bool? get active => _$this._active;
  set active(bool? active) => _$this._active = active;

  UpdateWarehouseLocationRequestBuilder() {
    UpdateWarehouseLocationRequest._defaults(this);
  }

  UpdateWarehouseLocationRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _active = $v.active;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateWarehouseLocationRequest other) {
    _$v = other as _$UpdateWarehouseLocationRequest;
  }

  @override
  void update(void Function(UpdateWarehouseLocationRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateWarehouseLocationRequest build() => _build();

  _$UpdateWarehouseLocationRequest _build() {
    final _$result = _$v ??
        _$UpdateWarehouseLocationRequest._(
          name: name,
          active: active,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
