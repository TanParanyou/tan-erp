// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'totp_setup_payload_dto.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$TotpSetupPayloadDto extends TotpSetupPayloadDto {
  @override
  final String? secretKey;
  @override
  final String? otpAuthUri;

  factory _$TotpSetupPayloadDto(
          [void Function(TotpSetupPayloadDtoBuilder)? updates]) =>
      (TotpSetupPayloadDtoBuilder()..update(updates))._build();

  _$TotpSetupPayloadDto._({this.secretKey, this.otpAuthUri}) : super._();
  @override
  TotpSetupPayloadDto rebuild(
          void Function(TotpSetupPayloadDtoBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  TotpSetupPayloadDtoBuilder toBuilder() =>
      TotpSetupPayloadDtoBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is TotpSetupPayloadDto &&
        secretKey == other.secretKey &&
        otpAuthUri == other.otpAuthUri;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, secretKey.hashCode);
    _$hash = $jc(_$hash, otpAuthUri.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'TotpSetupPayloadDto')
          ..add('secretKey', secretKey)
          ..add('otpAuthUri', otpAuthUri))
        .toString();
  }
}

class TotpSetupPayloadDtoBuilder
    implements Builder<TotpSetupPayloadDto, TotpSetupPayloadDtoBuilder> {
  _$TotpSetupPayloadDto? _$v;

  String? _secretKey;
  String? get secretKey => _$this._secretKey;
  set secretKey(String? secretKey) => _$this._secretKey = secretKey;

  String? _otpAuthUri;
  String? get otpAuthUri => _$this._otpAuthUri;
  set otpAuthUri(String? otpAuthUri) => _$this._otpAuthUri = otpAuthUri;

  TotpSetupPayloadDtoBuilder() {
    TotpSetupPayloadDto._defaults(this);
  }

  TotpSetupPayloadDtoBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _secretKey = $v.secretKey;
      _otpAuthUri = $v.otpAuthUri;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(TotpSetupPayloadDto other) {
    _$v = other as _$TotpSetupPayloadDto;
  }

  @override
  void update(void Function(TotpSetupPayloadDtoBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  TotpSetupPayloadDto build() => _build();

  _$TotpSetupPayloadDto _build() {
    final _$result = _$v ??
        _$TotpSetupPayloadDto._(
          secretKey: secretKey,
          otpAuthUri: otpAuthUri,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
