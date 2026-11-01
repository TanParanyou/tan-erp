// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'working_calendar_holiday_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkingCalendarHolidayRequest extends WorkingCalendarHolidayRequest {
  @override
  final Date? holidayDate;
  @override
  final String? name;
  @override
  final String? branchId;

  factory _$WorkingCalendarHolidayRequest(
          [void Function(WorkingCalendarHolidayRequestBuilder)? updates]) =>
      (WorkingCalendarHolidayRequestBuilder()..update(updates))._build();

  _$WorkingCalendarHolidayRequest._(
      {this.holidayDate, this.name, this.branchId})
      : super._();
  @override
  WorkingCalendarHolidayRequest rebuild(
          void Function(WorkingCalendarHolidayRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkingCalendarHolidayRequestBuilder toBuilder() =>
      WorkingCalendarHolidayRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkingCalendarHolidayRequest &&
        holidayDate == other.holidayDate &&
        name == other.name &&
        branchId == other.branchId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, holidayDate.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkingCalendarHolidayRequest')
          ..add('holidayDate', holidayDate)
          ..add('name', name)
          ..add('branchId', branchId))
        .toString();
  }
}

class WorkingCalendarHolidayRequestBuilder
    implements
        Builder<WorkingCalendarHolidayRequest,
            WorkingCalendarHolidayRequestBuilder> {
  _$WorkingCalendarHolidayRequest? _$v;

  Date? _holidayDate;
  Date? get holidayDate => _$this._holidayDate;
  set holidayDate(Date? holidayDate) => _$this._holidayDate = holidayDate;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  WorkingCalendarHolidayRequestBuilder() {
    WorkingCalendarHolidayRequest._defaults(this);
  }

  WorkingCalendarHolidayRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _holidayDate = $v.holidayDate;
      _name = $v.name;
      _branchId = $v.branchId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkingCalendarHolidayRequest other) {
    _$v = other as _$WorkingCalendarHolidayRequest;
  }

  @override
  void update(void Function(WorkingCalendarHolidayRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkingCalendarHolidayRequest build() => _build();

  _$WorkingCalendarHolidayRequest _build() {
    final _$result = _$v ??
        _$WorkingCalendarHolidayRequest._(
          holidayDate: holidayDate,
          name: name,
          branchId: branchId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
