// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_report_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationReportRowResponse extends QuotationReportRowResponse {
  @override
  final String? quotationId;
  @override
  final String? number;
  @override
  final String? customerCode;
  @override
  final String? customerName;
  @override
  final String? branchCode;
  @override
  final String? status;
  @override
  final double? totalAmount;
  @override
  final DateTime? issuedAtUtc;
  @override
  final DateTime? acceptedAtUtc;

  factory _$QuotationReportRowResponse(
          [void Function(QuotationReportRowResponseBuilder)? updates]) =>
      (QuotationReportRowResponseBuilder()..update(updates))._build();

  _$QuotationReportRowResponse._(
      {this.quotationId,
      this.number,
      this.customerCode,
      this.customerName,
      this.branchCode,
      this.status,
      this.totalAmount,
      this.issuedAtUtc,
      this.acceptedAtUtc})
      : super._();
  @override
  QuotationReportRowResponse rebuild(
          void Function(QuotationReportRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationReportRowResponseBuilder toBuilder() =>
      QuotationReportRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationReportRowResponse &&
        quotationId == other.quotationId &&
        number == other.number &&
        customerCode == other.customerCode &&
        customerName == other.customerName &&
        branchCode == other.branchCode &&
        status == other.status &&
        totalAmount == other.totalAmount &&
        issuedAtUtc == other.issuedAtUtc &&
        acceptedAtUtc == other.acceptedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, quotationId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, customerCode.hashCode);
    _$hash = $jc(_$hash, customerName.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, totalAmount.hashCode);
    _$hash = $jc(_$hash, issuedAtUtc.hashCode);
    _$hash = $jc(_$hash, acceptedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationReportRowResponse')
          ..add('quotationId', quotationId)
          ..add('number', number)
          ..add('customerCode', customerCode)
          ..add('customerName', customerName)
          ..add('branchCode', branchCode)
          ..add('status', status)
          ..add('totalAmount', totalAmount)
          ..add('issuedAtUtc', issuedAtUtc)
          ..add('acceptedAtUtc', acceptedAtUtc))
        .toString();
  }
}

class QuotationReportRowResponseBuilder
    implements
        Builder<QuotationReportRowResponse, QuotationReportRowResponseBuilder> {
  _$QuotationReportRowResponse? _$v;

  String? _quotationId;
  String? get quotationId => _$this._quotationId;
  set quotationId(String? quotationId) => _$this._quotationId = quotationId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _customerCode;
  String? get customerCode => _$this._customerCode;
  set customerCode(String? customerCode) => _$this._customerCode = customerCode;

  String? _customerName;
  String? get customerName => _$this._customerName;
  set customerName(String? customerName) => _$this._customerName = customerName;

  String? _branchCode;
  String? get branchCode => _$this._branchCode;
  set branchCode(String? branchCode) => _$this._branchCode = branchCode;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  double? _totalAmount;
  double? get totalAmount => _$this._totalAmount;
  set totalAmount(double? totalAmount) => _$this._totalAmount = totalAmount;

  DateTime? _issuedAtUtc;
  DateTime? get issuedAtUtc => _$this._issuedAtUtc;
  set issuedAtUtc(DateTime? issuedAtUtc) => _$this._issuedAtUtc = issuedAtUtc;

  DateTime? _acceptedAtUtc;
  DateTime? get acceptedAtUtc => _$this._acceptedAtUtc;
  set acceptedAtUtc(DateTime? acceptedAtUtc) =>
      _$this._acceptedAtUtc = acceptedAtUtc;

  QuotationReportRowResponseBuilder() {
    QuotationReportRowResponse._defaults(this);
  }

  QuotationReportRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _quotationId = $v.quotationId;
      _number = $v.number;
      _customerCode = $v.customerCode;
      _customerName = $v.customerName;
      _branchCode = $v.branchCode;
      _status = $v.status;
      _totalAmount = $v.totalAmount;
      _issuedAtUtc = $v.issuedAtUtc;
      _acceptedAtUtc = $v.acceptedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationReportRowResponse other) {
    _$v = other as _$QuotationReportRowResponse;
  }

  @override
  void update(void Function(QuotationReportRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationReportRowResponse build() => _build();

  _$QuotationReportRowResponse _build() {
    final _$result = _$v ??
        _$QuotationReportRowResponse._(
          quotationId: quotationId,
          number: number,
          customerCode: customerCode,
          customerName: customerName,
          branchCode: branchCode,
          status: status,
          totalAmount: totalAmount,
          issuedAtUtc: issuedAtUtc,
          acceptedAtUtc: acceptedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
