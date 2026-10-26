// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'installation_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InstallationRequest extends InstallationRequest {
  @override
  final String? projectId;
  @override
  final Date? scheduledStart;
  @override
  final Date? scheduledEnd;
  @override
  final String? crewName;
  @override
  final String? note;
  @override
  final BuiltList<ChecklistItemRequest>? checklist;

  factory _$InstallationRequest(
          [void Function(InstallationRequestBuilder)? updates]) =>
      (InstallationRequestBuilder()..update(updates))._build();

  _$InstallationRequest._(
      {this.projectId,
      this.scheduledStart,
      this.scheduledEnd,
      this.crewName,
      this.note,
      this.checklist})
      : super._();
  @override
  InstallationRequest rebuild(
          void Function(InstallationRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InstallationRequestBuilder toBuilder() =>
      InstallationRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InstallationRequest &&
        projectId == other.projectId &&
        scheduledStart == other.scheduledStart &&
        scheduledEnd == other.scheduledEnd &&
        crewName == other.crewName &&
        note == other.note &&
        checklist == other.checklist;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, projectId.hashCode);
    _$hash = $jc(_$hash, scheduledStart.hashCode);
    _$hash = $jc(_$hash, scheduledEnd.hashCode);
    _$hash = $jc(_$hash, crewName.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, checklist.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'InstallationRequest')
          ..add('projectId', projectId)
          ..add('scheduledStart', scheduledStart)
          ..add('scheduledEnd', scheduledEnd)
          ..add('crewName', crewName)
          ..add('note', note)
          ..add('checklist', checklist))
        .toString();
  }
}

class InstallationRequestBuilder
    implements Builder<InstallationRequest, InstallationRequestBuilder> {
  _$InstallationRequest? _$v;

  String? _projectId;
  String? get projectId => _$this._projectId;
  set projectId(String? projectId) => _$this._projectId = projectId;

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

  ListBuilder<ChecklistItemRequest>? _checklist;
  ListBuilder<ChecklistItemRequest> get checklist =>
      _$this._checklist ??= ListBuilder<ChecklistItemRequest>();
  set checklist(ListBuilder<ChecklistItemRequest>? checklist) =>
      _$this._checklist = checklist;

  InstallationRequestBuilder() {
    InstallationRequest._defaults(this);
  }

  InstallationRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _projectId = $v.projectId;
      _scheduledStart = $v.scheduledStart;
      _scheduledEnd = $v.scheduledEnd;
      _crewName = $v.crewName;
      _note = $v.note;
      _checklist = $v.checklist?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(InstallationRequest other) {
    _$v = other as _$InstallationRequest;
  }

  @override
  void update(void Function(InstallationRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InstallationRequest build() => _build();

  _$InstallationRequest _build() {
    _$InstallationRequest _$result;
    try {
      _$result = _$v ??
          _$InstallationRequest._(
            projectId: projectId,
            scheduledStart: scheduledStart,
            scheduledEnd: scheduledEnd,
            crewName: crewName,
            note: note,
            checklist: _checklist?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'checklist';
        _checklist?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'InstallationRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
