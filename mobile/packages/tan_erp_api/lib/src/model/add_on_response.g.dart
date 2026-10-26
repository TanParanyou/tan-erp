// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'add_on_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AddOnResponse extends AddOnResponse {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final double? amount;
  @override
  final bool? perLine;

  factory _$AddOnResponse([void Function(AddOnResponseBuilder)? updates]) =>
      (AddOnResponseBuilder()..update(updates))._build();

  _$AddOnResponse._({this.code, this.name, this.amount, this.perLine})
      : super._();
  @override
  AddOnResponse rebuild(void Function(AddOnResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AddOnResponseBuilder toBuilder() => AddOnResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AddOnResponse &&
        code == other.code &&
        name == other.name &&
        amount == other.amount &&
        perLine == other.perLine;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, amount.hashCode);
    _$hash = $jc(_$hash, perLine.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AddOnResponse')
          ..add('code', code)
          ..add('name', name)
          ..add('amount', amount)
          ..add('perLine', perLine))
        .toString();
  }
}

class AddOnResponseBuilder
    implements Builder<AddOnResponse, AddOnResponseBuilder> {
  _$AddOnResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  double? _amount;
  double? get amount => _$this._amount;
  set amount(double? amount) => _$this._amount = amount;

  bool? _perLine;
  bool? get perLine => _$this._perLine;
  set perLine(bool? perLine) => _$this._perLine = perLine;

  AddOnResponseBuilder() {
    AddOnResponse._defaults(this);
  }

  AddOnResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _amount = $v.amount;
      _perLine = $v.perLine;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AddOnResponse other) {
    _$v = other as _$AddOnResponse;
  }

  @override
  void update(void Function(AddOnResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AddOnResponse build() => _build();

  _$AddOnResponse _build() {
    final _$result = _$v ??
        _$AddOnResponse._(
          code: code,
          name: name,
          amount: amount,
          perLine: perLine,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
