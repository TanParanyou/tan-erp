// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_approval_trigger_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewApprovalTriggerResponse
    extends EstimateReviewApprovalTriggerResponse {
  @override
  final String? code;
  @override
  final double? actualValue;
  @override
  final double? thresholdValue;
  @override
  final String? unit;

  factory _$EstimateReviewApprovalTriggerResponse(
          [void Function(EstimateReviewApprovalTriggerResponseBuilder)?
              updates]) =>
      (EstimateReviewApprovalTriggerResponseBuilder()..update(updates))
          ._build();

  _$EstimateReviewApprovalTriggerResponse._(
      {this.code, this.actualValue, this.thresholdValue, this.unit})
      : super._();
  @override
  EstimateReviewApprovalTriggerResponse rebuild(
          void Function(EstimateReviewApprovalTriggerResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewApprovalTriggerResponseBuilder toBuilder() =>
      EstimateReviewApprovalTriggerResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewApprovalTriggerResponse &&
        code == other.code &&
        actualValue == other.actualValue &&
        thresholdValue == other.thresholdValue &&
        unit == other.unit;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, actualValue.hashCode);
    _$hash = $jc(_$hash, thresholdValue.hashCode);
    _$hash = $jc(_$hash, unit.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(
            r'EstimateReviewApprovalTriggerResponse')
          ..add('code', code)
          ..add('actualValue', actualValue)
          ..add('thresholdValue', thresholdValue)
          ..add('unit', unit))
        .toString();
  }
}

class EstimateReviewApprovalTriggerResponseBuilder
    implements
        Builder<EstimateReviewApprovalTriggerResponse,
            EstimateReviewApprovalTriggerResponseBuilder> {
  _$EstimateReviewApprovalTriggerResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  double? _actualValue;
  double? get actualValue => _$this._actualValue;
  set actualValue(double? actualValue) => _$this._actualValue = actualValue;

  double? _thresholdValue;
  double? get thresholdValue => _$this._thresholdValue;
  set thresholdValue(double? thresholdValue) =>
      _$this._thresholdValue = thresholdValue;

  String? _unit;
  String? get unit => _$this._unit;
  set unit(String? unit) => _$this._unit = unit;

  EstimateReviewApprovalTriggerResponseBuilder() {
    EstimateReviewApprovalTriggerResponse._defaults(this);
  }

  EstimateReviewApprovalTriggerResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _actualValue = $v.actualValue;
      _thresholdValue = $v.thresholdValue;
      _unit = $v.unit;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewApprovalTriggerResponse other) {
    _$v = other as _$EstimateReviewApprovalTriggerResponse;
  }

  @override
  void update(
      void Function(EstimateReviewApprovalTriggerResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewApprovalTriggerResponse build() => _build();

  _$EstimateReviewApprovalTriggerResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewApprovalTriggerResponse._(
          code: code,
          actualValue: actualValue,
          thresholdValue: thresholdValue,
          unit: unit,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
