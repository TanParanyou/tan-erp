// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'payment_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PaymentRequest extends PaymentRequest {
  @override
  final double? amount;
  @override
  final String? method;
  @override
  final String? reference;
  @override
  final Date? receivedDate;

  factory _$PaymentRequest([void Function(PaymentRequestBuilder)? updates]) =>
      (PaymentRequestBuilder()..update(updates))._build();

  _$PaymentRequest._(
      {this.amount, this.method, this.reference, this.receivedDate})
      : super._();
  @override
  PaymentRequest rebuild(void Function(PaymentRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PaymentRequestBuilder toBuilder() => PaymentRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PaymentRequest &&
        amount == other.amount &&
        method == other.method &&
        reference == other.reference &&
        receivedDate == other.receivedDate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, method.hashCode);
    _$hash = $jc(_$hash, reference.hashCode);
    _$hash = $jc(_$hash, receivedDate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PaymentRequest')
          ..add('amount', amount)
          ..add('method', method)
          ..add('reference', reference)
          ..add('receivedDate', receivedDate))
        .toString();
  }
}

class PaymentRequestBuilder
    implements Builder<PaymentRequest, PaymentRequestBuilder> {
  _$PaymentRequest? _$v;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  String? _method;
  String? get method => _$this._method;
  set method(String? method) => _$this._method = method;

  String? _reference;
  String? get reference => _$this._reference;
  set reference(String? reference) => _$this._reference = reference;

  Date? _receivedDate;
  Date? get receivedDate => _$this._receivedDate;
  set receivedDate(Date? receivedDate) => _$this._receivedDate = receivedDate;

  PaymentRequestBuilder() {
    PaymentRequest._defaults(this);
  }

  PaymentRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _amount = $v.amount;
      _method = $v.method;
      _reference = $v.reference;
      _receivedDate = $v.receivedDate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PaymentRequest other) {
    _$v = other as _$PaymentRequest;
  }

  @override
  void update(void Function(PaymentRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PaymentRequest build() => _build();

  _$PaymentRequest _build() {
    final _$result = _$v ??
        _$PaymentRequest._(
          amount: amount,
          method: method,
          reference: reference,
          receivedDate: receivedDate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
