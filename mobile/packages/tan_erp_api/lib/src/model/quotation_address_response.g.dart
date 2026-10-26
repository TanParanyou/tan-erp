// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_address_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationAddressResponse extends QuotationAddressResponse {
  @override
  final String? label;
  @override
  final String? addressLine1;
  @override
  final String? subdistrict;
  @override
  final String? district;
  @override
  final String? province;
  @override
  final String? postalCode;
  @override
  final String? countryCode;

  factory _$QuotationAddressResponse(
          [void Function(QuotationAddressResponseBuilder)? updates]) =>
      (QuotationAddressResponseBuilder()..update(updates))._build();

  _$QuotationAddressResponse._(
      {this.label,
      this.addressLine1,
      this.subdistrict,
      this.district,
      this.province,
      this.postalCode,
      this.countryCode})
      : super._();
  @override
  QuotationAddressResponse rebuild(
          void Function(QuotationAddressResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationAddressResponseBuilder toBuilder() =>
      QuotationAddressResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationAddressResponse &&
        label == other.label &&
        addressLine1 == other.addressLine1 &&
        subdistrict == other.subdistrict &&
        district == other.district &&
        province == other.province &&
        postalCode == other.postalCode &&
        countryCode == other.countryCode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, label.hashCode);
    _$hash = $jc(_$hash, addressLine1.hashCode);
    _$hash = $jc(_$hash, subdistrict.hashCode);
    _$hash = $jc(_$hash, district.hashCode);
    _$hash = $jc(_$hash, province.hashCode);
    _$hash = $jc(_$hash, postalCode.hashCode);
    _$hash = $jc(_$hash, countryCode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationAddressResponse')
          ..add('label', label)
          ..add('addressLine1', addressLine1)
          ..add('subdistrict', subdistrict)
          ..add('district', district)
          ..add('province', province)
          ..add('postalCode', postalCode)
          ..add('countryCode', countryCode))
        .toString();
  }
}

class QuotationAddressResponseBuilder
    implements
        Builder<QuotationAddressResponse, QuotationAddressResponseBuilder> {
  _$QuotationAddressResponse? _$v;

  String? _label;
  String? get label => _$this._label;
  set label(String? label) => _$this._label = label;

  String? _addressLine1;
  String? get addressLine1 => _$this._addressLine1;
  set addressLine1(String? addressLine1) => _$this._addressLine1 = addressLine1;

  String? _subdistrict;
  String? get subdistrict => _$this._subdistrict;
  set subdistrict(String? subdistrict) => _$this._subdistrict = subdistrict;

  String? _district;
  String? get district => _$this._district;
  set district(String? district) => _$this._district = district;

  String? _province;
  String? get province => _$this._province;
  set province(String? province) => _$this._province = province;

  String? _postalCode;
  String? get postalCode => _$this._postalCode;
  set postalCode(String? postalCode) => _$this._postalCode = postalCode;

  String? _countryCode;
  String? get countryCode => _$this._countryCode;
  set countryCode(String? countryCode) => _$this._countryCode = countryCode;

  QuotationAddressResponseBuilder() {
    QuotationAddressResponse._defaults(this);
  }

  QuotationAddressResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _label = $v.label;
      _addressLine1 = $v.addressLine1;
      _subdistrict = $v.subdistrict;
      _district = $v.district;
      _province = $v.province;
      _postalCode = $v.postalCode;
      _countryCode = $v.countryCode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationAddressResponse other) {
    _$v = other as _$QuotationAddressResponse;
  }

  @override
  void update(void Function(QuotationAddressResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationAddressResponse build() => _build();

  _$QuotationAddressResponse _build() {
    final _$result = _$v ??
        _$QuotationAddressResponse._(
          label: label,
          addressLine1: addressLine1,
          subdistrict: subdistrict,
          district: district,
          province: province,
          postalCode: postalCode,
          countryCode: countryCode,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
