// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'unregister_push_device_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UnregisterPushDeviceRequest extends UnregisterPushDeviceRequest {
  @override
  final String? token;

  factory _$UnregisterPushDeviceRequest(
          [void Function(UnregisterPushDeviceRequestBuilder)? updates]) =>
      (UnregisterPushDeviceRequestBuilder()..update(updates))._build();

  _$UnregisterPushDeviceRequest._({this.token}) : super._();
  @override
  UnregisterPushDeviceRequest rebuild(
          void Function(UnregisterPushDeviceRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UnregisterPushDeviceRequestBuilder toBuilder() =>
      UnregisterPushDeviceRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UnregisterPushDeviceRequest && token == other.token;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, token.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UnregisterPushDeviceRequest')
          ..add('token', token))
        .toString();
  }
}

class UnregisterPushDeviceRequestBuilder
    implements
        Builder<UnregisterPushDeviceRequest,
            UnregisterPushDeviceRequestBuilder> {
  _$UnregisterPushDeviceRequest? _$v;

  String? _token;
  String? get token => _$this._token;
  set token(String? token) => _$this._token = token;

  UnregisterPushDeviceRequestBuilder() {
    UnregisterPushDeviceRequest._defaults(this);
  }

  UnregisterPushDeviceRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _token = $v.token;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UnregisterPushDeviceRequest other) {
    _$v = other as _$UnregisterPushDeviceRequest;
  }

  @override
  void update(void Function(UnregisterPushDeviceRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UnregisterPushDeviceRequest build() => _build();

  _$UnregisterPushDeviceRequest _build() {
    final _$result = _$v ??
        _$UnregisterPushDeviceRequest._(
          token: token,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
