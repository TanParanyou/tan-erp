// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ar_aging_report_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ArAgingReportRowResponse extends ArAgingReportRowResponse {
  @override
  final String? billingDocumentId;
  @override
  final String? number;
  @override
  final String? projectCode;
  @override
  final String? customerCode;
  @override
  final String? customerName;
  @override
  final String? branchCode;
  @override
  final Date? dueDate;
  @override
  final int? daysOverdue;
  @override
  final String? agingBucket;
  @override
  final double? grossTotal;
  @override
  final double? paidAmount;
  @override
  final double? outstandingAmount;

  factory _$ArAgingReportRowResponse(
          [void Function(ArAgingReportRowResponseBuilder)? updates]) =>
      (ArAgingReportRowResponseBuilder()..update(updates))._build();

  _$ArAgingReportRowResponse._(
      {this.billingDocumentId,
      this.number,
      this.projectCode,
      this.customerCode,
      this.customerName,
      this.branchCode,
      this.dueDate,
      this.daysOverdue,
      this.agingBucket,
      this.grossTotal,
      this.paidAmount,
      this.outstandingAmount})
      : super._();
  @override
  ArAgingReportRowResponse rebuild(
          void Function(ArAgingReportRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ArAgingReportRowResponseBuilder toBuilder() =>
      ArAgingReportRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ArAgingReportRowResponse &&
        billingDocumentId == other.billingDocumentId &&
        number == other.number &&
        projectCode == other.projectCode &&
        customerCode == other.customerCode &&
        customerName == other.customerName &&
        branchCode == other.branchCode &&
        dueDate == other.dueDate &&
        daysOverdue == other.daysOverdue &&
        agingBucket == other.agingBucket &&
        grossTotal == other.grossTotal &&
        paidAmount == other.paidAmount &&
        outstandingAmount == other.outstandingAmount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, billingDocumentId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, customerCode.hashCode);
    _$hash = $jc(_$hash, customerName.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, dueDate.hashCode);
    _$hash = $jc(_$hash, daysOverdue.hashCode);
    _$hash = $jc(_$hash, agingBucket.hashCode);
    _$hash = $jc(_$hash, grossTotal.hashCode);
    _$hash = $jc(_$hash, paidAmount.hashCode);
    _$hash = $jc(_$hash, outstandingAmount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ArAgingReportRowResponse')
          ..add('billingDocumentId', billingDocumentId)
          ..add('number', number)
          ..add('projectCode', projectCode)
          ..add('customerCode', customerCode)
          ..add('customerName', customerName)
          ..add('branchCode', branchCode)
          ..add('dueDate', dueDate)
          ..add('daysOverdue', daysOverdue)
          ..add('agingBucket', agingBucket)
          ..add('grossTotal', grossTotal)
          ..add('paidAmount', paidAmount)
          ..add('outstandingAmount', outstandingAmount))
        .toString();
  }
}

class ArAgingReportRowResponseBuilder
    implements
        Builder<ArAgingReportRowResponse, ArAgingReportRowResponseBuilder> {
  _$ArAgingReportRowResponse? _$v;

  String? _billingDocumentId;
  String? get billingDocumentId => _$this._billingDocumentId;
  set billingDocumentId(String? billingDocumentId) =>
      _$this._billingDocumentId = billingDocumentId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  String? _customerCode;
  String? get customerCode => _$this._customerCode;
  set customerCode(String? customerCode) => _$this._customerCode = customerCode;

  String? _customerName;
  String? get customerName => _$this._customerName;
  set customerName(String? customerName) => _$this._customerName = customerName;

  String? _branchCode;
  String? get branchCode => _$this._branchCode;
  set branchCode(String? branchCode) => _$this._branchCode = branchCode;

  Date? _dueDate;
  Date? get dueDate => _$this._dueDate;
  set dueDate(Date? dueDate) => _$this._dueDate = dueDate;

  int? _daysOverdue;
  int? get daysOverdue => _$this._daysOverdue;
  set daysOverdue(int? daysOverdue) => _$this._daysOverdue = daysOverdue;

  String? _agingBucket;
  String? get agingBucket => _$this._agingBucket;
  set agingBucket(String? agingBucket) => _$this._agingBucket = agingBucket;

  double? _grossTotal;
  double? get grossTotal => _$this._grossTotal;
  set grossTotal(double? grossTotal) => _$this._grossTotal = grossTotal;

  double? _paidAmount;
  double? get paidAmount => _$this._paidAmount;
  set paidAmount(double? paidAmount) => _$this._paidAmount = paidAmount;

  double? _outstandingAmount;
  double? get outstandingAmount => _$this._outstandingAmount;
  set outstandingAmount(double? outstandingAmount) =>
      _$this._outstandingAmount = outstandingAmount;

  ArAgingReportRowResponseBuilder() {
    ArAgingReportRowResponse._defaults(this);
  }

  ArAgingReportRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _billingDocumentId = $v.billingDocumentId;
      _number = $v.number;
      _projectCode = $v.projectCode;
      _customerCode = $v.customerCode;
      _customerName = $v.customerName;
      _branchCode = $v.branchCode;
      _dueDate = $v.dueDate;
      _daysOverdue = $v.daysOverdue;
      _agingBucket = $v.agingBucket;
      _grossTotal = $v.grossTotal;
      _paidAmount = $v.paidAmount;
      _outstandingAmount = $v.outstandingAmount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ArAgingReportRowResponse other) {
    _$v = other as _$ArAgingReportRowResponse;
  }

  @override
  void update(void Function(ArAgingReportRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ArAgingReportRowResponse build() => _build();

  _$ArAgingReportRowResponse _build() {
    final _$result = _$v ??
        _$ArAgingReportRowResponse._(
          billingDocumentId: billingDocumentId,
          number: number,
          projectCode: projectCode,
          customerCode: customerCode,
          customerName: customerName,
          branchCode: branchCode,
          dueDate: dueDate,
          daysOverdue: daysOverdue,
          agingBucket: agingBucket,
          grossTotal: grossTotal,
          paidAmount: paidAmount,
          outstandingAmount: outstandingAmount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
