// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_preferences_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UserPreferencesDto extends UserPreferencesDto {
  @override
  final String? themeMode;
  @override
  final String? density;
  @override
  final int? textScale;

  factory _$UserPreferencesDto(
          [void Function(UserPreferencesDtoBuilder)? updates]) =>
      (UserPreferencesDtoBuilder()..update(updates))._build();

  _$UserPreferencesDto._({this.themeMode, this.density, this.textScale})
      : super._();
  @override
  UserPreferencesDto rebuild(
          void Function(UserPreferencesDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UserPreferencesDtoBuilder toBuilder() =>
      UserPreferencesDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UserPreferencesDto &&
        themeMode == other.themeMode &&
        density == other.density &&
        textScale == other.textScale;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, themeMode.hashCode);
    _$hash = $jc(_$hash, density.hashCode);
    _$hash = $jc(_$hash, textScale.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UserPreferencesDto')
          ..add('themeMode', themeMode)
          ..add('density', density)
          ..add('textScale', textScale))
        .toString();
  }
}

class UserPreferencesDtoBuilder
    implements Builder<UserPreferencesDto, UserPreferencesDtoBuilder> {
  _$UserPreferencesDto? _$v;

  String? _themeMode;
  String? get themeMode => _$this._themeMode;
  set themeMode(String? themeMode) => _$this._themeMode = themeMode;

  String? _density;
  String? get density => _$this._density;
  set density(String? density) => _$this._density = density;

  int? _textScale;
  int? get textScale => _$this._textScale;
  set textScale(int? textScale) => _$this._textScale = textScale;

  UserPreferencesDtoBuilder() {
    UserPreferencesDto._defaults(this);
  }

  UserPreferencesDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _themeMode = $v.themeMode;
      _density = $v.density;
      _textScale = $v.textScale;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UserPreferencesDto other) {
    _$v = other as _$UserPreferencesDto;
  }

  @override
  void update(void Function(UserPreferencesDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UserPreferencesDto build() => _build();

  _$UserPreferencesDto _build() {
    final _$result = _$v ??
        _$UserPreferencesDto._(
          themeMode: themeMode,
          density: density,
          textScale: textScale,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
