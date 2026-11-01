// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_request_list_item_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ServiceRequestListItemResponse extends ServiceRequestListItemResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? title;
  @override
  final String? priority;
  @override
  final String? status;
  @override
  final bool? inWarranty;
  @override
  final String? projectCode;
  @override
  final ServiceSlaResponse? sla;
  @override
  final double? serviceChargeAmount;
  @override
  final String? billingDocumentId;
  @override
  final String? billingDocumentNumber;
  @override
  final DateTime? createdAtUtc;

  factory _$ServiceRequestListItemResponse(
          [void Function(ServiceRequestListItemResponseBuilder)? updates]) =>
      (ServiceRequestListItemResponseBuilder()..update(updates))._build();

  _$ServiceRequestListItemResponse._(
      {this.id,
      this.number,
      this.title,
      this.priority,
      this.status,
      this.inWarranty,
      this.projectCode,
      this.sla,
      this.serviceChargeAmount,
      this.billingDocumentId,
      this.billingDocumentNumber,
      this.createdAtUtc})
      : super._();
  @override
  ServiceRequestListItemResponse rebuild(
          void Function(ServiceRequestListItemResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ServiceRequestListItemResponseBuilder toBuilder() =>
      ServiceRequestListItemResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ServiceRequestListItemResponse &&
        id == other.id &&
        number == other.number &&
        title == other.title &&
        priority == other.priority &&
        status == other.status &&
        inWarranty == other.inWarranty &&
        projectCode == other.projectCode &&
        sla == other.sla &&
        serviceChargeAmount == other.serviceChargeAmount &&
        billingDocumentId == other.billingDocumentId &&
        billingDocumentNumber == other.billingDocumentNumber &&
        createdAtUtc == other.createdAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jc(_$hash, priority.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, inWarranty.hashCode);
    _$hash = $jc(_$hash, projectCode.hashCode);
    _$hash = $jc(_$hash, sla.hashCode);
    _$hash = $jc(_$hash, serviceChargeAmount.hashCode);
    _$hash = $jc(_$hash, billingDocumentId.hashCode);
    _$hash = $jc(_$hash, billingDocumentNumber.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ServiceRequestListItemResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('title', title)
          ..add('priority', priority)
          ..add('status', status)
          ..add('inWarranty', inWarranty)
          ..add('projectCode', projectCode)
          ..add('sla', sla)
          ..add('serviceChargeAmount', serviceChargeAmount)
          ..add('billingDocumentId', billingDocumentId)
          ..add('billingDocumentNumber', billingDocumentNumber)
          ..add('createdAtUtc', createdAtUtc))
        .toString();
  }
}

class ServiceRequestListItemResponseBuilder
    implements
        Builder<ServiceRequestListItemResponse,
            ServiceRequestListItemResponseBuilder> {
  _$ServiceRequestListItemResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  String? _priority;
  String? get priority => _$this._priority;
  set priority(String? priority) => _$this._priority = priority;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  bool? _inWarranty;
  bool? get inWarranty => _$this._inWarranty;
  set inWarranty(bool? inWarranty) => _$this._inWarranty = inWarranty;

  String? _projectCode;
  String? get projectCode => _$this._projectCode;
  set projectCode(String? projectCode) => _$this._projectCode = projectCode;

  ServiceSlaResponseBuilder? _sla;
  ServiceSlaResponseBuilder get sla =>
      _$this._sla ??= ServiceSlaResponseBuilder();
  set sla(ServiceSlaResponseBuilder? sla) => _$this._sla = sla;

  double? _serviceChargeAmount;
  double? get serviceChargeAmount => _$this._serviceChargeAmount;
  set serviceChargeAmount(double? serviceChargeAmount) =>
      _$this._serviceChargeAmount = serviceChargeAmount;

  String? _billingDocumentId;
  String? get billingDocumentId => _$this._billingDocumentId;
  set billingDocumentId(String? billingDocumentId) =>
      _$this._billingDocumentId = billingDocumentId;

  String? _billingDocumentNumber;
  String? get billingDocumentNumber => _$this._billingDocumentNumber;
  set billingDocumentNumber(String? billingDocumentNumber) =>
      _$this._billingDocumentNumber = billingDocumentNumber;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  ServiceRequestListItemResponseBuilder() {
    ServiceRequestListItemResponse._defaults(this);
  }

  ServiceRequestListItemResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _title = $v.title;
      _priority = $v.priority;
      _status = $v.status;
      _inWarranty = $v.inWarranty;
      _projectCode = $v.projectCode;
      _sla = $v.sla?.toBuilder();
      _serviceChargeAmount = $v.serviceChargeAmount;
      _billingDocumentId = $v.billingDocumentId;
      _billingDocumentNumber = $v.billingDocumentNumber;
      _createdAtUtc = $v.createdAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ServiceRequestListItemResponse other) {
    _$v = other as _$ServiceRequestListItemResponse;
  }

  @override
  void update(void Function(ServiceRequestListItemResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ServiceRequestListItemResponse build() => _build();

  _$ServiceRequestListItemResponse _build() {
    _$ServiceRequestListItemResponse _$result;
    try {
      _$result = _$v ??
          _$ServiceRequestListItemResponse._(
            id: id,
            number: number,
            title: title,
            priority: priority,
            status: status,
            inWarranty: inWarranty,
            projectCode: projectCode,
            sla: _sla?.build(),
            serviceChargeAmount: serviceChargeAmount,
            billingDocumentId: billingDocumentId,
            billingDocumentNumber: billingDocumentNumber,
            createdAtUtc: createdAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'sla';
        _sla?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ServiceRequestListItemResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
