// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'working_calendar_holiday_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkingCalendarHolidayResponse extends WorkingCalendarHolidayResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final Date? holidayDate;
  @override
  final String? name;
  @override
  final DateTime? createdAtUtc;

  factory _$WorkingCalendarHolidayResponse(
          [void Function(WorkingCalendarHolidayResponseBuilder)? updates]) =>
      (WorkingCalendarHolidayResponseBuilder()..update(updates))._build();

  _$WorkingCalendarHolidayResponse._(
      {this.id, this.branchId, this.holidayDate, this.name, this.createdAtUtc})
      : super._();
  @override
  WorkingCalendarHolidayResponse rebuild(
          void Function(WorkingCalendarHolidayResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkingCalendarHolidayResponseBuilder toBuilder() =>
      WorkingCalendarHolidayResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkingCalendarHolidayResponse &&
        id == other.id &&
        branchId == other.branchId &&
        holidayDate == other.holidayDate &&
        name == other.name &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, holidayDate.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkingCalendarHolidayResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('holidayDate', holidayDate)
          ..add('name', name)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class WorkingCalendarHolidayResponseBuilder
    implements
        Builder<WorkingCalendarHolidayResponse,
            WorkingCalendarHolidayResponseBuilder> {
  _$WorkingCalendarHolidayResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  Date? _holidayDate;
  Date? get holidayDate => _$this._holidayDate;
  set holidayDate(Date? holidayDate) => _$this._holidayDate = holidayDate;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  WorkingCalendarHolidayResponseBuilder() {
    WorkingCalendarHolidayResponse._defaults(this);
  }

  WorkingCalendarHolidayResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _holidayDate = $v.holidayDate;
      _name = $v.name;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkingCalendarHolidayResponse other) {
    _$v = other as _$WorkingCalendarHolidayResponse;
  }

  @override
  void update(void Function(WorkingCalendarHolidayResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkingCalendarHolidayResponse build() => _build();

  _$WorkingCalendarHolidayResponse _build() {
    final _$result = _$v ??
        _$WorkingCalendarHolidayResponse._(
          id: id,
          branchId: branchId,
          holidayDate: holidayDate,
          name: name,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
