// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quick_estimate_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuickEstimateListResponse extends QuickEstimateListResponse {
  @override
  final BuiltList<QuickEstimateListItemResponse>? items;
  @override
  final QuickEstimatePaginationResponse? pagination;

  factory _$QuickEstimateListResponse(
          [void Function(QuickEstimateListResponseBuilder)? updates]) =>
      (QuickEstimateListResponseBuilder()..update(updates))._build();

  _$QuickEstimateListResponse._({this.items, this.pagination}) : super._();
  @override
  QuickEstimateListResponse rebuild(
          void Function(QuickEstimateListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuickEstimateListResponseBuilder toBuilder() =>
      QuickEstimateListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuickEstimateListResponse &&
        items == other.items &&
        pagination == other.pagination;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, pagination.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuickEstimateListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class QuickEstimateListResponseBuilder
    implements
        Builder<QuickEstimateListResponse, QuickEstimateListResponseBuilder> {
  _$QuickEstimateListResponse? _$v;

  ListBuilder<QuickEstimateListItemResponse>? _items;
  ListBuilder<QuickEstimateListItemResponse> get items =>
      _$this._items ??= ListBuilder<QuickEstimateListItemResponse>();
  set items(ListBuilder<QuickEstimateListItemResponse>? items) =>
      _$this._items = items;

  QuickEstimatePaginationResponseBuilder? _pagination;
  QuickEstimatePaginationResponseBuilder get pagination =>
      _$this._pagination ??= QuickEstimatePaginationResponseBuilder();
  set pagination(QuickEstimatePaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  QuickEstimateListResponseBuilder() {
    QuickEstimateListResponse._defaults(this);
  }

  QuickEstimateListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuickEstimateListResponse other) {
    _$v = other as _$QuickEstimateListResponse;
  }

  @override
  void update(void Function(QuickEstimateListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuickEstimateListResponse build() => _build();

  _$QuickEstimateListResponse _build() {
    _$QuickEstimateListResponse _$result;
    try {
      _$result = _$v ??
          _$QuickEstimateListResponse._(
            items: _items?.build(),
            pagination: _pagination?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
        _$failedField = 'pagination';
        _pagination?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'QuickEstimateListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
