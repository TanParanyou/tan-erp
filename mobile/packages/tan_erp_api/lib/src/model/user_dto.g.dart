// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UserDto extends UserDto {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? email;
  @override
  final String? phoneNumber;
  @override
  final String? avatarFileId;
  @override
  final String? preferredLocale;
  @override
  final UserPreferencesDto? preferences;

  factory _$UserDto([void Function(UserDtoBuilder)? updates]) =>
      (UserDtoBuilder()..update(updates))._build();

  _$UserDto._(
      {this.id,
      this.displayName,
      this.email,
      this.phoneNumber,
      this.avatarFileId,
      this.preferredLocale,
      this.preferences})
      : super._();
  @override
  UserDto rebuild(void Function(UserDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UserDtoBuilder toBuilder() => UserDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UserDto &&
        id == other.id &&
        displayName == other.displayName &&
        email == other.email &&
        phoneNumber == other.phoneNumber &&
        avatarFileId == other.avatarFileId &&
        preferredLocale == other.preferredLocale &&
        preferences == other.preferences;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jc(_$hash, phoneNumber.hashCode);
    _$hash = $jc(_$hash, avatarFileId.hashCode);
    _$hash = $jc(_$hash, preferredLocale.hashCode);
    _$hash = $jc(_$hash, preferences.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UserDto')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('email', email)
          ..add('phoneNumber', phoneNumber)
          ..add('avatarFileId', avatarFileId)
          ..add('preferredLocale', preferredLocale)
          ..add('preferences', preferences))
        .toString();
  }
}

class UserDtoBuilder implements Builder<UserDto, UserDtoBuilder> {
  _$UserDto? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  String? _phoneNumber;
  String? get phoneNumber => _$this._phoneNumber;
  set phoneNumber(String? phoneNumber) => _$this._phoneNumber = phoneNumber;

  String? _avatarFileId;
  String? get avatarFileId => _$this._avatarFileId;
  set avatarFileId(String? avatarFileId) => _$this._avatarFileId = avatarFileId;

  String? _preferredLocale;
  String? get preferredLocale => _$this._preferredLocale;
  set preferredLocale(String? preferredLocale) =>
      _$this._preferredLocale = preferredLocale;

  UserPreferencesDtoBuilder? _preferences;
  UserPreferencesDtoBuilder get preferences =>
      _$this._preferences ??= UserPreferencesDtoBuilder();
  set preferences(UserPreferencesDtoBuilder? preferences) =>
      _$this._preferences = preferences;

  UserDtoBuilder() {
    UserDto._defaults(this);
  }

  UserDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _email = $v.email;
      _phoneNumber = $v.phoneNumber;
      _avatarFileId = $v.avatarFileId;
      _preferredLocale = $v.preferredLocale;
      _preferences = $v.preferences?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UserDto other) {
    _$v = other as _$UserDto;
  }

  @override
  void update(void Function(UserDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UserDto build() => _build();

  _$UserDto _build() {
    _$UserDto _$result;
    try {
      _$result = _$v ??
          _$UserDto._(
            id: id,
            displayName: displayName,
            email: email,
            phoneNumber: phoneNumber,
            avatarFileId: avatarFileId,
            preferredLocale: preferredLocale,
            preferences: _preferences?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'preferences';
        _preferences?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UserDto', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
