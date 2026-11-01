// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'payment_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PaymentResponse extends PaymentResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final double? amount;
  @override
  final String? method;
  @override
  final String? reference;
  @override
  final Date? receivedDate;
  @override
  final String? status;
  @override
  final String? reversalReason;
  @override
  final DateTime? reversedAtUtc;
  @override
  final FinancePersonResponse? recordedBy;
  @override
  final DateTime? recordedAtUtc;
  @override
  final String? receiptNumber;
  @override
  final double? whtRate;
  @override
  final double? whtAmount;
  @override
  final double? netCashReceived;
  @override
  final String? whtType;
  @override
  final String? whtCertificateNumber;

  factory _$PaymentResponse([void Function(PaymentResponseBuilder)? updates]) =>
      (PaymentResponseBuilder()..update(updates))._build();

  _$PaymentResponse._(
      {this.id,
      this.number,
      this.amount,
      this.method,
      this.reference,
      this.receivedDate,
      this.status,
      this.reversalReason,
      this.reversedAtUtc,
      this.recordedBy,
      this.recordedAtUtc,
      this.receiptNumber,
      this.whtRate,
      this.whtAmount,
      this.netCashReceived,
      this.whtType,
      this.whtCertificateNumber})
      : super._();
  @override
  PaymentResponse rebuild(void Function(PaymentResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PaymentResponseBuilder toBuilder() => PaymentResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PaymentResponse &&
        id == other.id &&
        number == other.number &&
        amount == other.amount &&
        method == other.method &&
        reference == other.reference &&
        receivedDate == other.receivedDate &&
        status == other.status &&
        reversalReason == other.reversalReason &&
        reversedAtUtc == other.reversedAtUtc &&
        recordedBy == other.recordedBy &&
        recordedAtUtc == other.recordedAtUtc &&
        receiptNumber == other.receiptNumber &&
        whtRate == other.whtRate &&
        whtAmount == other.whtAmount &&
        netCashReceived == other.netCashReceived &&
        whtType == other.whtType &&
        whtCertificateNumber == other.whtCertificateNumber;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, method.hashCode);
    _$hash = $jc(_$hash, reference.hashCode);
    _$hash = $jc(_$hash, receivedDate.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, reversalReason.hashCode);
    _$hash = $jc(_$hash, reversedAtUtc.hashCode);
    _$hash = $jc(_$hash, recordedBy.hashCode);
    _$hash = $jc(_$hash, recordedAtUtc.hashCode);
    _$hash = $jc(_$hash, receiptNumber.hashCode);
    _$hash = $jc(_$hash, whtRate.hashCode);
    _$hash = $jc(_$hash, whtAmount.hashCode);
    _$hash = $jc(_$hash, netCashReceived.hashCode);
    _$hash = $jc(_$hash, whtType.hashCode);
    _$hash = $jc(_$hash, whtCertificateNumber.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PaymentResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('amount', amount)
          ..add('method', method)
          ..add('reference', reference)
          ..add('receivedDate', receivedDate)
          ..add('status', status)
          ..add('reversalReason', reversalReason)
          ..add('reversedAtUtc', reversedAtUtc)
          ..add('recordedBy', recordedBy)
          ..add('recordedAtUtc', recordedAtUtc)
          ..add('receiptNumber', receiptNumber)
          ..add('whtRate', whtRate)
          ..add('whtAmount', whtAmount)
          ..add('netCashReceived', netCashReceived)
          ..add('whtType', whtType)
          ..add('whtCertificateNumber', whtCertificateNumber))
        .toString();
  }
}

class PaymentResponseBuilder
    implements Builder<PaymentResponse, PaymentResponseBuilder> {
  _$PaymentResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

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

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _reversalReason;
  String? get reversalReason => _$this._reversalReason;
  set reversalReason(String? reversalReason) =>
      _$this._reversalReason = reversalReason;

  DateTime? _reversedAtUtc;
  DateTime? get reversedAtUtc => _$this._reversedAtUtc;
  set reversedAtUtc(DateTime? reversedAtUtc) =>
      _$this._reversedAtUtc = reversedAtUtc;

  FinancePersonResponseBuilder? _recordedBy;
  FinancePersonResponseBuilder get recordedBy =>
      _$this._recordedBy ??= FinancePersonResponseBuilder();
  set recordedBy(FinancePersonResponseBuilder? recordedBy) =>
      _$this._recordedBy = recordedBy;

  DateTime? _recordedAtUtc;
  DateTime? get recordedAtUtc => _$this._recordedAtUtc;
  set recordedAtUtc(DateTime? recordedAtUtc) =>
      _$this._recordedAtUtc = recordedAtUtc;

  String? _receiptNumber;
  String? get receiptNumber => _$this._receiptNumber;
  set receiptNumber(String? receiptNumber) =>
      _$this._receiptNumber = receiptNumber;

  double? _whtRate;
  double? get whtRate => _$this._whtRate;
  set whtRate(double? whtRate) => _$this._whtRate = whtRate;

  double? _whtAmount;
  double? get whtAmount => _$this._whtAmount;
  set whtAmount(double? whtAmount) => _$this._whtAmount = whtAmount;

  double? _netCashReceived;
  double? get netCashReceived => _$this._netCashReceived;
  set netCashReceived(double? netCashReceived) =>
      _$this._netCashReceived = netCashReceived;

  String? _whtType;
  String? get whtType => _$this._whtType;
  set whtType(String? whtType) => _$this._whtType = whtType;

  String? _whtCertificateNumber;
  String? get whtCertificateNumber => _$this._whtCertificateNumber;
  set whtCertificateNumber(String? whtCertificateNumber) =>
      _$this._whtCertificateNumber = whtCertificateNumber;

  PaymentResponseBuilder() {
    PaymentResponse._defaults(this);
  }

  PaymentResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _amount = $v.amount;
      _method = $v.method;
      _reference = $v.reference;
      _receivedDate = $v.receivedDate;
      _status = $v.status;
      _reversalReason = $v.reversalReason;
      _reversedAtUtc = $v.reversedAtUtc;
      _recordedBy = $v.recordedBy?.toBuilder();
      _recordedAtUtc = $v.recordedAtUtc;
      _receiptNumber = $v.receiptNumber;
      _whtRate = $v.whtRate;
      _whtAmount = $v.whtAmount;
      _netCashReceived = $v.netCashReceived;
      _whtType = $v.whtType;
      _whtCertificateNumber = $v.whtCertificateNumber;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PaymentResponse other) {
    _$v = other as _$PaymentResponse;
  }

  @override
  void update(void Function(PaymentResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PaymentResponse build() => _build();

  _$PaymentResponse _build() {
    _$PaymentResponse _$result;
    try {
      _$result = _$v ??
          _$PaymentResponse._(
            id: id,
            number: number,
            amount: amount,
            method: method,
            reference: reference,
            receivedDate: receivedDate,
            status: status,
            reversalReason: reversalReason,
            reversedAtUtc: reversedAtUtc,
            recordedBy: _recordedBy?.build(),
            recordedAtUtc: recordedAtUtc,
            receiptNumber: receiptNumber,
            whtRate: whtRate,
            whtAmount: whtAmount,
            netCashReceived: netCashReceived,
            whtType: whtType,
            whtCertificateNumber: whtCertificateNumber,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'recordedBy';
        _recordedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PaymentResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
