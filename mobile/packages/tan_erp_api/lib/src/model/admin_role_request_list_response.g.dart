// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_role_request_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminRoleRequestListResponse extends AdminRoleRequestListResponse {
  @override
  final BuiltList<AdminRoleRequestResponse>? items;

  factory _$AdminRoleRequestListResponse(
          [void Function(AdminRoleRequestListResponseBuilder)? updates]) =>
      (AdminRoleRequestListResponseBuilder()..update(updates))._build();

  _$AdminRoleRequestListResponse._({this.items}) : super._();
  @override
  AdminRoleRequestListResponse rebuild(
          void Function(AdminRoleRequestListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminRoleRequestListResponseBuilder toBuilder() =>
      AdminRoleRequestListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminRoleRequestListResponse && items == other.items;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdminRoleRequestListResponse')
          ..add('items', items))
        .toString();
  }
}

class AdminRoleRequestListResponseBuilder
    implements
        Builder<AdminRoleRequestListResponse,
            AdminRoleRequestListResponseBuilder> {
  _$AdminRoleRequestListResponse? _$v;

  ListBuilder<AdminRoleRequestResponse>? _items;
  ListBuilder<AdminRoleRequestResponse> get items =>
      _$this._items ??= ListBuilder<AdminRoleRequestResponse>();
  set items(ListBuilder<AdminRoleRequestResponse>? items) =>
      _$this._items = items;

  AdminRoleRequestListResponseBuilder() {
    AdminRoleRequestListResponse._defaults(this);
  }

  AdminRoleRequestListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminRoleRequestListResponse other) {
    _$v = other as _$AdminRoleRequestListResponse;
  }

  @override
  void update(void Function(AdminRoleRequestListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminRoleRequestListResponse build() => _build();

  _$AdminRoleRequestListResponse _build() {
    _$AdminRoleRequestListResponse _$result;
    try {
      _$result = _$v ??
          _$AdminRoleRequestListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AdminRoleRequestListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
