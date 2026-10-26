// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_revision_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateRevisionResponse extends EstimateRevisionResponse {
  @override
  final String? id;
  @override
  final String? estimateId;
  @override
  final int? revisionNo;
  @override
  final String? status;
  @override
  final String? currency;
  @override
  final int? calculationVersion;
  @override
  final bool? calculationOutdated;
  @override
  final String? calculationPolicyVersion;
  @override
  final String? taxPolicyVersion;
  @override
  final double? netCost;
  @override
  final double? sellingBeforeDiscount;
  @override
  final String? discountType;
  @override
  final double? discountValue;
  @override
  final String? discountReasonCode;
  @override
  final double? discountAmount;
  @override
  final double? netBeforeTax;
  @override
  final double? taxAmount;
  @override
  final double? grandTotal;
  @override
  final double? marginAmount;
  @override
  final double? marginRate;
  @override
  final double? markupRate;
  @override
  final String? calculationSnapshotJson;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;
  @override
  final BuiltList<EstimateSectionResponse>? sections;
  @override
  final String? readiness;
  @override
  final BuiltList<EstimateReadinessReasonResponse>? readinessReasons;

  factory _$EstimateRevisionResponse(
          [void Function(EstimateRevisionResponseBuilder)? updates]) =>
      (EstimateRevisionResponseBuilder()..update(updates))._build();

  _$EstimateRevisionResponse._(
      {this.id,
      this.estimateId,
      this.revisionNo,
      this.status,
      this.currency,
      this.calculationVersion,
      this.calculationOutdated,
      this.calculationPolicyVersion,
      this.taxPolicyVersion,
      this.netCost,
      this.sellingBeforeDiscount,
      this.discountType,
      this.discountValue,
      this.discountReasonCode,
      this.discountAmount,
      this.netBeforeTax,
      this.taxAmount,
      this.grandTotal,
      this.marginAmount,
      this.marginRate,
      this.markupRate,
      this.calculationSnapshotJson,
      this.rowVersion,
      this.createdAtUtc,
      this.updatedAtUtc,
      this.sections,
      this.readiness,
      this.readinessReasons})
      : super._();
  @override
  EstimateRevisionResponse rebuild(
          void Function(EstimateRevisionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateRevisionResponseBuilder toBuilder() =>
      EstimateRevisionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateRevisionResponse &&
        id == other.id &&
        estimateId == other.estimateId &&
        revisionNo == other.revisionNo &&
        status == other.status &&
        currency == other.currency &&
        calculationVersion == other.calculationVersion &&
        calculationOutdated == other.calculationOutdated &&
        calculationPolicyVersion == other.calculationPolicyVersion &&
        taxPolicyVersion == other.taxPolicyVersion &&
        netCost == other.netCost &&
        sellingBeforeDiscount == other.sellingBeforeDiscount &&
        discountType == other.discountType &&
        discountValue == other.discountValue &&
        discountReasonCode == other.discountReasonCode &&
        discountAmount == other.discountAmount &&
        netBeforeTax == other.netBeforeTax &&
        taxAmount == other.taxAmount &&
        grandTotal == other.grandTotal &&
        marginAmount == other.marginAmount &&
        marginRate == other.marginRate &&
        markupRate == other.markupRate &&
        calculationSnapshotJson == other.calculationSnapshotJson &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc &&
        sections == other.sections &&
        readiness == other.readiness &&
        readinessReasons == other.readinessReasons;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, estimateId.hashCode);
    _$hash = $jc(_$hash, revisionNo.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, calculationVersion.hashCode);
    _$hash = $jc(_$hash, calculationOutdated.hashCode);
    _$hash = $jc(_$hash, calculationPolicyVersion.hashCode);
    _$hash = $jc(_$hash, taxPolicyVersion.hashCode);
    _$hash = $jc(_$hash, netCost.hashCode);
    _$hash = $jc(_$hash, sellingBeforeDiscount.hashCode);
    _$hash = $jc(_$hash, discountType.hashCode);
    _$hash = $jc(_$hash, discountValue.hashCode);
    _$hash = $jc(_$hash, discountReasonCode.hashCode);
    _$hash = $jc(_$hash, discountAmount.hashCode);
    _$hash = $jc(_$hash, netBeforeTax.hashCode);
    _$hash = $jc(_$hash, taxAmount.hashCode);
    _$hash = $jc(_$hash, grandTotal.hashCode);
    _$hash = $jc(_$hash, marginAmount.hashCode);
    _$hash = $jc(_$hash, marginRate.hashCode);
    _$hash = $jc(_$hash, markupRate.hashCode);
    _$hash = $jc(_$hash, calculationSnapshotJson.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jc(_$hash, sections.hashCode);
    _$hash = $jc(_$hash, readiness.hashCode);
    _$hash = $jc(_$hash, readinessReasons.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateRevisionResponse')
          ..add('id', id)
          ..add('estimateId', estimateId)
          ..add('revisionNo', revisionNo)
          ..add('status', status)
          ..add('currency', currency)
          ..add('calculationVersion', calculationVersion)
          ..add('calculationOutdated', calculationOutdated)
          ..add('calculationPolicyVersion', calculationPolicyVersion)
          ..add('taxPolicyVersion', taxPolicyVersion)
          ..add('netCost', netCost)
          ..add('sellingBeforeDiscount', sellingBeforeDiscount)
          ..add('discountType', discountType)
          ..add('discountValue', discountValue)
          ..add('discountReasonCode', discountReasonCode)
          ..add('discountAmount', discountAmount)
          ..add('netBeforeTax', netBeforeTax)
          ..add('taxAmount', taxAmount)
          ..add('grandTotal', grandTotal)
          ..add('marginAmount', marginAmount)
          ..add('marginRate', marginRate)
          ..add('markupRate', markupRate)
          ..add('calculationSnapshotJson', calculationSnapshotJson)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc)
          ..add('sections', sections)
          ..add('readiness', readiness)
          ..add('readinessReasons', readinessReasons))
        .toString();
  }
}

class EstimateRevisionResponseBuilder
    implements
        Builder<EstimateRevisionResponse, EstimateRevisionResponseBuilder> {
  _$EstimateRevisionResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _estimateId;
  String? get estimateId => _$this._estimateId;
  set estimateId(String? estimateId) => _$this._estimateId = estimateId;

  int? _revisionNo;
  int? get revisionNo => _$this._revisionNo;
  set revisionNo(int? revisionNo) => _$this._revisionNo = revisionNo;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  int? _calculationVersion;
  int? get calculationVersion => _$this._calculationVersion;
  set calculationVersion(int? calculationVersion) =>
      _$this._calculationVersion = calculationVersion;

  bool? _calculationOutdated;
  bool? get calculationOutdated => _$this._calculationOutdated;
  set calculationOutdated(bool? calculationOutdated) =>
      _$this._calculationOutdated = calculationOutdated;

  String? _calculationPolicyVersion;
  String? get calculationPolicyVersion => _$this._calculationPolicyVersion;
  set calculationPolicyVersion(String? calculationPolicyVersion) =>
      _$this._calculationPolicyVersion = calculationPolicyVersion;

  String? _taxPolicyVersion;
  String? get taxPolicyVersion => _$this._taxPolicyVersion;
  set taxPolicyVersion(String? taxPolicyVersion) =>
      _$this._taxPolicyVersion = taxPolicyVersion;

  double? _netCost;
  double? get netCost => _$this._netCost;
  set netCost(double? netCost) => _$this._netCost = netCost;

  double? _sellingBeforeDiscount;
  double? get sellingBeforeDiscount => _$this._sellingBeforeDiscount;
  set sellingBeforeDiscount(double? sellingBeforeDiscount) =>
      _$this._sellingBeforeDiscount = sellingBeforeDiscount;

  String? _discountType;
  String? get discountType => _$this._discountType;
  set discountType(String? discountType) => _$this._discountType = discountType;

  double? _discountValue;
  double? get discountValue => _$this._discountValue;
  set discountValue(double? discountValue) =>
      _$this._discountValue = discountValue;

  String? _discountReasonCode;
  String? get discountReasonCode => _$this._discountReasonCode;
  set discountReasonCode(String? discountReasonCode) =>
      _$this._discountReasonCode = discountReasonCode;

  double? _discountAmount;
  double? get discountAmount => _$this._discountAmount;
  set discountAmount(double? discountAmount) =>
      _$this._discountAmount = discountAmount;

  double? _netBeforeTax;
  double? get netBeforeTax => _$this._netBeforeTax;
  set netBeforeTax(double? netBeforeTax) => _$this._netBeforeTax = netBeforeTax;

  double? _taxAmount;
  double? get taxAmount => _$this._taxAmount;
  set taxAmount(double? taxAmount) => _$this._taxAmount = taxAmount;

  double? _grandTotal;
  double? get grandTotal => _$this._grandTotal;
  set grandTotal(double? grandTotal) => _$this._grandTotal = grandTotal;

  double? _marginAmount;
  double? get marginAmount => _$this._marginAmount;
  set marginAmount(double? marginAmount) => _$this._marginAmount = marginAmount;

  double? _marginRate;
  double? get marginRate => _$this._marginRate;
  set marginRate(double? marginRate) => _$this._marginRate = marginRate;

  double? _markupRate;
  double? get markupRate => _$this._markupRate;
  set markupRate(double? markupRate) => _$this._markupRate = markupRate;

  String? _calculationSnapshotJson;
  String? get calculationSnapshotJson => _$this._calculationSnapshotJson;
  set calculationSnapshotJson(String? calculationSnapshotJson) =>
      _$this._calculationSnapshotJson = calculationSnapshotJson;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  ListBuilder<EstimateSectionResponse>? _sections;
  ListBuilder<EstimateSectionResponse> get sections =>
      _$this._sections ??= ListBuilder<EstimateSectionResponse>();
  set sections(ListBuilder<EstimateSectionResponse>? sections) =>
      _$this._sections = sections;

  String? _readiness;
  String? get readiness => _$this._readiness;
  set readiness(String? readiness) => _$this._readiness = readiness;

  ListBuilder<EstimateReadinessReasonResponse>? _readinessReasons;
  ListBuilder<EstimateReadinessReasonResponse> get readinessReasons =>
      _$this._readinessReasons ??=
          ListBuilder<EstimateReadinessReasonResponse>();
  set readinessReasons(
          ListBuilder<EstimateReadinessReasonResponse>? readinessReasons) =>
      _$this._readinessReasons = readinessReasons;

  EstimateRevisionResponseBuilder() {
    EstimateRevisionResponse._defaults(this);
  }

  EstimateRevisionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _estimateId = $v.estimateId;
      _revisionNo = $v.revisionNo;
      _status = $v.status;
      _currency = $v.currency;
      _calculationVersion = $v.calculationVersion;
      _calculationOutdated = $v.calculationOutdated;
      _calculationPolicyVersion = $v.calculationPolicyVersion;
      _taxPolicyVersion = $v.taxPolicyVersion;
      _netCost = $v.netCost;
      _sellingBeforeDiscount = $v.sellingBeforeDiscount;
      _discountType = $v.discountType;
      _discountValue = $v.discountValue;
      _discountReasonCode = $v.discountReasonCode;
      _discountAmount = $v.discountAmount;
      _netBeforeTax = $v.netBeforeTax;
      _taxAmount = $v.taxAmount;
      _grandTotal = $v.grandTotal;
      _marginAmount = $v.marginAmount;
      _marginRate = $v.marginRate;
      _markupRate = $v.markupRate;
      _calculationSnapshotJson = $v.calculationSnapshotJson;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _sections = $v.sections?.toBuilder();
      _readiness = $v.readiness;
      _readinessReasons = $v.readinessReasons?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateRevisionResponse other) {
    _$v = other as _$EstimateRevisionResponse;
  }

  @override
  void update(void Function(EstimateRevisionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateRevisionResponse build() => _build();

  _$EstimateRevisionResponse _build() {
    _$EstimateRevisionResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateRevisionResponse._(
            id: id,
            estimateId: estimateId,
            revisionNo: revisionNo,
            status: status,
            currency: currency,
            calculationVersion: calculationVersion,
            calculationOutdated: calculationOutdated,
            calculationPolicyVersion: calculationPolicyVersion,
            taxPolicyVersion: taxPolicyVersion,
            netCost: netCost,
            sellingBeforeDiscount: sellingBeforeDiscount,
            discountType: discountType,
            discountValue: discountValue,
            discountReasonCode: discountReasonCode,
            discountAmount: discountAmount,
            netBeforeTax: netBeforeTax,
            taxAmount: taxAmount,
            grandTotal: grandTotal,
            marginAmount: marginAmount,
            marginRate: marginRate,
            markupRate: markupRate,
            calculationSnapshotJson: calculationSnapshotJson,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            updatedAtUtc: updatedAtUtc,
            sections: _sections?.build(),
            readiness: readiness,
            readinessReasons: _readinessReasons?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'sections';
        _sections?.build();

        _$failedField = 'readinessReasons';
        _readinessReasons?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateRevisionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
