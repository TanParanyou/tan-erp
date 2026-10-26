// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_otp_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PublicOtpRequest extends PublicOtpRequest {
  @override
  final String? channel;

  factory _$PublicOtpRequest(
          [void Function(PublicOtpRequestBuilder)? updates]) =>
      (PublicOtpRequestBuilder()..update(updates))._build();

  _$PublicOtpRequest._({this.channel}) : super._();
  @override
  PublicOtpRequest rebuild(void Function(PublicOtpRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PublicOtpRequestBuilder toBuilder() =>
      PublicOtpRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PublicOtpRequest && channel == other.channel;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, channel.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PublicOtpRequest')
          ..add('channel', channel))
        .toString();
  }
}

class PublicOtpRequestBuilder
    implements Builder<PublicOtpRequest, PublicOtpRequestBuilder> {
  _$PublicOtpRequest? _$v;

  String? _channel;
  String? get channel => _$this._channel;
  set channel(String? channel) => _$this._channel = channel;

  PublicOtpRequestBuilder() {
    PublicOtpRequest._defaults(this);
  }

  PublicOtpRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _channel = $v.channel;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PublicOtpRequest other) {
    _$v = other as _$PublicOtpRequest;
  }

  @override
  void update(void Function(PublicOtpRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PublicOtpRequest build() => _build();

  _$PublicOtpRequest _build() {
    final _$result = _$v ??
        _$PublicOtpRequest._(
          channel: channel,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
