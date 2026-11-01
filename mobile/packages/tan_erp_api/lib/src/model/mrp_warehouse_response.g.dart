// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_warehouse_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpWarehouseResponse extends MrpWarehouseResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$MrpWarehouseResponse(
          [void Function(MrpWarehouseResponseBuilder)? updates]) =>
      (MrpWarehouseResponseBuilder()..update(updates))._build();

  _$MrpWarehouseResponse._({this.id, this.code, this.name}) : super._();
  @override
  MrpWarehouseResponse rebuild(
          void Function(MrpWarehouseResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpWarehouseResponseBuilder toBuilder() =>
      MrpWarehouseResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpWarehouseResponse &&
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
    return (newBuiltValueToStringHelper(r'MrpWarehouseResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class MrpWarehouseResponseBuilder
    implements Builder<MrpWarehouseResponse, MrpWarehouseResponseBuilder> {
  _$MrpWarehouseResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  MrpWarehouseResponseBuilder() {
    MrpWarehouseResponse._defaults(this);
  }

  MrpWarehouseResponseBuilder get _$this {
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
  void replace(MrpWarehouseResponse other) {
    _$v = other as _$MrpWarehouseResponse;
  }

  @override
  void update(void Function(MrpWarehouseResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpWarehouseResponse build() => _build();

  _$MrpWarehouseResponse _build() {
    final _$result = _$v ??
        _$MrpWarehouseResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
