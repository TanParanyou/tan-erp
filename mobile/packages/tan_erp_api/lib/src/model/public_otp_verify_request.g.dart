// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_otp_verify_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PublicOtpVerifyRequest extends PublicOtpVerifyRequest {
  @override
  final String? code;

  factory _$PublicOtpVerifyRequest(
          [void Function(PublicOtpVerifyRequestBuilder)? updates]) =>
      (PublicOtpVerifyRequestBuilder()..update(updates))._build();

  _$PublicOtpVerifyRequest._({this.code}) : super._();
  @override
  PublicOtpVerifyRequest rebuild(
          void Function(PublicOtpVerifyRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PublicOtpVerifyRequestBuilder toBuilder() =>
      PublicOtpVerifyRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PublicOtpVerifyRequest && code == other.code;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PublicOtpVerifyRequest')
          ..add('code', code))
        .toString();
  }
}

class PublicOtpVerifyRequestBuilder
    implements Builder<PublicOtpVerifyRequest, PublicOtpVerifyRequestBuilder> {
  _$PublicOtpVerifyRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  PublicOtpVerifyRequestBuilder() {
    PublicOtpVerifyRequest._defaults(this);
  }

  PublicOtpVerifyRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PublicOtpVerifyRequest other) {
    _$v = other as _$PublicOtpVerifyRequest;
  }

  @override
  void update(void Function(PublicOtpVerifyRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PublicOtpVerifyRequest build() => _build();

  _$PublicOtpVerifyRequest _build() {
    final _$result = _$v ??
        _$PublicOtpVerifyRequest._(
          code: code,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
