// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quick_estimate_pagination_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuickEstimatePaginationResponse
    extends QuickEstimatePaginationResponse {
  @override
  final int? page;
  @override
  final int? pageSize;
  @override
  final int? totalCount;
  @override
  final int? totalPages;

  factory _$QuickEstimatePaginationResponse(
          [void Function(QuickEstimatePaginationResponseBuilder)? updates]) =>
      (QuickEstimatePaginationResponseBuilder()..update(updates))._build();

  _$QuickEstimatePaginationResponse._(
      {this.page, this.pageSize, this.totalCount, this.totalPages})
      : super._();
  @override
  QuickEstimatePaginationResponse rebuild(
          void Function(QuickEstimatePaginationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuickEstimatePaginationResponseBuilder toBuilder() =>
      QuickEstimatePaginationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuickEstimatePaginationResponse &&
        page == other.page &&
        pageSize == other.pageSize &&
        totalCount == other.totalCount &&
        totalPages == other.totalPages;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, page.hashCode);
    _$hash = $jc(_$hash, pageSize.hashCode);
    _$hash = $jc(_$hash, totalCount.hashCode);
    _$hash = $jc(_$hash, totalPages.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuickEstimatePaginationResponse')
          ..add('page', page)
          ..add('pageSize', pageSize)
          ..add('totalCount', totalCount)
          ..add('totalPages', totalPages))
        .toString();
  }
}

class QuickEstimatePaginationResponseBuilder
    implements
        Builder<QuickEstimatePaginationResponse,
            QuickEstimatePaginationResponseBuilder> {
  _$QuickEstimatePaginationResponse? _$v;

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

  QuickEstimatePaginationResponseBuilder() {
    QuickEstimatePaginationResponse._defaults(this);
  }

  QuickEstimatePaginationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _page = $v.page;
      _pageSize = $v.pageSize;
      _totalCount = $v.totalCount;
      _totalPages = $v.totalPages;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuickEstimatePaginationResponse other) {
    _$v = other as _$QuickEstimatePaginationResponse;
  }

  @override
  void update(void Function(QuickEstimatePaginationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuickEstimatePaginationResponse build() => _build();

  _$QuickEstimatePaginationResponse _build() {
    final _$result = _$v ??
        _$QuickEstimatePaginationResponse._(
          page: page,
          pageSize: pageSize,
          totalCount: totalCount,
          totalPages: totalPages,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
