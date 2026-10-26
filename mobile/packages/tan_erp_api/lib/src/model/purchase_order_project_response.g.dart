// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_order_project_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$PurchaseOrderProjectResponse extends PurchaseOrderProjectResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$PurchaseOrderProjectResponse(
          [void Function(PurchaseOrderProjectResponseBuilder)? updates]) =>
      (PurchaseOrderProjectResponseBuilder()..update(updates))._build();

  _$PurchaseOrderProjectResponse._({this.id, this.code, this.name}) : super._();
  @override
  PurchaseOrderProjectResponse rebuild(
          void Function(PurchaseOrderProjectResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  PurchaseOrderProjectResponseBuilder toBuilder() =>
      PurchaseOrderProjectResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is PurchaseOrderProjectResponse &&
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
    return (newBuiltValueToStringHelper(r'PurchaseOrderProjectResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class PurchaseOrderProjectResponseBuilder
    implements
        Builder<PurchaseOrderProjectResponse,
            PurchaseOrderProjectResponseBuilder> {
  _$PurchaseOrderProjectResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  PurchaseOrderProjectResponseBuilder() {
    PurchaseOrderProjectResponse._defaults(this);
  }

  PurchaseOrderProjectResponseBuilder get _$this {
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
  void replace(PurchaseOrderProjectResponse other) {
    _$v = other as _$PurchaseOrderProjectResponse;
  }

  @override
  void update(void Function(PurchaseOrderProjectResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  PurchaseOrderProjectResponse build() => _build();

  _$PurchaseOrderProjectResponse _build() {
    final _$result = _$v ??
        _$PurchaseOrderProjectResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
