// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_sla_summary_metrics_response_report_section_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceSlaSummaryMetricsResponseReportSectionResponse
    extends ServiceSlaSummaryMetricsResponseReportSectionResponse {
  @override
  final bool? permitted;
  @override
  final ServiceSlaSummaryMetricsResponse? summary;

  factory _$ServiceSlaSummaryMetricsResponseReportSectionResponse(
          [void Function(
                  ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder)?
              updates]) =>
      (ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder()
            ..update(updates))
          ._build();

  _$ServiceSlaSummaryMetricsResponseReportSectionResponse._(
      {this.permitted, this.summary})
      : super._();
  @override
  ServiceSlaSummaryMetricsResponseReportSectionResponse rebuild(
          void Function(
                  ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder toBuilder() =>
      ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder()
        ..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceSlaSummaryMetricsResponseReportSectionResponse &&
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
            r'ServiceSlaSummaryMetricsResponseReportSectionResponse')
          ..add('permitted', permitted)
          ..add('summary', summary))
        .toString();
  }
}

class ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder
    implements
        Builder<ServiceSlaSummaryMetricsResponseReportSectionResponse,
            ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder> {
  _$ServiceSlaSummaryMetricsResponseReportSectionResponse? _$v;

  bool? _permitted;
  bool? get permitted => _$this._permitted;
  set permitted(bool? permitted) => _$this._permitted = permitted;

  ServiceSlaSummaryMetricsResponseBuilder? _summary;
  ServiceSlaSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= ServiceSlaSummaryMetricsResponseBuilder();
  set summary(ServiceSlaSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder() {
    ServiceSlaSummaryMetricsResponseReportSectionResponse._defaults(this);
  }

  ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _permitted = $v.permitted;
      _summary = $v.summary?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceSlaSummaryMetricsResponseReportSectionResponse other) {
    _$v = other as _$ServiceSlaSummaryMetricsResponseReportSectionResponse;
  }

  @override
  void update(
      void Function(
              ServiceSlaSummaryMetricsResponseReportSectionResponseBuilder)?
          updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceSlaSummaryMetricsResponseReportSectionResponse build() => _build();

  _$ServiceSlaSummaryMetricsResponseReportSectionResponse _build() {
    _$ServiceSlaSummaryMetricsResponseReportSectionResponse _$result;
    try {
      _$result = _$v ??
          _$ServiceSlaSummaryMetricsResponseReportSectionResponse._(
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
            r'ServiceSlaSummaryMetricsResponseReportSectionResponse',
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
