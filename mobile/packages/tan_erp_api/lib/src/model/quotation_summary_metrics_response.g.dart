// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_summary_metrics_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationSummaryMetricsResponse
    extends QuotationSummaryMetricsResponse {
  @override
  final int? totalCount;
  @override
  final int? issuedCount;
  @override
  final double? issuedAmount;
  @override
  final int? acceptedCount;
  @override
  final double? acceptedAmount;
  @override
  final double? acceptanceRatePercent;

  factory _$QuotationSummaryMetricsResponse(
          [void Function(QuotationSummaryMetricsResponseBuilder)? updates]) =>
      (QuotationSummaryMetricsResponseBuilder()..update(updates))._build();

  _$QuotationSummaryMetricsResponse._(
      {this.totalCount,
      this.issuedCount,
      this.issuedAmount,
      this.acceptedCount,
      this.acceptedAmount,
      this.acceptanceRatePercent})
      : super._();
  @override
  QuotationSummaryMetricsResponse rebuild(
          void Function(QuotationSummaryMetricsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationSummaryMetricsResponseBuilder toBuilder() =>
      QuotationSummaryMetricsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationSummaryMetricsResponse &&
        totalCount == other.totalCount &&
        issuedCount == other.issuedCount &&
        issuedAmount == other.issuedAmount &&
        acceptedCount == other.acceptedCount &&
        acceptedAmount == other.acceptedAmount &&
        acceptanceRatePercent == other.acceptanceRatePercent;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, totalCount.hashCode);
    _$hash = $jc(_$hash, issuedCount.hashCode);
    _$hash = $jc(_$hash, issuedAmount.hashCode);
    _$hash = $jc(_$hash, acceptedCount.hashCode);
    _$hash = $jc(_$hash, acceptedAmount.hashCode);
    _$hash = $jc(_$hash, acceptanceRatePercent.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationSummaryMetricsResponse')
          ..add('totalCount', totalCount)
          ..add('issuedCount', issuedCount)
          ..add('issuedAmount', issuedAmount)
          ..add('acceptedCount', acceptedCount)
          ..add('acceptedAmount', acceptedAmount)
          ..add('acceptanceRatePercent', acceptanceRatePercent))
        .toString();
  }
}

class QuotationSummaryMetricsResponseBuilder
    implements
        Builder<QuotationSummaryMetricsResponse,
            QuotationSummaryMetricsResponseBuilder> {
  _$QuotationSummaryMetricsResponse? _$v;

  int? _totalCount;
  int? get totalCount => _$this._totalCount;
  set totalCount(int? totalCount) => _$this._totalCount = totalCount;

  int? _issuedCount;
  int? get issuedCount => _$this._issuedCount;
  set issuedCount(int? issuedCount) => _$this._issuedCount = issuedCount;

  double? _issuedAmount;
  double? get issuedAmount => _$this._issuedAmount;
  set issuedAmount(double? issuedAmount) => _$this._issuedAmount = issuedAmount;

  int? _acceptedCount;
  int? get acceptedCount => _$this._acceptedCount;
  set acceptedCount(int? acceptedCount) =>
      _$this._acceptedCount = acceptedCount;

  double? _acceptedAmount;
  double? get acceptedAmount => _$this._acceptedAmount;
  set acceptedAmount(double? acceptedAmount) =>
      _$this._acceptedAmount = acceptedAmount;

  double? _acceptanceRatePercent;
  double? get acceptanceRatePercent => _$this._acceptanceRatePercent;
  set acceptanceRatePercent(double? acceptanceRatePercent) =>
      _$this._acceptanceRatePercent = acceptanceRatePercent;

  QuotationSummaryMetricsResponseBuilder() {
    QuotationSummaryMetricsResponse._defaults(this);
  }

  QuotationSummaryMetricsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _totalCount = $v.totalCount;
      _issuedCount = $v.issuedCount;
      _issuedAmount = $v.issuedAmount;
      _acceptedCount = $v.acceptedCount;
      _acceptedAmount = $v.acceptedAmount;
      _acceptanceRatePercent = $v.acceptanceRatePercent;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationSummaryMetricsResponse other) {
    _$v = other as _$QuotationSummaryMetricsResponse;
  }

  @override
  void update(void Function(QuotationSummaryMetricsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationSummaryMetricsResponse build() => _build();

  _$QuotationSummaryMetricsResponse _build() {
    final _$result = _$v ??
        _$QuotationSummaryMetricsResponse._(
          totalCount: totalCount,
          issuedCount: issuedCount,
          issuedAmount: issuedAmount,
          acceptedCount: acceptedCount,
          acceptedAmount: acceptedAmount,
          acceptanceRatePercent: acceptanceRatePercent,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
