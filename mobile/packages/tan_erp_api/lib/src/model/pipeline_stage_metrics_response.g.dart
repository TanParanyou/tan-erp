// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pipeline_stage_metrics_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PipelineStageMetricsResponse extends PipelineStageMetricsResponse {
  @override
  final String? stage;
  @override
  final int? opportunityCount;
  @override
  final double? expectedBudgetTotal;

  factory _$PipelineStageMetricsResponse(
          [void Function(PipelineStageMetricsResponseBuilder)? updates]) =>
      (PipelineStageMetricsResponseBuilder()..update(updates))._build();

  _$PipelineStageMetricsResponse._(
      {this.stage, this.opportunityCount, this.expectedBudgetTotal})
      : super._();
  @override
  PipelineStageMetricsResponse rebuild(
          void Function(PipelineStageMetricsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PipelineStageMetricsResponseBuilder toBuilder() =>
      PipelineStageMetricsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PipelineStageMetricsResponse &&
        stage == other.stage &&
        opportunityCount == other.opportunityCount &&
        expectedBudgetTotal == other.expectedBudgetTotal;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, stage.hashCode);
    _$hash = $jc(_$hash, opportunityCount.hashCode);
    _$hash = $jc(_$hash, expectedBudgetTotal.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PipelineStageMetricsResponse')
          ..add('stage', stage)
          ..add('opportunityCount', opportunityCount)
          ..add('expectedBudgetTotal', expectedBudgetTotal))
        .toString();
  }
}

class PipelineStageMetricsResponseBuilder
    implements
        Builder<PipelineStageMetricsResponse,
            PipelineStageMetricsResponseBuilder> {
  _$PipelineStageMetricsResponse? _$v;

  String? _stage;
  String? get stage => _$this._stage;
  set stage(String? stage) => _$this._stage = stage;

  int? _opportunityCount;
  int? get opportunityCount => _$this._opportunityCount;
  set opportunityCount(int? opportunityCount) =>
      _$this._opportunityCount = opportunityCount;

  double? _expectedBudgetTotal;
  double? get expectedBudgetTotal => _$this._expectedBudgetTotal;
  set expectedBudgetTotal(double? expectedBudgetTotal) =>
      _$this._expectedBudgetTotal = expectedBudgetTotal;

  PipelineStageMetricsResponseBuilder() {
    PipelineStageMetricsResponse._defaults(this);
  }

  PipelineStageMetricsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _stage = $v.stage;
      _opportunityCount = $v.opportunityCount;
      _expectedBudgetTotal = $v.expectedBudgetTotal;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PipelineStageMetricsResponse other) {
    _$v = other as _$PipelineStageMetricsResponse;
  }

  @override
  void update(void Function(PipelineStageMetricsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PipelineStageMetricsResponse build() => _build();

  _$PipelineStageMetricsResponse _build() {
    final _$result = _$v ??
        _$PipelineStageMetricsResponse._(
          stage: stage,
          opportunityCount: opportunityCount,
          expectedBudgetTotal: expectedBudgetTotal,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
