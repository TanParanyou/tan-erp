// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'billing_line_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BillingLineRequest extends BillingLineRequest {
  @override
  final String? description;
  @override
  final double? taxBaseAmount;
  @override
  final double? vatRate;

  factory _$BillingLineRequest(
          [void Function(BillingLineRequestBuilder)? updates]) =>
      (BillingLineRequestBuilder()..update(updates))._build();

  _$BillingLineRequest._({this.description, this.taxBaseAmount, this.vatRate})
      : super._();
  @override
  BillingLineRequest rebuild(
          void Function(BillingLineRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BillingLineRequestBuilder toBuilder() =>
      BillingLineRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BillingLineRequest &&
        description == other.description &&
        taxBaseAmount == other.taxBaseAmount &&
        vatRate == other.vatRate;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, taxBaseAmount.hashCode);
    _$hash = $jc(_$hash, vatRate.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BillingLineRequest')
          ..add('description', description)
          ..add('taxBaseAmount', taxBaseAmount)
          ..add('vatRate', vatRate))
        .toString();
  }
}

class BillingLineRequestBuilder
    implements Builder<BillingLineRequest, BillingLineRequestBuilder> {
  _$BillingLineRequest? _$v;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  double? _taxBaseAmount;
  double? get taxBaseAmount => _$this._taxBaseAmount;
  set taxBaseAmount(double? taxBaseAmount) =>
      _$this._taxBaseAmount = taxBaseAmount;

  double? _vatRate;
  double? get vatRate => _$this._vatRate;
  set vatRate(double? vatRate) => _$this._vatRate = vatRate;

  BillingLineRequestBuilder() {
    BillingLineRequest._defaults(this);
  }

  BillingLineRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _description = $v.description;
      _taxBaseAmount = $v.taxBaseAmount;
      _vatRate = $v.vatRate;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BillingLineRequest other) {
    _$v = other as _$BillingLineRequest;
  }

  @override
  void update(void Function(BillingLineRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BillingLineRequest build() => _build();

  _$BillingLineRequest _build() {
    final _$result = _$v ??
        _$BillingLineRequest._(
          description: description,
          taxBaseAmount: taxBaseAmount,
          vatRate: vatRate,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
