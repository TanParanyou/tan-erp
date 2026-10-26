// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_organization_profile_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateOrganizationProfileRequest
    extends UpdateOrganizationProfileRequest {
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

  factory _$UpdateOrganizationProfileRequest(
          [void Function(UpdateOrganizationProfileRequestBuilder)? updates]) =>
      (UpdateOrganizationProfileRequestBuilder()..update(updates))._build();

  _$UpdateOrganizationProfileRequest._(
      {this.name,
      this.nameEn,
      this.taxIdentifier,
      this.addressTh,
      this.addressEn,
      this.phone})
      : super._();
  @override
  UpdateOrganizationProfileRequest rebuild(
          void Function(UpdateOrganizationProfileRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateOrganizationProfileRequestBuilder toBuilder() =>
      UpdateOrganizationProfileRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateOrganizationProfileRequest &&
        name == other.name &&
        nameEn == other.nameEn &&
        taxIdentifier == other.taxIdentifier &&
        addressTh == other.addressTh &&
        addressEn == other.addressEn &&
        phone == other.phone;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, taxIdentifier.hashCode);
    _$hash = $jc(_$hash, addressTh.hashCode);
    _$hash = $jc(_$hash, addressEn.hashCode);
    _$hash = $jc(_$hash, phone.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateOrganizationProfileRequest')
          ..add('name', name)
          ..add('nameEn', nameEn)
          ..add('taxIdentifier', taxIdentifier)
          ..add('addressTh', addressTh)
          ..add('addressEn', addressEn)
          ..add('phone', phone))
        .toString();
  }
}

class UpdateOrganizationProfileRequestBuilder
    implements
        Builder<UpdateOrganizationProfileRequest,
            UpdateOrganizationProfileRequestBuilder> {
  _$UpdateOrganizationProfileRequest? _$v;

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

  UpdateOrganizationProfileRequestBuilder() {
    UpdateOrganizationProfileRequest._defaults(this);
  }

  UpdateOrganizationProfileRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _name = $v.name;
      _nameEn = $v.nameEn;
      _taxIdentifier = $v.taxIdentifier;
      _addressTh = $v.addressTh;
      _addressEn = $v.addressEn;
      _phone = $v.phone;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateOrganizationProfileRequest other) {
    _$v = other as _$UpdateOrganizationProfileRequest;
  }

  @override
  void update(void Function(UpdateOrganizationProfileRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateOrganizationProfileRequest build() => _build();

  _$UpdateOrganizationProfileRequest _build() {
    final _$result = _$v ??
        _$UpdateOrganizationProfileRequest._(
          name: name,
          nameEn: nameEn,
          taxIdentifier: taxIdentifier,
          addressTh: addressTh,
          addressEn: addressEn,
          phone: phone,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
