// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pipeline_summary_metrics_response_report_section_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PipelineSummaryMetricsResponseReportSectionResponse
    extends PipelineSummaryMetricsResponseReportSectionResponse {
  @override
  final bool? permitted;
  @override
  final PipelineSummaryMetricsResponse? summary;

  factory _$PipelineSummaryMetricsResponseReportSectionResponse(
          [void Function(
                  PipelineSummaryMetricsResponseReportSectionResponseBuilder)?
              updates]) =>
      (PipelineSummaryMetricsResponseReportSectionResponseBuilder()
            ..update(updates))
          ._build();

  _$PipelineSummaryMetricsResponseReportSectionResponse._(
      {this.permitted, this.summary})
      : super._();
  @override
  PipelineSummaryMetricsResponseReportSectionResponse rebuild(
          void Function(
                  PipelineSummaryMetricsResponseReportSectionResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PipelineSummaryMetricsResponseReportSectionResponseBuilder toBuilder() =>
      PipelineSummaryMetricsResponseReportSectionResponseBuilder()
        ..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PipelineSummaryMetricsResponseReportSectionResponse &&
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
            r'PipelineSummaryMetricsResponseReportSectionResponse')
          ..add('permitted', permitted)
          ..add('summary', summary))
        .toString();
  }
}

class PipelineSummaryMetricsResponseReportSectionResponseBuilder
    implements
        Builder<PipelineSummaryMetricsResponseReportSectionResponse,
            PipelineSummaryMetricsResponseReportSectionResponseBuilder> {
  _$PipelineSummaryMetricsResponseReportSectionResponse? _$v;

  bool? _permitted;
  bool? get permitted => _$this._permitted;
  set permitted(bool? permitted) => _$this._permitted = permitted;

  PipelineSummaryMetricsResponseBuilder? _summary;
  PipelineSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= PipelineSummaryMetricsResponseBuilder();
  set summary(PipelineSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  PipelineSummaryMetricsResponseReportSectionResponseBuilder() {
    PipelineSummaryMetricsResponseReportSectionResponse._defaults(this);
  }

  PipelineSummaryMetricsResponseReportSectionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _permitted = $v.permitted;
      _summary = $v.summary?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PipelineSummaryMetricsResponseReportSectionResponse other) {
    _$v = other as _$PipelineSummaryMetricsResponseReportSectionResponse;
  }

  @override
  void update(
      void Function(PipelineSummaryMetricsResponseReportSectionResponseBuilder)?
          updates) {
    if (updates != null) updates(this);
  }

  @override
  PipelineSummaryMetricsResponseReportSectionResponse build() => _build();

  _$PipelineSummaryMetricsResponseReportSectionResponse _build() {
    _$PipelineSummaryMetricsResponseReportSectionResponse _$result;
    try {
      _$result = _$v ??
          _$PipelineSummaryMetricsResponseReportSectionResponse._(
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
            r'PipelineSummaryMetricsResponseReportSectionResponse',
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
