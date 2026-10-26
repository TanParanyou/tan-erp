// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'catalog_brand_facet_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CatalogBrandFacetResponse extends CatalogBrandFacetResponse {
  @override
  final String? id;
  @override
  final LocalizedTextResponse? name;
  @override
  final String? imageFileId;
  @override
  final int? count;

  factory _$CatalogBrandFacetResponse(
          [void Function(CatalogBrandFacetResponseBuilder)? updates]) =>
      (CatalogBrandFacetResponseBuilder()..update(updates))._build();

  _$CatalogBrandFacetResponse._(
      {this.id, this.name, this.imageFileId, this.count})
      : super._();
  @override
  CatalogBrandFacetResponse rebuild(
          void Function(CatalogBrandFacetResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CatalogBrandFacetResponseBuilder toBuilder() =>
      CatalogBrandFacetResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CatalogBrandFacetResponse &&
        id == other.id &&
        name == other.name &&
        imageFileId == other.imageFileId &&
        count == other.count;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jc(_$hash, count.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CatalogBrandFacetResponse')
          ..add('id', id)
          ..add('name', name)
          ..add('imageFileId', imageFileId)
          ..add('count', count))
        .toString();
  }
}

class CatalogBrandFacetResponseBuilder
    implements
        Builder<CatalogBrandFacetResponse, CatalogBrandFacetResponseBuilder> {
  _$CatalogBrandFacetResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  LocalizedTextResponseBuilder? _name;
  LocalizedTextResponseBuilder get name =>
      _$this._name ??= LocalizedTextResponseBuilder();
  set name(LocalizedTextResponseBuilder? name) => _$this._name = name;

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  int? _count;
  int? get count => _$this._count;
  set count(int? count) => _$this._count = count;

  CatalogBrandFacetResponseBuilder() {
    CatalogBrandFacetResponse._defaults(this);
  }

  CatalogBrandFacetResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name?.toBuilder();
      _imageFileId = $v.imageFileId;
      _count = $v.count;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CatalogBrandFacetResponse other) {
    _$v = other as _$CatalogBrandFacetResponse;
  }

  @override
  void update(void Function(CatalogBrandFacetResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CatalogBrandFacetResponse build() => _build();

  _$CatalogBrandFacetResponse _build() {
    _$CatalogBrandFacetResponse _$result;
    try {
      _$result = _$v ??
          _$CatalogBrandFacetResponse._(
            id: id,
            name: _name?.build(),
            imageFileId: imageFileId,
            count: count,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        _name?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CatalogBrandFacetResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
