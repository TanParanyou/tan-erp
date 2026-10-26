// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'current_user_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CurrentUserResponse extends CurrentUserResponse {
  @override
  final UserDto? user;
  @override
  final BuiltList<MembershipDto>? memberships;

  factory _$CurrentUserResponse(
          [void Function(CurrentUserResponseBuilder)? updates]) =>
      (CurrentUserResponseBuilder()..update(updates))._build();

  _$CurrentUserResponse._({this.user, this.memberships}) : super._();
  @override
  CurrentUserResponse rebuild(
          void Function(CurrentUserResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CurrentUserResponseBuilder toBuilder() =>
      CurrentUserResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CurrentUserResponse &&
        user == other.user &&
        memberships == other.memberships;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, user.hashCode);
    _$hash = $jc(_$hash, memberships.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CurrentUserResponse')
          ..add('user', user)
          ..add('memberships', memberships))
        .toString();
  }
}

class CurrentUserResponseBuilder
    implements Builder<CurrentUserResponse, CurrentUserResponseBuilder> {
  _$CurrentUserResponse? _$v;

  UserDtoBuilder? _user;
  UserDtoBuilder get user => _$this._user ??= UserDtoBuilder();
  set user(UserDtoBuilder? user) => _$this._user = user;

  ListBuilder<MembershipDto>? _memberships;
  ListBuilder<MembershipDto> get memberships =>
      _$this._memberships ??= ListBuilder<MembershipDto>();
  set memberships(ListBuilder<MembershipDto>? memberships) =>
      _$this._memberships = memberships;

  CurrentUserResponseBuilder() {
    CurrentUserResponse._defaults(this);
  }

  CurrentUserResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _user = $v.user?.toBuilder();
      _memberships = $v.memberships?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CurrentUserResponse other) {
    _$v = other as _$CurrentUserResponse;
  }

  @override
  void update(void Function(CurrentUserResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CurrentUserResponse build() => _build();

  _$CurrentUserResponse _build() {
    _$CurrentUserResponse _$result;
    try {
      _$result = _$v ??
          _$CurrentUserResponse._(
            user: _user?.build(),
            memberships: _memberships?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'user';
        _user?.build();
        _$failedField = 'memberships';
        _memberships?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CurrentUserResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
