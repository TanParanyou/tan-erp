// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_calculation_snapshot_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateCalculationSnapshotResponse
    extends EstimateCalculationSnapshotResponse {
  @override
  final String? id;
  @override
  final String? estimateRevisionId;
  @override
  final int? calculationVersion;
  @override
  final String? inputHash;
  @override
  final String? snapshotJson;
  @override
  final String? calculationPolicyVersionId;
  @override
  final String? taxPolicyVersionId;
  @override
  final String? calculationPolicyVersion;
  @override
  final String? taxPolicyVersion;
  @override
  final String? calculationPolicyHash;
  @override
  final String? taxPolicyHash;
  @override
  final String? capturedByUserId;
  @override
  final DateTime? capturedAtUtc;

  factory _$EstimateCalculationSnapshotResponse(
          [void Function(EstimateCalculationSnapshotResponseBuilder)?
              updates]) =>
      (EstimateCalculationSnapshotResponseBuilder()..update(updates))._build();

  _$EstimateCalculationSnapshotResponse._(
      {this.id,
      this.estimateRevisionId,
      this.calculationVersion,
      this.inputHash,
      this.snapshotJson,
      this.calculationPolicyVersionId,
      this.taxPolicyVersionId,
      this.calculationPolicyVersion,
      this.taxPolicyVersion,
      this.calculationPolicyHash,
      this.taxPolicyHash,
      this.capturedByUserId,
      this.capturedAtUtc})
      : super._();
  @override
  EstimateCalculationSnapshotResponse rebuild(
          void Function(EstimateCalculationSnapshotResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateCalculationSnapshotResponseBuilder toBuilder() =>
      EstimateCalculationSnapshotResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateCalculationSnapshotResponse &&
        id == other.id &&
        estimateRevisionId == other.estimateRevisionId &&
        calculationVersion == other.calculationVersion &&
        inputHash == other.inputHash &&
        snapshotJson == other.snapshotJson &&
        calculationPolicyVersionId == other.calculationPolicyVersionId &&
        taxPolicyVersionId == other.taxPolicyVersionId &&
        calculationPolicyVersion == other.calculationPolicyVersion &&
        taxPolicyVersion == other.taxPolicyVersion &&
        calculationPolicyHash == other.calculationPolicyHash &&
        taxPolicyHash == other.taxPolicyHash &&
        capturedByUserId == other.capturedByUserId &&
        capturedAtUtc == other.capturedAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, estimateRevisionId.hashCode);
    _$hash = $jc(_$hash, calculationVersion.hashCode);
    _$hash = $jc(_$hash, inputHash.hashCode);
    _$hash = $jc(_$hash, snapshotJson.hashCode);
    _$hash = $jc(_$hash, calculationPolicyVersionId.hashCode);
    _$hash = $jc(_$hash, taxPolicyVersionId.hashCode);
    _$hash = $jc(_$hash, calculationPolicyVersion.hashCode);
    _$hash = $jc(_$hash, taxPolicyVersion.hashCode);
    _$hash = $jc(_$hash, calculationPolicyHash.hashCode);
    _$hash = $jc(_$hash, taxPolicyHash.hashCode);
    _$hash = $jc(_$hash, capturedByUserId.hashCode);
    _$hash = $jc(_$hash, capturedAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateCalculationSnapshotResponse')
          ..add('id', id)
          ..add('estimateRevisionId', estimateRevisionId)
          ..add('calculationVersion', calculationVersion)
          ..add('inputHash', inputHash)
          ..add('snapshotJson', snapshotJson)
          ..add('calculationPolicyVersionId', calculationPolicyVersionId)
          ..add('taxPolicyVersionId', taxPolicyVersionId)
          ..add('calculationPolicyVersion', calculationPolicyVersion)
          ..add('taxPolicyVersion', taxPolicyVersion)
          ..add('calculationPolicyHash', calculationPolicyHash)
          ..add('taxPolicyHash', taxPolicyHash)
          ..add('capturedByUserId', capturedByUserId)
          ..add('capturedAtUtc', capturedAtUtc))
        .toString();
  }
}

class EstimateCalculationSnapshotResponseBuilder
    implements
        Builder<EstimateCalculationSnapshotResponse,
            EstimateCalculationSnapshotResponseBuilder> {
  _$EstimateCalculationSnapshotResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _estimateRevisionId;
  String? get estimateRevisionId => _$this._estimateRevisionId;
  set estimateRevisionId(String? estimateRevisionId) =>
      _$this._estimateRevisionId = estimateRevisionId;

  int? _calculationVersion;
  int? get calculationVersion => _$this._calculationVersion;
  set calculationVersion(int? calculationVersion) =>
      _$this._calculationVersion = calculationVersion;

  String? _inputHash;
  String? get inputHash => _$this._inputHash;
  set inputHash(String? inputHash) => _$this._inputHash = inputHash;

  String? _snapshotJson;
  String? get snapshotJson => _$this._snapshotJson;
  set snapshotJson(String? snapshotJson) => _$this._snapshotJson = snapshotJson;

  String? _calculationPolicyVersionId;
  String? get calculationPolicyVersionId => _$this._calculationPolicyVersionId;
  set calculationPolicyVersionId(String? calculationPolicyVersionId) =>
      _$this._calculationPolicyVersionId = calculationPolicyVersionId;

  String? _taxPolicyVersionId;
  String? get taxPolicyVersionId => _$this._taxPolicyVersionId;
  set taxPolicyVersionId(String? taxPolicyVersionId) =>
      _$this._taxPolicyVersionId = taxPolicyVersionId;

  String? _calculationPolicyVersion;
  String? get calculationPolicyVersion => _$this._calculationPolicyVersion;
  set calculationPolicyVersion(String? calculationPolicyVersion) =>
      _$this._calculationPolicyVersion = calculationPolicyVersion;

  String? _taxPolicyVersion;
  String? get taxPolicyVersion => _$this._taxPolicyVersion;
  set taxPolicyVersion(String? taxPolicyVersion) =>
      _$this._taxPolicyVersion = taxPolicyVersion;

  String? _calculationPolicyHash;
  String? get calculationPolicyHash => _$this._calculationPolicyHash;
  set calculationPolicyHash(String? calculationPolicyHash) =>
      _$this._calculationPolicyHash = calculationPolicyHash;

  String? _taxPolicyHash;
  String? get taxPolicyHash => _$this._taxPolicyHash;
  set taxPolicyHash(String? taxPolicyHash) =>
      _$this._taxPolicyHash = taxPolicyHash;

  String? _capturedByUserId;
  String? get capturedByUserId => _$this._capturedByUserId;
  set capturedByUserId(String? capturedByUserId) =>
      _$this._capturedByUserId = capturedByUserId;

  DateTime? _capturedAtUtc;
  DateTime? get capturedAtUtc => _$this._capturedAtUtc;
  set capturedAtUtc(DateTime? capturedAtUtc) =>
      _$this._capturedAtUtc = capturedAtUtc;

  EstimateCalculationSnapshotResponseBuilder() {
    EstimateCalculationSnapshotResponse._defaults(this);
  }

  EstimateCalculationSnapshotResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _estimateRevisionId = $v.estimateRevisionId;
      _calculationVersion = $v.calculationVersion;
      _inputHash = $v.inputHash;
      _snapshotJson = $v.snapshotJson;
      _calculationPolicyVersionId = $v.calculationPolicyVersionId;
      _taxPolicyVersionId = $v.taxPolicyVersionId;
      _calculationPolicyVersion = $v.calculationPolicyVersion;
      _taxPolicyVersion = $v.taxPolicyVersion;
      _calculationPolicyHash = $v.calculationPolicyHash;
      _taxPolicyHash = $v.taxPolicyHash;
      _capturedByUserId = $v.capturedByUserId;
      _capturedAtUtc = $v.capturedAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateCalculationSnapshotResponse other) {
    _$v = other as _$EstimateCalculationSnapshotResponse;
  }

  @override
  void update(
      void Function(EstimateCalculationSnapshotResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateCalculationSnapshotResponse build() => _build();

  _$EstimateCalculationSnapshotResponse _build() {
    final _$result = _$v ??
        _$EstimateCalculationSnapshotResponse._(
          id: id,
          estimateRevisionId: estimateRevisionId,
          calculationVersion: calculationVersion,
          inputHash: inputHash,
          snapshotJson: snapshotJson,
          calculationPolicyVersionId: calculationPolicyVersionId,
          taxPolicyVersionId: taxPolicyVersionId,
          calculationPolicyVersion: calculationPolicyVersion,
          taxPolicyVersion: taxPolicyVersion,
          calculationPolicyHash: calculationPolicyHash,
          taxPolicyHash: taxPolicyHash,
          capturedByUserId: capturedByUserId,
          capturedAtUtc: capturedAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
