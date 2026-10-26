// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cost_review_queue_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CostReviewQueueItemResponse extends CostReviewQueueItemResponse {
  @override
  final String? id;
  @override
  final String? itemId;
  @override
  final CostReviewReferenceResponse? item;
  @override
  final String? scope;
  @override
  final String? unitId;
  @override
  final CostReviewReferenceResponse? unit;
  @override
  final String? costSourceId;
  @override
  final CostReviewReferenceResponse? costSource;
  @override
  final double? amount;
  @override
  final String? currency;
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
  final String? sourceReference;
  @override
  final String? reason;
  @override
  final String? evidenceFileId;
  @override
  final CostReviewPersonResponse? maker;
  @override
  final CostReviewPersonResponse? lastEditor;
  @override
  final String? rowVersion;
  @override
  final DateTime? updatedAtUtc;

  factory _$CostReviewQueueItemResponse(
          [void Function(CostReviewQueueItemResponseBuilder)? updates]) =>
      (CostReviewQueueItemResponseBuilder()..update(updates))._build();

  _$CostReviewQueueItemResponse._(
      {this.id,
      this.itemId,
      this.item,
      this.scope,
      this.unitId,
      this.unit,
      this.costSourceId,
      this.costSource,
      this.amount,
      this.currency,
      this.minimumQuantity,
      this.maximumQuantity,
      this.effectiveFromUtc,
      this.effectiveToUtc,
      this.status,
      this.sourceReference,
      this.reason,
      this.evidenceFileId,
      this.maker,
      this.lastEditor,
      this.rowVersion,
      this.updatedAtUtc})
      : super._();
  @override
  CostReviewQueueItemResponse rebuild(
          void Function(CostReviewQueueItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CostReviewQueueItemResponseBuilder toBuilder() =>
      CostReviewQueueItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CostReviewQueueItemResponse &&
        id == other.id &&
        itemId == other.itemId &&
        item == other.item &&
        scope == other.scope &&
        unitId == other.unitId &&
        unit == other.unit &&
        costSourceId == other.costSourceId &&
        costSource == other.costSource &&
        amount == other.amount &&
        currency == other.currency &&
        minimumQuantity == other.minimumQuantity &&
        maximumQuantity == other.maximumQuantity &&
        effectiveFromUtc == other.effectiveFromUtc &&
        effectiveToUtc == other.effectiveToUtc &&
        status == other.status &&
        sourceReference == other.sourceReference &&
        reason == other.reason &&
        evidenceFileId == other.evidenceFileId &&
        maker == other.maker &&
        lastEditor == other.lastEditor &&
        rowVersion == other.rowVersion &&
        updatedAtUtc == other.updatedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, itemId.hashCode);
    _$hash = $jc(_$hash, item.hashCode);
    _$hash = $jc(_$hash, scope.hashCode);
    _$hash = $jc(_$hash, unitId.hashCode);
    _$hash = $jc(_$hash, unit.hashCode);
    _$hash = $jc(_$hash, costSourceId.hashCode);
    _$hash = $jc(_$hash, costSource.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, minimumQuantity.hashCode);
    _$hash = $jc(_$hash, maximumQuantity.hashCode);
    _$hash = $jc(_$hash, effectiveFromUtc.hashCode);
    _$hash = $jc(_$hash, effectiveToUtc.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, sourceReference.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, evidenceFileId.hashCode);
    _$hash = $jc(_$hash, maker.hashCode);
    _$hash = $jc(_$hash, lastEditor.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CostReviewQueueItemResponse')
          ..add('id', id)
          ..add('itemId', itemId)
          ..add('item', item)
          ..add('scope', scope)
          ..add('unitId', unitId)
          ..add('unit', unit)
          ..add('costSourceId', costSourceId)
          ..add('costSource', costSource)
          ..add('amount', amount)
          ..add('currency', currency)
          ..add('minimumQuantity', minimumQuantity)
          ..add('maximumQuantity', maximumQuantity)
          ..add('effectiveFromUtc', effectiveFromUtc)
          ..add('effectiveToUtc', effectiveToUtc)
          ..add('status', status)
          ..add('sourceReference', sourceReference)
          ..add('reason', reason)
          ..add('evidenceFileId', evidenceFileId)
          ..add('maker', maker)
          ..add('lastEditor', lastEditor)
          ..add('rowVersion', rowVersion)
          ..add('updatedAtUtc', updatedAtUtc))
        .toString();
  }
}

class CostReviewQueueItemResponseBuilder
    implements
        Builder<CostReviewQueueItemResponse,
            CostReviewQueueItemResponseBuilder> {
  _$CostReviewQueueItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _itemId;
  String? get itemId => _$this._itemId;
  set itemId(String? itemId) => _$this._itemId = itemId;

  CostReviewReferenceResponseBuilder? _item;
  CostReviewReferenceResponseBuilder get item =>
      _$this._item ??= CostReviewReferenceResponseBuilder();
  set item(CostReviewReferenceResponseBuilder? item) => _$this._item = item;

  String? _scope;
  String? get scope => _$this._scope;
  set scope(String? scope) => _$this._scope = scope;

  String? _unitId;
  String? get unitId => _$this._unitId;
  set unitId(String? unitId) => _$this._unitId = unitId;

  CostReviewReferenceResponseBuilder? _unit;
  CostReviewReferenceResponseBuilder get unit =>
      _$this._unit ??= CostReviewReferenceResponseBuilder();
  set unit(CostReviewReferenceResponseBuilder? unit) => _$this._unit = unit;

  String? _costSourceId;
  String? get costSourceId => _$this._costSourceId;
  set costSourceId(String? costSourceId) => _$this._costSourceId = costSourceId;

  CostReviewReferenceResponseBuilder? _costSource;
  CostReviewReferenceResponseBuilder get costSource =>
      _$this._costSource ??= CostReviewReferenceResponseBuilder();
  set costSource(CostReviewReferenceResponseBuilder? costSource) =>
      _$this._costSource = costSource;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

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

  CostReviewPersonResponseBuilder? _maker;
  CostReviewPersonResponseBuilder get maker =>
      _$this._maker ??= CostReviewPersonResponseBuilder();
  set maker(CostReviewPersonResponseBuilder? maker) => _$this._maker = maker;

  CostReviewPersonResponseBuilder? _lastEditor;
  CostReviewPersonResponseBuilder get lastEditor =>
      _$this._lastEditor ??= CostReviewPersonResponseBuilder();
  set lastEditor(CostReviewPersonResponseBuilder? lastEditor) =>
      _$this._lastEditor = lastEditor;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  CostReviewQueueItemResponseBuilder() {
    CostReviewQueueItemResponse._defaults(this);
  }

  CostReviewQueueItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _itemId = $v.itemId;
      _item = $v.item?.toBuilder();
      _scope = $v.scope;
      _unitId = $v.unitId;
      _unit = $v.unit?.toBuilder();
      _costSourceId = $v.costSourceId;
      _costSource = $v.costSource?.toBuilder();
      _amount = $v.amount;
      _currency = $v.currency;
      _minimumQuantity = $v.minimumQuantity;
      _maximumQuantity = $v.maximumQuantity;
      _effectiveFromUtc = $v.effectiveFromUtc;
      _effectiveToUtc = $v.effectiveToUtc;
      _status = $v.status;
      _sourceReference = $v.sourceReference;
      _reason = $v.reason;
      _evidenceFileId = $v.evidenceFileId;
      _maker = $v.maker?.toBuilder();
      _lastEditor = $v.lastEditor?.toBuilder();
      _rowVersion = $v.rowVersion;
      _updatedAtUtc = $v.updatedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CostReviewQueueItemResponse other) {
    _$v = other as _$CostReviewQueueItemResponse;
  }

  @override
  void update(void Function(CostReviewQueueItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CostReviewQueueItemResponse build() => _build();

  _$CostReviewQueueItemResponse _build() {
    _$CostReviewQueueItemResponse _$result;
    try {
      _$result = _$v ??
          _$CostReviewQueueItemResponse._(
            id: id,
            itemId: itemId,
            item: _item?.build(),
            scope: scope,
            unitId: unitId,
            unit: _unit?.build(),
            costSourceId: costSourceId,
            costSource: _costSource?.build(),
            amount: amount,
            currency: currency,
            minimumQuantity: minimumQuantity,
            maximumQuantity: maximumQuantity,
            effectiveFromUtc: effectiveFromUtc,
            effectiveToUtc: effectiveToUtc,
            status: status,
            sourceReference: sourceReference,
            reason: reason,
            evidenceFileId: evidenceFileId,
            maker: _maker?.build(),
            lastEditor: _lastEditor?.build(),
            rowVersion: rowVersion,
            updatedAtUtc: updatedAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'item';
        _item?.build();

        _$failedField = 'unit';
        _unit?.build();

        _$failedField = 'costSource';
        _costSource?.build();

        _$failedField = 'maker';
        _maker?.build();
        _$failedField = 'lastEditor';
        _lastEditor?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CostReviewQueueItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
