// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'unregister_push_device_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UnregisterPushDeviceResponse extends UnregisterPushDeviceResponse {
  @override
  final bool? removed;

  factory _$UnregisterPushDeviceResponse(
          [void Function(UnregisterPushDeviceResponseBuilder)? updates]) =>
      (UnregisterPushDeviceResponseBuilder()..update(updates))._build();

  _$UnregisterPushDeviceResponse._({this.removed}) : super._();
  @override
  UnregisterPushDeviceResponse rebuild(
          void Function(UnregisterPushDeviceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UnregisterPushDeviceResponseBuilder toBuilder() =>
      UnregisterPushDeviceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UnregisterPushDeviceResponse && removed == other.removed;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, removed.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UnregisterPushDeviceResponse')
          ..add('removed', removed))
        .toString();
  }
}

class UnregisterPushDeviceResponseBuilder
    implements
        Builder<UnregisterPushDeviceResponse,
            UnregisterPushDeviceResponseBuilder> {
  _$UnregisterPushDeviceResponse? _$v;

  bool? _removed;
  bool? get removed => _$this._removed;
  set removed(bool? removed) => _$this._removed = removed;

  UnregisterPushDeviceResponseBuilder() {
    UnregisterPushDeviceResponse._defaults(this);
  }

  UnregisterPushDeviceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _removed = $v.removed;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UnregisterPushDeviceResponse other) {
    _$v = other as _$UnregisterPushDeviceResponse;
  }

  @override
  void update(void Function(UnregisterPushDeviceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UnregisterPushDeviceResponse build() => _build();

  _$UnregisterPushDeviceResponse _build() {
    final _$result = _$v ??
        _$UnregisterPushDeviceResponse._(
          removed: removed,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
