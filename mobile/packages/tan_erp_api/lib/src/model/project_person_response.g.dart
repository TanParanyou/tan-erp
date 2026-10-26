// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_person_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectPersonResponse extends ProjectPersonResponse {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? email;

  factory _$ProjectPersonResponse(
          [void Function(ProjectPersonResponseBuilder)? updates]) =>
      (ProjectPersonResponseBuilder()..update(updates))._build();

  _$ProjectPersonResponse._({this.id, this.displayName, this.email})
      : super._();
  @override
  ProjectPersonResponse rebuild(
          void Function(ProjectPersonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectPersonResponseBuilder toBuilder() =>
      ProjectPersonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectPersonResponse &&
        id == other.id &&
        displayName == other.displayName &&
        email == other.email;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectPersonResponse')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('email', email))
        .toString();
  }
}

class ProjectPersonResponseBuilder
    implements Builder<ProjectPersonResponse, ProjectPersonResponseBuilder> {
  _$ProjectPersonResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  ProjectPersonResponseBuilder() {
    ProjectPersonResponse._defaults(this);
  }

  ProjectPersonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _email = $v.email;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectPersonResponse other) {
    _$v = other as _$ProjectPersonResponse;
  }

  @override
  void update(void Function(ProjectPersonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectPersonResponse build() => _build();

  _$ProjectPersonResponse _build() {
    final _$result = _$v ??
        _$ProjectPersonResponse._(
          id: id,
          displayName: displayName,
          email: email,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
