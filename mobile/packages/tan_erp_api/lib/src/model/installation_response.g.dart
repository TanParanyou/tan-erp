// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'installation_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InstallationResponse extends InstallationResponse {
  @override
  final String? id;
  @override
  final String? branchId;
  @override
  final String? number;
  @override
  final String? status;
  @override
  final ServiceProjectRefResponse? project;
  @override
  final Date? scheduledStart;
  @override
  final Date? scheduledEnd;
  @override
  final String? crewName;
  @override
  final String? note;
  @override
  final String? cancelReason;
  @override
  final DateTime? readyAtUtc;
  @override
  final HandoverResponse? handover;
  @override
  final int? disputeCount;
  @override
  final String? lastDisputeNote;
  @override
  final WarrantyRefResponse? warranty;
  @override
  final ServicePersonResponse? createdBy;
  @override
  final DateTime? createdAtUtc;
  @override
  final String? rowVersion;
  @override
  final BuiltList<ChecklistItemResponse>? checklist;
  @override
  final BuiltList<DefectResponse>? defects;

  factory _$InstallationResponse(
          [void Function(InstallationResponseBuilder)? updates]) =>
      (InstallationResponseBuilder()..update(updates))._build();

  _$InstallationResponse._(
      {this.id,
      this.branchId,
      this.number,
      this.status,
      this.project,
      this.scheduledStart,
      this.scheduledEnd,
      this.crewName,
      this.note,
      this.cancelReason,
      this.readyAtUtc,
      this.handover,
      this.disputeCount,
      this.lastDisputeNote,
      this.warranty,
      this.createdBy,
      this.createdAtUtc,
      this.rowVersion,
      this.checklist,
      this.defects})
      : super._();
  @override
  InstallationResponse rebuild(
          void Function(InstallationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InstallationResponseBuilder toBuilder() =>
      InstallationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InstallationResponse &&
        id == other.id &&
        branchId == other.branchId &&
        number == other.number &&
        status == other.status &&
        project == other.project &&
        scheduledStart == other.scheduledStart &&
        scheduledEnd == other.scheduledEnd &&
        crewName == other.crewName &&
        note == other.note &&
        cancelReason == other.cancelReason &&
        readyAtUtc == other.readyAtUtc &&
        handover == other.handover &&
        disputeCount == other.disputeCount &&
        lastDisputeNote == other.lastDisputeNote &&
        warranty == other.warranty &&
        createdBy == other.createdBy &&
        createdAtUtc == other.createdAtUtc &&
        rowVersion == other.rowVersion &&
        checklist == other.checklist &&
        defects == other.defects;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, branchId.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, project.hashCode);
    _$hash = $jc(_$hash, scheduledStart.hashCode);
    _$hash = $jc(_$hash, scheduledEnd.hashCode);
    _$hash = $jc(_$hash, crewName.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, cancelReason.hashCode);
    _$hash = $jc(_$hash, readyAtUtc.hashCode);
    _$hash = $jc(_$hash, handover.hashCode);
    _$hash = $jc(_$hash, disputeCount.hashCode);
    _$hash = $jc(_$hash, lastDisputeNote.hashCode);
    _$hash = $jc(_$hash, warranty.hashCode);
    _$hash = $jc(_$hash, createdBy.hashCode);
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, checklist.hashCode);
    _$hash = $jc(_$hash, defects.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'InstallationResponse')
          ..add('id', id)
          ..add('branchId', branchId)
          ..add('number', number)
          ..add('status', status)
          ..add('project', project)
          ..add('scheduledStart', scheduledStart)
          ..add('scheduledEnd', scheduledEnd)
          ..add('crewName', crewName)
          ..add('note', note)
          ..add('cancelReason', cancelReason)
          ..add('readyAtUtc', readyAtUtc)
          ..add('handover', handover)
          ..add('disputeCount', disputeCount)
          ..add('lastDisputeNote', lastDisputeNote)
          ..add('warranty', warranty)
          ..add('createdBy', createdBy)
          ..add('createdAtUtc', createdAtUtc)
          ..add('rowVersion', rowVersion)
          ..add('checklist', checklist)
          ..add('defects', defects))
        .toString();
  }
}

class InstallationResponseBuilder
    implements Builder<InstallationResponse, InstallationResponseBuilder> {
  _$InstallationResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _branchId;
  String? get branchId => _$this._branchId;
  set branchId(String? branchId) => _$this._branchId = branchId;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  ServiceProjectRefResponseBuilder? _project;
  ServiceProjectRefResponseBuilder get project =>
      _$this._project ??= ServiceProjectRefResponseBuilder();
  set project(ServiceProjectRefResponseBuilder? project) =>
      _$this._project = project;

  Date? _scheduledStart;
  Date? get scheduledStart => _$this._scheduledStart;
  set scheduledStart(Date? scheduledStart) =>
      _$this._scheduledStart = scheduledStart;

  Date? _scheduledEnd;
  Date? get scheduledEnd => _$this._scheduledEnd;
  set scheduledEnd(Date? scheduledEnd) => _$this._scheduledEnd = scheduledEnd;

  String? _crewName;
  String? get crewName => _$this._crewName;
  set crewName(String? crewName) => _$this._crewName = crewName;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  String? _cancelReason;
  String? get cancelReason => _$this._cancelReason;
  set cancelReason(String? cancelReason) => _$this._cancelReason = cancelReason;

  DateTime? _readyAtUtc;
  DateTime? get readyAtUtc => _$this._readyAtUtc;
  set readyAtUtc(DateTime? readyAtUtc) => _$this._readyAtUtc = readyAtUtc;

  HandoverResponseBuilder? _handover;
  HandoverResponseBuilder get handover =>
      _$this._handover ??= HandoverResponseBuilder();
  set handover(HandoverResponseBuilder? handover) =>
      _$this._handover = handover;

  int? _disputeCount;
  int? get disputeCount => _$this._disputeCount;
  set disputeCount(int? disputeCount) => _$this._disputeCount = disputeCount;

  String? _lastDisputeNote;
  String? get lastDisputeNote => _$this._lastDisputeNote;
  set lastDisputeNote(String? lastDisputeNote) =>
      _$this._lastDisputeNote = lastDisputeNote;

  WarrantyRefResponseBuilder? _warranty;
  WarrantyRefResponseBuilder get warranty =>
      _$this._warranty ??= WarrantyRefResponseBuilder();
  set warranty(WarrantyRefResponseBuilder? warranty) =>
      _$this._warranty = warranty;

  ServicePersonResponseBuilder? _createdBy;
  ServicePersonResponseBuilder get createdBy =>
      _$this._createdBy ??= ServicePersonResponseBuilder();
  set createdBy(ServicePersonResponseBuilder? createdBy) =>
      _$this._createdBy = createdBy;

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ListBuilder<ChecklistItemResponse>? _checklist;
  ListBuilder<ChecklistItemResponse> get checklist =>
      _$this._checklist ??= ListBuilder<ChecklistItemResponse>();
  set checklist(ListBuilder<ChecklistItemResponse>? checklist) =>
      _$this._checklist = checklist;

  ListBuilder<DefectResponse>? _defects;
  ListBuilder<DefectResponse> get defects =>
      _$this._defects ??= ListBuilder<DefectResponse>();
  set defects(ListBuilder<DefectResponse>? defects) =>
      _$this._defects = defects;

  InstallationResponseBuilder() {
    InstallationResponse._defaults(this);
  }

  InstallationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _branchId = $v.branchId;
      _number = $v.number;
      _status = $v.status;
      _project = $v.project?.toBuilder();
      _scheduledStart = $v.scheduledStart;
      _scheduledEnd = $v.scheduledEnd;
      _crewName = $v.crewName;
      _note = $v.note;
      _cancelReason = $v.cancelReason;
      _readyAtUtc = $v.readyAtUtc;
      _handover = $v.handover?.toBuilder();
      _disputeCount = $v.disputeCount;
      _lastDisputeNote = $v.lastDisputeNote;
      _warranty = $v.warranty?.toBuilder();
      _createdBy = $v.createdBy?.toBuilder();
      _createdAtUtc = $v.createdAtUtc;
      _rowVersion = $v.rowVersion;
      _checklist = $v.checklist?.toBuilder();
      _defects = $v.defects?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(InstallationResponse other) {
    _$v = other as _$InstallationResponse;
  }

  @override
  void update(void Function(InstallationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InstallationResponse build() => _build();

  _$InstallationResponse _build() {
    _$InstallationResponse _$result;
    try {
      _$result = _$v ??
          _$InstallationResponse._(
            id: id,
            branchId: branchId,
            number: number,
            status: status,
            project: _project?.build(),
            scheduledStart: scheduledStart,
            scheduledEnd: scheduledEnd,
            crewName: crewName,
            note: note,
            cancelReason: cancelReason,
            readyAtUtc: readyAtUtc,
            handover: _handover?.build(),
            disputeCount: disputeCount,
            lastDisputeNote: lastDisputeNote,
            warranty: _warranty?.build(),
            createdBy: _createdBy?.build(),
            createdAtUtc: createdAtUtc,
            rowVersion: rowVersion,
            checklist: _checklist?.build(),
            defects: _defects?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'project';
        _project?.build();

        _$failedField = 'handover';
        _handover?.build();

        _$failedField = 'warranty';
        _warranty?.build();
        _$failedField = 'createdBy';
        _createdBy?.build();

        _$failedField = 'checklist';
        _checklist?.build();
        _$failedField = 'defects';
        _defects?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'InstallationResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
