// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'technician_assignment_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$TechnicianAssignmentResponse extends TechnicianAssignmentResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final ServicePersonResponse? technician;
  @override
  final Date? workDate;
  @override
  final String? assignmentKind;
  @override
  final String? installationJobId;
  @override
  final String? installationJobNumber;
  @override
  final String? serviceRequestId;
  @override
  final String? serviceRequestNumber;
  @override
  final String? note;
  @override
  final ServicePersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final bool? hasConflict;

  factory _$TechnicianAssignmentResponse(
          [void Function(TechnicianAssignmentResponseBuilder)? updates]) =>
      (TechnicianAssignmentResponseBuilder()..update(updates))._build();

  _$TechnicianAssignmentResponse._(
      {this.id,
      this.branchId,
      this.technician,
      this.workDate,
      this.assignmentKind,
      this.installationJobId,
      this.installationJobNumber,
      this.serviceRequestId,
      this.serviceRequestNumber,
      this.note,
      this.createdBy,
      this.createdAtUtc,
      this.hasConflict})
      : super._();
  @override
  TechnicianAssignmentResponse rebuild(
          void Function(TechnicianAssignmentResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  TechnicianAssignmentResponseBuilder toBuilder() =>
      TechnicianAssignmentResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is TechnicianAssignmentResponse &&
        id == other.id &&
        branchId == other.branchId &&
        technician == other.technician &&
        workDate == other.workDate &&
        assignmentKind == other.assignmentKind &&
        installationJobId == other.installationJobId &&
        installationJobNumber == other.installationJobNumber &&
        serviceRequestId == other.serviceRequestId &&
        serviceRequestNumber == other.serviceRequestNumber &&
        note == other.note &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        hasConflict == other.hasConflict;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, technician.hashCode);
    _$hash = $jc(_$hash, workDate.hashCode);
    _$hash = $jc(_$hash, assignmentKind.hashCode);
    _$hash = $jc(_$hash, installationJobId.hashCode);
    _$hash = $jc(_$hash, installationJobNumber.hashCode);
    _$hash = $jc(_$hash, serviceRequestId.hashCode);
    _$hash = $jc(_$hash, serviceRequestNumber.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, hasConflict.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'TechnicianAssignmentResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('technician', technician)
          ..add('workDate', workDate)
          ..add('assignmentKind', assignmentKind)
          ..add('installationJobId', installationJobId)
          ..add('installationJobNumber', installationJobNumber)
          ..add('serviceRequestId', serviceRequestId)
          ..add('serviceRequestNumber', serviceRequestNumber)
          ..add('note', note)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('hasConflict', hasConflict))
        .toString();
  }
}

class TechnicianAssignmentResponseBuilder
    implements
        Builder<TechnicianAssignmentResponse,
            TechnicianAssignmentResponseBuilder> {
  _$TechnicianAssignmentResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  ServicePersonResponseBuilder? _technician;
  ServicePersonResponseBuilder get technician =>
      _$this._technician ??= ServicePersonResponseBuilder();
  set technician(ServicePersonResponseBuilder? technician) =>
      _$this._technician = technician;

  Date? _workDate;
  Date? get workDate => _$this._workDate;
  set workDate(Date? workDate) => _$this._workDate = workDate;

  String? _assignmentKind;
  String? get assignmentKind => _$this._assignmentKind;
  set assignmentKind(String? assignmentKind) =>
      _$this._assignmentKind = assignmentKind;

  String? _installationJobId;
  String? get installationJobId => _$this._installationJobId;
  set installationJobId(String? installationJobId) =>
      _$this._installationJobId = installationJobId;

  String? _installationJobNumber;
  String? get installationJobNumber => _$this._installationJobNumber;
  set installationJobNumber(String? installationJobNumber) =>
      _$this._installationJobNumber = installationJobNumber;

  String? _serviceRequestId;
  String? get serviceRequestId => _$this._serviceRequestId;
  set serviceRequestId(String? serviceRequestId) =>
      _$this._serviceRequestId = serviceRequestId;

  String? _serviceRequestNumber;
  String? get serviceRequestNumber => _$this._serviceRequestNumber;
  set serviceRequestNumber(String? serviceRequestNumber) =>
      _$this._serviceRequestNumber = serviceRequestNumber;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ServicePersonResponseBuilder? _createdBy;
  ServicePersonResponseBuilder get createdBy =>
      _$this._createdBy ??= ServicePersonResponseBuilder();
  set createdBy(ServicePersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  bool? _hasConflict;
  bool? get hasConflict => _$this._hasConflict;
  set hasConflict(bool? hasConflict) => _$this._hasConflict = hasConflict;

  TechnicianAssignmentResponseBuilder() {
    TechnicianAssignmentResponse._defaults(this);
  }

  TechnicianAssignmentResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _technician = $v.technician?.toBuilder();
      _workDate = $v.workDate;
      _assignmentKind = $v.assignmentKind;
      _installationJobId = $v.installationJobId;
      _installationJobNumber = $v.installationJobNumber;
      _serviceRequestId = $v.serviceRequestId;
      _serviceRequestNumber = $v.serviceRequestNumber;
      _note = $v.note;
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _hasConflict = $v.hasConflict;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(TechnicianAssignmentResponse other) {
    _$v = other as _$TechnicianAssignmentResponse;
  }

  @override
  void update(void Function(TechnicianAssignmentResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  TechnicianAssignmentResponse build() => _build();

  _$TechnicianAssignmentResponse _build() {
    _$TechnicianAssignmentResponse _$result;
    try {
      _$result = _$v ??
          _$TechnicianAssignmentResponse._(
            id: id,
            branchId: branchId,
            technician: _technician?.build(),
            workDate: workDate,
            assignmentKind: assignmentKind,
            installationJobId: installationJobId,
            installationJobNumber: installationJobNumber,
            serviceRequestId: serviceRequestId,
            serviceRequestNumber: serviceRequestNumber,
            note: note,
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            hasConflict: hasConflict,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'technician';
        _technician?.build();

        _$failedField = 'createdBy';
        _createdBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'TechnicianAssignmentResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
