// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'app_version_policy_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AppVersionPolicyResponse extends AppVersionPolicyResponse {
  @override
  final String? minimumVersion;
  @override
  final String? latestVersion;

  factory _$AppVersionPolicyResponse(
          [void Function(AppVersionPolicyResponseBuilder)? updates]) =>
      (AppVersionPolicyResponseBuilder()..update(updates))._build();

  _$AppVersionPolicyResponse._({this.minimumVersion, this.latestVersion})
      : super._();
  @override
  AppVersionPolicyResponse rebuild(
          void Function(AppVersionPolicyResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AppVersionPolicyResponseBuilder toBuilder() =>
      AppVersionPolicyResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AppVersionPolicyResponse &&
        minimumVersion == other.minimumVersion &&
        latestVersion == other.latestVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, minimumVersion.hashCode);
    _$hash = $jc(_$hash, latestVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AppVersionPolicyResponse')
          ..add('minimumVersion', minimumVersion)
          ..add('latestVersion', latestVersion))
        .toString();
  }
}

class AppVersionPolicyResponseBuilder
    implements
        Builder<AppVersionPolicyResponse, AppVersionPolicyResponseBuilder> {
  _$AppVersionPolicyResponse? _$v;

  String? _minimumVersion;
  String? get minimumVersion => _$this._minimumVersion;
  set minimumVersion(String? minimumVersion) =>
      _$this._minimumVersion = minimumVersion;

  String? _latestVersion;
  String? get latestVersion => _$this._latestVersion;
  set latestVersion(String? latestVersion) =>
      _$this._latestVersion = latestVersion;

  AppVersionPolicyResponseBuilder() {
    AppVersionPolicyResponse._defaults(this);
  }

  AppVersionPolicyResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _minimumVersion = $v.minimumVersion;
      _latestVersion = $v.latestVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AppVersionPolicyResponse other) {
    _$v = other as _$AppVersionPolicyResponse;
  }

  @override
  void update(void Function(AppVersionPolicyResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AppVersionPolicyResponse build() => _build();

  _$AppVersionPolicyResponse _build() {
    final _$result = _$v ??
        _$AppVersionPolicyResponse._(
          minimumVersion: minimumVersion,
          latestVersion: latestVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
