// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_technician_assignment_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreateTechnicianAssignmentRequest
    extends CreateTechnicianAssignmentRequest {
  @override
  final String? technicianUserId;
  @override
  final Date? workDate;
  @override
  final String? assignmentKind;
  @override
  final String? installationJobId;
  @override
  final String? serviceRequestId;
  @override
  final String? note;
  @override
  final bool? allowConflict;

  factory _$CreateTechnicianAssignmentRequest(
          [void Function(CreateTechnicianAssignmentRequestBuilder)? updates]) =>
      (CreateTechnicianAssignmentRequestBuilder()..update(updates))._build();

  _$CreateTechnicianAssignmentRequest._(
      {this.technicianUserId,
      this.workDate,
      this.assignmentKind,
      this.installationJobId,
      this.serviceRequestId,
      this.note,
      this.allowConflict})
      : super._();
  @override
  CreateTechnicianAssignmentRequest rebuild(
          void Function(CreateTechnicianAssignmentRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreateTechnicianAssignmentRequestBuilder toBuilder() =>
      CreateTechnicianAssignmentRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreateTechnicianAssignmentRequest &&
        technicianUserId == other.technicianUserId &&
        workDate == other.workDate &&
        assignmentKind == other.assignmentKind &&
        installationJobId == other.installationJobId &&
        serviceRequestId == other.serviceRequestId &&
        note == other.note &&
        allowConflict == other.allowConflict;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, technicianUserId.hashCode);
    _$hash = $jc(_$hash, workDate.hashCode);
    _$hash = $jc(_$hash, assignmentKind.hashCode);
    _$hash = $jc(_$hash, installationJobId.hashCode);
    _$hash = $jc(_$hash, serviceRequestId.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, allowConflict.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreateTechnicianAssignmentRequest')
          ..add('technicianUserId', technicianUserId)
          ..add('workDate', workDate)
          ..add('assignmentKind', assignmentKind)
          ..add('installationJobId', installationJobId)
          ..add('serviceRequestId', serviceRequestId)
          ..add('note', note)
          ..add('allowConflict', allowConflict))
        .toString();
  }
}

class CreateTechnicianAssignmentRequestBuilder
    implements
        Builder<CreateTechnicianAssignmentRequest,
            CreateTechnicianAssignmentRequestBuilder> {
  _$CreateTechnicianAssignmentRequest? _$v;

  String? _technicianUserId;
  String? get technicianUserId => _$this._technicianUserId;
  set technicianUserId(String? technicianUserId) =>
      _$this._technicianUserId = technicianUserId;

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

  String? _serviceRequestId;
  String? get serviceRequestId => _$this._serviceRequestId;
  set serviceRequestId(String? serviceRequestId) =>
      _$this._serviceRequestId = serviceRequestId;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  bool? _allowConflict;
  bool? get allowConflict => _$this._allowConflict;
  set allowConflict(bool? allowConflict) =>
      _$this._allowConflict = allowConflict;

  CreateTechnicianAssignmentRequestBuilder() {
    CreateTechnicianAssignmentRequest._defaults(this);
  }

  CreateTechnicianAssignmentRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _technicianUserId = $v.technicianUserId;
      _workDate = $v.workDate;
      _assignmentKind = $v.assignmentKind;
      _installationJobId = $v.installationJobId;
      _serviceRequestId = $v.serviceRequestId;
      _note = $v.note;
      _allowConflict = $v.allowConflict;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreateTechnicianAssignmentRequest other) {
    _$v = other as _$CreateTechnicianAssignmentRequest;
  }

  @override
  void update(
      void Function(CreateTechnicianAssignmentRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreateTechnicianAssignmentRequest build() => _build();

  _$CreateTechnicianAssignmentRequest _build() {
    final _$result = _$v ??
        _$CreateTechnicianAssignmentRequest._(
          technicianUserId: technicianUserId,
          workDate: workDate,
          assignmentKind: assignmentKind,
          installationJobId: installationJobId,
          serviceRequestId: serviceRequestId,
          note: note,
          allowConflict: allowConflict,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
