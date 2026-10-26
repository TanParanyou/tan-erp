// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_user_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminUserListResponse extends AdminUserListResponse {
  @override
  final BuiltList<AdminUserResponse>? items;
  @override
  final AdminPaginationResponse? pagination;

  factory _$AdminUserListResponse(
          [void Function(AdminUserListResponseBuilder)? updates]) =>
      (AdminUserListResponseBuilder()..update(updates))._build();

  _$AdminUserListResponse._({this.items, this.pagination}) : super._();
  @override
  AdminUserListResponse rebuild(
          void Function(AdminUserListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminUserListResponseBuilder toBuilder() =>
      AdminUserListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminUserListResponse &&
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
    return (newBuiltValueToStringHelper(r'AdminUserListResponse')
          ..add('items', items)
          ..add('pagination', pagination))
        .toString();
  }
}

class AdminUserListResponseBuilder
    implements Builder<AdminUserListResponse, AdminUserListResponseBuilder> {
  _$AdminUserListResponse? _$v;

  ListBuilder<AdminUserResponse>? _items;
  ListBuilder<AdminUserResponse> get items =>
      _$this._items ??= ListBuilder<AdminUserResponse>();
  set items(ListBuilder<AdminUserResponse>? items) => _$this._items = items;

  AdminPaginationResponseBuilder? _pagination;
  AdminPaginationResponseBuilder get pagination =>
      _$this._pagination ??= AdminPaginationResponseBuilder();
  set pagination(AdminPaginationResponseBuilder? pagination) =>
      _$this._pagination = pagination;

  AdminUserListResponseBuilder() {
    AdminUserListResponse._defaults(this);
  }

  AdminUserListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _pagination = $v.pagination?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminUserListResponse other) {
    _$v = other as _$AdminUserListResponse;
  }

  @override
  void update(void Function(AdminUserListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminUserListResponse build() => _build();

  _$AdminUserListResponse _build() {
    _$AdminUserListResponse _$result;
    try {
      _$result = _$v ??
          _$AdminUserListResponse._(
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
            r'AdminUserListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
