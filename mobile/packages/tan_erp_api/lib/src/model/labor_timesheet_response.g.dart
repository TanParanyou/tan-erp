// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'labor_timesheet_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$LaborTimesheetResponse extends LaborTimesheetResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final ProjectPersonResponse? worker;
  @override
  final Date? workDate;
  @override
  final double? hours;
  @override
  final double? hourlyRate;
  @override
  final double? costAmount;
  @override
  final String? wbsNodeId;
  @override
  final String? wbsCode;
  @override
  final String? workOrderId;
  @override
  final String? workOrderNumber;
  @override
  final String? taskDescription;
  @override
  final String? status;
  @override
  final bool? isOvertimeWarning;
  @override
  final ProjectPersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? submittedAtUtc;
  @override
  final ProjectPersonResponse? decidedBy;
  @override
  final DateTime? decidedAtUtc;
  @override
  final String? decisionNote;
  @override
  final String? rowVersion;

  factory _$LaborTimesheetResponse(
          [void Function(LaborTimesheetResponseBuilder)? updates]) =>
      (LaborTimesheetResponseBuilder()..update(updates))._build();

  _$LaborTimesheetResponse._(
      {this.id,
      this.number,
      this.worker,
      this.workDate,
      this.hours,
      this.hourlyRate,
      this.costAmount,
      this.wbsNodeId,
      this.wbsCode,
      this.workOrderId,
      this.workOrderNumber,
      this.taskDescription,
      this.status,
      this.isOvertimeWarning,
      this.createdBy,
      this.createdAtUtc,
      this.submittedAtUtc,
      this.decidedBy,
      this.decidedAtUtc,
      this.decisionNote,
      this.rowVersion})
      : super._();
  @override
  LaborTimesheetResponse rebuild(
          void Function(LaborTimesheetResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  LaborTimesheetResponseBuilder toBuilder() =>
      LaborTimesheetResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is LaborTimesheetResponse &&
        id == other.id &&
        number == other.number &&
        worker == other.worker &&
        workDate == other.workDate &&
        hours == other.hours &&
        hourlyRate == other.hourlyRate &&
        costAmount == other.costAmount &&
        wbsNodeId == other.wbsNodeId &&
        wbsCode == other.wbsCode &&
        workOrderId == other.workOrderId &&
        workOrderNumber == other.workOrderNumber &&
        taskDescription == other.taskDescription &&
        status == other.status &&
        isOvertimeWarning == other.isOvertimeWarning &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        submittedAtUtc == other.submittedAtUtc &&
        decidedBy == other.decidedBy &&
        decidedAtUtc == other.decidedAtUtc &&
        decisionNote == other.decisionNote &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, worker.hashCode);
    _$hash = $jc(_$hash, workDate.hashCode);
    _$hash = $jc(_$hash, hours.hashCode);
    _$hash = $jc(_$hash, hourlyRate.hashCode);
    _$hash = $jc(_$hash, costAmount.hashCode);
    _$hash = $jc(_$hash, wbsNodeId.hashCode);
    _$hash = $jc(_$hash, wbsCode.hashCode);
    _$hash = $jc(_$hash, workOrderId.hashCode);
    _$hash = $jc(_$hash, workOrderNumber.hashCode);
    _$hash = $jc(_$hash, taskDescription.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, isOvertimeWarning.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, submittedAtUtc.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jc(_$hash, decisionNote.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'LaborTimesheetResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('worker', worker)
          ..add('workDate', workDate)
          ..add('hours', hours)
          ..add('hourlyRate', hourlyRate)
          ..add('costAmount', costAmount)
          ..add('wbsNodeId', wbsNodeId)
          ..add('wbsCode', wbsCode)
          ..add('workOrderId', workOrderId)
          ..add('workOrderNumber', workOrderNumber)
          ..add('taskDescription', taskDescription)
          ..add('status', status)
          ..add('isOvertimeWarning', isOvertimeWarning)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('submittedAtUtc', submittedAtUtc)
          ..add('decidedBy', decidedBy)
          ..add('decidedAtUtc', decidedAtUtc)
          ..add('decisionNote', decisionNote)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class LaborTimesheetResponseBuilder
    implements Builder<LaborTimesheetResponse, LaborTimesheetResponseBuilder> {
  _$LaborTimesheetResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  ProjectPersonResponseBuilder? _worker;
  ProjectPersonResponseBuilder get worker =>
      _$this._worker ??= ProjectPersonResponseBuilder();
  set worker(ProjectPersonResponseBuilder? worker) => _$this._worker = worker;

  Date? _workDate;
  Date? get workDate => _$this._workDate;
  set workDate(Date? workDate) => _$this._workDate = workDate;

  double? _hours;
  double? get hours => _$this._hours;
  set hours(double? hours) => _$this._hours = hours;

  double? _hourlyRate;
  double? get hourlyRate => _$this._hourlyRate;
  set hourlyRate(double? hourlyRate) => _$this._hourlyRate = hourlyRate;

  double? _costAmount;
  double? get costAmount => _$this._costAmount;
  set costAmount(double? costAmount) => _$this._costAmount = costAmount;

  String? _wbsNodeId;
  String? get wbsNodeId => _$this._wbsNodeId;
  set wbsNodeId(String? wbsNodeId) => _$this._wbsNodeId = wbsNodeId;

  String? _wbsCode;
  String? get wbsCode => _$this._wbsCode;
  set wbsCode(String? wbsCode) => _$this._wbsCode = wbsCode;

  String? _workOrderId;
  String? get workOrderId => _$this._workOrderId;
  set workOrderId(String? workOrderId) => _$this._workOrderId = workOrderId;

  String? _workOrderNumber;
  String? get workOrderNumber => _$this._workOrderNumber;
  set workOrderNumber(String? workOrderNumber) =>
      _$this._workOrderNumber = workOrderNumber;

  String? _taskDescription;
  String? get taskDescription => _$this._taskDescription;
  set taskDescription(String? taskDescription) =>
      _$this._taskDescription = taskDescription;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  bool? _isOvertimeWarning;
  bool? get isOvertimeWarning => _$this._isOvertimeWarning;
  set isOvertimeWarning(bool? isOvertimeWarning) =>
      _$this._isOvertimeWarning = isOvertimeWarning;

  ProjectPersonResponseBuilder? _createdBy;
  ProjectPersonResponseBuilder get createdBy =>
      _$this._createdBy ??= ProjectPersonResponseBuilder();
  set createdBy(ProjectPersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _submittedAtUtc;
  DateTime? get submittedAtUtc => _$this._submittedAtUtc;
  set submittedAtUtc(DateTime? submittedAtUtc) =>
      _$this._submittedAtUtc = submittedAtUtc;

  ProjectPersonResponseBuilder? _decidedBy;
  ProjectPersonResponseBuilder get decidedBy =>
      _$this._decidedBy ??= ProjectPersonResponseBuilder();
  set decidedBy(ProjectPersonResponseBuilder? decidedBy) =>
      _$this._decidedBy = decidedBy;

  DateTime? _decidedAtUtc;
  DateTime? get decidedAtUtc => _$this._decidedAtUtc;
  set decidedAtUtc(DateTime? decidedAtUtc) =>
      _$this._decidedAtUtc = decidedAtUtc;

  String? _decisionNote;
  String? get decisionNote => _$this._decisionNote;
  set decisionNote(String? decisionNote) => _$this._decisionNote = decisionNote;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  LaborTimesheetResponseBuilder() {
    LaborTimesheetResponse._defaults(this);
  }

  LaborTimesheetResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _worker = $v.worker?.toBuilder();
      _workDate = $v.workDate;
      _hours = $v.hours;
      _hourlyRate = $v.hourlyRate;
      _costAmount = $v.costAmount;
      _wbsNodeId = $v.wbsNodeId;
      _wbsCode = $v.wbsCode;
      _workOrderId = $v.workOrderId;
      _workOrderNumber = $v.workOrderNumber;
      _taskDescription = $v.taskDescription;
      _status = $v.status;
      _isOvertimeWarning = $v.isOvertimeWarning;
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _submittedAtUtc = $v.submittedAtUtc;
      _decidedBy = $v.decidedBy?.toBuilder();
      _decidedAtUtc = $v.decidedAtUtc;
      _decisionNote = $v.decisionNote;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(LaborTimesheetResponse other) {
    _$v = other as _$LaborTimesheetResponse;
  }

  @override
  void update(void Function(LaborTimesheetResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  LaborTimesheetResponse build() => _build();

  _$LaborTimesheetResponse _build() {
    _$LaborTimesheetResponse _$result;
    try {
      _$result = _$v ??
          _$LaborTimesheetResponse._(
            id: id,
            number: number,
            worker: _worker?.build(),
            workDate: workDate,
            hours: hours,
            hourlyRate: hourlyRate,
            costAmount: costAmount,
            wbsNodeId: wbsNodeId,
            wbsCode: wbsCode,
            workOrderId: workOrderId,
            workOrderNumber: workOrderNumber,
            taskDescription: taskDescription,
            status: status,
            isOvertimeWarning: isOvertimeWarning,
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            submittedAtUtc: submittedAtUtc,
            decidedBy: _decidedBy?.build(),
            decidedAtUtc: decidedAtUtc,
            decisionNote: decisionNote,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'worker';
        _worker?.build();

        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'decidedBy';
        _decidedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'LaborTimesheetResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
