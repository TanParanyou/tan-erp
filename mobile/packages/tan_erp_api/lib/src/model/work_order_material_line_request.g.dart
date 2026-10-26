// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_material_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderMaterialLineRequest extends WorkOrderMaterialLineRequest {
  @override
  final String? itemId;
  @override
  final double? quantity;

  factory _$WorkOrderMaterialLineRequest(
          [void Function(WorkOrderMaterialLineRequestBuilder)? updates]) =>
      (WorkOrderMaterialLineRequestBuilder()..update(updates))._build();

  _$WorkOrderMaterialLineRequest._({this.itemId, this.quantity}) : super._();
  @override
  WorkOrderMaterialLineRequest rebuild(
          void Function(WorkOrderMaterialLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderMaterialLineRequestBuilder toBuilder() =>
      WorkOrderMaterialLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderMaterialLineRequest &&
        itemId == other.itemId &&
        quantity == other.quantity;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderMaterialLineRequest')
          ..add('itemId', itemId)
          ..add('quantity', quantity))
        .toString();
  }
}

class WorkOrderMaterialLineRequestBuilder
    implements
        Builder<WorkOrderMaterialLineRequest,
            WorkOrderMaterialLineRequestBuilder> {
  _$WorkOrderMaterialLineRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  WorkOrderMaterialLineRequestBuilder() {
    WorkOrderMaterialLineRequest._defaults(this);
  }

  WorkOrderMaterialLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _quantity = $v.quantity;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderMaterialLineRequest other) {
    _$v = other as _$WorkOrderMaterialLineRequest;
  }

  @override
  void update(void Function(WorkOrderMaterialLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderMaterialLineRequest build() => _build();

  _$WorkOrderMaterialLineRequest _build() {
    final _$result = _$v ??
        _$WorkOrderMaterialLineRequest._(
          itemId: itemId,
          quantity: quantity,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
