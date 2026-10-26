// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_run_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpRunResponse extends MrpRunResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final Date? asOfDate;
  @override
  final int? purchaseLeadTimeDays;
  @override
  final int? productionLeadTimeDays;
  @override
  final String? inputHash;
  @override
  final MrpSnapshotSummaryResponse? snapshot;
  @override
  final MrpPersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final BuiltList<MrpRecommendationResponse>? recommendations;

  factory _$MrpRunResponse([void Function(MrpRunResponseBuilder)? updates]) =>
      (MrpRunResponseBuilder()..update(updates))._build();

  _$MrpRunResponse._(
      {this.id,
      this.number,
      this.asOfDate,
      this.purchaseLeadTimeDays,
      this.productionLeadTimeDays,
      this.inputHash,
      this.snapshot,
      this.createdBy,
      this.createdAtUtc,
      this.recommendations})
      : super._();
  @override
  MrpRunResponse rebuild(void Function(MrpRunResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpRunResponseBuilder toBuilder() => MrpRunResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpRunResponse &&
        id == other.id &&
        number == other.number &&
        asOfDate == other.asOfDate &&
        purchaseLeadTimeDays == other.purchaseLeadTimeDays &&
        productionLeadTimeDays == other.productionLeadTimeDays &&
        inputHash == other.inputHash &&
        snapshot == other.snapshot &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        recommendations == other.recommendations;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, asOfDate.hashCode);
    _$hash = $jc(_$hash, purchaseLeadTimeDays.hashCode);
    _$hash = $jc(_$hash, productionLeadTimeDays.hashCode);
    _$hash = $jc(_$hash, inputHash.hashCode);
    _$hash = $jc(_$hash, snapshot.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, recommendations.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpRunResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('asOfDate', asOfDate)
          ..add('purchaseLeadTimeDays', purchaseLeadTimeDays)
          ..add('productionLeadTimeDays', productionLeadTimeDays)
          ..add('inputHash', inputHash)
          ..add('snapshot', snapshot)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('recommendations', recommendations))
        .toString();
  }
}

class MrpRunResponseBuilder
    implements Builder<MrpRunResponse, MrpRunResponseBuilder> {
  _$MrpRunResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  Date? _asOfDate;
  Date? get asOfDate => _$this._asOfDate;
  set asOfDate(Date? asOfDate) => _$this._asOfDate = asOfDate;

  int? _purchaseLeadTimeDays;
  int? get purchaseLeadTimeDays => _$this._purchaseLeadTimeDays;
  set purchaseLeadTimeDays(int? purchaseLeadTimeDays) =>
      _$this._purchaseLeadTimeDays = purchaseLeadTimeDays;

  int? _productionLeadTimeDays;
  int? get productionLeadTimeDays => _$this._productionLeadTimeDays;
  set productionLeadTimeDays(int? productionLeadTimeDays) =>
      _$this._productionLeadTimeDays = productionLeadTimeDays;

  String? _inputHash;
  String? get inputHash => _$this._inputHash;
  set inputHash(String? inputHash) => _$this._inputHash = inputHash;

  MrpSnapshotSummaryResponseBuilder? _snapshot;
  MrpSnapshotSummaryResponseBuilder get snapshot =>
      _$this._snapshot ??= MrpSnapshotSummaryResponseBuilder();
  set snapshot(MrpSnapshotSummaryResponseBuilder? snapshot) =>
      _$this._snapshot = snapshot;

  MrpPersonResponseBuilder? _createdBy;
  MrpPersonResponseBuilder get createdBy =>
      _$this._createdBy ??= MrpPersonResponseBuilder();
  set createdBy(MrpPersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  ListBuilder<MrpRecommendationResponse>? _recommendations;
  ListBuilder<MrpRecommendationResponse> get recommendations =>
      _$this._recommendations ??= ListBuilder<MrpRecommendationResponse>();
  set recommendations(
          ListBuilder<MrpRecommendationResponse>? recommendations) =>
      _$this._recommendations = recommendations;

  MrpRunResponseBuilder() {
    MrpRunResponse._defaults(this);
  }

  MrpRunResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _asOfDate = $v.asOfDate;
      _purchaseLeadTimeDays = $v.purchaseLeadTimeDays;
      _productionLeadTimeDays = $v.productionLeadTimeDays;
      _inputHash = $v.inputHash;
      _snapshot = $v.snapshot?.toBuilder();
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _recommendations = $v.recommendations?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpRunResponse other) {
    _$v = other as _$MrpRunResponse;
  }

  @override
  void update(void Function(MrpRunResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpRunResponse build() => _build();

  _$MrpRunResponse _build() {
    _$MrpRunResponse _$result;
    try {
      _$result = _$v ??
          _$MrpRunResponse._(
            id: id,
            number: number,
            asOfDate: asOfDate,
            purchaseLeadTimeDays: purchaseLeadTimeDays,
            productionLeadTimeDays: productionLeadTimeDays,
            inputHash: inputHash,
            snapshot: _snapshot?.build(),
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            recommendations: _recommendations?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'snapshot';
        _snapshot?.build();
        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'recommendations';
        _recommendations?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'MrpRunResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
