// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_budget_summary_metrics_response_report_section_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBudgetSummaryMetricsResponseReportSectionResponse
    extends ProjectBudgetSummaryMetricsResponseReportSectionResponse {
  @override
  final bool? permitted;
  @override
  final ProjectBudgetSummaryMetricsResponse? summary;

  factory _$ProjectBudgetSummaryMetricsResponseReportSectionResponse(
          [void Function(
                  ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder)?
              updates]) =>
      (ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder()
            ..update(updates))
          ._build();

  _$ProjectBudgetSummaryMetricsResponseReportSectionResponse._(
      {this.permitted, this.summary})
      : super._();
  @override
  ProjectBudgetSummaryMetricsResponseReportSectionResponse rebuild(
          void Function(
                  ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder toBuilder() =>
      ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder()
        ..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBudgetSummaryMetricsResponseReportSectionResponse &&
        permitted == other.permitted &&
        summary == other.summary;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, permitted.hashCode);
    _$hash = $jc(_$hash, summary.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(
            r'ProjectBudgetSummaryMetricsResponseReportSectionResponse')
          ..add('permitted', permitted)
          ..add('summary', summary))
        .toString();
  }
}

class ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder
    implements
        Builder<ProjectBudgetSummaryMetricsResponseReportSectionResponse,
            ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder> {
  _$ProjectBudgetSummaryMetricsResponseReportSectionResponse? _$v;

  bool? _permitted;
  bool? get permitted => _$this._permitted;
  set permitted(bool? permitted) => _$this._permitted = permitted;

  ProjectBudgetSummaryMetricsResponseBuilder? _summary;
  ProjectBudgetSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= ProjectBudgetSummaryMetricsResponseBuilder();
  set summary(ProjectBudgetSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder() {
    ProjectBudgetSummaryMetricsResponseReportSectionResponse._defaults(this);
  }

  ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _permitted = $v.permitted;
      _summary = $v.summary?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBudgetSummaryMetricsResponseReportSectionResponse other) {
    _$v = other as _$ProjectBudgetSummaryMetricsResponseReportSectionResponse;
  }

  @override
  void update(
      void Function(
              ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder)?
          updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBudgetSummaryMetricsResponseReportSectionResponse build() => _build();

  _$ProjectBudgetSummaryMetricsResponseReportSectionResponse _build() {
    _$ProjectBudgetSummaryMetricsResponseReportSectionResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectBudgetSummaryMetricsResponseReportSectionResponse._(
            permitted: permitted,
            summary: _summary?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'summary';
        _summary?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectBudgetSummaryMetricsResponseReportSectionResponse',
            _$failedField,
            e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
