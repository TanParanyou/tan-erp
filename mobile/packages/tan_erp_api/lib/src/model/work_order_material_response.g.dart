// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_material_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderMaterialResponse extends WorkOrderMaterialResponse {
  @override
  final String? id;
  @override
  final ProductionItemResponse? item;
  @override
  final double? requiredQuantity;
  @override
  final double? issuedQuantity;
  @override
  final double? returnedQuantity;
  @override
  final double? netIssuedQuantity;
  @override
  final double? remainingQuantity;
  @override
  final double? issuedValue;
  @override
  final double? returnedValue;

  factory _$WorkOrderMaterialResponse(
          [void Function(WorkOrderMaterialResponseBuilder)? updates]) =>
      (WorkOrderMaterialResponseBuilder()..update(updates))._build();

  _$WorkOrderMaterialResponse._(
      {this.id,
      this.item,
      this.requiredQuantity,
      this.issuedQuantity,
      this.returnedQuantity,
      this.netIssuedQuantity,
      this.remainingQuantity,
      this.issuedValue,
      this.returnedValue})
      : super._();
  @override
  WorkOrderMaterialResponse rebuild(
          void Function(WorkOrderMaterialResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderMaterialResponseBuilder toBuilder() =>
      WorkOrderMaterialResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderMaterialResponse &&
        id == other.id &&
        item == other.item &&
        requiredQuantity == other.requiredQuantity &&
        issuedQuantity == other.issuedQuantity &&
        returnedQuantity == other.returnedQuantity &&
        netIssuedQuantity == other.netIssuedQuantity &&
        remainingQuantity == other.remainingQuantity &&
        issuedValue == other.issuedValue &&
        returnedValue == other.returnedValue;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, requiredQuantity.hashCode);
    _$hash = $jc(_$hash, issuedQuantity.hashCode);
    _$hash = $jc(_$hash, returnedQuantity.hashCode);
    _$hash = $jc(_$hash, netIssuedQuantity.hashCode);
    _$hash = $jc(_$hash, remainingQuantity.hashCode);
    _$hash = $jc(_$hash, issuedValue.hashCode);
    _$hash = $jc(_$hash, returnedValue.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderMaterialResponse')
          ..add('id', id)
          ..add('item', item)
          ..add('requiredQuantity', requiredQuantity)
          ..add('issuedQuantity', issuedQuantity)
          ..add('returnedQuantity', returnedQuantity)
          ..add('netIssuedQuantity', netIssuedQuantity)
          ..add('remainingQuantity', remainingQuantity)
          ..add('issuedValue', issuedValue)
          ..add('returnedValue', returnedValue))
        .toString();
  }
}

class WorkOrderMaterialResponseBuilder
    implements
        Builder<WorkOrderMaterialResponse, WorkOrderMaterialResponseBuilder> {
  _$WorkOrderMaterialResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  ProductionItemResponseBuilder? _item;
  ProductionItemResponseBuilder get item =>
      _$this._item ??= ProductionItemResponseBuilder();
  set item(ProductionItemResponseBuilder? item) => _$this._item = item;

  double? _requiredQuantity;
  double? get requiredQuantity => _$this._requiredQuantity;
  set requiredQuantity(double? requiredQuantity) =>
      _$this._requiredQuantity = requiredQuantity;

  double? _issuedQuantity;
  double? get issuedQuantity => _$this._issuedQuantity;
  set issuedQuantity(double? issuedQuantity) =>
      _$this._issuedQuantity = issuedQuantity;

  double? _returnedQuantity;
  double? get returnedQuantity => _$this._returnedQuantity;
  set returnedQuantity(double? returnedQuantity) =>
      _$this._returnedQuantity = returnedQuantity;

  double? _netIssuedQuantity;
  double? get netIssuedQuantity => _$this._netIssuedQuantity;
  set netIssuedQuantity(double? netIssuedQuantity) =>
      _$this._netIssuedQuantity = netIssuedQuantity;

  double? _remainingQuantity;
  double? get remainingQuantity => _$this._remainingQuantity;
  set remainingQuantity(double? remainingQuantity) =>
      _$this._remainingQuantity = remainingQuantity;

  double? _issuedValue;
  double? get issuedValue => _$this._issuedValue;
  set issuedValue(double? issuedValue) => _$this._issuedValue = issuedValue;

  double? _returnedValue;
  double? get returnedValue => _$this._returnedValue;
  set returnedValue(double? returnedValue) =>
      _$this._returnedValue = returnedValue;

  WorkOrderMaterialResponseBuilder() {
    WorkOrderMaterialResponse._defaults(this);
  }

  WorkOrderMaterialResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _item = $v.item?.toBuilder();
      _requiredQuantity = $v.requiredQuantity;
      _issuedQuantity = $v.issuedQuantity;
      _returnedQuantity = $v.returnedQuantity;
      _netIssuedQuantity = $v.netIssuedQuantity;
      _remainingQuantity = $v.remainingQuantity;
      _issuedValue = $v.issuedValue;
      _returnedValue = $v.returnedValue;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderMaterialResponse other) {
    _$v = other as _$WorkOrderMaterialResponse;
  }

  @override
  void update(void Function(WorkOrderMaterialResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderMaterialResponse build() => _build();

  _$WorkOrderMaterialResponse _build() {
    _$WorkOrderMaterialResponse _$result;
    try {
      _$result = _$v ??
          _$WorkOrderMaterialResponse._(
            id: id,
            item: _item?.build(),
            requiredQuantity: requiredQuantity,
            issuedQuantity: issuedQuantity,
            returnedQuantity: returnedQuantity,
            netIssuedQuantity: netIssuedQuantity,
            remainingQuantity: remainingQuantity,
            issuedValue: issuedValue,
            returnedValue: returnedValue,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'item';
        _item?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WorkOrderMaterialResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
