// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'capture_signature_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CaptureSignatureRequest extends CaptureSignatureRequest {
  @override
  final String? purpose;
  @override
  final String? signerName;
  @override
  final String? signerRole;
  @override
  final String? imageFileId;
  @override
  final bool? consentAccepted;
  @override
  final String? consentTextVersion;

  factory _$CaptureSignatureRequest(
          [void Function(CaptureSignatureRequestBuilder)? updates]) =>
      (CaptureSignatureRequestBuilder()..update(updates))._build();

  _$CaptureSignatureRequest._(
      {this.purpose,
      this.signerName,
      this.signerRole,
      this.imageFileId,
      this.consentAccepted,
      this.consentTextVersion})
      : super._();
  @override
  CaptureSignatureRequest rebuild(
          void Function(CaptureSignatureRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CaptureSignatureRequestBuilder toBuilder() =>
      CaptureSignatureRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CaptureSignatureRequest &&
        purpose == other.purpose &&
        signerName == other.signerName &&
        signerRole == other.signerRole &&
        imageFileId == other.imageFileId &&
        consentAccepted == other.consentAccepted &&
        consentTextVersion == other.consentTextVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, purpose.hashCode);
    _$hash = $jc(_$hash, signerName.hashCode);
    _$hash = $jc(_$hash, signerRole.hashCode);
    _$hash = $jc(_$hash, imageFileId.hashCode);
    _$hash = $jc(_$hash, consentAccepted.hashCode);
    _$hash = $jc(_$hash, consentTextVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CaptureSignatureRequest')
          ..add('purpose', purpose)
          ..add('signerName', signerName)
          ..add('signerRole', signerRole)
          ..add('imageFileId', imageFileId)
          ..add('consentAccepted', consentAccepted)
          ..add('consentTextVersion', consentTextVersion))
        .toString();
  }
}

class CaptureSignatureRequestBuilder
    implements
        Builder<CaptureSignatureRequest, CaptureSignatureRequestBuilder> {
  _$CaptureSignatureRequest? _$v;

  String? _purpose;
  String? get purpose => _$this._purpose;
  set purpose(String? purpose) => _$this._purpose = purpose;

  String? _signerName;
  String? get signerName => _$this._signerName;
  set signerName(String? signerName) => _$this._signerName = signerName;

  String? _signerRole;
  String? get signerRole => _$this._signerRole;
  set signerRole(String? signerRole) => _$this._signerRole = signerRole;

  String? _imageFileId;
  String? get imageFileId => _$this._imageFileId;
  set imageFileId(String? imageFileId) => _$this._imageFileId = imageFileId;

  bool? _consentAccepted;
  bool? get consentAccepted => _$this._consentAccepted;
  set consentAccepted(bool? consentAccepted) =>
      _$this._consentAccepted = consentAccepted;

  String? _consentTextVersion;
  String? get consentTextVersion => _$this._consentTextVersion;
  set consentTextVersion(String? consentTextVersion) =>
      _$this._consentTextVersion = consentTextVersion;

  CaptureSignatureRequestBuilder() {
    CaptureSignatureRequest._defaults(this);
  }

  CaptureSignatureRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _purpose = $v.purpose;
      _signerName = $v.signerName;
      _signerRole = $v.signerRole;
      _imageFileId = $v.imageFileId;
      _consentAccepted = $v.consentAccepted;
      _consentTextVersion = $v.consentTextVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CaptureSignatureRequest other) {
    _$v = other as _$CaptureSignatureRequest;
  }

  @override
  void update(void Function(CaptureSignatureRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CaptureSignatureRequest build() => _build();

  _$CaptureSignatureRequest _build() {
    final _$result = _$v ??
        _$CaptureSignatureRequest._(
          purpose: purpose,
          signerName: signerName,
          signerRole: signerRole,
          imageFileId: imageFileId,
          consentAccepted: consentAccepted,
          consentTextVersion: consentTextVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
