// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationResponse extends QuotationResponse {
  @override
  final String? quotationId;
  @override
  final String? estimateId;
  @override
  final String? opportunityId;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final double? grandTotal;
  @override
  final DateTime? issuedAtUtc;
  @override
  final String? estimateRevisionId;
  @override
  final int? revisionNo;
  @override
  final String? opportunityStage;
  @override
  final String? opportunityRowVersion;
  @override
  final String? estimateRowVersion;

  factory _$QuotationResponse(
          [void Function(QuotationResponseBuilder)? updates]) =>
      (QuotationResponseBuilder()..update(updates))._build();

  _$QuotationResponse._(
      {this.quotationId,
      this.estimateId,
      this.opportunityId,
      this.number,
      this.status,
      this.grandTotal,
      this.issuedAtUtc,
      this.estimateRevisionId,
      this.revisionNo,
      this.opportunityStage,
      this.opportunityRowVersion,
      this.estimateRowVersion})
      : super._();
  @override
  QuotationResponse rebuild(void Function(QuotationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationResponseBuilder toBuilder() =>
      QuotationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationResponse &&
        quotationId == other.quotationId &&
        estimateId == other.estimateId &&
        opportunityId == other.opportunityId &&
        number == other.number &&
        status == other.status &&
        grandTotal == other.grandTotal &&
        issuedAtUtc == other.issuedAtUtc &&
        estimateRevisionId == other.estimateRevisionId &&
        revisionNo == other.revisionNo &&
        opportunityStage == other.opportunityStage &&
        opportunityRowVersion == other.opportunityRowVersion &&
        estimateRowVersion == other.estimateRowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, quotationId.hashCode);
    _$hash = $jc(_$hash, estimateId.hashCode);
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, grandTotal.hashCode);
    _$hash = $jc(_$hash, issuedAtUtc.hashCode);
    _$hash = $jc(_$hash, estimateRevisionId.hashCode);
    _$hash = $jc(_$hash, revisionNo.hashCode);
    _$hash = $jc(_$hash, opportunityStage.hashCode);
    _$hash = $jc(_$hash, opportunityRowVersion.hashCode);
    _$hash = $jc(_$hash, estimateRowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationResponse')
          ..add('quotationId', quotationId)
          ..add('estimateId', estimateId)
          ..add('opportunityId', opportunityId)
          ..add('number', number)
          ..add('status', status)
          ..add('grandTotal', grandTotal)
          ..add('issuedAtUtc', issuedAtUtc)
          ..add('estimateRevisionId', estimateRevisionId)
          ..add('revisionNo', revisionNo)
          ..add('opportunityStage', opportunityStage)
          ..add('opportunityRowVersion', opportunityRowVersion)
          ..add('estimateRowVersion', estimateRowVersion))
        .toString();
  }
}

class QuotationResponseBuilder
    implements Builder<QuotationResponse, QuotationResponseBuilder> {
  _$QuotationResponse? _$v;

  String? _quotationId;
  String? get quotationId => _$this._quotationId;
  set quotationId(String? quotationId) => _$this._quotationId = quotationId;

  String? _estimateId;
  String? get estimateId => _$this._estimateId;
  set estimateId(String? estimateId) => _$this._estimateId = estimateId;

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  double? _grandTotal;
  double? get grandTotal => _$this._grandTotal;
  set grandTotal(double? grandTotal) => _$this._grandTotal = grandTotal;

  DateTime? _issuedAtUtc;
  DateTime? get issuedAtUtc => _$this._issuedAtUtc;
  set issuedAtUtc(DateTime? issuedAtUtc) => _$this._issuedAtUtc = issuedAtUtc;

  String? _estimateRevisionId;
  String? get estimateRevisionId => _$this._estimateRevisionId;
  set estimateRevisionId(String? estimateRevisionId) =>
      _$this._estimateRevisionId = estimateRevisionId;

  int? _revisionNo;
  int? get revisionNo => _$this._revisionNo;
  set revisionNo(int? revisionNo) => _$this._revisionNo = revisionNo;

  String? _opportunityStage;
  String? get opportunityStage => _$this._opportunityStage;
  set opportunityStage(String? opportunityStage) =>
      _$this._opportunityStage = opportunityStage;

  String? _opportunityRowVersion;
  String? get opportunityRowVersion => _$this._opportunityRowVersion;
  set opportunityRowVersion(String? opportunityRowVersion) =>
      _$this._opportunityRowVersion = opportunityRowVersion;

  String? _estimateRowVersion;
  String? get estimateRowVersion => _$this._estimateRowVersion;
  set estimateRowVersion(String? estimateRowVersion) =>
      _$this._estimateRowVersion = estimateRowVersion;

  QuotationResponseBuilder() {
    QuotationResponse._defaults(this);
  }

  QuotationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _quotationId = $v.quotationId;
      _estimateId = $v.estimateId;
      _opportunityId = $v.opportunityId;
      _number = $v.number;
      _status = $v.status;
      _grandTotal = $v.grandTotal;
      _issuedAtUtc = $v.issuedAtUtc;
      _estimateRevisionId = $v.estimateRevisionId;
      _revisionNo = $v.revisionNo;
      _opportunityStage = $v.opportunityStage;
      _opportunityRowVersion = $v.opportunityRowVersion;
      _estimateRowVersion = $v.estimateRowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationResponse other) {
    _$v = other as _$QuotationResponse;
  }

  @override
  void update(void Function(QuotationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationResponse build() => _build();

  _$QuotationResponse _build() {
    final _$result = _$v ??
        _$QuotationResponse._(
          quotationId: quotationId,
          estimateId: estimateId,
          opportunityId: opportunityId,
          number: number,
          status: status,
          grandTotal: grandTotal,
          issuedAtUtc: issuedAtUtc,
          estimateRevisionId: estimateRevisionId,
          revisionNo: revisionNo,
          opportunityStage: opportunityStage,
          opportunityRowVersion: opportunityRowVersion,
          estimateRowVersion: estimateRowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
