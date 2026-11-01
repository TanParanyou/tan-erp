// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ar_aging_report_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ArAgingReportResponse extends ArAgingReportResponse {
  @override
  final Date? asOfDate;
  @override
  final double? totalCurrent;
  @override
  final double? totalBucket1To30;
  @override
  final double? totalBucket31To60;
  @override
  final double? totalBucket61To90;
  @override
  final double? totalBucketOver90;
  @override
  final double? grandTotalOutstanding;
  @override
  final BuiltList<ArAgingCustomerBucketResponse>? customers;

  factory _$ArAgingReportResponse(
          [void Function(ArAgingReportResponseBuilder)? updates]) =>
      (ArAgingReportResponseBuilder()..update(updates))._build();

  _$ArAgingReportResponse._(
      {this.asOfDate,
      this.totalCurrent,
      this.totalBucket1To30,
      this.totalBucket31To60,
      this.totalBucket61To90,
      this.totalBucketOver90,
      this.grandTotalOutstanding,
      this.customers})
      : super._();
  @override
  ArAgingReportResponse rebuild(
          void Function(ArAgingReportResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ArAgingReportResponseBuilder toBuilder() =>
      ArAgingReportResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ArAgingReportResponse &&
        asOfDate == other.asOfDate &&
        totalCurrent == other.totalCurrent &&
        totalBucket1To30 == other.totalBucket1To30 &&
        totalBucket31To60 == other.totalBucket31To60 &&
        totalBucket61To90 == other.totalBucket61To90 &&
        totalBucketOver90 == other.totalBucketOver90 &&
        grandTotalOutstanding == other.grandTotalOutstanding &&
        customers == other.customers;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, asOfDate.hashCode);
    _$hash = $jc(_$hash, totalCurrent.hashCode);
    _$hash = $jc(_$hash, totalBucket1To30.hashCode);
    _$hash = $jc(_$hash, totalBucket31To60.hashCode);
    _$hash = $jc(_$hash, totalBucket61To90.hashCode);
    _$hash = $jc(_$hash, totalBucketOver90.hashCode);
    _$hash = $jc(_$hash, grandTotalOutstanding.hashCode);
    _$hash = $jc(_$hash, customers.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ArAgingReportResponse')
          ..add('asOfDate', asOfDate)
          ..add('totalCurrent', totalCurrent)
          ..add('totalBucket1To30', totalBucket1To30)
          ..add('totalBucket31To60', totalBucket31To60)
          ..add('totalBucket61To90', totalBucket61To90)
          ..add('totalBucketOver90', totalBucketOver90)
          ..add('grandTotalOutstanding', grandTotalOutstanding)
          ..add('customers', customers))
        .toString();
  }
}

class ArAgingReportResponseBuilder
    implements Builder<ArAgingReportResponse, ArAgingReportResponseBuilder> {
  _$ArAgingReportResponse? _$v;

  Date? _asOfDate;
  Date? get asOfDate => _$this._asOfDate;
  set asOfDate(Date? asOfDate) => _$this._asOfDate = asOfDate;

  double? _totalCurrent;
  double? get totalCurrent => _$this._totalCurrent;
  set totalCurrent(double? totalCurrent) => _$this._totalCurrent = totalCurrent;

  double? _totalBucket1To30;
  double? get totalBucket1To30 => _$this._totalBucket1To30;
  set totalBucket1To30(double? totalBucket1To30) =>
      _$this._totalBucket1To30 = totalBucket1To30;

  double? _totalBucket31To60;
  double? get totalBucket31To60 => _$this._totalBucket31To60;
  set totalBucket31To60(double? totalBucket31To60) =>
      _$this._totalBucket31To60 = totalBucket31To60;

  double? _totalBucket61To90;
  double? get totalBucket61To90 => _$this._totalBucket61To90;
  set totalBucket61To90(double? totalBucket61To90) =>
      _$this._totalBucket61To90 = totalBucket61To90;

  double? _totalBucketOver90;
  double? get totalBucketOver90 => _$this._totalBucketOver90;
  set totalBucketOver90(double? totalBucketOver90) =>
      _$this._totalBucketOver90 = totalBucketOver90;

  double? _grandTotalOutstanding;
  double? get grandTotalOutstanding => _$this._grandTotalOutstanding;
  set grandTotalOutstanding(double? grandTotalOutstanding) =>
      _$this._grandTotalOutstanding = grandTotalOutstanding;

  ListBuilder<ArAgingCustomerBucketResponse>? _customers;
  ListBuilder<ArAgingCustomerBucketResponse> get customers =>
      _$this._customers ??= ListBuilder<ArAgingCustomerBucketResponse>();
  set customers(ListBuilder<ArAgingCustomerBucketResponse>? customers) =>
      _$this._customers = customers;

  ArAgingReportResponseBuilder() {
    ArAgingReportResponse._defaults(this);
  }

  ArAgingReportResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _asOfDate = $v.asOfDate;
      _totalCurrent = $v.totalCurrent;
      _totalBucket1To30 = $v.totalBucket1To30;
      _totalBucket31To60 = $v.totalBucket31To60;
      _totalBucket61To90 = $v.totalBucket61To90;
      _totalBucketOver90 = $v.totalBucketOver90;
      _grandTotalOutstanding = $v.grandTotalOutstanding;
      _customers = $v.customers?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ArAgingReportResponse other) {
    _$v = other as _$ArAgingReportResponse;
  }

  @override
  void update(void Function(ArAgingReportResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ArAgingReportResponse build() => _build();

  _$ArAgingReportResponse _build() {
    _$ArAgingReportResponse _$result;
    try {
      _$result = _$v ??
          _$ArAgingReportResponse._(
            asOfDate: asOfDate,
            totalCurrent: totalCurrent,
            totalBucket1To30: totalBucket1To30,
            totalBucket31To60: totalBucket31To60,
            totalBucket61To90: totalBucket61To90,
            totalBucketOver90: totalBucketOver90,
            grandTotalOutstanding: grandTotalOutstanding,
            customers: _customers?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'customers';
        _customers?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ArAgingReportResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
