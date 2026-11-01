// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'credit_override_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CreditOverrideRequest extends CreditOverrideRequest {
  @override
  final String? customerId;
  @override
  final double? requestedCeilingAmount;
  @override
  final String? reason;
  @override
  final Date? expiresOn;

  factory _$CreditOverrideRequest(
          [void Function(CreditOverrideRequestBuilder)? updates]) =>
      (CreditOverrideRequestBuilder()..update(updates))._build();

  _$CreditOverrideRequest._(
      {this.customerId,
      this.requestedCeilingAmount,
      this.reason,
      this.expiresOn})
      : super._();
  @override
  CreditOverrideRequest rebuild(
          void Function(CreditOverrideRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CreditOverrideRequestBuilder toBuilder() =>
      CreditOverrideRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CreditOverrideRequest &&
        customerId == other.customerId &&
        requestedCeilingAmount == other.requestedCeilingAmount &&
        reason == other.reason &&
        expiresOn == other.expiresOn;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, requestedCeilingAmount.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, expiresOn.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CreditOverrideRequest')
          ..add('customerId', customerId)
          ..add('requestedCeilingAmount', requestedCeilingAmount)
          ..add('reason', reason)
          ..add('expiresOn', expiresOn))
        .toString();
  }
}

class CreditOverrideRequestBuilder
    implements Builder<CreditOverrideRequest, CreditOverrideRequestBuilder> {
  _$CreditOverrideRequest? _$v;

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

  double? _requestedCeilingAmount;
  double? get requestedCeilingAmount => _$this._requestedCeilingAmount;
  set requestedCeilingAmount(double? requestedCeilingAmount) =>
      _$this._requestedCeilingAmount = requestedCeilingAmount;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  Date? _expiresOn;
  Date? get expiresOn => _$this._expiresOn;
  set expiresOn(Date? expiresOn) => _$this._expiresOn = expiresOn;

  CreditOverrideRequestBuilder() {
    CreditOverrideRequest._defaults(this);
  }

  CreditOverrideRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _customerId = $v.customerId;
      _requestedCeilingAmount = $v.requestedCeilingAmount;
      _reason = $v.reason;
      _expiresOn = $v.expiresOn;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CreditOverrideRequest other) {
    _$v = other as _$CreditOverrideRequest;
  }

  @override
  void update(void Function(CreditOverrideRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CreditOverrideRequest build() => _build();

  _$CreditOverrideRequest _build() {
    final _$result = _$v ??
        _$CreditOverrideRequest._(
          customerId: customerId,
          requestedCeilingAmount: requestedCeilingAmount,
          reason: reason,
          expiresOn: expiresOn,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
