// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_price_override_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewPriceOverrideResponse
    extends EstimateReviewPriceOverrideResponse {
  @override
  final String? workItemCode;
  @override
  final String? workItemDescription;
  @override
  final double? fixedPriceUnitAmount;
  @override
  final String? reasonCode;

  factory _$EstimateReviewPriceOverrideResponse(
          [void Function(EstimateReviewPriceOverrideResponseBuilder)?
              updates]) =>
      (EstimateReviewPriceOverrideResponseBuilder()..update(updates))._build();

  _$EstimateReviewPriceOverrideResponse._(
      {this.workItemCode,
      this.workItemDescription,
      this.fixedPriceUnitAmount,
      this.reasonCode})
      : super._();
  @override
  EstimateReviewPriceOverrideResponse rebuild(
          void Function(EstimateReviewPriceOverrideResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewPriceOverrideResponseBuilder toBuilder() =>
      EstimateReviewPriceOverrideResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewPriceOverrideResponse &&
        workItemCode == other.workItemCode &&
        workItemDescription == other.workItemDescription &&
        fixedPriceUnitAmount == other.fixedPriceUnitAmount &&
        reasonCode == other.reasonCode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, workItemCode.hashCode);
    _$hash = $jc(_$hash, workItemDescription.hashCode);
    _$hash = $jc(_$hash, fixedPriceUnitAmount.hashCode);
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'EstimateReviewPriceOverrideResponse')
          ..add('workItemCode', workItemCode)
          ..add('workItemDescription', workItemDescription)
          ..add('fixedPriceUnitAmount', fixedPriceUnitAmount)
          ..add('reasonCode', reasonCode))
        .toString();
  }
}

class EstimateReviewPriceOverrideResponseBuilder
    implements
        Builder<EstimateReviewPriceOverrideResponse,
            EstimateReviewPriceOverrideResponseBuilder> {
  _$EstimateReviewPriceOverrideResponse? _$v;

  String? _workItemCode;
  String? get workItemCode => _$this._workItemCode;
  set workItemCode(String? workItemCode) => _$this._workItemCode = workItemCode;

  String? _workItemDescription;
  String? get workItemDescription => _$this._workItemDescription;
  set workItemDescription(String? workItemDescription) =>
      _$this._workItemDescription = workItemDescription;

  double? _fixedPriceUnitAmount;
  double? get fixedPriceUnitAmount => _$this._fixedPriceUnitAmount;
  set fixedPriceUnitAmount(double? fixedPriceUnitAmount) =>
      _$this._fixedPriceUnitAmount = fixedPriceUnitAmount;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  EstimateReviewPriceOverrideResponseBuilder() {
    EstimateReviewPriceOverrideResponse._defaults(this);
  }

  EstimateReviewPriceOverrideResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _workItemCode = $v.workItemCode;
      _workItemDescription = $v.workItemDescription;
      _fixedPriceUnitAmount = $v.fixedPriceUnitAmount;
      _reasonCode = $v.reasonCode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewPriceOverrideResponse other) {
    _$v = other as _$EstimateReviewPriceOverrideResponse;
  }

  @override
  void update(
      void Function(EstimateReviewPriceOverrideResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewPriceOverrideResponse build() => _build();

  _$EstimateReviewPriceOverrideResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewPriceOverrideResponse._(
          workItemCode: workItemCode,
          workItemDescription: workItemDescription,
          fixedPriceUnitAmount: fixedPriceUnitAmount,
          reasonCode: reasonCode,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
