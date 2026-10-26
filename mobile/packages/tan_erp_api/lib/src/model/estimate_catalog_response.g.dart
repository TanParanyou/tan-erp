// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_catalog_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateCatalogResponse extends EstimateCatalogResponse {
  @override
  final BuiltList<EstimateCatalogItemResponse>? items;
  @override
  final CatalogFacetsResponse? facets;
  @override
  final CatalogPageInfoResponse? pageInfo;

  factory _$EstimateCatalogResponse(
          [void Function(EstimateCatalogResponseBuilder)? updates]) =>
      (EstimateCatalogResponseBuilder()..update(updates))._build();

  _$EstimateCatalogResponse._({this.items, this.facets, this.pageInfo})
      : super._();
  @override
  EstimateCatalogResponse rebuild(
          void Function(EstimateCatalogResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateCatalogResponseBuilder toBuilder() =>
      EstimateCatalogResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateCatalogResponse &&
        items == other.items &&
        facets == other.facets &&
        pageInfo == other.pageInfo;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, facets.hashCode);
    _$hash = $jc(_$hash, pageInfo.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateCatalogResponse')
          ..add('items', items)
          ..add('facets', facets)
          ..add('pageInfo', pageInfo))
        .toString();
  }
}

class EstimateCatalogResponseBuilder
    implements
        Builder<EstimateCatalogResponse, EstimateCatalogResponseBuilder> {
  _$EstimateCatalogResponse? _$v;

  ListBuilder<EstimateCatalogItemResponse>? _items;
  ListBuilder<EstimateCatalogItemResponse> get items =>
      _$this._items ??= ListBuilder<EstimateCatalogItemResponse>();
  set items(ListBuilder<EstimateCatalogItemResponse>? items) =>
      _$this._items = items;

  CatalogFacetsResponseBuilder? _facets;
  CatalogFacetsResponseBuilder get facets =>
      _$this._facets ??= CatalogFacetsResponseBuilder();
  set facets(CatalogFacetsResponseBuilder? facets) => _$this._facets = facets;

  CatalogPageInfoResponseBuilder? _pageInfo;
  CatalogPageInfoResponseBuilder get pageInfo =>
      _$this._pageInfo ??= CatalogPageInfoResponseBuilder();
  set pageInfo(CatalogPageInfoResponseBuilder? pageInfo) =>
      _$this._pageInfo = pageInfo;

  EstimateCatalogResponseBuilder() {
    EstimateCatalogResponse._defaults(this);
  }

  EstimateCatalogResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _facets = $v.facets?.toBuilder();
      _pageInfo = $v.pageInfo?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateCatalogResponse other) {
    _$v = other as _$EstimateCatalogResponse;
  }

  @override
  void update(void Function(EstimateCatalogResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateCatalogResponse build() => _build();

  _$EstimateCatalogResponse _build() {
    _$EstimateCatalogResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateCatalogResponse._(
            items: _items?.build(),
            facets: _facets?.build(),
            pageInfo: _pageInfo?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
        _$failedField = 'facets';
        _facets?.build();
        _$failedField = 'pageInfo';
        _pageInfo?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateCatalogResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
