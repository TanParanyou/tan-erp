// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_project_wbs_node_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateProjectWbsNodeRequest extends UpdateProjectWbsNodeRequest {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? expectedVersion;

  factory _$UpdateProjectWbsNodeRequest(
          [void Function(UpdateProjectWbsNodeRequestBuilder)? updates]) =>
      (UpdateProjectWbsNodeRequestBuilder()..update(updates))._build();

  _$UpdateProjectWbsNodeRequest._({this.code, this.name, this.expectedVersion})
      : super._();
  @override
  UpdateProjectWbsNodeRequest rebuild(
          void Function(UpdateProjectWbsNodeRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateProjectWbsNodeRequestBuilder toBuilder() =>
      UpdateProjectWbsNodeRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateProjectWbsNodeRequest &&
        code == other.code &&
        name == other.name &&
        expectedVersion == other.expectedVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, expectedVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateProjectWbsNodeRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('expectedVersion', expectedVersion))
        .toString();
  }
}

class UpdateProjectWbsNodeRequestBuilder
    implements
        Builder<UpdateProjectWbsNodeRequest,
            UpdateProjectWbsNodeRequestBuilder> {
  _$UpdateProjectWbsNodeRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _expectedVersion;
  String? get expectedVersion => _$this._expectedVersion;
  set expectedVersion(String? expectedVersion) =>
      _$this._expectedVersion = expectedVersion;

  UpdateProjectWbsNodeRequestBuilder() {
    UpdateProjectWbsNodeRequest._defaults(this);
  }

  UpdateProjectWbsNodeRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _expectedVersion = $v.expectedVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateProjectWbsNodeRequest other) {
    _$v = other as _$UpdateProjectWbsNodeRequest;
  }

  @override
  void update(void Function(UpdateProjectWbsNodeRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateProjectWbsNodeRequest build() => _build();

  _$UpdateProjectWbsNodeRequest _build() {
    final _$result = _$v ??
        _$UpdateProjectWbsNodeRequest._(
          code: code,
          name: name,
          expectedVersion: expectedVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
