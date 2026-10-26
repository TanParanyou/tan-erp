// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_history_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationHistoryItemResponse extends QuotationHistoryItemResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final double? totalAmount;
  @override
  final String? estimateRevisionId;
  @override
  final int? estimateRevisionNo;
  @override
  final DateTime? issuedAtUtc;
  @override
  final DateTime? acceptedAtUtc;
  @override
  final String? supersedesQuotationId;
  @override
  final String? supersedesNumber;
  @override
  final String? supersededByQuotationId;
  @override
  final String? supersededByNumber;
  @override
  final String? amendmentReason;
  @override
  final DateTime? voidedAtUtc;
  @override
  final QuotationPersonResponse? voidedBy;
  @override
  final String? voidReason;
  @override
  final String? rowVersion;
  @override
  final QuotationDocumentSummaryResponse? document;

  factory _$QuotationHistoryItemResponse(
          [void Function(QuotationHistoryItemResponseBuilder)? updates]) =>
      (QuotationHistoryItemResponseBuilder()..update(updates))._build();

  _$QuotationHistoryItemResponse._(
      {this.id,
      this.number,
      this.status,
      this.totalAmount,
      this.estimateRevisionId,
      this.estimateRevisionNo,
      this.issuedAtUtc,
      this.acceptedAtUtc,
      this.supersedesQuotationId,
      this.supersedesNumber,
      this.supersededByQuotationId,
      this.supersededByNumber,
      this.amendmentReason,
      this.voidedAtUtc,
      this.voidedBy,
      this.voidReason,
      this.rowVersion,
      this.document})
      : super._();
  @override
  QuotationHistoryItemResponse rebuild(
          void Function(QuotationHistoryItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationHistoryItemResponseBuilder toBuilder() =>
      QuotationHistoryItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationHistoryItemResponse &&
        id == other.id &&
        number == other.number &&
        status == other.status &&
        totalAmount == other.totalAmount &&
        estimateRevisionId == other.estimateRevisionId &&
        estimateRevisionNo == other.estimateRevisionNo &&
        issuedAtUtc == other.issuedAtUtc &&
        acceptedAtUtc == other.acceptedAtUtc &&
        supersedesQuotationId == other.supersedesQuotationId &&
        supersedesNumber == other.supersedesNumber &&
        supersededByQuotationId == other.supersededByQuotationId &&
        supersededByNumber == other.supersededByNumber &&
        amendmentReason == other.amendmentReason &&
        voidedAtUtc == other.voidedAtUtc &&
        voidedBy == other.voidedBy &&
        voidReason == other.voidReason &&
        rowVersion == other.rowVersion &&
        document == other.document;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, totalAmount.hashCode);
    _$hash = $jc(_$hash, estimateRevisionId.hashCode);
    _$hash = $jc(_$hash, estimateRevisionNo.hashCode);
    _$hash = $jc(_$hash, issuedAtUtc.hashCode);
    _$hash = $jc(_$hash, acceptedAtUtc.hashCode);
    _$hash = $jc(_$hash, supersedesQuotationId.hashCode);
    _$hash = $jc(_$hash, supersedesNumber.hashCode);
    _$hash = $jc(_$hash, supersededByQuotationId.hashCode);
    _$hash = $jc(_$hash, supersededByNumber.hashCode);
    _$hash = $jc(_$hash, amendmentReason.hashCode);
    _$hash = $jc(_$hash, voidedAtUtc.hashCode);
    _$hash = $jc(_$hash, voidedBy.hashCode);
    _$hash = $jc(_$hash, voidReason.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, document.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationHistoryItemResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('status', status)
          ..add('totalAmount', totalAmount)
          ..add('estimateRevisionId', estimateRevisionId)
          ..add('estimateRevisionNo', estimateRevisionNo)
          ..add('issuedAtUtc', issuedAtUtc)
          ..add('acceptedAtUtc', acceptedAtUtc)
          ..add('supersedesQuotationId', supersedesQuotationId)
          ..add('supersedesNumber', supersedesNumber)
          ..add('supersededByQuotationId', supersededByQuotationId)
          ..add('supersededByNumber', supersededByNumber)
          ..add('amendmentReason', amendmentReason)
          ..add('voidedAtUtc', voidedAtUtc)
          ..add('voidedBy', voidedBy)
          ..add('voidReason', voidReason)
          ..add('rowVersion', rowVersion)
          ..add('document', document))
        .toString();
  }
}

class QuotationHistoryItemResponseBuilder
    implements
        Builder<QuotationHistoryItemResponse,
            QuotationHistoryItemResponseBuilder> {
  _$QuotationHistoryItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  double? _totalAmount;
  double? get totalAmount => _$this._totalAmount;
  set totalAmount(double? totalAmount) => _$this._totalAmount = totalAmount;

  String? _estimateRevisionId;
  String? get estimateRevisionId => _$this._estimateRevisionId;
  set estimateRevisionId(String? estimateRevisionId) =>
      _$this._estimateRevisionId = estimateRevisionId;

  int? _estimateRevisionNo;
  int? get estimateRevisionNo => _$this._estimateRevisionNo;
  set estimateRevisionNo(int? estimateRevisionNo) =>
      _$this._estimateRevisionNo = estimateRevisionNo;

  DateTime? _issuedAtUtc;
  DateTime? get issuedAtUtc => _$this._issuedAtUtc;
  set issuedAtUtc(DateTime? issuedAtUtc) => _$this._issuedAtUtc = issuedAtUtc;

  DateTime? _acceptedAtUtc;
  DateTime? get acceptedAtUtc => _$this._acceptedAtUtc;
  set acceptedAtUtc(DateTime? acceptedAtUtc) =>
      _$this._acceptedAtUtc = acceptedAtUtc;

  String? _supersedesQuotationId;
  String? get supersedesQuotationId => _$this._supersedesQuotationId;
  set supersedesQuotationId(String? supersedesQuotationId) =>
      _$this._supersedesQuotationId = supersedesQuotationId;

  String? _supersedesNumber;
  String? get supersedesNumber => _$this._supersedesNumber;
  set supersedesNumber(String? supersedesNumber) =>
      _$this._supersedesNumber = supersedesNumber;

  String? _supersededByQuotationId;
  String? get supersededByQuotationId => _$this._supersededByQuotationId;
  set supersededByQuotationId(String? supersededByQuotationId) =>
      _$this._supersededByQuotationId = supersededByQuotationId;

  String? _supersededByNumber;
  String? get supersededByNumber => _$this._supersededByNumber;
  set supersededByNumber(String? supersededByNumber) =>
      _$this._supersededByNumber = supersededByNumber;

  String? _amendmentReason;
  String? get amendmentReason => _$this._amendmentReason;
  set amendmentReason(String? amendmentReason) =>
      _$this._amendmentReason = amendmentReason;

  DateTime? _voidedAtUtc;
  DateTime? get voidedAtUtc => _$this._voidedAtUtc;
  set voidedAtUtc(DateTime? voidedAtUtc) => _$this._voidedAtUtc = voidedAtUtc;

  QuotationPersonResponseBuilder? _voidedBy;
  QuotationPersonResponseBuilder get voidedBy =>
      _$this._voidedBy ??= QuotationPersonResponseBuilder();
  set voidedBy(QuotationPersonResponseBuilder? voidedBy) =>
      _$this._voidedBy = voidedBy;

  String? _voidReason;
  String? get voidReason => _$this._voidReason;
  set voidReason(String? voidReason) => _$this._voidReason = voidReason;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  QuotationDocumentSummaryResponseBuilder? _document;
  QuotationDocumentSummaryResponseBuilder get document =>
      _$this._document ??= QuotationDocumentSummaryResponseBuilder();
  set document(QuotationDocumentSummaryResponseBuilder? document) =>
      _$this._document = document;

  QuotationHistoryItemResponseBuilder() {
    QuotationHistoryItemResponse._defaults(this);
  }

  QuotationHistoryItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _status = $v.status;
      _totalAmount = $v.totalAmount;
      _estimateRevisionId = $v.estimateRevisionId;
      _estimateRevisionNo = $v.estimateRevisionNo;
      _issuedAtUtc = $v.issuedAtUtc;
      _acceptedAtUtc = $v.acceptedAtUtc;
      _supersedesQuotationId = $v.supersedesQuotationId;
      _supersedesNumber = $v.supersedesNumber;
      _supersededByQuotationId = $v.supersededByQuotationId;
      _supersededByNumber = $v.supersededByNumber;
      _amendmentReason = $v.amendmentReason;
      _voidedAtUtc = $v.voidedAtUtc;
      _voidedBy = $v.voidedBy?.toBuilder();
      _voidReason = $v.voidReason;
      _rowVersion = $v.rowVersion;
      _document = $v.document?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationHistoryItemResponse other) {
    _$v = other as _$QuotationHistoryItemResponse;
  }

  @override
  void update(void Function(QuotationHistoryItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationHistoryItemResponse build() => _build();

  _$QuotationHistoryItemResponse _build() {
    _$QuotationHistoryItemResponse _$result;
    try {
      _$result = _$v ??
          _$QuotationHistoryItemResponse._(
            id: id,
            number: number,
            status: status,
            totalAmount: totalAmount,
            estimateRevisionId: estimateRevisionId,
            estimateRevisionNo: estimateRevisionNo,
            issuedAtUtc: issuedAtUtc,
            acceptedAtUtc: acceptedAtUtc,
            supersedesQuotationId: supersedesQuotationId,
            supersedesNumber: supersedesNumber,
            supersededByQuotationId: supersededByQuotationId,
            supersededByNumber: supersededByNumber,
            amendmentReason: amendmentReason,
            voidedAtUtc: voidedAtUtc,
            voidedBy: _voidedBy?.build(),
            voidReason: voidReason,
            rowVersion: rowVersion,
            document: _document?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'voidedBy';
        _voidedBy?.build();

        _$failedField = 'document';
        _document?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'QuotationHistoryItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
