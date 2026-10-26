// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_milestone_version_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectMilestoneVersionRequest extends ProjectMilestoneVersionRequest {
  @override
  final String? expectedVersion;

  factory _$ProjectMilestoneVersionRequest(
          [void Function(ProjectMilestoneVersionRequestBuilder)? updates]) =>
      (ProjectMilestoneVersionRequestBuilder()..update(updates))._build();

  _$ProjectMilestoneVersionRequest._({this.expectedVersion}) : super._();
  @override
  ProjectMilestoneVersionRequest rebuild(
          void Function(ProjectMilestoneVersionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectMilestoneVersionRequestBuilder toBuilder() =>
      ProjectMilestoneVersionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectMilestoneVersionRequest &&
        expectedVersion == other.expectedVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectMilestoneVersionRequest')
          ..add('expectedVersion', expectedVersion))
        .toString();
  }
}

class ProjectMilestoneVersionRequestBuilder
    implements
        Builder<ProjectMilestoneVersionRequest,
            ProjectMilestoneVersionRequestBuilder> {
  _$ProjectMilestoneVersionRequest? _$v;

  String? _expectedVersion;
  String? get expectedVersion => _$this._expectedVersion;
  set expectedVersion(String? expectedVersion) =>
      _$this._expectedVersion = expectedVersion;

  ProjectMilestoneVersionRequestBuilder() {
    ProjectMilestoneVersionRequest._defaults(this);
  }

  ProjectMilestoneVersionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedVersion = $v.expectedVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectMilestoneVersionRequest other) {
    _$v = other as _$ProjectMilestoneVersionRequest;
  }

  @override
  void update(void Function(ProjectMilestoneVersionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectMilestoneVersionRequest build() => _build();

  _$ProjectMilestoneVersionRequest _build() {
    final _$result = _$v ??
        _$ProjectMilestoneVersionRequest._(
          expectedVersion: expectedVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
