// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_acceptance_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PublicAcceptanceResponse extends PublicAcceptanceResponse {
  @override
  final String? status;
  @override
  final DateTime? acceptedAtUtc;
  @override
  final String? quotationNumber;
  @override
  final AcceptanceEvidenceResponse? evidence;

  factory _$PublicAcceptanceResponse(
          [void Function(PublicAcceptanceResponseBuilder)? updates]) =>
      (PublicAcceptanceResponseBuilder()..update(updates))._build();

  _$PublicAcceptanceResponse._(
      {this.status, this.acceptedAtUtc, this.quotationNumber, this.evidence})
      : super._();
  @override
  PublicAcceptanceResponse rebuild(
          void Function(PublicAcceptanceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PublicAcceptanceResponseBuilder toBuilder() =>
      PublicAcceptanceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PublicAcceptanceResponse &&
        status == other.status &&
        acceptedAtUtc == other.acceptedAtUtc &&
        quotationNumber == other.quotationNumber &&
        evidence == other.evidence;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, acceptedAtUtc.hashCode);
    _$hash = $jc(_$hash, quotationNumber.hashCode);
    _$hash = $jc(_$hash, evidence.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PublicAcceptanceResponse')
          ..add('status', status)
          ..add('acceptedAtUtc', acceptedAtUtc)
          ..add('quotationNumber', quotationNumber)
          ..add('evidence', evidence))
        .toString();
  }
}

class PublicAcceptanceResponseBuilder
    implements
        Builder<PublicAcceptanceResponse, PublicAcceptanceResponseBuilder> {
  _$PublicAcceptanceResponse? _$v;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  DateTime? _acceptedAtUtc;
  DateTime? get acceptedAtUtc => _$this._acceptedAtUtc;
  set acceptedAtUtc(DateTime? acceptedAtUtc) =>
      _$this._acceptedAtUtc = acceptedAtUtc;

  String? _quotationNumber;
  String? get quotationNumber => _$this._quotationNumber;
  set quotationNumber(String? quotationNumber) =>
      _$this._quotationNumber = quotationNumber;

  AcceptanceEvidenceResponseBuilder? _evidence;
  AcceptanceEvidenceResponseBuilder get evidence =>
      _$this._evidence ??= AcceptanceEvidenceResponseBuilder();
  set evidence(AcceptanceEvidenceResponseBuilder? evidence) =>
      _$this._evidence = evidence;

  PublicAcceptanceResponseBuilder() {
    PublicAcceptanceResponse._defaults(this);
  }

  PublicAcceptanceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _status = $v.status;
      _acceptedAtUtc = $v.acceptedAtUtc;
      _quotationNumber = $v.quotationNumber;
      _evidence = $v.evidence?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PublicAcceptanceResponse other) {
    _$v = other as _$PublicAcceptanceResponse;
  }

  @override
  void update(void Function(PublicAcceptanceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PublicAcceptanceResponse build() => _build();

  _$PublicAcceptanceResponse _build() {
    _$PublicAcceptanceResponse _$result;
    try {
      _$result = _$v ??
          _$PublicAcceptanceResponse._(
            status: status,
            acceptedAtUtc: acceptedAtUtc,
            quotationNumber: quotationNumber,
            evidence: _evidence?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'evidence';
        _evidence?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'PublicAcceptanceResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
