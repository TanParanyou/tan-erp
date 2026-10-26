// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UserListItemResponse extends UserListItemResponse {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? email;
  @override
  final String? branchId;

  factory _$UserListItemResponse(
          [void Function(UserListItemResponseBuilder)? updates]) =>
      (UserListItemResponseBuilder()..update(updates))._build();

  _$UserListItemResponse._(
      {this.id, this.displayName, this.email, this.branchId})
      : super._();
  @override
  UserListItemResponse rebuild(
          void Function(UserListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UserListItemResponseBuilder toBuilder() =>
      UserListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UserListItemResponse &&
        id == other.id &&
        displayName == other.displayName &&
        email == other.email &&
        branchId == other.branchId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UserListItemResponse')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('email', email)
          ..add('branchId', branchId))
        .toString();
  }
}

class UserListItemResponseBuilder
    implements Builder<UserListItemResponse, UserListItemResponseBuilder> {
  _$UserListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  UserListItemResponseBuilder() {
    UserListItemResponse._defaults(this);
  }

  UserListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _email = $v.email;
      _branchId = $v.branchId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UserListItemResponse other) {
    _$v = other as _$UserListItemResponse;
  }

  @override
  void update(void Function(UserListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UserListItemResponse build() => _build();

  _$UserListItemResponse _build() {
    final _$result = _$v ??
        _$UserListItemResponse._(
          id: id,
          displayName: displayName,
          email: email,
          branchId: branchId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
