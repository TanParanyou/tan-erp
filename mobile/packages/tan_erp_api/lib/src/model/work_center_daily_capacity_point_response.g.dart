// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_center_daily_capacity_point_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkCenterDailyCapacityPointResponse
    extends WorkCenterDailyCapacityPointResponse {
  @override
  final Date? date;
  @override
  final double? capacityHours;
  @override
  final double? plannedHours;
  @override
  final double? remainingHours;
  @override
  final double? utilizationPercent;
  @override
  final bool? isOverloaded;

  factory _$WorkCenterDailyCapacityPointResponse(
          [void Function(WorkCenterDailyCapacityPointResponseBuilder)?
              updates]) =>
      (WorkCenterDailyCapacityPointResponseBuilder()..update(updates))._build();

  _$WorkCenterDailyCapacityPointResponse._(
      {this.date,
      this.capacityHours,
      this.plannedHours,
      this.remainingHours,
      this.utilizationPercent,
      this.isOverloaded})
      : super._();
  @override
  WorkCenterDailyCapacityPointResponse rebuild(
          void Function(WorkCenterDailyCapacityPointResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkCenterDailyCapacityPointResponseBuilder toBuilder() =>
      WorkCenterDailyCapacityPointResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkCenterDailyCapacityPointResponse &&
        date == other.date &&
        capacityHours == other.capacityHours &&
        plannedHours == other.plannedHours &&
        remainingHours == other.remainingHours &&
        utilizationPercent == other.utilizationPercent &&
        isOverloaded == other.isOverloaded;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, date.hashCode);
    _$hash = $jc(_$hash, capacityHours.hashCode);
    _$hash = $jc(_$hash, plannedHours.hashCode);
    _$hash = $jc(_$hash, remainingHours.hashCode);
    _$hash = $jc(_$hash, utilizationPercent.hashCode);
    _$hash = $jc(_$hash, isOverloaded.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkCenterDailyCapacityPointResponse')
          ..add('date', date)
          ..add('capacityHours', capacityHours)
          ..add('plannedHours', plannedHours)
          ..add('remainingHours', remainingHours)
          ..add('utilizationPercent', utilizationPercent)
          ..add('isOverloaded', isOverloaded))
        .toString();
  }
}

class WorkCenterDailyCapacityPointResponseBuilder
    implements
        Builder<WorkCenterDailyCapacityPointResponse,
            WorkCenterDailyCapacityPointResponseBuilder> {
  _$WorkCenterDailyCapacityPointResponse? _$v;

  Date? _date;
  Date? get date => _$this._date;
  set date(Date? date) => _$this._date = date;

  double? _capacityHours;
  double? get capacityHours => _$this._capacityHours;
  set capacityHours(double? capacityHours) =>
      _$this._capacityHours = capacityHours;

  double? _plannedHours;
  double? get plannedHours => _$this._plannedHours;
  set plannedHours(double? plannedHours) => _$this._plannedHours = plannedHours;

  double? _remainingHours;
  double? get remainingHours => _$this._remainingHours;
  set remainingHours(double? remainingHours) =>
      _$this._remainingHours = remainingHours;

  double? _utilizationPercent;
  double? get utilizationPercent => _$this._utilizationPercent;
  set utilizationPercent(double? utilizationPercent) =>
      _$this._utilizationPercent = utilizationPercent;

  bool? _isOverloaded;
  bool? get isOverloaded => _$this._isOverloaded;
  set isOverloaded(bool? isOverloaded) => _$this._isOverloaded = isOverloaded;

  WorkCenterDailyCapacityPointResponseBuilder() {
    WorkCenterDailyCapacityPointResponse._defaults(this);
  }

  WorkCenterDailyCapacityPointResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _date = $v.date;
      _capacityHours = $v.capacityHours;
      _plannedHours = $v.plannedHours;
      _remainingHours = $v.remainingHours;
      _utilizationPercent = $v.utilizationPercent;
      _isOverloaded = $v.isOverloaded;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkCenterDailyCapacityPointResponse other) {
    _$v = other as _$WorkCenterDailyCapacityPointResponse;
  }

  @override
  void update(
      void Function(WorkCenterDailyCapacityPointResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkCenterDailyCapacityPointResponse build() => _build();

  _$WorkCenterDailyCapacityPointResponse _build() {
    final _$result = _$v ??
        _$WorkCenterDailyCapacityPointResponse._(
          date: date,
          capacityHours: capacityHours,
          plannedHours: plannedHours,
          remainingHours: remainingHours,
          utilizationPercent: utilizationPercent,
          isOverloaded: isOverloaded,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
