// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'work_order_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$WorkOrderRefResponse extends WorkOrderRefResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$WorkOrderRefResponse(
          [void Function(WorkOrderRefResponseBuilder)? updates]) =>
      (WorkOrderRefResponseBuilder()..update(updates))._build();

  _$WorkOrderRefResponse._({this.id, this.code, this.name}) : super._();
  @override
  WorkOrderRefResponse rebuild(
          void Function(WorkOrderRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  WorkOrderRefResponseBuilder toBuilder() =>
      WorkOrderRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is WorkOrderRefResponse &&
        id == other.id &&
        code == other.code &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'WorkOrderRefResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class WorkOrderRefResponseBuilder
    implements Builder<WorkOrderRefResponse, WorkOrderRefResponseBuilder> {
  _$WorkOrderRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  WorkOrderRefResponseBuilder() {
    WorkOrderRefResponse._defaults(this);
  }

  WorkOrderRefResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _code = $v.code;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(WorkOrderRefResponse other) {
    _$v = other as _$WorkOrderRefResponse;
  }

  @override
  void update(void Function(WorkOrderRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  WorkOrderRefResponse build() => _build();

  _$WorkOrderRefResponse _build() {
    final _$result = _$v ??
        _$WorkOrderRefResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
