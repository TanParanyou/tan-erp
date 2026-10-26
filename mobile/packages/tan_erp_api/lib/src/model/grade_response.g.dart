// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'grade_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$GradeResponse extends GradeResponse {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final double? factor;

  factory _$GradeResponse([void Function(GradeResponseBuilder)? updates]) =>
      (GradeResponseBuilder()..update(updates))._build();

  _$GradeResponse._({this.code, this.name, this.factor}) : super._();
  @override
  GradeResponse rebuild(void Function(GradeResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  GradeResponseBuilder toBuilder() => GradeResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is GradeResponse &&
        code == other.code &&
        name == other.name &&
        factor == other.factor;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, factor.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'GradeResponse')
          ..add('code', code)
          ..add('name', name)
          ..add('factor', factor))
        .toString();
  }
}

class GradeResponseBuilder
    implements Builder<GradeResponse, GradeResponseBuilder> {
  _$GradeResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  double? _factor;
  double? get factor => _$this._factor;
  set factor(double? factor) => _$this._factor = factor;

  GradeResponseBuilder() {
    GradeResponse._defaults(this);
  }

  GradeResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _factor = $v.factor;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(GradeResponse other) {
    _$v = other as _$GradeResponse;
  }

  @override
  void update(void Function(GradeResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  GradeResponse build() => _build();

  _$GradeResponse _build() {
    final _$result = _$v ??
        _$GradeResponse._(
          code: code,
          name: name,
          factor: factor,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
