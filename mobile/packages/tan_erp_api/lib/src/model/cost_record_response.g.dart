// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cost_record_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CostRecordResponse extends CostRecordResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? itemId;
  @override
  final String? scope;
  @override
  final String? branchId;
  @override
  final String? unitId;
  @override
  final LocalizedTextDto? unitName;
  @override
  final String? currency;
  @override
  final double? amount;
  @override
  final double? minimumQuantity;
  @override
  final double? maximumQuantity;
  @override
  final DateTime? effectiveFromUtc;
  @override
  final DateTime? effectiveToUtc;
  @override
  final String? status;
  @override
  final int? version;
  @override
  final String? costSourceId;
  @override
  final LocalizedTextDto? costSourceName;
  @override
  final String? sourceReference;
  @override
  final String? reason;
  @override
  final String? evidenceFileId;
  @override
  final String? createdByUserId;
  @override
  final String? lastFinancialEditorId;
  @override
  final String? approvedByUserId;
  @override
  final String? publishedByUserId;
  @override
  final String? rowVersion;
  @override
  final DateTime? createdAtUtc;
  @override
  final DateTime? updatedAtUtc;

  factory _$CostRecordResponse(
          [void Function(CostRecordResponseBuilder)? updates]) =>
      (CostRecordResponseBuilder()..update(updates))._build();

  _$CostRecordResponse._(
      {this.id,
      this.organizationId,
      this.itemId,
      this.scope,
      this.branchId,
      this.unitId,
      this.unitName,
      this.currency,
      this.amount,
      this.minimumQuantity,
      this.maximumQuantity,
      this.effectiveFromUtc,
      this.effectiveToUtc,
      this.status,
      this.version,
      this.costSourceId,
      this.costSourceName,
      this.sourceReference,
      this.reason,
      this.evidenceFileId,
      this.createdByUserId,
      this.lastFinancialEditorId,
      this.approvedByUserId,
      this.publishedByUserId,
      this.rowVersion,
      this.createdAtUtc,
      this.updatedAtUtc})
      : super._();
  @override
  CostRecordResponse rebuild(
          void Function(CostRecordResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CostRecordResponseBuilder toBuilder() =>
      CostRecordResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CostRecordResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        itemId == other.itemId &&
        scope == other.scope &&
        branchId == other.branchId &&
        unitId == other.unitId &&
        unitName == other.unitName &&
        currency == other.currency &&
        amount == other.amount &&
        minimumQuantity == other.minimumQuantity &&
        maximumQuantity == other.maximumQuantity &&
        effectiveFromUtc == other.effectiveFromUtc &&
        effectiveToUtc == other.effectiveToUtc &&
        status == other.status &&
        version == other.version &&
        costSourceId == other.costSourceId &&
        costSourceName == other.costSourceName &&
        sourceReference == other.sourceReference &&
        reason == other.reason &&
        evidenceFileId == other.evidenceFileId &&
        createdByUserId == other.createdByUserId &&
        lastFinancialEditorId == other.lastFinancialEditorId &&
        approvedByUserId == other.approvedByUserId &&
        publishedByUserId == other.publishedByUserId &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, scope.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, unitId.hashCode);
    _$hash = $jc(_$hash, unitName.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, minimumQuantity.hashCode);
    _$hash = $jc(_$hash, maximumQuantity.hashCode);
    _$hash = $jc(_$hash, effectiveFromUtc.hashCode);
    _$hash = $jc(_$hash, effectiveToUtc.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, version.hashCode);
    _$hash = $jc(_$hash, costSourceId.hashCode);
    _$hash = $jc(_$hash, costSourceName.hashCode);
    _$hash = $jc(_$hash, sourceReference.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, evidenceFileId.hashCode);
    _$hash = $jc(_$hash, createdByUserId.hashCode);
    _$hash = $jc(_$hash, lastFinancialEditorId.hashCode);
    _$hash = $jc(_$hash, approvedByUserId.hashCode);
    _$hash = $jc(_$hash, publishedByUserId.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CostRecordResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('itemId', itemId)
          ..add('scope', scope)
          ..add('branchId', branchId)
          ..add('unitId', unitId)
          ..add('unitName', unitName)
          ..add('currency', currency)
          ..add('amount', amount)
          ..add('minimumQuantity', minimumQuantity)
          ..add('maximumQuantity', maximumQuantity)
          ..add('effectiveFromUtc', effectiveFromUtc)
          ..add('effectiveToUtc', effectiveToUtc)
          ..add('status', status)
          ..add('version', version)
          ..add('costSourceId', costSourceId)
          ..add('costSourceName', costSourceName)
          ..add('sourceReference', sourceReference)
          ..add('reason', reason)
          ..add('evidenceFileId', evidenceFileId)
          ..add('createdByUserId', createdByUserId)
          ..add('lastFinancialEditorId', lastFinancialEditorId)
          ..add('approvedByUserId', approvedByUserId)
          ..add('publishedByUserId', publishedByUserId)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class CostRecordResponseBuilder
    implements Builder<CostRecordResponse, CostRecordResponseBuilder> {
  _$CostRecordResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _organizationId;
  String? get organizationId => _$this._organizationId;
  set organizationId(String? organizationId) =>
      _$this._organizationId = organizationId;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  String? _scope;
  String? get scope => _$this._scope;
  set scope(String? scope) => _$this._scope = scope;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _unitId;
  String? get unitId => _$this._unitId;
  set unitId(String? unitId) => _$this._unitId = unitId;

  LocalizedTextDtoBuilder? _unitName;
  LocalizedTextDtoBuilder get unitName =>
      _$this._unitName ??= LocalizedTextDtoBuilder();
  set unitName(LocalizedTextDtoBuilder? unitName) =>
      _$this._unitName = unitName;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  double? _minimumQuantity;
  double? get minimumQuantity => _$this._minimumQuantity;
  set minimumQuantity(double? minimumQuantity) =>
      _$this._minimumQuantity = minimumQuantity;

  double? _maximumQuantity;
  double? get maximumQuantity => _$this._maximumQuantity;
  set maximumQuantity(double? maximumQuantity) =>
      _$this._maximumQuantity = maximumQuantity;

  DateTime? _effectiveFromUtc;
  DateTime? get effectiveFromUtc => _$this._effectiveFromUtc;
  set effectiveFromUtc(DateTime? effectiveFromUtc) =>
      _$this._effectiveFromUtc = effectiveFromUtc;

  DateTime? _effectiveToUtc;
  DateTime? get effectiveToUtc => _$this._effectiveToUtc;
  set effectiveToUtc(DateTime? effectiveToUtc) =>
      _$this._effectiveToUtc = effectiveToUtc;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  int? _version;
  int? get version => _$this._version;
  set version(int? version) => _$this._version = version;

  String? _costSourceId;
  String? get costSourceId => _$this._costSourceId;
  set costSourceId(String? costSourceId) => _$this._costSourceId = costSourceId;

  LocalizedTextDtoBuilder? _costSourceName;
  LocalizedTextDtoBuilder get costSourceName =>
      _$this._costSourceName ??= LocalizedTextDtoBuilder();
  set costSourceName(LocalizedTextDtoBuilder? costSourceName) =>
      _$this._costSourceName = costSourceName;

  String? _sourceReference;
  String? get sourceReference => _$this._sourceReference;
  set sourceReference(String? sourceReference) =>
      _$this._sourceReference = sourceReference;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  String? _evidenceFileId;
  String? get evidenceFileId => _$this._evidenceFileId;
  set evidenceFileId(String? evidenceFileId) =>
      _$this._evidenceFileId = evidenceFileId;

  String? _createdByUserId;
  String? get createdByUserId => _$this._createdByUserId;
  set createdByUserId(String? createdByUserId) =>
      _$this._createdByUserId = createdByUserId;

  String? _lastFinancialEditorId;
  String? get lastFinancialEditorId => _$this._lastFinancialEditorId;
  set lastFinancialEditorId(String? lastFinancialEditorId) =>
      _$this._lastFinancialEditorId = lastFinancialEditorId;

  String? _approvedByUserId;
  String? get approvedByUserId => _$this._approvedByUserId;
  set approvedByUserId(String? approvedByUserId) =>
      _$this._approvedByUserId = approvedByUserId;

  String? _publishedByUserId;
  String? get publishedByUserId => _$this._publishedByUserId;
  set publishedByUserId(String? publishedByUserId) =>
      _$this._publishedByUserId = publishedByUserId;

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

  CostRecordResponseBuilder() {
    CostRecordResponse._defaults(this);
  }

  CostRecordResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _itemId = $v.itemId;
      _scope = $v.scope;
      _branchId = $v.branchId;
      _unitId = $v.unitId;
      _unitName = $v.unitName?.toBuilder();
      _currency = $v.currency;
      _amount = $v.amount;
      _minimumQuantity = $v.minimumQuantity;
      _maximumQuantity = $v.maximumQuantity;
      _effectiveFromUtc = $v.effectiveFromUtc;
      _effectiveToUtc = $v.effectiveToUtc;
      _status = $v.status;
      _version = $v.version;
      _costSourceId = $v.costSourceId;
      _costSourceName = $v.costSourceName?.toBuilder();
      _sourceReference = $v.sourceReference;
      _reason = $v.reason;
      _evidenceFileId = $v.evidenceFileId;
      _createdByUserId = $v.createdByUserId;
      _lastFinancialEditorId = $v.lastFinancialEditorId;
      _approvedByUserId = $v.approvedByUserId;
      _publishedByUserId = $v.publishedByUserId;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CostRecordResponse other) {
    _$v = other as _$CostRecordResponse;
  }

  @override
  void update(void Function(CostRecordResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CostRecordResponse build() => _build();

  _$CostRecordResponse _build() {
    _$CostRecordResponse _$result;
    try {
      _$result = _$v ??
          _$CostRecordResponse._(
            id: id,
            organizationId: organizationId,
            itemId: itemId,
            scope: scope,
            branchId: branchId,
            unitId: unitId,
            unitName: _unitName?.build(),
            currency: currency,
            amount: amount,
            minimumQuantity: minimumQuantity,
            maximumQuantity: maximumQuantity,
            effectiveFromUtc: effectiveFromUtc,
            effectiveToUtc: effectiveToUtc,
            status: status,
            version: version,
            costSourceId: costSourceId,
            costSourceName: _costSourceName?.build(),
            sourceReference: sourceReference,
            reason: reason,
            evidenceFileId: evidenceFileId,
            createdByUserId: createdByUserId,
            lastFinancialEditorId: lastFinancialEditorId,
            approvedByUserId: approvedByUserId,
            publishedByUserId: publishedByUserId,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            updatedAtUtc: updatedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'unitName';
        _unitName?.build();

        _$failedField = 'costSourceName';
        _costSourceName?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CostRecordResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
