// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_work_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateWorkItemResponse extends EstimateWorkItemResponse {
  @override
  final String? id;
  @override
  final String? estimateSectionId;
  @override
  final String? code;
  @override
  final String? descriptionTh;
  @override
  final String? descriptionEn;
  @override
  final double? quantity;
  @override
  final String? unitCode;
  @override
  final String? sellingRuleType;
  @override
  final double? sellingRuleValue;
  @override
  final String? sellingRuleReasonCode;
  @override
  final double? unitCost;
  @override
  final double? totalCost;
  @override
  final double? unitSellingPrice;
  @override
  final double? totalSellingPrice;
  @override
  final int? sortOrder;
  @override
  final BuiltList<EstimateCostComponentResponse>? costComponents;
  @override
  final EstimateWorkItemItemResponse? item;
  @override
  final String? overrideReasonCode;
  @override
  final String? overrideReason;

  factory _$EstimateWorkItemResponse(
          [void Function(EstimateWorkItemResponseBuilder)? updates]) =>
      (EstimateWorkItemResponseBuilder()..update(updates))._build();

  _$EstimateWorkItemResponse._(
      {this.id,
      this.estimateSectionId,
      this.code,
      this.descriptionTh,
      this.descriptionEn,
      this.quantity,
      this.unitCode,
      this.sellingRuleType,
      this.sellingRuleValue,
      this.sellingRuleReasonCode,
      this.unitCost,
      this.totalCost,
      this.unitSellingPrice,
      this.totalSellingPrice,
      this.sortOrder,
      this.costComponents,
      this.item,
      this.overrideReasonCode,
      this.overrideReason})
      : super._();
  @override
  EstimateWorkItemResponse rebuild(
          void Function(EstimateWorkItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateWorkItemResponseBuilder toBuilder() =>
      EstimateWorkItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateWorkItemResponse &&
        id == other.id &&
        estimateSectionId == other.estimateSectionId &&
        code == other.code &&
        descriptionTh == other.descriptionTh &&
        descriptionEn == other.descriptionEn &&
        quantity == other.quantity &&
        unitCode == other.unitCode &&
        sellingRuleType == other.sellingRuleType &&
        sellingRuleValue == other.sellingRuleValue &&
        sellingRuleReasonCode == other.sellingRuleReasonCode &&
        unitCost == other.unitCost &&
        totalCost == other.totalCost &&
        unitSellingPrice == other.unitSellingPrice &&
        totalSellingPrice == other.totalSellingPrice &&
        sortOrder == other.sortOrder &&
        costComponents == other.costComponents &&
        item == other.item &&
        overrideReasonCode == other.overrideReasonCode &&
        overrideReason == other.overrideReason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, estimateSectionId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, descriptionTh.hashCode);
    _$hash = $jc(_$hash, descriptionEn.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, sellingRuleType.hashCode);
    _$hash = $jc(_$hash, sellingRuleValue.hashCode);
    _$hash = $jc(_$hash, sellingRuleReasonCode.hashCode);
    _$hash = $jc(_$hash, unitCost.hashCode);
    _$hash = $jc(_$hash, totalCost.hashCode);
    _$hash = $jc(_$hash, unitSellingPrice.hashCode);
    _$hash = $jc(_$hash, totalSellingPrice.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, costComponents.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, overrideReasonCode.hashCode);
    _$hash = $jc(_$hash, overrideReason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateWorkItemResponse')
          ..add('id', id)
          ..add('estimateSectionId', estimateSectionId)
          ..add('code', code)
          ..add('descriptionTh', descriptionTh)
          ..add('descriptionEn', descriptionEn)
          ..add('quantity', quantity)
          ..add('unitCode', unitCode)
          ..add('sellingRuleType', sellingRuleType)
          ..add('sellingRuleValue', sellingRuleValue)
          ..add('sellingRuleReasonCode', sellingRuleReasonCode)
          ..add('unitCost', unitCost)
          ..add('totalCost', totalCost)
          ..add('unitSellingPrice', unitSellingPrice)
          ..add('totalSellingPrice', totalSellingPrice)
          ..add('sortOrder', sortOrder)
          ..add('costComponents', costComponents)
          ..add('item', item)
          ..add('overrideReasonCode', overrideReasonCode)
          ..add('overrideReason', overrideReason))
        .toString();
  }
}

class EstimateWorkItemResponseBuilder
    implements
        Builder<EstimateWorkItemResponse, EstimateWorkItemResponseBuilder> {
  _$EstimateWorkItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _estimateSectionId;
  String? get estimateSectionId => _$this._estimateSectionId;
  set estimateSectionId(String? estimateSectionId) =>
      _$this._estimateSectionId = estimateSectionId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _descriptionTh;
  String? get descriptionTh => _$this._descriptionTh;
  set descriptionTh(String? descriptionTh) =>
      _$this._descriptionTh = descriptionTh;

  String? _descriptionEn;
  String? get descriptionEn => _$this._descriptionEn;
  set descriptionEn(String? descriptionEn) =>
      _$this._descriptionEn = descriptionEn;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  String? _sellingRuleType;
  String? get sellingRuleType => _$this._sellingRuleType;
  set sellingRuleType(String? sellingRuleType) =>
      _$this._sellingRuleType = sellingRuleType;

  double? _sellingRuleValue;
  double? get sellingRuleValue => _$this._sellingRuleValue;
  set sellingRuleValue(double? sellingRuleValue) =>
      _$this._sellingRuleValue = sellingRuleValue;

  String? _sellingRuleReasonCode;
  String? get sellingRuleReasonCode => _$this._sellingRuleReasonCode;
  set sellingRuleReasonCode(String? sellingRuleReasonCode) =>
      _$this._sellingRuleReasonCode = sellingRuleReasonCode;

  double? _unitCost;
  double? get unitCost => _$this._unitCost;
  set unitCost(double? unitCost) => _$this._unitCost = unitCost;

  double? _totalCost;
  double? get totalCost => _$this._totalCost;
  set totalCost(double? totalCost) => _$this._totalCost = totalCost;

  double? _unitSellingPrice;
  double? get unitSellingPrice => _$this._unitSellingPrice;
  set unitSellingPrice(double? unitSellingPrice) =>
      _$this._unitSellingPrice = unitSellingPrice;

  double? _totalSellingPrice;
  double? get totalSellingPrice => _$this._totalSellingPrice;
  set totalSellingPrice(double? totalSellingPrice) =>
      _$this._totalSellingPrice = totalSellingPrice;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  ListBuilder<EstimateCostComponentResponse>? _costComponents;
  ListBuilder<EstimateCostComponentResponse> get costComponents =>
      _$this._costComponents ??= ListBuilder<EstimateCostComponentResponse>();
  set costComponents(
          ListBuilder<EstimateCostComponentResponse>? costComponents) =>
      _$this._costComponents = costComponents;

  EstimateWorkItemItemResponseBuilder? _item;
  EstimateWorkItemItemResponseBuilder get item =>
      _$this._item ??= EstimateWorkItemItemResponseBuilder();
  set item(EstimateWorkItemItemResponseBuilder? item) => _$this._item = item;

  String? _overrideReasonCode;
  String? get overrideReasonCode => _$this._overrideReasonCode;
  set overrideReasonCode(String? overrideReasonCode) =>
      _$this._overrideReasonCode = overrideReasonCode;

  String? _overrideReason;
  String? get overrideReason => _$this._overrideReason;
  set overrideReason(String? overrideReason) =>
      _$this._overrideReason = overrideReason;

  EstimateWorkItemResponseBuilder() {
    EstimateWorkItemResponse._defaults(this);
  }

  EstimateWorkItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _estimateSectionId = $v.estimateSectionId;
      _code = $v.code;
      _descriptionTh = $v.descriptionTh;
      _descriptionEn = $v.descriptionEn;
      _quantity = $v.quantity;
      _unitCode = $v.unitCode;
      _sellingRuleType = $v.sellingRuleType;
      _sellingRuleValue = $v.sellingRuleValue;
      _sellingRuleReasonCode = $v.sellingRuleReasonCode;
      _unitCost = $v.unitCost;
      _totalCost = $v.totalCost;
      _unitSellingPrice = $v.unitSellingPrice;
      _totalSellingPrice = $v.totalSellingPrice;
      _sortOrder = $v.sortOrder;
      _costComponents = $v.costComponents?.toBuilder();
      _item = $v.item?.toBuilder();
      _overrideReasonCode = $v.overrideReasonCode;
      _overrideReason = $v.overrideReason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateWorkItemResponse other) {
    _$v = other as _$EstimateWorkItemResponse;
  }

  @override
  void update(void Function(EstimateWorkItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateWorkItemResponse build() => _build();

  _$EstimateWorkItemResponse _build() {
    _$EstimateWorkItemResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateWorkItemResponse._(
            id: id,
            estimateSectionId: estimateSectionId,
            code: code,
            descriptionTh: descriptionTh,
            descriptionEn: descriptionEn,
            quantity: quantity,
            unitCode: unitCode,
            sellingRuleType: sellingRuleType,
            sellingRuleValue: sellingRuleValue,
            sellingRuleReasonCode: sellingRuleReasonCode,
            unitCost: unitCost,
            totalCost: totalCost,
            unitSellingPrice: unitSellingPrice,
            totalSellingPrice: totalSellingPrice,
            sortOrder: sortOrder,
            costComponents: _costComponents?.build(),
            item: _item?.build(),
            overrideReasonCode: overrideReasonCode,
            overrideReason: overrideReason,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'costComponents';
        _costComponents?.build();
        _$failedField = 'item';
        _item?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateWorkItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
