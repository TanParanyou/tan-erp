// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteSummaryResponse extends SiteSummaryResponse {
  @override
  final String? id;
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

  factory _$SiteSummaryResponse(
          [void Function(SiteSummaryResponseBuilder)? updates]) =>
      (SiteSummaryResponseBuilder()..update(updates))._build();

  _$SiteSummaryResponse._(
      {this.id,
      this.label,
      this.addressLine1,
      this.subdistrict,
      this.district,
      this.province,
      this.postalCode})
      : super._();
  @override
  SiteSummaryResponse rebuild(
          void Function(SiteSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteSummaryResponseBuilder toBuilder() =>
      SiteSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteSummaryResponse &&
        id == other.id &&
        label == other.label &&
        addressLine1 == other.addressLine1 &&
        subdistrict == other.subdistrict &&
        district == other.district &&
        province == other.province &&
        postalCode == other.postalCode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, label.hashCode);
    _$hash = $jc(_$hash, addressLine1.hashCode);
    _$hash = $jc(_$hash, subdistrict.hashCode);
    _$hash = $jc(_$hash, district.hashCode);
    _$hash = $jc(_$hash, province.hashCode);
    _$hash = $jc(_$hash, postalCode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteSummaryResponse')
          ..add('id', id)
          ..add('label', label)
          ..add('addressLine1', addressLine1)
          ..add('subdistrict', subdistrict)
          ..add('district', district)
          ..add('province', province)
          ..add('postalCode', postalCode))
        .toString();
  }
}

class SiteSummaryResponseBuilder
    implements Builder<SiteSummaryResponse, SiteSummaryResponseBuilder> {
  _$SiteSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

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

  SiteSummaryResponseBuilder() {
    SiteSummaryResponse._defaults(this);
  }

  SiteSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _label = $v.label;
      _addressLine1 = $v.addressLine1;
      _subdistrict = $v.subdistrict;
      _district = $v.district;
      _province = $v.province;
      _postalCode = $v.postalCode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteSummaryResponse other) {
    _$v = other as _$SiteSummaryResponse;
  }

  @override
  void update(void Function(SiteSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteSummaryResponse build() => _build();

  _$SiteSummaryResponse _build() {
    final _$result = _$v ??
        _$SiteSummaryResponse._(
          id: id,
          label: label,
          addressLine1: addressLine1,
          subdistrict: subdistrict,
          district: district,
          province: province,
          postalCode: postalCode,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
