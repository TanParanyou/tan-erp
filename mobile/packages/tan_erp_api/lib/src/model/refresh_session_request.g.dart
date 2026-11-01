// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'refresh_session_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RefreshSessionRequest extends RefreshSessionRequest {
  @override
  final String? refreshToken;
  @override
  final String? clientPlatform;

  factory _$RefreshSessionRequest(
          [void Function(RefreshSessionRequestBuilder)? updates]) =>
      (RefreshSessionRequestBuilder()..update(updates))._build();

  _$RefreshSessionRequest._({this.refreshToken, this.clientPlatform})
      : super._();
  @override
  RefreshSessionRequest rebuild(
          void Function(RefreshSessionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RefreshSessionRequestBuilder toBuilder() =>
      RefreshSessionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RefreshSessionRequest &&
        refreshToken == other.refreshToken &&
        clientPlatform == other.clientPlatform;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, refreshToken.hashCode);
    _$hash = $jc(_$hash, clientPlatform.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RefreshSessionRequest')
          ..add('refreshToken', refreshToken)
          ..add('clientPlatform', clientPlatform))
        .toString();
  }
}

class RefreshSessionRequestBuilder
    implements Builder<RefreshSessionRequest, RefreshSessionRequestBuilder> {
  _$RefreshSessionRequest? _$v;

  String? _refreshToken;
  String? get refreshToken => _$this._refreshToken;
  set refreshToken(String? refreshToken) => _$this._refreshToken = refreshToken;

  String? _clientPlatform;
  String? get clientPlatform => _$this._clientPlatform;
  set clientPlatform(String? clientPlatform) =>
      _$this._clientPlatform = clientPlatform;

  RefreshSessionRequestBuilder() {
    RefreshSessionRequest._defaults(this);
  }

  RefreshSessionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _refreshToken = $v.refreshToken;
      _clientPlatform = $v.clientPlatform;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RefreshSessionRequest other) {
    _$v = other as _$RefreshSessionRequest;
  }

  @override
  void update(void Function(RefreshSessionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RefreshSessionRequest build() => _build();

  _$RefreshSessionRequest _build() {
    final _$result = _$v ??
        _$RefreshSessionRequest._(
          refreshToken: refreshToken,
          clientPlatform: clientPlatform,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
