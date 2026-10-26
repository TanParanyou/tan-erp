// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'acceptance_evidence_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AcceptanceEvidenceResponse extends AcceptanceEvidenceResponse {
  @override
  final String? signerName;
  @override
  final String? signerRole;
  @override
  final String? consentVersion;
  @override
  final bool? hasSignatureImage;
  @override
  final String? signatureHash;
  @override
  final DateTime? acceptedAtUtc;

  factory _$AcceptanceEvidenceResponse(
          [void Function(AcceptanceEvidenceResponseBuilder)? updates]) =>
      (AcceptanceEvidenceResponseBuilder()..update(updates))._build();

  _$AcceptanceEvidenceResponse._(
      {this.signerName,
      this.signerRole,
      this.consentVersion,
      this.hasSignatureImage,
      this.signatureHash,
      this.acceptedAtUtc})
      : super._();
  @override
  AcceptanceEvidenceResponse rebuild(
          void Function(AcceptanceEvidenceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AcceptanceEvidenceResponseBuilder toBuilder() =>
      AcceptanceEvidenceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AcceptanceEvidenceResponse &&
        signerName == other.signerName &&
        signerRole == other.signerRole &&
        consentVersion == other.consentVersion &&
        hasSignatureImage == other.hasSignatureImage &&
        signatureHash == other.signatureHash &&
        acceptedAtUtc == other.acceptedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, signerName.hashCode);
    _$hash = $jc(_$hash, signerRole.hashCode);
    _$hash = $jc(_$hash, consentVersion.hashCode);
    _$hash = $jc(_$hash, hasSignatureImage.hashCode);
    _$hash = $jc(_$hash, signatureHash.hashCode);
    _$hash = $jc(_$hash, acceptedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AcceptanceEvidenceResponse')
          ..add('signerName', signerName)
          ..add('signerRole', signerRole)
          ..add('consentVersion', consentVersion)
          ..add('hasSignatureImage', hasSignatureImage)
          ..add('signatureHash', signatureHash)
          ..add('acceptedAtUtc', acceptedAtUtc))
        .toString();
  }
}

class AcceptanceEvidenceResponseBuilder
    implements
        Builder<AcceptanceEvidenceResponse, AcceptanceEvidenceResponseBuilder> {
  _$AcceptanceEvidenceResponse? _$v;

  String? _signerName;
  String? get signerName => _$this._signerName;
  set signerName(String? signerName) => _$this._signerName = signerName;

  String? _signerRole;
  String? get signerRole => _$this._signerRole;
  set signerRole(String? signerRole) => _$this._signerRole = signerRole;

  String? _consentVersion;
  String? get consentVersion => _$this._consentVersion;
  set consentVersion(String? consentVersion) =>
      _$this._consentVersion = consentVersion;

  bool? _hasSignatureImage;
  bool? get hasSignatureImage => _$this._hasSignatureImage;
  set hasSignatureImage(bool? hasSignatureImage) =>
      _$this._hasSignatureImage = hasSignatureImage;

  String? _signatureHash;
  String? get signatureHash => _$this._signatureHash;
  set signatureHash(String? signatureHash) =>
      _$this._signatureHash = signatureHash;

  DateTime? _acceptedAtUtc;
  DateTime? get acceptedAtUtc => _$this._acceptedAtUtc;
  set acceptedAtUtc(DateTime? acceptedAtUtc) =>
      _$this._acceptedAtUtc = acceptedAtUtc;

  AcceptanceEvidenceResponseBuilder() {
    AcceptanceEvidenceResponse._defaults(this);
  }

  AcceptanceEvidenceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _signerName = $v.signerName;
      _signerRole = $v.signerRole;
      _consentVersion = $v.consentVersion;
      _hasSignatureImage = $v.hasSignatureImage;
      _signatureHash = $v.signatureHash;
      _acceptedAtUtc = $v.acceptedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AcceptanceEvidenceResponse other) {
    _$v = other as _$AcceptanceEvidenceResponse;
  }

  @override
  void update(void Function(AcceptanceEvidenceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AcceptanceEvidenceResponse build() => _build();

  _$AcceptanceEvidenceResponse _build() {
    final _$result = _$v ??
        _$AcceptanceEvidenceResponse._(
          signerName: signerName,
          signerRole: signerRole,
          consentVersion: consentVersion,
          hasSignatureImage: hasSignatureImage,
          signatureHash: signatureHash,
          acceptedAtUtc: acceptedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
