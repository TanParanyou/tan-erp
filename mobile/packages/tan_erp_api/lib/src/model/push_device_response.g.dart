// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'push_device_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PushDeviceResponse extends PushDeviceResponse {
  @override
  final String? token;
  @override
  final String? platform;
  @override
  final DateTime? registeredAtUtc;

  factory _$PushDeviceResponse(
          [void Function(PushDeviceResponseBuilder)? updates]) =>
      (PushDeviceResponseBuilder()..update(updates))._build();

  _$PushDeviceResponse._({this.token, this.platform, this.registeredAtUtc})
      : super._();
  @override
  PushDeviceResponse rebuild(
          void Function(PushDeviceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PushDeviceResponseBuilder toBuilder() =>
      PushDeviceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PushDeviceResponse &&
        token == other.token &&
        platform == other.platform &&
        registeredAtUtc == other.registeredAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, token.hashCode);
    _$hash = $jc(_$hash, platform.hashCode);
    _$hash = $jc(_$hash, registeredAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PushDeviceResponse')
          ..add('token', token)
          ..add('platform', platform)
          ..add('registeredAtUtc', registeredAtUtc))
        .toString();
  }
}

class PushDeviceResponseBuilder
    implements Builder<PushDeviceResponse, PushDeviceResponseBuilder> {
  _$PushDeviceResponse? _$v;

  String? _token;
  String? get token => _$this._token;
  set token(String? token) => _$this._token = token;

  String? _platform;
  String? get platform => _$this._platform;
  set platform(String? platform) => _$this._platform = platform;

  DateTime? _registeredAtUtc;
  DateTime? get registeredAtUtc => _$this._registeredAtUtc;
  set registeredAtUtc(DateTime? registeredAtUtc) =>
      _$this._registeredAtUtc = registeredAtUtc;

  PushDeviceResponseBuilder() {
    PushDeviceResponse._defaults(this);
  }

  PushDeviceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _token = $v.token;
      _platform = $v.platform;
      _registeredAtUtc = $v.registeredAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PushDeviceResponse other) {
    _$v = other as _$PushDeviceResponse;
  }

  @override
  void update(void Function(PushDeviceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PushDeviceResponse build() => _build();

  _$PushDeviceResponse _build() {
    final _$result = _$v ??
        _$PushDeviceResponse._(
          token: token,
          platform: platform,
          registeredAtUtc: registeredAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
