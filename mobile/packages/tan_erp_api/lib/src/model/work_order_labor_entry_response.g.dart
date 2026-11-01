// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_labor_entry_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderLaborEntryResponse extends WorkOrderLaborEntryResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final ProductionPersonResponse? worker;
  @override
  final Date? workDate;
  @override
  final double? hours;
  @override
  final double? hourlyRate;
  @override
  final double? costAmount;
  @override
  final String? taskDescription;
  @override
  final DateTime? decidedAtUtc;

  factory _$WorkOrderLaborEntryResponse(
          [void Function(WorkOrderLaborEntryResponseBuilder)? updates]) =>
      (WorkOrderLaborEntryResponseBuilder()..update(updates))._build();

  _$WorkOrderLaborEntryResponse._(
      {this.id,
      this.number,
      this.worker,
      this.workDate,
      this.hours,
      this.hourlyRate,
      this.costAmount,
      this.taskDescription,
      this.decidedAtUtc})
      : super._();
  @override
  WorkOrderLaborEntryResponse rebuild(
          void Function(WorkOrderLaborEntryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderLaborEntryResponseBuilder toBuilder() =>
      WorkOrderLaborEntryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderLaborEntryResponse &&
        id == other.id &&
        number == other.number &&
        worker == other.worker &&
        workDate == other.workDate &&
        hours == other.hours &&
        hourlyRate == other.hourlyRate &&
        costAmount == other.costAmount &&
        taskDescription == other.taskDescription &&
        decidedAtUtc == other.decidedAtUtc;
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
    _$hash = $jc(_$hash, taskDescription.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderLaborEntryResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('worker', worker)
          ..add('workDate', workDate)
          ..add('hours', hours)
          ..add('hourlyRate', hourlyRate)
          ..add('costAmount', costAmount)
          ..add('taskDescription', taskDescription)
          ..add('decidedAtUtc', decidedAtUtc))
        .toString();
  }
}

class WorkOrderLaborEntryResponseBuilder
    implements
        Builder<WorkOrderLaborEntryResponse,
            WorkOrderLaborEntryResponseBuilder> {
  _$WorkOrderLaborEntryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  ProductionPersonResponseBuilder? _worker;
  ProductionPersonResponseBuilder get worker =>
      _$this._worker ??= ProductionPersonResponseBuilder();
  set worker(ProductionPersonResponseBuilder? worker) =>
      _$this._worker = worker;

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

  String? _taskDescription;
  String? get taskDescription => _$this._taskDescription;
  set taskDescription(String? taskDescription) =>
      _$this._taskDescription = taskDescription;

  DateTime? _decidedAtUtc;
  DateTime? get decidedAtUtc => _$this._decidedAtUtc;
  set decidedAtUtc(DateTime? decidedAtUtc) =>
      _$this._decidedAtUtc = decidedAtUtc;

  WorkOrderLaborEntryResponseBuilder() {
    WorkOrderLaborEntryResponse._defaults(this);
  }

  WorkOrderLaborEntryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _worker = $v.worker?.toBuilder();
      _workDate = $v.workDate;
      _hours = $v.hours;
      _hourlyRate = $v.hourlyRate;
      _costAmount = $v.costAmount;
      _taskDescription = $v.taskDescription;
      _decidedAtUtc = $v.decidedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderLaborEntryResponse other) {
    _$v = other as _$WorkOrderLaborEntryResponse;
  }

  @override
  void update(void Function(WorkOrderLaborEntryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderLaborEntryResponse build() => _build();

  _$WorkOrderLaborEntryResponse _build() {
    _$WorkOrderLaborEntryResponse _$result;
    try {
      _$result = _$v ??
          _$WorkOrderLaborEntryResponse._(
            id: id,
            number: number,
            worker: _worker?.build(),
            workDate: workDate,
            hours: hours,
            hourlyRate: hourlyRate,
            costAmount: costAmount,
            taskDescription: taskDescription,
            decidedAtUtc: decidedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'worker';
        _worker?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'WorkOrderLaborEntryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
