// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_customer_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationCustomerResponse extends QuotationCustomerResponse {
  @override
  final String? customerType;
  @override
  final String? displayName;
  @override
  final String? displayNameTh;
  @override
  final String? displayNameEn;
  @override
  final String? legalName;
  @override
  final String? taxIdentifier;
  @override
  final String? branchCode;
  @override
  final QuotationAddressResponse? address;

  factory _$QuotationCustomerResponse(
          [void Function(QuotationCustomerResponseBuilder)? updates]) =>
      (QuotationCustomerResponseBuilder()..update(updates))._build();

  _$QuotationCustomerResponse._(
      {this.customerType,
      this.displayName,
      this.displayNameTh,
      this.displayNameEn,
      this.legalName,
      this.taxIdentifier,
      this.branchCode,
      this.address})
      : super._();
  @override
  QuotationCustomerResponse rebuild(
          void Function(QuotationCustomerResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationCustomerResponseBuilder toBuilder() =>
      QuotationCustomerResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationCustomerResponse &&
        customerType == other.customerType &&
        displayName == other.displayName &&
        displayNameTh == other.displayNameTh &&
        displayNameEn == other.displayNameEn &&
        legalName == other.legalName &&
        taxIdentifier == other.taxIdentifier &&
        branchCode == other.branchCode &&
        address == other.address;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, customerType.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, displayNameTh.hashCode);
    _$hash = $jc(_$hash, displayNameEn.hashCode);
    _$hash = $jc(_$hash, legalName.hashCode);
    _$hash = $jc(_$hash, taxIdentifier.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, address.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationCustomerResponse')
          ..add('customerType', customerType)
          ..add('displayName', displayName)
          ..add('displayNameTh', displayNameTh)
          ..add('displayNameEn', displayNameEn)
          ..add('legalName', legalName)
          ..add('taxIdentifier', taxIdentifier)
          ..add('branchCode', branchCode)
          ..add('address', address))
        .toString();
  }
}

class QuotationCustomerResponseBuilder
    implements
        Builder<QuotationCustomerResponse, QuotationCustomerResponseBuilder> {
  _$QuotationCustomerResponse? _$v;

  String? _customerType;
  String? get customerType => _$this._customerType;
  set customerType(String? customerType) => _$this._customerType = customerType;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _displayNameTh;
  String? get displayNameTh => _$this._displayNameTh;
  set displayNameTh(String? displayNameTh) =>
      _$this._displayNameTh = displayNameTh;

  String? _displayNameEn;
  String? get displayNameEn => _$this._displayNameEn;
  set displayNameEn(String? displayNameEn) =>
      _$this._displayNameEn = displayNameEn;

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

  QuotationAddressResponseBuilder? _address;
  QuotationAddressResponseBuilder get address =>
      _$this._address ??= QuotationAddressResponseBuilder();
  set address(QuotationAddressResponseBuilder? address) =>
      _$this._address = address;

  QuotationCustomerResponseBuilder() {
    QuotationCustomerResponse._defaults(this);
  }

  QuotationCustomerResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _customerType = $v.customerType;
      _displayName = $v.displayName;
      _displayNameTh = $v.displayNameTh;
      _displayNameEn = $v.displayNameEn;
      _legalName = $v.legalName;
      _taxIdentifier = $v.taxIdentifier;
      _branchCode = $v.branchCode;
      _address = $v.address?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationCustomerResponse other) {
    _$v = other as _$QuotationCustomerResponse;
  }

  @override
  void update(void Function(QuotationCustomerResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationCustomerResponse build() => _build();

  _$QuotationCustomerResponse _build() {
    _$QuotationCustomerResponse _$result;
    try {
      _$result = _$v ??
          _$QuotationCustomerResponse._(
            customerType: customerType,
            displayName: displayName,
            displayNameTh: displayNameTh,
            displayNameEn: displayNameEn,
            legalName: legalName,
            taxIdentifier: taxIdentifier,
            branchCode: branchCode,
            address: _address?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'address';
        _address?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'QuotationCustomerResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
