// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'billing_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BillingLineResponse extends BillingLineResponse {
  @override
  final String? id;
  @override
  final int? lineNumber;
  @override
  final String? description;
  @override
  final double? taxBaseAmount;
  @override
  final double? vatRate;
  @override
  final double? vatAmount;
  @override
  final double? totalAmount;

  factory _$BillingLineResponse(
          [void Function(BillingLineResponseBuilder)? updates]) =>
      (BillingLineResponseBuilder()..update(updates))._build();

  _$BillingLineResponse._(
      {this.id,
      this.lineNumber,
      this.description,
      this.taxBaseAmount,
      this.vatRate,
      this.vatAmount,
      this.totalAmount})
      : super._();
  @override
  BillingLineResponse rebuild(
          void Function(BillingLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BillingLineResponseBuilder toBuilder() =>
      BillingLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BillingLineResponse &&
        id == other.id &&
        lineNumber == other.lineNumber &&
        description == other.description &&
        taxBaseAmount == other.taxBaseAmount &&
        vatRate == other.vatRate &&
        vatAmount == other.vatAmount &&
        totalAmount == other.totalAmount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, lineNumber.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, taxBaseAmount.hashCode);
    _$hash = $jc(_$hash, vatRate.hashCode);
    _$hash = $jc(_$hash, vatAmount.hashCode);
    _$hash = $jc(_$hash, totalAmount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BillingLineResponse')
          ..add('id', id)
          ..add('lineNumber', lineNumber)
          ..add('description', description)
          ..add('taxBaseAmount', taxBaseAmount)
          ..add('vatRate', vatRate)
          ..add('vatAmount', vatAmount)
          ..add('totalAmount', totalAmount))
        .toString();
  }
}

class BillingLineResponseBuilder
    implements Builder<BillingLineResponse, BillingLineResponseBuilder> {
  _$BillingLineResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _lineNumber;
  int? get lineNumber => _$this._lineNumber;
  set lineNumber(int? lineNumber) => _$this._lineNumber = lineNumber;

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

  double? _vatAmount;
  double? get vatAmount => _$this._vatAmount;
  set vatAmount(double? vatAmount) => _$this._vatAmount = vatAmount;

  double? _totalAmount;
  double? get totalAmount => _$this._totalAmount;
  set totalAmount(double? totalAmount) => _$this._totalAmount = totalAmount;

  BillingLineResponseBuilder() {
    BillingLineResponse._defaults(this);
  }

  BillingLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _lineNumber = $v.lineNumber;
      _description = $v.description;
      _taxBaseAmount = $v.taxBaseAmount;
      _vatRate = $v.vatRate;
      _vatAmount = $v.vatAmount;
      _totalAmount = $v.totalAmount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BillingLineResponse other) {
    _$v = other as _$BillingLineResponse;
  }

  @override
  void update(void Function(BillingLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BillingLineResponse build() => _build();

  _$BillingLineResponse _build() {
    final _$result = _$v ??
        _$BillingLineResponse._(
          id: id,
          lineNumber: lineNumber,
          description: description,
          taxBaseAmount: taxBaseAmount,
          vatRate: vatRate,
          vatAmount: vatAmount,
          totalAmount: totalAmount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
