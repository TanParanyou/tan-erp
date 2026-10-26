// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_my_preferences_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateMyPreferencesRequest extends UpdateMyPreferencesRequest {
  @override
  final String? themeMode;
  @override
  final String? density;
  @override
  final int? textScale;
  @override
  final String? preferredLocale;

  factory _$UpdateMyPreferencesRequest(
          [void Function(UpdateMyPreferencesRequestBuilder)? updates]) =>
      (UpdateMyPreferencesRequestBuilder()..update(updates))._build();

  _$UpdateMyPreferencesRequest._(
      {this.themeMode, this.density, this.textScale, this.preferredLocale})
      : super._();
  @override
  UpdateMyPreferencesRequest rebuild(
          void Function(UpdateMyPreferencesRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateMyPreferencesRequestBuilder toBuilder() =>
      UpdateMyPreferencesRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateMyPreferencesRequest &&
        themeMode == other.themeMode &&
        density == other.density &&
        textScale == other.textScale &&
        preferredLocale == other.preferredLocale;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, themeMode.hashCode);
    _$hash = $jc(_$hash, density.hashCode);
    _$hash = $jc(_$hash, textScale.hashCode);
    _$hash = $jc(_$hash, preferredLocale.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateMyPreferencesRequest')
          ..add('themeMode', themeMode)
          ..add('density', density)
          ..add('textScale', textScale)
          ..add('preferredLocale', preferredLocale))
        .toString();
  }
}

class UpdateMyPreferencesRequestBuilder
    implements
        Builder<UpdateMyPreferencesRequest, UpdateMyPreferencesRequestBuilder> {
  _$UpdateMyPreferencesRequest? _$v;

  String? _themeMode;
  String? get themeMode => _$this._themeMode;
  set themeMode(String? themeMode) => _$this._themeMode = themeMode;

  String? _density;
  String? get density => _$this._density;
  set density(String? density) => _$this._density = density;

  int? _textScale;
  int? get textScale => _$this._textScale;
  set textScale(int? textScale) => _$this._textScale = textScale;

  String? _preferredLocale;
  String? get preferredLocale => _$this._preferredLocale;
  set preferredLocale(String? preferredLocale) =>
      _$this._preferredLocale = preferredLocale;

  UpdateMyPreferencesRequestBuilder() {
    UpdateMyPreferencesRequest._defaults(this);
  }

  UpdateMyPreferencesRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _themeMode = $v.themeMode;
      _density = $v.density;
      _textScale = $v.textScale;
      _preferredLocale = $v.preferredLocale;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateMyPreferencesRequest other) {
    _$v = other as _$UpdateMyPreferencesRequest;
  }

  @override
  void update(void Function(UpdateMyPreferencesRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateMyPreferencesRequest build() => _build();

  _$UpdateMyPreferencesRequest _build() {
    final _$result = _$v ??
        _$UpdateMyPreferencesRequest._(
          themeMode: themeMode,
          density: density,
          textScale: textScale,
          preferredLocale: preferredLocale,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
