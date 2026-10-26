// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reservation_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReservationResponse extends ReservationResponse {
  @override
  final String? id;
  @override
  final InventoryWarehouseRefResponse? warehouse;
  @override
  final InventoryItemRefResponse? item;
  @override
  final String? projectId;
  @override
  final String? projectCode;
  @override
  final double? quantity;
  @override
  final String? status;
  @override
  final String? note;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;

  factory _$ReservationResponse(
          [void Function(ReservationResponseBuilder)? updates]) =>
      (ReservationResponseBuilder()..update(updates))._build();

  _$ReservationResponse._(
      {this.id,
      this.warehouse,
      this.item,
      this.projectId,
      this.projectCode,
      this.quantity,
      this.status,
      this.note,
      this.rowVersion,
      this.createdAtUtc})
      : super._();
  @override
  ReservationResponse rebuild(
          void Function(ReservationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReservationResponseBuilder toBuilder() =>
      ReservationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReservationResponse &&
        id == other.id &&
        warehouse == other.warehouse &&
        item == other.item &&
        projectId == other.projectId &&
        projectCode == other.projectCode &&
        quantity == other.quantity &&
        status == other.status &&
        note == other.note &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, warehouse.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReservationResponse')
          ..add('id', id)
          ..add('warehouse', warehouse)
          ..add('item', item)
          ..add('projectId', projectId)
          ..add('projectCode', projectCode)
          ..add('quantity', quantity)
          ..add('status', status)
          ..add('note', note)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class ReservationResponseBuilder
    implements Builder<ReservationResponse, ReservationResponseBuilder> {
  _$ReservationResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  InventoryWarehouseRefResponseBuilder? _warehouse;
  InventoryWarehouseRefResponseBuilder get warehouse =>
      _$this._warehouse ??= InventoryWarehouseRefResponseBuilder();
  set warehouse(InventoryWarehouseRefResponseBuilder? warehouse) =>
      _$this._warehouse = warehouse;

  InventoryItemRefResponseBuilder? _item;
  InventoryItemRefResponseBuilder get item =>
      _$this._item ??= InventoryItemRefResponseBuilder();
  set item(InventoryItemRefResponseBuilder? item) => _$this._item = item;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  ReservationResponseBuilder() {
    ReservationResponse._defaults(this);
  }

  ReservationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _warehouse = $v.warehouse?.toBuilder();
      _item = $v.item?.toBuilder();
      _projectId = $v.projectId;
      _projectCode = $v.projectCode;
      _quantity = $v.quantity;
      _status = $v.status;
      _note = $v.note;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReservationResponse other) {
    _$v = other as _$ReservationResponse;
  }

  @override
  void update(void Function(ReservationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReservationResponse build() => _build();

  _$ReservationResponse _build() {
    _$ReservationResponse _$result;
    try {
      _$result = _$v ??
          _$ReservationResponse._(
            id: id,
            warehouse: _warehouse?.build(),
            item: _item?.build(),
            projectId: projectId,
            projectCode: projectCode,
            quantity: quantity,
            status: status,
            note: note,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'warehouse';
        _warehouse?.build();
        _$failedField = 'item';
        _item?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ReservationResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
