// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'complexity_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ComplexityRequest extends ComplexityRequest {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final double? factor;
  @override
  final double? riskModifier;

  factory _$ComplexityRequest(
          [void Function(ComplexityRequestBuilder)? updates]) =>
      (ComplexityRequestBuilder()..update(updates))._build();

  _$ComplexityRequest._({this.code, this.name, this.factor, this.riskModifier})
      : super._();
  @override
  ComplexityRequest rebuild(void Function(ComplexityRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ComplexityRequestBuilder toBuilder() =>
      ComplexityRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ComplexityRequest &&
        code == other.code &&
        name == other.name &&
        factor == other.factor &&
        riskModifier == other.riskModifier;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, factor.hashCode);
    _$hash = $jc(_$hash, riskModifier.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ComplexityRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('factor', factor)
          ..add('riskModifier', riskModifier))
        .toString();
  }
}

class ComplexityRequestBuilder
    implements Builder<ComplexityRequest, ComplexityRequestBuilder> {
  _$ComplexityRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  double? _factor;
  double? get factor => _$this._factor;
  set factor(double? factor) => _$this._factor = factor;

  double? _riskModifier;
  double? get riskModifier => _$this._riskModifier;
  set riskModifier(double? riskModifier) => _$this._riskModifier = riskModifier;

  ComplexityRequestBuilder() {
    ComplexityRequest._defaults(this);
  }

  ComplexityRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _factor = $v.factor;
      _riskModifier = $v.riskModifier;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ComplexityRequest other) {
    _$v = other as _$ComplexityRequest;
  }

  @override
  void update(void Function(ComplexityRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ComplexityRequest build() => _build();

  _$ComplexityRequest _build() {
    final _$result = _$v ??
        _$ComplexityRequest._(
          code: code,
          name: name,
          factor: factor,
          riskModifier: riskModifier,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
