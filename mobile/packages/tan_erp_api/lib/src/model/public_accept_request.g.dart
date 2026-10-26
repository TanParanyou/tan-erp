// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_accept_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PublicAcceptRequest extends PublicAcceptRequest {
  @override
  final String? signerName;
  @override
  final String? signerRole;
  @override
  final bool? consentAccepted;
  @override
  final String? consentVersion;
  @override
  final String? signatureImage;
  @override
  final String? documentSha256;
  @override
  final String? otpVerificationToken;

  factory _$PublicAcceptRequest(
          [void Function(PublicAcceptRequestBuilder)? updates]) =>
      (PublicAcceptRequestBuilder()..update(updates))._build();

  _$PublicAcceptRequest._(
      {this.signerName,
      this.signerRole,
      this.consentAccepted,
      this.consentVersion,
      this.signatureImage,
      this.documentSha256,
      this.otpVerificationToken})
      : super._();
  @override
  PublicAcceptRequest rebuild(
          void Function(PublicAcceptRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PublicAcceptRequestBuilder toBuilder() =>
      PublicAcceptRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PublicAcceptRequest &&
        signerName == other.signerName &&
        signerRole == other.signerRole &&
        consentAccepted == other.consentAccepted &&
        consentVersion == other.consentVersion &&
        signatureImage == other.signatureImage &&
        documentSha256 == other.documentSha256 &&
        otpVerificationToken == other.otpVerificationToken;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, signerName.hashCode);
    _$hash = $jc(_$hash, signerRole.hashCode);
    _$hash = $jc(_$hash, consentAccepted.hashCode);
    _$hash = $jc(_$hash, consentVersion.hashCode);
    _$hash = $jc(_$hash, signatureImage.hashCode);
    _$hash = $jc(_$hash, documentSha256.hashCode);
    _$hash = $jc(_$hash, otpVerificationToken.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PublicAcceptRequest')
          ..add('signerName', signerName)
          ..add('signerRole', signerRole)
          ..add('consentAccepted', consentAccepted)
          ..add('consentVersion', consentVersion)
          ..add('signatureImage', signatureImage)
          ..add('documentSha256', documentSha256)
          ..add('otpVerificationToken', otpVerificationToken))
        .toString();
  }
}

class PublicAcceptRequestBuilder
    implements Builder<PublicAcceptRequest, PublicAcceptRequestBuilder> {
  _$PublicAcceptRequest? _$v;

  String? _signerName;
  String? get signerName => _$this._signerName;
  set signerName(String? signerName) => _$this._signerName = signerName;

  String? _signerRole;
  String? get signerRole => _$this._signerRole;
  set signerRole(String? signerRole) => _$this._signerRole = signerRole;

  bool? _consentAccepted;
  bool? get consentAccepted => _$this._consentAccepted;
  set consentAccepted(bool? consentAccepted) =>
      _$this._consentAccepted = consentAccepted;

  String? _consentVersion;
  String? get consentVersion => _$this._consentVersion;
  set consentVersion(String? consentVersion) =>
      _$this._consentVersion = consentVersion;

  String? _signatureImage;
  String? get signatureImage => _$this._signatureImage;
  set signatureImage(String? signatureImage) =>
      _$this._signatureImage = signatureImage;

  String? _documentSha256;
  String? get documentSha256 => _$this._documentSha256;
  set documentSha256(String? documentSha256) =>
      _$this._documentSha256 = documentSha256;

  String? _otpVerificationToken;
  String? get otpVerificationToken => _$this._otpVerificationToken;
  set otpVerificationToken(String? otpVerificationToken) =>
      _$this._otpVerificationToken = otpVerificationToken;

  PublicAcceptRequestBuilder() {
    PublicAcceptRequest._defaults(this);
  }

  PublicAcceptRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _signerName = $v.signerName;
      _signerRole = $v.signerRole;
      _consentAccepted = $v.consentAccepted;
      _consentVersion = $v.consentVersion;
      _signatureImage = $v.signatureImage;
      _documentSha256 = $v.documentSha256;
      _otpVerificationToken = $v.otpVerificationToken;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PublicAcceptRequest other) {
    _$v = other as _$PublicAcceptRequest;
  }

  @override
  void update(void Function(PublicAcceptRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PublicAcceptRequest build() => _build();

  _$PublicAcceptRequest _build() {
    final _$result = _$v ??
        _$PublicAcceptRequest._(
          signerName: signerName,
          signerRole: signerRole,
          consentAccepted: consentAccepted,
          consentVersion: consentVersion,
          signatureImage: signatureImage,
          documentSha256: documentSha256,
          otpVerificationToken: otpVerificationToken,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
