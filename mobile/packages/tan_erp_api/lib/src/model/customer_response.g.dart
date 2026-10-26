// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerResponse extends CustomerResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? customerType;
  @override
  final String? displayNameTh;
  @override
  final String? displayNameEn;
  @override
  final String? preferredLocale;
  @override
  final String? status;
  @override
  final CustomerContactResponse? primaryContact;
  @override
  final BuiltList<DuplicateCustomerResponse>? duplicateCandidates;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? leadSource;
  @override
  final String? leadSourceNote;
  @override
  final String? imageFileId;
  @override
  final String? legalName;
  @override
  final String? taxIdentifier;
  @override
  final String? branchCode;
  @override
  final int? creditTermDays;
  @override
  final double? creditLimit;
  @override
  final String? currencyCode;
  @override
  final String? billingCycle;
  @override
  final int? billingDay;
  @override
  final String? paymentConditionNote;
  @override
  final String? inactiveReason;

  factory _$CustomerResponse(
          [void Function(CustomerResponseBuilder)? updates]) =>
      (CustomerResponseBuilder()..update(updates))._build();

  _$CustomerResponse._(
      {this.id,
      this.code,
      this.customerType,
      this.displayNameTh,
      this.displayNameEn,
      this.preferredLocale,
      this.status,
      this.primaryContact,
      this.duplicateCandidates,
      this.rowVersion,
      this.createdAtUtc,
      this.leadSource,
      this.leadSourceNote,
      this.imageFileId,
      this.legalName,
      this.taxIdentifier,
      this.branchCode,
      this.creditTermDays,
      this.creditLimit,
      this.currencyCode,
      this.billingCycle,
      this.billingDay,
      this.paymentConditionNote,
      this.inactiveReason})
      : super._();
  @override
  CustomerResponse rebuild(void Function(CustomerResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerResponseBuilder toBuilder() =>
      CustomerResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerResponse &&
        id == other.id &&
        code == other.code &&
        customerType == other.customerType &&
        displayNameTh == other.displayNameTh &&
        displayNameEn == other.displayNameEn &&
        preferredLocale == other.preferredLocale &&
        status == other.status &&
        primaryContact == other.primaryContact &&
        duplicateCandidates == other.duplicateCandidates &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        leadSource == other.leadSource &&
        leadSourceNote == other.leadSourceNote &&
        imageFileId == other.imageFileId &&
        legalName == other.legalName &&
        taxIdentifier == other.taxIdentifier &&
        branchCode == other.branchCode &&
        creditTermDays == other.creditTermDays &&
        creditLimit == other.creditLimit &&
        currencyCode == other.currencyCode &&
        billingCycle == other.billingCycle &&
        billingDay == other.billingDay &&
        paymentConditionNote == other.paymentConditionNote &&
        inactiveReason == other.inactiveReason;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, customerType.hashCode);
    _$hash = $jc(_$hash, displayNameTh.hashCode);
    _$hash = $jc(_$hash, displayNameEn.hashCode);
    _$hash = $jc(_$hash, preferredLocale.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, primaryContact.hashCode);
    _$hash = $jc(_$hash, duplicateCandidates.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, leadSource.hashCode);
    _$hash = $jc(_$hash, leadSourceNote.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jc(_$hash, legalName.hashCode);
    _$hash = $jc(_$hash, taxIdentifier.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, creditTermDays.hashCode);
    _$hash = $jc(_$hash, creditLimit.hashCode);
    _$hash = $jc(_$hash, currencyCode.hashCode);
    _$hash = $jc(_$hash, billingCycle.hashCode);
    _$hash = $jc(_$hash, billingDay.hashCode);
    _$hash = $jc(_$hash, paymentConditionNote.hashCode);
    _$hash = $jc(_$hash, inactiveReason.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CustomerResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('customerType', customerType)
          ..add('displayNameTh', displayNameTh)
          ..add('displayNameEn', displayNameEn)
          ..add('preferredLocale', preferredLocale)
          ..add('status', status)
          ..add('primaryContact', primaryContact)
          ..add('duplicateCandidates', duplicateCandidates)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('leadSource', leadSource)
          ..add('leadSourceNote', leadSourceNote)
          ..add('imageFileId', imageFileId)
          ..add('legalName', legalName)
          ..add('taxIdentifier', taxIdentifier)
          ..add('branchCode', branchCode)
          ..add('creditTermDays', creditTermDays)
          ..add('creditLimit', creditLimit)
          ..add('currencyCode', currencyCode)
          ..add('billingCycle', billingCycle)
          ..add('billingDay', billingDay)
          ..add('paymentConditionNote', paymentConditionNote)
          ..add('inactiveReason', inactiveReason))
        .toString();
  }
}

class CustomerResponseBuilder
    implements Builder<CustomerResponse, CustomerResponseBuilder> {
  _$CustomerResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _customerType;
  String? get customerType => _$this._customerType;
  set customerType(String? customerType) => _$this._customerType = customerType;

  String? _displayNameTh;
  String? get displayNameTh => _$this._displayNameTh;
  set displayNameTh(String? displayNameTh) =>
      _$this._displayNameTh = displayNameTh;

  String? _displayNameEn;
  String? get displayNameEn => _$this._displayNameEn;
  set displayNameEn(String? displayNameEn) =>
      _$this._displayNameEn = displayNameEn;

  String? _preferredLocale;
  String? get preferredLocale => _$this._preferredLocale;
  set preferredLocale(String? preferredLocale) =>
      _$this._preferredLocale = preferredLocale;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  CustomerContactResponseBuilder? _primaryContact;
  CustomerContactResponseBuilder get primaryContact =>
      _$this._primaryContact ??= CustomerContactResponseBuilder();
  set primaryContact(CustomerContactResponseBuilder? primaryContact) =>
      _$this._primaryContact = primaryContact;

  ListBuilder<DuplicateCustomerResponse>? _duplicateCandidates;
  ListBuilder<DuplicateCustomerResponse> get duplicateCandidates =>
      _$this._duplicateCandidates ??= ListBuilder<DuplicateCustomerResponse>();
  set duplicateCandidates(
          ListBuilder<DuplicateCustomerResponse>? duplicateCandidates) =>
      _$this._duplicateCandidates = duplicateCandidates;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _leadSource;
  String? get leadSource => _$this._leadSource;
  set leadSource(String? leadSource) => _$this._leadSource = leadSource;

  String? _leadSourceNote;
  String? get leadSourceNote => _$this._leadSourceNote;
  set leadSourceNote(String? leadSourceNote) =>
      _$this._leadSourceNote = leadSourceNote;

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  String? _legalName;
  String? get legalName => _$this._legalName;
  set legalName(String? legalName) => _$this._legalName = legalName;

  String? _taxIdentifier;
  String? get taxIdentifier => _$this._taxIdentifier;
  set taxIdentifier(String? taxIdentifier) =>
      _$this._taxIdentifier = taxIdentifier;

  String? _branchCode;
  String? get branchCode => _$this._branchCode;
  set branchCode(String? branchCode) => _$this._branchCode = branchCode;

  int? _creditTermDays;
  int? get creditTermDays => _$this._creditTermDays;
  set creditTermDays(int? creditTermDays) =>
      _$this._creditTermDays = creditTermDays;

  double? _creditLimit;
  double? get creditLimit => _$this._creditLimit;
  set creditLimit(double? creditLimit) => _$this._creditLimit = creditLimit;

  String? _currencyCode;
  String? get currencyCode => _$this._currencyCode;
  set currencyCode(String? currencyCode) => _$this._currencyCode = currencyCode;

  String? _billingCycle;
  String? get billingCycle => _$this._billingCycle;
  set billingCycle(String? billingCycle) => _$this._billingCycle = billingCycle;

  int? _billingDay;
  int? get billingDay => _$this._billingDay;
  set billingDay(int? billingDay) => _$this._billingDay = billingDay;

  String? _paymentConditionNote;
  String? get paymentConditionNote => _$this._paymentConditionNote;
  set paymentConditionNote(String? paymentConditionNote) =>
      _$this._paymentConditionNote = paymentConditionNote;

  String? _inactiveReason;
  String? get inactiveReason => _$this._inactiveReason;
  set inactiveReason(String? inactiveReason) =>
      _$this._inactiveReason = inactiveReason;

  CustomerResponseBuilder() {
    CustomerResponse._defaults(this);
  }

  CustomerResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _customerType = $v.customerType;
      _displayNameTh = $v.displayNameTh;
      _displayNameEn = $v.displayNameEn;
      _preferredLocale = $v.preferredLocale;
      _status = $v.status;
      _primaryContact = $v.primaryContact?.toBuilder();
      _duplicateCandidates = $v.duplicateCandidates?.toBuilder();
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _leadSource = $v.leadSource;
      _leadSourceNote = $v.leadSourceNote;
      _imageFileId = $v.imageFileId;
      _legalName = $v.legalName;
      _taxIdentifier = $v.taxIdentifier;
      _branchCode = $v.branchCode;
      _creditTermDays = $v.creditTermDays;
      _creditLimit = $v.creditLimit;
      _currencyCode = $v.currencyCode;
      _billingCycle = $v.billingCycle;
      _billingDay = $v.billingDay;
      _paymentConditionNote = $v.paymentConditionNote;
      _inactiveReason = $v.inactiveReason;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerResponse other) {
    _$v = other as _$CustomerResponse;
  }

  @override
  void update(void Function(CustomerResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerResponse build() => _build();

  _$CustomerResponse _build() {
    _$CustomerResponse _$result;
    try {
      _$result = _$v ??
          _$CustomerResponse._(
            id: id,
            code: code,
            customerType: customerType,
            displayNameTh: displayNameTh,
            displayNameEn: displayNameEn,
            preferredLocale: preferredLocale,
            status: status,
            primaryContact: _primaryContact?.build(),
            duplicateCandidates: _duplicateCandidates?.build(),
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            leadSource: leadSource,
            leadSourceNote: leadSourceNote,
            imageFileId: imageFileId,
            legalName: legalName,
            taxIdentifier: taxIdentifier,
            branchCode: branchCode,
            creditTermDays: creditTermDays,
            creditLimit: creditLimit,
            currencyCode: currencyCode,
            billingCycle: billingCycle,
            billingDay: billingDay,
            paymentConditionNote: paymentConditionNote,
            inactiveReason: inactiveReason,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'primaryContact';
        _primaryContact?.build();
        _$failedField = 'duplicateCandidates';
        _duplicateCandidates?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CustomerResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
