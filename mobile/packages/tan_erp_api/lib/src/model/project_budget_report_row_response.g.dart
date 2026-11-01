// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_budget_report_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBudgetReportRowResponse extends ProjectBudgetReportRowResponse {
  @override
  final String? projectId;
  @override
  final String? projectCode;
  @override
  final String? projectName;
  @override
  final String? branchCode;
  @override
  final String? status;
  @override
  final double? baselineContractAmount;
  @override
  final double? baselineBudget;
  @override
  final double? approvedChangeOrderBudget;
  @override
  final double? currentBudget;
  @override
  final double? committedAmount;
  @override
  final double? actualAmount;
  @override
  final double? varianceAmount;
  @override
  final bool? isOverBudget;

  factory _$ProjectBudgetReportRowResponse(
          [void Function(ProjectBudgetReportRowResponseBuilder)? updates]) =>
      (ProjectBudgetReportRowResponseBuilder()..update(updates))._build();

  _$ProjectBudgetReportRowResponse._(
      {this.projectId,
      this.projectCode,
      this.projectName,
      this.branchCode,
      this.status,
      this.baselineContractAmount,
      this.baselineBudget,
      this.approvedChangeOrderBudget,
      this.currentBudget,
      this.committedAmount,
      this.actualAmount,
      this.varianceAmount,
      this.isOverBudget})
      : super._();
  @override
  ProjectBudgetReportRowResponse rebuild(
          void Function(ProjectBudgetReportRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBudgetReportRowResponseBuilder toBuilder() =>
      ProjectBudgetReportRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBudgetReportRowResponse &&
        projectId == other.projectId &&
        projectCode == other.projectCode &&
        projectName == other.projectName &&
        branchCode == other.branchCode &&
        status == other.status &&
        baselineContractAmount == other.baselineContractAmount &&
        baselineBudget == other.baselineBudget &&
        approvedChangeOrderBudget == other.approvedChangeOrderBudget &&
        currentBudget == other.currentBudget &&
        committedAmount == other.committedAmount &&
        actualAmount == other.actualAmount &&
        varianceAmount == other.varianceAmount &&
        isOverBudget == other.isOverBudget;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, projectName.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, baselineContractAmount.hashCode);
    _$hash = $jc(_$hash, baselineBudget.hashCode);
    _$hash = $jc(_$hash, approvedChangeOrderBudget.hashCode);
    _$hash = $jc(_$hash, currentBudget.hashCode);
    _$hash = $jc(_$hash, committedAmount.hashCode);
    _$hash = $jc(_$hash, actualAmount.hashCode);
    _$hash = $jc(_$hash, varianceAmount.hashCode);
    _$hash = $jc(_$hash, isOverBudget.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectBudgetReportRowResponse')
          ..add('projectId', projectId)
          ..add('projectCode', projectCode)
          ..add('projectName', projectName)
          ..add('branchCode', branchCode)
          ..add('status', status)
          ..add('baselineContractAmount', baselineContractAmount)
          ..add('baselineBudget', baselineBudget)
          ..add('approvedChangeOrderBudget', approvedChangeOrderBudget)
          ..add('currentBudget', currentBudget)
          ..add('committedAmount', committedAmount)
          ..add('actualAmount', actualAmount)
          ..add('varianceAmount', varianceAmount)
          ..add('isOverBudget', isOverBudget))
        .toString();
  }
}

class ProjectBudgetReportRowResponseBuilder
    implements
        Builder<ProjectBudgetReportRowResponse,
            ProjectBudgetReportRowResponseBuilder> {
  _$ProjectBudgetReportRowResponse? _$v;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  String? _projectName;
  String? get projectName => _$this._projectName;
  set projectName(String? projectName) => _$this._projectName = projectName;

  String? _branchCode;
  String? get branchCode => _$this._branchCode;
  set branchCode(String? branchCode) => _$this._branchCode = branchCode;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  double? _baselineContractAmount;
  double? get baselineContractAmount => _$this._baselineContractAmount;
  set baselineContractAmount(double? baselineContractAmount) =>
      _$this._baselineContractAmount = baselineContractAmount;

  double? _baselineBudget;
  double? get baselineBudget => _$this._baselineBudget;
  set baselineBudget(double? baselineBudget) =>
      _$this._baselineBudget = baselineBudget;

  double? _approvedChangeOrderBudget;
  double? get approvedChangeOrderBudget => _$this._approvedChangeOrderBudget;
  set approvedChangeOrderBudget(double? approvedChangeOrderBudget) =>
      _$this._approvedChangeOrderBudget = approvedChangeOrderBudget;

  double? _currentBudget;
  double? get currentBudget => _$this._currentBudget;
  set currentBudget(double? currentBudget) =>
      _$this._currentBudget = currentBudget;

  double? _committedAmount;
  double? get committedAmount => _$this._committedAmount;
  set committedAmount(double? committedAmount) =>
      _$this._committedAmount = committedAmount;

  double? _actualAmount;
  double? get actualAmount => _$this._actualAmount;
  set actualAmount(double? actualAmount) => _$this._actualAmount = actualAmount;

  double? _varianceAmount;
  double? get varianceAmount => _$this._varianceAmount;
  set varianceAmount(double? varianceAmount) =>
      _$this._varianceAmount = varianceAmount;

  bool? _isOverBudget;
  bool? get isOverBudget => _$this._isOverBudget;
  set isOverBudget(bool? isOverBudget) => _$this._isOverBudget = isOverBudget;

  ProjectBudgetReportRowResponseBuilder() {
    ProjectBudgetReportRowResponse._defaults(this);
  }

  ProjectBudgetReportRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _projectId = $v.projectId;
      _projectCode = $v.projectCode;
      _projectName = $v.projectName;
      _branchCode = $v.branchCode;
      _status = $v.status;
      _baselineContractAmount = $v.baselineContractAmount;
      _baselineBudget = $v.baselineBudget;
      _approvedChangeOrderBudget = $v.approvedChangeOrderBudget;
      _currentBudget = $v.currentBudget;
      _committedAmount = $v.committedAmount;
      _actualAmount = $v.actualAmount;
      _varianceAmount = $v.varianceAmount;
      _isOverBudget = $v.isOverBudget;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBudgetReportRowResponse other) {
    _$v = other as _$ProjectBudgetReportRowResponse;
  }

  @override
  void update(void Function(ProjectBudgetReportRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBudgetReportRowResponse build() => _build();

  _$ProjectBudgetReportRowResponse _build() {
    final _$result = _$v ??
        _$ProjectBudgetReportRowResponse._(
          projectId: projectId,
          projectCode: projectCode,
          projectName: projectName,
          branchCode: branchCode,
          status: status,
          baselineContractAmount: baselineContractAmount,
          baselineBudget: baselineBudget,
          approvedChangeOrderBudget: approvedChangeOrderBudget,
          currentBudget: currentBudget,
          committedAmount: committedAmount,
          actualAmount: actualAmount,
          varianceAmount: varianceAmount,
          isOverBudget: isOverBudget,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
