// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_report_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationReportResponse extends QuotationReportResponse {
  @override
  final QuotationSummaryMetricsResponse? summary;
  @override
  final BuiltList<QuotationReportRowResponse>? rows;

  factory _$QuotationReportResponse(
          [void Function(QuotationReportResponseBuilder)? updates]) =>
      (QuotationReportResponseBuilder()..update(updates))._build();

  _$QuotationReportResponse._({this.summary, this.rows}) : super._();
  @override
  QuotationReportResponse rebuild(
          void Function(QuotationReportResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationReportResponseBuilder toBuilder() =>
      QuotationReportResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationReportResponse &&
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
    return (newBuiltValueToStringHelper(r'QuotationReportResponse')
          ..add('summary', summary)
          ..add('rows', rows))
        .toString();
  }
}

class QuotationReportResponseBuilder
    implements
        Builder<QuotationReportResponse, QuotationReportResponseBuilder> {
  _$QuotationReportResponse? _$v;

  QuotationSummaryMetricsResponseBuilder? _summary;
  QuotationSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= QuotationSummaryMetricsResponseBuilder();
  set summary(QuotationSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  ListBuilder<QuotationReportRowResponse>? _rows;
  ListBuilder<QuotationReportRowResponse> get rows =>
      _$this._rows ??= ListBuilder<QuotationReportRowResponse>();
  set rows(ListBuilder<QuotationReportRowResponse>? rows) =>
      _$this._rows = rows;

  QuotationReportResponseBuilder() {
    QuotationReportResponse._defaults(this);
  }

  QuotationReportResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _summary = $v.summary?.toBuilder();
      _rows = $v.rows?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationReportResponse other) {
    _$v = other as _$QuotationReportResponse;
  }

  @override
  void update(void Function(QuotationReportResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationReportResponse build() => _build();

  _$QuotationReportResponse _build() {
    _$QuotationReportResponse _$result;
    try {
      _$result = _$v ??
          _$QuotationReportResponse._(
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
            r'QuotationReportResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
