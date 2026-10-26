// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'warehouse_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WarehouseResponse extends WarehouseResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? address;
  @override
  final String? status;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;

  factory _$WarehouseResponse(
          [void Function(WarehouseResponseBuilder)? updates]) =>
      (WarehouseResponseBuilder()..update(updates))._build();

  _$WarehouseResponse._(
      {this.id,
      this.branchId,
      this.code,
      this.name,
      this.address,
      this.status,
      this.rowVersion,
      this.createdAtUtc})
      : super._();
  @override
  WarehouseResponse rebuild(void Function(WarehouseResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WarehouseResponseBuilder toBuilder() =>
      WarehouseResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WarehouseResponse &&
        id == other.id &&
        branchId == other.branchId &&
        code == other.code &&
        name == other.name &&
        address == other.address &&
        status == other.status &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, address.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WarehouseResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('code', code)
          ..add('name', name)
          ..add('address', address)
          ..add('status', status)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class WarehouseResponseBuilder
    implements Builder<WarehouseResponse, WarehouseResponseBuilder> {
  _$WarehouseResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _address;
  String? get address => _$this._address;
  set address(String? address) => _$this._address = address;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  WarehouseResponseBuilder() {
    WarehouseResponse._defaults(this);
  }

  WarehouseResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _code = $v.code;
      _name = $v.name;
      _address = $v.address;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WarehouseResponse other) {
    _$v = other as _$WarehouseResponse;
  }

  @override
  void update(void Function(WarehouseResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WarehouseResponse build() => _build();

  _$WarehouseResponse _build() {
    final _$result = _$v ??
        _$WarehouseResponse._(
          id: id,
          branchId: branchId,
          code: code,
          name: name,
          address: address,
          status: status,
          rowVersion: rowVersion,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
