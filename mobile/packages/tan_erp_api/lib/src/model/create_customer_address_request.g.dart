// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_customer_address_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateCustomerAddressRequest extends CreateCustomerAddressRequest {
  @override
  final String? addressType;
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
  @override
  final bool? isPrimary;

  factory _$CreateCustomerAddressRequest(
          [void Function(CreateCustomerAddressRequestBuilder)? updates]) =>
      (CreateCustomerAddressRequestBuilder()..update(updates))._build();

  _$CreateCustomerAddressRequest._(
      {this.addressType,
      this.label,
      this.addressLine1,
      this.subdistrict,
      this.district,
      this.province,
      this.postalCode,
      this.countryCode,
      this.isPrimary})
      : super._();
  @override
  CreateCustomerAddressRequest rebuild(
          void Function(CreateCustomerAddressRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateCustomerAddressRequestBuilder toBuilder() =>
      CreateCustomerAddressRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateCustomerAddressRequest &&
        addressType == other.addressType &&
        label == other.label &&
        addressLine1 == other.addressLine1 &&
        subdistrict == other.subdistrict &&
        district == other.district &&
        province == other.province &&
        postalCode == other.postalCode &&
        countryCode == other.countryCode &&
        isPrimary == other.isPrimary;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, addressType.hashCode);
    _$hash = $jc(_$hash, label.hashCode);
    _$hash = $jc(_$hash, addressLine1.hashCode);
    _$hash = $jc(_$hash, subdistrict.hashCode);
    _$hash = $jc(_$hash, district.hashCode);
    _$hash = $jc(_$hash, province.hashCode);
    _$hash = $jc(_$hash, postalCode.hashCode);
    _$hash = $jc(_$hash, countryCode.hashCode);
    _$hash = $jc(_$hash, isPrimary.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateCustomerAddressRequest')
          ..add('addressType', addressType)
          ..add('label', label)
          ..add('addressLine1', addressLine1)
          ..add('subdistrict', subdistrict)
          ..add('district', district)
          ..add('province', province)
          ..add('postalCode', postalCode)
          ..add('countryCode', countryCode)
          ..add('isPrimary', isPrimary))
        .toString();
  }
}

class CreateCustomerAddressRequestBuilder
    implements
        Builder<CreateCustomerAddressRequest,
            CreateCustomerAddressRequestBuilder> {
  _$CreateCustomerAddressRequest? _$v;

  String? _addressType;
  String? get addressType => _$this._addressType;
  set addressType(String? addressType) => _$this._addressType = addressType;

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

  bool? _isPrimary;
  bool? get isPrimary => _$this._isPrimary;
  set isPrimary(bool? isPrimary) => _$this._isPrimary = isPrimary;

  CreateCustomerAddressRequestBuilder() {
    CreateCustomerAddressRequest._defaults(this);
  }

  CreateCustomerAddressRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _addressType = $v.addressType;
      _label = $v.label;
      _addressLine1 = $v.addressLine1;
      _subdistrict = $v.subdistrict;
      _district = $v.district;
      _province = $v.province;
      _postalCode = $v.postalCode;
      _countryCode = $v.countryCode;
      _isPrimary = $v.isPrimary;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateCustomerAddressRequest other) {
    _$v = other as _$CreateCustomerAddressRequest;
  }

  @override
  void update(void Function(CreateCustomerAddressRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateCustomerAddressRequest build() => _build();

  _$CreateCustomerAddressRequest _build() {
    final _$result = _$v ??
        _$CreateCustomerAddressRequest._(
          addressType: addressType,
          label: label,
          addressLine1: addressLine1,
          subdistrict: subdistrict,
          district: district,
          province: province,
          postalCode: postalCode,
          countryCode: countryCode,
          isPrimary: isPrimary,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
