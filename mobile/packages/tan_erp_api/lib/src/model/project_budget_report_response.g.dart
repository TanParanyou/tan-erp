// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_budget_report_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectBudgetReportResponse extends ProjectBudgetReportResponse {
  @override
  final ProjectBudgetSummaryMetricsResponse? summary;
  @override
  final BuiltList<ProjectBudgetReportRowResponse>? rows;

  factory _$ProjectBudgetReportResponse(
          [void Function(ProjectBudgetReportResponseBuilder)? updates]) =>
      (ProjectBudgetReportResponseBuilder()..update(updates))._build();

  _$ProjectBudgetReportResponse._({this.summary, this.rows}) : super._();
  @override
  ProjectBudgetReportResponse rebuild(
          void Function(ProjectBudgetReportResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectBudgetReportResponseBuilder toBuilder() =>
      ProjectBudgetReportResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectBudgetReportResponse &&
        summary == other.summary &&
        rows == other.rows;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, summary.hashCode);
    _$hash = $jc(_$hash, rows.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectBudgetReportResponse')
          ..add('summary', summary)
          ..add('rows', rows))
        .toString();
  }
}

class ProjectBudgetReportResponseBuilder
    implements
        Builder<ProjectBudgetReportResponse,
            ProjectBudgetReportResponseBuilder> {
  _$ProjectBudgetReportResponse? _$v;

  ProjectBudgetSummaryMetricsResponseBuilder? _summary;
  ProjectBudgetSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= ProjectBudgetSummaryMetricsResponseBuilder();
  set summary(ProjectBudgetSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  ListBuilder<ProjectBudgetReportRowResponse>? _rows;
  ListBuilder<ProjectBudgetReportRowResponse> get rows =>
      _$this._rows ??= ListBuilder<ProjectBudgetReportRowResponse>();
  set rows(ListBuilder<ProjectBudgetReportRowResponse>? rows) =>
      _$this._rows = rows;

  ProjectBudgetReportResponseBuilder() {
    ProjectBudgetReportResponse._defaults(this);
  }

  ProjectBudgetReportResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _summary = $v.summary?.toBuilder();
      _rows = $v.rows?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectBudgetReportResponse other) {
    _$v = other as _$ProjectBudgetReportResponse;
  }

  @override
  void update(void Function(ProjectBudgetReportResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectBudgetReportResponse build() => _build();

  _$ProjectBudgetReportResponse _build() {
    _$ProjectBudgetReportResponse _$result;
    try {
      _$result = _$v ??
          _$ProjectBudgetReportResponse._(
            summary: _summary?.build(),
            rows: _rows?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'summary';
        _summary?.build();
        _$failedField = 'rows';
        _rows?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ProjectBudgetReportResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
