// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'opportunity_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OpportunityListResponse extends OpportunityListResponse {
  @override
  final BuiltList<OpportunityResponse>? items;
  @override
  final PaginationMetadataResponse? pagination;

  factory _$OpportunityListResponse(
          [void Function(OpportunityListResponseBuilder)? updates]) =>
      (OpportunityListResponseBuilder()..update(updates))._build();

  _$OpportunityListResponse._({this.items, this.pagination}) : super._();
  @override
  OpportunityListResponse rebuild(
          void Function(OpportunityListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OpportunityListResponseBuilder toBuilder() =>
      OpportunityListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OpportunityListResponse &&
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
    return (newBuiltValueToStringHelper(r'OpportunityListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class OpportunityListResponseBuilder
    implements
        Builder<OpportunityListResponse, OpportunityListResponseBuilder> {
  _$OpportunityListResponse? _$v;

  ListBuilder<OpportunityResponse>? _items;
  ListBuilder<OpportunityResponse> get items =>
      _$this._items ??= ListBuilder<OpportunityResponse>();
  set items(ListBuilder<OpportunityResponse>? items) => _$this._items = items;

  PaginationMetadataResponseBuilder? _pagination;
  PaginationMetadataResponseBuilder get pagination =>
      _$this._pagination ??= PaginationMetadataResponseBuilder();
  set pagination(PaginationMetadataResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  OpportunityListResponseBuilder() {
    OpportunityListResponse._defaults(this);
  }

  OpportunityListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OpportunityListResponse other) {
    _$v = other as _$OpportunityListResponse;
  }

  @override
  void update(void Function(OpportunityListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OpportunityListResponse build() => _build();

  _$OpportunityListResponse _build() {
    _$OpportunityListResponse _$result;
    try {
      _$result = _$v ??
          _$OpportunityListResponse._(
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
            r'OpportunityListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
