// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'role_definition_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RoleDefinitionListResponse extends RoleDefinitionListResponse {
  @override
  final BuiltList<RoleDefinitionSummaryResponse>? items;
  @override
  final AdminPaginationResponse? pagination;

  factory _$RoleDefinitionListResponse(
          [void Function(RoleDefinitionListResponseBuilder)? updates]) =>
      (RoleDefinitionListResponseBuilder()..update(updates))._build();

  _$RoleDefinitionListResponse._({this.items, this.pagination}) : super._();
  @override
  RoleDefinitionListResponse rebuild(
          void Function(RoleDefinitionListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RoleDefinitionListResponseBuilder toBuilder() =>
      RoleDefinitionListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RoleDefinitionListResponse &&
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
    return (newBuiltValueToStringHelper(r'RoleDefinitionListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class RoleDefinitionListResponseBuilder
    implements
        Builder<RoleDefinitionListResponse, RoleDefinitionListResponseBuilder> {
  _$RoleDefinitionListResponse? _$v;

  ListBuilder<RoleDefinitionSummaryResponse>? _items;
  ListBuilder<RoleDefinitionSummaryResponse> get items =>
      _$this._items ??= ListBuilder<RoleDefinitionSummaryResponse>();
  set items(ListBuilder<RoleDefinitionSummaryResponse>? items) =>
      _$this._items = items;

  AdminPaginationResponseBuilder? _pagination;
  AdminPaginationResponseBuilder get pagination =>
      _$this._pagination ??= AdminPaginationResponseBuilder();
  set pagination(AdminPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  RoleDefinitionListResponseBuilder() {
    RoleDefinitionListResponse._defaults(this);
  }

  RoleDefinitionListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RoleDefinitionListResponse other) {
    _$v = other as _$RoleDefinitionListResponse;
  }

  @override
  void update(void Function(RoleDefinitionListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RoleDefinitionListResponse build() => _build();

  _$RoleDefinitionListResponse _build() {
    _$RoleDefinitionListResponse _$result;
    try {
      _$result = _$v ??
          _$RoleDefinitionListResponse._(
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
            r'RoleDefinitionListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
