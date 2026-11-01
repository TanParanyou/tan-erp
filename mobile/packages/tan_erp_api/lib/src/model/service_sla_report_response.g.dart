// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_sla_report_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceSlaReportResponse extends ServiceSlaReportResponse {
  @override
  final ServiceSlaSummaryMetricsResponse? summary;
  @override
  final BuiltList<ServiceSlaReportRowResponse>? rows;

  factory _$ServiceSlaReportResponse(
          [void Function(ServiceSlaReportResponseBuilder)? updates]) =>
      (ServiceSlaReportResponseBuilder()..update(updates))._build();

  _$ServiceSlaReportResponse._({this.summary, this.rows}) : super._();
  @override
  ServiceSlaReportResponse rebuild(
          void Function(ServiceSlaReportResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceSlaReportResponseBuilder toBuilder() =>
      ServiceSlaReportResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceSlaReportResponse &&
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
    return (newBuiltValueToStringHelper(r'ServiceSlaReportResponse')
          ..add('summary', summary)
          ..add('rows', rows))
        .toString();
  }
}

class ServiceSlaReportResponseBuilder
    implements
        Builder<ServiceSlaReportResponse, ServiceSlaReportResponseBuilder> {
  _$ServiceSlaReportResponse? _$v;

  ServiceSlaSummaryMetricsResponseBuilder? _summary;
  ServiceSlaSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= ServiceSlaSummaryMetricsResponseBuilder();
  set summary(ServiceSlaSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  ListBuilder<ServiceSlaReportRowResponse>? _rows;
  ListBuilder<ServiceSlaReportRowResponse> get rows =>
      _$this._rows ??= ListBuilder<ServiceSlaReportRowResponse>();
  set rows(ListBuilder<ServiceSlaReportRowResponse>? rows) =>
      _$this._rows = rows;

  ServiceSlaReportResponseBuilder() {
    ServiceSlaReportResponse._defaults(this);
  }

  ServiceSlaReportResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _summary = $v.summary?.toBuilder();
      _rows = $v.rows?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceSlaReportResponse other) {
    _$v = other as _$ServiceSlaReportResponse;
  }

  @override
  void update(void Function(ServiceSlaReportResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceSlaReportResponse build() => _build();

  _$ServiceSlaReportResponse _build() {
    _$ServiceSlaReportResponse _$result;
    try {
      _$result = _$v ??
          _$ServiceSlaReportResponse._(
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
            r'ServiceSlaReportResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
