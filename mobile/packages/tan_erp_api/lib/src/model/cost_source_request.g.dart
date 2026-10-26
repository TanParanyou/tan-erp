// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cost_source_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CostSourceRequest extends CostSourceRequest {
  @override
  final String? code;
  @override
  final LocalizedTextInput name;
  @override
  final String? sourceType;

  factory _$CostSourceRequest(
          [void Function(CostSourceRequestBuilder)? updates]) =>
      (CostSourceRequestBuilder()..update(updates))._build();

  _$CostSourceRequest._({this.code, required this.name, this.sourceType})
      : super._();
  @override
  CostSourceRequest rebuild(void Function(CostSourceRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CostSourceRequestBuilder toBuilder() =>
      CostSourceRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CostSourceRequest &&
        code == other.code &&
        name == other.name &&
        sourceType == other.sourceType;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, sourceType.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CostSourceRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('sourceType', sourceType))
        .toString();
  }
}

class CostSourceRequestBuilder
    implements Builder<CostSourceRequest, CostSourceRequestBuilder> {
  _$CostSourceRequest? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  LocalizedTextInputBuilder? _name;
  LocalizedTextInputBuilder get name =>
      _$this._name ??= LocalizedTextInputBuilder();
  set name(LocalizedTextInputBuilder? name) => _$this._name = name;

  String? _sourceType;
  String? get sourceType => _$this._sourceType;
  set sourceType(String? sourceType) => _$this._sourceType = sourceType;

  CostSourceRequestBuilder() {
    CostSourceRequest._defaults(this);
  }

  CostSourceRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name.toBuilder();
      _sourceType = $v.sourceType;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CostSourceRequest other) {
    _$v = other as _$CostSourceRequest;
  }

  @override
  void update(void Function(CostSourceRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CostSourceRequest build() => _build();

  _$CostSourceRequest _build() {
    _$CostSourceRequest _$result;
    try {
      _$result = _$v ??
          _$CostSourceRequest._(
            code: code,
            name: name.build(),
            sourceType: sourceType,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'name';
        name.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CostSourceRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
