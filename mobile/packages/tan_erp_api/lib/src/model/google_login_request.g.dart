// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'google_login_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$GoogleLoginRequest extends GoogleLoginRequest {
  @override
  final String? idToken;
  @override
  final String? clientPlatform;

  factory _$GoogleLoginRequest(
          [void Function(GoogleLoginRequestBuilder)? updates]) =>
      (GoogleLoginRequestBuilder()..update(updates))._build();

  _$GoogleLoginRequest._({this.idToken, this.clientPlatform}) : super._();
  @override
  GoogleLoginRequest rebuild(
          void Function(GoogleLoginRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  GoogleLoginRequestBuilder toBuilder() =>
      GoogleLoginRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is GoogleLoginRequest &&
        idToken == other.idToken &&
        clientPlatform == other.clientPlatform;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, idToken.hashCode);
    _$hash = $jc(_$hash, clientPlatform.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'GoogleLoginRequest')
          ..add('idToken', idToken)
          ..add('clientPlatform', clientPlatform))
        .toString();
  }
}

class GoogleLoginRequestBuilder
    implements Builder<GoogleLoginRequest, GoogleLoginRequestBuilder> {
  _$GoogleLoginRequest? _$v;

  String? _idToken;
  String? get idToken => _$this._idToken;
  set idToken(String? idToken) => _$this._idToken = idToken;

  String? _clientPlatform;
  String? get clientPlatform => _$this._clientPlatform;
  set clientPlatform(String? clientPlatform) =>
      _$this._clientPlatform = clientPlatform;

  GoogleLoginRequestBuilder() {
    GoogleLoginRequest._defaults(this);
  }

  GoogleLoginRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _idToken = $v.idToken;
      _clientPlatform = $v.clientPlatform;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(GoogleLoginRequest other) {
    _$v = other as _$GoogleLoginRequest;
  }

  @override
  void update(void Function(GoogleLoginRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  GoogleLoginRequest build() => _build();

  _$GoogleLoginRequest _build() {
    final _$result = _$v ??
        _$GoogleLoginRequest._(
          idToken: idToken,
          clientPlatform: clientPlatform,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
