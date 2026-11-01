// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'set_project_labor_rate_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SetProjectLaborRateRequest extends SetProjectLaborRateRequest {
  @override
  final String? workerUserId;
  @override
  final double? hourlyRate;

  factory _$SetProjectLaborRateRequest(
          [void Function(SetProjectLaborRateRequestBuilder)? updates]) =>
      (SetProjectLaborRateRequestBuilder()..update(updates))._build();

  _$SetProjectLaborRateRequest._({this.workerUserId, this.hourlyRate})
      : super._();
  @override
  SetProjectLaborRateRequest rebuild(
          void Function(SetProjectLaborRateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SetProjectLaborRateRequestBuilder toBuilder() =>
      SetProjectLaborRateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SetProjectLaborRateRequest &&
        workerUserId == other.workerUserId &&
        hourlyRate == other.hourlyRate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, workerUserId.hashCode);
    _$hash = $jc(_$hash, hourlyRate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SetProjectLaborRateRequest')
          ..add('workerUserId', workerUserId)
          ..add('hourlyRate', hourlyRate))
        .toString();
  }
}

class SetProjectLaborRateRequestBuilder
    implements
        Builder<SetProjectLaborRateRequest, SetProjectLaborRateRequestBuilder> {
  _$SetProjectLaborRateRequest? _$v;

  String? _workerUserId;
  String? get workerUserId => _$this._workerUserId;
  set workerUserId(String? workerUserId) => _$this._workerUserId = workerUserId;

  double? _hourlyRate;
  double? get hourlyRate => _$this._hourlyRate;
  set hourlyRate(double? hourlyRate) => _$this._hourlyRate = hourlyRate;

  SetProjectLaborRateRequestBuilder() {
    SetProjectLaborRateRequest._defaults(this);
  }

  SetProjectLaborRateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _workerUserId = $v.workerUserId;
      _hourlyRate = $v.hourlyRate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SetProjectLaborRateRequest other) {
    _$v = other as _$SetProjectLaborRateRequest;
  }

  @override
  void update(void Function(SetProjectLaborRateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SetProjectLaborRateRequest build() => _build();

  _$SetProjectLaborRateRequest _build() {
    final _$result = _$v ??
        _$SetProjectLaborRateRequest._(
          workerUserId: workerUserId,
          hourlyRate: hourlyRate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
