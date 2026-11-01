// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'working_calendar_holiday_list_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkingCalendarHolidayListResponse
    extends WorkingCalendarHolidayListResponse {
  @override
  final BuiltList<WorkingCalendarHolidayResponse>? items;

  factory _$WorkingCalendarHolidayListResponse(
          [void Function(WorkingCalendarHolidayListResponseBuilder)?
              updates]) =>
      (WorkingCalendarHolidayListResponseBuilder()..update(updates))._build();

  _$WorkingCalendarHolidayListResponse._({this.items}) : super._();
  @override
  WorkingCalendarHolidayListResponse rebuild(
          void Function(WorkingCalendarHolidayListResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkingCalendarHolidayListResponseBuilder toBuilder() =>
      WorkingCalendarHolidayListResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkingCalendarHolidayListResponse && items == other.items;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, items.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkingCalendarHolidayListResponse')
          ..add('items', items))
        .toString();
  }
}

class WorkingCalendarHolidayListResponseBuilder
    implements
        Builder<WorkingCalendarHolidayListResponse,
            WorkingCalendarHolidayListResponseBuilder> {
  _$WorkingCalendarHolidayListResponse? _$v;

  ListBuilder<WorkingCalendarHolidayResponse>? _items;
  ListBuilder<WorkingCalendarHolidayResponse> get items =>
      _$this._items ??= ListBuilder<WorkingCalendarHolidayResponse>();
  set items(ListBuilder<WorkingCalendarHolidayResponse>? items) =>
      _$this._items = items;

  WorkingCalendarHolidayListResponseBuilder() {
    WorkingCalendarHolidayListResponse._defaults(this);
  }

  WorkingCalendarHolidayListResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _items = $v.items?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkingCalendarHolidayListResponse other) {
    _$v = other as _$WorkingCalendarHolidayListResponse;
  }

  @override
  void update(
      void Function(WorkingCalendarHolidayListResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkingCalendarHolidayListResponse build() => _build();

  _$WorkingCalendarHolidayListResponse _build() {
    _$WorkingCalendarHolidayListResponse _$result;
    try {
      _$result = _$v ??
          _$WorkingCalendarHolidayListResponse._(
            items: _items?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'items';
        _items?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WorkingCalendarHolidayListResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
