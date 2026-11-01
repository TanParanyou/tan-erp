// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ar_aging_summary_metrics_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ArAgingSummaryMetricsResponse extends ArAgingSummaryMetricsResponse {
  @override
  final Date? asOfDate;
  @override
  final int? invoiceCount;
  @override
  final double? current;
  @override
  final double? bucket1To30;
  @override
  final double? bucket31To60;
  @override
  final double? bucket61To90;
  @override
  final double? bucketOver90;
  @override
  final double? totalOutstanding;

  factory _$ArAgingSummaryMetricsResponse(
          [void Function(ArAgingSummaryMetricsResponseBuilder)? updates]) =>
      (ArAgingSummaryMetricsResponseBuilder()..update(updates))._build();

  _$ArAgingSummaryMetricsResponse._(
      {this.asOfDate,
      this.invoiceCount,
      this.current,
      this.bucket1To30,
      this.bucket31To60,
      this.bucket61To90,
      this.bucketOver90,
      this.totalOutstanding})
      : super._();
  @override
  ArAgingSummaryMetricsResponse rebuild(
          void Function(ArAgingSummaryMetricsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ArAgingSummaryMetricsResponseBuilder toBuilder() =>
      ArAgingSummaryMetricsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ArAgingSummaryMetricsResponse &&
        asOfDate == other.asOfDate &&
        invoiceCount == other.invoiceCount &&
        current == other.current &&
        bucket1To30 == other.bucket1To30 &&
        bucket31To60 == other.bucket31To60 &&
        bucket61To90 == other.bucket61To90 &&
        bucketOver90 == other.bucketOver90 &&
        totalOutstanding == other.totalOutstanding;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, asOfDate.hashCode);
    _$hash = $jc(_$hash, invoiceCount.hashCode);
    _$hash = $jc(_$hash, current.hashCode);
    _$hash = $jc(_$hash, bucket1To30.hashCode);
    _$hash = $jc(_$hash, bucket31To60.hashCode);
    _$hash = $jc(_$hash, bucket61To90.hashCode);
    _$hash = $jc(_$hash, bucketOver90.hashCode);
    _$hash = $jc(_$hash, totalOutstanding.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ArAgingSummaryMetricsResponse')
          ..add('asOfDate', asOfDate)
          ..add('invoiceCount', invoiceCount)
          ..add('current', current)
          ..add('bucket1To30', bucket1To30)
          ..add('bucket31To60', bucket31To60)
          ..add('bucket61To90', bucket61To90)
          ..add('bucketOver90', bucketOver90)
          ..add('totalOutstanding', totalOutstanding))
        .toString();
  }
}

class ArAgingSummaryMetricsResponseBuilder
    implements
        Builder<ArAgingSummaryMetricsResponse,
            ArAgingSummaryMetricsResponseBuilder> {
  _$ArAgingSummaryMetricsResponse? _$v;

  Date? _asOfDate;
  Date? get asOfDate => _$this._asOfDate;
  set asOfDate(Date? asOfDate) => _$this._asOfDate = asOfDate;

  int? _invoiceCount;
  int? get invoiceCount => _$this._invoiceCount;
  set invoiceCount(int? invoiceCount) => _$this._invoiceCount = invoiceCount;

  double? _current;
  double? get current => _$this._current;
  set current(double? current) => _$this._current = current;

  double? _bucket1To30;
  double? get bucket1To30 => _$this._bucket1To30;
  set bucket1To30(double? bucket1To30) => _$this._bucket1To30 = bucket1To30;

  double? _bucket31To60;
  double? get bucket31To60 => _$this._bucket31To60;
  set bucket31To60(double? bucket31To60) => _$this._bucket31To60 = bucket31To60;

  double? _bucket61To90;
  double? get bucket61To90 => _$this._bucket61To90;
  set bucket61To90(double? bucket61To90) => _$this._bucket61To90 = bucket61To90;

  double? _bucketOver90;
  double? get bucketOver90 => _$this._bucketOver90;
  set bucketOver90(double? bucketOver90) => _$this._bucketOver90 = bucketOver90;

  double? _totalOutstanding;
  double? get totalOutstanding => _$this._totalOutstanding;
  set totalOutstanding(double? totalOutstanding) =>
      _$this._totalOutstanding = totalOutstanding;

  ArAgingSummaryMetricsResponseBuilder() {
    ArAgingSummaryMetricsResponse._defaults(this);
  }

  ArAgingSummaryMetricsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _asOfDate = $v.asOfDate;
      _invoiceCount = $v.invoiceCount;
      _current = $v.current;
      _bucket1To30 = $v.bucket1To30;
      _bucket31To60 = $v.bucket31To60;
      _bucket61To90 = $v.bucket61To90;
      _bucketOver90 = $v.bucketOver90;
      _totalOutstanding = $v.totalOutstanding;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ArAgingSummaryMetricsResponse other) {
    _$v = other as _$ArAgingSummaryMetricsResponse;
  }

  @override
  void update(void Function(ArAgingSummaryMetricsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ArAgingSummaryMetricsResponse build() => _build();

  _$ArAgingSummaryMetricsResponse _build() {
    final _$result = _$v ??
        _$ArAgingSummaryMetricsResponse._(
          asOfDate: asOfDate,
          invoiceCount: invoiceCount,
          current: current,
          bucket1To30: bucket1To30,
          bucket31To60: bucket31To60,
          bucket61To90: bucket61To90,
          bucketOver90: bucketOver90,
          totalOutstanding: totalOutstanding,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
