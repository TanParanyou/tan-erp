// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_cost_evidence_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewCostEvidenceResponse
    extends EstimateReviewCostEvidenceResponse {
  @override
  final String? workItemCode;
  @override
  final String? workItemDescription;
  @override
  final String? type;
  @override
  final String? description;
  @override
  final double? quantity;
  @override
  final String? unitCode;
  @override
  final double? unitCost;
  @override
  final String? currency;
  @override
  final double? totalCost;
  @override
  final String? costOrigin;
  @override
  final String? itemCode;
  @override
  final int? costRecordVersion;
  @override
  final String? costSourceCode;
  @override
  final String? sourceReference;
  @override
  final String? evidenceFileId;
  @override
  final String? costRecordReason;
  @override
  final String? provisionalReasonCode;
  @override
  final String? provisionalNote;
  @override
  final bool? isProvisional;
  @override
  final EstimateConversionSnapshotResponse? conversion;

  factory _$EstimateReviewCostEvidenceResponse(
          [void Function(EstimateReviewCostEvidenceResponseBuilder)?
              updates]) =>
      (EstimateReviewCostEvidenceResponseBuilder()..update(updates))._build();

  _$EstimateReviewCostEvidenceResponse._(
      {this.workItemCode,
      this.workItemDescription,
      this.type,
      this.description,
      this.quantity,
      this.unitCode,
      this.unitCost,
      this.currency,
      this.totalCost,
      this.costOrigin,
      this.itemCode,
      this.costRecordVersion,
      this.costSourceCode,
      this.sourceReference,
      this.evidenceFileId,
      this.costRecordReason,
      this.provisionalReasonCode,
      this.provisionalNote,
      this.isProvisional,
      this.conversion})
      : super._();
  @override
  EstimateReviewCostEvidenceResponse rebuild(
          void Function(EstimateReviewCostEvidenceResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewCostEvidenceResponseBuilder toBuilder() =>
      EstimateReviewCostEvidenceResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewCostEvidenceResponse &&
        workItemCode == other.workItemCode &&
        workItemDescription == other.workItemDescription &&
        type == other.type &&
        description == other.description &&
        quantity == other.quantity &&
        unitCode == other.unitCode &&
        unitCost == other.unitCost &&
        currency == other.currency &&
        totalCost == other.totalCost &&
        costOrigin == other.costOrigin &&
        itemCode == other.itemCode &&
        costRecordVersion == other.costRecordVersion &&
        costSourceCode == other.costSourceCode &&
        sourceReference == other.sourceReference &&
        evidenceFileId == other.evidenceFileId &&
        costRecordReason == other.costRecordReason &&
        provisionalReasonCode == other.provisionalReasonCode &&
        provisionalNote == other.provisionalNote &&
        isProvisional == other.isProvisional &&
        conversion == other.conversion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, workItemCode.hashCode);
    _$hash = $jc(_$hash, workItemDescription.hashCode);
    _$hash = $jc(_$hash, type.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, quantity.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, unitCost.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, totalCost.hashCode);
    _$hash = $jc(_$hash, costOrigin.hashCode);
    _$hash = $jc(_$hash, itemCode.hashCode);
    _$hash = $jc(_$hash, costRecordVersion.hashCode);
    _$hash = $jc(_$hash, costSourceCode.hashCode);
    _$hash = $jc(_$hash, sourceReference.hashCode);
    _$hash = $jc(_$hash, evidenceFileId.hashCode);
    _$hash = $jc(_$hash, costRecordReason.hashCode);
    _$hash = $jc(_$hash, provisionalReasonCode.hashCode);
    _$hash = $jc(_$hash, provisionalNote.hashCode);
    _$hash = $jc(_$hash, isProvisional.hashCode);
    _$hash = $jc(_$hash, conversion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReviewCostEvidenceResponse')
          ..add('workItemCode', workItemCode)
          ..add('workItemDescription', workItemDescription)
          ..add('type', type)
          ..add('description', description)
          ..add('quantity', quantity)
          ..add('unitCode', unitCode)
          ..add('unitCost', unitCost)
          ..add('currency', currency)
          ..add('totalCost', totalCost)
          ..add('costOrigin', costOrigin)
          ..add('itemCode', itemCode)
          ..add('costRecordVersion', costRecordVersion)
          ..add('costSourceCode', costSourceCode)
          ..add('sourceReference', sourceReference)
          ..add('evidenceFileId', evidenceFileId)
          ..add('costRecordReason', costRecordReason)
          ..add('provisionalReasonCode', provisionalReasonCode)
          ..add('provisionalNote', provisionalNote)
          ..add('isProvisional', isProvisional)
          ..add('conversion', conversion))
        .toString();
  }
}

class EstimateReviewCostEvidenceResponseBuilder
    implements
        Builder<EstimateReviewCostEvidenceResponse,
            EstimateReviewCostEvidenceResponseBuilder> {
  _$EstimateReviewCostEvidenceResponse? _$v;

  String? _workItemCode;
  String? get workItemCode => _$this._workItemCode;
  set workItemCode(String? workItemCode) => _$this._workItemCode = workItemCode;

  String? _workItemDescription;
  String? get workItemDescription => _$this._workItemDescription;
  set workItemDescription(String? workItemDescription) =>
      _$this._workItemDescription = workItemDescription;

  String? _type;
  String? get type => _$this._type;
  set type(String? type) => _$this._type = type;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  double? _quantity;
  double? get quantity => _$this._quantity;
  set quantity(double? quantity) => _$this._quantity = quantity;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  double? _unitCost;
  double? get unitCost => _$this._unitCost;
  set unitCost(double? unitCost) => _$this._unitCost = unitCost;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  double? _totalCost;
  double? get totalCost => _$this._totalCost;
  set totalCost(double? totalCost) => _$this._totalCost = totalCost;

  String? _costOrigin;
  String? get costOrigin => _$this._costOrigin;
  set costOrigin(String? costOrigin) => _$this._costOrigin = costOrigin;

  String? _itemCode;
  String? get itemCode => _$this._itemCode;
  set itemCode(String? itemCode) => _$this._itemCode = itemCode;

  int? _costRecordVersion;
  int? get costRecordVersion => _$this._costRecordVersion;
  set costRecordVersion(int? costRecordVersion) =>
      _$this._costRecordVersion = costRecordVersion;

  String? _costSourceCode;
  String? get costSourceCode => _$this._costSourceCode;
  set costSourceCode(String? costSourceCode) =>
      _$this._costSourceCode = costSourceCode;

  String? _sourceReference;
  String? get sourceReference => _$this._sourceReference;
  set sourceReference(String? sourceReference) =>
      _$this._sourceReference = sourceReference;

  String? _evidenceFileId;
  String? get evidenceFileId => _$this._evidenceFileId;
  set evidenceFileId(String? evidenceFileId) =>
      _$this._evidenceFileId = evidenceFileId;

  String? _costRecordReason;
  String? get costRecordReason => _$this._costRecordReason;
  set costRecordReason(String? costRecordReason) =>
      _$this._costRecordReason = costRecordReason;

  String? _provisionalReasonCode;
  String? get provisionalReasonCode => _$this._provisionalReasonCode;
  set provisionalReasonCode(String? provisionalReasonCode) =>
      _$this._provisionalReasonCode = provisionalReasonCode;

  String? _provisionalNote;
  String? get provisionalNote => _$this._provisionalNote;
  set provisionalNote(String? provisionalNote) =>
      _$this._provisionalNote = provisionalNote;

  bool? _isProvisional;
  bool? get isProvisional => _$this._isProvisional;
  set isProvisional(bool? isProvisional) =>
      _$this._isProvisional = isProvisional;

  EstimateConversionSnapshotResponseBuilder? _conversion;
  EstimateConversionSnapshotResponseBuilder get conversion =>
      _$this._conversion ??= EstimateConversionSnapshotResponseBuilder();
  set conversion(EstimateConversionSnapshotResponseBuilder? conversion) =>
      _$this._conversion = conversion;

  EstimateReviewCostEvidenceResponseBuilder() {
    EstimateReviewCostEvidenceResponse._defaults(this);
  }

  EstimateReviewCostEvidenceResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _workItemCode = $v.workItemCode;
      _workItemDescription = $v.workItemDescription;
      _type = $v.type;
      _description = $v.description;
      _quantity = $v.quantity;
      _unitCode = $v.unitCode;
      _unitCost = $v.unitCost;
      _currency = $v.currency;
      _totalCost = $v.totalCost;
      _costOrigin = $v.costOrigin;
      _itemCode = $v.itemCode;
      _costRecordVersion = $v.costRecordVersion;
      _costSourceCode = $v.costSourceCode;
      _sourceReference = $v.sourceReference;
      _evidenceFileId = $v.evidenceFileId;
      _costRecordReason = $v.costRecordReason;
      _provisionalReasonCode = $v.provisionalReasonCode;
      _provisionalNote = $v.provisionalNote;
      _isProvisional = $v.isProvisional;
      _conversion = $v.conversion?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewCostEvidenceResponse other) {
    _$v = other as _$EstimateReviewCostEvidenceResponse;
  }

  @override
  void update(
      void Function(EstimateReviewCostEvidenceResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewCostEvidenceResponse build() => _build();

  _$EstimateReviewCostEvidenceResponse _build() {
    _$EstimateReviewCostEvidenceResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateReviewCostEvidenceResponse._(
            workItemCode: workItemCode,
            workItemDescription: workItemDescription,
            type: type,
            description: description,
            quantity: quantity,
            unitCode: unitCode,
            unitCost: unitCost,
            currency: currency,
            totalCost: totalCost,
            costOrigin: costOrigin,
            itemCode: itemCode,
            costRecordVersion: costRecordVersion,
            costSourceCode: costSourceCode,
            sourceReference: sourceReference,
            evidenceFileId: evidenceFileId,
            costRecordReason: costRecordReason,
            provisionalReasonCode: provisionalReasonCode,
            provisionalNote: provisionalNote,
            isProvisional: isProvisional,
            conversion: _conversion?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'conversion';
        _conversion?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateReviewCostEvidenceResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
