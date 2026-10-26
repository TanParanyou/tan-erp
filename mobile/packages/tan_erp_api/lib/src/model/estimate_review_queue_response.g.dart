// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_queue_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewQueueResponse extends EstimateReviewQueueResponse {
  @override
  final BuiltList<EstimateReviewQueueItemResponse>? items;
  @override
  final int? totalCount;
  @override
  final int? pageNumber;
  @override
  final int? pageSize;

  factory _$EstimateReviewQueueResponse(
          [void Function(EstimateReviewQueueResponseBuilder)? updates]) =>
      (EstimateReviewQueueResponseBuilder()..update(updates))._build();

  _$EstimateReviewQueueResponse._(
      {this.items, this.totalCount, this.pageNumber, this.pageSize})
      : super._();
  @override
  EstimateReviewQueueResponse rebuild(
          void Function(EstimateReviewQueueResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewQueueResponseBuilder toBuilder() =>
      EstimateReviewQueueResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewQueueResponse &&
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
    return (newBuiltValueToStringHelper(r'EstimateReviewQueueResponse')
          ..add('items', items)
          ..add('totalCount', totalCount)
          ..add('pageNumber', pageNumber)
          ..add('pageSize', pageSize))
        .toString();
  }
}

class EstimateReviewQueueResponseBuilder
    implements
        Builder<EstimateReviewQueueResponse,
            EstimateReviewQueueResponseBuilder> {
  _$EstimateReviewQueueResponse? _$v;

  ListBuilder<EstimateReviewQueueItemResponse>? _items;
  ListBuilder<EstimateReviewQueueItemResponse> get items =>
      _$this._items ??= ListBuilder<EstimateReviewQueueItemResponse>();
  set items(ListBuilder<EstimateReviewQueueItemResponse>? items) =>
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

  EstimateReviewQueueResponseBuilder() {
    EstimateReviewQueueResponse._defaults(this);
  }

  EstimateReviewQueueResponseBuilder get _$this {
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
  void replace(EstimateReviewQueueResponse other) {
    _$v = other as _$EstimateReviewQueueResponse;
  }

  @override
  void update(void Function(EstimateReviewQueueResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewQueueResponse build() => _build();

  _$EstimateReviewQueueResponse _build() {
    _$EstimateReviewQueueResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateReviewQueueResponse._(
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
            r'EstimateReviewQueueResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
