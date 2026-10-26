// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_customer_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateCustomerRequest extends UpdateCustomerRequest {
  @override
  final String? customerType;
  @override
  final String? displayNameTh;
  @override
  final String? displayNameEn;
  @override
  final String? preferredLocale;
  @override
  final String? leadSource;
  @override
  final String? leadSourceNote;
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
  final bool? hasTaxIdentifier;

  factory _$UpdateCustomerRequest(
          [void Function(UpdateCustomerRequestBuilder)? updates]) =>
      (UpdateCustomerRequestBuilder()..update(updates))._build();

  _$UpdateCustomerRequest._(
      {this.customerType,
      this.displayNameTh,
      this.displayNameEn,
      this.preferredLocale,
      this.leadSource,
      this.leadSourceNote,
      this.legalName,
      this.taxIdentifier,
      this.branchCode,
      this.creditTermDays,
      this.creditLimit,
      this.currencyCode,
      this.billingCycle,
      this.billingDay,
      this.paymentConditionNote,
      this.hasTaxIdentifier})
      : super._();
  @override
  UpdateCustomerRequest rebuild(
          void Function(UpdateCustomerRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateCustomerRequestBuilder toBuilder() =>
      UpdateCustomerRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateCustomerRequest &&
        customerType == other.customerType &&
        displayNameTh == other.displayNameTh &&
        displayNameEn == other.displayNameEn &&
        preferredLocale == other.preferredLocale &&
        leadSource == other.leadSource &&
        leadSourceNote == other.leadSourceNote &&
        legalName == other.legalName &&
        taxIdentifier == other.taxIdentifier &&
        branchCode == other.branchCode &&
        creditTermDays == other.creditTermDays &&
        creditLimit == other.creditLimit &&
        currencyCode == other.currencyCode &&
        billingCycle == other.billingCycle &&
        billingDay == other.billingDay &&
        paymentConditionNote == other.paymentConditionNote &&
        hasTaxIdentifier == other.hasTaxIdentifier;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, customerType.hashCode);
    _$hash = $jc(_$hash, displayNameTh.hashCode);
    _$hash = $jc(_$hash, displayNameEn.hashCode);
    _$hash = $jc(_$hash, preferredLocale.hashCode);
    _$hash = $jc(_$hash, leadSource.hashCode);
    _$hash = $jc(_$hash, leadSourceNote.hashCode);
    _$hash = $jc(_$hash, legalName.hashCode);
    _$hash = $jc(_$hash, taxIdentifier.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, creditTermDays.hashCode);
    _$hash = $jc(_$hash, creditLimit.hashCode);
    _$hash = $jc(_$hash, currencyCode.hashCode);
    _$hash = $jc(_$hash, billingCycle.hashCode);
    _$hash = $jc(_$hash, billingDay.hashCode);
    _$hash = $jc(_$hash, paymentConditionNote.hashCode);
    _$hash = $jc(_$hash, hasTaxIdentifier.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateCustomerRequest')
          ..add('customerType', customerType)
          ..add('displayNameTh', displayNameTh)
          ..add('displayNameEn', displayNameEn)
          ..add('preferredLocale', preferredLocale)
          ..add('leadSource', leadSource)
          ..add('leadSourceNote', leadSourceNote)
          ..add('legalName', legalName)
          ..add('taxIdentifier', taxIdentifier)
          ..add('branchCode', branchCode)
          ..add('creditTermDays', creditTermDays)
          ..add('creditLimit', creditLimit)
          ..add('currencyCode', currencyCode)
          ..add('billingCycle', billingCycle)
          ..add('billingDay', billingDay)
          ..add('paymentConditionNote', paymentConditionNote)
          ..add('hasTaxIdentifier', hasTaxIdentifier))
        .toString();
  }
}

class UpdateCustomerRequestBuilder
    implements Builder<UpdateCustomerRequest, UpdateCustomerRequestBuilder> {
  _$UpdateCustomerRequest? _$v;

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

  String? _leadSource;
  String? get leadSource => _$this._leadSource;
  set leadSource(String? leadSource) => _$this._leadSource = leadSource;

  String? _leadSourceNote;
  String? get leadSourceNote => _$this._leadSourceNote;
  set leadSourceNote(String? leadSourceNote) =>
      _$this._leadSourceNote = leadSourceNote;

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

  bool? _hasTaxIdentifier;
  bool? get hasTaxIdentifier => _$this._hasTaxIdentifier;
  set hasTaxIdentifier(bool? hasTaxIdentifier) =>
      _$this._hasTaxIdentifier = hasTaxIdentifier;

  UpdateCustomerRequestBuilder() {
    UpdateCustomerRequest._defaults(this);
  }

  UpdateCustomerRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _customerType = $v.customerType;
      _displayNameTh = $v.displayNameTh;
      _displayNameEn = $v.displayNameEn;
      _preferredLocale = $v.preferredLocale;
      _leadSource = $v.leadSource;
      _leadSourceNote = $v.leadSourceNote;
      _legalName = $v.legalName;
      _taxIdentifier = $v.taxIdentifier;
      _branchCode = $v.branchCode;
      _creditTermDays = $v.creditTermDays;
      _creditLimit = $v.creditLimit;
      _currencyCode = $v.currencyCode;
      _billingCycle = $v.billingCycle;
      _billingDay = $v.billingDay;
      _paymentConditionNote = $v.paymentConditionNote;
      _hasTaxIdentifier = $v.hasTaxIdentifier;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateCustomerRequest other) {
    _$v = other as _$UpdateCustomerRequest;
  }

  @override
  void update(void Function(UpdateCustomerRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateCustomerRequest build() => _build();

  _$UpdateCustomerRequest _build() {
    final _$result = _$v ??
        _$UpdateCustomerRequest._(
          customerType: customerType,
          displayNameTh: displayNameTh,
          displayNameEn: displayNameEn,
          preferredLocale: preferredLocale,
          leadSource: leadSource,
          leadSourceNote: leadSourceNote,
          legalName: legalName,
          taxIdentifier: taxIdentifier,
          branchCode: branchCode,
          creditTermDays: creditTermDays,
          creditLimit: creditLimit,
          currencyCode: currencyCode,
          billingCycle: billingCycle,
          billingDay: billingDay,
          paymentConditionNote: paymentConditionNote,
          hasTaxIdentifier: hasTaxIdentifier,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
