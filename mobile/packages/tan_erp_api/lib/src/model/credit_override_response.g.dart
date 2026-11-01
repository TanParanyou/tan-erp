// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'credit_override_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreditOverrideResponse extends CreditOverrideResponse {
  @override
  final String? id;
  @override
  final String? organizationId;
  @override
  final String? branchId;
  @override
  final String? customerId;
  @override
  final String? customerName;
  @override
  final double? requestedCeilingAmount;
  @override
  final String? reason;
  @override
  final Date? expiresOn;
  @override
  final String? status;
  @override
  final FinancePersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final FinancePersonResponse? decidedBy;
  @override
  final DateTime? decidedAtUtc;
  @override
  final String? decisionNote;
  @override
  final String? rowVersion;

  factory _$CreditOverrideResponse(
          [void Function(CreditOverrideResponseBuilder)? updates]) =>
      (CreditOverrideResponseBuilder()..update(updates))._build();

  _$CreditOverrideResponse._(
      {this.id,
      this.organizationId,
      this.branchId,
      this.customerId,
      this.customerName,
      this.requestedCeilingAmount,
      this.reason,
      this.expiresOn,
      this.status,
      this.createdBy,
      this.createdAtUtc,
      this.decidedBy,
      this.decidedAtUtc,
      this.decisionNote,
      this.rowVersion})
      : super._();
  @override
  CreditOverrideResponse rebuild(
          void Function(CreditOverrideResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreditOverrideResponseBuilder toBuilder() =>
      CreditOverrideResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreditOverrideResponse &&
        id == other.id &&
        organizationId == other.organizationId &&
        branchId == other.branchId &&
        customerId == other.customerId &&
        customerName == other.customerName &&
        requestedCeilingAmount == other.requestedCeilingAmount &&
        reason == other.reason &&
        expiresOn == other.expiresOn &&
        status == other.status &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        decidedBy == other.decidedBy &&
        decidedAtUtc == other.decidedAtUtc &&
        decisionNote == other.decisionNote &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, organizationId.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, customerName.hashCode);
    _$hash = $jc(_$hash, requestedCeilingAmount.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, expiresOn.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jc(_$hash, decisionNote.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreditOverrideResponse')
          ..add('id', id)
          ..add('organizationId', organizationId)
          ..add('branchId', branchId)
          ..add('customerId', customerId)
          ..add('customerName', customerName)
          ..add('requestedCeilingAmount', requestedCeilingAmount)
          ..add('reason', reason)
          ..add('expiresOn', expiresOn)
          ..add('status', status)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('decidedBy', decidedBy)
          ..add('decidedAtUtc', decidedAtUtc)
          ..add('decisionNote', decisionNote)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class CreditOverrideResponseBuilder
    implements Builder<CreditOverrideResponse, CreditOverrideResponseBuilder> {
  _$CreditOverrideResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _organizationId;
  String? get organizationId => _$this._organizationId;
  set organizationId(String? organizationId) =>
      _$this._organizationId = organizationId;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

  String? _customerName;
  String? get customerName => _$this._customerName;
  set customerName(String? customerName) => _$this._customerName = customerName;

  double? _requestedCeilingAmount;
  double? get requestedCeilingAmount => _$this._requestedCeilingAmount;
  set requestedCeilingAmount(double? requestedCeilingAmount) =>
      _$this._requestedCeilingAmount = requestedCeilingAmount;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  Date? _expiresOn;
  Date? get expiresOn => _$this._expiresOn;
  set expiresOn(Date? expiresOn) => _$this._expiresOn = expiresOn;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  FinancePersonResponseBuilder? _createdBy;
  FinancePersonResponseBuilder get createdBy =>
      _$this._createdBy ??= FinancePersonResponseBuilder();
  set createdBy(FinancePersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  FinancePersonResponseBuilder? _decidedBy;
  FinancePersonResponseBuilder get decidedBy =>
      _$this._decidedBy ??= FinancePersonResponseBuilder();
  set decidedBy(FinancePersonResponseBuilder? decidedBy) =>
      _$this._decidedBy = decidedBy;

  DateTime? _decidedAtUtc;
  DateTime? get decidedAtUtc => _$this._decidedAtUtc;
  set decidedAtUtc(DateTime? decidedAtUtc) =>
      _$this._decidedAtUtc = decidedAtUtc;

  String? _decisionNote;
  String? get decisionNote => _$this._decisionNote;
  set decisionNote(String? decisionNote) => _$this._decisionNote = decisionNote;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  CreditOverrideResponseBuilder() {
    CreditOverrideResponse._defaults(this);
  }

  CreditOverrideResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _organizationId = $v.organizationId;
      _branchId = $v.branchId;
      _customerId = $v.customerId;
      _customerName = $v.customerName;
      _requestedCeilingAmount = $v.requestedCeilingAmount;
      _reason = $v.reason;
      _expiresOn = $v.expiresOn;
      _status = $v.status;
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _decidedBy = $v.decidedBy?.toBuilder();
      _decidedAtUtc = $v.decidedAtUtc;
      _decisionNote = $v.decisionNote;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreditOverrideResponse other) {
    _$v = other as _$CreditOverrideResponse;
  }

  @override
  void update(void Function(CreditOverrideResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreditOverrideResponse build() => _build();

  _$CreditOverrideResponse _build() {
    _$CreditOverrideResponse _$result;
    try {
      _$result = _$v ??
          _$CreditOverrideResponse._(
            id: id,
            organizationId: organizationId,
            branchId: branchId,
            customerId: customerId,
            customerName: customerName,
            requestedCeilingAmount: requestedCeilingAmount,
            reason: reason,
            expiresOn: expiresOn,
            status: status,
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            decidedBy: _decidedBy?.build(),
            decidedAtUtc: decidedAtUtc,
            decisionNote: decisionNote,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'decidedBy';
        _decidedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CreditOverrideResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
