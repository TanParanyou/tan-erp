// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_center_capacity_view_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkCenterCapacityViewResponse extends WorkCenterCapacityViewResponse {
  @override
  final Date? fromDate;
  @override
  final Date? toDate;
  @override
  final BuiltList<WorkCenterCapacityRowResponse>? workCenters;

  factory _$WorkCenterCapacityViewResponse(
          [void Function(WorkCenterCapacityViewResponseBuilder)? updates]) =>
      (WorkCenterCapacityViewResponseBuilder()..update(updates))._build();

  _$WorkCenterCapacityViewResponse._(
      {this.fromDate, this.toDate, this.workCenters})
      : super._();
  @override
  WorkCenterCapacityViewResponse rebuild(
          void Function(WorkCenterCapacityViewResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkCenterCapacityViewResponseBuilder toBuilder() =>
      WorkCenterCapacityViewResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkCenterCapacityViewResponse &&
        fromDate == other.fromDate &&
        toDate == other.toDate &&
        workCenters == other.workCenters;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, fromDate.hashCode);
    _$hash = $jc(_$hash, toDate.hashCode);
    _$hash = $jc(_$hash, workCenters.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkCenterCapacityViewResponse')
          ..add('fromDate', fromDate)
          ..add('toDate', toDate)
          ..add('workCenters', workCenters))
        .toString();
  }
}

class WorkCenterCapacityViewResponseBuilder
    implements
        Builder<WorkCenterCapacityViewResponse,
            WorkCenterCapacityViewResponseBuilder> {
  _$WorkCenterCapacityViewResponse? _$v;

  Date? _fromDate;
  Date? get fromDate => _$this._fromDate;
  set fromDate(Date? fromDate) => _$this._fromDate = fromDate;

  Date? _toDate;
  Date? get toDate => _$this._toDate;
  set toDate(Date? toDate) => _$this._toDate = toDate;

  ListBuilder<WorkCenterCapacityRowResponse>? _workCenters;
  ListBuilder<WorkCenterCapacityRowResponse> get workCenters =>
      _$this._workCenters ??= ListBuilder<WorkCenterCapacityRowResponse>();
  set workCenters(ListBuilder<WorkCenterCapacityRowResponse>? workCenters) =>
      _$this._workCenters = workCenters;

  WorkCenterCapacityViewResponseBuilder() {
    WorkCenterCapacityViewResponse._defaults(this);
  }

  WorkCenterCapacityViewResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fromDate = $v.fromDate;
      _toDate = $v.toDate;
      _workCenters = $v.workCenters?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkCenterCapacityViewResponse other) {
    _$v = other as _$WorkCenterCapacityViewResponse;
  }

  @override
  void update(void Function(WorkCenterCapacityViewResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkCenterCapacityViewResponse build() => _build();

  _$WorkCenterCapacityViewResponse _build() {
    _$WorkCenterCapacityViewResponse _$result;
    try {
      _$result = _$v ??
          _$WorkCenterCapacityViewResponse._(
            fromDate: fromDate,
            toDate: toDate,
            workCenters: _workCenters?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'workCenters';
        _workCenters?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WorkCenterCapacityViewResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
