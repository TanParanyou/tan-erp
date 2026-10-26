// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'acceptance_link_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AcceptanceLinkResponse extends AcceptanceLinkResponse {
  @override
  final String? id;
  @override
  final String? quotationId;
  @override
  final String? quotationNumber;
  @override
  final String? status;
  @override
  final String? quotationStatus;
  @override
  final bool? isUsable;
  @override
  final String? signerHint;
  @override
  final DateTime? expiresAtUtc;
  @override
  final DateTime? createdAtUtc;
  @override
  final QuotationPersonResponse? createdBy;
  @override
  final DateTime? revokedAtUtc;
  @override
  final DateTime? acceptedAtUtc;
  @override
  final AcceptanceEvidenceResponse? evidence;

  factory _$AcceptanceLinkResponse(
          [void Function(AcceptanceLinkResponseBuilder)? updates]) =>
      (AcceptanceLinkResponseBuilder()..update(updates))._build();

  _$AcceptanceLinkResponse._(
      {this.id,
      this.quotationId,
      this.quotationNumber,
      this.status,
      this.quotationStatus,
      this.isUsable,
      this.signerHint,
      this.expiresAtUtc,
      this.createdAtUtc,
      this.createdBy,
      this.revokedAtUtc,
      this.acceptedAtUtc,
      this.evidence})
      : super._();
  @override
  AcceptanceLinkResponse rebuild(
          void Function(AcceptanceLinkResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AcceptanceLinkResponseBuilder toBuilder() =>
      AcceptanceLinkResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AcceptanceLinkResponse &&
        id == other.id &&
        quotationId == other.quotationId &&
        quotationNumber == other.quotationNumber &&
        status == other.status &&
        quotationStatus == other.quotationStatus &&
        isUsable == other.isUsable &&
        signerHint == other.signerHint &&
        expiresAtUtc == other.expiresAtUtc &&
        createdAtUtc == other.createdAtUtc &&
        createdBy == other.createdBy &&
        revokedAtUtc == other.revokedAtUtc &&
        acceptedAtUtc == other.acceptedAtUtc &&
        evidence == other.evidence;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, quotationId.hashCode);
    _$hash = $jc(_$hash, quotationNumber.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, quotationStatus.hashCode);
    _$hash = $jc(_$hash, isUsable.hashCode);
    _$hash = $jc(_$hash, signerHint.hashCode);
    _$hash = $jc(_$hash, expiresAtUtc.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, revokedAtUtc.hashCode);
    _$hash = $jc(_$hash, acceptedAtUtc.hashCode);
    _$hash = $jc(_$hash, evidence.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AcceptanceLinkResponse')
          ..add('id', id)
          ..add('quotationId', quotationId)
          ..add('quotationNumber', quotationNumber)
          ..add('status', status)
          ..add('quotationStatus', quotationStatus)
          ..add('isUsable', isUsable)
          ..add('signerHint', signerHint)
          ..add('expiresAtUtc', expiresAtUtc)
          ..add('createdAtUtc', createdAtUtc)
          ..add('createdBy', createdBy)
          ..add('revokedAtUtc', revokedAtUtc)
          ..add('acceptedAtUtc', acceptedAtUtc)
          ..add('evidence', evidence))
        .toString();
  }
}

class AcceptanceLinkResponseBuilder
    implements Builder<AcceptanceLinkResponse, AcceptanceLinkResponseBuilder> {
  _$AcceptanceLinkResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _quotationId;
  String? get quotationId => _$this._quotationId;
  set quotationId(String? quotationId) => _$this._quotationId = quotationId;

  String? _quotationNumber;
  String? get quotationNumber => _$this._quotationNumber;
  set quotationNumber(String? quotationNumber) =>
      _$this._quotationNumber = quotationNumber;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _quotationStatus;
  String? get quotationStatus => _$this._quotationStatus;
  set quotationStatus(String? quotationStatus) =>
      _$this._quotationStatus = quotationStatus;

  bool? _isUsable;
  bool? get isUsable => _$this._isUsable;
  set isUsable(bool? isUsable) => _$this._isUsable = isUsable;

  String? _signerHint;
  String? get signerHint => _$this._signerHint;
  set signerHint(String? signerHint) => _$this._signerHint = signerHint;

  DateTime? _expiresAtUtc;
  DateTime? get expiresAtUtc => _$this._expiresAtUtc;
  set expiresAtUtc(DateTime? expiresAtUtc) =>
      _$this._expiresAtUtc = expiresAtUtc;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  QuotationPersonResponseBuilder? _createdBy;
  QuotationPersonResponseBuilder get createdBy =>
      _$this._createdBy ??= QuotationPersonResponseBuilder();
  set createdBy(QuotationPersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _revokedAtUtc;
  DateTime? get revokedAtUtc => _$this._revokedAtUtc;
  set revokedAtUtc(DateTime? revokedAtUtc) =>
      _$this._revokedAtUtc = revokedAtUtc;

  DateTime? _acceptedAtUtc;
  DateTime? get acceptedAtUtc => _$this._acceptedAtUtc;
  set acceptedAtUtc(DateTime? acceptedAtUtc) =>
      _$this._acceptedAtUtc = acceptedAtUtc;

  AcceptanceEvidenceResponseBuilder? _evidence;
  AcceptanceEvidenceResponseBuilder get evidence =>
      _$this._evidence ??= AcceptanceEvidenceResponseBuilder();
  set evidence(AcceptanceEvidenceResponseBuilder? evidence) =>
      _$this._evidence = evidence;

  AcceptanceLinkResponseBuilder() {
    AcceptanceLinkResponse._defaults(this);
  }

  AcceptanceLinkResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _quotationId = $v.quotationId;
      _quotationNumber = $v.quotationNumber;
      _status = $v.status;
      _quotationStatus = $v.quotationStatus;
      _isUsable = $v.isUsable;
      _signerHint = $v.signerHint;
      _expiresAtUtc = $v.expiresAtUtc;
      _createdAtUtc = $v.createdAtUtc;
      _createdBy = $v.createdBy?.toBuilder();
      _revokedAtUtc = $v.revokedAtUtc;
      _acceptedAtUtc = $v.acceptedAtUtc;
      _evidence = $v.evidence?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AcceptanceLinkResponse other) {
    _$v = other as _$AcceptanceLinkResponse;
  }

  @override
  void update(void Function(AcceptanceLinkResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AcceptanceLinkResponse build() => _build();

  _$AcceptanceLinkResponse _build() {
    _$AcceptanceLinkResponse _$result;
    try {
      _$result = _$v ??
          _$AcceptanceLinkResponse._(
            id: id,
            quotationId: quotationId,
            quotationNumber: quotationNumber,
            status: status,
            quotationStatus: quotationStatus,
            isUsable: isUsable,
            signerHint: signerHint,
            expiresAtUtc: expiresAtUtc,
            createdAtUtc: createdAtUtc,
            createdBy: _createdBy?.build(),
            revokedAtUtc: revokedAtUtc,
            acceptedAtUtc: acceptedAtUtc,
            evidence: _evidence?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'evidence';
        _evidence?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AcceptanceLinkResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
