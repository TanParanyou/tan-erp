// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_center_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkCenterRefResponse extends WorkCenterRefResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final double? dailyCapacityHours;
  @override
  final double? hourlyRate;

  factory _$WorkCenterRefResponse(
          [void Function(WorkCenterRefResponseBuilder)? updates]) =>
      (WorkCenterRefResponseBuilder()..update(updates))._build();

  _$WorkCenterRefResponse._(
      {this.id, this.code, this.name, this.dailyCapacityHours, this.hourlyRate})
      : super._();
  @override
  WorkCenterRefResponse rebuild(
          void Function(WorkCenterRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkCenterRefResponseBuilder toBuilder() =>
      WorkCenterRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkCenterRefResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name &&
        dailyCapacityHours == other.dailyCapacityHours &&
        hourlyRate == other.hourlyRate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, dailyCapacityHours.hashCode);
    _$hash = $jc(_$hash, hourlyRate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkCenterRefResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name)
          ..add('dailyCapacityHours', dailyCapacityHours)
          ..add('hourlyRate', hourlyRate))
        .toString();
  }
}

class WorkCenterRefResponseBuilder
    implements Builder<WorkCenterRefResponse, WorkCenterRefResponseBuilder> {
  _$WorkCenterRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

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

  WorkCenterRefResponseBuilder() {
    WorkCenterRefResponse._defaults(this);
  }

  WorkCenterRefResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name;
      _dailyCapacityHours = $v.dailyCapacityHours;
      _hourlyRate = $v.hourlyRate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkCenterRefResponse other) {
    _$v = other as _$WorkCenterRefResponse;
  }

  @override
  void update(void Function(WorkCenterRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkCenterRefResponse build() => _build();

  _$WorkCenterRefResponse _build() {
    final _$result = _$v ??
        _$WorkCenterRefResponse._(
          id: id,
          code: code,
          name: name,
          dailyCapacityHours: dailyCapacityHours,
          hourlyRate: hourlyRate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
