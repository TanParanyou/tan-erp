// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'inventory_location_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InventoryLocationRefResponse extends InventoryLocationRefResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final bool? isDefault;

  factory _$InventoryLocationRefResponse(
          [void Function(InventoryLocationRefResponseBuilder)? updates]) =>
      (InventoryLocationRefResponseBuilder()..update(updates))._build();

  _$InventoryLocationRefResponse._(
      {this.id, this.code, this.name, this.isDefault})
      : super._();
  @override
  InventoryLocationRefResponse rebuild(
          void Function(InventoryLocationRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InventoryLocationRefResponseBuilder toBuilder() =>
      InventoryLocationRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InventoryLocationRefResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        isDefault == other.isDefault;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, isDefault.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'InventoryLocationRefResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('isDefault', isDefault))
        .toString();
  }
}

class InventoryLocationRefResponseBuilder
    implements
        Builder<InventoryLocationRefResponse,
            InventoryLocationRefResponseBuilder> {
  _$InventoryLocationRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  bool? _isDefault;
  bool? get isDefault => _$this._isDefault;
  set isDefault(bool? isDefault) => _$this._isDefault = isDefault;

  InventoryLocationRefResponseBuilder() {
    InventoryLocationRefResponse._defaults(this);
  }

  InventoryLocationRefResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name;
      _isDefault = $v.isDefault;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(InventoryLocationRefResponse other) {
    _$v = other as _$InventoryLocationRefResponse;
  }

  @override
  void update(void Function(InventoryLocationRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InventoryLocationRefResponse build() => _build();

  _$InventoryLocationRefResponse _build() {
    final _$result = _$v ??
        _$InventoryLocationRefResponse._(
          id: id,
          code: code,
          name: name,
          isDefault: isDefault,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
