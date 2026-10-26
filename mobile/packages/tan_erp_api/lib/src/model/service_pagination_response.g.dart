// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_pagination_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServicePaginationResponse extends ServicePaginationResponse {
  @override
  final int? page;
  @override
  final int? pageSize;
  @override
  final int? totalCount;
  @override
  final int? totalPages;

  factory _$ServicePaginationResponse(
          [void Function(ServicePaginationResponseBuilder)? updates]) =>
      (ServicePaginationResponseBuilder()..update(updates))._build();

  _$ServicePaginationResponse._(
      {this.page, this.pageSize, this.totalCount, this.totalPages})
      : super._();
  @override
  ServicePaginationResponse rebuild(
          void Function(ServicePaginationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServicePaginationResponseBuilder toBuilder() =>
      ServicePaginationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServicePaginationResponse &&
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
    return (newBuiltValueToStringHelper(r'ServicePaginationResponse')
          ..add('page', page)
          ..add('pageSize', pageSize)
          ..add('totalCount', totalCount)
          ..add('totalPages', totalPages))
        .toString();
  }
}

class ServicePaginationResponseBuilder
    implements
        Builder<ServicePaginationResponse, ServicePaginationResponseBuilder> {
  _$ServicePaginationResponse? _$v;

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

  ServicePaginationResponseBuilder() {
    ServicePaginationResponse._defaults(this);
  }

  ServicePaginationResponseBuilder get _$this {
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
  void replace(ServicePaginationResponse other) {
    _$v = other as _$ServicePaginationResponse;
  }

  @override
  void update(void Function(ServicePaginationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServicePaginationResponse build() => _build();

  _$ServicePaginationResponse _build() {
    final _$result = _$v ??
        _$ServicePaginationResponse._(
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
