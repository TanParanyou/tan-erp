// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_otp_request_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PublicOtpRequestResponse extends PublicOtpRequestResponse {
  @override
  final int? resendAfterSeconds;
  @override
  final int? expiresInSeconds;

  factory _$PublicOtpRequestResponse(
          [void Function(PublicOtpRequestResponseBuilder)? updates]) =>
      (PublicOtpRequestResponseBuilder()..update(updates))._build();

  _$PublicOtpRequestResponse._({this.resendAfterSeconds, this.expiresInSeconds})
      : super._();
  @override
  PublicOtpRequestResponse rebuild(
          void Function(PublicOtpRequestResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PublicOtpRequestResponseBuilder toBuilder() =>
      PublicOtpRequestResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PublicOtpRequestResponse &&
        resendAfterSeconds == other.resendAfterSeconds &&
        expiresInSeconds == other.expiresInSeconds;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, resendAfterSeconds.hashCode);
    _$hash = $jc(_$hash, expiresInSeconds.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PublicOtpRequestResponse')
          ..add('resendAfterSeconds', resendAfterSeconds)
          ..add('expiresInSeconds', expiresInSeconds))
        .toString();
  }
}

class PublicOtpRequestResponseBuilder
    implements
        Builder<PublicOtpRequestResponse, PublicOtpRequestResponseBuilder> {
  _$PublicOtpRequestResponse? _$v;

  int? _resendAfterSeconds;
  int? get resendAfterSeconds => _$this._resendAfterSeconds;
  set resendAfterSeconds(int? resendAfterSeconds) =>
      _$this._resendAfterSeconds = resendAfterSeconds;

  int? _expiresInSeconds;
  int? get expiresInSeconds => _$this._expiresInSeconds;
  set expiresInSeconds(int? expiresInSeconds) =>
      _$this._expiresInSeconds = expiresInSeconds;

  PublicOtpRequestResponseBuilder() {
    PublicOtpRequestResponse._defaults(this);
  }

  PublicOtpRequestResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _resendAfterSeconds = $v.resendAfterSeconds;
      _expiresInSeconds = $v.expiresInSeconds;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PublicOtpRequestResponse other) {
    _$v = other as _$PublicOtpRequestResponse;
  }

  @override
  void update(void Function(PublicOtpRequestResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PublicOtpRequestResponse build() => _build();

  _$PublicOtpRequestResponse _build() {
    final _$result = _$v ??
        _$PublicOtpRequestResponse._(
          resendAfterSeconds: resendAfterSeconds,
          expiresInSeconds: expiresInSeconds,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
