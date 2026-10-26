// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_estimate_cost_component_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateEstimateCostComponentDto extends UpdateEstimateCostComponentDto {
  @override
  final String? id;
  @override
  final String? type;
  @override
  final String? description;
  @override
  final double? quantity;
  @override
  final String? unitCode;
  @override
  final double? unitCost;
  @override
  final String? currency;
  @override
  final int? sortOrder;
  @override
  final String? itemId;
  @override
  final String? costRecordId;
  @override
  final int? costRecordVersion;
  @override
  final String? provisionalReasonCode;
  @override
  final String? provisionalNote;
  @override
  final String? conversionId;

  factory _$UpdateEstimateCostComponentDto(
          [void Function(UpdateEstimateCostComponentDtoBuilder)? updates]) =>
      (UpdateEstimateCostComponentDtoBuilder()..update(updates))._build();

  _$UpdateEstimateCostComponentDto._(
      {this.id,
      this.type,
      this.description,
      this.quantity,
      this.unitCode,
      this.unitCost,
      this.currency,
      this.sortOrder,
      this.itemId,
      this.costRecordId,
      this.costRecordVersion,
      this.provisionalReasonCode,
      this.provisionalNote,
      this.conversionId})
      : super._();
  @override
  UpdateEstimateCostComponentDto rebuild(
          void Function(UpdateEstimateCostComponentDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateEstimateCostComponentDtoBuilder toBuilder() =>
      UpdateEstimateCostComponentDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateEstimateCostComponentDto &&
        id == other.id &&
        type == other.type &&
        description == other.description &&
        quantity == other.quantity &&
        unitCode == other.unitCode &&
        unitCost == other.unitCost &&
        currency == other.currency &&
        sortOrder == other.sortOrder &&
        itemId == other.itemId &&
        costRecordId == other.costRecordId &&
        costRecordVersion == other.costRecordVersion &&
        provisionalReasonCode == other.provisionalReasonCode &&
        provisionalNote == other.provisionalNote &&
        conversionId == other.conversionId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, type.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, unitCost.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, costRecordId.hashCode);
    _$hash = $jc(_$hash, costRecordVersion.hashCode);
    _$hash = $jc(_$hash, provisionalReasonCode.hashCode);
    _$hash = $jc(_$hash, provisionalNote.hashCode);
    _$hash = $jc(_$hash, conversionId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateEstimateCostComponentDto')
          ..add('id', id)
          ..add('type', type)
          ..add('description', description)
          ..add('quantity', quantity)
          ..add('unitCode', unitCode)
          ..add('unitCost', unitCost)
          ..add('currency', currency)
          ..add('sortOrder', sortOrder)
          ..add('itemId', itemId)
          ..add('costRecordId', costRecordId)
          ..add('costRecordVersion', costRecordVersion)
          ..add('provisionalReasonCode', provisionalReasonCode)
          ..add('provisionalNote', provisionalNote)
          ..add('conversionId', conversionId))
        .toString();
  }
}

class UpdateEstimateCostComponentDtoBuilder
    implements
        Builder<UpdateEstimateCostComponentDto,
            UpdateEstimateCostComponentDtoBuilder> {
  _$UpdateEstimateCostComponentDto? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _type;
  String? get type => _$this._type;
  set type(String? type) => _$this._type = type;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  double? _unitCost;
  double? get unitCost => _$this._unitCost;
  set unitCost(double? unitCost) => _$this._unitCost = unitCost;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _costRecordId;
  String? get costRecordId => _$this._costRecordId;
  set costRecordId(String? costRecordId) => _$this._costRecordId = costRecordId;

  int? _costRecordVersion;
  int? get costRecordVersion => _$this._costRecordVersion;
  set costRecordVersion(int? costRecordVersion) =>
      _$this._costRecordVersion = costRecordVersion;

  String? _provisionalReasonCode;
  String? get provisionalReasonCode => _$this._provisionalReasonCode;
  set provisionalReasonCode(String? provisionalReasonCode) =>
      _$this._provisionalReasonCode = provisionalReasonCode;

  String? _provisionalNote;
  String? get provisionalNote => _$this._provisionalNote;
  set provisionalNote(String? provisionalNote) =>
      _$this._provisionalNote = provisionalNote;

  String? _conversionId;
  String? get conversionId => _$this._conversionId;
  set conversionId(String? conversionId) => _$this._conversionId = conversionId;

  UpdateEstimateCostComponentDtoBuilder() {
    UpdateEstimateCostComponentDto._defaults(this);
  }

  UpdateEstimateCostComponentDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _type = $v.type;
      _description = $v.description;
      _quantity = $v.quantity;
      _unitCode = $v.unitCode;
      _unitCost = $v.unitCost;
      _currency = $v.currency;
      _sortOrder = $v.sortOrder;
      _itemId = $v.itemId;
      _costRecordId = $v.costRecordId;
      _costRecordVersion = $v.costRecordVersion;
      _provisionalReasonCode = $v.provisionalReasonCode;
      _provisionalNote = $v.provisionalNote;
      _conversionId = $v.conversionId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateEstimateCostComponentDto other) {
    _$v = other as _$UpdateEstimateCostComponentDto;
  }

  @override
  void update(void Function(UpdateEstimateCostComponentDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateEstimateCostComponentDto build() => _build();

  _$UpdateEstimateCostComponentDto _build() {
    final _$result = _$v ??
        _$UpdateEstimateCostComponentDto._(
          id: id,
          type: type,
          description: description,
          quantity: quantity,
          unitCode: unitCode,
          unitCost: unitCost,
          currency: currency,
          sortOrder: sortOrder,
          itemId: itemId,
          costRecordId: costRecordId,
          costRecordVersion: costRecordVersion,
          provisionalReasonCode: provisionalReasonCode,
          provisionalNote: provisionalNote,
          conversionId: conversionId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
