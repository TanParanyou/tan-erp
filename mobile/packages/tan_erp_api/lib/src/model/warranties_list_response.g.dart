// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'warranties_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WarrantiesListResponse extends WarrantiesListResponse {
  @override
  final BuiltList<WarrantyResponse>? items;
  @override
  final ServicePaginationResponse? pagination;

  factory _$WarrantiesListResponse(
          [void Function(WarrantiesListResponseBuilder)? updates]) =>
      (WarrantiesListResponseBuilder()..update(updates))._build();

  _$WarrantiesListResponse._({this.items, this.pagination}) : super._();
  @override
  WarrantiesListResponse rebuild(
          void Function(WarrantiesListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WarrantiesListResponseBuilder toBuilder() =>
      WarrantiesListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WarrantiesListResponse &&
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
    return (newBuiltValueToStringHelper(r'WarrantiesListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class WarrantiesListResponseBuilder
    implements Builder<WarrantiesListResponse, WarrantiesListResponseBuilder> {
  _$WarrantiesListResponse? _$v;

  ListBuilder<WarrantyResponse>? _items;
  ListBuilder<WarrantyResponse> get items =>
      _$this._items ??= ListBuilder<WarrantyResponse>();
  set items(ListBuilder<WarrantyResponse>? items) => _$this._items = items;

  ServicePaginationResponseBuilder? _pagination;
  ServicePaginationResponseBuilder get pagination =>
      _$this._pagination ??= ServicePaginationResponseBuilder();
  set pagination(ServicePaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  WarrantiesListResponseBuilder() {
    WarrantiesListResponse._defaults(this);
  }

  WarrantiesListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WarrantiesListResponse other) {
    _$v = other as _$WarrantiesListResponse;
  }

  @override
  void update(void Function(WarrantiesListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WarrantiesListResponse build() => _build();

  _$WarrantiesListResponse _build() {
    _$WarrantiesListResponse _$result;
    try {
      _$result = _$v ??
          _$WarrantiesListResponse._(
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
            r'WarrantiesListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
