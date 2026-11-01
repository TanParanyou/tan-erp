// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_work_center_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateWorkCenterRequest extends UpdateWorkCenterRequest {
  @override
  final String? name;
  @override
  final double? dailyCapacityHours;
  @override
  final double? hourlyRate;

  factory _$UpdateWorkCenterRequest(
          [void Function(UpdateWorkCenterRequestBuilder)? updates]) =>
      (UpdateWorkCenterRequestBuilder()..update(updates))._build();

  _$UpdateWorkCenterRequest._(
      {this.name, this.dailyCapacityHours, this.hourlyRate})
      : super._();
  @override
  UpdateWorkCenterRequest rebuild(
          void Function(UpdateWorkCenterRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateWorkCenterRequestBuilder toBuilder() =>
      UpdateWorkCenterRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateWorkCenterRequest &&
        name == other.name &&
        dailyCapacityHours == other.dailyCapacityHours &&
        hourlyRate == other.hourlyRate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, dailyCapacityHours.hashCode);
    _$hash = $jc(_$hash, hourlyRate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateWorkCenterRequest')
          ..add('name', name)
          ..add('dailyCapacityHours', dailyCapacityHours)
          ..add('hourlyRate', hourlyRate))
        .toString();
  }
}

class UpdateWorkCenterRequestBuilder
    implements
        Builder<UpdateWorkCenterRequest, UpdateWorkCenterRequestBuilder> {
  _$UpdateWorkCenterRequest? _$v;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  double? _dailyCapacityHours;
  double? get dailyCapacityHours => _$this._dailyCapacityHours;
  set dailyCapacityHours(double? dailyCapacityHours) =>
      _$this._dailyCapacityHours = dailyCapacityHours;

  double? _hourlyRate;
  double? get hourlyRate => _$this._hourlyRate;
  set hourlyRate(double? hourlyRate) => _$this._hourlyRate = hourlyRate;

  UpdateWorkCenterRequestBuilder() {
    UpdateWorkCenterRequest._defaults(this);
  }

  UpdateWorkCenterRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _dailyCapacityHours = $v.dailyCapacityHours;
      _hourlyRate = $v.hourlyRate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateWorkCenterRequest other) {
    _$v = other as _$UpdateWorkCenterRequest;
  }

  @override
  void update(void Function(UpdateWorkCenterRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateWorkCenterRequest build() => _build();

  _$UpdateWorkCenterRequest _build() {
    final _$result = _$v ??
        _$UpdateWorkCenterRequest._(
          name: name,
          dailyCapacityHours: dailyCapacityHours,
          hourlyRate: hourlyRate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
