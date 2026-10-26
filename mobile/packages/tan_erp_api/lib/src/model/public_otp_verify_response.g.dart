// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_otp_verify_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PublicOtpVerifyResponse extends PublicOtpVerifyResponse {
  @override
  final String? verificationToken;
  @override
  final DateTime? expiresAtUtc;

  factory _$PublicOtpVerifyResponse(
          [void Function(PublicOtpVerifyResponseBuilder)? updates]) =>
      (PublicOtpVerifyResponseBuilder()..update(updates))._build();

  _$PublicOtpVerifyResponse._({this.verificationToken, this.expiresAtUtc})
      : super._();
  @override
  PublicOtpVerifyResponse rebuild(
          void Function(PublicOtpVerifyResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PublicOtpVerifyResponseBuilder toBuilder() =>
      PublicOtpVerifyResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PublicOtpVerifyResponse &&
        verificationToken == other.verificationToken &&
        expiresAtUtc == other.expiresAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, verificationToken.hashCode);
    _$hash = $jc(_$hash, expiresAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PublicOtpVerifyResponse')
          ..add('verificationToken', verificationToken)
          ..add('expiresAtUtc', expiresAtUtc))
        .toString();
  }
}

class PublicOtpVerifyResponseBuilder
    implements
        Builder<PublicOtpVerifyResponse, PublicOtpVerifyResponseBuilder> {
  _$PublicOtpVerifyResponse? _$v;

  String? _verificationToken;
  String? get verificationToken => _$this._verificationToken;
  set verificationToken(String? verificationToken) =>
      _$this._verificationToken = verificationToken;

  DateTime? _expiresAtUtc;
  DateTime? get expiresAtUtc => _$this._expiresAtUtc;
  set expiresAtUtc(DateTime? expiresAtUtc) =>
      _$this._expiresAtUtc = expiresAtUtc;

  PublicOtpVerifyResponseBuilder() {
    PublicOtpVerifyResponse._defaults(this);
  }

  PublicOtpVerifyResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _verificationToken = $v.verificationToken;
      _expiresAtUtc = $v.expiresAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PublicOtpVerifyResponse other) {
    _$v = other as _$PublicOtpVerifyResponse;
  }

  @override
  void update(void Function(PublicOtpVerifyResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PublicOtpVerifyResponse build() => _build();

  _$PublicOtpVerifyResponse _build() {
    final _$result = _$v ??
        _$PublicOtpVerifyResponse._(
          verificationToken: verificationToken,
          expiresAtUtc: expiresAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
