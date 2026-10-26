// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'catalog_page_info_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CatalogPageInfoResponse extends CatalogPageInfoResponse {
  @override
  final String? nextCursor;
  @override
  final bool? hasNextPage;

  factory _$CatalogPageInfoResponse(
          [void Function(CatalogPageInfoResponseBuilder)? updates]) =>
      (CatalogPageInfoResponseBuilder()..update(updates))._build();

  _$CatalogPageInfoResponse._({this.nextCursor, this.hasNextPage}) : super._();
  @override
  CatalogPageInfoResponse rebuild(
          void Function(CatalogPageInfoResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CatalogPageInfoResponseBuilder toBuilder() =>
      CatalogPageInfoResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CatalogPageInfoResponse &&
        nextCursor == other.nextCursor &&
        hasNextPage == other.hasNextPage;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, nextCursor.hashCode);
    _$hash = $jc(_$hash, hasNextPage.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CatalogPageInfoResponse')
          ..add('nextCursor', nextCursor)
          ..add('hasNextPage', hasNextPage))
        .toString();
  }
}

class CatalogPageInfoResponseBuilder
    implements
        Builder<CatalogPageInfoResponse, CatalogPageInfoResponseBuilder> {
  _$CatalogPageInfoResponse? _$v;

  String? _nextCursor;
  String? get nextCursor => _$this._nextCursor;
  set nextCursor(String? nextCursor) => _$this._nextCursor = nextCursor;

  bool? _hasNextPage;
  bool? get hasNextPage => _$this._hasNextPage;
  set hasNextPage(bool? hasNextPage) => _$this._hasNextPage = hasNextPage;

  CatalogPageInfoResponseBuilder() {
    CatalogPageInfoResponse._defaults(this);
  }

  CatalogPageInfoResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _nextCursor = $v.nextCursor;
      _hasNextPage = $v.hasNextPage;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CatalogPageInfoResponse other) {
    _$v = other as _$CatalogPageInfoResponse;
  }

  @override
  void update(void Function(CatalogPageInfoResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CatalogPageInfoResponse build() => _build();

  _$CatalogPageInfoResponse _build() {
    final _$result = _$v ??
        _$CatalogPageInfoResponse._(
          nextCursor: nextCursor,
          hasNextPage: hasNextPage,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
