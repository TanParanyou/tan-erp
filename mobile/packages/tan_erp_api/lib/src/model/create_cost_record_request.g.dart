// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_cost_record_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateCostRecordRequest extends CreateCostRecordRequest {
  @override
  final String scope;
  @override
  final String? branchId;
  @override
  final String unitId;
  @override
  final String currency;
  @override
  final double amount;
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

  factory _$CreateCostRecordRequest(
          [void Function(CreateCostRecordRequestBuilder)? updates]) =>
      (CreateCostRecordRequestBuilder()..update(updates))._build();

  _$CreateCostRecordRequest._(
      {required this.scope,
      this.branchId,
      required this.unitId,
      required this.currency,
      required this.amount,
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
  CreateCostRecordRequest rebuild(
          void Function(CreateCostRecordRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateCostRecordRequestBuilder toBuilder() =>
      CreateCostRecordRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateCostRecordRequest &&
        scope == other.scope &&
        branchId == other.branchId &&
        unitId == other.unitId &&
        currency == other.currency &&
        amount == other.amount &&
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
    _$hash = $jc(_$hash, scope.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, unitId.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
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
    return (newBuiltValueToStringHelper(r'CreateCostRecordRequest')
          ..add('scope', scope)
          ..add('branchId', branchId)
          ..add('unitId', unitId)
          ..add('currency', currency)
          ..add('amount', amount)
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

class CreateCostRecordRequestBuilder
    implements
        Builder<CreateCostRecordRequest, CreateCostRecordRequestBuilder> {
  _$CreateCostRecordRequest? _$v;

  String? _scope;
  String? get scope => _$this._scope;
  set scope(String? scope) => _$this._scope = scope;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _unitId;
  String? get unitId => _$this._unitId;
  set unitId(String? unitId) => _$this._unitId = unitId;

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

  CreateCostRecordRequestBuilder() {
    CreateCostRecordRequest._defaults(this);
  }

  CreateCostRecordRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _scope = $v.scope;
      _branchId = $v.branchId;
      _unitId = $v.unitId;
      _currency = $v.currency;
      _amount = $v.amount;
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
  void replace(CreateCostRecordRequest other) {
    _$v = other as _$CreateCostRecordRequest;
  }

  @override
  void update(void Function(CreateCostRecordRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateCostRecordRequest build() => _build();

  _$CreateCostRecordRequest _build() {
    final _$result = _$v ??
        _$CreateCostRecordRequest._(
          scope: BuiltValueNullFieldError.checkNotNull(
              scope, r'CreateCostRecordRequest', 'scope'),
          branchId: branchId,
          unitId: BuiltValueNullFieldError.checkNotNull(
              unitId, r'CreateCostRecordRequest', 'unitId'),
          currency: BuiltValueNullFieldError.checkNotNull(
              currency, r'CreateCostRecordRequest', 'currency'),
          amount: BuiltValueNullFieldError.checkNotNull(
              amount, r'CreateCostRecordRequest', 'amount'),
          minimumQuantity: minimumQuantity,
          maximumQuantity: maximumQuantity,
          effectiveFromUtc: BuiltValueNullFieldError.checkNotNull(
              effectiveFromUtc, r'CreateCostRecordRequest', 'effectiveFromUtc'),
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
