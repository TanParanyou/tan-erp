// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'add_on_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AddOnRequest extends AddOnRequest {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final double? amount;
  @override
  final bool? perLine;

  factory _$AddOnRequest([void Function(AddOnRequestBuilder)? updates]) =>
      (AddOnRequestBuilder()..update(updates))._build();

  _$AddOnRequest._({this.code, this.name, this.amount, this.perLine})
      : super._();
  @override
  AddOnRequest rebuild(void Function(AddOnRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AddOnRequestBuilder toBuilder() => AddOnRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AddOnRequest &&
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
    return (newBuiltValueToStringHelper(r'AddOnRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('amount', amount)
          ..add('perLine', perLine))
        .toString();
  }
}

class AddOnRequestBuilder
    implements Builder<AddOnRequest, AddOnRequestBuilder> {
  _$AddOnRequest? _$v;

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

  AddOnRequestBuilder() {
    AddOnRequest._defaults(this);
  }

  AddOnRequestBuilder get _$this {
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
  void replace(AddOnRequest other) {
    _$v = other as _$AddOnRequest;
  }

  @override
  void update(void Function(AddOnRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AddOnRequest build() => _build();

  _$AddOnRequest _build() {
    final _$result = _$v ??
        _$AddOnRequest._(
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
