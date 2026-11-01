// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_center_capacity_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkCenterCapacityRowResponse extends WorkCenterCapacityRowResponse {
  @override
  final String? workCenterId;
  @override
  final String? branchId;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final double? dailyCapacityHours;
  @override
  final double? hourlyRate;
  @override
  final double? totalCapacityHours;
  @override
  final double? totalPlannedHours;
  @override
  final double? utilizationPercent;
  @override
  final bool? isOverloaded;
  @override
  final BuiltList<WorkCenterDailyCapacityPointResponse>? days;

  factory _$WorkCenterCapacityRowResponse(
          [void Function(WorkCenterCapacityRowResponseBuilder)? updates]) =>
      (WorkCenterCapacityRowResponseBuilder()..update(updates))._build();

  _$WorkCenterCapacityRowResponse._(
      {this.workCenterId,
      this.branchId,
      this.code,
      this.name,
      this.dailyCapacityHours,
      this.hourlyRate,
      this.totalCapacityHours,
      this.totalPlannedHours,
      this.utilizationPercent,
      this.isOverloaded,
      this.days})
      : super._();
  @override
  WorkCenterCapacityRowResponse rebuild(
          void Function(WorkCenterCapacityRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkCenterCapacityRowResponseBuilder toBuilder() =>
      WorkCenterCapacityRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkCenterCapacityRowResponse &&
        workCenterId == other.workCenterId &&
        branchId == other.branchId &&
        code == other.code &&
        name == other.name &&
        dailyCapacityHours == other.dailyCapacityHours &&
        hourlyRate == other.hourlyRate &&
        totalCapacityHours == other.totalCapacityHours &&
        totalPlannedHours == other.totalPlannedHours &&
        utilizationPercent == other.utilizationPercent &&
        isOverloaded == other.isOverloaded &&
        days == other.days;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, workCenterId.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, dailyCapacityHours.hashCode);
    _$hash = $jc(_$hash, hourlyRate.hashCode);
    _$hash = $jc(_$hash, totalCapacityHours.hashCode);
    _$hash = $jc(_$hash, totalPlannedHours.hashCode);
    _$hash = $jc(_$hash, utilizationPercent.hashCode);
    _$hash = $jc(_$hash, isOverloaded.hashCode);
    _$hash = $jc(_$hash, days.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkCenterCapacityRowResponse')
          ..add('workCenterId', workCenterId)
          ..add('branchId', branchId)
          ..add('code', code)
          ..add('name', name)
          ..add('dailyCapacityHours', dailyCapacityHours)
          ..add('hourlyRate', hourlyRate)
          ..add('totalCapacityHours', totalCapacityHours)
          ..add('totalPlannedHours', totalPlannedHours)
          ..add('utilizationPercent', utilizationPercent)
          ..add('isOverloaded', isOverloaded)
          ..add('days', days))
        .toString();
  }
}

class WorkCenterCapacityRowResponseBuilder
    implements
        Builder<WorkCenterCapacityRowResponse,
            WorkCenterCapacityRowResponseBuilder> {
  _$WorkCenterCapacityRowResponse? _$v;

  String? _workCenterId;
  String? get workCenterId => _$this._workCenterId;
  set workCenterId(String? workCenterId) => _$this._workCenterId = workCenterId;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

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

  double? _totalCapacityHours;
  double? get totalCapacityHours => _$this._totalCapacityHours;
  set totalCapacityHours(double? totalCapacityHours) =>
      _$this._totalCapacityHours = totalCapacityHours;

  double? _totalPlannedHours;
  double? get totalPlannedHours => _$this._totalPlannedHours;
  set totalPlannedHours(double? totalPlannedHours) =>
      _$this._totalPlannedHours = totalPlannedHours;

  double? _utilizationPercent;
  double? get utilizationPercent => _$this._utilizationPercent;
  set utilizationPercent(double? utilizationPercent) =>
      _$this._utilizationPercent = utilizationPercent;

  bool? _isOverloaded;
  bool? get isOverloaded => _$this._isOverloaded;
  set isOverloaded(bool? isOverloaded) => _$this._isOverloaded = isOverloaded;

  ListBuilder<WorkCenterDailyCapacityPointResponse>? _days;
  ListBuilder<WorkCenterDailyCapacityPointResponse> get days =>
      _$this._days ??= ListBuilder<WorkCenterDailyCapacityPointResponse>();
  set days(ListBuilder<WorkCenterDailyCapacityPointResponse>? days) =>
      _$this._days = days;

  WorkCenterCapacityRowResponseBuilder() {
    WorkCenterCapacityRowResponse._defaults(this);
  }

  WorkCenterCapacityRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _workCenterId = $v.workCenterId;
      _branchId = $v.branchId;
      _code = $v.code;
      _name = $v.name;
      _dailyCapacityHours = $v.dailyCapacityHours;
      _hourlyRate = $v.hourlyRate;
      _totalCapacityHours = $v.totalCapacityHours;
      _totalPlannedHours = $v.totalPlannedHours;
      _utilizationPercent = $v.utilizationPercent;
      _isOverloaded = $v.isOverloaded;
      _days = $v.days?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkCenterCapacityRowResponse other) {
    _$v = other as _$WorkCenterCapacityRowResponse;
  }

  @override
  void update(void Function(WorkCenterCapacityRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkCenterCapacityRowResponse build() => _build();

  _$WorkCenterCapacityRowResponse _build() {
    _$WorkCenterCapacityRowResponse _$result;
    try {
      _$result = _$v ??
          _$WorkCenterCapacityRowResponse._(
            workCenterId: workCenterId,
            branchId: branchId,
            code: code,
            name: name,
            dailyCapacityHours: dailyCapacityHours,
            hourlyRate: hourlyRate,
            totalCapacityHours: totalCapacityHours,
            totalPlannedHours: totalPlannedHours,
            utilizationPercent: utilizationPercent,
            isOverloaded: isOverloaded,
            days: _days?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'days';
        _days?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WorkCenterCapacityRowResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
