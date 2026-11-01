// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reporting_ar_aging_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReportingArAgingResponse extends ReportingArAgingResponse {
  @override
  final ArAgingSummaryMetricsResponse? summary;
  @override
  final BuiltList<ArAgingReportRowResponse>? rows;

  factory _$ReportingArAgingResponse(
          [void Function(ReportingArAgingResponseBuilder)? updates]) =>
      (ReportingArAgingResponseBuilder()..update(updates))._build();

  _$ReportingArAgingResponse._({this.summary, this.rows}) : super._();
  @override
  ReportingArAgingResponse rebuild(
          void Function(ReportingArAgingResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReportingArAgingResponseBuilder toBuilder() =>
      ReportingArAgingResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReportingArAgingResponse &&
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
    return (newBuiltValueToStringHelper(r'ReportingArAgingResponse')
          ..add('summary', summary)
          ..add('rows', rows))
        .toString();
  }
}

class ReportingArAgingResponseBuilder
    implements
        Builder<ReportingArAgingResponse, ReportingArAgingResponseBuilder> {
  _$ReportingArAgingResponse? _$v;

  ArAgingSummaryMetricsResponseBuilder? _summary;
  ArAgingSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= ArAgingSummaryMetricsResponseBuilder();
  set summary(ArAgingSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  ListBuilder<ArAgingReportRowResponse>? _rows;
  ListBuilder<ArAgingReportRowResponse> get rows =>
      _$this._rows ??= ListBuilder<ArAgingReportRowResponse>();
  set rows(ListBuilder<ArAgingReportRowResponse>? rows) => _$this._rows = rows;

  ReportingArAgingResponseBuilder() {
    ReportingArAgingResponse._defaults(this);
  }

  ReportingArAgingResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _summary = $v.summary?.toBuilder();
      _rows = $v.rows?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReportingArAgingResponse other) {
    _$v = other as _$ReportingArAgingResponse;
  }

  @override
  void update(void Function(ReportingArAgingResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReportingArAgingResponse build() => _build();

  _$ReportingArAgingResponse _build() {
    _$ReportingArAgingResponse _$result;
    try {
      _$result = _$v ??
          _$ReportingArAgingResponse._(
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
            r'ReportingArAgingResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
