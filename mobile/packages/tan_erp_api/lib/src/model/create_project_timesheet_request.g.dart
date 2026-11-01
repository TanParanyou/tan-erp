// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_project_timesheet_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateProjectTimesheetRequest extends CreateProjectTimesheetRequest {
  @override
  final String? workerUserId;
  @override
  final Date? workDate;
  @override
  final double? hours;
  @override
  final String? wbsNodeId;
  @override
  final String? workOrderId;
  @override
  final String? taskDescription;

  factory _$CreateProjectTimesheetRequest(
          [void Function(CreateProjectTimesheetRequestBuilder)? updates]) =>
      (CreateProjectTimesheetRequestBuilder()..update(updates))._build();

  _$CreateProjectTimesheetRequest._(
      {this.workerUserId,
      this.workDate,
      this.hours,
      this.wbsNodeId,
      this.workOrderId,
      this.taskDescription})
      : super._();
  @override
  CreateProjectTimesheetRequest rebuild(
          void Function(CreateProjectTimesheetRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateProjectTimesheetRequestBuilder toBuilder() =>
      CreateProjectTimesheetRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateProjectTimesheetRequest &&
        workerUserId == other.workerUserId &&
        workDate == other.workDate &&
        hours == other.hours &&
        wbsNodeId == other.wbsNodeId &&
        workOrderId == other.workOrderId &&
        taskDescription == other.taskDescription;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, workerUserId.hashCode);
    _$hash = $jc(_$hash, workDate.hashCode);
    _$hash = $jc(_$hash, hours.hashCode);
    _$hash = $jc(_$hash, wbsNodeId.hashCode);
    _$hash = $jc(_$hash, workOrderId.hashCode);
    _$hash = $jc(_$hash, taskDescription.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateProjectTimesheetRequest')
          ..add('workerUserId', workerUserId)
          ..add('workDate', workDate)
          ..add('hours', hours)
          ..add('wbsNodeId', wbsNodeId)
          ..add('workOrderId', workOrderId)
          ..add('taskDescription', taskDescription))
        .toString();
  }
}

class CreateProjectTimesheetRequestBuilder
    implements
        Builder<CreateProjectTimesheetRequest,
            CreateProjectTimesheetRequestBuilder> {
  _$CreateProjectTimesheetRequest? _$v;

  String? _workerUserId;
  String? get workerUserId => _$this._workerUserId;
  set workerUserId(String? workerUserId) => _$this._workerUserId = workerUserId;

  Date? _workDate;
  Date? get workDate => _$this._workDate;
  set workDate(Date? workDate) => _$this._workDate = workDate;

  double? _hours;
  double? get hours => _$this._hours;
  set hours(double? hours) => _$this._hours = hours;

  String? _wbsNodeId;
  String? get wbsNodeId => _$this._wbsNodeId;
  set wbsNodeId(String? wbsNodeId) => _$this._wbsNodeId = wbsNodeId;

  String? _workOrderId;
  String? get workOrderId => _$this._workOrderId;
  set workOrderId(String? workOrderId) => _$this._workOrderId = workOrderId;

  String? _taskDescription;
  String? get taskDescription => _$this._taskDescription;
  set taskDescription(String? taskDescription) =>
      _$this._taskDescription = taskDescription;

  CreateProjectTimesheetRequestBuilder() {
    CreateProjectTimesheetRequest._defaults(this);
  }

  CreateProjectTimesheetRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _workerUserId = $v.workerUserId;
      _workDate = $v.workDate;
      _hours = $v.hours;
      _wbsNodeId = $v.wbsNodeId;
      _workOrderId = $v.workOrderId;
      _taskDescription = $v.taskDescription;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateProjectTimesheetRequest other) {
    _$v = other as _$CreateProjectTimesheetRequest;
  }

  @override
  void update(void Function(CreateProjectTimesheetRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateProjectTimesheetRequest build() => _build();

  _$CreateProjectTimesheetRequest _build() {
    final _$result = _$v ??
        _$CreateProjectTimesheetRequest._(
          workerUserId: workerUserId,
          workDate: workDate,
          hours: hours,
          wbsNodeId: wbsNodeId,
          workOrderId: workOrderId,
          taskDescription: taskDescription,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
