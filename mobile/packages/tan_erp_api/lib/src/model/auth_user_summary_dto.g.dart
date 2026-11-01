// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'auth_user_summary_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AuthUserSummaryDto extends AuthUserSummaryDto {
  @override
  final String? id;
  @override
  final String? uid;
  @override
  final String? email;
  @override
  final String? displayName;

  factory _$AuthUserSummaryDto(
          [void Function(AuthUserSummaryDtoBuilder)? updates]) =>
      (AuthUserSummaryDtoBuilder()..update(updates))._build();

  _$AuthUserSummaryDto._({this.id, this.uid, this.email, this.displayName})
      : super._();
  @override
  AuthUserSummaryDto rebuild(
          void Function(AuthUserSummaryDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AuthUserSummaryDtoBuilder toBuilder() =>
      AuthUserSummaryDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AuthUserSummaryDto &&
        id == other.id &&
        uid == other.uid &&
        email == other.email &&
        displayName == other.displayName;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, uid.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AuthUserSummaryDto')
          ..add('id', id)
          ..add('uid', uid)
          ..add('email', email)
          ..add('displayName', displayName))
        .toString();
  }
}

class AuthUserSummaryDtoBuilder
    implements Builder<AuthUserSummaryDto, AuthUserSummaryDtoBuilder> {
  _$AuthUserSummaryDto? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _uid;
  String? get uid => _$this._uid;
  set uid(String? uid) => _$this._uid = uid;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  AuthUserSummaryDtoBuilder() {
    AuthUserSummaryDto._defaults(this);
  }

  AuthUserSummaryDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _uid = $v.uid;
      _email = $v.email;
      _displayName = $v.displayName;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AuthUserSummaryDto other) {
    _$v = other as _$AuthUserSummaryDto;
  }

  @override
  void update(void Function(AuthUserSummaryDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AuthUserSummaryDto build() => _build();

  _$AuthUserSummaryDto _build() {
    final _$result = _$v ??
        _$AuthUserSummaryDto._(
          id: id,
          uid: uid,
          email: email,
          displayName: displayName,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
