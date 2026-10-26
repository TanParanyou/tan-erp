// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_totals_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationTotalsResponse extends QuotationTotalsResponse {
  @override
  final double? subtotal;
  @override
  final String? discountType;
  @override
  final double? discountValue;
  @override
  final double? discountAmount;
  @override
  final double? netBeforeTax;
  @override
  final double? taxAmount;
  @override
  final double? grandTotal;

  factory _$QuotationTotalsResponse(
          [void Function(QuotationTotalsResponseBuilder)? updates]) =>
      (QuotationTotalsResponseBuilder()..update(updates))._build();

  _$QuotationTotalsResponse._(
      {this.subtotal,
      this.discountType,
      this.discountValue,
      this.discountAmount,
      this.netBeforeTax,
      this.taxAmount,
      this.grandTotal})
      : super._();
  @override
  QuotationTotalsResponse rebuild(
          void Function(QuotationTotalsResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationTotalsResponseBuilder toBuilder() =>
      QuotationTotalsResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationTotalsResponse &&
        subtotal == other.subtotal &&
        discountType == other.discountType &&
        discountValue == other.discountValue &&
        discountAmount == other.discountAmount &&
        netBeforeTax == other.netBeforeTax &&
        taxAmount == other.taxAmount &&
        grandTotal == other.grandTotal;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, subtotal.hashCode);
    _$hash = $jc(_$hash, discountType.hashCode);
    _$hash = $jc(_$hash, discountValue.hashCode);
    _$hash = $jc(_$hash, discountAmount.hashCode);
    _$hash = $jc(_$hash, netBeforeTax.hashCode);
    _$hash = $jc(_$hash, taxAmount.hashCode);
    _$hash = $jc(_$hash, grandTotal.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationTotalsResponse')
          ..add('subtotal', subtotal)
          ..add('discountType', discountType)
          ..add('discountValue', discountValue)
          ..add('discountAmount', discountAmount)
          ..add('netBeforeTax', netBeforeTax)
          ..add('taxAmount', taxAmount)
          ..add('grandTotal', grandTotal))
        .toString();
  }
}

class QuotationTotalsResponseBuilder
    implements
        Builder<QuotationTotalsResponse, QuotationTotalsResponseBuilder> {
  _$QuotationTotalsResponse? _$v;

  double? _subtotal;
  double? get subtotal => _$this._subtotal;
  set subtotal(double? subtotal) => _$this._subtotal = subtotal;

  String? _discountType;
  String? get discountType => _$this._discountType;
  set discountType(String? discountType) => _$this._discountType = discountType;

  double? _discountValue;
  double? get discountValue => _$this._discountValue;
  set discountValue(double? discountValue) =>
      _$this._discountValue = discountValue;

  double? _discountAmount;
  double? get discountAmount => _$this._discountAmount;
  set discountAmount(double? discountAmount) =>
      _$this._discountAmount = discountAmount;

  double? _netBeforeTax;
  double? get netBeforeTax => _$this._netBeforeTax;
  set netBeforeTax(double? netBeforeTax) => _$this._netBeforeTax = netBeforeTax;

  double? _taxAmount;
  double? get taxAmount => _$this._taxAmount;
  set taxAmount(double? taxAmount) => _$this._taxAmount = taxAmount;

  double? _grandTotal;
  double? get grandTotal => _$this._grandTotal;
  set grandTotal(double? grandTotal) => _$this._grandTotal = grandTotal;

  QuotationTotalsResponseBuilder() {
    QuotationTotalsResponse._defaults(this);
  }

  QuotationTotalsResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _subtotal = $v.subtotal;
      _discountType = $v.discountType;
      _discountValue = $v.discountValue;
      _discountAmount = $v.discountAmount;
      _netBeforeTax = $v.netBeforeTax;
      _taxAmount = $v.taxAmount;
      _grandTotal = $v.grandTotal;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationTotalsResponse other) {
    _$v = other as _$QuotationTotalsResponse;
  }

  @override
  void update(void Function(QuotationTotalsResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationTotalsResponse build() => _build();

  _$QuotationTotalsResponse _build() {
    final _$result = _$v ??
        _$QuotationTotalsResponse._(
          subtotal: subtotal,
          discountType: discountType,
          discountValue: discountValue,
          discountAmount: discountAmount,
          netBeforeTax: netBeforeTax,
          taxAmount: taxAmount,
          grandTotal: grandTotal,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
