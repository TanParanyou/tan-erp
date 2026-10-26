// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_change_order_action_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectChangeOrderActionRequest
    extends ProjectChangeOrderActionRequest {
  @override
  final String? expectedVersion;
  @override
  final String? note;

  factory _$ProjectChangeOrderActionRequest(
          [void Function(ProjectChangeOrderActionRequestBuilder)? updates]) =>
      (ProjectChangeOrderActionRequestBuilder()..update(updates))._build();

  _$ProjectChangeOrderActionRequest._({this.expectedVersion, this.note})
      : super._();
  @override
  ProjectChangeOrderActionRequest rebuild(
          void Function(ProjectChangeOrderActionRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectChangeOrderActionRequestBuilder toBuilder() =>
      ProjectChangeOrderActionRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectChangeOrderActionRequest &&
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
    return (newBuiltValueToStringHelper(r'ProjectChangeOrderActionRequest')
          ..add('expectedVersion', expectedVersion)
          ..add('note', note))
        .toString();
  }
}

class ProjectChangeOrderActionRequestBuilder
    implements
        Builder<ProjectChangeOrderActionRequest,
            ProjectChangeOrderActionRequestBuilder> {
  _$ProjectChangeOrderActionRequest? _$v;

  String? _expectedVersion;
  String? get expectedVersion => _$this._expectedVersion;
  set expectedVersion(String? expectedVersion) =>
      _$this._expectedVersion = expectedVersion;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ProjectChangeOrderActionRequestBuilder() {
    ProjectChangeOrderActionRequest._defaults(this);
  }

  ProjectChangeOrderActionRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedVersion = $v.expectedVersion;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectChangeOrderActionRequest other) {
    _$v = other as _$ProjectChangeOrderActionRequest;
  }

  @override
  void update(void Function(ProjectChangeOrderActionRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectChangeOrderActionRequest build() => _build();

  _$ProjectChangeOrderActionRequest _build() {
    final _$result = _$v ??
        _$ProjectChangeOrderActionRequest._(
          expectedVersion: expectedVersion,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
