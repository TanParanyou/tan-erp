// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'opportunity_metrics_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OpportunityMetricsResponse extends OpportunityMetricsResponse {
  @override
  final double? totalPipelineValue;
  @override
  final int? activeDealsCount;
  @override
  final int? closingThisMonthCount;
  @override
  final double? wonValue;
  @override
  final int? wonDealsCount;
  @override
  final double? winRatePercentage;

  factory _$OpportunityMetricsResponse(
          [void Function(OpportunityMetricsResponseBuilder)? updates]) =>
      (OpportunityMetricsResponseBuilder()..update(updates))._build();

  _$OpportunityMetricsResponse._(
      {this.totalPipelineValue,
      this.activeDealsCount,
      this.closingThisMonthCount,
      this.wonValue,
      this.wonDealsCount,
      this.winRatePercentage})
      : super._();
  @override
  OpportunityMetricsResponse rebuild(
          void Function(OpportunityMetricsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OpportunityMetricsResponseBuilder toBuilder() =>
      OpportunityMetricsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OpportunityMetricsResponse &&
        totalPipelineValue == other.totalPipelineValue &&
        activeDealsCount == other.activeDealsCount &&
        closingThisMonthCount == other.closingThisMonthCount &&
        wonValue == other.wonValue &&
        wonDealsCount == other.wonDealsCount &&
        winRatePercentage == other.winRatePercentage;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, totalPipelineValue.hashCode);
    _$hash = $jc(_$hash, activeDealsCount.hashCode);
    _$hash = $jc(_$hash, closingThisMonthCount.hashCode);
    _$hash = $jc(_$hash, wonValue.hashCode);
    _$hash = $jc(_$hash, wonDealsCount.hashCode);
    _$hash = $jc(_$hash, winRatePercentage.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'OpportunityMetricsResponse')
          ..add('totalPipelineValue', totalPipelineValue)
          ..add('activeDealsCount', activeDealsCount)
          ..add('closingThisMonthCount', closingThisMonthCount)
          ..add('wonValue', wonValue)
          ..add('wonDealsCount', wonDealsCount)
          ..add('winRatePercentage', winRatePercentage))
        .toString();
  }
}

class OpportunityMetricsResponseBuilder
    implements
        Builder<OpportunityMetricsResponse, OpportunityMetricsResponseBuilder> {
  _$OpportunityMetricsResponse? _$v;

  double? _totalPipelineValue;
  double? get totalPipelineValue => _$this._totalPipelineValue;
  set totalPipelineValue(double? totalPipelineValue) =>
      _$this._totalPipelineValue = totalPipelineValue;

  int? _activeDealsCount;
  int? get activeDealsCount => _$this._activeDealsCount;
  set activeDealsCount(int? activeDealsCount) =>
      _$this._activeDealsCount = activeDealsCount;

  int? _closingThisMonthCount;
  int? get closingThisMonthCount => _$this._closingThisMonthCount;
  set closingThisMonthCount(int? closingThisMonthCount) =>
      _$this._closingThisMonthCount = closingThisMonthCount;

  double? _wonValue;
  double? get wonValue => _$this._wonValue;
  set wonValue(double? wonValue) => _$this._wonValue = wonValue;

  int? _wonDealsCount;
  int? get wonDealsCount => _$this._wonDealsCount;
  set wonDealsCount(int? wonDealsCount) =>
      _$this._wonDealsCount = wonDealsCount;

  double? _winRatePercentage;
  double? get winRatePercentage => _$this._winRatePercentage;
  set winRatePercentage(double? winRatePercentage) =>
      _$this._winRatePercentage = winRatePercentage;

  OpportunityMetricsResponseBuilder() {
    OpportunityMetricsResponse._defaults(this);
  }

  OpportunityMetricsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _totalPipelineValue = $v.totalPipelineValue;
      _activeDealsCount = $v.activeDealsCount;
      _closingThisMonthCount = $v.closingThisMonthCount;
      _wonValue = $v.wonValue;
      _wonDealsCount = $v.wonDealsCount;
      _winRatePercentage = $v.winRatePercentage;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(OpportunityMetricsResponse other) {
    _$v = other as _$OpportunityMetricsResponse;
  }

  @override
  void update(void Function(OpportunityMetricsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OpportunityMetricsResponse build() => _build();

  _$OpportunityMetricsResponse _build() {
    final _$result = _$v ??
        _$OpportunityMetricsResponse._(
          totalPipelineValue: totalPipelineValue,
          activeDealsCount: activeDealsCount,
          closingThisMonthCount: closingThisMonthCount,
          wonValue: wonValue,
          wonDealsCount: wonDealsCount,
          winRatePercentage: winRatePercentage,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
