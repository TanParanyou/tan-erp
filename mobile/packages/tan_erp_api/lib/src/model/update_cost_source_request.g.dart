// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_cost_source_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateCostSourceRequest extends UpdateCostSourceRequest {
  @override
  final String code;
  @override
  final LocalizedTextInput name;
  @override
  final String? sourceType;

  factory _$UpdateCostSourceRequest(
          [void Function(UpdateCostSourceRequestBuilder)? updates]) =>
      (UpdateCostSourceRequestBuilder()..update(updates))._build();

  _$UpdateCostSourceRequest._(
      {required this.code, required this.name, this.sourceType})
      : super._();
  @override
  UpdateCostSourceRequest rebuild(
          void Function(UpdateCostSourceRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateCostSourceRequestBuilder toBuilder() =>
      UpdateCostSourceRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateCostSourceRequest &&
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
    return (newBuiltValueToStringHelper(r'UpdateCostSourceRequest')
          ..add('code', code)
          ..add('name', name)
          ..add('sourceType', sourceType))
        .toString();
  }
}

class UpdateCostSourceRequestBuilder
    implements
        Builder<UpdateCostSourceRequest, UpdateCostSourceRequestBuilder> {
  _$UpdateCostSourceRequest? _$v;

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

  UpdateCostSourceRequestBuilder() {
    UpdateCostSourceRequest._defaults(this);
  }

  UpdateCostSourceRequestBuilder get _$this {
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
  void replace(UpdateCostSourceRequest other) {
    _$v = other as _$UpdateCostSourceRequest;
  }

  @override
  void update(void Function(UpdateCostSourceRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateCostSourceRequest build() => _build();

  _$UpdateCostSourceRequest _build() {
    _$UpdateCostSourceRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateCostSourceRequest._(
            code: BuiltValueNullFieldError.checkNotNull(
                code, r'UpdateCostSourceRequest', 'code'),
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
            r'UpdateCostSourceRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
