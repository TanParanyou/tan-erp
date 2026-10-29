// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'register_push_device_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RegisterPushDeviceRequest extends RegisterPushDeviceRequest {
  @override
  final String? token;
  @override
  final String? platform;

  factory _$RegisterPushDeviceRequest(
          [void Function(RegisterPushDeviceRequestBuilder)? updates]) =>
      (RegisterPushDeviceRequestBuilder()..update(updates))._build();

  _$RegisterPushDeviceRequest._({this.token, this.platform}) : super._();
  @override
  RegisterPushDeviceRequest rebuild(
          void Function(RegisterPushDeviceRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RegisterPushDeviceRequestBuilder toBuilder() =>
      RegisterPushDeviceRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RegisterPushDeviceRequest &&
        token == other.token &&
        platform == other.platform;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, token.hashCode);
    _$hash = $jc(_$hash, platform.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RegisterPushDeviceRequest')
          ..add('token', token)
          ..add('platform', platform))
        .toString();
  }
}

class RegisterPushDeviceRequestBuilder
    implements
        Builder<RegisterPushDeviceRequest, RegisterPushDeviceRequestBuilder> {
  _$RegisterPushDeviceRequest? _$v;

  String? _token;
  String? get token => _$this._token;
  set token(String? token) => _$this._token = token;

  String? _platform;
  String? get platform => _$this._platform;
  set platform(String? platform) => _$this._platform = platform;

  RegisterPushDeviceRequestBuilder() {
    RegisterPushDeviceRequest._defaults(this);
  }

  RegisterPushDeviceRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _token = $v.token;
      _platform = $v.platform;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RegisterPushDeviceRequest other) {
    _$v = other as _$RegisterPushDeviceRequest;
  }

  @override
  void update(void Function(RegisterPushDeviceRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RegisterPushDeviceRequest build() => _build();

  _$RegisterPushDeviceRequest _build() {
    final _$result = _$v ??
        _$RegisterPushDeviceRequest._(
          token: token,
          platform: platform,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
