// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'estimate_review_approval_thresholds_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$EstimateReviewApprovalThresholdsResponse
    extends EstimateReviewApprovalThresholdsResponse {
  @override
  final double? managerAmountLimit;
  @override
  final double? financialAmountLimit;
  @override
  final double? directorAmountLimit;
  @override
  final double? checkerMinimumMarginRate;
  @override
  final double? directorMinimumMarginRate;
  @override
  final double? discountLimitRate;

  factory _$EstimateReviewApprovalThresholdsResponse(
          [void Function(EstimateReviewApprovalThresholdsResponseBuilder)?
              updates]) =>
      (EstimateReviewApprovalThresholdsResponseBuilder()..update(updates))
          ._build();

  _$EstimateReviewApprovalThresholdsResponse._(
      {this.managerAmountLimit,
      this.financialAmountLimit,
      this.directorAmountLimit,
      this.checkerMinimumMarginRate,
      this.directorMinimumMarginRate,
      this.discountLimitRate})
      : super._();
  @override
  EstimateReviewApprovalThresholdsResponse rebuild(
          void Function(EstimateReviewApprovalThresholdsResponseBuilder)
              updates) =>
      (toBuilder()..update(updates)).build();

  @override
  EstimateReviewApprovalThresholdsResponseBuilder toBuilder() =>
      EstimateReviewApprovalThresholdsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is EstimateReviewApprovalThresholdsResponse &&
        managerAmountLimit == other.managerAmountLimit &&
        financialAmountLimit == other.financialAmountLimit &&
        directorAmountLimit == other.directorAmountLimit &&
        checkerMinimumMarginRate == other.checkerMinimumMarginRate &&
        directorMinimumMarginRate == other.directorMinimumMarginRate &&
        discountLimitRate == other.discountLimitRate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, managerAmountLimit.hashCode);
    _$hash = $jc(_$hash, financialAmountLimit.hashCode);
    _$hash = $jc(_$hash, directorAmountLimit.hashCode);
    _$hash = $jc(_$hash, checkerMinimumMarginRate.hashCode);
    _$hash = $jc(_$hash, directorMinimumMarginRate.hashCode);
    _$hash = $jc(_$hash, discountLimitRate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(
            r'EstimateReviewApprovalThresholdsResponse')
          ..add('managerAmountLimit', managerAmountLimit)
          ..add('financialAmountLimit', financialAmountLimit)
          ..add('directorAmountLimit', directorAmountLimit)
          ..add('checkerMinimumMarginRate', checkerMinimumMarginRate)
          ..add('directorMinimumMarginRate', directorMinimumMarginRate)
          ..add('discountLimitRate', discountLimitRate))
        .toString();
  }
}

class EstimateReviewApprovalThresholdsResponseBuilder
    implements
        Builder<EstimateReviewApprovalThresholdsResponse,
            EstimateReviewApprovalThresholdsResponseBuilder> {
  _$EstimateReviewApprovalThresholdsResponse? _$v;

  double? _managerAmountLimit;
  double? get managerAmountLimit => _$this._managerAmountLimit;
  set managerAmountLimit(double? managerAmountLimit) =>
      _$this._managerAmountLimit = managerAmountLimit;

  double? _financialAmountLimit;
  double? get financialAmountLimit => _$this._financialAmountLimit;
  set financialAmountLimit(double? financialAmountLimit) =>
      _$this._financialAmountLimit = financialAmountLimit;

  double? _directorAmountLimit;
  double? get directorAmountLimit => _$this._directorAmountLimit;
  set directorAmountLimit(double? directorAmountLimit) =>
      _$this._directorAmountLimit = directorAmountLimit;

  double? _checkerMinimumMarginRate;
  double? get checkerMinimumMarginRate => _$this._checkerMinimumMarginRate;
  set checkerMinimumMarginRate(double? checkerMinimumMarginRate) =>
      _$this._checkerMinimumMarginRate = checkerMinimumMarginRate;

  double? _directorMinimumMarginRate;
  double? get directorMinimumMarginRate => _$this._directorMinimumMarginRate;
  set directorMinimumMarginRate(double? directorMinimumMarginRate) =>
      _$this._directorMinimumMarginRate = directorMinimumMarginRate;

  double? _discountLimitRate;
  double? get discountLimitRate => _$this._discountLimitRate;
  set discountLimitRate(double? discountLimitRate) =>
      _$this._discountLimitRate = discountLimitRate;

  EstimateReviewApprovalThresholdsResponseBuilder() {
    EstimateReviewApprovalThresholdsResponse._defaults(this);
  }

  EstimateReviewApprovalThresholdsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _managerAmountLimit = $v.managerAmountLimit;
      _financialAmountLimit = $v.financialAmountLimit;
      _directorAmountLimit = $v.directorAmountLimit;
      _checkerMinimumMarginRate = $v.checkerMinimumMarginRate;
      _directorMinimumMarginRate = $v.directorMinimumMarginRate;
      _discountLimitRate = $v.discountLimitRate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(EstimateReviewApprovalThresholdsResponse other) {
    _$v = other as _$EstimateReviewApprovalThresholdsResponse;
  }

  @override
  void update(
      void Function(EstimateReviewApprovalThresholdsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  EstimateReviewApprovalThresholdsResponse build() => _build();

  _$EstimateReviewApprovalThresholdsResponse _build() {
    final _$result = _$v ??
        _$EstimateReviewApprovalThresholdsResponse._(
          managerAmountLimit: managerAmountLimit,
          financialAmountLimit: financialAmountLimit,
          directorAmountLimit: directorAmountLimit,
          checkerMinimumMarginRate: checkerMinimumMarginRate,
          directorMinimumMarginRate: directorMinimumMarginRate,
          discountLimitRate: discountLimitRate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
