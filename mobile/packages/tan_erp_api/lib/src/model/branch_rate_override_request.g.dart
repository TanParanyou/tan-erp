// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_rate_override_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchRateOverrideRequest extends BranchRateOverrideRequest {
  @override
  final double? referenceRate;
  @override
  final double? minimumCharge;
  @override
  final Date? effectiveFrom;
  @override
  final Date? effectiveTo;
  @override
  final String? reasonCode;
  @override
  final String? reasonDetail;
  @override
  final String? sourceReference;

  factory _$BranchRateOverrideRequest(
          [void Function(BranchRateOverrideRequestBuilder)? updates]) =>
      (BranchRateOverrideRequestBuilder()..update(updates))._build();

  _$BranchRateOverrideRequest._(
      {this.referenceRate,
      this.minimumCharge,
      this.effectiveFrom,
      this.effectiveTo,
      this.reasonCode,
      this.reasonDetail,
      this.sourceReference})
      : super._();
  @override
  BranchRateOverrideRequest rebuild(
          void Function(BranchRateOverrideRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchRateOverrideRequestBuilder toBuilder() =>
      BranchRateOverrideRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchRateOverrideRequest &&
        referenceRate == other.referenceRate &&
        minimumCharge == other.minimumCharge &&
        effectiveFrom == other.effectiveFrom &&
        effectiveTo == other.effectiveTo &&
        reasonCode == other.reasonCode &&
        reasonDetail == other.reasonDetail &&
        sourceReference == other.sourceReference;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, referenceRate.hashCode);
    _$hash = $jc(_$hash, minimumCharge.hashCode);
    _$hash = $jc(_$hash, effectiveFrom.hashCode);
    _$hash = $jc(_$hash, effectiveTo.hashCode);
    _$hash = $jc(_$hash, reasonCode.hashCode);
    _$hash = $jc(_$hash, reasonDetail.hashCode);
    _$hash = $jc(_$hash, sourceReference.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BranchRateOverrideRequest')
          ..add('referenceRate', referenceRate)
          ..add('minimumCharge', minimumCharge)
          ..add('effectiveFrom', effectiveFrom)
          ..add('effectiveTo', effectiveTo)
          ..add('reasonCode', reasonCode)
          ..add('reasonDetail', reasonDetail)
          ..add('sourceReference', sourceReference))
        .toString();
  }
}

class BranchRateOverrideRequestBuilder
    implements
        Builder<BranchRateOverrideRequest, BranchRateOverrideRequestBuilder> {
  _$BranchRateOverrideRequest? _$v;

  double? _referenceRate;
  double? get referenceRate => _$this._referenceRate;
  set referenceRate(double? referenceRate) =>
      _$this._referenceRate = referenceRate;

  double? _minimumCharge;
  double? get minimumCharge => _$this._minimumCharge;
  set minimumCharge(double? minimumCharge) =>
      _$this._minimumCharge = minimumCharge;

  Date? _effectiveFrom;
  Date? get effectiveFrom => _$this._effectiveFrom;
  set effectiveFrom(Date? effectiveFrom) =>
      _$this._effectiveFrom = effectiveFrom;

  Date? _effectiveTo;
  Date? get effectiveTo => _$this._effectiveTo;
  set effectiveTo(Date? effectiveTo) => _$this._effectiveTo = effectiveTo;

  String? _reasonCode;
  String? get reasonCode => _$this._reasonCode;
  set reasonCode(String? reasonCode) => _$this._reasonCode = reasonCode;

  String? _reasonDetail;
  String? get reasonDetail => _$this._reasonDetail;
  set reasonDetail(String? reasonDetail) => _$this._reasonDetail = reasonDetail;

  String? _sourceReference;
  String? get sourceReference => _$this._sourceReference;
  set sourceReference(String? sourceReference) =>
      _$this._sourceReference = sourceReference;

  BranchRateOverrideRequestBuilder() {
    BranchRateOverrideRequest._defaults(this);
  }

  BranchRateOverrideRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _referenceRate = $v.referenceRate;
      _minimumCharge = $v.minimumCharge;
      _effectiveFrom = $v.effectiveFrom;
      _effectiveTo = $v.effectiveTo;
      _reasonCode = $v.reasonCode;
      _reasonDetail = $v.reasonDetail;
      _sourceReference = $v.sourceReference;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchRateOverrideRequest other) {
    _$v = other as _$BranchRateOverrideRequest;
  }

  @override
  void update(void Function(BranchRateOverrideRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchRateOverrideRequest build() => _build();

  _$BranchRateOverrideRequest _build() {
    final _$result = _$v ??
        _$BranchRateOverrideRequest._(
          referenceRate: referenceRate,
          minimumCharge: minimumCharge,
          effectiveFrom: effectiveFrom,
          effectiveTo: effectiveTo,
          reasonCode: reasonCode,
          reasonDetail: reasonDetail,
          sourceReference: sourceReference,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
