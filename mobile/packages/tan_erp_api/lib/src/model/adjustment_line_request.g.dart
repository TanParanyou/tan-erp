// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'adjustment_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdjustmentLineRequest extends AdjustmentLineRequest {
  @override
  final String? itemId;
  @override
  final double? countedQuantity;
  @override
  final double? unitCost;

  factory _$AdjustmentLineRequest(
          [void Function(AdjustmentLineRequestBuilder)? updates]) =>
      (AdjustmentLineRequestBuilder()..update(updates))._build();

  _$AdjustmentLineRequest._({this.itemId, this.countedQuantity, this.unitCost})
      : super._();
  @override
  AdjustmentLineRequest rebuild(
          void Function(AdjustmentLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdjustmentLineRequestBuilder toBuilder() =>
      AdjustmentLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdjustmentLineRequest &&
        itemId == other.itemId &&
        countedQuantity == other.countedQuantity &&
        unitCost == other.unitCost;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, countedQuantity.hashCode);
    _$hash = $jc(_$hash, unitCost.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdjustmentLineRequest')
          ..add('itemId', itemId)
          ..add('countedQuantity', countedQuantity)
          ..add('unitCost', unitCost))
        .toString();
  }
}

class AdjustmentLineRequestBuilder
    implements Builder<AdjustmentLineRequest, AdjustmentLineRequestBuilder> {
  _$AdjustmentLineRequest? _$v;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  double? _countedQuantity;
  double? get countedQuantity => _$this._countedQuantity;
  set countedQuantity(double? countedQuantity) =>
      _$this._countedQuantity = countedQuantity;

  double? _unitCost;
  double? get unitCost => _$this._unitCost;
  set unitCost(double? unitCost) => _$this._unitCost = unitCost;

  AdjustmentLineRequestBuilder() {
    AdjustmentLineRequest._defaults(this);
  }

  AdjustmentLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemId = $v.itemId;
      _countedQuantity = $v.countedQuantity;
      _unitCost = $v.unitCost;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdjustmentLineRequest other) {
    _$v = other as _$AdjustmentLineRequest;
  }

  @override
  void update(void Function(AdjustmentLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdjustmentLineRequest build() => _build();

  _$AdjustmentLineRequest _build() {
    final _$result = _$v ??
        _$AdjustmentLineRequest._(
          itemId: itemId,
          countedQuantity: countedQuantity,
          unitCost: unitCost,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
