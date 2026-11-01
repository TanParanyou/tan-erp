// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_budget_summary_metrics_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBudgetSummaryMetricsResponse
    extends ProjectBudgetSummaryMetricsResponse {
  @override
  final int? projectCount;
  @override
  final int? overBudgetCount;
  @override
  final double? totalBaselineBudget;
  @override
  final double? totalApprovedChangeOrderBudget;
  @override
  final double? totalCurrentBudget;
  @override
  final double? totalCommittedAmount;
  @override
  final double? totalActualAmount;
  @override
  final double? totalVarianceAmount;

  factory _$ProjectBudgetSummaryMetricsResponse(
          [void Function(ProjectBudgetSummaryMetricsResponseBuilder)?
              updates]) =>
      (ProjectBudgetSummaryMetricsResponseBuilder()..update(updates))._build();

  _$ProjectBudgetSummaryMetricsResponse._(
      {this.projectCount,
      this.overBudgetCount,
      this.totalBaselineBudget,
      this.totalApprovedChangeOrderBudget,
      this.totalCurrentBudget,
      this.totalCommittedAmount,
      this.totalActualAmount,
      this.totalVarianceAmount})
      : super._();
  @override
  ProjectBudgetSummaryMetricsResponse rebuild(
          void Function(ProjectBudgetSummaryMetricsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBudgetSummaryMetricsResponseBuilder toBuilder() =>
      ProjectBudgetSummaryMetricsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBudgetSummaryMetricsResponse &&
        projectCount == other.projectCount &&
        overBudgetCount == other.overBudgetCount &&
        totalBaselineBudget == other.totalBaselineBudget &&
        totalApprovedChangeOrderBudget ==
            other.totalApprovedChangeOrderBudget &&
        totalCurrentBudget == other.totalCurrentBudget &&
        totalCommittedAmount == other.totalCommittedAmount &&
        totalActualAmount == other.totalActualAmount &&
        totalVarianceAmount == other.totalVarianceAmount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, projectCount.hashCode);
    _$hash = $jc(_$hash, overBudgetCount.hashCode);
    _$hash = $jc(_$hash, totalBaselineBudget.hashCode);
    _$hash = $jc(_$hash, totalApprovedChangeOrderBudget.hashCode);
    _$hash = $jc(_$hash, totalCurrentBudget.hashCode);
    _$hash = $jc(_$hash, totalCommittedAmount.hashCode);
    _$hash = $jc(_$hash, totalActualAmount.hashCode);
    _$hash = $jc(_$hash, totalVarianceAmount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectBudgetSummaryMetricsResponse')
          ..add('projectCount', projectCount)
          ..add('overBudgetCount', overBudgetCount)
          ..add('totalBaselineBudget', totalBaselineBudget)
          ..add(
              'totalApprovedChangeOrderBudget', totalApprovedChangeOrderBudget)
          ..add('totalCurrentBudget', totalCurrentBudget)
          ..add('totalCommittedAmount', totalCommittedAmount)
          ..add('totalActualAmount', totalActualAmount)
          ..add('totalVarianceAmount', totalVarianceAmount))
        .toString();
  }
}

class ProjectBudgetSummaryMetricsResponseBuilder
    implements
        Builder<ProjectBudgetSummaryMetricsResponse,
            ProjectBudgetSummaryMetricsResponseBuilder> {
  _$ProjectBudgetSummaryMetricsResponse? _$v;

  int? _projectCount;
  int? get projectCount => _$this._projectCount;
  set projectCount(int? projectCount) => _$this._projectCount = projectCount;

  int? _overBudgetCount;
  int? get overBudgetCount => _$this._overBudgetCount;
  set overBudgetCount(int? overBudgetCount) =>
      _$this._overBudgetCount = overBudgetCount;

  double? _totalBaselineBudget;
  double? get totalBaselineBudget => _$this._totalBaselineBudget;
  set totalBaselineBudget(double? totalBaselineBudget) =>
      _$this._totalBaselineBudget = totalBaselineBudget;

  double? _totalApprovedChangeOrderBudget;
  double? get totalApprovedChangeOrderBudget =>
      _$this._totalApprovedChangeOrderBudget;
  set totalApprovedChangeOrderBudget(double? totalApprovedChangeOrderBudget) =>
      _$this._totalApprovedChangeOrderBudget = totalApprovedChangeOrderBudget;

  double? _totalCurrentBudget;
  double? get totalCurrentBudget => _$this._totalCurrentBudget;
  set totalCurrentBudget(double? totalCurrentBudget) =>
      _$this._totalCurrentBudget = totalCurrentBudget;

  double? _totalCommittedAmount;
  double? get totalCommittedAmount => _$this._totalCommittedAmount;
  set totalCommittedAmount(double? totalCommittedAmount) =>
      _$this._totalCommittedAmount = totalCommittedAmount;

  double? _totalActualAmount;
  double? get totalActualAmount => _$this._totalActualAmount;
  set totalActualAmount(double? totalActualAmount) =>
      _$this._totalActualAmount = totalActualAmount;

  double? _totalVarianceAmount;
  double? get totalVarianceAmount => _$this._totalVarianceAmount;
  set totalVarianceAmount(double? totalVarianceAmount) =>
      _$this._totalVarianceAmount = totalVarianceAmount;

  ProjectBudgetSummaryMetricsResponseBuilder() {
    ProjectBudgetSummaryMetricsResponse._defaults(this);
  }

  ProjectBudgetSummaryMetricsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _projectCount = $v.projectCount;
      _overBudgetCount = $v.overBudgetCount;
      _totalBaselineBudget = $v.totalBaselineBudget;
      _totalApprovedChangeOrderBudget = $v.totalApprovedChangeOrderBudget;
      _totalCurrentBudget = $v.totalCurrentBudget;
      _totalCommittedAmount = $v.totalCommittedAmount;
      _totalActualAmount = $v.totalActualAmount;
      _totalVarianceAmount = $v.totalVarianceAmount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBudgetSummaryMetricsResponse other) {
    _$v = other as _$ProjectBudgetSummaryMetricsResponse;
  }

  @override
  void update(
      void Function(ProjectBudgetSummaryMetricsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBudgetSummaryMetricsResponse build() => _build();

  _$ProjectBudgetSummaryMetricsResponse _build() {
    final _$result = _$v ??
        _$ProjectBudgetSummaryMetricsResponse._(
          projectCount: projectCount,
          overBudgetCount: overBudgetCount,
          totalBaselineBudget: totalBaselineBudget,
          totalApprovedChangeOrderBudget: totalApprovedChangeOrderBudget,
          totalCurrentBudget: totalCurrentBudget,
          totalCommittedAmount: totalCommittedAmount,
          totalActualAmount: totalActualAmount,
          totalVarianceAmount: totalVarianceAmount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
