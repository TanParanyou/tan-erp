// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_readiness_reason_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewReadinessReasonResponse
    extends EstimateReviewReadinessReasonResponse {
  @override
  final String? code;
  @override
  final String? targetType;
  @override
  final String? targetId;
  @override
  final String? targetField;

  factory _$EstimateReviewReadinessReasonResponse(
          [void Function(EstimateReviewReadinessReasonResponseBuilder)?
              updates]) =>
      (EstimateReviewReadinessReasonResponseBuilder()..update(updates))
          ._build();

  _$EstimateReviewReadinessReasonResponse._(
      {this.code, this.targetType, this.targetId, this.targetField})
      : super._();
  @override
  EstimateReviewReadinessReasonResponse rebuild(
          void Function(EstimateReviewReadinessReasonResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewReadinessReasonResponseBuilder toBuilder() =>
      EstimateReviewReadinessReasonResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewReadinessReasonResponse &&
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
    return (newBuiltValueToStringHelper(
            r'EstimateReviewReadinessReasonResponse')
          ..add('code', code)
          ..add('targetType', targetType)
          ..add('targetId', targetId)
          ..add('targetField', targetField))
        .toString();
  }
}

class EstimateReviewReadinessReasonResponseBuilder
    implements
        Builder<EstimateReviewReadinessReasonResponse,
            EstimateReviewReadinessReasonResponseBuilder> {
  _$EstimateReviewReadinessReasonResponse? _$v;

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

  EstimateReviewReadinessReasonResponseBuilder() {
    EstimateReviewReadinessReasonResponse._defaults(this);
  }

  EstimateReviewReadinessReasonResponseBuilder get _$this {
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
  void replace(EstimateReviewReadinessReasonResponse other) {
    _$v = other as _$EstimateReviewReadinessReasonResponse;
  }

  @override
  void update(
      void Function(EstimateReviewReadinessReasonResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewReadinessReasonResponse build() => _build();

  _$EstimateReviewReadinessReasonResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewReadinessReasonResponse._(
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
