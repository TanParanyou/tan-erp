// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_rate_override_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchRateOverrideResponse extends BranchRateOverrideResponse {
  @override
  final String? id;
  @override
  final String? templateId;
  @override
  final String? branchId;
  @override
  final String? branchCode;
  @override
  final String? status;
  @override
  final double? referenceRate;
  @override
  final double? minimumCharge;
  @override
  final Date? effectiveFrom;
  @override
  final Date? effectiveTo;
  @override
  final String? reasonCode;
  @override
  final String? reasonDetail;
  @override
  final String? sourceReference;
  @override
  final QePersonResponse? requestedBy;
  @override
  final DateTime? requestedAtUtc;
  @override
  final QePersonResponse? decidedBy;
  @override
  final DateTime? decidedAtUtc;
  @override
  final String? decisionNote;
  @override
  final String? rowVersion;

  factory _$BranchRateOverrideResponse(
          [void Function(BranchRateOverrideResponseBuilder)? updates]) =>
      (BranchRateOverrideResponseBuilder()..update(updates))._build();

  _$BranchRateOverrideResponse._(
      {this.id,
      this.templateId,
      this.branchId,
      this.branchCode,
      this.status,
      this.referenceRate,
      this.minimumCharge,
      this.effectiveFrom,
      this.effectiveTo,
      this.reasonCode,
      this.reasonDetail,
      this.sourceReference,
      this.requestedBy,
      this.requestedAtUtc,
      this.decidedBy,
      this.decidedAtUtc,
      this.decisionNote,
      this.rowVersion})
      : super._();
  @override
  BranchRateOverrideResponse rebuild(
          void Function(BranchRateOverrideResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchRateOverrideResponseBuilder toBuilder() =>
      BranchRateOverrideResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchRateOverrideResponse &&
        id == other.id &&
        templateId == other.templateId &&
        branchId == other.branchId &&
        branchCode == other.branchCode &&
        status == other.status &&
        referenceRate == other.referenceRate &&
        minimumCharge == other.minimumCharge &&
        effectiveFrom == other.effectiveFrom &&
        effectiveTo == other.effectiveTo &&
        reasonCode == other.reasonCode &&
        reasonDetail == other.reasonDetail &&
        sourceReference == other.sourceReference &&
        requestedBy == other.requestedBy &&
        requestedAtUtc == other.requestedAtUtc &&
        decidedBy == other.decidedBy &&
        decidedAtUtc == other.decidedAtUtc &&
        decisionNote == other.decisionNote &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, templateId.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, branchCode.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, referenceRate.hashCode);
    _$hash = $jc(_$hash, minimumCharge.hashCode);
    _$hash = $jc(_$hash, effectiveFrom.hashCode);
    _$hash = $jc(_$hash, effectiveTo.hashCode);
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jc(_$hash, reasonDetail.hashCode);
    _$hash = $jc(_$hash, sourceReference.hashCode);
    _$hash = $jc(_$hash, requestedBy.hashCode);
    _$hash = $jc(_$hash, requestedAtUtc.hashCode);
    _$hash = $jc(_$hash, decidedBy.hashCode);
    _$hash = $jc(_$hash, decidedAtUtc.hashCode);
    _$hash = $jc(_$hash, decisionNote.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BranchRateOverrideResponse')
          ..add('id', id)
          ..add('templateId', templateId)
          ..add('branchId', branchId)
          ..add('branchCode', branchCode)
          ..add('status', status)
          ..add('referenceRate', referenceRate)
          ..add('minimumCharge', minimumCharge)
          ..add('effectiveFrom', effectiveFrom)
          ..add('effectiveTo', effectiveTo)
          ..add('reasonCode', reasonCode)
          ..add('reasonDetail', reasonDetail)
          ..add('sourceReference', sourceReference)
          ..add('requestedBy', requestedBy)
          ..add('requestedAtUtc', requestedAtUtc)
          ..add('decidedBy', decidedBy)
          ..add('decidedAtUtc', decidedAtUtc)
          ..add('decisionNote', decisionNote)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class BranchRateOverrideResponseBuilder
    implements
        Builder<BranchRateOverrideResponse, BranchRateOverrideResponseBuilder> {
  _$BranchRateOverrideResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _templateId;
  String? get templateId => _$this._templateId;
  set templateId(String? templateId) => _$this._templateId = templateId;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _branchCode;
  String? get branchCode => _$this._branchCode;
  set branchCode(String? branchCode) => _$this._branchCode = branchCode;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  double? _referenceRate;
  double? get referenceRate => _$this._referenceRate;
  set referenceRate(double? referenceRate) =>
      _$this._referenceRate = referenceRate;

  double? _minimumCharge;
  double? get minimumCharge => _$this._minimumCharge;
  set minimumCharge(double? minimumCharge) =>
      _$this._minimumCharge = minimumCharge;

  Date? _effectiveFrom;
  Date? get effectiveFrom => _$this._effectiveFrom;
  set effectiveFrom(Date? effectiveFrom) =>
      _$this._effectiveFrom = effectiveFrom;

  Date? _effectiveTo;
  Date? get effectiveTo => _$this._effectiveTo;
  set effectiveTo(Date? effectiveTo) => _$this._effectiveTo = effectiveTo;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  String? _reasonDetail;
  String? get reasonDetail => _$this._reasonDetail;
  set reasonDetail(String? reasonDetail) => _$this._reasonDetail = reasonDetail;

  String? _sourceReference;
  String? get sourceReference => _$this._sourceReference;
  set sourceReference(String? sourceReference) =>
      _$this._sourceReference = sourceReference;

  QePersonResponseBuilder? _requestedBy;
  QePersonResponseBuilder get requestedBy =>
      _$this._requestedBy ??= QePersonResponseBuilder();
  set requestedBy(QePersonResponseBuilder? requestedBy) =>
      _$this._requestedBy = requestedBy;

  DateTime? _requestedAtUtc;
  DateTime? get requestedAtUtc => _$this._requestedAtUtc;
  set requestedAtUtc(DateTime? requestedAtUtc) =>
      _$this._requestedAtUtc = requestedAtUtc;

  QePersonResponseBuilder? _decidedBy;
  QePersonResponseBuilder get decidedBy =>
      _$this._decidedBy ??= QePersonResponseBuilder();
  set decidedBy(QePersonResponseBuilder? decidedBy) =>
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

  BranchRateOverrideResponseBuilder() {
    BranchRateOverrideResponse._defaults(this);
  }

  BranchRateOverrideResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _templateId = $v.templateId;
      _branchId = $v.branchId;
      _branchCode = $v.branchCode;
      _status = $v.status;
      _referenceRate = $v.referenceRate;
      _minimumCharge = $v.minimumCharge;
      _effectiveFrom = $v.effectiveFrom;
      _effectiveTo = $v.effectiveTo;
      _reasonCode = $v.reasonCode;
      _reasonDetail = $v.reasonDetail;
      _sourceReference = $v.sourceReference;
      _requestedBy = $v.requestedBy?.toBuilder();
      _requestedAtUtc = $v.requestedAtUtc;
      _decidedBy = $v.decidedBy?.toBuilder();
      _decidedAtUtc = $v.decidedAtUtc;
      _decisionNote = $v.decisionNote;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchRateOverrideResponse other) {
    _$v = other as _$BranchRateOverrideResponse;
  }

  @override
  void update(void Function(BranchRateOverrideResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchRateOverrideResponse build() => _build();

  _$BranchRateOverrideResponse _build() {
    _$BranchRateOverrideResponse _$result;
    try {
      _$result = _$v ??
          _$BranchRateOverrideResponse._(
            id: id,
            templateId: templateId,
            branchId: branchId,
            branchCode: branchCode,
            status: status,
            referenceRate: referenceRate,
            minimumCharge: minimumCharge,
            effectiveFrom: effectiveFrom,
            effectiveTo: effectiveTo,
            reasonCode: reasonCode,
            reasonDetail: reasonDetail,
            sourceReference: sourceReference,
            requestedBy: _requestedBy?.build(),
            requestedAtUtc: requestedAtUtc,
            decidedBy: _decidedBy?.build(),
            decidedAtUtc: decidedAtUtc,
            decisionNote: decisionNote,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'requestedBy';
        _requestedBy?.build();

        _$failedField = 'decidedBy';
        _decidedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BranchRateOverrideResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
