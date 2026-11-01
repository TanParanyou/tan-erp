// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_credit_exposure_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CustomerCreditExposureResponse extends CustomerCreditExposureResponse {
  @override
  final String? customerId;
  @override
  final String? customerName;
  @override
  final double? creditLimit;
  @override
  final int? creditTermDays;
  @override
  final double? outstandingReceivables;
  @override
  final double? unbilledAcceptedAmount;
  @override
  final double? totalExposure;
  @override
  final double? effectiveCreditCeiling;
  @override
  final double? availableCredit;
  @override
  final bool? isOverLimit;
  @override
  final BuiltList<CreditOverrideResponse>? activeOverrides;

  factory _$CustomerCreditExposureResponse(
          [void Function(CustomerCreditExposureResponseBuilder)? updates]) =>
      (CustomerCreditExposureResponseBuilder()..update(updates))._build();

  _$CustomerCreditExposureResponse._(
      {this.customerId,
      this.customerName,
      this.creditLimit,
      this.creditTermDays,
      this.outstandingReceivables,
      this.unbilledAcceptedAmount,
      this.totalExposure,
      this.effectiveCreditCeiling,
      this.availableCredit,
      this.isOverLimit,
      this.activeOverrides})
      : super._();
  @override
  CustomerCreditExposureResponse rebuild(
          void Function(CustomerCreditExposureResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CustomerCreditExposureResponseBuilder toBuilder() =>
      CustomerCreditExposureResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CustomerCreditExposureResponse &&
        customerId == other.customerId &&
        customerName == other.customerName &&
        creditLimit == other.creditLimit &&
        creditTermDays == other.creditTermDays &&
        outstandingReceivables == other.outstandingReceivables &&
        unbilledAcceptedAmount == other.unbilledAcceptedAmount &&
        totalExposure == other.totalExposure &&
        effectiveCreditCeiling == other.effectiveCreditCeiling &&
        availableCredit == other.availableCredit &&
        isOverLimit == other.isOverLimit &&
        activeOverrides == other.activeOverrides;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, customerId.hashCode);
    _$hash = $jc(_$hash, customerName.hashCode);
    _$hash = $jc(_$hash, creditLimit.hashCode);
    _$hash = $jc(_$hash, creditTermDays.hashCode);
    _$hash = $jc(_$hash, outstandingReceivables.hashCode);
    _$hash = $jc(_$hash, unbilledAcceptedAmount.hashCode);
    _$hash = $jc(_$hash, totalExposure.hashCode);
    _$hash = $jc(_$hash, effectiveCreditCeiling.hashCode);
    _$hash = $jc(_$hash, availableCredit.hashCode);
    _$hash = $jc(_$hash, isOverLimit.hashCode);
    _$hash = $jc(_$hash, activeOverrides.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CustomerCreditExposureResponse')
          ..add('customerId', customerId)
          ..add('customerName', customerName)
          ..add('creditLimit', creditLimit)
          ..add('creditTermDays', creditTermDays)
          ..add('outstandingReceivables', outstandingReceivables)
          ..add('unbilledAcceptedAmount', unbilledAcceptedAmount)
          ..add('totalExposure', totalExposure)
          ..add('effectiveCreditCeiling', effectiveCreditCeiling)
          ..add('availableCredit', availableCredit)
          ..add('isOverLimit', isOverLimit)
          ..add('activeOverrides', activeOverrides))
        .toString();
  }
}

class CustomerCreditExposureResponseBuilder
    implements
        Builder<CustomerCreditExposureResponse,
            CustomerCreditExposureResponseBuilder> {
  _$CustomerCreditExposureResponse? _$v;

  String? _customerId;
  String? get customerId => _$this._customerId;
  set customerId(String? customerId) => _$this._customerId = customerId;

  String? _customerName;
  String? get customerName => _$this._customerName;
  set customerName(String? customerName) => _$this._customerName = customerName;

  double? _creditLimit;
  double? get creditLimit => _$this._creditLimit;
  set creditLimit(double? creditLimit) => _$this._creditLimit = creditLimit;

  int? _creditTermDays;
  int? get creditTermDays => _$this._creditTermDays;
  set creditTermDays(int? creditTermDays) =>
      _$this._creditTermDays = creditTermDays;

  double? _outstandingReceivables;
  double? get outstandingReceivables => _$this._outstandingReceivables;
  set outstandingReceivables(double? outstandingReceivables) =>
      _$this._outstandingReceivables = outstandingReceivables;

  double? _unbilledAcceptedAmount;
  double? get unbilledAcceptedAmount => _$this._unbilledAcceptedAmount;
  set unbilledAcceptedAmount(double? unbilledAcceptedAmount) =>
      _$this._unbilledAcceptedAmount = unbilledAcceptedAmount;

  double? _totalExposure;
  double? get totalExposure => _$this._totalExposure;
  set totalExposure(double? totalExposure) =>
      _$this._totalExposure = totalExposure;

  double? _effectiveCreditCeiling;
  double? get effectiveCreditCeiling => _$this._effectiveCreditCeiling;
  set effectiveCreditCeiling(double? effectiveCreditCeiling) =>
      _$this._effectiveCreditCeiling = effectiveCreditCeiling;

  double? _availableCredit;
  double? get availableCredit => _$this._availableCredit;
  set availableCredit(double? availableCredit) =>
      _$this._availableCredit = availableCredit;

  bool? _isOverLimit;
  bool? get isOverLimit => _$this._isOverLimit;
  set isOverLimit(bool? isOverLimit) => _$this._isOverLimit = isOverLimit;

  ListBuilder<CreditOverrideResponse>? _activeOverrides;
  ListBuilder<CreditOverrideResponse> get activeOverrides =>
      _$this._activeOverrides ??= ListBuilder<CreditOverrideResponse>();
  set activeOverrides(ListBuilder<CreditOverrideResponse>? activeOverrides) =>
      _$this._activeOverrides = activeOverrides;

  CustomerCreditExposureResponseBuilder() {
    CustomerCreditExposureResponse._defaults(this);
  }

  CustomerCreditExposureResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _customerId = $v.customerId;
      _customerName = $v.customerName;
      _creditLimit = $v.creditLimit;
      _creditTermDays = $v.creditTermDays;
      _outstandingReceivables = $v.outstandingReceivables;
      _unbilledAcceptedAmount = $v.unbilledAcceptedAmount;
      _totalExposure = $v.totalExposure;
      _effectiveCreditCeiling = $v.effectiveCreditCeiling;
      _availableCredit = $v.availableCredit;
      _isOverLimit = $v.isOverLimit;
      _activeOverrides = $v.activeOverrides?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CustomerCreditExposureResponse other) {
    _$v = other as _$CustomerCreditExposureResponse;
  }

  @override
  void update(void Function(CustomerCreditExposureResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CustomerCreditExposureResponse build() => _build();

  _$CustomerCreditExposureResponse _build() {
    _$CustomerCreditExposureResponse _$result;
    try {
      _$result = _$v ??
          _$CustomerCreditExposureResponse._(
            customerId: customerId,
            customerName: customerName,
            creditLimit: creditLimit,
            creditTermDays: creditTermDays,
            outstandingReceivables: outstandingReceivables,
            unbilledAcceptedAmount: unbilledAcceptedAmount,
            totalExposure: totalExposure,
            effectiveCreditCeiling: effectiveCreditCeiling,
            availableCredit: availableCredit,
            isOverLimit: isOverLimit,
            activeOverrides: _activeOverrides?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'activeOverrides';
        _activeOverrides?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'CustomerCreditExposureResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
