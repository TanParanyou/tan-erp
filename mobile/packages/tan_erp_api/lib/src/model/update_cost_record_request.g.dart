// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_cost_record_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateCostRecordRequest extends UpdateCostRecordRequest {
  @override
  final double amount;
  @override
  final String currency;
  @override
  final String unitId;
  @override
  final double? minimumQuantity;
  @override
  final double? maximumQuantity;
  @override
  final DateTime effectiveFromUtc;
  @override
  final DateTime? effectiveToUtc;
  @override
  final String? costSourceId;
  @override
  final String? sourceReference;
  @override
  final String? reason;
  @override
  final String? evidenceFileId;

  factory _$UpdateCostRecordRequest(
          [void Function(UpdateCostRecordRequestBuilder)? updates]) =>
      (UpdateCostRecordRequestBuilder()..update(updates))._build();

  _$UpdateCostRecordRequest._(
      {required this.amount,
      required this.currency,
      required this.unitId,
      this.minimumQuantity,
      this.maximumQuantity,
      required this.effectiveFromUtc,
      this.effectiveToUtc,
      this.costSourceId,
      this.sourceReference,
      this.reason,
      this.evidenceFileId})
      : super._();
  @override
  UpdateCostRecordRequest rebuild(
          void Function(UpdateCostRecordRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateCostRecordRequestBuilder toBuilder() =>
      UpdateCostRecordRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateCostRecordRequest &&
        amount == other.amount &&
        currency == other.currency &&
        unitId == other.unitId &&
        minimumQuantity == other.minimumQuantity &&
        maximumQuantity == other.maximumQuantity &&
        effectiveFromUtc == other.effectiveFromUtc &&
        effectiveToUtc == other.effectiveToUtc &&
        costSourceId == other.costSourceId &&
        sourceReference == other.sourceReference &&
        reason == other.reason &&
        evidenceFileId == other.evidenceFileId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, unitId.hashCode);
    _$hash = $jc(_$hash, minimumQuantity.hashCode);
    _$hash = $jc(_$hash, maximumQuantity.hashCode);
    _$hash = $jc(_$hash, effectiveFromUtc.hashCode);
    _$hash = $jc(_$hash, effectiveToUtc.hashCode);
    _$hash = $jc(_$hash, costSourceId.hashCode);
    _$hash = $jc(_$hash, sourceReference.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, evidenceFileId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateCostRecordRequest')
          ..add('amount', amount)
          ..add('currency', currency)
          ..add('unitId', unitId)
          ..add('minimumQuantity', minimumQuantity)
          ..add('maximumQuantity', maximumQuantity)
          ..add('effectiveFromUtc', effectiveFromUtc)
          ..add('effectiveToUtc', effectiveToUtc)
          ..add('costSourceId', costSourceId)
          ..add('sourceReference', sourceReference)
          ..add('reason', reason)
          ..add('evidenceFileId', evidenceFileId))
        .toString();
  }
}

class UpdateCostRecordRequestBuilder
    implements
        Builder<UpdateCostRecordRequest, UpdateCostRecordRequestBuilder> {
  _$UpdateCostRecordRequest? _$v;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  String? _unitId;
  String? get unitId => _$this._unitId;
  set unitId(String? unitId) => _$this._unitId = unitId;

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

  String? _costSourceId;
  String? get costSourceId => _$this._costSourceId;
  set costSourceId(String? costSourceId) => _$this._costSourceId = costSourceId;

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

  UpdateCostRecordRequestBuilder() {
    UpdateCostRecordRequest._defaults(this);
  }

  UpdateCostRecordRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _amount = $v.amount;
      _currency = $v.currency;
      _unitId = $v.unitId;
      _minimumQuantity = $v.minimumQuantity;
      _maximumQuantity = $v.maximumQuantity;
      _effectiveFromUtc = $v.effectiveFromUtc;
      _effectiveToUtc = $v.effectiveToUtc;
      _costSourceId = $v.costSourceId;
      _sourceReference = $v.sourceReference;
      _reason = $v.reason;
      _evidenceFileId = $v.evidenceFileId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateCostRecordRequest other) {
    _$v = other as _$UpdateCostRecordRequest;
  }

  @override
  void update(void Function(UpdateCostRecordRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateCostRecordRequest build() => _build();

  _$UpdateCostRecordRequest _build() {
    final _$result = _$v ??
        _$UpdateCostRecordRequest._(
          amount: BuiltValueNullFieldError.checkNotNull(
              amount, r'UpdateCostRecordRequest', 'amount'),
          currency: BuiltValueNullFieldError.checkNotNull(
              currency, r'UpdateCostRecordRequest', 'currency'),
          unitId: BuiltValueNullFieldError.checkNotNull(
              unitId, r'UpdateCostRecordRequest', 'unitId'),
          minimumQuantity: minimumQuantity,
          maximumQuantity: maximumQuantity,
          effectiveFromUtc: BuiltValueNullFieldError.checkNotNull(
              effectiveFromUtc, r'UpdateCostRecordRequest', 'effectiveFromUtc'),
          effectiveToUtc: effectiveToUtc,
          costSourceId: costSourceId,
          sourceReference: sourceReference,
          reason: reason,
          evidenceFileId: evidenceFileId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
