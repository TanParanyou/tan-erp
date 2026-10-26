// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'finance_project_ref_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$FinanceProjectRefResponse extends FinanceProjectRefResponse {
  @override
  final String? id;
  @override
  final String? code;
  @override
  final String? name;

  factory _$FinanceProjectRefResponse(
          [void Function(FinanceProjectRefResponseBuilder)? updates]) =>
      (FinanceProjectRefResponseBuilder()..update(updates))._build();

  _$FinanceProjectRefResponse._({this.id, this.code, this.name}) : super._();
  @override
  FinanceProjectRefResponse rebuild(
          void Function(FinanceProjectRefResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  FinanceProjectRefResponseBuilder toBuilder() =>
      FinanceProjectRefResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is FinanceProjectRefResponse &&
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
    return (newBuiltValueToStringHelper(r'FinanceProjectRefResponse')
          ..add('id', id)
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class FinanceProjectRefResponseBuilder
    implements
        Builder<FinanceProjectRefResponse, FinanceProjectRefResponseBuilder> {
  _$FinanceProjectRefResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  FinanceProjectRefResponseBuilder() {
    FinanceProjectRefResponse._defaults(this);
  }

  FinanceProjectRefResponseBuilder get _$this {
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
  void replace(FinanceProjectRefResponse other) {
    _$v = other as _$FinanceProjectRefResponse;
  }

  @override
  void update(void Function(FinanceProjectRefResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  FinanceProjectRefResponse build() => _build();

  _$FinanceProjectRefResponse _build() {
    final _$result = _$v ??
        _$FinanceProjectRefResponse._(
          id: id,
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
