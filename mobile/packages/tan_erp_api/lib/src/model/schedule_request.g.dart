// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'schedule_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ScheduleRequest extends ScheduleRequest {
  @override
  final Date? date;

  factory _$ScheduleRequest([void Function(ScheduleRequestBuilder)? updates]) =>
      (ScheduleRequestBuilder()..update(updates))._build();

  _$ScheduleRequest._({this.date}) : super._();
  @override
  ScheduleRequest rebuild(void Function(ScheduleRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ScheduleRequestBuilder toBuilder() => ScheduleRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ScheduleRequest && date == other.date;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, date.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ScheduleRequest')..add('date', date))
        .toString();
  }
}

class ScheduleRequestBuilder
    implements Builder<ScheduleRequest, ScheduleRequestBuilder> {
  _$ScheduleRequest? _$v;

  Date? _date;
  Date? get date => _$this._date;
  set date(Date? date) => _$this._date = date;

  ScheduleRequestBuilder() {
    ScheduleRequest._defaults(this);
  }

  ScheduleRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _date = $v.date;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ScheduleRequest other) {
    _$v = other as _$ScheduleRequest;
  }

  @override
  void update(void Function(ScheduleRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ScheduleRequest build() => _build();

  _$ScheduleRequest _build() {
    final _$result = _$v ??
        _$ScheduleRequest._(
          date: date,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
