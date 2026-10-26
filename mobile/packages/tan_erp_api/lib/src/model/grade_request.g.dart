// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'grade_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$GradeRequest extends GradeRequest {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final double? factor;

  factory _$GradeRequest([void Function(GradeRequestBuilder)? updates]) =>
      (GradeRequestBuilder()..update(updates))._build();

  _$GradeRequest._({this.code, this.name, this.factor}) : super._();
  @override
  GradeRequest rebuild(void Function(GradeRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  GradeRequestBuilder toBuilder() => GradeRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is GradeRequest &&
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
    return (newBuiltValueToStringHelper(r'GradeRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('factor', factor))
        .toString();
  }
}

class GradeRequestBuilder
    implements Builder<GradeRequest, GradeRequestBuilder> {
  _$GradeRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  double? _factor;
  double? get factor => _$this._factor;
  set factor(double? factor) => _$this._factor = factor;

  GradeRequestBuilder() {
    GradeRequest._defaults(this);
  }

  GradeRequestBuilder get _$this {
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
  void replace(GradeRequest other) {
    _$v = other as _$GradeRequest;
  }

  @override
  void update(void Function(GradeRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  GradeRequest build() => _build();

  _$GradeRequest _build() {
    final _$result = _$v ??
        _$GradeRequest._(
          code: code,
          name: name,
          factor: factor,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
