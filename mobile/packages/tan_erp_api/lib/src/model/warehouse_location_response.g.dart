// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'warehouse_location_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WarehouseLocationResponse extends WarehouseLocationResponse {
  @override
  final String? id;
  @override
  final String? warehouseId;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final bool? isDefault;
  @override
  final String? status;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;

  factory _$WarehouseLocationResponse(
          [void Function(WarehouseLocationResponseBuilder)? updates]) =>
      (WarehouseLocationResponseBuilder()..update(updates))._build();

  _$WarehouseLocationResponse._(
      {this.id,
      this.warehouseId,
      this.code,
      this.name,
      this.isDefault,
      this.status,
      this.rowVersion,
      this.createdAtUtc,
      this.updatedAtUtc})
      : super._();
  @override
  WarehouseLocationResponse rebuild(
          void Function(WarehouseLocationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WarehouseLocationResponseBuilder toBuilder() =>
      WarehouseLocationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WarehouseLocationResponse &&
        id == other.id &&
        warehouseId == other.warehouseId &&
        code == other.code &&
        name == other.name &&
        isDefault == other.isDefault &&
        status == other.status &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, isDefault.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WarehouseLocationResponse')
          ..add('id', id)
          ..add('warehouseId', warehouseId)
          ..add('code', code)
          ..add('name', name)
          ..add('isDefault', isDefault)
          ..add('status', status)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class WarehouseLocationResponseBuilder
    implements
        Builder<WarehouseLocationResponse, WarehouseLocationResponseBuilder> {
  _$WarehouseLocationResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  bool? _isDefault;
  bool? get isDefault => _$this._isDefault;
  set isDefault(bool? isDefault) => _$this._isDefault = isDefault;

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

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  WarehouseLocationResponseBuilder() {
    WarehouseLocationResponse._defaults(this);
  }

  WarehouseLocationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _warehouseId = $v.warehouseId;
      _code = $v.code;
      _name = $v.name;
      _isDefault = $v.isDefault;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WarehouseLocationResponse other) {
    _$v = other as _$WarehouseLocationResponse;
  }

  @override
  void update(void Function(WarehouseLocationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WarehouseLocationResponse build() => _build();

  _$WarehouseLocationResponse _build() {
    final _$result = _$v ??
        _$WarehouseLocationResponse._(
          id: id,
          warehouseId: warehouseId,
          code: code,
          name: name,
          isDefault: isDefault,
          status: status,
          rowVersion: rowVersion,
          createdAtUtc: createdAtUtc,
          updatedAtUtc: updatedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
