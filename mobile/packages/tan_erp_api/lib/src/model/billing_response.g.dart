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
      this.payments})
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
        payments == other.payments;
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
          ..add('payments', payments))
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
