// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pagination_metadata_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PaginationMetadataResponse extends PaginationMetadataResponse {
  @override
  final int? page;
  @override
  final int? pageSize;
  @override
  final int? totalCount;
  @override
  final int? totalPages;
  @override
  final String? nextCursor;

  factory _$PaginationMetadataResponse(
          [void Function(PaginationMetadataResponseBuilder)? updates]) =>
      (PaginationMetadataResponseBuilder()..update(updates))._build();

  _$PaginationMetadataResponse._(
      {this.page,
      this.pageSize,
      this.totalCount,
      this.totalPages,
      this.nextCursor})
      : super._();
  @override
  PaginationMetadataResponse rebuild(
          void Function(PaginationMetadataResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PaginationMetadataResponseBuilder toBuilder() =>
      PaginationMetadataResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PaginationMetadataResponse &&
        page == other.page &&
        pageSize == other.pageSize &&
        totalCount == other.totalCount &&
        totalPages == other.totalPages &&
        nextCursor == other.nextCursor;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, page.hashCode);
    _$hash = $jc(_$hash, pageSize.hashCode);
    _$hash = $jc(_$hash, totalCount.hashCode);
    _$hash = $jc(_$hash, totalPages.hashCode);
    _$hash = $jc(_$hash, nextCursor.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PaginationMetadataResponse')
          ..add('page', page)
          ..add('pageSize', pageSize)
          ..add('totalCount', totalCount)
          ..add('totalPages', totalPages)
          ..add('nextCursor', nextCursor))
        .toString();
  }
}

class PaginationMetadataResponseBuilder
    implements
        Builder<PaginationMetadataResponse, PaginationMetadataResponseBuilder> {
  _$PaginationMetadataResponse? _$v;

  int? _page;
  int? get page => _$this._page;
  set page(int? page) => _$this._page = page;

  int? _pageSize;
  int? get pageSize => _$this._pageSize;
  set pageSize(int? pageSize) => _$this._pageSize = pageSize;

  int? _totalCount;
  int? get totalCount => _$this._totalCount;
  set totalCount(int? totalCount) => _$this._totalCount = totalCount;

  int? _totalPages;
  int? get totalPages => _$this._totalPages;
  set totalPages(int? totalPages) => _$this._totalPages = totalPages;

  String? _nextCursor;
  String? get nextCursor => _$this._nextCursor;
  set nextCursor(String? nextCursor) => _$this._nextCursor = nextCursor;

  PaginationMetadataResponseBuilder() {
    PaginationMetadataResponse._defaults(this);
  }

  PaginationMetadataResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _page = $v.page;
      _pageSize = $v.pageSize;
      _totalCount = $v.totalCount;
      _totalPages = $v.totalPages;
      _nextCursor = $v.nextCursor;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PaginationMetadataResponse other) {
    _$v = other as _$PaginationMetadataResponse;
  }

  @override
  void update(void Function(PaginationMetadataResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PaginationMetadataResponse build() => _build();

  _$PaginationMetadataResponse _build() {
    final _$result = _$v ??
        _$PaginationMetadataResponse._(
          page: page,
          pageSize: pageSize,
          totalCount: totalCount,
          totalPages: totalPages,
          nextCursor: nextCursor,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
