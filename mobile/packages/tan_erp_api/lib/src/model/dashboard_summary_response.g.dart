// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'dashboard_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$DashboardSummaryResponse extends DashboardSummaryResponse {
  @override
  final DateTime? generatedAtUtc;
  @override
  final String? effectiveBranchId;
  @override
  final PipelineSummaryMetricsResponseReportSectionResponse? pipeline;
  @override
  final QuotationSummaryMetricsResponseReportSectionResponse? quotations;
  @override
  final ProjectBudgetSummaryMetricsResponseReportSectionResponse?
      projectsBudget;
  @override
  final StockValueSummaryMetricsResponseReportSectionResponse? stockValue;
  @override
  final ArAgingSummaryMetricsResponseReportSectionResponse? arAging;
  @override
  final ServiceSlaSummaryMetricsResponseReportSectionResponse? serviceSla;

  factory _$DashboardSummaryResponse(
          [void Function(DashboardSummaryResponseBuilder)? updates]) =>
      (DashboardSummaryResponseBuilder()..update(updates))._build();

  _$DashboardSummaryResponse._(
      {this.generatedAtUtc,
      this.effectiveBranchId,
      this.pipeline,
      this.quotations,
      this.projectsBudget,
      this.stockValue,
      this.arAging,
      this.serviceSla})
      : super._();
  @override
  DashboardSummaryResponse rebuild(
          void Function(DashboardSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  DashboardSummaryResponseBuilder toBuilder() =>
      DashboardSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is DashboardSummaryResponse &&
        generatedAtUtc == other.generatedAtUtc &&
        effectiveBranchId == other.effectiveBranchId &&
        pipeline == other.pipeline &&
        quotations == other.quotations &&
        projectsBudget == other.projectsBudget &&
        stockValue == other.stockValue &&
        arAging == other.arAging &&
        serviceSla == other.serviceSla;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, generatedAtUtc.hashCode);
    _$hash = $jc(_$hash, effectiveBranchId.hashCode);
    _$hash = $jc(_$hash, pipeline.hashCode);
    _$hash = $jc(_$hash, quotations.hashCode);
    _$hash = $jc(_$hash, projectsBudget.hashCode);
    _$hash = $jc(_$hash, stockValue.hashCode);
    _$hash = $jc(_$hash, arAging.hashCode);
    _$hash = $jc(_$hash, serviceSla.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'DashboardSummaryResponse')
          ..add('generatedAtUtc', generatedAtUtc)
          ..add('effectiveBranchId', effectiveBranchId)
          ..add('pipeline', pipeline)
          ..add('quotations', quotations)
          ..add('projectsBudget', projectsBudget)
          ..add('stockValue', stockValue)
          ..add('arAging', arAging)
          ..add('serviceSla', serviceSla))
        .toString();
  }
}

class DashboardSummaryResponseBuilder
    implements
        Builder<DashboardSummaryResponse, DashboardSummaryResponseBuilder> {
  _$DashboardSummaryResponse? _$v;

  DateTime? _generatedAtUtc;
  DateTime? get generatedAtUtc => _$this._generatedAtUtc;
  set generatedAtUtc(DateTime? generatedAtUtc) =>
      _$this._generatedAtUtc = generatedAtUtc;

  String? _effectiveBranchId;
  String? get effectiveBranchId => _$this._effectiveBranchId;
  set effectiveBranchId(String? effectiveBranchId) =>
      _$this._effectiveBranchId = effectiveBranchId;

  PipelineSummaryMetricsResponseReportSectionResponseBuilder? _pipeline;
  PipelineSummaryMetricsResponseReportSectionResponseBuilder get pipeline =>
      _$this._pipeline ??=
          PipelineSummaryMetricsResponseReportSectionResponseBuilder();
  set pipeline(
          PipelineSummaryMetricsResponseReportSectionResponseBuilder?
              pipeline) =>
      _$this._pipeline = pipeline;

  QuotationSummaryMetricsResponseReportSectionResponseBuilder? _quotations;
  QuotationSummaryMetricsResponseReportSectionResponseBuilder get quotations =>
      _$this._quotations ??=
          QuotationSummaryMetricsResponseReportSectionResponseBuilder();
  set quotations(
          QuotationSummaryMetricsResponseReportSectionResponseBuilder?
              quotations) =>
      _$this._quotations = quotations;

  ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder?
      _projectsBudget;
  ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder
      get projectsBudget => _$this._projectsBudget ??=
          ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder();
  set projectsBudget(
          ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder?
              projectsBudget) =>
      _$this._projectsBudget = projectsBudget;

  StockValueSummaryMetricsResponseReportSectionResponseBuilder? _stockValue;
  StockValueSummaryMetricsResponseReportSectionResponseBuilder get stockValue =>
      _$this._stockValue ??=
          StockValueSummaryMetricsResponseReportSectionResponseBuilder();
  set stockValue(
          StockValueSummaryMetricsResponseReportSectionResponseBuilder?
              stockValue) =>
      _$this._stockValue = stockValue;

  ArAgingSummaryMetricsResponseReportSectionResponseBuilder? _arAging;
  ArAgingSummaryMetricsResponseReportSectionResponseBuilder get arAging =>
      _$this._arAging ??=
          ArAgingSummaryMetricsResponseReportSectionResponseBuilder();
  set arAging(
          ArAgingSummaryMetricsResponseReportSectionResponseBuilder? arAging) =>
      _$this._arAging = arAging;

  ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder? _serviceSla;
  ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder get serviceSla =>
      _$this._serviceSla ??=
          ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder();
  set serviceSla(
          ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder?
              serviceSla) =>
      _$this._serviceSla = serviceSla;

  DashboardSummaryResponseBuilder() {
    DashboardSummaryResponse._defaults(this);
  }

  DashboardSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _generatedAtUtc = $v.generatedAtUtc;
      _effectiveBranchId = $v.effectiveBranchId;
      _pipeline = $v.pipeline?.toBuilder();
      _quotations = $v.quotations?.toBuilder();
      _projectsBudget = $v.projectsBudget?.toBuilder();
      _stockValue = $v.stockValue?.toBuilder();
      _arAging = $v.arAging?.toBuilder();
      _serviceSla = $v.serviceSla?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(DashboardSummaryResponse other) {
    _$v = other as _$DashboardSummaryResponse;
  }

  @override
  void update(void Function(DashboardSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  DashboardSummaryResponse build() => _build();

  _$DashboardSummaryResponse _build() {
    _$DashboardSummaryResponse _$result;
    try {
      _$result = _$v ??
          _$DashboardSummaryResponse._(
            generatedAtUtc: generatedAtUtc,
            effectiveBranchId: effectiveBranchId,
            pipeline: _pipeline?.build(),
            quotations: _quotations?.build(),
            projectsBudget: _projectsBudget?.build(),
            stockValue: _stockValue?.build(),
            arAging: _arAging?.build(),
            serviceSla: _serviceSla?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'pipeline';
        _pipeline?.build();
        _$failedField = 'quotations';
        _quotations?.build();
        _$failedField = 'projectsBudget';
        _projectsBudget?.build();
        _$failedField = 'stockValue';
        _stockValue?.build();
        _$failedField = 'arAging';
        _arAging?.build();
        _$failedField = 'serviceSla';
        _serviceSla?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'DashboardSummaryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
