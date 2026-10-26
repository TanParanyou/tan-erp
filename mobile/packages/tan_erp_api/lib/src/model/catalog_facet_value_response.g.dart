// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'catalog_facet_value_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CatalogFacetValueResponse extends CatalogFacetValueResponse {
  @override
  final String? value;
  @override
  final int? count;

  factory _$CatalogFacetValueResponse(
          [void Function(CatalogFacetValueResponseBuilder)? updates]) =>
      (CatalogFacetValueResponseBuilder()..update(updates))._build();

  _$CatalogFacetValueResponse._({this.value, this.count}) : super._();
  @override
  CatalogFacetValueResponse rebuild(
          void Function(CatalogFacetValueResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CatalogFacetValueResponseBuilder toBuilder() =>
      CatalogFacetValueResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CatalogFacetValueResponse &&
        value == other.value &&
        count == other.count;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, value.hashCode);
    _$hash = $jc(_$hash, count.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CatalogFacetValueResponse')
          ..add('value', value)
          ..add('count', count))
        .toString();
  }
}

class CatalogFacetValueResponseBuilder
    implements
        Builder<CatalogFacetValueResponse, CatalogFacetValueResponseBuilder> {
  _$CatalogFacetValueResponse? _$v;

  String? _value;
  String? get value => _$this._value;
  set value(String? value) => _$this._value = value;

  int? _count;
  int? get count => _$this._count;
  set count(int? count) => _$this._count = count;

  CatalogFacetValueResponseBuilder() {
    CatalogFacetValueResponse._defaults(this);
  }

  CatalogFacetValueResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _value = $v.value;
      _count = $v.count;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CatalogFacetValueResponse other) {
    _$v = other as _$CatalogFacetValueResponse;
  }

  @override
  void update(void Function(CatalogFacetValueResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CatalogFacetValueResponse build() => _build();

  _$CatalogFacetValueResponse _build() {
    final _$result = _$v ??
        _$CatalogFacetValueResponse._(
          value: value,
          count: count,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
