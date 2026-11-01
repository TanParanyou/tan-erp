// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'add_project_wbs_node_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AddProjectWbsNodeRequest extends AddProjectWbsNodeRequest {
  @override
  final String? parentId;
  @override
  final String? code;
  @override
  final String? name;

  factory _$AddProjectWbsNodeRequest(
          [void Function(AddProjectWbsNodeRequestBuilder)? updates]) =>
      (AddProjectWbsNodeRequestBuilder()..update(updates))._build();

  _$AddProjectWbsNodeRequest._({this.parentId, this.code, this.name})
      : super._();
  @override
  AddProjectWbsNodeRequest rebuild(
          void Function(AddProjectWbsNodeRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AddProjectWbsNodeRequestBuilder toBuilder() =>
      AddProjectWbsNodeRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AddProjectWbsNodeRequest &&
        parentId == other.parentId &&
        code == other.code &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, parentId.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AddProjectWbsNodeRequest')
          ..add('parentId', parentId)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class AddProjectWbsNodeRequestBuilder
    implements
        Builder<AddProjectWbsNodeRequest, AddProjectWbsNodeRequestBuilder> {
  _$AddProjectWbsNodeRequest? _$v;

  String? _parentId;
  String? get parentId => _$this._parentId;
  set parentId(String? parentId) => _$this._parentId = parentId;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  AddProjectWbsNodeRequestBuilder() {
    AddProjectWbsNodeRequest._defaults(this);
  }

  AddProjectWbsNodeRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _parentId = $v.parentId;
      _code = $v.code;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AddProjectWbsNodeRequest other) {
    _$v = other as _$AddProjectWbsNodeRequest;
  }

  @override
  void update(void Function(AddProjectWbsNodeRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AddProjectWbsNodeRequest build() => _build();

  _$AddProjectWbsNodeRequest _build() {
    final _$result = _$v ??
        _$AddProjectWbsNodeRequest._(
          parentId: parentId,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
