// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_approval_limit_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateApprovalLimitRequest extends UpdateApprovalLimitRequest {
  @override
  final bool? isUnlimited;
  @override
  final double? maxAmount;
  @override
  final bool? isActive;

  factory _$UpdateApprovalLimitRequest(
          [void Function(UpdateApprovalLimitRequestBuilder)? updates]) =>
      (UpdateApprovalLimitRequestBuilder()..update(updates))._build();

  _$UpdateApprovalLimitRequest._(
      {this.isUnlimited, this.maxAmount, this.isActive})
      : super._();
  @override
  UpdateApprovalLimitRequest rebuild(
          void Function(UpdateApprovalLimitRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateApprovalLimitRequestBuilder toBuilder() =>
      UpdateApprovalLimitRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateApprovalLimitRequest &&
        isUnlimited == other.isUnlimited &&
        maxAmount == other.maxAmount &&
        isActive == other.isActive;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, isUnlimited.hashCode);
    _$hash = $jc(_$hash, maxAmount.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateApprovalLimitRequest')
          ..add('isUnlimited', isUnlimited)
          ..add('maxAmount', maxAmount)
          ..add('isActive', isActive))
        .toString();
  }
}

class UpdateApprovalLimitRequestBuilder
    implements
        Builder<UpdateApprovalLimitRequest, UpdateApprovalLimitRequestBuilder> {
  _$UpdateApprovalLimitRequest? _$v;

  bool? _isUnlimited;
  bool? get isUnlimited => _$this._isUnlimited;
  set isUnlimited(bool? isUnlimited) => _$this._isUnlimited = isUnlimited;

  double? _maxAmount;
  double? get maxAmount => _$this._maxAmount;
  set maxAmount(double? maxAmount) => _$this._maxAmount = maxAmount;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  UpdateApprovalLimitRequestBuilder() {
    UpdateApprovalLimitRequest._defaults(this);
  }

  UpdateApprovalLimitRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _isUnlimited = $v.isUnlimited;
      _maxAmount = $v.maxAmount;
      _isActive = $v.isActive;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateApprovalLimitRequest other) {
    _$v = other as _$UpdateApprovalLimitRequest;
  }

  @override
  void update(void Function(UpdateApprovalLimitRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateApprovalLimitRequest build() => _build();

  _$UpdateApprovalLimitRequest _build() {
    final _$result = _$v ??
        _$UpdateApprovalLimitRequest._(
          isUnlimited: isUnlimited,
          maxAmount: maxAmount,
          isActive: isActive,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
