// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_customer_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateCustomerRequest extends CreateCustomerRequest {
  @override
  final String? customerType;
  @override
  final String? displayNameTh;
  @override
  final String? displayNameEn;
  @override
  final String? preferredLocale;
  @override
  final CreatePrimaryContactRequest? primaryContact;
  @override
  final String? leadSource;
  @override
  final String? leadSourceNote;
  @override
  final String? imageFileId;
  @override
  final String? fileUploadIntentId;

  factory _$CreateCustomerRequest(
          [void Function(CreateCustomerRequestBuilder)? updates]) =>
      (CreateCustomerRequestBuilder()..update(updates))._build();

  _$CreateCustomerRequest._(
      {this.customerType,
      this.displayNameTh,
      this.displayNameEn,
      this.preferredLocale,
      this.primaryContact,
      this.leadSource,
      this.leadSourceNote,
      this.imageFileId,
      this.fileUploadIntentId})
      : super._();
  @override
  CreateCustomerRequest rebuild(
          void Function(CreateCustomerRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateCustomerRequestBuilder toBuilder() =>
      CreateCustomerRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateCustomerRequest &&
        customerType == other.customerType &&
        displayNameTh == other.displayNameTh &&
        displayNameEn == other.displayNameEn &&
        preferredLocale == other.preferredLocale &&
        primaryContact == other.primaryContact &&
        leadSource == other.leadSource &&
        leadSourceNote == other.leadSourceNote &&
        imageFileId == other.imageFileId &&
        fileUploadIntentId == other.fileUploadIntentId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, customerType.hashCode);
    _$hash = $jc(_$hash, displayNameTh.hashCode);
    _$hash = $jc(_$hash, displayNameEn.hashCode);
    _$hash = $jc(_$hash, preferredLocale.hashCode);
    _$hash = $jc(_$hash, primaryContact.hashCode);
    _$hash = $jc(_$hash, leadSource.hashCode);
    _$hash = $jc(_$hash, leadSourceNote.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jc(_$hash, fileUploadIntentId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateCustomerRequest')
          ..add('customerType', customerType)
          ..add('displayNameTh', displayNameTh)
          ..add('displayNameEn', displayNameEn)
          ..add('preferredLocale', preferredLocale)
          ..add('primaryContact', primaryContact)
          ..add('leadSource', leadSource)
          ..add('leadSourceNote', leadSourceNote)
          ..add('imageFileId', imageFileId)
          ..add('fileUploadIntentId', fileUploadIntentId))
        .toString();
  }
}

class CreateCustomerRequestBuilder
    implements Builder<CreateCustomerRequest, CreateCustomerRequestBuilder> {
  _$CreateCustomerRequest? _$v;

  String? _customerType;
  String? get customerType => _$this._customerType;
  set customerType(String? customerType) => _$this._customerType = customerType;

  String? _displayNameTh;
  String? get displayNameTh => _$this._displayNameTh;
  set displayNameTh(String? displayNameTh) =>
      _$this._displayNameTh = displayNameTh;

  String? _displayNameEn;
  String? get displayNameEn => _$this._displayNameEn;
  set displayNameEn(String? displayNameEn) =>
      _$this._displayNameEn = displayNameEn;

  String? _preferredLocale;
  String? get preferredLocale => _$this._preferredLocale;
  set preferredLocale(String? preferredLocale) =>
      _$this._preferredLocale = preferredLocale;

  CreatePrimaryContactRequestBuilder? _primaryContact;
  CreatePrimaryContactRequestBuilder get primaryContact =>
      _$this._primaryContact ??= CreatePrimaryContactRequestBuilder();
  set primaryContact(CreatePrimaryContactRequestBuilder? primaryContact) =>
      _$this._primaryContact = primaryContact;

  String? _leadSource;
  String? get leadSource => _$this._leadSource;
  set leadSource(String? leadSource) => _$this._leadSource = leadSource;

  String? _leadSourceNote;
  String? get leadSourceNote => _$this._leadSourceNote;
  set leadSourceNote(String? leadSourceNote) =>
      _$this._leadSourceNote = leadSourceNote;

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  String? _fileUploadIntentId;
  String? get fileUploadIntentId => _$this._fileUploadIntentId;
  set fileUploadIntentId(String? fileUploadIntentId) =>
      _$this._fileUploadIntentId = fileUploadIntentId;

  CreateCustomerRequestBuilder() {
    CreateCustomerRequest._defaults(this);
  }

  CreateCustomerRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _customerType = $v.customerType;
      _displayNameTh = $v.displayNameTh;
      _displayNameEn = $v.displayNameEn;
      _preferredLocale = $v.preferredLocale;
      _primaryContact = $v.primaryContact?.toBuilder();
      _leadSource = $v.leadSource;
      _leadSourceNote = $v.leadSourceNote;
      _imageFileId = $v.imageFileId;
      _fileUploadIntentId = $v.fileUploadIntentId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateCustomerRequest other) {
    _$v = other as _$CreateCustomerRequest;
  }

  @override
  void update(void Function(CreateCustomerRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateCustomerRequest build() => _build();

  _$CreateCustomerRequest _build() {
    _$CreateCustomerRequest _$result;
    try {
      _$result = _$v ??
          _$CreateCustomerRequest._(
            customerType: customerType,
            displayNameTh: displayNameTh,
            displayNameEn: displayNameEn,
            preferredLocale: preferredLocale,
            primaryContact: _primaryContact?.build(),
            leadSource: leadSource,
            leadSourceNote: leadSourceNote,
            imageFileId: imageFileId,
            fileUploadIntentId: fileUploadIntentId,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'primaryContact';
        _primaryContact?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreateCustomerRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
