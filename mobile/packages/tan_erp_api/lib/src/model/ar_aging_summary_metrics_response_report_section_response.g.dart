// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ar_aging_summary_metrics_response_report_section_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ArAgingSummaryMetricsResponseReportSectionResponse
    extends ArAgingSummaryMetricsResponseReportSectionResponse {
  @override
  final bool? permitted;
  @override
  final ArAgingSummaryMetricsResponse? summary;

  factory _$ArAgingSummaryMetricsResponseReportSectionResponse(
          [void Function(
                  ArAgingSummaryMetricsResponseReportSectionResponseBuilder)?
              updates]) =>
      (ArAgingSummaryMetricsResponseReportSectionResponseBuilder()
            ..update(updates))
          ._build();

  _$ArAgingSummaryMetricsResponseReportSectionResponse._(
      {this.permitted, this.summary})
      : super._();
  @override
  ArAgingSummaryMetricsResponseReportSectionResponse rebuild(
          void Function(
                  ArAgingSummaryMetricsResponseReportSectionResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ArAgingSummaryMetricsResponseReportSectionResponseBuilder toBuilder() =>
      ArAgingSummaryMetricsResponseReportSectionResponseBuilder()
        ..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ArAgingSummaryMetricsResponseReportSectionResponse &&
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
            r'ArAgingSummaryMetricsResponseReportSectionResponse')
          ..add('permitted', permitted)
          ..add('summary', summary))
        .toString();
  }
}

class ArAgingSummaryMetricsResponseReportSectionResponseBuilder
    implements
        Builder<ArAgingSummaryMetricsResponseReportSectionResponse,
            ArAgingSummaryMetricsResponseReportSectionResponseBuilder> {
  _$ArAgingSummaryMetricsResponseReportSectionResponse? _$v;

  bool? _permitted;
  bool? get permitted => _$this._permitted;
  set permitted(bool? permitted) => _$this._permitted = permitted;

  ArAgingSummaryMetricsResponseBuilder? _summary;
  ArAgingSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= ArAgingSummaryMetricsResponseBuilder();
  set summary(ArAgingSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  ArAgingSummaryMetricsResponseReportSectionResponseBuilder() {
    ArAgingSummaryMetricsResponseReportSectionResponse._defaults(this);
  }

  ArAgingSummaryMetricsResponseReportSectionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _permitted = $v.permitted;
      _summary = $v.summary?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ArAgingSummaryMetricsResponseReportSectionResponse other) {
    _$v = other as _$ArAgingSummaryMetricsResponseReportSectionResponse;
  }

  @override
  void update(
      void Function(ArAgingSummaryMetricsResponseReportSectionResponseBuilder)?
          updates) {
    if (updates != null) updates(this);
  }

  @override
  ArAgingSummaryMetricsResponseReportSectionResponse build() => _build();

  _$ArAgingSummaryMetricsResponseReportSectionResponse _build() {
    _$ArAgingSummaryMetricsResponseReportSectionResponse _$result;
    try {
      _$result = _$v ??
          _$ArAgingSummaryMetricsResponseReportSectionResponse._(
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
            r'ArAgingSummaryMetricsResponseReportSectionResponse',
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
