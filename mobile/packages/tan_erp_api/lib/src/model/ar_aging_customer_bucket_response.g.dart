// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ar_aging_customer_bucket_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ArAgingCustomerBucketResponse extends ArAgingCustomerBucketResponse {
  @override
  final String? customerId;
  @override
  final String? customerCode;
  @override
  final String? customerName;
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

  factory _$ArAgingCustomerBucketResponse(
          [void Function(ArAgingCustomerBucketResponseBuilder)? updates]) =>
      (ArAgingCustomerBucketResponseBuilder()..update(updates))._build();

  _$ArAgingCustomerBucketResponse._(
      {this.customerId,
      this.customerCode,
      this.customerName,
      this.current,
      this.bucket1To30,
      this.bucket31To60,
      this.bucket61To90,
      this.bucketOver90,
      this.totalOutstanding})
      : super._();
  @override
  ArAgingCustomerBucketResponse rebuild(
          void Function(ArAgingCustomerBucketResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ArAgingCustomerBucketResponseBuilder toBuilder() =>
      ArAgingCustomerBucketResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ArAgingCustomerBucketResponse &&
        customerId == other.customerId &&
        customerCode == other.customerCode &&
        customerName == other.customerName &&
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
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, customerCode.hashCode);
    _$hash = $jc(_$hash, customerName.hashCode);
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
    return (newBuiltValueToStringHelper(r'ArAgingCustomerBucketResponse')
          ..add('customerId', customerId)
          ..add('customerCode', customerCode)
          ..add('customerName', customerName)
          ..add('current', current)
          ..add('bucket1To30', bucket1To30)
          ..add('bucket31To60', bucket31To60)
          ..add('bucket61To90', bucket61To90)
          ..add('bucketOver90', bucketOver90)
          ..add('totalOutstanding', totalOutstanding))
        .toString();
  }
}

class ArAgingCustomerBucketResponseBuilder
    implements
        Builder<ArAgingCustomerBucketResponse,
            ArAgingCustomerBucketResponseBuilder> {
  _$ArAgingCustomerBucketResponse? _$v;

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

  String? _customerCode;
  String? get customerCode => _$this._customerCode;
  set customerCode(String? customerCode) => _$this._customerCode = customerCode;

  String? _customerName;
  String? get customerName => _$this._customerName;
  set customerName(String? customerName) => _$this._customerName = customerName;

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

  ArAgingCustomerBucketResponseBuilder() {
    ArAgingCustomerBucketResponse._defaults(this);
  }

  ArAgingCustomerBucketResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _customerId = $v.customerId;
      _customerCode = $v.customerCode;
      _customerName = $v.customerName;
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
  void replace(ArAgingCustomerBucketResponse other) {
    _$v = other as _$ArAgingCustomerBucketResponse;
  }

  @override
  void update(void Function(ArAgingCustomerBucketResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ArAgingCustomerBucketResponse build() => _build();

  _$ArAgingCustomerBucketResponse _build() {
    final _$result = _$v ??
        _$ArAgingCustomerBucketResponse._(
          customerId: customerId,
          customerCode: customerCode,
          customerName: customerName,
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
