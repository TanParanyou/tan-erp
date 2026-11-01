// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_timesheet_action_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectTimesheetActionRequest extends ProjectTimesheetActionRequest {
  @override
  final String? expectedVersion;
  @override
  final String? note;

  factory _$ProjectTimesheetActionRequest(
          [void Function(ProjectTimesheetActionRequestBuilder)? updates]) =>
      (ProjectTimesheetActionRequestBuilder()..update(updates))._build();

  _$ProjectTimesheetActionRequest._({this.expectedVersion, this.note})
      : super._();
  @override
  ProjectTimesheetActionRequest rebuild(
          void Function(ProjectTimesheetActionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectTimesheetActionRequestBuilder toBuilder() =>
      ProjectTimesheetActionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectTimesheetActionRequest &&
        expectedVersion == other.expectedVersion &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedVersion.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectTimesheetActionRequest')
          ..add('expectedVersion', expectedVersion)
          ..add('note', note))
        .toString();
  }
}

class ProjectTimesheetActionRequestBuilder
    implements
        Builder<ProjectTimesheetActionRequest,
            ProjectTimesheetActionRequestBuilder> {
  _$ProjectTimesheetActionRequest? _$v;

  String? _expectedVersion;
  String? get expectedVersion => _$this._expectedVersion;
  set expectedVersion(String? expectedVersion) =>
      _$this._expectedVersion = expectedVersion;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ProjectTimesheetActionRequestBuilder() {
    ProjectTimesheetActionRequest._defaults(this);
  }

  ProjectTimesheetActionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedVersion = $v.expectedVersion;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectTimesheetActionRequest other) {
    _$v = other as _$ProjectTimesheetActionRequest;
  }

  @override
  void update(void Function(ProjectTimesheetActionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectTimesheetActionRequest build() => _build();

  _$ProjectTimesheetActionRequest _build() {
    final _$result = _$v ??
        _$ProjectTimesheetActionRequest._(
          expectedVersion: expectedVersion,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
