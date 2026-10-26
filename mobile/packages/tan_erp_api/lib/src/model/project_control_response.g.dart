// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_control_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectControlResponse extends ProjectControlResponse {
  @override
  final String? projectId;
  @override
  final String? status;
  @override
  final String? statusReason;
  @override
  final Date? plannedStartDate;
  @override
  final Date? plannedEndDate;
  @override
  final DateTime? activatedAtUtc;
  @override
  final DateTime? completedAtUtc;
  @override
  final String? rowVersion;
  @override
  final ProjectBudgetResponse? budget;
  @override
  final ProjectContractResponse? contract;
  @override
  final ProjectProgressResponse? progress;
  @override
  final BuiltList<ProjectMilestoneResponse>? milestones;
  @override
  final BuiltList<ProjectChangeOrderResponse>? changeOrders;
  @override
  final BuiltList<ProjectStatusHistoryResponse>? history;

  factory _$ProjectControlResponse(
          [void Function(ProjectControlResponseBuilder)? updates]) =>
      (ProjectControlResponseBuilder()..update(updates))._build();

  _$ProjectControlResponse._(
      {this.projectId,
      this.status,
      this.statusReason,
      this.plannedStartDate,
      this.plannedEndDate,
      this.activatedAtUtc,
      this.completedAtUtc,
      this.rowVersion,
      this.budget,
      this.contract,
      this.progress,
      this.milestones,
      this.changeOrders,
      this.history})
      : super._();
  @override
  ProjectControlResponse rebuild(
          void Function(ProjectControlResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectControlResponseBuilder toBuilder() =>
      ProjectControlResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectControlResponse &&
        projectId == other.projectId &&
        status == other.status &&
        statusReason == other.statusReason &&
        plannedStartDate == other.plannedStartDate &&
        plannedEndDate == other.plannedEndDate &&
        activatedAtUtc == other.activatedAtUtc &&
        completedAtUtc == other.completedAtUtc &&
        rowVersion == other.rowVersion &&
        budget == other.budget &&
        contract == other.contract &&
        progress == other.progress &&
        milestones == other.milestones &&
        changeOrders == other.changeOrders &&
        history == other.history;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, statusReason.hashCode);
    _$hash = $jc(_$hash, plannedStartDate.hashCode);
    _$hash = $jc(_$hash, plannedEndDate.hashCode);
    _$hash = $jc(_$hash, activatedAtUtc.hashCode);
    _$hash = $jc(_$hash, completedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, budget.hashCode);
    _$hash = $jc(_$hash, contract.hashCode);
    _$hash = $jc(_$hash, progress.hashCode);
    _$hash = $jc(_$hash, milestones.hashCode);
    _$hash = $jc(_$hash, changeOrders.hashCode);
    _$hash = $jc(_$hash, history.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectControlResponse')
          ..add('projectId', projectId)
          ..add('status', status)
          ..add('statusReason', statusReason)
          ..add('plannedStartDate', plannedStartDate)
          ..add('plannedEndDate', plannedEndDate)
          ..add('activatedAtUtc', activatedAtUtc)
          ..add('completedAtUtc', completedAtUtc)
          ..add('rowVersion', rowVersion)
          ..add('budget', budget)
          ..add('contract', contract)
          ..add('progress', progress)
          ..add('milestones', milestones)
          ..add('changeOrders', changeOrders)
          ..add('history', history))
        .toString();
  }
}

class ProjectControlResponseBuilder
    implements Builder<ProjectControlResponse, ProjectControlResponseBuilder> {
  _$ProjectControlResponse? _$v;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _statusReason;
  String? get statusReason => _$this._statusReason;
  set statusReason(String? statusReason) => _$this._statusReason = statusReason;

  Date? _plannedStartDate;
  Date? get plannedStartDate => _$this._plannedStartDate;
  set plannedStartDate(Date? plannedStartDate) =>
      _$this._plannedStartDate = plannedStartDate;

  Date? _plannedEndDate;
  Date? get plannedEndDate => _$this._plannedEndDate;
  set plannedEndDate(Date? plannedEndDate) =>
      _$this._plannedEndDate = plannedEndDate;

  DateTime? _activatedAtUtc;
  DateTime? get activatedAtUtc => _$this._activatedAtUtc;
  set activatedAtUtc(DateTime? activatedAtUtc) =>
      _$this._activatedAtUtc = activatedAtUtc;

  DateTime? _completedAtUtc;
  DateTime? get completedAtUtc => _$this._completedAtUtc;
  set completedAtUtc(DateTime? completedAtUtc) =>
      _$this._completedAtUtc = completedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ProjectBudgetResponseBuilder? _budget;
  ProjectBudgetResponseBuilder get budget =>
      _$this._budget ??= ProjectBudgetResponseBuilder();
  set budget(ProjectBudgetResponseBuilder? budget) => _$this._budget = budget;

  ProjectContractResponseBuilder? _contract;
  ProjectContractResponseBuilder get contract =>
      _$this._contract ??= ProjectContractResponseBuilder();
  set contract(ProjectContractResponseBuilder? contract) =>
      _$this._contract = contract;

  ProjectProgressResponseBuilder? _progress;
  ProjectProgressResponseBuilder get progress =>
      _$this._progress ??= ProjectProgressResponseBuilder();
  set progress(ProjectProgressResponseBuilder? progress) =>
      _$this._progress = progress;

  ListBuilder<ProjectMilestoneResponse>? _milestones;
  ListBuilder<ProjectMilestoneResponse> get milestones =>
      _$this._milestones ??= ListBuilder<ProjectMilestoneResponse>();
  set milestones(ListBuilder<ProjectMilestoneResponse>? milestones) =>
      _$this._milestones = milestones;

  ListBuilder<ProjectChangeOrderResponse>? _changeOrders;
  ListBuilder<ProjectChangeOrderResponse> get changeOrders =>
      _$this._changeOrders ??= ListBuilder<ProjectChangeOrderResponse>();
  set changeOrders(ListBuilder<ProjectChangeOrderResponse>? changeOrders) =>
      _$this._changeOrders = changeOrders;

  ListBuilder<ProjectStatusHistoryResponse>? _history;
  ListBuilder<ProjectStatusHistoryResponse> get history =>
      _$this._history ??= ListBuilder<ProjectStatusHistoryResponse>();
  set history(ListBuilder<ProjectStatusHistoryResponse>? history) =>
      _$this._history = history;

  ProjectControlResponseBuilder() {
    ProjectControlResponse._defaults(this);
  }

  ProjectControlResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _projectId = $v.projectId;
      _status = $v.status;
      _statusReason = $v.statusReason;
      _plannedStartDate = $v.plannedStartDate;
      _plannedEndDate = $v.plannedEndDate;
      _activatedAtUtc = $v.activatedAtUtc;
      _completedAtUtc = $v.completedAtUtc;
      _rowVersion = $v.rowVersion;
      _budget = $v.budget?.toBuilder();
      _contract = $v.contract?.toBuilder();
      _progress = $v.progress?.toBuilder();
      _milestones = $v.milestones?.toBuilder();
      _changeOrders = $v.changeOrders?.toBuilder();
      _history = $v.history?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectControlResponse other) {
    _$v = other as _$ProjectControlResponse;
  }

  @override
  void update(void Function(ProjectControlResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectControlResponse build() => _build();

  _$ProjectControlResponse _build() {
    _$ProjectControlResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectControlResponse._(
            projectId: projectId,
            status: status,
            statusReason: statusReason,
            plannedStartDate: plannedStartDate,
            plannedEndDate: plannedEndDate,
            activatedAtUtc: activatedAtUtc,
            completedAtUtc: completedAtUtc,
            rowVersion: rowVersion,
            budget: _budget?.build(),
            contract: _contract?.build(),
            progress: _progress?.build(),
            milestones: _milestones?.build(),
            changeOrders: _changeOrders?.build(),
            history: _history?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'budget';
        _budget?.build();
        _$failedField = 'contract';
        _contract?.build();
        _$failedField = 'progress';
        _progress?.build();
        _$failedField = 'milestones';
        _milestones?.build();
        _$failedField = 'changeOrders';
        _changeOrders?.build();
        _$failedField = 'history';
        _history?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectControlResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
