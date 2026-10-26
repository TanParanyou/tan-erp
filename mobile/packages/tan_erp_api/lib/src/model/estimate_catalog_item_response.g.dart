// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_catalog_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateCatalogItemResponse extends EstimateCatalogItemResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final LocalizedTextResponse? name;
  @override
  final LocalizedTextResponse? description;
  @override
  final String? itemType;
  @override
  final String? costComponentType;
  @override
  final CategorySummaryResponse? category;
  @override
  final BrandSummaryResponse? brand;
  @override
  final UnitSummaryResponse? baseUnit;
  @override
  final JsonObject? attributes;
  @override
  final CatalogPrimaryImageResponse? primaryImage;
  @override
  final CatalogResolvedCostResponse? resolvedCost;

  factory _$EstimateCatalogItemResponse(
          [void Function(EstimateCatalogItemResponseBuilder)? updates]) =>
      (EstimateCatalogItemResponseBuilder()..update(updates))._build();

  _$EstimateCatalogItemResponse._(
      {this.id,
      this.code,
      this.name,
      this.description,
      this.itemType,
      this.costComponentType,
      this.category,
      this.brand,
      this.baseUnit,
      this.attributes,
      this.primaryImage,
      this.resolvedCost})
      : super._();
  @override
  EstimateCatalogItemResponse rebuild(
          void Function(EstimateCatalogItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateCatalogItemResponseBuilder toBuilder() =>
      EstimateCatalogItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateCatalogItemResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        description == other.description &&
        itemType == other.itemType &&
        costComponentType == other.costComponentType &&
        category == other.category &&
        brand == other.brand &&
        baseUnit == other.baseUnit &&
        attributes == other.attributes &&
        primaryImage == other.primaryImage &&
        resolvedCost == other.resolvedCost;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, itemType.hashCode);
    _$hash = $jc(_$hash, costComponentType.hashCode);
    _$hash = $jc(_$hash, category.hashCode);
    _$hash = $jc(_$hash, brand.hashCode);
    _$hash = $jc(_$hash, baseUnit.hashCode);
    _$hash = $jc(_$hash, attributes.hashCode);
    _$hash = $jc(_$hash, primaryImage.hashCode);
    _$hash = $jc(_$hash, resolvedCost.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateCatalogItemResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('description', description)
          ..add('itemType', itemType)
          ..add('costComponentType', costComponentType)
          ..add('category', category)
          ..add('brand', brand)
          ..add('baseUnit', baseUnit)
          ..add('attributes', attributes)
          ..add('primaryImage', primaryImage)
          ..add('resolvedCost', resolvedCost))
        .toString();
  }
}

class EstimateCatalogItemResponseBuilder
    implements
        Builder<EstimateCatalogItemResponse,
            EstimateCatalogItemResponseBuilder> {
  _$EstimateCatalogItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  LocalizedTextResponseBuilder? _description;
  LocalizedTextResponseBuilder get description =>
      _$this._description ??= LocalizedTextResponseBuilder();
  set description(LocalizedTextResponseBuilder? description) =>
      _$this._description = description;

  String? _itemType;
  String? get itemType => _$this._itemType;
  set itemType(String? itemType) => _$this._itemType = itemType;

  String? _costComponentType;
  String? get costComponentType => _$this._costComponentType;
  set costComponentType(String? costComponentType) =>
      _$this._costComponentType = costComponentType;

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

  JsonObject? _attributes;
  JsonObject? get attributes => _$this._attributes;
  set attributes(JsonObject? attributes) => _$this._attributes = attributes;

  CatalogPrimaryImageResponseBuilder? _primaryImage;
  CatalogPrimaryImageResponseBuilder get primaryImage =>
      _$this._primaryImage ??= CatalogPrimaryImageResponseBuilder();
  set primaryImage(CatalogPrimaryImageResponseBuilder? primaryImage) =>
      _$this._primaryImage = primaryImage;

  CatalogResolvedCostResponseBuilder? _resolvedCost;
  CatalogResolvedCostResponseBuilder get resolvedCost =>
      _$this._resolvedCost ??= CatalogResolvedCostResponseBuilder();
  set resolvedCost(CatalogResolvedCostResponseBuilder? resolvedCost) =>
      _$this._resolvedCost = resolvedCost;

  EstimateCatalogItemResponseBuilder() {
    EstimateCatalogItemResponse._defaults(this);
  }

  EstimateCatalogItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name?.toBuilder();
      _description = $v.description?.toBuilder();
      _itemType = $v.itemType;
      _costComponentType = $v.costComponentType;
      _category = $v.category?.toBuilder();
      _brand = $v.brand?.toBuilder();
      _baseUnit = $v.baseUnit?.toBuilder();
      _attributes = $v.attributes;
      _primaryImage = $v.primaryImage?.toBuilder();
      _resolvedCost = $v.resolvedCost?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateCatalogItemResponse other) {
    _$v = other as _$EstimateCatalogItemResponse;
  }

  @override
  void update(void Function(EstimateCatalogItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateCatalogItemResponse build() => _build();

  _$EstimateCatalogItemResponse _build() {
    _$EstimateCatalogItemResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateCatalogItemResponse._(
            id: id,
            code: code,
            name: _name?.build(),
            description: _description?.build(),
            itemType: itemType,
            costComponentType: costComponentType,
            category: _category?.build(),
            brand: _brand?.build(),
            baseUnit: _baseUnit?.build(),
            attributes: attributes,
            primaryImage: _primaryImage?.build(),
            resolvedCost: _resolvedCost?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
        _$failedField = 'description';
        _description?.build();

        _$failedField = 'category';
        _category?.build();
        _$failedField = 'brand';
        _brand?.build();
        _$failedField = 'baseUnit';
        _baseUnit?.build();

        _$failedField = 'primaryImage';
        _primaryImage?.build();
        _$failedField = 'resolvedCost';
        _resolvedCost?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateCatalogItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
