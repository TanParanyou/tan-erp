// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_wbs_node_version_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectWbsNodeVersionRequest extends ProjectWbsNodeVersionRequest {
  @override
  final String? expectedVersion;

  factory _$ProjectWbsNodeVersionRequest(
          [void Function(ProjectWbsNodeVersionRequestBuilder)? updates]) =>
      (ProjectWbsNodeVersionRequestBuilder()..update(updates))._build();

  _$ProjectWbsNodeVersionRequest._({this.expectedVersion}) : super._();
  @override
  ProjectWbsNodeVersionRequest rebuild(
          void Function(ProjectWbsNodeVersionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectWbsNodeVersionRequestBuilder toBuilder() =>
      ProjectWbsNodeVersionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectWbsNodeVersionRequest &&
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
    return (newBuiltValueToStringHelper(r'ProjectWbsNodeVersionRequest')
          ..add('expectedVersion', expectedVersion))
        .toString();
  }
}

class ProjectWbsNodeVersionRequestBuilder
    implements
        Builder<ProjectWbsNodeVersionRequest,
            ProjectWbsNodeVersionRequestBuilder> {
  _$ProjectWbsNodeVersionRequest? _$v;

  String? _expectedVersion;
  String? get expectedVersion => _$this._expectedVersion;
  set expectedVersion(String? expectedVersion) =>
      _$this._expectedVersion = expectedVersion;

  ProjectWbsNodeVersionRequestBuilder() {
    ProjectWbsNodeVersionRequest._defaults(this);
  }

  ProjectWbsNodeVersionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedVersion = $v.expectedVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectWbsNodeVersionRequest other) {
    _$v = other as _$ProjectWbsNodeVersionRequest;
  }

  @override
  void update(void Function(ProjectWbsNodeVersionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectWbsNodeVersionRequest build() => _build();

  _$ProjectWbsNodeVersionRequest _build() {
    final _$result = _$v ??
        _$ProjectWbsNodeVersionRequest._(
          expectedVersion: expectedVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
