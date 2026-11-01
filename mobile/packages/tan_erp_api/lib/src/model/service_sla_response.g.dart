// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_sla_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceSlaResponse extends ServiceSlaResponse {
  @override
  final int? responseHours;
  @override
  final int? resolveHours;
  @override
  final double? elapsedHours;
  @override
  final int? pausedSeconds;
  @override
  final String? status;

  factory _$ServiceSlaResponse(
          [void Function(ServiceSlaResponseBuilder)? updates]) =>
      (ServiceSlaResponseBuilder()..update(updates))._build();

  _$ServiceSlaResponse._(
      {this.responseHours,
      this.resolveHours,
      this.elapsedHours,
      this.pausedSeconds,
      this.status})
      : super._();
  @override
  ServiceSlaResponse rebuild(
          void Function(ServiceSlaResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceSlaResponseBuilder toBuilder() =>
      ServiceSlaResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceSlaResponse &&
        responseHours == other.responseHours &&
        resolveHours == other.resolveHours &&
        elapsedHours == other.elapsedHours &&
        pausedSeconds == other.pausedSeconds &&
        status == other.status;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, responseHours.hashCode);
    _$hash = $jc(_$hash, resolveHours.hashCode);
    _$hash = $jc(_$hash, elapsedHours.hashCode);
    _$hash = $jc(_$hash, pausedSeconds.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ServiceSlaResponse')
          ..add('responseHours', responseHours)
          ..add('resolveHours', resolveHours)
          ..add('elapsedHours', elapsedHours)
          ..add('pausedSeconds', pausedSeconds)
          ..add('status', status))
        .toString();
  }
}

class ServiceSlaResponseBuilder
    implements Builder<ServiceSlaResponse, ServiceSlaResponseBuilder> {
  _$ServiceSlaResponse? _$v;

  int? _responseHours;
  int? get responseHours => _$this._responseHours;
  set responseHours(int? responseHours) =>
      _$this._responseHours = responseHours;

  int? _resolveHours;
  int? get resolveHours => _$this._resolveHours;
  set resolveHours(int? resolveHours) => _$this._resolveHours = resolveHours;

  double? _elapsedHours;
  double? get elapsedHours => _$this._elapsedHours;
  set elapsedHours(double? elapsedHours) => _$this._elapsedHours = elapsedHours;

  int? _pausedSeconds;
  int? get pausedSeconds => _$this._pausedSeconds;
  set pausedSeconds(int? pausedSeconds) =>
      _$this._pausedSeconds = pausedSeconds;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  ServiceSlaResponseBuilder() {
    ServiceSlaResponse._defaults(this);
  }

  ServiceSlaResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _responseHours = $v.responseHours;
      _resolveHours = $v.resolveHours;
      _elapsedHours = $v.elapsedHours;
      _pausedSeconds = $v.pausedSeconds;
      _status = $v.status;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceSlaResponse other) {
    _$v = other as _$ServiceSlaResponse;
  }

  @override
  void update(void Function(ServiceSlaResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceSlaResponse build() => _build();

  _$ServiceSlaResponse _build() {
    final _$result = _$v ??
        _$ServiceSlaResponse._(
          responseHours: responseHours,
          resolveHours: resolveHours,
          elapsedHours: elapsedHours,
          pausedSeconds: pausedSeconds,
          status: status,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
