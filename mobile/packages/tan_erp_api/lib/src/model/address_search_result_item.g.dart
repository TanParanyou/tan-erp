// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'address_search_result_item.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AddressSearchResultItem extends AddressSearchResultItem {
  @override
  final String? subdistrictCode;
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
  final double? latitude;
  @override
  final double? longitude;
  @override
  final String? displayText;

  factory _$AddressSearchResultItem(
          [void Function(AddressSearchResultItemBuilder)? updates]) =>
      (AddressSearchResultItemBuilder()..update(updates))._build();

  _$AddressSearchResultItem._(
      {this.subdistrictCode,
      this.subdistrict,
      this.district,
      this.province,
      this.postalCode,
      this.countryCode,
      this.latitude,
      this.longitude,
      this.displayText})
      : super._();
  @override
  AddressSearchResultItem rebuild(
          void Function(AddressSearchResultItemBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AddressSearchResultItemBuilder toBuilder() =>
      AddressSearchResultItemBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AddressSearchResultItem &&
        subdistrictCode == other.subdistrictCode &&
        subdistrict == other.subdistrict &&
        district == other.district &&
        province == other.province &&
        postalCode == other.postalCode &&
        countryCode == other.countryCode &&
        latitude == other.latitude &&
        longitude == other.longitude &&
        displayText == other.displayText;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, subdistrictCode.hashCode);
    _$hash = $jc(_$hash, subdistrict.hashCode);
    _$hash = $jc(_$hash, district.hashCode);
    _$hash = $jc(_$hash, province.hashCode);
    _$hash = $jc(_$hash, postalCode.hashCode);
    _$hash = $jc(_$hash, countryCode.hashCode);
    _$hash = $jc(_$hash, latitude.hashCode);
    _$hash = $jc(_$hash, longitude.hashCode);
    _$hash = $jc(_$hash, displayText.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AddressSearchResultItem')
          ..add('subdistrictCode', subdistrictCode)
          ..add('subdistrict', subdistrict)
          ..add('district', district)
          ..add('province', province)
          ..add('postalCode', postalCode)
          ..add('countryCode', countryCode)
          ..add('latitude', latitude)
          ..add('longitude', longitude)
          ..add('displayText', displayText))
        .toString();
  }
}

class AddressSearchResultItemBuilder
    implements
        Builder<AddressSearchResultItem, AddressSearchResultItemBuilder> {
  _$AddressSearchResultItem? _$v;

  String? _subdistrictCode;
  String? get subdistrictCode => _$this._subdistrictCode;
  set subdistrictCode(String? subdistrictCode) =>
      _$this._subdistrictCode = subdistrictCode;

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

  double? _latitude;
  double? get latitude => _$this._latitude;
  set latitude(double? latitude) => _$this._latitude = latitude;

  double? _longitude;
  double? get longitude => _$this._longitude;
  set longitude(double? longitude) => _$this._longitude = longitude;

  String? _displayText;
  String? get displayText => _$this._displayText;
  set displayText(String? displayText) => _$this._displayText = displayText;

  AddressSearchResultItemBuilder() {
    AddressSearchResultItem._defaults(this);
  }

  AddressSearchResultItemBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _subdistrictCode = $v.subdistrictCode;
      _subdistrict = $v.subdistrict;
      _district = $v.district;
      _province = $v.province;
      _postalCode = $v.postalCode;
      _countryCode = $v.countryCode;
      _latitude = $v.latitude;
      _longitude = $v.longitude;
      _displayText = $v.displayText;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AddressSearchResultItem other) {
    _$v = other as _$AddressSearchResultItem;
  }

  @override
  void update(void Function(AddressSearchResultItemBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AddressSearchResultItem build() => _build();

  _$AddressSearchResultItem _build() {
    final _$result = _$v ??
        _$AddressSearchResultItem._(
          subdistrictCode: subdistrictCode,
          subdistrict: subdistrict,
          district: district,
          province: province,
          postalCode: postalCode,
          countryCode: countryCode,
          latitude: latitude,
          longitude: longitude,
          displayText: displayText,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
