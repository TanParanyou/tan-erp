// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_return_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierReturnResponse extends SupplierReturnResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final String? goodsReceiptId;
  @override
  final String? goodsReceiptNumber;
  @override
  final DateTime? returnedAtUtc;
  @override
  final String? reason;
  @override
  final double? totalReturnAmount;
  @override
  final String? stockDocumentNumber;
  @override
  final ProcurementPersonResponse? returnedBy;
  @override
  final BuiltList<SupplierReturnLineResponse>? lines;

  factory _$SupplierReturnResponse(
          [void Function(SupplierReturnResponseBuilder)? updates]) =>
      (SupplierReturnResponseBuilder()..update(updates))._build();

  _$SupplierReturnResponse._(
      {this.id,
      this.number,
      this.goodsReceiptId,
      this.goodsReceiptNumber,
      this.returnedAtUtc,
      this.reason,
      this.totalReturnAmount,
      this.stockDocumentNumber,
      this.returnedBy,
      this.lines})
      : super._();
  @override
  SupplierReturnResponse rebuild(
          void Function(SupplierReturnResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierReturnResponseBuilder toBuilder() =>
      SupplierReturnResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierReturnResponse &&
        id == other.id &&
        number == other.number &&
        goodsReceiptId == other.goodsReceiptId &&
        goodsReceiptNumber == other.goodsReceiptNumber &&
        returnedAtUtc == other.returnedAtUtc &&
        reason == other.reason &&
        totalReturnAmount == other.totalReturnAmount &&
        stockDocumentNumber == other.stockDocumentNumber &&
        returnedBy == other.returnedBy &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, goodsReceiptId.hashCode);
    _$hash = $jc(_$hash, goodsReceiptNumber.hashCode);
    _$hash = $jc(_$hash, returnedAtUtc.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, totalReturnAmount.hashCode);
    _$hash = $jc(_$hash, stockDocumentNumber.hashCode);
    _$hash = $jc(_$hash, returnedBy.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierReturnResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('goodsReceiptId', goodsReceiptId)
          ..add('goodsReceiptNumber', goodsReceiptNumber)
          ..add('returnedAtUtc', returnedAtUtc)
          ..add('reason', reason)
          ..add('totalReturnAmount', totalReturnAmount)
          ..add('stockDocumentNumber', stockDocumentNumber)
          ..add('returnedBy', returnedBy)
          ..add('lines', lines))
        .toString();
  }
}

class SupplierReturnResponseBuilder
    implements Builder<SupplierReturnResponse, SupplierReturnResponseBuilder> {
  _$SupplierReturnResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  String? _goodsReceiptId;
  String? get goodsReceiptId => _$this._goodsReceiptId;
  set goodsReceiptId(String? goodsReceiptId) =>
      _$this._goodsReceiptId = goodsReceiptId;

  String? _goodsReceiptNumber;
  String? get goodsReceiptNumber => _$this._goodsReceiptNumber;
  set goodsReceiptNumber(String? goodsReceiptNumber) =>
      _$this._goodsReceiptNumber = goodsReceiptNumber;

  DateTime? _returnedAtUtc;
  DateTime? get returnedAtUtc => _$this._returnedAtUtc;
  set returnedAtUtc(DateTime? returnedAtUtc) =>
      _$this._returnedAtUtc = returnedAtUtc;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  double? _totalReturnAmount;
  double? get totalReturnAmount => _$this._totalReturnAmount;
  set totalReturnAmount(double? totalReturnAmount) =>
      _$this._totalReturnAmount = totalReturnAmount;

  String? _stockDocumentNumber;
  String? get stockDocumentNumber => _$this._stockDocumentNumber;
  set stockDocumentNumber(String? stockDocumentNumber) =>
      _$this._stockDocumentNumber = stockDocumentNumber;

  ProcurementPersonResponseBuilder? _returnedBy;
  ProcurementPersonResponseBuilder get returnedBy =>
      _$this._returnedBy ??= ProcurementPersonResponseBuilder();
  set returnedBy(ProcurementPersonResponseBuilder? returnedBy) =>
      _$this._returnedBy = returnedBy;

  ListBuilder<SupplierReturnLineResponse>? _lines;
  ListBuilder<SupplierReturnLineResponse> get lines =>
      _$this._lines ??= ListBuilder<SupplierReturnLineResponse>();
  set lines(ListBuilder<SupplierReturnLineResponse>? lines) =>
      _$this._lines = lines;

  SupplierReturnResponseBuilder() {
    SupplierReturnResponse._defaults(this);
  }

  SupplierReturnResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _goodsReceiptId = $v.goodsReceiptId;
      _goodsReceiptNumber = $v.goodsReceiptNumber;
      _returnedAtUtc = $v.returnedAtUtc;
      _reason = $v.reason;
      _totalReturnAmount = $v.totalReturnAmount;
      _stockDocumentNumber = $v.stockDocumentNumber;
      _returnedBy = $v.returnedBy?.toBuilder();
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierReturnResponse other) {
    _$v = other as _$SupplierReturnResponse;
  }

  @override
  void update(void Function(SupplierReturnResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierReturnResponse build() => _build();

  _$SupplierReturnResponse _build() {
    _$SupplierReturnResponse _$result;
    try {
      _$result = _$v ??
          _$SupplierReturnResponse._(
            id: id,
            number: number,
            goodsReceiptId: goodsReceiptId,
            goodsReceiptNumber: goodsReceiptNumber,
            returnedAtUtc: returnedAtUtc,
            reason: reason,
            totalReturnAmount: totalReturnAmount,
            stockDocumentNumber: stockDocumentNumber,
            returnedBy: _returnedBy?.build(),
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'returnedBy';
        _returnedBy?.build();
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SupplierReturnResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
