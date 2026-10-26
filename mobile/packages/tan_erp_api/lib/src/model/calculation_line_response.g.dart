// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'calculation_line_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CalculationLineResponse extends CalculationLineResponse {
  @override
  final String? lineId;
  @override
  final String? workSubtype;
  @override
  final double? billableQuantity;
  @override
  final double? baseAmount;
  @override
  final double? adjustedAmount;

  factory _$CalculationLineResponse(
          [void Function(CalculationLineResponseBuilder)? updates]) =>
      (CalculationLineResponseBuilder()..update(updates))._build();

  _$CalculationLineResponse._(
      {this.lineId,
      this.workSubtype,
      this.billableQuantity,
      this.baseAmount,
      this.adjustedAmount})
      : super._();
  @override
  CalculationLineResponse rebuild(
          void Function(CalculationLineResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CalculationLineResponseBuilder toBuilder() =>
      CalculationLineResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CalculationLineResponse &&
        lineId == other.lineId &&
        workSubtype == other.workSubtype &&
        billableQuantity == other.billableQuantity &&
        baseAmount == other.baseAmount &&
        adjustedAmount == other.adjustedAmount;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, lineId.hashCode);
    _$hash = $jc(_$hash, workSubtype.hashCode);
    _$hash = $jc(_$hash, billableQuantity.hashCode);
    _$hash = $jc(_$hash, baseAmount.hashCode);
    _$hash = $jc(_$hash, adjustedAmount.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CalculationLineResponse')
          ..add('lineId', lineId)
          ..add('workSubtype', workSubtype)
          ..add('billableQuantity', billableQuantity)
          ..add('baseAmount', baseAmount)
          ..add('adjustedAmount', adjustedAmount))
        .toString();
  }
}

class CalculationLineResponseBuilder
    implements
        Builder<CalculationLineResponse, CalculationLineResponseBuilder> {
  _$CalculationLineResponse? _$v;

  String? _lineId;
  String? get lineId => _$this._lineId;
  set lineId(String? lineId) => _$this._lineId = lineId;

  String? _workSubtype;
  String? get workSubtype => _$this._workSubtype;
  set workSubtype(String? workSubtype) => _$this._workSubtype = workSubtype;

  double? _billableQuantity;
  double? get billableQuantity => _$this._billableQuantity;
  set billableQuantity(double? billableQuantity) =>
      _$this._billableQuantity = billableQuantity;

  double? _baseAmount;
  double? get baseAmount => _$this._baseAmount;
  set baseAmount(double? baseAmount) => _$this._baseAmount = baseAmount;

  double? _adjustedAmount;
  double? get adjustedAmount => _$this._adjustedAmount;
  set adjustedAmount(double? adjustedAmount) =>
      _$this._adjustedAmount = adjustedAmount;

  CalculationLineResponseBuilder() {
    CalculationLineResponse._defaults(this);
  }

  CalculationLineResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _lineId = $v.lineId;
      _workSubtype = $v.workSubtype;
      _billableQuantity = $v.billableQuantity;
      _baseAmount = $v.baseAmount;
      _adjustedAmount = $v.adjustedAmount;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CalculationLineResponse other) {
    _$v = other as _$CalculationLineResponse;
  }

  @override
  void update(void Function(CalculationLineResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CalculationLineResponse build() => _build();

  _$CalculationLineResponse _build() {
    final _$result = _$v ??
        _$CalculationLineResponse._(
          lineId: lineId,
          workSubtype: workSubtype,
          billableQuantity: billableQuantity,
          baseAmount: baseAmount,
          adjustedAmount: adjustedAmount,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
