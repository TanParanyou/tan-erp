// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_my_profile_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateMyProfileRequest extends UpdateMyProfileRequest {
  @override
  final String? displayName;
  @override
  final String? phoneNumber;
  @override
  final String? preferredLocale;
  @override
  final String? avatarFileId;

  factory _$UpdateMyProfileRequest(
          [void Function(UpdateMyProfileRequestBuilder)? updates]) =>
      (UpdateMyProfileRequestBuilder()..update(updates))._build();

  _$UpdateMyProfileRequest._(
      {this.displayName,
      this.phoneNumber,
      this.preferredLocale,
      this.avatarFileId})
      : super._();
  @override
  UpdateMyProfileRequest rebuild(
          void Function(UpdateMyProfileRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateMyProfileRequestBuilder toBuilder() =>
      UpdateMyProfileRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateMyProfileRequest &&
        displayName == other.displayName &&
        phoneNumber == other.phoneNumber &&
        preferredLocale == other.preferredLocale &&
        avatarFileId == other.avatarFileId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, phoneNumber.hashCode);
    _$hash = $jc(_$hash, preferredLocale.hashCode);
    _$hash = $jc(_$hash, avatarFileId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateMyProfileRequest')
          ..add('displayName', displayName)
          ..add('phoneNumber', phoneNumber)
          ..add('preferredLocale', preferredLocale)
          ..add('avatarFileId', avatarFileId))
        .toString();
  }
}

class UpdateMyProfileRequestBuilder
    implements Builder<UpdateMyProfileRequest, UpdateMyProfileRequestBuilder> {
  _$UpdateMyProfileRequest? _$v;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _phoneNumber;
  String? get phoneNumber => _$this._phoneNumber;
  set phoneNumber(String? phoneNumber) => _$this._phoneNumber = phoneNumber;

  String? _preferredLocale;
  String? get preferredLocale => _$this._preferredLocale;
  set preferredLocale(String? preferredLocale) =>
      _$this._preferredLocale = preferredLocale;

  String? _avatarFileId;
  String? get avatarFileId => _$this._avatarFileId;
  set avatarFileId(String? avatarFileId) => _$this._avatarFileId = avatarFileId;

  UpdateMyProfileRequestBuilder() {
    UpdateMyProfileRequest._defaults(this);
  }

  UpdateMyProfileRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _displayName = $v.displayName;
      _phoneNumber = $v.phoneNumber;
      _preferredLocale = $v.preferredLocale;
      _avatarFileId = $v.avatarFileId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateMyProfileRequest other) {
    _$v = other as _$UpdateMyProfileRequest;
  }

  @override
  void update(void Function(UpdateMyProfileRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateMyProfileRequest build() => _build();

  _$UpdateMyProfileRequest _build() {
    final _$result = _$v ??
        _$UpdateMyProfileRequest._(
          displayName: displayName,
          phoneNumber: phoneNumber,
          preferredLocale: preferredLocale,
          avatarFileId: avatarFileId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
