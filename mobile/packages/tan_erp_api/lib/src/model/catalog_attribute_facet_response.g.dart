// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'catalog_attribute_facet_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CatalogAttributeFacetResponse extends CatalogAttributeFacetResponse {
  @override
  final String? key;
  @override
  final String? value;
  @override
  final int? count;

  factory _$CatalogAttributeFacetResponse(
          [void Function(CatalogAttributeFacetResponseBuilder)? updates]) =>
      (CatalogAttributeFacetResponseBuilder()..update(updates))._build();

  _$CatalogAttributeFacetResponse._({this.key, this.value, this.count})
      : super._();
  @override
  CatalogAttributeFacetResponse rebuild(
          void Function(CatalogAttributeFacetResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CatalogAttributeFacetResponseBuilder toBuilder() =>
      CatalogAttributeFacetResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CatalogAttributeFacetResponse &&
        key == other.key &&
        value == other.value &&
        count == other.count;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, key.hashCode);
    _$hash = $jc(_$hash, value.hashCode);
    _$hash = $jc(_$hash, count.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CatalogAttributeFacetResponse')
          ..add('key', key)
          ..add('value', value)
          ..add('count', count))
        .toString();
  }
}

class CatalogAttributeFacetResponseBuilder
    implements
        Builder<CatalogAttributeFacetResponse,
            CatalogAttributeFacetResponseBuilder> {
  _$CatalogAttributeFacetResponse? _$v;

  String? _key;
  String? get key => _$this._key;
  set key(String? key) => _$this._key = key;

  String? _value;
  String? get value => _$this._value;
  set value(String? value) => _$this._value = value;

  int? _count;
  int? get count => _$this._count;
  set count(int? count) => _$this._count = count;

  CatalogAttributeFacetResponseBuilder() {
    CatalogAttributeFacetResponse._defaults(this);
  }

  CatalogAttributeFacetResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _key = $v.key;
      _value = $v.value;
      _count = $v.count;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CatalogAttributeFacetResponse other) {
    _$v = other as _$CatalogAttributeFacetResponse;
  }

  @override
  void update(void Function(CatalogAttributeFacetResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CatalogAttributeFacetResponse build() => _build();

  _$CatalogAttributeFacetResponse _build() {
    final _$result = _$v ??
        _$CatalogAttributeFacetResponse._(
          key: key,
          value: value,
          count: count,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
