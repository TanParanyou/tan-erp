// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_value_report_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockValueReportResponse extends StockValueReportResponse {
  @override
  final StockValueSummaryMetricsResponse? summary;
  @override
  final BuiltList<StockValueReportRowResponse>? rows;

  factory _$StockValueReportResponse(
          [void Function(StockValueReportResponseBuilder)? updates]) =>
      (StockValueReportResponseBuilder()..update(updates))._build();

  _$StockValueReportResponse._({this.summary, this.rows}) : super._();
  @override
  StockValueReportResponse rebuild(
          void Function(StockValueReportResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockValueReportResponseBuilder toBuilder() =>
      StockValueReportResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockValueReportResponse &&
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
    return (newBuiltValueToStringHelper(r'StockValueReportResponse')
          ..add('summary', summary)
          ..add('rows', rows))
        .toString();
  }
}

class StockValueReportResponseBuilder
    implements
        Builder<StockValueReportResponse, StockValueReportResponseBuilder> {
  _$StockValueReportResponse? _$v;

  StockValueSummaryMetricsResponseBuilder? _summary;
  StockValueSummaryMetricsResponseBuilder get summary =>
      _$this._summary ??= StockValueSummaryMetricsResponseBuilder();
  set summary(StockValueSummaryMetricsResponseBuilder? summary) =>
      _$this._summary = summary;

  ListBuilder<StockValueReportRowResponse>? _rows;
  ListBuilder<StockValueReportRowResponse> get rows =>
      _$this._rows ??= ListBuilder<StockValueReportRowResponse>();
  set rows(ListBuilder<StockValueReportRowResponse>? rows) =>
      _$this._rows = rows;

  StockValueReportResponseBuilder() {
    StockValueReportResponse._defaults(this);
  }

  StockValueReportResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _summary = $v.summary?.toBuilder();
      _rows = $v.rows?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockValueReportResponse other) {
    _$v = other as _$StockValueReportResponse;
  }

  @override
  void update(void Function(StockValueReportResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockValueReportResponse build() => _build();

  _$StockValueReportResponse _build() {
    _$StockValueReportResponse _$result;
    try {
      _$result = _$v ??
          _$StockValueReportResponse._(
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
            r'StockValueReportResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
