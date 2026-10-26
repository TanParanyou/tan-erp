// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'catalog_resolved_cost_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CatalogResolvedCostResponse extends CatalogResolvedCostResponse {
  @override
  final String? costRecordId;
  @override
  final int? version;
  @override
  final double? amount;
  @override
  final String? currency;
  @override
  final String? unitCode;
  @override
  final String? scope;
  @override
  final DateTime? effectiveFromUtc;
  @override
  final String? policyVersion;
  @override
  final String? costSourceId;
  @override
  final String? costSourceCode;
  @override
  final String? sourceReference;
  @override
  final String? evidenceFileId;

  factory _$CatalogResolvedCostResponse(
          [void Function(CatalogResolvedCostResponseBuilder)? updates]) =>
      (CatalogResolvedCostResponseBuilder()..update(updates))._build();

  _$CatalogResolvedCostResponse._(
      {this.costRecordId,
      this.version,
      this.amount,
      this.currency,
      this.unitCode,
      this.scope,
      this.effectiveFromUtc,
      this.policyVersion,
      this.costSourceId,
      this.costSourceCode,
      this.sourceReference,
      this.evidenceFileId})
      : super._();
  @override
  CatalogResolvedCostResponse rebuild(
          void Function(CatalogResolvedCostResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CatalogResolvedCostResponseBuilder toBuilder() =>
      CatalogResolvedCostResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CatalogResolvedCostResponse &&
        costRecordId == other.costRecordId &&
        version == other.version &&
        amount == other.amount &&
        currency == other.currency &&
        unitCode == other.unitCode &&
        scope == other.scope &&
        effectiveFromUtc == other.effectiveFromUtc &&
        policyVersion == other.policyVersion &&
        costSourceId == other.costSourceId &&
        costSourceCode == other.costSourceCode &&
        sourceReference == other.sourceReference &&
        evidenceFileId == other.evidenceFileId;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, costRecordId.hashCode);
    _$hash = $jc(_$hash, version.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, unitCode.hashCode);
    _$hash = $jc(_$hash, scope.hashCode);
    _$hash = $jc(_$hash, effectiveFromUtc.hashCode);
    _$hash = $jc(_$hash, policyVersion.hashCode);
    _$hash = $jc(_$hash, costSourceId.hashCode);
    _$hash = $jc(_$hash, costSourceCode.hashCode);
    _$hash = $jc(_$hash, sourceReference.hashCode);
    _$hash = $jc(_$hash, evidenceFileId.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CatalogResolvedCostResponse')
          ..add('costRecordId', costRecordId)
          ..add('version', version)
          ..add('amount', amount)
          ..add('currency', currency)
          ..add('unitCode', unitCode)
          ..add('scope', scope)
          ..add('effectiveFromUtc', effectiveFromUtc)
          ..add('policyVersion', policyVersion)
          ..add('costSourceId', costSourceId)
          ..add('costSourceCode', costSourceCode)
          ..add('sourceReference', sourceReference)
          ..add('evidenceFileId', evidenceFileId))
        .toString();
  }
}

class CatalogResolvedCostResponseBuilder
    implements
        Builder<CatalogResolvedCostResponse,
            CatalogResolvedCostResponseBuilder> {
  _$CatalogResolvedCostResponse? _$v;

  String? _costRecordId;
  String? get costRecordId => _$this._costRecordId;
  set costRecordId(String? costRecordId) => _$this._costRecordId = costRecordId;

  int? _version;
  int? get version => _$this._version;
  set version(int? version) => _$this._version = version;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  String? _unitCode;
  String? get unitCode => _$this._unitCode;
  set unitCode(String? unitCode) => _$this._unitCode = unitCode;

  String? _scope;
  String? get scope => _$this._scope;
  set scope(String? scope) => _$this._scope = scope;

  DateTime? _effectiveFromUtc;
  DateTime? get effectiveFromUtc => _$this._effectiveFromUtc;
  set effectiveFromUtc(DateTime? effectiveFromUtc) =>
      _$this._effectiveFromUtc = effectiveFromUtc;

  String? _policyVersion;
  String? get policyVersion => _$this._policyVersion;
  set policyVersion(String? policyVersion) =>
      _$this._policyVersion = policyVersion;

  String? _costSourceId;
  String? get costSourceId => _$this._costSourceId;
  set costSourceId(String? costSourceId) => _$this._costSourceId = costSourceId;

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

  CatalogResolvedCostResponseBuilder() {
    CatalogResolvedCostResponse._defaults(this);
  }

  CatalogResolvedCostResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _costRecordId = $v.costRecordId;
      _version = $v.version;
      _amount = $v.amount;
      _currency = $v.currency;
      _unitCode = $v.unitCode;
      _scope = $v.scope;
      _effectiveFromUtc = $v.effectiveFromUtc;
      _policyVersion = $v.policyVersion;
      _costSourceId = $v.costSourceId;
      _costSourceCode = $v.costSourceCode;
      _sourceReference = $v.sourceReference;
      _evidenceFileId = $v.evidenceFileId;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CatalogResolvedCostResponse other) {
    _$v = other as _$CatalogResolvedCostResponse;
  }

  @override
  void update(void Function(CatalogResolvedCostResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CatalogResolvedCostResponse build() => _build();

  _$CatalogResolvedCostResponse _build() {
    final _$result = _$v ??
        _$CatalogResolvedCostResponse._(
          costRecordId: costRecordId,
          version: version,
          amount: amount,
          currency: currency,
          unitCode: unitCode,
          scope: scope,
          effectiveFromUtc: effectiveFromUtc,
          policyVersion: policyVersion,
          costSourceId: costSourceId,
          costSourceCode: costSourceCode,
          sourceReference: sourceReference,
          evidenceFileId: evidenceFileId,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
