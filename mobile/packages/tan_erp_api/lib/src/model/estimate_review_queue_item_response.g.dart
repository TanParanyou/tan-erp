// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_queue_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewQueueItemResponse
    extends EstimateReviewQueueItemResponse {
  @override
  final String? approvalRequestId;
  @override
  final String? estimateId;
  @override
  final String? estimateNumber;
  @override
  final String? estimateRowVersion;
  @override
  final String? revisionId;
  @override
  final int? revisionNo;
  @override
  final String? revisionStatus;
  @override
  final int? calculationVersion;
  @override
  final String? currency;
  @override
  final double? grandTotal;
  @override
  final double? marginRate;
  @override
  final String? opportunityId;
  @override
  final EstimateReviewQueueOpportunityResponse? opportunity;
  @override
  final EstimateReviewQueueCustomerResponse? customer;
  @override
  final EstimateReviewQueueBranchResponse? branch;
  @override
  final EstimateReviewQueuePersonResponse? requestedBy;
  @override
  final DateTime? requestedAtUtc;
  @override
  final String? submissionNote;
  @override
  final String? calculationInputHash;
  @override
  final String? calculationSnapshotHash;
  @override
  final EstimateReviewFrozenRouteResponse? frozenRoute;
  @override
  final String? readiness;
  @override
  final BuiltList<EstimateReviewReadinessReasonResponse>? readinessReasons;
  @override
  final BuiltList<EstimateReviewPriceOverrideResponse>? priceOverrides;
  @override
  final BuiltList<EstimateReviewCostEvidenceResponse>? costEvidence;
  @override
  final EstimateReviewRevisionDiffResponse? revisionDiff;
  @override
  final int? provisionalCostCount;

  factory _$EstimateReviewQueueItemResponse(
          [void Function(EstimateReviewQueueItemResponseBuilder)? updates]) =>
      (EstimateReviewQueueItemResponseBuilder()..update(updates))._build();

  _$EstimateReviewQueueItemResponse._(
      {this.approvalRequestId,
      this.estimateId,
      this.estimateNumber,
      this.estimateRowVersion,
      this.revisionId,
      this.revisionNo,
      this.revisionStatus,
      this.calculationVersion,
      this.currency,
      this.grandTotal,
      this.marginRate,
      this.opportunityId,
      this.opportunity,
      this.customer,
      this.branch,
      this.requestedBy,
      this.requestedAtUtc,
      this.submissionNote,
      this.calculationInputHash,
      this.calculationSnapshotHash,
      this.frozenRoute,
      this.readiness,
      this.readinessReasons,
      this.priceOverrides,
      this.costEvidence,
      this.revisionDiff,
      this.provisionalCostCount})
      : super._();
  @override
  EstimateReviewQueueItemResponse rebuild(
          void Function(EstimateReviewQueueItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewQueueItemResponseBuilder toBuilder() =>
      EstimateReviewQueueItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewQueueItemResponse &&
        approvalRequestId == other.approvalRequestId &&
        estimateId == other.estimateId &&
        estimateNumber == other.estimateNumber &&
        estimateRowVersion == other.estimateRowVersion &&
        revisionId == other.revisionId &&
        revisionNo == other.revisionNo &&
        revisionStatus == other.revisionStatus &&
        calculationVersion == other.calculationVersion &&
        currency == other.currency &&
        grandTotal == other.grandTotal &&
        marginRate == other.marginRate &&
        opportunityId == other.opportunityId &&
        opportunity == other.opportunity &&
        customer == other.customer &&
        branch == other.branch &&
        requestedBy == other.requestedBy &&
        requestedAtUtc == other.requestedAtUtc &&
        submissionNote == other.submissionNote &&
        calculationInputHash == other.calculationInputHash &&
        calculationSnapshotHash == other.calculationSnapshotHash &&
        frozenRoute == other.frozenRoute &&
        readiness == other.readiness &&
        readinessReasons == other.readinessReasons &&
        priceOverrides == other.priceOverrides &&
        costEvidence == other.costEvidence &&
        revisionDiff == other.revisionDiff &&
        provisionalCostCount == other.provisionalCostCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, approvalRequestId.hashCode);
    _$hash = $jc(_$hash, estimateId.hashCode);
    _$hash = $jc(_$hash, estimateNumber.hashCode);
    _$hash = $jc(_$hash, estimateRowVersion.hashCode);
    _$hash = $jc(_$hash, revisionId.hashCode);
    _$hash = $jc(_$hash, revisionNo.hashCode);
    _$hash = $jc(_$hash, revisionStatus.hashCode);
    _$hash = $jc(_$hash, calculationVersion.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, grandTotal.hashCode);
    _$hash = $jc(_$hash, marginRate.hashCode);
    _$hash = $jc(_$hash, opportunityId.hashCode);
    _$hash = $jc(_$hash, opportunity.hashCode);
    _$hash = $jc(_$hash, customer.hashCode);
    _$hash = $jc(_$hash, branch.hashCode);
    _$hash = $jc(_$hash, requestedBy.hashCode);
    _$hash = $jc(_$hash, requestedAtUtc.hashCode);
    _$hash = $jc(_$hash, submissionNote.hashCode);
    _$hash = $jc(_$hash, calculationInputHash.hashCode);
    _$hash = $jc(_$hash, calculationSnapshotHash.hashCode);
    _$hash = $jc(_$hash, frozenRoute.hashCode);
    _$hash = $jc(_$hash, readiness.hashCode);
    _$hash = $jc(_$hash, readinessReasons.hashCode);
    _$hash = $jc(_$hash, priceOverrides.hashCode);
    _$hash = $jc(_$hash, costEvidence.hashCode);
    _$hash = $jc(_$hash, revisionDiff.hashCode);
    _$hash = $jc(_$hash, provisionalCostCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReviewQueueItemResponse')
          ..add('approvalRequestId', approvalRequestId)
          ..add('estimateId', estimateId)
          ..add('estimateNumber', estimateNumber)
          ..add('estimateRowVersion', estimateRowVersion)
          ..add('revisionId', revisionId)
          ..add('revisionNo', revisionNo)
          ..add('revisionStatus', revisionStatus)
          ..add('calculationVersion', calculationVersion)
          ..add('currency', currency)
          ..add('grandTotal', grandTotal)
          ..add('marginRate', marginRate)
          ..add('opportunityId', opportunityId)
          ..add('opportunity', opportunity)
          ..add('customer', customer)
          ..add('branch', branch)
          ..add('requestedBy', requestedBy)
          ..add('requestedAtUtc', requestedAtUtc)
          ..add('submissionNote', submissionNote)
          ..add('calculationInputHash', calculationInputHash)
          ..add('calculationSnapshotHash', calculationSnapshotHash)
          ..add('frozenRoute', frozenRoute)
          ..add('readiness', readiness)
          ..add('readinessReasons', readinessReasons)
          ..add('priceOverrides', priceOverrides)
          ..add('costEvidence', costEvidence)
          ..add('revisionDiff', revisionDiff)
          ..add('provisionalCostCount', provisionalCostCount))
        .toString();
  }
}

class EstimateReviewQueueItemResponseBuilder
    implements
        Builder<EstimateReviewQueueItemResponse,
            EstimateReviewQueueItemResponseBuilder> {
  _$EstimateReviewQueueItemResponse? _$v;

  String? _approvalRequestId;
  String? get approvalRequestId => _$this._approvalRequestId;
  set approvalRequestId(String? approvalRequestId) =>
      _$this._approvalRequestId = approvalRequestId;

  String? _estimateId;
  String? get estimateId => _$this._estimateId;
  set estimateId(String? estimateId) => _$this._estimateId = estimateId;

  String? _estimateNumber;
  String? get estimateNumber => _$this._estimateNumber;
  set estimateNumber(String? estimateNumber) =>
      _$this._estimateNumber = estimateNumber;

  String? _estimateRowVersion;
  String? get estimateRowVersion => _$this._estimateRowVersion;
  set estimateRowVersion(String? estimateRowVersion) =>
      _$this._estimateRowVersion = estimateRowVersion;

  String? _revisionId;
  String? get revisionId => _$this._revisionId;
  set revisionId(String? revisionId) => _$this._revisionId = revisionId;

  int? _revisionNo;
  int? get revisionNo => _$this._revisionNo;
  set revisionNo(int? revisionNo) => _$this._revisionNo = revisionNo;

  String? _revisionStatus;
  String? get revisionStatus => _$this._revisionStatus;
  set revisionStatus(String? revisionStatus) =>
      _$this._revisionStatus = revisionStatus;

  int? _calculationVersion;
  int? get calculationVersion => _$this._calculationVersion;
  set calculationVersion(int? calculationVersion) =>
      _$this._calculationVersion = calculationVersion;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  double? _grandTotal;
  double? get grandTotal => _$this._grandTotal;
  set grandTotal(double? grandTotal) => _$this._grandTotal = grandTotal;

  double? _marginRate;
  double? get marginRate => _$this._marginRate;
  set marginRate(double? marginRate) => _$this._marginRate = marginRate;

  String? _opportunityId;
  String? get opportunityId => _$this._opportunityId;
  set opportunityId(String? opportunityId) =>
      _$this._opportunityId = opportunityId;

  EstimateReviewQueueOpportunityResponseBuilder? _opportunity;
  EstimateReviewQueueOpportunityResponseBuilder get opportunity =>
      _$this._opportunity ??= EstimateReviewQueueOpportunityResponseBuilder();
  set opportunity(EstimateReviewQueueOpportunityResponseBuilder? opportunity) =>
      _$this._opportunity = opportunity;

  EstimateReviewQueueCustomerResponseBuilder? _customer;
  EstimateReviewQueueCustomerResponseBuilder get customer =>
      _$this._customer ??= EstimateReviewQueueCustomerResponseBuilder();
  set customer(EstimateReviewQueueCustomerResponseBuilder? customer) =>
      _$this._customer = customer;

  EstimateReviewQueueBranchResponseBuilder? _branch;
  EstimateReviewQueueBranchResponseBuilder get branch =>
      _$this._branch ??= EstimateReviewQueueBranchResponseBuilder();
  set branch(EstimateReviewQueueBranchResponseBuilder? branch) =>
      _$this._branch = branch;

  EstimateReviewQueuePersonResponseBuilder? _requestedBy;
  EstimateReviewQueuePersonResponseBuilder get requestedBy =>
      _$this._requestedBy ??= EstimateReviewQueuePersonResponseBuilder();
  set requestedBy(EstimateReviewQueuePersonResponseBuilder? requestedBy) =>
      _$this._requestedBy = requestedBy;

  DateTime? _requestedAtUtc;
  DateTime? get requestedAtUtc => _$this._requestedAtUtc;
  set requestedAtUtc(DateTime? requestedAtUtc) =>
      _$this._requestedAtUtc = requestedAtUtc;

  String? _submissionNote;
  String? get submissionNote => _$this._submissionNote;
  set submissionNote(String? submissionNote) =>
      _$this._submissionNote = submissionNote;

  String? _calculationInputHash;
  String? get calculationInputHash => _$this._calculationInputHash;
  set calculationInputHash(String? calculationInputHash) =>
      _$this._calculationInputHash = calculationInputHash;

  String? _calculationSnapshotHash;
  String? get calculationSnapshotHash => _$this._calculationSnapshotHash;
  set calculationSnapshotHash(String? calculationSnapshotHash) =>
      _$this._calculationSnapshotHash = calculationSnapshotHash;

  EstimateReviewFrozenRouteResponseBuilder? _frozenRoute;
  EstimateReviewFrozenRouteResponseBuilder get frozenRoute =>
      _$this._frozenRoute ??= EstimateReviewFrozenRouteResponseBuilder();
  set frozenRoute(EstimateReviewFrozenRouteResponseBuilder? frozenRoute) =>
      _$this._frozenRoute = frozenRoute;

  String? _readiness;
  String? get readiness => _$this._readiness;
  set readiness(String? readiness) => _$this._readiness = readiness;

  ListBuilder<EstimateReviewReadinessReasonResponse>? _readinessReasons;
  ListBuilder<EstimateReviewReadinessReasonResponse> get readinessReasons =>
      _$this._readinessReasons ??=
          ListBuilder<EstimateReviewReadinessReasonResponse>();
  set readinessReasons(
          ListBuilder<EstimateReviewReadinessReasonResponse>?
              readinessReasons) =>
      _$this._readinessReasons = readinessReasons;

  ListBuilder<EstimateReviewPriceOverrideResponse>? _priceOverrides;
  ListBuilder<EstimateReviewPriceOverrideResponse> get priceOverrides =>
      _$this._priceOverrides ??=
          ListBuilder<EstimateReviewPriceOverrideResponse>();
  set priceOverrides(
          ListBuilder<EstimateReviewPriceOverrideResponse>? priceOverrides) =>
      _$this._priceOverrides = priceOverrides;

  ListBuilder<EstimateReviewCostEvidenceResponse>? _costEvidence;
  ListBuilder<EstimateReviewCostEvidenceResponse> get costEvidence =>
      _$this._costEvidence ??=
          ListBuilder<EstimateReviewCostEvidenceResponse>();
  set costEvidence(
          ListBuilder<EstimateReviewCostEvidenceResponse>? costEvidence) =>
      _$this._costEvidence = costEvidence;

  EstimateReviewRevisionDiffResponseBuilder? _revisionDiff;
  EstimateReviewRevisionDiffResponseBuilder get revisionDiff =>
      _$this._revisionDiff ??= EstimateReviewRevisionDiffResponseBuilder();
  set revisionDiff(EstimateReviewRevisionDiffResponseBuilder? revisionDiff) =>
      _$this._revisionDiff = revisionDiff;

  int? _provisionalCostCount;
  int? get provisionalCostCount => _$this._provisionalCostCount;
  set provisionalCostCount(int? provisionalCostCount) =>
      _$this._provisionalCostCount = provisionalCostCount;

  EstimateReviewQueueItemResponseBuilder() {
    EstimateReviewQueueItemResponse._defaults(this);
  }

  EstimateReviewQueueItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _approvalRequestId = $v.approvalRequestId;
      _estimateId = $v.estimateId;
      _estimateNumber = $v.estimateNumber;
      _estimateRowVersion = $v.estimateRowVersion;
      _revisionId = $v.revisionId;
      _revisionNo = $v.revisionNo;
      _revisionStatus = $v.revisionStatus;
      _calculationVersion = $v.calculationVersion;
      _currency = $v.currency;
      _grandTotal = $v.grandTotal;
      _marginRate = $v.marginRate;
      _opportunityId = $v.opportunityId;
      _opportunity = $v.opportunity?.toBuilder();
      _customer = $v.customer?.toBuilder();
      _branch = $v.branch?.toBuilder();
      _requestedBy = $v.requestedBy?.toBuilder();
      _requestedAtUtc = $v.requestedAtUtc;
      _submissionNote = $v.submissionNote;
      _calculationInputHash = $v.calculationInputHash;
      _calculationSnapshotHash = $v.calculationSnapshotHash;
      _frozenRoute = $v.frozenRoute?.toBuilder();
      _readiness = $v.readiness;
      _readinessReasons = $v.readinessReasons?.toBuilder();
      _priceOverrides = $v.priceOverrides?.toBuilder();
      _costEvidence = $v.costEvidence?.toBuilder();
      _revisionDiff = $v.revisionDiff?.toBuilder();
      _provisionalCostCount = $v.provisionalCostCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewQueueItemResponse other) {
    _$v = other as _$EstimateReviewQueueItemResponse;
  }

  @override
  void update(void Function(EstimateReviewQueueItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewQueueItemResponse build() => _build();

  _$EstimateReviewQueueItemResponse _build() {
    _$EstimateReviewQueueItemResponse _$result;
    try {
      _$result = _$v ??
          _$EstimateReviewQueueItemResponse._(
            approvalRequestId: approvalRequestId,
            estimateId: estimateId,
            estimateNumber: estimateNumber,
            estimateRowVersion: estimateRowVersion,
            revisionId: revisionId,
            revisionNo: revisionNo,
            revisionStatus: revisionStatus,
            calculationVersion: calculationVersion,
            currency: currency,
            grandTotal: grandTotal,
            marginRate: marginRate,
            opportunityId: opportunityId,
            opportunity: _opportunity?.build(),
            customer: _customer?.build(),
            branch: _branch?.build(),
            requestedBy: _requestedBy?.build(),
            requestedAtUtc: requestedAtUtc,
            submissionNote: submissionNote,
            calculationInputHash: calculationInputHash,
            calculationSnapshotHash: calculationSnapshotHash,
            frozenRoute: _frozenRoute?.build(),
            readiness: readiness,
            readinessReasons: _readinessReasons?.build(),
            priceOverrides: _priceOverrides?.build(),
            costEvidence: _costEvidence?.build(),
            revisionDiff: _revisionDiff?.build(),
            provisionalCostCount: provisionalCostCount,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'opportunity';
        _opportunity?.build();
        _$failedField = 'customer';
        _customer?.build();
        _$failedField = 'branch';
        _branch?.build();
        _$failedField = 'requestedBy';
        _requestedBy?.build();

        _$failedField = 'frozenRoute';
        _frozenRoute?.build();

        _$failedField = 'readinessReasons';
        _readinessReasons?.build();
        _$failedField = 'priceOverrides';
        _priceOverrides?.build();
        _$failedField = 'costEvidence';
        _costEvidence?.build();
        _$failedField = 'revisionDiff';
        _revisionDiff?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'EstimateReviewQueueItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
