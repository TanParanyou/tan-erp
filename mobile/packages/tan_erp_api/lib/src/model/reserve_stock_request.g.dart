// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reserve_stock_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReserveStockRequest extends ReserveStockRequest {
  @override
  final String? warehouseId;
  @override
  final String? itemId;
  @override
  final String? projectId;
  @override
  final double? quantity;
  @override
  final String? note;

  factory _$ReserveStockRequest(
          [void Function(ReserveStockRequestBuilder)? updates]) =>
      (ReserveStockRequestBuilder()..update(updates))._build();

  _$ReserveStockRequest._(
      {this.warehouseId, this.itemId, this.projectId, this.quantity, this.note})
      : super._();
  @override
  ReserveStockRequest rebuild(
          void Function(ReserveStockRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReserveStockRequestBuilder toBuilder() =>
      ReserveStockRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReserveStockRequest &&
        warehouseId == other.warehouseId &&
        itemId == other.itemId &&
        projectId == other.projectId &&
        quantity == other.quantity &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReserveStockRequest')
          ..add('warehouseId', warehouseId)
          ..add('itemId', itemId)
          ..add('projectId', projectId)
          ..add('quantity', quantity)
          ..add('note', note))
        .toString();
  }
}

class ReserveStockRequestBuilder
    implements Builder<ReserveStockRequest, ReserveStockRequestBuilder> {
  _$ReserveStockRequest? _$v;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ReserveStockRequestBuilder() {
    ReserveStockRequest._defaults(this);
  }

  ReserveStockRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _warehouseId = $v.warehouseId;
      _itemId = $v.itemId;
      _projectId = $v.projectId;
      _quantity = $v.quantity;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReserveStockRequest other) {
    _$v = other as _$ReserveStockRequest;
  }

  @override
  void update(void Function(ReserveStockRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReserveStockRequest build() => _build();

  _$ReserveStockRequest _build() {
    final _$result = _$v ??
        _$ReserveStockRequest._(
          warehouseId: warehouseId,
          itemId: itemId,
          projectId: projectId,
          quantity: quantity,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
