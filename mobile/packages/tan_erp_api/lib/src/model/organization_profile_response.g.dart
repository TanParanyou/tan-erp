// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'organization_profile_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OrganizationProfileResponse extends OrganizationProfileResponse {
  @override
  final String? id;
  @override
  final String? name;
  @override
  final String? nameEn;
  @override
  final String? taxIdentifier;
  @override
  final String? addressTh;
  @override
  final String? addressEn;
  @override
  final String? phone;
  @override
  final String? rowVersion;

  factory _$OrganizationProfileResponse(
          [void Function(OrganizationProfileResponseBuilder)? updates]) =>
      (OrganizationProfileResponseBuilder()..update(updates))._build();

  _$OrganizationProfileResponse._(
      {this.id,
      this.name,
      this.nameEn,
      this.taxIdentifier,
      this.addressTh,
      this.addressEn,
      this.phone,
      this.rowVersion})
      : super._();
  @override
  OrganizationProfileResponse rebuild(
          void Function(OrganizationProfileResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OrganizationProfileResponseBuilder toBuilder() =>
      OrganizationProfileResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OrganizationProfileResponse &&
        id == other.id &&
        name == other.name &&
        nameEn == other.nameEn &&
        taxIdentifier == other.taxIdentifier &&
        addressTh == other.addressTh &&
        addressEn == other.addressEn &&
        phone == other.phone &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, taxIdentifier.hashCode);
    _$hash = $jc(_$hash, addressTh.hashCode);
    _$hash = $jc(_$hash, addressEn.hashCode);
    _$hash = $jc(_$hash, phone.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OrganizationProfileResponse')
          ..add('id', id)
          ..add('name', name)
          ..add('nameEn', nameEn)
          ..add('taxIdentifier', taxIdentifier)
          ..add('addressTh', addressTh)
          ..add('addressEn', addressEn)
          ..add('phone', phone)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class OrganizationProfileResponseBuilder
    implements
        Builder<OrganizationProfileResponse,
            OrganizationProfileResponseBuilder> {
  _$OrganizationProfileResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _nameEn;
  String? get nameEn => _$this._nameEn;
  set nameEn(String? nameEn) => _$this._nameEn = nameEn;

  String? _taxIdentifier;
  String? get taxIdentifier => _$this._taxIdentifier;
  set taxIdentifier(String? taxIdentifier) =>
      _$this._taxIdentifier = taxIdentifier;

  String? _addressTh;
  String? get addressTh => _$this._addressTh;
  set addressTh(String? addressTh) => _$this._addressTh = addressTh;

  String? _addressEn;
  String? get addressEn => _$this._addressEn;
  set addressEn(String? addressEn) => _$this._addressEn = addressEn;

  String? _phone;
  String? get phone => _$this._phone;
  set phone(String? phone) => _$this._phone = phone;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  OrganizationProfileResponseBuilder() {
    OrganizationProfileResponse._defaults(this);
  }

  OrganizationProfileResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name;
      _nameEn = $v.nameEn;
      _taxIdentifier = $v.taxIdentifier;
      _addressTh = $v.addressTh;
      _addressEn = $v.addressEn;
      _phone = $v.phone;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OrganizationProfileResponse other) {
    _$v = other as _$OrganizationProfileResponse;
  }

  @override
  void update(void Function(OrganizationProfileResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OrganizationProfileResponse build() => _build();

  _$OrganizationProfileResponse _build() {
    final _$result = _$v ??
        _$OrganizationProfileResponse._(
          id: id,
          name: name,
          nameEn: nameEn,
          taxIdentifier: taxIdentifier,
          addressTh: addressTh,
          addressEn: addressEn,
          phone: phone,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
