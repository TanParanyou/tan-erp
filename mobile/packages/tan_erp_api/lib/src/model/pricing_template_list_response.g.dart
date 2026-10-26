// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pricing_template_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PricingTemplateListResponse extends PricingTemplateListResponse {
  @override
  final BuiltList<PricingTemplateListItemResponse>? items;
  @override
  final QuickEstimatePaginationResponse? pagination;

  factory _$PricingTemplateListResponse(
          [void Function(PricingTemplateListResponseBuilder)? updates]) =>
      (PricingTemplateListResponseBuilder()..update(updates))._build();

  _$PricingTemplateListResponse._({this.items, this.pagination}) : super._();
  @override
  PricingTemplateListResponse rebuild(
          void Function(PricingTemplateListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PricingTemplateListResponseBuilder toBuilder() =>
      PricingTemplateListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PricingTemplateListResponse &&
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
    return (newBuiltValueToStringHelper(r'PricingTemplateListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class PricingTemplateListResponseBuilder
    implements
        Builder<PricingTemplateListResponse,
            PricingTemplateListResponseBuilder> {
  _$PricingTemplateListResponse? _$v;

  ListBuilder<PricingTemplateListItemResponse>? _items;
  ListBuilder<PricingTemplateListItemResponse> get items =>
      _$this._items ??= ListBuilder<PricingTemplateListItemResponse>();
  set items(ListBuilder<PricingTemplateListItemResponse>? items) =>
      _$this._items = items;

  QuickEstimatePaginationResponseBuilder? _pagination;
  QuickEstimatePaginationResponseBuilder get pagination =>
      _$this._pagination ??= QuickEstimatePaginationResponseBuilder();
  set pagination(QuickEstimatePaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  PricingTemplateListResponseBuilder() {
    PricingTemplateListResponse._defaults(this);
  }

  PricingTemplateListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PricingTemplateListResponse other) {
    _$v = other as _$PricingTemplateListResponse;
  }

  @override
  void update(void Function(PricingTemplateListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PricingTemplateListResponse build() => _build();

  _$PricingTemplateListResponse _build() {
    _$PricingTemplateListResponse _$result;
    try {
      _$result = _$v ??
          _$PricingTemplateListResponse._(
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
            r'PricingTemplateListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
