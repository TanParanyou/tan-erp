// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UserListResponse extends UserListResponse {
  @override
  final BuiltList<UserListItemResponse>? items;
  @override
  final int? totalCount;

  factory _$UserListResponse(
          [void Function(UserListResponseBuilder)? updates]) =>
      (UserListResponseBuilder()..update(updates))._build();

  _$UserListResponse._({this.items, this.totalCount}) : super._();
  @override
  UserListResponse rebuild(void Function(UserListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UserListResponseBuilder toBuilder() =>
      UserListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UserListResponse &&
        items == other.items &&
        totalCount == other.totalCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jc(_$hash, totalCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UserListResponse')
          ..add('items', items)
          ..add('totalCount', totalCount))
        .toString();
  }
}

class UserListResponseBuilder
    implements Builder<UserListResponse, UserListResponseBuilder> {
  _$UserListResponse? _$v;

  ListBuilder<UserListItemResponse>? _items;
  ListBuilder<UserListItemResponse> get items =>
      _$this._items ??= ListBuilder<UserListItemResponse>();
  set items(ListBuilder<UserListItemResponse>? items) => _$this._items = items;

  int? _totalCount;
  int? get totalCount => _$this._totalCount;
  set totalCount(int? totalCount) => _$this._totalCount = totalCount;

  UserListResponseBuilder() {
    UserListResponse._defaults(this);
  }

  UserListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _totalCount = $v.totalCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UserListResponse other) {
    _$v = other as _$UserListResponse;
  }

  @override
  void update(void Function(UserListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UserListResponse build() => _build();

  _$UserListResponse _build() {
    _$UserListResponse _$result;
    try {
      _$result = _$v ??
          _$UserListResponse._(
            items: _items?.build(),
            totalCount: totalCount,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UserListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
