// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_estimate_work_item_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateEstimateWorkItemDto extends UpdateEstimateWorkItemDto {
  @override
  final String? id;
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
  final int? sortOrder;
  @override
  final BuiltList<UpdateEstimateCostComponentDto>? costComponents;
  @override
  final String? sellingRuleReasonCode;
  @override
  final String? itemId;
  @override
  final String? overrideReasonCode;
  @override
  final String? overrideReason;

  factory _$UpdateEstimateWorkItemDto(
          [void Function(UpdateEstimateWorkItemDtoBuilder)? updates]) =>
      (UpdateEstimateWorkItemDtoBuilder()..update(updates))._build();

  _$UpdateEstimateWorkItemDto._(
      {this.id,
      this.code,
      this.descriptionTh,
      this.descriptionEn,
      this.quantity,
      this.unitCode,
      this.sellingRuleType,
      this.sellingRuleValue,
      this.sortOrder,
      this.costComponents,
      this.sellingRuleReasonCode,
      this.itemId,
      this.overrideReasonCode,
      this.overrideReason})
      : super._();
  @override
  UpdateEstimateWorkItemDto rebuild(
          void Function(UpdateEstimateWorkItemDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateEstimateWorkItemDtoBuilder toBuilder() =>
      UpdateEstimateWorkItemDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateEstimateWorkItemDto &&
        id == other.id &&
        code == other.code &&
        descriptionTh == other.descriptionTh &&
        descriptionEn == other.descriptionEn &&
        quantity == other.quantity &&
        unitCode == other.unitCode &&
        sellingRuleType == other.sellingRuleType &&
        sellingRuleValue == other.sellingRuleValue &&
        sortOrder == other.sortOrder &&
        costComponents == other.costComponents &&
        sellingRuleReasonCode == other.sellingRuleReasonCode &&
        itemId == other.itemId &&
        overrideReasonCode == other.overrideReasonCode &&
        overrideReason == other.overrideReason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, descriptionTh.hashCode);
    _$hash = $jc(_$hash, descriptionEn.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, sellingRuleType.hashCode);
    _$hash = $jc(_$hash, sellingRuleValue.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, costComponents.hashCode);
    _$hash = $jc(_$hash, sellingRuleReasonCode.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, overrideReasonCode.hashCode);
    _$hash = $jc(_$hash, overrideReason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateEstimateWorkItemDto')
          ..add('id', id)
          ..add('code', code)
          ..add('descriptionTh', descriptionTh)
          ..add('descriptionEn', descriptionEn)
          ..add('quantity', quantity)
          ..add('unitCode', unitCode)
          ..add('sellingRuleType', sellingRuleType)
          ..add('sellingRuleValue', sellingRuleValue)
          ..add('sortOrder', sortOrder)
          ..add('costComponents', costComponents)
          ..add('sellingRuleReasonCode', sellingRuleReasonCode)
          ..add('itemId', itemId)
          ..add('overrideReasonCode', overrideReasonCode)
          ..add('overrideReason', overrideReason))
        .toString();
  }
}

class UpdateEstimateWorkItemDtoBuilder
    implements
        Builder<UpdateEstimateWorkItemDto, UpdateEstimateWorkItemDtoBuilder> {
  _$UpdateEstimateWorkItemDto? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  ListBuilder<UpdateEstimateCostComponentDto>? _costComponents;
  ListBuilder<UpdateEstimateCostComponentDto> get costComponents =>
      _$this._costComponents ??= ListBuilder<UpdateEstimateCostComponentDto>();
  set costComponents(
          ListBuilder<UpdateEstimateCostComponentDto>? costComponents) =>
      _$this._costComponents = costComponents;

  String? _sellingRuleReasonCode;
  String? get sellingRuleReasonCode => _$this._sellingRuleReasonCode;
  set sellingRuleReasonCode(String? sellingRuleReasonCode) =>
      _$this._sellingRuleReasonCode = sellingRuleReasonCode;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _overrideReasonCode;
  String? get overrideReasonCode => _$this._overrideReasonCode;
  set overrideReasonCode(String? overrideReasonCode) =>
      _$this._overrideReasonCode = overrideReasonCode;

  String? _overrideReason;
  String? get overrideReason => _$this._overrideReason;
  set overrideReason(String? overrideReason) =>
      _$this._overrideReason = overrideReason;

  UpdateEstimateWorkItemDtoBuilder() {
    UpdateEstimateWorkItemDto._defaults(this);
  }

  UpdateEstimateWorkItemDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _descriptionTh = $v.descriptionTh;
      _descriptionEn = $v.descriptionEn;
      _quantity = $v.quantity;
      _unitCode = $v.unitCode;
      _sellingRuleType = $v.sellingRuleType;
      _sellingRuleValue = $v.sellingRuleValue;
      _sortOrder = $v.sortOrder;
      _costComponents = $v.costComponents?.toBuilder();
      _sellingRuleReasonCode = $v.sellingRuleReasonCode;
      _itemId = $v.itemId;
      _overrideReasonCode = $v.overrideReasonCode;
      _overrideReason = $v.overrideReason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateEstimateWorkItemDto other) {
    _$v = other as _$UpdateEstimateWorkItemDto;
  }

  @override
  void update(void Function(UpdateEstimateWorkItemDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateEstimateWorkItemDto build() => _build();

  _$UpdateEstimateWorkItemDto _build() {
    _$UpdateEstimateWorkItemDto _$result;
    try {
      _$result = _$v ??
          _$UpdateEstimateWorkItemDto._(
            id: id,
            code: code,
            descriptionTh: descriptionTh,
            descriptionEn: descriptionEn,
            quantity: quantity,
            unitCode: unitCode,
            sellingRuleType: sellingRuleType,
            sellingRuleValue: sellingRuleValue,
            sortOrder: sortOrder,
            costComponents: _costComponents?.build(),
            sellingRuleReasonCode: sellingRuleReasonCode,
            itemId: itemId,
            overrideReasonCode: overrideReasonCode,
            overrideReason: overrideReason,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'costComponents';
        _costComponents?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateEstimateWorkItemDto', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
