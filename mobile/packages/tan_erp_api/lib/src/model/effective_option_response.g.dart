// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'effective_option_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EffectiveOptionResponse extends EffectiveOptionResponse {
  @override
  final String? code;
  @override
  final String? name;

  factory _$EffectiveOptionResponse(
          [void Function(EffectiveOptionResponseBuilder)? updates]) =>
      (EffectiveOptionResponseBuilder()..update(updates))._build();

  _$EffectiveOptionResponse._({this.code, this.name}) : super._();
  @override
  EffectiveOptionResponse rebuild(
          void Function(EffectiveOptionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EffectiveOptionResponseBuilder toBuilder() =>
      EffectiveOptionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EffectiveOptionResponse &&
        code == other.code &&
        name == other.name;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EffectiveOptionResponse')
          ..add('code', code)
          ..add('name', name))
        .toString();
  }
}

class EffectiveOptionResponseBuilder
    implements
        Builder<EffectiveOptionResponse, EffectiveOptionResponseBuilder> {
  _$EffectiveOptionResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  EffectiveOptionResponseBuilder() {
    EffectiveOptionResponse._defaults(this);
  }

  EffectiveOptionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EffectiveOptionResponse other) {
    _$v = other as _$EffectiveOptionResponse;
  }

  @override
  void update(void Function(EffectiveOptionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EffectiveOptionResponse build() => _build();

  _$EffectiveOptionResponse _build() {
    final _$result = _$v ??
        _$EffectiveOptionResponse._(
          code: code,
          name: name,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
