// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'my_security_status_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MySecurityStatusResponse extends MySecurityStatusResponse {
  @override
  final bool? hasPassword;
  @override
  final bool? hasGoogleLinked;
  @override
  final bool? isTotpEnabled;
  @override
  final bool? requireTwoFactor;
  @override
  final int? remainingBackupCodesCount;
  @override
  final DateTime? totpEnabledAtUtc;
  @override
  final DateTime? passwordChangedAtUtc;

  factory _$MySecurityStatusResponse(
          [void Function(MySecurityStatusResponseBuilder)? updates]) =>
      (MySecurityStatusResponseBuilder()..update(updates))._build();

  _$MySecurityStatusResponse._(
      {this.hasPassword,
      this.hasGoogleLinked,
      this.isTotpEnabled,
      this.requireTwoFactor,
      this.remainingBackupCodesCount,
      this.totpEnabledAtUtc,
      this.passwordChangedAtUtc})
      : super._();
  @override
  MySecurityStatusResponse rebuild(
          void Function(MySecurityStatusResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MySecurityStatusResponseBuilder toBuilder() =>
      MySecurityStatusResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MySecurityStatusResponse &&
        hasPassword == other.hasPassword &&
        hasGoogleLinked == other.hasGoogleLinked &&
        isTotpEnabled == other.isTotpEnabled &&
        requireTwoFactor == other.requireTwoFactor &&
        remainingBackupCodesCount == other.remainingBackupCodesCount &&
        totpEnabledAtUtc == other.totpEnabledAtUtc &&
        passwordChangedAtUtc == other.passwordChangedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, hasPassword.hashCode);
    _$hash = $jc(_$hash, hasGoogleLinked.hashCode);
    _$hash = $jc(_$hash, isTotpEnabled.hashCode);
    _$hash = $jc(_$hash, requireTwoFactor.hashCode);
    _$hash = $jc(_$hash, remainingBackupCodesCount.hashCode);
    _$hash = $jc(_$hash, totpEnabledAtUtc.hashCode);
    _$hash = $jc(_$hash, passwordChangedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MySecurityStatusResponse')
          ..add('hasPassword', hasPassword)
          ..add('hasGoogleLinked', hasGoogleLinked)
          ..add('isTotpEnabled', isTotpEnabled)
          ..add('requireTwoFactor', requireTwoFactor)
          ..add('remainingBackupCodesCount', remainingBackupCodesCount)
          ..add('totpEnabledAtUtc', totpEnabledAtUtc)
          ..add('passwordChangedAtUtc', passwordChangedAtUtc))
        .toString();
  }
}

class MySecurityStatusResponseBuilder
    implements
        Builder<MySecurityStatusResponse, MySecurityStatusResponseBuilder> {
  _$MySecurityStatusResponse? _$v;

  bool? _hasPassword;
  bool? get hasPassword => _$this._hasPassword;
  set hasPassword(bool? hasPassword) => _$this._hasPassword = hasPassword;

  bool? _hasGoogleLinked;
  bool? get hasGoogleLinked => _$this._hasGoogleLinked;
  set hasGoogleLinked(bool? hasGoogleLinked) =>
      _$this._hasGoogleLinked = hasGoogleLinked;

  bool? _isTotpEnabled;
  bool? get isTotpEnabled => _$this._isTotpEnabled;
  set isTotpEnabled(bool? isTotpEnabled) =>
      _$this._isTotpEnabled = isTotpEnabled;

  bool? _requireTwoFactor;
  bool? get requireTwoFactor => _$this._requireTwoFactor;
  set requireTwoFactor(bool? requireTwoFactor) =>
      _$this._requireTwoFactor = requireTwoFactor;

  int? _remainingBackupCodesCount;
  int? get remainingBackupCodesCount => _$this._remainingBackupCodesCount;
  set remainingBackupCodesCount(int? remainingBackupCodesCount) =>
      _$this._remainingBackupCodesCount = remainingBackupCodesCount;

  DateTime? _totpEnabledAtUtc;
  DateTime? get totpEnabledAtUtc => _$this._totpEnabledAtUtc;
  set totpEnabledAtUtc(DateTime? totpEnabledAtUtc) =>
      _$this._totpEnabledAtUtc = totpEnabledAtUtc;

  DateTime? _passwordChangedAtUtc;
  DateTime? get passwordChangedAtUtc => _$this._passwordChangedAtUtc;
  set passwordChangedAtUtc(DateTime? passwordChangedAtUtc) =>
      _$this._passwordChangedAtUtc = passwordChangedAtUtc;

  MySecurityStatusResponseBuilder() {
    MySecurityStatusResponse._defaults(this);
  }

  MySecurityStatusResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _hasPassword = $v.hasPassword;
      _hasGoogleLinked = $v.hasGoogleLinked;
      _isTotpEnabled = $v.isTotpEnabled;
      _requireTwoFactor = $v.requireTwoFactor;
      _remainingBackupCodesCount = $v.remainingBackupCodesCount;
      _totpEnabledAtUtc = $v.totpEnabledAtUtc;
      _passwordChangedAtUtc = $v.passwordChangedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MySecurityStatusResponse other) {
    _$v = other as _$MySecurityStatusResponse;
  }

  @override
  void update(void Function(MySecurityStatusResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MySecurityStatusResponse build() => _build();

  _$MySecurityStatusResponse _build() {
    final _$result = _$v ??
        _$MySecurityStatusResponse._(
          hasPassword: hasPassword,
          hasGoogleLinked: hasGoogleLinked,
          isTotpEnabled: isTotpEnabled,
          requireTwoFactor: requireTwoFactor,
          remainingBackupCodesCount: remainingBackupCodesCount,
          totpEnabledAtUtc: totpEnabledAtUtc,
          passwordChangedAtUtc: passwordChangedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
