// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_acceptance_view_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PublicAcceptanceViewResponse extends PublicAcceptanceViewResponse {
  @override
  final String? status;
  @override
  final DateTime? expiresAtUtc;
  @override
  final String? signerHint;
  @override
  final String? consentVersion;
  @override
  final DateTime? acceptedAtUtc;
  @override
  final QuotationDocumentResponse? document;
  @override
  final PublicQuotationPdfResponse? pdf;
  @override
  final bool? otpRequired;
  @override
  final BuiltList<String>? otpChannels;

  factory _$PublicAcceptanceViewResponse(
          [void Function(PublicAcceptanceViewResponseBuilder)? updates]) =>
      (PublicAcceptanceViewResponseBuilder()..update(updates))._build();

  _$PublicAcceptanceViewResponse._(
      {this.status,
      this.expiresAtUtc,
      this.signerHint,
      this.consentVersion,
      this.acceptedAtUtc,
      this.document,
      this.pdf,
      this.otpRequired,
      this.otpChannels})
      : super._();
  @override
  PublicAcceptanceViewResponse rebuild(
          void Function(PublicAcceptanceViewResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PublicAcceptanceViewResponseBuilder toBuilder() =>
      PublicAcceptanceViewResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PublicAcceptanceViewResponse &&
        status == other.status &&
        expiresAtUtc == other.expiresAtUtc &&
        signerHint == other.signerHint &&
        consentVersion == other.consentVersion &&
        acceptedAtUtc == other.acceptedAtUtc &&
        document == other.document &&
        pdf == other.pdf &&
        otpRequired == other.otpRequired &&
        otpChannels == other.otpChannels;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, expiresAtUtc.hashCode);
    _$hash = $jc(_$hash, signerHint.hashCode);
    _$hash = $jc(_$hash, consentVersion.hashCode);
    _$hash = $jc(_$hash, acceptedAtUtc.hashCode);
    _$hash = $jc(_$hash, document.hashCode);
    _$hash = $jc(_$hash, pdf.hashCode);
    _$hash = $jc(_$hash, otpRequired.hashCode);
    _$hash = $jc(_$hash, otpChannels.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PublicAcceptanceViewResponse')
          ..add('status', status)
          ..add('expiresAtUtc', expiresAtUtc)
          ..add('signerHint', signerHint)
          ..add('consentVersion', consentVersion)
          ..add('acceptedAtUtc', acceptedAtUtc)
          ..add('document', document)
          ..add('pdf', pdf)
          ..add('otpRequired', otpRequired)
          ..add('otpChannels', otpChannels))
        .toString();
  }
}

class PublicAcceptanceViewResponseBuilder
    implements
        Builder<PublicAcceptanceViewResponse,
            PublicAcceptanceViewResponseBuilder> {
  _$PublicAcceptanceViewResponse? _$v;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  DateTime? _expiresAtUtc;
  DateTime? get expiresAtUtc => _$this._expiresAtUtc;
  set expiresAtUtc(DateTime? expiresAtUtc) =>
      _$this._expiresAtUtc = expiresAtUtc;

  String? _signerHint;
  String? get signerHint => _$this._signerHint;
  set signerHint(String? signerHint) => _$this._signerHint = signerHint;

  String? _consentVersion;
  String? get consentVersion => _$this._consentVersion;
  set consentVersion(String? consentVersion) =>
      _$this._consentVersion = consentVersion;

  DateTime? _acceptedAtUtc;
  DateTime? get acceptedAtUtc => _$this._acceptedAtUtc;
  set acceptedAtUtc(DateTime? acceptedAtUtc) =>
      _$this._acceptedAtUtc = acceptedAtUtc;

  QuotationDocumentResponseBuilder? _document;
  QuotationDocumentResponseBuilder get document =>
      _$this._document ??= QuotationDocumentResponseBuilder();
  set document(QuotationDocumentResponseBuilder? document) =>
      _$this._document = document;

  PublicQuotationPdfResponseBuilder? _pdf;
  PublicQuotationPdfResponseBuilder get pdf =>
      _$this._pdf ??= PublicQuotationPdfResponseBuilder();
  set pdf(PublicQuotationPdfResponseBuilder? pdf) => _$this._pdf = pdf;

  bool? _otpRequired;
  bool? get otpRequired => _$this._otpRequired;
  set otpRequired(bool? otpRequired) => _$this._otpRequired = otpRequired;

  ListBuilder<String>? _otpChannels;
  ListBuilder<String> get otpChannels =>
      _$this._otpChannels ??= ListBuilder<String>();
  set otpChannels(ListBuilder<String>? otpChannels) =>
      _$this._otpChannels = otpChannels;

  PublicAcceptanceViewResponseBuilder() {
    PublicAcceptanceViewResponse._defaults(this);
  }

  PublicAcceptanceViewResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _status = $v.status;
      _expiresAtUtc = $v.expiresAtUtc;
      _signerHint = $v.signerHint;
      _consentVersion = $v.consentVersion;
      _acceptedAtUtc = $v.acceptedAtUtc;
      _document = $v.document?.toBuilder();
      _pdf = $v.pdf?.toBuilder();
      _otpRequired = $v.otpRequired;
      _otpChannels = $v.otpChannels?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PublicAcceptanceViewResponse other) {
    _$v = other as _$PublicAcceptanceViewResponse;
  }

  @override
  void update(void Function(PublicAcceptanceViewResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PublicAcceptanceViewResponse build() => _build();

  _$PublicAcceptanceViewResponse _build() {
    _$PublicAcceptanceViewResponse _$result;
    try {
      _$result = _$v ??
          _$PublicAcceptanceViewResponse._(
            status: status,
            expiresAtUtc: expiresAtUtc,
            signerHint: signerHint,
            consentVersion: consentVersion,
            acceptedAtUtc: acceptedAtUtc,
            document: _document?.build(),
            pdf: _pdf?.build(),
            otpRequired: otpRequired,
            otpChannels: _otpChannels?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'document';
        _document?.build();
        _$failedField = 'pdf';
        _pdf?.build();

        _$failedField = 'otpChannels';
        _otpChannels?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PublicAcceptanceViewResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
