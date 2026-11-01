// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_value_summary_metrics_response_report_section_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockValueSummaryMetricsResponseReportSectionResponse
    extends StockValueSummaryMetricsResponseReportSectionResponse {
  @override
  final bool? permitted;
  @override
  final StockValueSummaryMetricsResponse? summary;

  factory _$StockValueSummaryMetricsResponseReportSectionResponse(
          [void Function(
                  StockValueSummaryMetricsResponseReportSectionResponseBuilder)?
              updates]) =>
      (StockValueSummaryMetricsResponseReportSectionResponseBuilder()
            ..update(updates))
          ._build();

  _$StockValueSummaryMetricsResponseReportSectionResponse._(
      {this.permitted, this.summary})
      : super._();
  @override
  StockValueSummaryMetricsResponseReportSectionResponse rebuild(
          void Function(
                  StockValueSummaryMetricsResponseReportSectionResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockValueSummaryMetricsResponseReportSectionResponseBuilder toBuilder() =>
      StockValueSummaryMetricsResponseReportSectionResponseBuilder()
        ..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockValueSummaryMetricsResponseReportSectionResponse &&
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
            r'StockValueSummaryMetricsResponseReportSectionResponse')
          ..add('permitted', permitted)
          ..add('summary', summary))
        .toString();
  }
}

class StockValueSummaryMetricsResponseReportSectionResponseBuilder
    implements
        Builder<StockValueSummaryMetricsResponseReportSectionResponse,
            StockValueSummaryMetricsResponseReportSectionResponseBuilder> {
  _$StockValueSummaryMetricsResponseReportSectionResponse? _$v;

  bool? _permitted;
  bool? get permitted => _$this._permitted;
  set permitted(bool? permitted) => _$this._permitted = permitted;

  StockValueSummaryMetricsResponseBuilder? _summary;
  StockValueSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= StockValueSummaryMetricsResponseBuilder();
  set summary(StockValueSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  StockValueSummaryMetricsResponseReportSectionResponseBuilder() {
    StockValueSummaryMetricsResponseReportSectionResponse._defaults(this);
  }

  StockValueSummaryMetricsResponseReportSectionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _permitted = $v.permitted;
      _summary = $v.summary?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockValueSummaryMetricsResponseReportSectionResponse other) {
    _$v = other as _$StockValueSummaryMetricsResponseReportSectionResponse;
  }

  @override
  void update(
      void Function(
              StockValueSummaryMetricsResponseReportSectionResponseBuilder)?
          updates) {
    if (updates != null) updates(this);
  }

  @override
  StockValueSummaryMetricsResponseReportSectionResponse build() => _build();

  _$StockValueSummaryMetricsResponseReportSectionResponse _build() {
    _$StockValueSummaryMetricsResponseReportSectionResponse _$result;
    try {
      _$result = _$v ??
          _$StockValueSummaryMetricsResponseReportSectionResponse._(
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
            r'StockValueSummaryMetricsResponseReportSectionResponse',
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
