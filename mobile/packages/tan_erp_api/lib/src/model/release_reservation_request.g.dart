// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'release_reservation_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReleaseReservationRequest extends ReleaseReservationRequest {
  @override
  final String? expectedVersion;

  factory _$ReleaseReservationRequest(
          [void Function(ReleaseReservationRequestBuilder)? updates]) =>
      (ReleaseReservationRequestBuilder()..update(updates))._build();

  _$ReleaseReservationRequest._({this.expectedVersion}) : super._();
  @override
  ReleaseReservationRequest rebuild(
          void Function(ReleaseReservationRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReleaseReservationRequestBuilder toBuilder() =>
      ReleaseReservationRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReleaseReservationRequest &&
        expectedVersion == other.expectedVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReleaseReservationRequest')
          ..add('expectedVersion', expectedVersion))
        .toString();
  }
}

class ReleaseReservationRequestBuilder
    implements
        Builder<ReleaseReservationRequest, ReleaseReservationRequestBuilder> {
  _$ReleaseReservationRequest? _$v;

  String? _expectedVersion;
  String? get expectedVersion => _$this._expectedVersion;
  set expectedVersion(String? expectedVersion) =>
      _$this._expectedVersion = expectedVersion;

  ReleaseReservationRequestBuilder() {
    ReleaseReservationRequest._defaults(this);
  }

  ReleaseReservationRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedVersion = $v.expectedVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReleaseReservationRequest other) {
    _$v = other as _$ReleaseReservationRequest;
  }

  @override
  void update(void Function(ReleaseReservationRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReleaseReservationRequest build() => _build();

  _$ReleaseReservationRequest _build() {
    final _$result = _$v ??
        _$ReleaseReservationRequest._(
          expectedVersion: expectedVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
