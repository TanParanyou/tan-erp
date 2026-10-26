// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_address_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerAddressResponse extends CustomerAddressResponse {
  @override
  final String? id;
  @override
  final String? customerId;
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
  final String? status;
  @override
  final bool? isPrimary;
  @override
  final String? rowVersion;

  factory _$CustomerAddressResponse(
          [void Function(CustomerAddressResponseBuilder)? updates]) =>
      (CustomerAddressResponseBuilder()..update(updates))._build();

  _$CustomerAddressResponse._(
      {this.id,
      this.customerId,
      this.addressType,
      this.label,
      this.addressLine1,
      this.subdistrict,
      this.district,
      this.province,
      this.postalCode,
      this.countryCode,
      this.status,
      this.isPrimary,
      this.rowVersion})
      : super._();
  @override
  CustomerAddressResponse rebuild(
          void Function(CustomerAddressResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerAddressResponseBuilder toBuilder() =>
      CustomerAddressResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerAddressResponse &&
        id == other.id &&
        customerId == other.customerId &&
        addressType == other.addressType &&
        label == other.label &&
        addressLine1 == other.addressLine1 &&
        subdistrict == other.subdistrict &&
        district == other.district &&
        province == other.province &&
        postalCode == other.postalCode &&
        countryCode == other.countryCode &&
        status == other.status &&
        isPrimary == other.isPrimary &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, addressType.hashCode);
    _$hash = $jc(_$hash, label.hashCode);
    _$hash = $jc(_$hash, addressLine1.hashCode);
    _$hash = $jc(_$hash, subdistrict.hashCode);
    _$hash = $jc(_$hash, district.hashCode);
    _$hash = $jc(_$hash, province.hashCode);
    _$hash = $jc(_$hash, postalCode.hashCode);
    _$hash = $jc(_$hash, countryCode.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, isPrimary.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CustomerAddressResponse')
          ..add('id', id)
          ..add('customerId', customerId)
          ..add('addressType', addressType)
          ..add('label', label)
          ..add('addressLine1', addressLine1)
          ..add('subdistrict', subdistrict)
          ..add('district', district)
          ..add('province', province)
          ..add('postalCode', postalCode)
          ..add('countryCode', countryCode)
          ..add('status', status)
          ..add('isPrimary', isPrimary)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class CustomerAddressResponseBuilder
    implements
        Builder<CustomerAddressResponse, CustomerAddressResponseBuilder> {
  _$CustomerAddressResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

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

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  bool? _isPrimary;
  bool? get isPrimary => _$this._isPrimary;
  set isPrimary(bool? isPrimary) => _$this._isPrimary = isPrimary;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  CustomerAddressResponseBuilder() {
    CustomerAddressResponse._defaults(this);
  }

  CustomerAddressResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _customerId = $v.customerId;
      _addressType = $v.addressType;
      _label = $v.label;
      _addressLine1 = $v.addressLine1;
      _subdistrict = $v.subdistrict;
      _district = $v.district;
      _province = $v.province;
      _postalCode = $v.postalCode;
      _countryCode = $v.countryCode;
      _status = $v.status;
      _isPrimary = $v.isPrimary;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerAddressResponse other) {
    _$v = other as _$CustomerAddressResponse;
  }

  @override
  void update(void Function(CustomerAddressResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerAddressResponse build() => _build();

  _$CustomerAddressResponse _build() {
    final _$result = _$v ??
        _$CustomerAddressResponse._(
          id: id,
          customerId: customerId,
          addressType: addressType,
          label: label,
          addressLine1: addressLine1,
          subdistrict: subdistrict,
          district: district,
          province: province,
          postalCode: postalCode,
          countryCode: countryCode,
          status: status,
          isPrimary: isPrimary,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
