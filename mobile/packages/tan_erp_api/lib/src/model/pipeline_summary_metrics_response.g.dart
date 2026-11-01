// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'pipeline_summary_metrics_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PipelineSummaryMetricsResponse extends PipelineSummaryMetricsResponse {
  @override
  final int? totalCount;
  @override
  final double? totalExpectedBudget;
  @override
  final int? openCount;
  @override
  final double? openExpectedBudget;
  @override
  final int? wonCount;
  @override
  final double? wonBudget;
  @override
  final int? lostCount;
  @override
  final double? winRatePercent;

  factory _$PipelineSummaryMetricsResponse(
          [void Function(PipelineSummaryMetricsResponseBuilder)? updates]) =>
      (PipelineSummaryMetricsResponseBuilder()..update(updates))._build();

  _$PipelineSummaryMetricsResponse._(
      {this.totalCount,
      this.totalExpectedBudget,
      this.openCount,
      this.openExpectedBudget,
      this.wonCount,
      this.wonBudget,
      this.lostCount,
      this.winRatePercent})
      : super._();
  @override
  PipelineSummaryMetricsResponse rebuild(
          void Function(PipelineSummaryMetricsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PipelineSummaryMetricsResponseBuilder toBuilder() =>
      PipelineSummaryMetricsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PipelineSummaryMetricsResponse &&
        totalCount == other.totalCount &&
        totalExpectedBudget == other.totalExpectedBudget &&
        openCount == other.openCount &&
        openExpectedBudget == other.openExpectedBudget &&
        wonCount == other.wonCount &&
        wonBudget == other.wonBudget &&
        lostCount == other.lostCount &&
        winRatePercent == other.winRatePercent;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, totalCount.hashCode);
    _$hash = $jc(_$hash, totalExpectedBudget.hashCode);
    _$hash = $jc(_$hash, openCount.hashCode);
    _$hash = $jc(_$hash, openExpectedBudget.hashCode);
    _$hash = $jc(_$hash, wonCount.hashCode);
    _$hash = $jc(_$hash, wonBudget.hashCode);
    _$hash = $jc(_$hash, lostCount.hashCode);
    _$hash = $jc(_$hash, winRatePercent.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'PipelineSummaryMetricsResponse')
          ..add('totalCount', totalCount)
          ..add('totalExpectedBudget', totalExpectedBudget)
          ..add('openCount', openCount)
          ..add('openExpectedBudget', openExpectedBudget)
          ..add('wonCount', wonCount)
          ..add('wonBudget', wonBudget)
          ..add('lostCount', lostCount)
          ..add('winRatePercent', winRatePercent))
        .toString();
  }
}

class PipelineSummaryMetricsResponseBuilder
    implements
        Builder<PipelineSummaryMetricsResponse,
            PipelineSummaryMetricsResponseBuilder> {
  _$PipelineSummaryMetricsResponse? _$v;

  int? _totalCount;
  int? get totalCount => _$this._totalCount;
  set totalCount(int? totalCount) => _$this._totalCount = totalCount;

  double? _totalExpectedBudget;
  double? get totalExpectedBudget => _$this._totalExpectedBudget;
  set totalExpectedBudget(double? totalExpectedBudget) =>
      _$this._totalExpectedBudget = totalExpectedBudget;

  int? _openCount;
  int? get openCount => _$this._openCount;
  set openCount(int? openCount) => _$this._openCount = openCount;

  double? _openExpectedBudget;
  double? get openExpectedBudget => _$this._openExpectedBudget;
  set openExpectedBudget(double? openExpectedBudget) =>
      _$this._openExpectedBudget = openExpectedBudget;

  int? _wonCount;
  int? get wonCount => _$this._wonCount;
  set wonCount(int? wonCount) => _$this._wonCount = wonCount;

  double? _wonBudget;
  double? get wonBudget => _$this._wonBudget;
  set wonBudget(double? wonBudget) => _$this._wonBudget = wonBudget;

  int? _lostCount;
  int? get lostCount => _$this._lostCount;
  set lostCount(int? lostCount) => _$this._lostCount = lostCount;

  double? _winRatePercent;
  double? get winRatePercent => _$this._winRatePercent;
  set winRatePercent(double? winRatePercent) =>
      _$this._winRatePercent = winRatePercent;

  PipelineSummaryMetricsResponseBuilder() {
    PipelineSummaryMetricsResponse._defaults(this);
  }

  PipelineSummaryMetricsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _totalCount = $v.totalCount;
      _totalExpectedBudget = $v.totalExpectedBudget;
      _openCount = $v.openCount;
      _openExpectedBudget = $v.openExpectedBudget;
      _wonCount = $v.wonCount;
      _wonBudget = $v.wonBudget;
      _lostCount = $v.lostCount;
      _winRatePercent = $v.winRatePercent;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(PipelineSummaryMetricsResponse other) {
    _$v = other as _$PipelineSummaryMetricsResponse;
  }

  @override
  void update(void Function(PipelineSummaryMetricsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PipelineSummaryMetricsResponse build() => _build();

  _$PipelineSummaryMetricsResponse _build() {
    final _$result = _$v ??
        _$PipelineSummaryMetricsResponse._(
          totalCount: totalCount,
          totalExpectedBudget: totalExpectedBudget,
          openCount: openCount,
          openExpectedBudget: openExpectedBudget,
          wonCount: wonCount,
          wonBudget: wonBudget,
          lostCount: lostCount,
          winRatePercent: winRatePercent,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
