// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'inventory_warehouse_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InventoryWarehouseRefResponse extends InventoryWarehouseRefResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$InventoryWarehouseRefResponse(
          [void Function(InventoryWarehouseRefResponseBuilder)? updates]) =>
      (InventoryWarehouseRefResponseBuilder()..update(updates))._build();

  _$InventoryWarehouseRefResponse._({this.id, this.code, this.name})
      : super._();
  @override
  InventoryWarehouseRefResponse rebuild(
          void Function(InventoryWarehouseRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InventoryWarehouseRefResponseBuilder toBuilder() =>
      InventoryWarehouseRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InventoryWarehouseRefResponse &&
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
    return (newBuiltValueToStringHelper(r'InventoryWarehouseRefResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class InventoryWarehouseRefResponseBuilder
    implements
        Builder<InventoryWarehouseRefResponse,
            InventoryWarehouseRefResponseBuilder> {
  _$InventoryWarehouseRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  InventoryWarehouseRefResponseBuilder() {
    InventoryWarehouseRefResponse._defaults(this);
  }

  InventoryWarehouseRefResponseBuilder get _$this {
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
  void replace(InventoryWarehouseRefResponse other) {
    _$v = other as _$InventoryWarehouseRefResponse;
  }

  @override
  void update(void Function(InventoryWarehouseRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InventoryWarehouseRefResponse build() => _build();

  _$InventoryWarehouseRefResponse _build() {
    final _$result = _$v ??
        _$InventoryWarehouseRefResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
