// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_sla_report_row_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceSlaReportRowResponse extends ServiceSlaReportRowResponse {
  @override
  final String? serviceRequestId;
  @override
  final String? number;
  @override
  final String? title;
  @override
  final String? projectCode;
  @override
  final String? branchCode;
  @override
  final String? priority;
  @override
  final String? status;
  @override
  final bool? inWarranty;
  @override
  final String? slaStatus;
  @override
  final double? elapsedHours;
  @override
  final int? resolveTargetHours;
  @override
  final double? serviceChargeAmount;
  @override
  final DateTime? createdAtUtc;

  factory _$ServiceSlaReportRowResponse(
          [void Function(ServiceSlaReportRowResponseBuilder)? updates]) =>
      (ServiceSlaReportRowResponseBuilder()..update(updates))._build();

  _$ServiceSlaReportRowResponse._(
      {this.serviceRequestId,
      this.number,
      this.title,
      this.projectCode,
      this.branchCode,
      this.priority,
      this.status,
      this.inWarranty,
      this.slaStatus,
      this.elapsedHours,
      this.resolveTargetHours,
      this.serviceChargeAmount,
      this.createdAtUtc})
      : super._();
  @override
  ServiceSlaReportRowResponse rebuild(
          void Function(ServiceSlaReportRowResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceSlaReportRowResponseBuilder toBuilder() =>
      ServiceSlaReportRowResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceSlaReportRowResponse &&
        serviceRequestId == other.serviceRequestId &&
        number == other.number &&
        title == other.title &&
        projectCode == other.projectCode &&
        branchCode == other.branchCode &&
        priority == other.priority &&
        status == other.status &&
        inWarranty == other.inWarranty &&
        slaStatus == other.slaStatus &&
        elapsedHours == other.elapsedHours &&
        resolveTargetHours == other.resolveTargetHours &&
        serviceChargeAmount == other.serviceChargeAmount &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, serviceRequestId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, priority.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, inWarranty.hashCode);
    _$hash = $jc(_$hash, slaStatus.hashCode);
    _$hash = $jc(_$hash, elapsedHours.hashCode);
    _$hash = $jc(_$hash, resolveTargetHours.hashCode);
    _$hash = $jc(_$hash, serviceChargeAmount.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ServiceSlaReportRowResponse')
          ..add('serviceRequestId', serviceRequestId)
          ..add('number', number)
          ..add('title', title)
          ..add('projectCode', projectCode)
          ..add('branchCode', branchCode)
          ..add('priority', priority)
          ..add('status', status)
          ..add('inWarranty', inWarranty)
          ..add('slaStatus', slaStatus)
          ..add('elapsedHours', elapsedHours)
          ..add('resolveTargetHours', resolveTargetHours)
          ..add('serviceChargeAmount', serviceChargeAmount)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class ServiceSlaReportRowResponseBuilder
    implements
        Builder<ServiceSlaReportRowResponse,
            ServiceSlaReportRowResponseBuilder> {
  _$ServiceSlaReportRowResponse? _$v;

  String? _serviceRequestId;
  String? get serviceRequestId => _$this._serviceRequestId;
  set serviceRequestId(String? serviceRequestId) =>
      _$this._serviceRequestId = serviceRequestId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  String? _branchCode;
  String? get branchCode => _$this._branchCode;
  set branchCode(String? branchCode) => _$this._branchCode = branchCode;

  String? _priority;
  String? get priority => _$this._priority;
  set priority(String? priority) => _$this._priority = priority;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  bool? _inWarranty;
  bool? get inWarranty => _$this._inWarranty;
  set inWarranty(bool? inWarranty) => _$this._inWarranty = inWarranty;

  String? _slaStatus;
  String? get slaStatus => _$this._slaStatus;
  set slaStatus(String? slaStatus) => _$this._slaStatus = slaStatus;

  double? _elapsedHours;
  double? get elapsedHours => _$this._elapsedHours;
  set elapsedHours(double? elapsedHours) => _$this._elapsedHours = elapsedHours;

  int? _resolveTargetHours;
  int? get resolveTargetHours => _$this._resolveTargetHours;
  set resolveTargetHours(int? resolveTargetHours) =>
      _$this._resolveTargetHours = resolveTargetHours;

  double? _serviceChargeAmount;
  double? get serviceChargeAmount => _$this._serviceChargeAmount;
  set serviceChargeAmount(double? serviceChargeAmount) =>
      _$this._serviceChargeAmount = serviceChargeAmount;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  ServiceSlaReportRowResponseBuilder() {
    ServiceSlaReportRowResponse._defaults(this);
  }

  ServiceSlaReportRowResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _serviceRequestId = $v.serviceRequestId;
      _number = $v.number;
      _title = $v.title;
      _projectCode = $v.projectCode;
      _branchCode = $v.branchCode;
      _priority = $v.priority;
      _status = $v.status;
      _inWarranty = $v.inWarranty;
      _slaStatus = $v.slaStatus;
      _elapsedHours = $v.elapsedHours;
      _resolveTargetHours = $v.resolveTargetHours;
      _serviceChargeAmount = $v.serviceChargeAmount;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceSlaReportRowResponse other) {
    _$v = other as _$ServiceSlaReportRowResponse;
  }

  @override
  void update(void Function(ServiceSlaReportRowResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceSlaReportRowResponse build() => _build();

  _$ServiceSlaReportRowResponse _build() {
    final _$result = _$v ??
        _$ServiceSlaReportRowResponse._(
          serviceRequestId: serviceRequestId,
          number: number,
          title: title,
          projectCode: projectCode,
          branchCode: branchCode,
          priority: priority,
          status: status,
          inWarranty: inWarranty,
          slaStatus: slaStatus,
          elapsedHours: elapsedHours,
          resolveTargetHours: resolveTargetHours,
          serviceChargeAmount: serviceChargeAmount,
          createdAtUtc: createdAtUtc,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
