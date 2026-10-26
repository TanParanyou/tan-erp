// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'mrp_snapshot_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$MrpSnapshotSummaryResponse extends MrpSnapshotSummaryResponse {
  @override
  final int? demandCount;
  @override
  final int? supplyCount;
  @override
  final int? bomCount;
  @override
  final int? stockItemCount;

  factory _$MrpSnapshotSummaryResponse(
          [void Function(MrpSnapshotSummaryResponseBuilder)? updates]) =>
      (MrpSnapshotSummaryResponseBuilder()..update(updates))._build();

  _$MrpSnapshotSummaryResponse._(
      {this.demandCount, this.supplyCount, this.bomCount, this.stockItemCount})
      : super._();
  @override
  MrpSnapshotSummaryResponse rebuild(
          void Function(MrpSnapshotSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  MrpSnapshotSummaryResponseBuilder toBuilder() =>
      MrpSnapshotSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is MrpSnapshotSummaryResponse &&
        demandCount == other.demandCount &&
        supplyCount == other.supplyCount &&
        bomCount == other.bomCount &&
        stockItemCount == other.stockItemCount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, demandCount.hashCode);
    _$hash = $jc(_$hash, supplyCount.hashCode);
    _$hash = $jc(_$hash, bomCount.hashCode);
    _$hash = $jc(_$hash, stockItemCount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'MrpSnapshotSummaryResponse')
          ..add('demandCount', demandCount)
          ..add('supplyCount', supplyCount)
          ..add('bomCount', bomCount)
          ..add('stockItemCount', stockItemCount))
        .toString();
  }
}

class MrpSnapshotSummaryResponseBuilder
    implements
        Builder<MrpSnapshotSummaryResponse, MrpSnapshotSummaryResponseBuilder> {
  _$MrpSnapshotSummaryResponse? _$v;

  int? _demandCount;
  int? get demandCount => _$this._demandCount;
  set demandCount(int? demandCount) => _$this._demandCount = demandCount;

  int? _supplyCount;
  int? get supplyCount => _$this._supplyCount;
  set supplyCount(int? supplyCount) => _$this._supplyCount = supplyCount;

  int? _bomCount;
  int? get bomCount => _$this._bomCount;
  set bomCount(int? bomCount) => _$this._bomCount = bomCount;

  int? _stockItemCount;
  int? get stockItemCount => _$this._stockItemCount;
  set stockItemCount(int? stockItemCount) =>
      _$this._stockItemCount = stockItemCount;

  MrpSnapshotSummaryResponseBuilder() {
    MrpSnapshotSummaryResponse._defaults(this);
  }

  MrpSnapshotSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _demandCount = $v.demandCount;
      _supplyCount = $v.supplyCount;
      _bomCount = $v.bomCount;
      _stockItemCount = $v.stockItemCount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(MrpSnapshotSummaryResponse other) {
    _$v = other as _$MrpSnapshotSummaryResponse;
  }

  @override
  void update(void Function(MrpSnapshotSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  MrpSnapshotSummaryResponse build() => _build();

  _$MrpSnapshotSummaryResponse _build() {
    final _$result = _$v ??
        _$MrpSnapshotSummaryResponse._(
          demandCount: demandCount,
          supplyCount: supplyCount,
          bomCount: bomCount,
          stockItemCount: stockItemCount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
