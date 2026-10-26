// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_readiness_reason_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReadinessReasonResponse
    extends EstimateReadinessReasonResponse {
  @override
  final String? code;
  @override
  final String? targetType;
  @override
  final String? targetId;
  @override
  final String? targetField;

  factory _$EstimateReadinessReasonResponse(
          [void Function(EstimateReadinessReasonResponseBuilder)? updates]) =>
      (EstimateReadinessReasonResponseBuilder()..update(updates))._build();

  _$EstimateReadinessReasonResponse._(
      {this.code, this.targetType, this.targetId, this.targetField})
      : super._();
  @override
  EstimateReadinessReasonResponse rebuild(
          void Function(EstimateReadinessReasonResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReadinessReasonResponseBuilder toBuilder() =>
      EstimateReadinessReasonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReadinessReasonResponse &&
        code == other.code &&
        targetType == other.targetType &&
        targetId == other.targetId &&
        targetField == other.targetField;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, targetType.hashCode);
    _$hash = $jc(_$hash, targetId.hashCode);
    _$hash = $jc(_$hash, targetField.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReadinessReasonResponse')
          ..add('code', code)
          ..add('targetType', targetType)
          ..add('targetId', targetId)
          ..add('targetField', targetField))
        .toString();
  }
}

class EstimateReadinessReasonResponseBuilder
    implements
        Builder<EstimateReadinessReasonResponse,
            EstimateReadinessReasonResponseBuilder> {
  _$EstimateReadinessReasonResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _targetType;
  String? get targetType => _$this._targetType;
  set targetType(String? targetType) => _$this._targetType = targetType;

  String? _targetId;
  String? get targetId => _$this._targetId;
  set targetId(String? targetId) => _$this._targetId = targetId;

  String? _targetField;
  String? get targetField => _$this._targetField;
  set targetField(String? targetField) => _$this._targetField = targetField;

  EstimateReadinessReasonResponseBuilder() {
    EstimateReadinessReasonResponse._defaults(this);
  }

  EstimateReadinessReasonResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _targetType = $v.targetType;
      _targetId = $v.targetId;
      _targetField = $v.targetField;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReadinessReasonResponse other) {
    _$v = other as _$EstimateReadinessReasonResponse;
  }

  @override
  void update(void Function(EstimateReadinessReasonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReadinessReasonResponse build() => _build();

  _$EstimateReadinessReasonResponse _build() {
    final _$result = _$v ??
        _$EstimateReadinessReasonResponse._(
          code: code,
          targetType: targetType,
          targetId: targetId,
          targetField: targetField,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
