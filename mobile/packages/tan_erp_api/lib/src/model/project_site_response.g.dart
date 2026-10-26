// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_site_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectSiteResponse extends ProjectSiteResponse {
  @override
  final String? id;
  @override
  final String? label;

  factory _$ProjectSiteResponse(
          [void Function(ProjectSiteResponseBuilder)? updates]) =>
      (ProjectSiteResponseBuilder()..update(updates))._build();

  _$ProjectSiteResponse._({this.id, this.label}) : super._();
  @override
  ProjectSiteResponse rebuild(
          void Function(ProjectSiteResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectSiteResponseBuilder toBuilder() =>
      ProjectSiteResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectSiteResponse &&
        id == other.id &&
        label == other.label;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, label.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectSiteResponse')
          ..add('id', id)
          ..add('label', label))
        .toString();
  }
}

class ProjectSiteResponseBuilder
    implements Builder<ProjectSiteResponse, ProjectSiteResponseBuilder> {
  _$ProjectSiteResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _label;
  String? get label => _$this._label;
  set label(String? label) => _$this._label = label;

  ProjectSiteResponseBuilder() {
    ProjectSiteResponse._defaults(this);
  }

  ProjectSiteResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _label = $v.label;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectSiteResponse other) {
    _$v = other as _$ProjectSiteResponse;
  }

  @override
  void update(void Function(ProjectSiteResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectSiteResponse build() => _build();

  _$ProjectSiteResponse _build() {
    final _$result = _$v ??
        _$ProjectSiteResponse._(
          id: id,
          label: label,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
