// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'project_wbs_node_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ProjectWbsNodeResponse extends ProjectWbsNodeResponse {
  @override
  final String? id;
  @override
  final String? parentId;
  @override
  final String? code;
  @override
  final String? name;
  @override
  final int? level;
  @override
  final int? sortOrder;
  @override
  final String? rowVersion;

  factory _$ProjectWbsNodeResponse(
          [void Function(ProjectWbsNodeResponseBuilder)? updates]) =>
      (ProjectWbsNodeResponseBuilder()..update(updates))._build();

  _$ProjectWbsNodeResponse._(
      {this.id,
      this.parentId,
      this.code,
      this.name,
      this.level,
      this.sortOrder,
      this.rowVersion})
      : super._();
  @override
  ProjectWbsNodeResponse rebuild(
          void Function(ProjectWbsNodeResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ProjectWbsNodeResponseBuilder toBuilder() =>
      ProjectWbsNodeResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ProjectWbsNodeResponse &&
        id == other.id &&
        parentId == other.parentId &&
        code == other.code &&
        name == other.name &&
        level == other.level &&
        sortOrder == other.sortOrder &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, parentId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, level.hashCode);
    _$hash = $jc(_$hash, sortOrder.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ProjectWbsNodeResponse')
          ..add('id', id)
          ..add('parentId', parentId)
          ..add('code', code)
          ..add('name', name)
          ..add('level', level)
          ..add('sortOrder', sortOrder)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class ProjectWbsNodeResponseBuilder
    implements Builder<ProjectWbsNodeResponse, ProjectWbsNodeResponseBuilder> {
  _$ProjectWbsNodeResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _parentId;
  String? get parentId => _$this._parentId;
  set parentId(String? parentId) => _$this._parentId = parentId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  int? _level;
  int? get level => _$this._level;
  set level(int? level) => _$this._level = level;

  int? _sortOrder;
  int? get sortOrder => _$this._sortOrder;
  set sortOrder(int? sortOrder) => _$this._sortOrder = sortOrder;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  ProjectWbsNodeResponseBuilder() {
    ProjectWbsNodeResponse._defaults(this);
  }

  ProjectWbsNodeResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _parentId = $v.parentId;
      _code = $v.code;
      _name = $v.name;
      _level = $v.level;
      _sortOrder = $v.sortOrder;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ProjectWbsNodeResponse other) {
    _$v = other as _$ProjectWbsNodeResponse;
  }

  @override
  void update(void Function(ProjectWbsNodeResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ProjectWbsNodeResponse build() => _build();

  _$ProjectWbsNodeResponse _build() {
    final _$result = _$v ??
        _$ProjectWbsNodeResponse._(
          id: id,
          parentId: parentId,
          code: code,
          name: name,
          level: level,
          sortOrder: sortOrder,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
