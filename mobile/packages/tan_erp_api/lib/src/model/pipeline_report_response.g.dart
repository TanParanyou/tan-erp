// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pipeline_report_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PipelineReportResponse extends PipelineReportResponse {
  @override
  final PipelineSummaryMetricsResponse? summary;
  @override
  final BuiltList<PipelineStageMetricsResponse>? stages;

  factory _$PipelineReportResponse(
          [void Function(PipelineReportResponseBuilder)? updates]) =>
      (PipelineReportResponseBuilder()..update(updates))._build();

  _$PipelineReportResponse._({this.summary, this.stages}) : super._();
  @override
  PipelineReportResponse rebuild(
          void Function(PipelineReportResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PipelineReportResponseBuilder toBuilder() =>
      PipelineReportResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PipelineReportResponse &&
        summary == other.summary &&
        stages == other.stages;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, summary.hashCode);
    _$hash = $jc(_$hash, stages.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PipelineReportResponse')
          ..add('summary', summary)
          ..add('stages', stages))
        .toString();
  }
}

class PipelineReportResponseBuilder
    implements Builder<PipelineReportResponse, PipelineReportResponseBuilder> {
  _$PipelineReportResponse? _$v;

  PipelineSummaryMetricsResponseBuilder? _summary;
  PipelineSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= PipelineSummaryMetricsResponseBuilder();
  set summary(PipelineSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  ListBuilder<PipelineStageMetricsResponse>? _stages;
  ListBuilder<PipelineStageMetricsResponse> get stages =>
      _$this._stages ??= ListBuilder<PipelineStageMetricsResponse>();
  set stages(ListBuilder<PipelineStageMetricsResponse>? stages) =>
      _$this._stages = stages;

  PipelineReportResponseBuilder() {
    PipelineReportResponse._defaults(this);
  }

  PipelineReportResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _summary = $v.summary?.toBuilder();
      _stages = $v.stages?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PipelineReportResponse other) {
    _$v = other as _$PipelineReportResponse;
  }

  @override
  void update(void Function(PipelineReportResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PipelineReportResponse build() => _build();

  _$PipelineReportResponse _build() {
    _$PipelineReportResponse _$result;
    try {
      _$result = _$v ??
          _$PipelineReportResponse._(
            summary: _summary?.build(),
            stages: _stages?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'summary';
        _summary?.build();
        _$failedField = 'stages';
        _stages?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PipelineReportResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
