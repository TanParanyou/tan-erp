// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_site_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateSiteRequest extends CreateSiteRequest {
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
  final BuiltList<CreateSiteImageRequest>? images;
  @override
  final String? fileUploadIntentId;

  factory _$CreateSiteRequest(
          [void Function(CreateSiteRequestBuilder)? updates]) =>
      (CreateSiteRequestBuilder()..update(updates))._build();

  _$CreateSiteRequest._(
      {this.label,
      this.addressLine1,
      this.subdistrict,
      this.district,
      this.province,
      this.postalCode,
      this.countryCode,
      this.latitude,
      this.longitude,
      this.accessNote,
      this.images,
      this.fileUploadIntentId})
      : super._();
  @override
  CreateSiteRequest rebuild(void Function(CreateSiteRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateSiteRequestBuilder toBuilder() =>
      CreateSiteRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateSiteRequest &&
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
        images == other.images &&
        fileUploadIntentId == other.fileUploadIntentId;
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
    _$hash = $jc(_$hash, latitude.hashCode);
    _$hash = $jc(_$hash, longitude.hashCode);
    _$hash = $jc(_$hash, accessNote.hashCode);
    _$hash = $jc(_$hash, images.hashCode);
    _$hash = $jc(_$hash, fileUploadIntentId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateSiteRequest')
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
          ..add('images', images)
          ..add('fileUploadIntentId', fileUploadIntentId))
        .toString();
  }
}

class CreateSiteRequestBuilder
    implements Builder<CreateSiteRequest, CreateSiteRequestBuilder> {
  _$CreateSiteRequest? _$v;

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

  ListBuilder<CreateSiteImageRequest>? _images;
  ListBuilder<CreateSiteImageRequest> get images =>
      _$this._images ??= ListBuilder<CreateSiteImageRequest>();
  set images(ListBuilder<CreateSiteImageRequest>? images) =>
      _$this._images = images;

  String? _fileUploadIntentId;
  String? get fileUploadIntentId => _$this._fileUploadIntentId;
  set fileUploadIntentId(String? fileUploadIntentId) =>
      _$this._fileUploadIntentId = fileUploadIntentId;

  CreateSiteRequestBuilder() {
    CreateSiteRequest._defaults(this);
  }

  CreateSiteRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
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
      _images = $v.images?.toBuilder();
      _fileUploadIntentId = $v.fileUploadIntentId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateSiteRequest other) {
    _$v = other as _$CreateSiteRequest;
  }

  @override
  void update(void Function(CreateSiteRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateSiteRequest build() => _build();

  _$CreateSiteRequest _build() {
    _$CreateSiteRequest _$result;
    try {
      _$result = _$v ??
          _$CreateSiteRequest._(
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
            images: _images?.build(),
            fileUploadIntentId: fileUploadIntentId,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'images';
        _images?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreateSiteRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
