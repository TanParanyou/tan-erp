// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_summary_metrics_response_report_section_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationSummaryMetricsResponseReportSectionResponse
    extends QuotationSummaryMetricsResponseReportSectionResponse {
  @override
  final bool? permitted;
  @override
  final QuotationSummaryMetricsResponse? summary;

  factory _$QuotationSummaryMetricsResponseReportSectionResponse(
          [void Function(
                  QuotationSummaryMetricsResponseReportSectionResponseBuilder)?
              updates]) =>
      (QuotationSummaryMetricsResponseReportSectionResponseBuilder()
            ..update(updates))
          ._build();

  _$QuotationSummaryMetricsResponseReportSectionResponse._(
      {this.permitted, this.summary})
      : super._();
  @override
  QuotationSummaryMetricsResponseReportSectionResponse rebuild(
          void Function(
                  QuotationSummaryMetricsResponseReportSectionResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationSummaryMetricsResponseReportSectionResponseBuilder toBuilder() =>
      QuotationSummaryMetricsResponseReportSectionResponseBuilder()
        ..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationSummaryMetricsResponseReportSectionResponse &&
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
            r'QuotationSummaryMetricsResponseReportSectionResponse')
          ..add('permitted', permitted)
          ..add('summary', summary))
        .toString();
  }
}

class QuotationSummaryMetricsResponseReportSectionResponseBuilder
    implements
        Builder<QuotationSummaryMetricsResponseReportSectionResponse,
            QuotationSummaryMetricsResponseReportSectionResponseBuilder> {
  _$QuotationSummaryMetricsResponseReportSectionResponse? _$v;

  bool? _permitted;
  bool? get permitted => _$this._permitted;
  set permitted(bool? permitted) => _$this._permitted = permitted;

  QuotationSummaryMetricsResponseBuilder? _summary;
  QuotationSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= QuotationSummaryMetricsResponseBuilder();
  set summary(QuotationSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  QuotationSummaryMetricsResponseReportSectionResponseBuilder() {
    QuotationSummaryMetricsResponseReportSectionResponse._defaults(this);
  }

  QuotationSummaryMetricsResponseReportSectionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _permitted = $v.permitted;
      _summary = $v.summary?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationSummaryMetricsResponseReportSectionResponse other) {
    _$v = other as _$QuotationSummaryMetricsResponseReportSectionResponse;
  }

  @override
  void update(
      void Function(
              QuotationSummaryMetricsResponseReportSectionResponseBuilder)?
          updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationSummaryMetricsResponseReportSectionResponse build() => _build();

  _$QuotationSummaryMetricsResponseReportSectionResponse _build() {
    _$QuotationSummaryMetricsResponseReportSectionResponse _$result;
    try {
      _$result = _$v ??
          _$QuotationSummaryMetricsResponseReportSectionResponse._(
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
            r'QuotationSummaryMetricsResponseReportSectionResponse',
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
