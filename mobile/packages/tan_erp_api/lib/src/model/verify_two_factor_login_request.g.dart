// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'verify_two_factor_login_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$VerifyTwoFactorLoginRequest extends VerifyTwoFactorLoginRequest {
  @override
  final String? challengeToken;
  @override
  final String? code;
  @override
  final String? clientPlatform;

  factory _$VerifyTwoFactorLoginRequest(
          [void Function(VerifyTwoFactorLoginRequestBuilder)? updates]) =>
      (VerifyTwoFactorLoginRequestBuilder()..update(updates))._build();

  _$VerifyTwoFactorLoginRequest._(
      {this.challengeToken, this.code, this.clientPlatform})
      : super._();
  @override
  VerifyTwoFactorLoginRequest rebuild(
          void Function(VerifyTwoFactorLoginRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  VerifyTwoFactorLoginRequestBuilder toBuilder() =>
      VerifyTwoFactorLoginRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is VerifyTwoFactorLoginRequest &&
        challengeToken == other.challengeToken &&
        code == other.code &&
        clientPlatform == other.clientPlatform;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, challengeToken.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, clientPlatform.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'VerifyTwoFactorLoginRequest')
          ..add('challengeToken', challengeToken)
          ..add('code', code)
          ..add('clientPlatform', clientPlatform))
        .toString();
  }
}

class VerifyTwoFactorLoginRequestBuilder
    implements
        Builder<VerifyTwoFactorLoginRequest,
            VerifyTwoFactorLoginRequestBuilder> {
  _$VerifyTwoFactorLoginRequest? _$v;

  String? _challengeToken;
  String? get challengeToken => _$this._challengeToken;
  set challengeToken(String? challengeToken) =>
      _$this._challengeToken = challengeToken;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _clientPlatform;
  String? get clientPlatform => _$this._clientPlatform;
  set clientPlatform(String? clientPlatform) =>
      _$this._clientPlatform = clientPlatform;

  VerifyTwoFactorLoginRequestBuilder() {
    VerifyTwoFactorLoginRequest._defaults(this);
  }

  VerifyTwoFactorLoginRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _challengeToken = $v.challengeToken;
      _code = $v.code;
      _clientPlatform = $v.clientPlatform;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(VerifyTwoFactorLoginRequest other) {
    _$v = other as _$VerifyTwoFactorLoginRequest;
  }

  @override
  void update(void Function(VerifyTwoFactorLoginRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  VerifyTwoFactorLoginRequest build() => _build();

  _$VerifyTwoFactorLoginRequest _build() {
    final _$result = _$v ??
        _$VerifyTwoFactorLoginRequest._(
          challengeToken: challengeToken,
          code: code,
          clientPlatform: clientPlatform,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
