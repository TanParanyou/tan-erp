// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_sla_summary_metrics_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceSlaSummaryMetricsResponse
    extends ServiceSlaSummaryMetricsResponse {
  @override
  final int? totalCount;
  @override
  final int? openBacklogCount;
  @override
  final int? waitingCustomerCount;
  @override
  final int? slaWarningCount;
  @override
  final int? slaBreachedCount;
  @override
  final double? outOfWarrantyChargedTotal;

  factory _$ServiceSlaSummaryMetricsResponse(
          [void Function(ServiceSlaSummaryMetricsResponseBuilder)? updates]) =>
      (ServiceSlaSummaryMetricsResponseBuilder()..update(updates))._build();

  _$ServiceSlaSummaryMetricsResponse._(
      {this.totalCount,
      this.openBacklogCount,
      this.waitingCustomerCount,
      this.slaWarningCount,
      this.slaBreachedCount,
      this.outOfWarrantyChargedTotal})
      : super._();
  @override
  ServiceSlaSummaryMetricsResponse rebuild(
          void Function(ServiceSlaSummaryMetricsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceSlaSummaryMetricsResponseBuilder toBuilder() =>
      ServiceSlaSummaryMetricsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceSlaSummaryMetricsResponse &&
        totalCount == other.totalCount &&
        openBacklogCount == other.openBacklogCount &&
        waitingCustomerCount == other.waitingCustomerCount &&
        slaWarningCount == other.slaWarningCount &&
        slaBreachedCount == other.slaBreachedCount &&
        outOfWarrantyChargedTotal == other.outOfWarrantyChargedTotal;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, totalCount.hashCode);
    _$hash = $jc(_$hash, openBacklogCount.hashCode);
    _$hash = $jc(_$hash, waitingCustomerCount.hashCode);
    _$hash = $jc(_$hash, slaWarningCount.hashCode);
    _$hash = $jc(_$hash, slaBreachedCount.hashCode);
    _$hash = $jc(_$hash, outOfWarrantyChargedTotal.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ServiceSlaSummaryMetricsResponse')
          ..add('totalCount', totalCount)
          ..add('openBacklogCount', openBacklogCount)
          ..add('waitingCustomerCount', waitingCustomerCount)
          ..add('slaWarningCount', slaWarningCount)
          ..add('slaBreachedCount', slaBreachedCount)
          ..add('outOfWarrantyChargedTotal', outOfWarrantyChargedTotal))
        .toString();
  }
}

class ServiceSlaSummaryMetricsResponseBuilder
    implements
        Builder<ServiceSlaSummaryMetricsResponse,
            ServiceSlaSummaryMetricsResponseBuilder> {
  _$ServiceSlaSummaryMetricsResponse? _$v;

  int? _totalCount;
  int? get totalCount => _$this._totalCount;
  set totalCount(int? totalCount) => _$this._totalCount = totalCount;

  int? _openBacklogCount;
  int? get openBacklogCount => _$this._openBacklogCount;
  set openBacklogCount(int? openBacklogCount) =>
      _$this._openBacklogCount = openBacklogCount;

  int? _waitingCustomerCount;
  int? get waitingCustomerCount => _$this._waitingCustomerCount;
  set waitingCustomerCount(int? waitingCustomerCount) =>
      _$this._waitingCustomerCount = waitingCustomerCount;

  int? _slaWarningCount;
  int? get slaWarningCount => _$this._slaWarningCount;
  set slaWarningCount(int? slaWarningCount) =>
      _$this._slaWarningCount = slaWarningCount;

  int? _slaBreachedCount;
  int? get slaBreachedCount => _$this._slaBreachedCount;
  set slaBreachedCount(int? slaBreachedCount) =>
      _$this._slaBreachedCount = slaBreachedCount;

  double? _outOfWarrantyChargedTotal;
  double? get outOfWarrantyChargedTotal => _$this._outOfWarrantyChargedTotal;
  set outOfWarrantyChargedTotal(double? outOfWarrantyChargedTotal) =>
      _$this._outOfWarrantyChargedTotal = outOfWarrantyChargedTotal;

  ServiceSlaSummaryMetricsResponseBuilder() {
    ServiceSlaSummaryMetricsResponse._defaults(this);
  }

  ServiceSlaSummaryMetricsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _totalCount = $v.totalCount;
      _openBacklogCount = $v.openBacklogCount;
      _waitingCustomerCount = $v.waitingCustomerCount;
      _slaWarningCount = $v.slaWarningCount;
      _slaBreachedCount = $v.slaBreachedCount;
      _outOfWarrantyChargedTotal = $v.outOfWarrantyChargedTotal;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceSlaSummaryMetricsResponse other) {
    _$v = other as _$ServiceSlaSummaryMetricsResponse;
  }

  @override
  void update(void Function(ServiceSlaSummaryMetricsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceSlaSummaryMetricsResponse build() => _build();

  _$ServiceSlaSummaryMetricsResponse _build() {
    final _$result = _$v ??
        _$ServiceSlaSummaryMetricsResponse._(
          totalCount: totalCount,
          openBacklogCount: openBacklogCount,
          waitingCustomerCount: waitingCustomerCount,
          slaWarningCount: slaWarningCount,
          slaBreachedCount: slaBreachedCount,
          outOfWarrantyChargedTotal: outOfWarrantyChargedTotal,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
