// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'effective_add_on_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EffectiveAddOnResponse extends EffectiveAddOnResponse {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final bool? perLine;

  factory _$EffectiveAddOnResponse(
          [void Function(EffectiveAddOnResponseBuilder)? updates]) =>
      (EffectiveAddOnResponseBuilder()..update(updates))._build();

  _$EffectiveAddOnResponse._({this.code, this.name, this.perLine}) : super._();
  @override
  EffectiveAddOnResponse rebuild(
          void Function(EffectiveAddOnResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EffectiveAddOnResponseBuilder toBuilder() =>
      EffectiveAddOnResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EffectiveAddOnResponse &&
        code == other.code &&
        name == other.name &&
        perLine == other.perLine;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, perLine.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EffectiveAddOnResponse')
          ..add('code', code)
          ..add('name', name)
          ..add('perLine', perLine))
        .toString();
  }
}

class EffectiveAddOnResponseBuilder
    implements Builder<EffectiveAddOnResponse, EffectiveAddOnResponseBuilder> {
  _$EffectiveAddOnResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  bool? _perLine;
  bool? get perLine => _$this._perLine;
  set perLine(bool? perLine) => _$this._perLine = perLine;

  EffectiveAddOnResponseBuilder() {
    EffectiveAddOnResponse._defaults(this);
  }

  EffectiveAddOnResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _perLine = $v.perLine;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EffectiveAddOnResponse other) {
    _$v = other as _$EffectiveAddOnResponse;
  }

  @override
  void update(void Function(EffectiveAddOnResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EffectiveAddOnResponse build() => _build();

  _$EffectiveAddOnResponse _build() {
    final _$result = _$v ??
        _$EffectiveAddOnResponse._(
          code: code,
          name: name,
          perLine: perLine,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
