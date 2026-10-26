// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'site_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SiteResponse extends SiteResponse {
  @override
  final String? id;
  @override
  final String? customerId;
  @override
  final String? code;
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
  final double? latitude;
  @override
  final double? longitude;
  @override
  final String? accessNote;
  @override
  final String? status;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final BuiltList<SiteImageResponse>? images;

  factory _$SiteResponse([void Function(SiteResponseBuilder)? updates]) =>
      (SiteResponseBuilder()..update(updates))._build();

  _$SiteResponse._(
      {this.id,
      this.customerId,
      this.code,
      this.label,
      this.addressLine1,
      this.subdistrict,
      this.district,
      this.province,
      this.postalCode,
      this.countryCode,
      this.latitude,
      this.longitude,
      this.accessNote,
      this.status,
      this.rowVersion,
      this.createdAtUtc,
      this.images})
      : super._();
  @override
  SiteResponse rebuild(void Function(SiteResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SiteResponseBuilder toBuilder() => SiteResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SiteResponse &&
        id == other.id &&
        customerId == other.customerId &&
        code == other.code &&
        label == other.label &&
        addressLine1 == other.addressLine1 &&
        subdistrict == other.subdistrict &&
        district == other.district &&
        province == other.province &&
        postalCode == other.postalCode &&
        countryCode == other.countryCode &&
        latitude == other.latitude &&
        longitude == other.longitude &&
        accessNote == other.accessNote &&
        status == other.status &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        images == other.images;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, label.hashCode);
    _$hash = $jc(_$hash, addressLine1.hashCode);
    _$hash = $jc(_$hash, subdistrict.hashCode);
    _$hash = $jc(_$hash, district.hashCode);
    _$hash = $jc(_$hash, province.hashCode);
    _$hash = $jc(_$hash, postalCode.hashCode);
    _$hash = $jc(_$hash, countryCode.hashCode);
    _$hash = $jc(_$hash, latitude.hashCode);
    _$hash = $jc(_$hash, longitude.hashCode);
    _$hash = $jc(_$hash, accessNote.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, images.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SiteResponse')
          ..add('id', id)
          ..add('customerId', customerId)
          ..add('code', code)
          ..add('label', label)
          ..add('addressLine1', addressLine1)
          ..add('subdistrict', subdistrict)
          ..add('district', district)
          ..add('province', province)
          ..add('postalCode', postalCode)
          ..add('countryCode', countryCode)
          ..add('latitude', latitude)
          ..add('longitude', longitude)
          ..add('accessNote', accessNote)
          ..add('status', status)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('images', images))
        .toString();
  }
}

class SiteResponseBuilder
    implements Builder<SiteResponse, SiteResponseBuilder> {
  _$SiteResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

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

  double? _latitude;
  double? get latitude => _$this._latitude;
  set latitude(double? latitude) => _$this._latitude = latitude;

  double? _longitude;
  double? get longitude => _$this._longitude;
  set longitude(double? longitude) => _$this._longitude = longitude;

  String? _accessNote;
  String? get accessNote => _$this._accessNote;
  set accessNote(String? accessNote) => _$this._accessNote = accessNote;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  ListBuilder<SiteImageResponse>? _images;
  ListBuilder<SiteImageResponse> get images =>
      _$this._images ??= ListBuilder<SiteImageResponse>();
  set images(ListBuilder<SiteImageResponse>? images) => _$this._images = images;

  SiteResponseBuilder() {
    SiteResponse._defaults(this);
  }

  SiteResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _customerId = $v.customerId;
      _code = $v.code;
      _label = $v.label;
      _addressLine1 = $v.addressLine1;
      _subdistrict = $v.subdistrict;
      _district = $v.district;
      _province = $v.province;
      _postalCode = $v.postalCode;
      _countryCode = $v.countryCode;
      _latitude = $v.latitude;
      _longitude = $v.longitude;
      _accessNote = $v.accessNote;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _images = $v.images?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SiteResponse other) {
    _$v = other as _$SiteResponse;
  }

  @override
  void update(void Function(SiteResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SiteResponse build() => _build();

  _$SiteResponse _build() {
    _$SiteResponse _$result;
    try {
      _$result = _$v ??
          _$SiteResponse._(
            id: id,
            customerId: customerId,
            code: code,
            label: label,
            addressLine1: addressLine1,
            subdistrict: subdistrict,
            district: district,
            province: province,
            postalCode: postalCode,
            countryCode: countryCode,
            latitude: latitude,
            longitude: longitude,
            accessNote: accessNote,
            status: status,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            images: _images?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'images';
        _images?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SiteResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
