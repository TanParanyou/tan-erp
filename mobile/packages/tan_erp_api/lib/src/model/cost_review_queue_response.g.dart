// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cost_review_queue_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CostReviewQueueResponse extends CostReviewQueueResponse {
  @override
  final BuiltList<CostReviewQueueItemResponse>? items;
  @override
  final int? totalCount;
  @override
  final int? pageNumber;
  @override
  final int? pageSize;

  factory _$CostReviewQueueResponse(
          [void Function(CostReviewQueueResponseBuilder)? updates]) =>
      (CostReviewQueueResponseBuilder()..update(updates))._build();

  _$CostReviewQueueResponse._(
      {this.items, this.totalCount, this.pageNumber, this.pageSize})
      : super._();
  @override
  CostReviewQueueResponse rebuild(
          void Function(CostReviewQueueResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CostReviewQueueResponseBuilder toBuilder() =>
      CostReviewQueueResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CostReviewQueueResponse &&
        items == other.items &&
        totalCount == other.totalCount &&
        pageNumber == other.pageNumber &&
        pageSize == other.pageSize;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, totalCount.hashCode);
    _$hash = $jc(_$hash, pageNumber.hashCode);
    _$hash = $jc(_$hash, pageSize.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CostReviewQueueResponse')
          ..add('items', items)
          ..add('totalCount', totalCount)
          ..add('pageNumber', pageNumber)
          ..add('pageSize', pageSize))
        .toString();
  }
}

class CostReviewQueueResponseBuilder
    implements
        Builder<CostReviewQueueResponse, CostReviewQueueResponseBuilder> {
  _$CostReviewQueueResponse? _$v;

  ListBuilder<CostReviewQueueItemResponse>? _items;
  ListBuilder<CostReviewQueueItemResponse> get items =>
      _$this._items ??= ListBuilder<CostReviewQueueItemResponse>();
  set items(ListBuilder<CostReviewQueueItemResponse>? items) =>
      _$this._items = items;

  int? _totalCount;
  int? get totalCount => _$this._totalCount;
  set totalCount(int? totalCount) => _$this._totalCount = totalCount;

  int? _pageNumber;
  int? get pageNumber => _$this._pageNumber;
  set pageNumber(int? pageNumber) => _$this._pageNumber = pageNumber;

  int? _pageSize;
  int? get pageSize => _$this._pageSize;
  set pageSize(int? pageSize) => _$this._pageSize = pageSize;

  CostReviewQueueResponseBuilder() {
    CostReviewQueueResponse._defaults(this);
  }

  CostReviewQueueResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _totalCount = $v.totalCount;
      _pageNumber = $v.pageNumber;
      _pageSize = $v.pageSize;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CostReviewQueueResponse other) {
    _$v = other as _$CostReviewQueueResponse;
  }

  @override
  void update(void Function(CostReviewQueueResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CostReviewQueueResponse build() => _build();

  _$CostReviewQueueResponse _build() {
    _$CostReviewQueueResponse _$result;
    try {
      _$result = _$v ??
          _$CostReviewQueueResponse._(
            items: _items?.build(),
            totalCount: totalCount,
            pageNumber: pageNumber,
            pageSize: pageSize,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CostReviewQueueResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
