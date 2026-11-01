// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'auth_result_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AuthResultResponse extends AuthResultResponse {
  @override
  final String? status;
  @override
  final String? accessToken;
  @override
  final DateTime? expiresAtUtc;
  @override
  final String? refreshToken;
  @override
  final String? sessionId;
  @override
  final AuthUserSummaryDto? user;
  @override
  final String? challengeToken;
  @override
  final TotpSetupPayloadDto? totpSetup;
  @override
  final BuiltList<String>? backupCodes;

  factory _$AuthResultResponse(
          [void Function(AuthResultResponseBuilder)? updates]) =>
      (AuthResultResponseBuilder()..update(updates))._build();

  _$AuthResultResponse._(
      {this.status,
      this.accessToken,
      this.expiresAtUtc,
      this.refreshToken,
      this.sessionId,
      this.user,
      this.challengeToken,
      this.totpSetup,
      this.backupCodes})
      : super._();
  @override
  AuthResultResponse rebuild(
          void Function(AuthResultResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AuthResultResponseBuilder toBuilder() =>
      AuthResultResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AuthResultResponse &&
        status == other.status &&
        accessToken == other.accessToken &&
        expiresAtUtc == other.expiresAtUtc &&
        refreshToken == other.refreshToken &&
        sessionId == other.sessionId &&
        user == other.user &&
        challengeToken == other.challengeToken &&
        totpSetup == other.totpSetup &&
        backupCodes == other.backupCodes;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, accessToken.hashCode);
    _$hash = $jc(_$hash, expiresAtUtc.hashCode);
    _$hash = $jc(_$hash, refreshToken.hashCode);
    _$hash = $jc(_$hash, sessionId.hashCode);
    _$hash = $jc(_$hash, user.hashCode);
    _$hash = $jc(_$hash, challengeToken.hashCode);
    _$hash = $jc(_$hash, totpSetup.hashCode);
    _$hash = $jc(_$hash, backupCodes.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AuthResultResponse')
          ..add('status', status)
          ..add('accessToken', accessToken)
          ..add('expiresAtUtc', expiresAtUtc)
          ..add('refreshToken', refreshToken)
          ..add('sessionId', sessionId)
          ..add('user', user)
          ..add('challengeToken', challengeToken)
          ..add('totpSetup', totpSetup)
          ..add('backupCodes', backupCodes))
        .toString();
  }
}

class AuthResultResponseBuilder
    implements Builder<AuthResultResponse, AuthResultResponseBuilder> {
  _$AuthResultResponse? _$v;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _accessToken;
  String? get accessToken => _$this._accessToken;
  set accessToken(String? accessToken) => _$this._accessToken = accessToken;

  DateTime? _expiresAtUtc;
  DateTime? get expiresAtUtc => _$this._expiresAtUtc;
  set expiresAtUtc(DateTime? expiresAtUtc) =>
      _$this._expiresAtUtc = expiresAtUtc;

  String? _refreshToken;
  String? get refreshToken => _$this._refreshToken;
  set refreshToken(String? refreshToken) => _$this._refreshToken = refreshToken;

  String? _sessionId;
  String? get sessionId => _$this._sessionId;
  set sessionId(String? sessionId) => _$this._sessionId = sessionId;

  AuthUserSummaryDtoBuilder? _user;
  AuthUserSummaryDtoBuilder get user =>
      _$this._user ??= AuthUserSummaryDtoBuilder();
  set user(AuthUserSummaryDtoBuilder? user) => _$this._user = user;

  String? _challengeToken;
  String? get challengeToken => _$this._challengeToken;
  set challengeToken(String? challengeToken) =>
      _$this._challengeToken = challengeToken;

  TotpSetupPayloadDtoBuilder? _totpSetup;
  TotpSetupPayloadDtoBuilder get totpSetup =>
      _$this._totpSetup ??= TotpSetupPayloadDtoBuilder();
  set totpSetup(TotpSetupPayloadDtoBuilder? totpSetup) =>
      _$this._totpSetup = totpSetup;

  ListBuilder<String>? _backupCodes;
  ListBuilder<String> get backupCodes =>
      _$this._backupCodes ??= ListBuilder<String>();
  set backupCodes(ListBuilder<String>? backupCodes) =>
      _$this._backupCodes = backupCodes;

  AuthResultResponseBuilder() {
    AuthResultResponse._defaults(this);
  }

  AuthResultResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _status = $v.status;
      _accessToken = $v.accessToken;
      _expiresAtUtc = $v.expiresAtUtc;
      _refreshToken = $v.refreshToken;
      _sessionId = $v.sessionId;
      _user = $v.user?.toBuilder();
      _challengeToken = $v.challengeToken;
      _totpSetup = $v.totpSetup?.toBuilder();
      _backupCodes = $v.backupCodes?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AuthResultResponse other) {
    _$v = other as _$AuthResultResponse;
  }

  @override
  void update(void Function(AuthResultResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AuthResultResponse build() => _build();

  _$AuthResultResponse _build() {
    _$AuthResultResponse _$result;
    try {
      _$result = _$v ??
          _$AuthResultResponse._(
            status: status,
            accessToken: accessToken,
            expiresAtUtc: expiresAtUtc,
            refreshToken: refreshToken,
            sessionId: sessionId,
            user: _user?.build(),
            challengeToken: challengeToken,
            totpSetup: _totpSetup?.build(),
            backupCodes: _backupCodes?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'user';
        _user?.build();

        _$failedField = 'totpSetup';
        _totpSetup?.build();
        _$failedField = 'backupCodes';
        _backupCodes?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AuthResultResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
