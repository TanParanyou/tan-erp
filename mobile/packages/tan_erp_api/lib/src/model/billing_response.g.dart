// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'billing_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BillingResponse extends BillingResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? number;
  @override
  final FinanceProjectRefResponse? project;
  @override
  final String? kind;
  @override
  final String? description;
  @override
  final double? amount;
  @override
  final double? paidAmount;
  @override
  final double? outstanding;
  @override
  final String? currency;
  @override
  final Date? dueDate;
  @override
  final String? status;
  @override
  final String? referenceHash;
  @override
  final String? voidReason;
  @override
  final DateTime? voidedAtUtc;
  @override
  final FinancePersonResponse? createdBy;
  @override
  final DateTime? issuedAtUtc;
  @override
  final String? rowVersion;
  @override
  final BuiltList<PaymentResponse>? payments;
  @override
  final String? taxInvoiceNumber;
  @override
  final double? vatRate;
  @override
  final double? vatAmount;
  @override
  final double? grossTotal;
  @override
  final double? effectiveReceivable;
  @override
  final double? retentionRate;
  @override
  final double? retentionAmount;
  @override
  final String? retentionStatus;
  @override
  final DateTime? releasedRetentionAtUtc;
  @override
  final FinancePersonResponse? releasedRetentionBy;
  @override
  final double? creditNoteBaseAmount;
  @override
  final double? creditNoteVatAmount;
  @override
  final double? debitNoteBaseAmount;
  @override
  final double? debitNoteVatAmount;
  @override
  final BuiltList<BillingLineResponse>? lines;
  @override
  final BuiltList<BillingAdjustmentNoteResponse>? notes;

  factory _$BillingResponse([void Function(BillingResponseBuilder)? updates]) =>
      (BillingResponseBuilder()..update(updates))._build();

  _$BillingResponse._(
      {this.id,
      this.branchId,
      this.number,
      this.project,
      this.kind,
      this.description,
      this.amount,
      this.paidAmount,
      this.outstanding,
      this.currency,
      this.dueDate,
      this.status,
      this.referenceHash,
      this.voidReason,
      this.voidedAtUtc,
      this.createdBy,
      this.issuedAtUtc,
      this.rowVersion,
      this.payments,
      this.taxInvoiceNumber,
      this.vatRate,
      this.vatAmount,
      this.grossTotal,
      this.effectiveReceivable,
      this.retentionRate,
      this.retentionAmount,
      this.retentionStatus,
      this.releasedRetentionAtUtc,
      this.releasedRetentionBy,
      this.creditNoteBaseAmount,
      this.creditNoteVatAmount,
      this.debitNoteBaseAmount,
      this.debitNoteVatAmount,
      this.lines,
      this.notes})
      : super._();
  @override
  BillingResponse rebuild(void Function(BillingResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BillingResponseBuilder toBuilder() => BillingResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BillingResponse &&
        id == other.id &&
        branchId == other.branchId &&
        number == other.number &&
        project == other.project &&
        kind == other.kind &&
        description == other.description &&
        amount == other.amount &&
        paidAmount == other.paidAmount &&
        outstanding == other.outstanding &&
        currency == other.currency &&
        dueDate == other.dueDate &&
        status == other.status &&
        referenceHash == other.referenceHash &&
        voidReason == other.voidReason &&
        voidedAtUtc == other.voidedAtUtc &&
        createdBy == other.createdBy &&
        issuedAtUtc == other.issuedAtUtc &&
        rowVersion == other.rowVersion &&
        payments == other.payments &&
        taxInvoiceNumber == other.taxInvoiceNumber &&
        vatRate == other.vatRate &&
        vatAmount == other.vatAmount &&
        grossTotal == other.grossTotal &&
        effectiveReceivable == other.effectiveReceivable &&
        retentionRate == other.retentionRate &&
        retentionAmount == other.retentionAmount &&
        retentionStatus == other.retentionStatus &&
        releasedRetentionAtUtc == other.releasedRetentionAtUtc &&
        releasedRetentionBy == other.releasedRetentionBy &&
        creditNoteBaseAmount == other.creditNoteBaseAmount &&
        creditNoteVatAmount == other.creditNoteVatAmount &&
        debitNoteBaseAmount == other.debitNoteBaseAmount &&
        debitNoteVatAmount == other.debitNoteVatAmount &&
        lines == other.lines &&
        notes == other.notes;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, project.hashCode);
    _$hash = $jc(_$hash, kind.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, paidAmount.hashCode);
    _$hash = $jc(_$hash, outstanding.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, dueDate.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, referenceHash.hashCode);
    _$hash = $jc(_$hash, voidReason.hashCode);
    _$hash = $jc(_$hash, voidedAtUtc.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, issuedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, payments.hashCode);
    _$hash = $jc(_$hash, taxInvoiceNumber.hashCode);
    _$hash = $jc(_$hash, vatRate.hashCode);
    _$hash = $jc(_$hash, vatAmount.hashCode);
    _$hash = $jc(_$hash, grossTotal.hashCode);
    _$hash = $jc(_$hash, effectiveReceivable.hashCode);
    _$hash = $jc(_$hash, retentionRate.hashCode);
    _$hash = $jc(_$hash, retentionAmount.hashCode);
    _$hash = $jc(_$hash, retentionStatus.hashCode);
    _$hash = $jc(_$hash, releasedRetentionAtUtc.hashCode);
    _$hash = $jc(_$hash, releasedRetentionBy.hashCode);
    _$hash = $jc(_$hash, creditNoteBaseAmount.hashCode);
    _$hash = $jc(_$hash, creditNoteVatAmount.hashCode);
    _$hash = $jc(_$hash, debitNoteBaseAmount.hashCode);
    _$hash = $jc(_$hash, debitNoteVatAmount.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jc(_$hash, notes.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BillingResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('number', number)
          ..add('project', project)
          ..add('kind', kind)
          ..add('description', description)
          ..add('amount', amount)
          ..add('paidAmount', paidAmount)
          ..add('outstanding', outstanding)
          ..add('currency', currency)
          ..add('dueDate', dueDate)
          ..add('status', status)
          ..add('referenceHash', referenceHash)
          ..add('voidReason', voidReason)
          ..add('voidedAtUtc', voidedAtUtc)
          ..add('createdBy', createdBy)
          ..add('issuedAtUtc', issuedAtUtc)
          ..add('rowVersion', rowVersion)
          ..add('payments', payments)
          ..add('taxInvoiceNumber', taxInvoiceNumber)
          ..add('vatRate', vatRate)
          ..add('vatAmount', vatAmount)
          ..add('grossTotal', grossTotal)
          ..add('effectiveReceivable', effectiveReceivable)
          ..add('retentionRate', retentionRate)
          ..add('retentionAmount', retentionAmount)
          ..add('retentionStatus', retentionStatus)
          ..add('releasedRetentionAtUtc', releasedRetentionAtUtc)
          ..add('releasedRetentionBy', releasedRetentionBy)
          ..add('creditNoteBaseAmount', creditNoteBaseAmount)
          ..add('creditNoteVatAmount', creditNoteVatAmount)
          ..add('debitNoteBaseAmount', debitNoteBaseAmount)
          ..add('debitNoteVatAmount', debitNoteVatAmount)
          ..add('lines', lines)
          ..add('notes', notes))
        .toString();
  }
}

class BillingResponseBuilder
    implements Builder<BillingResponse, BillingResponseBuilder> {
  _$BillingResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  FinanceProjectRefResponseBuilder? _project;
  FinanceProjectRefResponseBuilder get project =>
      _$this._project ??= FinanceProjectRefResponseBuilder();
  set project(FinanceProjectRefResponseBuilder? project) =>
      _$this._project = project;

  String? _kind;
  String? get kind => _$this._kind;
  set kind(String? kind) => _$this._kind = kind;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  double? _paidAmount;
  double? get paidAmount => _$this._paidAmount;
  set paidAmount(double? paidAmount) => _$this._paidAmount = paidAmount;

  double? _outstanding;
  double? get outstanding => _$this._outstanding;
  set outstanding(double? outstanding) => _$this._outstanding = outstanding;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  Date? _dueDate;
  Date? get dueDate => _$this._dueDate;
  set dueDate(Date? dueDate) => _$this._dueDate = dueDate;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _referenceHash;
  String? get referenceHash => _$this._referenceHash;
  set referenceHash(String? referenceHash) =>
      _$this._referenceHash = referenceHash;

  String? _voidReason;
  String? get voidReason => _$this._voidReason;
  set voidReason(String? voidReason) => _$this._voidReason = voidReason;

  DateTime? _voidedAtUtc;
  DateTime? get voidedAtUtc => _$this._voidedAtUtc;
  set voidedAtUtc(DateTime? voidedAtUtc) => _$this._voidedAtUtc = voidedAtUtc;

  FinancePersonResponseBuilder? _createdBy;
  FinancePersonResponseBuilder get createdBy =>
      _$this._createdBy ??= FinancePersonResponseBuilder();
  set createdBy(FinancePersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _issuedAtUtc;
  DateTime? get issuedAtUtc => _$this._issuedAtUtc;
  set issuedAtUtc(DateTime? issuedAtUtc) => _$this._issuedAtUtc = issuedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ListBuilder<PaymentResponse>? _payments;
  ListBuilder<PaymentResponse> get payments =>
      _$this._payments ??= ListBuilder<PaymentResponse>();
  set payments(ListBuilder<PaymentResponse>? payments) =>
      _$this._payments = payments;

  String? _taxInvoiceNumber;
  String? get taxInvoiceNumber => _$this._taxInvoiceNumber;
  set taxInvoiceNumber(String? taxInvoiceNumber) =>
      _$this._taxInvoiceNumber = taxInvoiceNumber;

  double? _vatRate;
  double? get vatRate => _$this._vatRate;
  set vatRate(double? vatRate) => _$this._vatRate = vatRate;

  double? _vatAmount;
  double? get vatAmount => _$this._vatAmount;
  set vatAmount(double? vatAmount) => _$this._vatAmount = vatAmount;

  double? _grossTotal;
  double? get grossTotal => _$this._grossTotal;
  set grossTotal(double? grossTotal) => _$this._grossTotal = grossTotal;

  double? _effectiveReceivable;
  double? get effectiveReceivable => _$this._effectiveReceivable;
  set effectiveReceivable(double? effectiveReceivable) =>
      _$this._effectiveReceivable = effectiveReceivable;

  double? _retentionRate;
  double? get retentionRate => _$this._retentionRate;
  set retentionRate(double? retentionRate) =>
      _$this._retentionRate = retentionRate;

  double? _retentionAmount;
  double? get retentionAmount => _$this._retentionAmount;
  set retentionAmount(double? retentionAmount) =>
      _$this._retentionAmount = retentionAmount;

  String? _retentionStatus;
  String? get retentionStatus => _$this._retentionStatus;
  set retentionStatus(String? retentionStatus) =>
      _$this._retentionStatus = retentionStatus;

  DateTime? _releasedRetentionAtUtc;
  DateTime? get releasedRetentionAtUtc => _$this._releasedRetentionAtUtc;
  set releasedRetentionAtUtc(DateTime? releasedRetentionAtUtc) =>
      _$this._releasedRetentionAtUtc = releasedRetentionAtUtc;

  FinancePersonResponseBuilder? _releasedRetentionBy;
  FinancePersonResponseBuilder get releasedRetentionBy =>
      _$this._releasedRetentionBy ??= FinancePersonResponseBuilder();
  set releasedRetentionBy(FinancePersonResponseBuilder? releasedRetentionBy) =>
      _$this._releasedRetentionBy = releasedRetentionBy;

  double? _creditNoteBaseAmount;
  double? get creditNoteBaseAmount => _$this._creditNoteBaseAmount;
  set creditNoteBaseAmount(double? creditNoteBaseAmount) =>
      _$this._creditNoteBaseAmount = creditNoteBaseAmount;

  double? _creditNoteVatAmount;
  double? get creditNoteVatAmount => _$this._creditNoteVatAmount;
  set creditNoteVatAmount(double? creditNoteVatAmount) =>
      _$this._creditNoteVatAmount = creditNoteVatAmount;

  double? _debitNoteBaseAmount;
  double? get debitNoteBaseAmount => _$this._debitNoteBaseAmount;
  set debitNoteBaseAmount(double? debitNoteBaseAmount) =>
      _$this._debitNoteBaseAmount = debitNoteBaseAmount;

  double? _debitNoteVatAmount;
  double? get debitNoteVatAmount => _$this._debitNoteVatAmount;
  set debitNoteVatAmount(double? debitNoteVatAmount) =>
      _$this._debitNoteVatAmount = debitNoteVatAmount;

  ListBuilder<BillingLineResponse>? _lines;
  ListBuilder<BillingLineResponse> get lines =>
      _$this._lines ??= ListBuilder<BillingLineResponse>();
  set lines(ListBuilder<BillingLineResponse>? lines) => _$this._lines = lines;

  ListBuilder<BillingAdjustmentNoteResponse>? _notes;
  ListBuilder<BillingAdjustmentNoteResponse> get notes =>
      _$this._notes ??= ListBuilder<BillingAdjustmentNoteResponse>();
  set notes(ListBuilder<BillingAdjustmentNoteResponse>? notes) =>
      _$this._notes = notes;

  BillingResponseBuilder() {
    BillingResponse._defaults(this);
  }

  BillingResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _number = $v.number;
      _project = $v.project?.toBuilder();
      _kind = $v.kind;
      _description = $v.description;
      _amount = $v.amount;
      _paidAmount = $v.paidAmount;
      _outstanding = $v.outstanding;
      _currency = $v.currency;
      _dueDate = $v.dueDate;
      _status = $v.status;
      _referenceHash = $v.referenceHash;
      _voidReason = $v.voidReason;
      _voidedAtUtc = $v.voidedAtUtc;
      _createdBy = $v.createdBy?.toBuilder();
      _issuedAtUtc = $v.issuedAtUtc;
      _rowVersion = $v.rowVersion;
      _payments = $v.payments?.toBuilder();
      _taxInvoiceNumber = $v.taxInvoiceNumber;
      _vatRate = $v.vatRate;
      _vatAmount = $v.vatAmount;
      _grossTotal = $v.grossTotal;
      _effectiveReceivable = $v.effectiveReceivable;
      _retentionRate = $v.retentionRate;
      _retentionAmount = $v.retentionAmount;
      _retentionStatus = $v.retentionStatus;
      _releasedRetentionAtUtc = $v.releasedRetentionAtUtc;
      _releasedRetentionBy = $v.releasedRetentionBy?.toBuilder();
      _creditNoteBaseAmount = $v.creditNoteBaseAmount;
      _creditNoteVatAmount = $v.creditNoteVatAmount;
      _debitNoteBaseAmount = $v.debitNoteBaseAmount;
      _debitNoteVatAmount = $v.debitNoteVatAmount;
      _lines = $v.lines?.toBuilder();
      _notes = $v.notes?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BillingResponse other) {
    _$v = other as _$BillingResponse;
  }

  @override
  void update(void Function(BillingResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BillingResponse build() => _build();

  _$BillingResponse _build() {
    _$BillingResponse _$result;
    try {
      _$result = _$v ??
          _$BillingResponse._(
            id: id,
            branchId: branchId,
            number: number,
            project: _project?.build(),
            kind: kind,
            description: description,
            amount: amount,
            paidAmount: paidAmount,
            outstanding: outstanding,
            currency: currency,
            dueDate: dueDate,
            status: status,
            referenceHash: referenceHash,
            voidReason: voidReason,
            voidedAtUtc: voidedAtUtc,
            createdBy: _createdBy?.build(),
            issuedAtUtc: issuedAtUtc,
            rowVersion: rowVersion,
            payments: _payments?.build(),
            taxInvoiceNumber: taxInvoiceNumber,
            vatRate: vatRate,
            vatAmount: vatAmount,
            grossTotal: grossTotal,
            effectiveReceivable: effectiveReceivable,
            retentionRate: retentionRate,
            retentionAmount: retentionAmount,
            retentionStatus: retentionStatus,
            releasedRetentionAtUtc: releasedRetentionAtUtc,
            releasedRetentionBy: _releasedRetentionBy?.build(),
            creditNoteBaseAmount: creditNoteBaseAmount,
            creditNoteVatAmount: creditNoteVatAmount,
            debitNoteBaseAmount: debitNoteBaseAmount,
            debitNoteVatAmount: debitNoteVatAmount,
            lines: _lines?.build(),
            notes: _notes?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'project';
        _project?.build();

        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'payments';
        _payments?.build();

        _$failedField = 'releasedRetentionBy';
        _releasedRetentionBy?.build();

        _$failedField = 'lines';
        _lines?.build();
        _$failedField = 'notes';
        _notes?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BillingResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
