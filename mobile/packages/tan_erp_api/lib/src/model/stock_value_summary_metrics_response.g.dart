// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'stock_value_summary_metrics_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$StockValueSummaryMetricsResponse
    extends StockValueSummaryMetricsResponse {
  @override
  final int? warehouseCount;
  @override
  final int? skuCount;
  @override
  final double? totalOnHand;
  @override
  final double? totalReserved;
  @override
  final double? totalValue;

  factory _$StockValueSummaryMetricsResponse(
          [void Function(StockValueSummaryMetricsResponseBuilder)? updates]) =>
      (StockValueSummaryMetricsResponseBuilder()..update(updates))._build();

  _$StockValueSummaryMetricsResponse._(
      {this.warehouseCount,
      this.skuCount,
      this.totalOnHand,
      this.totalReserved,
      this.totalValue})
      : super._();
  @override
  StockValueSummaryMetricsResponse rebuild(
          void Function(StockValueSummaryMetricsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  StockValueSummaryMetricsResponseBuilder toBuilder() =>
      StockValueSummaryMetricsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is StockValueSummaryMetricsResponse &&
        warehouseCount == other.warehouseCount &&
        skuCount == other.skuCount &&
        totalOnHand == other.totalOnHand &&
        totalReserved == other.totalReserved &&
        totalValue == other.totalValue;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, warehouseCount.hashCode);
    _$hash = $jc(_$hash, skuCount.hashCode);
    _$hash = $jc(_$hash, totalOnHand.hashCode);
    _$hash = $jc(_$hash, totalReserved.hashCode);
    _$hash = $jc(_$hash, totalValue.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'StockValueSummaryMetricsResponse')
          ..add('warehouseCount', warehouseCount)
          ..add('skuCount', skuCount)
          ..add('totalOnHand', totalOnHand)
          ..add('totalReserved', totalReserved)
          ..add('totalValue', totalValue))
        .toString();
  }
}

class StockValueSummaryMetricsResponseBuilder
    implements
        Builder<StockValueSummaryMetricsResponse,
            StockValueSummaryMetricsResponseBuilder> {
  _$StockValueSummaryMetricsResponse? _$v;

  int? _warehouseCount;
  int? get warehouseCount => _$this._warehouseCount;
  set warehouseCount(int? warehouseCount) =>
      _$this._warehouseCount = warehouseCount;

  int? _skuCount;
  int? get skuCount => _$this._skuCount;
  set skuCount(int? skuCount) => _$this._skuCount = skuCount;

  double? _totalOnHand;
  double? get totalOnHand => _$this._totalOnHand;
  set totalOnHand(double? totalOnHand) => _$this._totalOnHand = totalOnHand;

  double? _totalReserved;
  double? get totalReserved => _$this._totalReserved;
  set totalReserved(double? totalReserved) =>
      _$this._totalReserved = totalReserved;

  double? _totalValue;
  double? get totalValue => _$this._totalValue;
  set totalValue(double? totalValue) => _$this._totalValue = totalValue;

  StockValueSummaryMetricsResponseBuilder() {
    StockValueSummaryMetricsResponse._defaults(this);
  }

  StockValueSummaryMetricsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _warehouseCount = $v.warehouseCount;
      _skuCount = $v.skuCount;
      _totalOnHand = $v.totalOnHand;
      _totalReserved = $v.totalReserved;
      _totalValue = $v.totalValue;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(StockValueSummaryMetricsResponse other) {
    _$v = other as _$StockValueSummaryMetricsResponse;
  }

  @override
  void update(void Function(StockValueSummaryMetricsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  StockValueSummaryMetricsResponse build() => _build();

  _$StockValueSummaryMetricsResponse _build() {
    final _$result = _$v ??
        _$StockValueSummaryMetricsResponse._(
          warehouseCount: warehouseCount,
          skuCount: skuCount,
          totalOnHand: totalOnHand,
          totalReserved: totalReserved,
          totalValue: totalValue,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
