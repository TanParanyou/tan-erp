// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'installations_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InstallationsListResponse extends InstallationsListResponse {
  @override
  final BuiltList<InstallationListItemResponse>? items;
  @override
  final ServicePaginationResponse? pagination;

  factory _$InstallationsListResponse(
          [void Function(InstallationsListResponseBuilder)? updates]) =>
      (InstallationsListResponseBuilder()..update(updates))._build();

  _$InstallationsListResponse._({this.items, this.pagination}) : super._();
  @override
  InstallationsListResponse rebuild(
          void Function(InstallationsListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InstallationsListResponseBuilder toBuilder() =>
      InstallationsListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InstallationsListResponse &&
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
    return (newBuiltValueToStringHelper(r'InstallationsListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class InstallationsListResponseBuilder
    implements
        Builder<InstallationsListResponse, InstallationsListResponseBuilder> {
  _$InstallationsListResponse? _$v;

  ListBuilder<InstallationListItemResponse>? _items;
  ListBuilder<InstallationListItemResponse> get items =>
      _$this._items ??= ListBuilder<InstallationListItemResponse>();
  set items(ListBuilder<InstallationListItemResponse>? items) =>
      _$this._items = items;

  ServicePaginationResponseBuilder? _pagination;
  ServicePaginationResponseBuilder get pagination =>
      _$this._pagination ??= ServicePaginationResponseBuilder();
  set pagination(ServicePaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  InstallationsListResponseBuilder() {
    InstallationsListResponse._defaults(this);
  }

  InstallationsListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(InstallationsListResponse other) {
    _$v = other as _$InstallationsListResponse;
  }

  @override
  void update(void Function(InstallationsListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InstallationsListResponse build() => _build();

  _$InstallationsListResponse _build() {
    _$InstallationsListResponse _$result;
    try {
      _$result = _$v ??
          _$InstallationsListResponse._(
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
            r'InstallationsListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
