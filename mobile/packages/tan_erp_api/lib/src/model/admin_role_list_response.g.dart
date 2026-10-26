// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_role_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdminRoleListResponse extends AdminRoleListResponse {
  @override
  final BuiltList<AdminRoleResponse>? items;

  factory _$AdminRoleListResponse(
          [void Function(AdminRoleListResponseBuilder)? updates]) =>
      (AdminRoleListResponseBuilder()..update(updates))._build();

  _$AdminRoleListResponse._({this.items}) : super._();
  @override
  AdminRoleListResponse rebuild(
          void Function(AdminRoleListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdminRoleListResponseBuilder toBuilder() =>
      AdminRoleListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdminRoleListResponse && items == other.items;
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
    return (newBuiltValueToStringHelper(r'AdminRoleListResponse')
          ..add('items', items))
        .toString();
  }
}

class AdminRoleListResponseBuilder
    implements Builder<AdminRoleListResponse, AdminRoleListResponseBuilder> {
  _$AdminRoleListResponse? _$v;

  ListBuilder<AdminRoleResponse>? _items;
  ListBuilder<AdminRoleResponse> get items =>
      _$this._items ??= ListBuilder<AdminRoleResponse>();
  set items(ListBuilder<AdminRoleResponse>? items) => _$this._items = items;

  AdminRoleListResponseBuilder() {
    AdminRoleListResponse._defaults(this);
  }

  AdminRoleListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdminRoleListResponse other) {
    _$v = other as _$AdminRoleListResponse;
  }

  @override
  void update(void Function(AdminRoleListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdminRoleListResponse build() => _build();

  _$AdminRoleListResponse _build() {
    _$AdminRoleListResponse _$result;
    try {
      _$result = _$v ??
          _$AdminRoleListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AdminRoleListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
