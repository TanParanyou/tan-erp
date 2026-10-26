// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calculate_estimate_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CalculateEstimateRequest extends CalculateEstimateRequest {
  @override
  final String? expectedRevisionVersion;
  @override
  final double? discountAmount;
  @override
  final String? discountType;
  @override
  final double? discountValue;
  @override
  final String? discountReasonCode;

  factory _$CalculateEstimateRequest(
          [void Function(CalculateEstimateRequestBuilder)? updates]) =>
      (CalculateEstimateRequestBuilder()..update(updates))._build();

  _$CalculateEstimateRequest._(
      {this.expectedRevisionVersion,
      this.discountAmount,
      this.discountType,
      this.discountValue,
      this.discountReasonCode})
      : super._();
  @override
  CalculateEstimateRequest rebuild(
          void Function(CalculateEstimateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CalculateEstimateRequestBuilder toBuilder() =>
      CalculateEstimateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CalculateEstimateRequest &&
        expectedRevisionVersion == other.expectedRevisionVersion &&
        discountAmount == other.discountAmount &&
        discountType == other.discountType &&
        discountValue == other.discountValue &&
        discountReasonCode == other.discountReasonCode;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, expectedRevisionVersion.hashCode);
    _$hash = $jc(_$hash, discountAmount.hashCode);
    _$hash = $jc(_$hash, discountType.hashCode);
    _$hash = $jc(_$hash, discountValue.hashCode);
    _$hash = $jc(_$hash, discountReasonCode.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CalculateEstimateRequest')
          ..add('expectedRevisionVersion', expectedRevisionVersion)
          ..add('discountAmount', discountAmount)
          ..add('discountType', discountType)
          ..add('discountValue', discountValue)
          ..add('discountReasonCode', discountReasonCode))
        .toString();
  }
}

class CalculateEstimateRequestBuilder
    implements
        Builder<CalculateEstimateRequest, CalculateEstimateRequestBuilder> {
  _$CalculateEstimateRequest? _$v;

  String? _expectedRevisionVersion;
  String? get expectedRevisionVersion => _$this._expectedRevisionVersion;
  set expectedRevisionVersion(String? expectedRevisionVersion) =>
      _$this._expectedRevisionVersion = expectedRevisionVersion;

  double? _discountAmount;
  double? get discountAmount => _$this._discountAmount;
  set discountAmount(double? discountAmount) =>
      _$this._discountAmount = discountAmount;

  String? _discountType;
  String? get discountType => _$this._discountType;
  set discountType(String? discountType) => _$this._discountType = discountType;

  double? _discountValue;
  double? get discountValue => _$this._discountValue;
  set discountValue(double? discountValue) =>
      _$this._discountValue = discountValue;

  String? _discountReasonCode;
  String? get discountReasonCode => _$this._discountReasonCode;
  set discountReasonCode(String? discountReasonCode) =>
      _$this._discountReasonCode = discountReasonCode;

  CalculateEstimateRequestBuilder() {
    CalculateEstimateRequest._defaults(this);
  }

  CalculateEstimateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _expectedRevisionVersion = $v.expectedRevisionVersion;
      _discountAmount = $v.discountAmount;
      _discountType = $v.discountType;
      _discountValue = $v.discountValue;
      _discountReasonCode = $v.discountReasonCode;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CalculateEstimateRequest other) {
    _$v = other as _$CalculateEstimateRequest;
  }

  @override
  void update(void Function(CalculateEstimateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CalculateEstimateRequest build() => _build();

  _$CalculateEstimateRequest _build() {
    final _$result = _$v ??
        _$CalculateEstimateRequest._(
          expectedRevisionVersion: expectedRevisionVersion,
          discountAmount: discountAmount,
          discountType: discountType,
          discountValue: discountValue,
          discountReasonCode: discountReasonCode,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
