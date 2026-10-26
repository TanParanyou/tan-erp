// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'catalog_facets_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CatalogFacetsResponse extends CatalogFacetsResponse {
  @override
  final BuiltList<CatalogFacetValueResponse>? itemTypes;
  @override
  final BuiltList<CatalogCategoryFacetResponse>? categories;
  @override
  final BuiltList<CatalogBrandFacetResponse>? brands;
  @override
  final BuiltList<CatalogAttributeFacetResponse>? attributes;

  factory _$CatalogFacetsResponse(
          [void Function(CatalogFacetsResponseBuilder)? updates]) =>
      (CatalogFacetsResponseBuilder()..update(updates))._build();

  _$CatalogFacetsResponse._(
      {this.itemTypes, this.categories, this.brands, this.attributes})
      : super._();
  @override
  CatalogFacetsResponse rebuild(
          void Function(CatalogFacetsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CatalogFacetsResponseBuilder toBuilder() =>
      CatalogFacetsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CatalogFacetsResponse &&
        itemTypes == other.itemTypes &&
        categories == other.categories &&
        brands == other.brands &&
        attributes == other.attributes;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, itemTypes.hashCode);
    _$hash = $jc(_$hash, categories.hashCode);
    _$hash = $jc(_$hash, brands.hashCode);
    _$hash = $jc(_$hash, attributes.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CatalogFacetsResponse')
          ..add('itemTypes', itemTypes)
          ..add('categories', categories)
          ..add('brands', brands)
          ..add('attributes', attributes))
        .toString();
  }
}

class CatalogFacetsResponseBuilder
    implements Builder<CatalogFacetsResponse, CatalogFacetsResponseBuilder> {
  _$CatalogFacetsResponse? _$v;

  ListBuilder<CatalogFacetValueResponse>? _itemTypes;
  ListBuilder<CatalogFacetValueResponse> get itemTypes =>
      _$this._itemTypes ??= ListBuilder<CatalogFacetValueResponse>();
  set itemTypes(ListBuilder<CatalogFacetValueResponse>? itemTypes) =>
      _$this._itemTypes = itemTypes;

  ListBuilder<CatalogCategoryFacetResponse>? _categories;
  ListBuilder<CatalogCategoryFacetResponse> get categories =>
      _$this._categories ??= ListBuilder<CatalogCategoryFacetResponse>();
  set categories(ListBuilder<CatalogCategoryFacetResponse>? categories) =>
      _$this._categories = categories;

  ListBuilder<CatalogBrandFacetResponse>? _brands;
  ListBuilder<CatalogBrandFacetResponse> get brands =>
      _$this._brands ??= ListBuilder<CatalogBrandFacetResponse>();
  set brands(ListBuilder<CatalogBrandFacetResponse>? brands) =>
      _$this._brands = brands;

  ListBuilder<CatalogAttributeFacetResponse>? _attributes;
  ListBuilder<CatalogAttributeFacetResponse> get attributes =>
      _$this._attributes ??= ListBuilder<CatalogAttributeFacetResponse>();
  set attributes(ListBuilder<CatalogAttributeFacetResponse>? attributes) =>
      _$this._attributes = attributes;

  CatalogFacetsResponseBuilder() {
    CatalogFacetsResponse._defaults(this);
  }

  CatalogFacetsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _itemTypes = $v.itemTypes?.toBuilder();
      _categories = $v.categories?.toBuilder();
      _brands = $v.brands?.toBuilder();
      _attributes = $v.attributes?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CatalogFacetsResponse other) {
    _$v = other as _$CatalogFacetsResponse;
  }

  @override
  void update(void Function(CatalogFacetsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CatalogFacetsResponse build() => _build();

  _$CatalogFacetsResponse _build() {
    _$CatalogFacetsResponse _$result;
    try {
      _$result = _$v ??
          _$CatalogFacetsResponse._(
            itemTypes: _itemTypes?.build(),
            categories: _categories?.build(),
            brands: _brands?.build(),
            attributes: _attributes?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'itemTypes';
        _itemTypes?.build();
        _$failedField = 'categories';
        _categories?.build();
        _$failedField = 'brands';
        _brands?.build();
        _$failedField = 'attributes';
        _attributes?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CatalogFacetsResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
