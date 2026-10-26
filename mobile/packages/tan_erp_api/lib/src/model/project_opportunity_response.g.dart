// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_opportunity_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectOpportunityResponse extends ProjectOpportunityResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? title;

  factory _$ProjectOpportunityResponse(
          [void Function(ProjectOpportunityResponseBuilder)? updates]) =>
      (ProjectOpportunityResponseBuilder()..update(updates))._build();

  _$ProjectOpportunityResponse._({this.id, this.code, this.title}) : super._();
  @override
  ProjectOpportunityResponse rebuild(
          void Function(ProjectOpportunityResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectOpportunityResponseBuilder toBuilder() =>
      ProjectOpportunityResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectOpportunityResponse &&
        id == other.id &&
        code == other.code &&
        title == other.title;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, title.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectOpportunityResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('title', title))
        .toString();
  }
}

class ProjectOpportunityResponseBuilder
    implements
        Builder<ProjectOpportunityResponse, ProjectOpportunityResponseBuilder> {
  _$ProjectOpportunityResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _title;
  String? get title => _$this._title;
  set title(String? title) => _$this._title = title;

  ProjectOpportunityResponseBuilder() {
    ProjectOpportunityResponse._defaults(this);
  }

  ProjectOpportunityResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _title = $v.title;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectOpportunityResponse other) {
    _$v = other as _$ProjectOpportunityResponse;
  }

  @override
  void update(void Function(ProjectOpportunityResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectOpportunityResponse build() => _build();

  _$ProjectOpportunityResponse _build() {
    final _$result = _$v ??
        _$ProjectOpportunityResponse._(
          id: id,
          code: code,
          title: title,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
