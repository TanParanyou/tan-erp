// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'signature_capture_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SignatureCaptureResponse extends SignatureCaptureResponse {
  @override
  final String? id;
  @override
  final String? ownerType;
  @override
  final String? ownerId;
  @override
  final String? purpose;
  @override
  final String? signerName;
  @override
  final String? signerRole;
  @override
  final DateTime? signedAtUtc;
  @override
  final String? imageFileId;
  @override
  final String? servingUrl;
  @override
  final String? consentTextVersion;
  @override
  final String? contentHash;
  @override
  final AttachmentPersonResponse? capturedBy;

  factory _$SignatureCaptureResponse(
          [void Function(SignatureCaptureResponseBuilder)? updates]) =>
      (SignatureCaptureResponseBuilder()..update(updates))._build();

  _$SignatureCaptureResponse._(
      {this.id,
      this.ownerType,
      this.ownerId,
      this.purpose,
      this.signerName,
      this.signerRole,
      this.signedAtUtc,
      this.imageFileId,
      this.servingUrl,
      this.consentTextVersion,
      this.contentHash,
      this.capturedBy})
      : super._();
  @override
  SignatureCaptureResponse rebuild(
          void Function(SignatureCaptureResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SignatureCaptureResponseBuilder toBuilder() =>
      SignatureCaptureResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SignatureCaptureResponse &&
        id == other.id &&
        ownerType == other.ownerType &&
        ownerId == other.ownerId &&
        purpose == other.purpose &&
        signerName == other.signerName &&
        signerRole == other.signerRole &&
        signedAtUtc == other.signedAtUtc &&
        imageFileId == other.imageFileId &&
        servingUrl == other.servingUrl &&
        consentTextVersion == other.consentTextVersion &&
        contentHash == other.contentHash &&
        capturedBy == other.capturedBy;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, ownerType.hashCode);
    _$hash = $jc(_$hash, ownerId.hashCode);
    _$hash = $jc(_$hash, purpose.hashCode);
    _$hash = $jc(_$hash, signerName.hashCode);
    _$hash = $jc(_$hash, signerRole.hashCode);
    _$hash = $jc(_$hash, signedAtUtc.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jc(_$hash, servingUrl.hashCode);
    _$hash = $jc(_$hash, consentTextVersion.hashCode);
    _$hash = $jc(_$hash, contentHash.hashCode);
    _$hash = $jc(_$hash, capturedBy.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SignatureCaptureResponse')
          ..add('id', id)
          ..add('ownerType', ownerType)
          ..add('ownerId', ownerId)
          ..add('purpose', purpose)
          ..add('signerName', signerName)
          ..add('signerRole', signerRole)
          ..add('signedAtUtc', signedAtUtc)
          ..add('imageFileId', imageFileId)
          ..add('servingUrl', servingUrl)
          ..add('consentTextVersion', consentTextVersion)
          ..add('contentHash', contentHash)
          ..add('capturedBy', capturedBy))
        .toString();
  }
}

class SignatureCaptureResponseBuilder
    implements
        Builder<SignatureCaptureResponse, SignatureCaptureResponseBuilder> {
  _$SignatureCaptureResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _ownerType;
  String? get ownerType => _$this._ownerType;
  set ownerType(String? ownerType) => _$this._ownerType = ownerType;

  String? _ownerId;
  String? get ownerId => _$this._ownerId;
  set ownerId(String? ownerId) => _$this._ownerId = ownerId;

  String? _purpose;
  String? get purpose => _$this._purpose;
  set purpose(String? purpose) => _$this._purpose = purpose;

  String? _signerName;
  String? get signerName => _$this._signerName;
  set signerName(String? signerName) => _$this._signerName = signerName;

  String? _signerRole;
  String? get signerRole => _$this._signerRole;
  set signerRole(String? signerRole) => _$this._signerRole = signerRole;

  DateTime? _signedAtUtc;
  DateTime? get signedAtUtc => _$this._signedAtUtc;
  set signedAtUtc(DateTime? signedAtUtc) => _$this._signedAtUtc = signedAtUtc;

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  String? _servingUrl;
  String? get servingUrl => _$this._servingUrl;
  set servingUrl(String? servingUrl) => _$this._servingUrl = servingUrl;

  String? _consentTextVersion;
  String? get consentTextVersion => _$this._consentTextVersion;
  set consentTextVersion(String? consentTextVersion) =>
      _$this._consentTextVersion = consentTextVersion;

  String? _contentHash;
  String? get contentHash => _$this._contentHash;
  set contentHash(String? contentHash) => _$this._contentHash = contentHash;

  AttachmentPersonResponseBuilder? _capturedBy;
  AttachmentPersonResponseBuilder get capturedBy =>
      _$this._capturedBy ??= AttachmentPersonResponseBuilder();
  set capturedBy(AttachmentPersonResponseBuilder? capturedBy) =>
      _$this._capturedBy = capturedBy;

  SignatureCaptureResponseBuilder() {
    SignatureCaptureResponse._defaults(this);
  }

  SignatureCaptureResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _ownerType = $v.ownerType;
      _ownerId = $v.ownerId;
      _purpose = $v.purpose;
      _signerName = $v.signerName;
      _signerRole = $v.signerRole;
      _signedAtUtc = $v.signedAtUtc;
      _imageFileId = $v.imageFileId;
      _servingUrl = $v.servingUrl;
      _consentTextVersion = $v.consentTextVersion;
      _contentHash = $v.contentHash;
      _capturedBy = $v.capturedBy?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SignatureCaptureResponse other) {
    _$v = other as _$SignatureCaptureResponse;
  }

  @override
  void update(void Function(SignatureCaptureResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SignatureCaptureResponse build() => _build();

  _$SignatureCaptureResponse _build() {
    _$SignatureCaptureResponse _$result;
    try {
      _$result = _$v ??
          _$SignatureCaptureResponse._(
            id: id,
            ownerType: ownerType,
            ownerId: ownerId,
            purpose: purpose,
            signerName: signerName,
            signerRole: signerRole,
            signedAtUtc: signedAtUtc,
            imageFileId: imageFileId,
            servingUrl: servingUrl,
            consentTextVersion: consentTextVersion,
            contentHash: contentHash,
            capturedBy: _capturedBy?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'capturedBy';
        _capturedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SignatureCaptureResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
