// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ItemResponse extends ItemResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? code;
  @override
  final String? itemType;
  @override
  final CategorySummaryResponse? category;
  @override
  final BrandSummaryResponse? brand;
  @override
  final UnitSummaryResponse? baseUnit;
  @override
  final LocalizedTextResponse? name;
  @override
  final LocalizedTextResponse? description;
  @override
  final String? taxCategoryCode;
  @override
  final String? availabilityMode;
  @override
  final ItemCapabilitiesResponse? capabilities;
  @override
  final BuiltMap<String, String>? attributes;
  @override
  final int? attributesSchemaVersion;
  @override
  final String? status;
  @override
  final bool? activatedOnce;
  @override
  final DateTime? activatedAtUtc;
  @override
  final String? activatedByUserId;
  @override
  final DateTime? inactiveAtUtc;
  @override
  final String? inactiveByUserId;
  @override
  final String? inactiveReasonCode;
  @override
  final String? inactiveReason;
  @override
  final BuiltList<ItemAliasResponse>? aliases;
  @override
  final BuiltList<ItemBranchAvailabilityResponse>? branchAvailabilities;
  @override
  final ItemImageSummaryResponse? primaryImage;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? createdByUserId;
  @override
  final DateTime? updatedAtUtc;
  @override
  final String? updatedByUserId;

  factory _$ItemResponse([void Function(ItemResponseBuilder)? updates]) =>
      (ItemResponseBuilder()..update(updates))._build();

  _$ItemResponse._(
      {this.id,
      this.organizationId,
      this.code,
      this.itemType,
      this.category,
      this.brand,
      this.baseUnit,
      this.name,
      this.description,
      this.taxCategoryCode,
      this.availabilityMode,
      this.capabilities,
      this.attributes,
      this.attributesSchemaVersion,
      this.status,
      this.activatedOnce,
      this.activatedAtUtc,
      this.activatedByUserId,
      this.inactiveAtUtc,
      this.inactiveByUserId,
      this.inactiveReasonCode,
      this.inactiveReason,
      this.aliases,
      this.branchAvailabilities,
      this.primaryImage,
      this.rowVersion,
      this.createdAtUtc,
      this.createdByUserId,
      this.updatedAtUtc,
      this.updatedByUserId})
      : super._();
  @override
  ItemResponse rebuild(void Function(ItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ItemResponseBuilder toBuilder() => ItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ItemResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        code == other.code &&
        itemType == other.itemType &&
        category == other.category &&
        brand == other.brand &&
        baseUnit == other.baseUnit &&
        name == other.name &&
        description == other.description &&
        taxCategoryCode == other.taxCategoryCode &&
        availabilityMode == other.availabilityMode &&
        capabilities == other.capabilities &&
        attributes == other.attributes &&
        attributesSchemaVersion == other.attributesSchemaVersion &&
        status == other.status &&
        activatedOnce == other.activatedOnce &&
        activatedAtUtc == other.activatedAtUtc &&
        activatedByUserId == other.activatedByUserId &&
        inactiveAtUtc == other.inactiveAtUtc &&
        inactiveByUserId == other.inactiveByUserId &&
        inactiveReasonCode == other.inactiveReasonCode &&
        inactiveReason == other.inactiveReason &&
        aliases == other.aliases &&
        branchAvailabilities == other.branchAvailabilities &&
        primaryImage == other.primaryImage &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        createdByUserId == other.createdByUserId &&
        updatedAtUtc == other.updatedAtUtc &&
        updatedByUserId == other.updatedByUserId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, itemType.hashCode);
    _$hash = $jc(_$hash, category.hashCode);
    _$hash = $jc(_$hash, brand.hashCode);
    _$hash = $jc(_$hash, baseUnit.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, taxCategoryCode.hashCode);
    _$hash = $jc(_$hash, availabilityMode.hashCode);
    _$hash = $jc(_$hash, capabilities.hashCode);
    _$hash = $jc(_$hash, attributes.hashCode);
    _$hash = $jc(_$hash, attributesSchemaVersion.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, activatedOnce.hashCode);
    _$hash = $jc(_$hash, activatedAtUtc.hashCode);
    _$hash = $jc(_$hash, activatedByUserId.hashCode);
    _$hash = $jc(_$hash, inactiveAtUtc.hashCode);
    _$hash = $jc(_$hash, inactiveByUserId.hashCode);
    _$hash = $jc(_$hash, inactiveReasonCode.hashCode);
    _$hash = $jc(_$hash, inactiveReason.hashCode);
    _$hash = $jc(_$hash, aliases.hashCode);
    _$hash = $jc(_$hash, branchAvailabilities.hashCode);
    _$hash = $jc(_$hash, primaryImage.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, createdByUserId.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedByUserId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ItemResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('code', code)
          ..add('itemType', itemType)
          ..add('category', category)
          ..add('brand', brand)
          ..add('baseUnit', baseUnit)
          ..add('name', name)
          ..add('description', description)
          ..add('taxCategoryCode', taxCategoryCode)
          ..add('availabilityMode', availabilityMode)
          ..add('capabilities', capabilities)
          ..add('attributes', attributes)
          ..add('attributesSchemaVersion', attributesSchemaVersion)
          ..add('status', status)
          ..add('activatedOnce', activatedOnce)
          ..add('activatedAtUtc', activatedAtUtc)
          ..add('activatedByUserId', activatedByUserId)
          ..add('inactiveAtUtc', inactiveAtUtc)
          ..add('inactiveByUserId', inactiveByUserId)
          ..add('inactiveReasonCode', inactiveReasonCode)
          ..add('inactiveReason', inactiveReason)
          ..add('aliases', aliases)
          ..add('branchAvailabilities', branchAvailabilities)
          ..add('primaryImage', primaryImage)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('createdByUserId', createdByUserId)
          ..add('updatedAtUtc', updatedAtUtc)
          ..add('updatedByUserId', updatedByUserId))
        .toString();
  }
}

class ItemResponseBuilder
    implements Builder<ItemResponse, ItemResponseBuilder> {
  _$ItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _organizationId;
  String? get organizationId => _$this._organizationId;
  set organizationId(String? organizationId) =>
      _$this._organizationId = organizationId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _itemType;
  String? get itemType => _$this._itemType;
  set itemType(String? itemType) => _$this._itemType = itemType;

  CategorySummaryResponseBuilder? _category;
  CategorySummaryResponseBuilder get category =>
      _$this._category ??= CategorySummaryResponseBuilder();
  set category(CategorySummaryResponseBuilder? category) =>
      _$this._category = category;

  BrandSummaryResponseBuilder? _brand;
  BrandSummaryResponseBuilder get brand =>
      _$this._brand ??= BrandSummaryResponseBuilder();
  set brand(BrandSummaryResponseBuilder? brand) => _$this._brand = brand;

  UnitSummaryResponseBuilder? _baseUnit;
  UnitSummaryResponseBuilder get baseUnit =>
      _$this._baseUnit ??= UnitSummaryResponseBuilder();
  set baseUnit(UnitSummaryResponseBuilder? baseUnit) =>
      _$this._baseUnit = baseUnit;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  LocalizedTextResponseBuilder? _description;
  LocalizedTextResponseBuilder get description =>
      _$this._description ??= LocalizedTextResponseBuilder();
  set description(LocalizedTextResponseBuilder? description) =>
      _$this._description = description;

  String? _taxCategoryCode;
  String? get taxCategoryCode => _$this._taxCategoryCode;
  set taxCategoryCode(String? taxCategoryCode) =>
      _$this._taxCategoryCode = taxCategoryCode;

  String? _availabilityMode;
  String? get availabilityMode => _$this._availabilityMode;
  set availabilityMode(String? availabilityMode) =>
      _$this._availabilityMode = availabilityMode;

  ItemCapabilitiesResponseBuilder? _capabilities;
  ItemCapabilitiesResponseBuilder get capabilities =>
      _$this._capabilities ??= ItemCapabilitiesResponseBuilder();
  set capabilities(ItemCapabilitiesResponseBuilder? capabilities) =>
      _$this._capabilities = capabilities;

  MapBuilder<String, String>? _attributes;
  MapBuilder<String, String> get attributes =>
      _$this._attributes ??= MapBuilder<String, String>();
  set attributes(MapBuilder<String, String>? attributes) =>
      _$this._attributes = attributes;

  int? _attributesSchemaVersion;
  int? get attributesSchemaVersion => _$this._attributesSchemaVersion;
  set attributesSchemaVersion(int? attributesSchemaVersion) =>
      _$this._attributesSchemaVersion = attributesSchemaVersion;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  bool? _activatedOnce;
  bool? get activatedOnce => _$this._activatedOnce;
  set activatedOnce(bool? activatedOnce) =>
      _$this._activatedOnce = activatedOnce;

  DateTime? _activatedAtUtc;
  DateTime? get activatedAtUtc => _$this._activatedAtUtc;
  set activatedAtUtc(DateTime? activatedAtUtc) =>
      _$this._activatedAtUtc = activatedAtUtc;

  String? _activatedByUserId;
  String? get activatedByUserId => _$this._activatedByUserId;
  set activatedByUserId(String? activatedByUserId) =>
      _$this._activatedByUserId = activatedByUserId;

  DateTime? _inactiveAtUtc;
  DateTime? get inactiveAtUtc => _$this._inactiveAtUtc;
  set inactiveAtUtc(DateTime? inactiveAtUtc) =>
      _$this._inactiveAtUtc = inactiveAtUtc;

  String? _inactiveByUserId;
  String? get inactiveByUserId => _$this._inactiveByUserId;
  set inactiveByUserId(String? inactiveByUserId) =>
      _$this._inactiveByUserId = inactiveByUserId;

  String? _inactiveReasonCode;
  String? get inactiveReasonCode => _$this._inactiveReasonCode;
  set inactiveReasonCode(String? inactiveReasonCode) =>
      _$this._inactiveReasonCode = inactiveReasonCode;

  String? _inactiveReason;
  String? get inactiveReason => _$this._inactiveReason;
  set inactiveReason(String? inactiveReason) =>
      _$this._inactiveReason = inactiveReason;

  ListBuilder<ItemAliasResponse>? _aliases;
  ListBuilder<ItemAliasResponse> get aliases =>
      _$this._aliases ??= ListBuilder<ItemAliasResponse>();
  set aliases(ListBuilder<ItemAliasResponse>? aliases) =>
      _$this._aliases = aliases;

  ListBuilder<ItemBranchAvailabilityResponse>? _branchAvailabilities;
  ListBuilder<ItemBranchAvailabilityResponse> get branchAvailabilities =>
      _$this._branchAvailabilities ??=
          ListBuilder<ItemBranchAvailabilityResponse>();
  set branchAvailabilities(
          ListBuilder<ItemBranchAvailabilityResponse>? branchAvailabilities) =>
      _$this._branchAvailabilities = branchAvailabilities;

  ItemImageSummaryResponseBuilder? _primaryImage;
  ItemImageSummaryResponseBuilder get primaryImage =>
      _$this._primaryImage ??= ItemImageSummaryResponseBuilder();
  set primaryImage(ItemImageSummaryResponseBuilder? primaryImage) =>
      _$this._primaryImage = primaryImage;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _createdByUserId;
  String? get createdByUserId => _$this._createdByUserId;
  set createdByUserId(String? createdByUserId) =>
      _$this._createdByUserId = createdByUserId;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  String? _updatedByUserId;
  String? get updatedByUserId => _$this._updatedByUserId;
  set updatedByUserId(String? updatedByUserId) =>
      _$this._updatedByUserId = updatedByUserId;

  ItemResponseBuilder() {
    ItemResponse._defaults(this);
  }

  ItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _code = $v.code;
      _itemType = $v.itemType;
      _category = $v.category?.toBuilder();
      _brand = $v.brand?.toBuilder();
      _baseUnit = $v.baseUnit?.toBuilder();
      _name = $v.name?.toBuilder();
      _description = $v.description?.toBuilder();
      _taxCategoryCode = $v.taxCategoryCode;
      _availabilityMode = $v.availabilityMode;
      _capabilities = $v.capabilities?.toBuilder();
      _attributes = $v.attributes?.toBuilder();
      _attributesSchemaVersion = $v.attributesSchemaVersion;
      _status = $v.status;
      _activatedOnce = $v.activatedOnce;
      _activatedAtUtc = $v.activatedAtUtc;
      _activatedByUserId = $v.activatedByUserId;
      _inactiveAtUtc = $v.inactiveAtUtc;
      _inactiveByUserId = $v.inactiveByUserId;
      _inactiveReasonCode = $v.inactiveReasonCode;
      _inactiveReason = $v.inactiveReason;
      _aliases = $v.aliases?.toBuilder();
      _branchAvailabilities = $v.branchAvailabilities?.toBuilder();
      _primaryImage = $v.primaryImage?.toBuilder();
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _createdByUserId = $v.createdByUserId;
      _updatedAtUtc = $v.updatedAtUtc;
      _updatedByUserId = $v.updatedByUserId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ItemResponse other) {
    _$v = other as _$ItemResponse;
  }

  @override
  void update(void Function(ItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ItemResponse build() => _build();

  _$ItemResponse _build() {
    _$ItemResponse _$result;
    try {
      _$result = _$v ??
          _$ItemResponse._(
            id: id,
            organizationId: organizationId,
            code: code,
            itemType: itemType,
            category: _category?.build(),
            brand: _brand?.build(),
            baseUnit: _baseUnit?.build(),
            name: _name?.build(),
            description: _description?.build(),
            taxCategoryCode: taxCategoryCode,
            availabilityMode: availabilityMode,
            capabilities: _capabilities?.build(),
            attributes: _attributes?.build(),
            attributesSchemaVersion: attributesSchemaVersion,
            status: status,
            activatedOnce: activatedOnce,
            activatedAtUtc: activatedAtUtc,
            activatedByUserId: activatedByUserId,
            inactiveAtUtc: inactiveAtUtc,
            inactiveByUserId: inactiveByUserId,
            inactiveReasonCode: inactiveReasonCode,
            inactiveReason: inactiveReason,
            aliases: _aliases?.build(),
            branchAvailabilities: _branchAvailabilities?.build(),
            primaryImage: _primaryImage?.build(),
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            createdByUserId: createdByUserId,
            updatedAtUtc: updatedAtUtc,
            updatedByUserId: updatedByUserId,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'category';
        _category?.build();
        _$failedField = 'brand';
        _brand?.build();
        _$failedField = 'baseUnit';
        _baseUnit?.build();
        _$failedField = 'name';
        _name?.build();
        _$failedField = 'description';
        _description?.build();

        _$failedField = 'capabilities';
        _capabilities?.build();
        _$failedField = 'attributes';
        _attributes?.build();

        _$failedField = 'aliases';
        _aliases?.build();
        _$failedField = 'branchAvailabilities';
        _branchAvailabilities?.build();
        _$failedField = 'primaryImage';
        _primaryImage?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
