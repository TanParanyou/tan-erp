// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'goods_receipt_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$GoodsReceiptSummaryResponse extends GoodsReceiptSummaryResponse {
  @override
  final String? id;
  @override
  final String? number;
  @override
  final DateTime? receivedAtUtc;
  @override
  final String? note;
  @override
  final ProcurementPersonResponse? receivedBy;
  @override
  final int? lineCount;
  @override
  final double? totalQuantity;
  @override
  final String? stockDocumentNumber;

  factory _$GoodsReceiptSummaryResponse(
          [void Function(GoodsReceiptSummaryResponseBuilder)? updates]) =>
      (GoodsReceiptSummaryResponseBuilder()..update(updates))._build();

  _$GoodsReceiptSummaryResponse._(
      {this.id,
      this.number,
      this.receivedAtUtc,
      this.note,
      this.receivedBy,
      this.lineCount,
      this.totalQuantity,
      this.stockDocumentNumber})
      : super._();
  @override
  GoodsReceiptSummaryResponse rebuild(
          void Function(GoodsReceiptSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  GoodsReceiptSummaryResponseBuilder toBuilder() =>
      GoodsReceiptSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is GoodsReceiptSummaryResponse &&
        id == other.id &&
        number == other.number &&
        receivedAtUtc == other.receivedAtUtc &&
        note == other.note &&
        receivedBy == other.receivedBy &&
        lineCount == other.lineCount &&
        totalQuantity == other.totalQuantity &&
        stockDocumentNumber == other.stockDocumentNumber;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, receivedAtUtc.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, receivedBy.hashCode);
    _$hash = $jc(_$hash, lineCount.hashCode);
    _$hash = $jc(_$hash, totalQuantity.hashCode);
    _$hash = $jc(_$hash, stockDocumentNumber.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'GoodsReceiptSummaryResponse')
          ..add('id', id)
          ..add('number', number)
          ..add('receivedAtUtc', receivedAtUtc)
          ..add('note', note)
          ..add('receivedBy', receivedBy)
          ..add('lineCount', lineCount)
          ..add('totalQuantity', totalQuantity)
          ..add('stockDocumentNumber', stockDocumentNumber))
        .toString();
  }
}

class GoodsReceiptSummaryResponseBuilder
    implements
        Builder<GoodsReceiptSummaryResponse,
            GoodsReceiptSummaryResponseBuilder> {
  _$GoodsReceiptSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  DateTime? _receivedAtUtc;
  DateTime? get receivedAtUtc => _$this._receivedAtUtc;
  set receivedAtUtc(DateTime? receivedAtUtc) =>
      _$this._receivedAtUtc = receivedAtUtc;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ProcurementPersonResponseBuilder? _receivedBy;
  ProcurementPersonResponseBuilder get receivedBy =>
      _$this._receivedBy ??= ProcurementPersonResponseBuilder();
  set receivedBy(ProcurementPersonResponseBuilder? receivedBy) =>
      _$this._receivedBy = receivedBy;

  int? _lineCount;
  int? get lineCount => _$this._lineCount;
  set lineCount(int? lineCount) => _$this._lineCount = lineCount;

  double? _totalQuantity;
  double? get totalQuantity => _$this._totalQuantity;
  set totalQuantity(double? totalQuantity) =>
      _$this._totalQuantity = totalQuantity;

  String? _stockDocumentNumber;
  String? get stockDocumentNumber => _$this._stockDocumentNumber;
  set stockDocumentNumber(String? stockDocumentNumber) =>
      _$this._stockDocumentNumber = stockDocumentNumber;

  GoodsReceiptSummaryResponseBuilder() {
    GoodsReceiptSummaryResponse._defaults(this);
  }

  GoodsReceiptSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _number = $v.number;
      _receivedAtUtc = $v.receivedAtUtc;
      _note = $v.note;
      _receivedBy = $v.receivedBy?.toBuilder();
      _lineCount = $v.lineCount;
      _totalQuantity = $v.totalQuantity;
      _stockDocumentNumber = $v.stockDocumentNumber;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(GoodsReceiptSummaryResponse other) {
    _$v = other as _$GoodsReceiptSummaryResponse;
  }

  @override
  void update(void Function(GoodsReceiptSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  GoodsReceiptSummaryResponse build() => _build();

  _$GoodsReceiptSummaryResponse _build() {
    _$GoodsReceiptSummaryResponse _$result;
    try {
      _$result = _$v ??
          _$GoodsReceiptSummaryResponse._(
            id: id,
            number: number,
            receivedAtUtc: receivedAtUtc,
            note: note,
            receivedBy: _receivedBy?.build(),
            lineCount: lineCount,
            totalQuantity: totalQuantity,
            stockDocumentNumber: stockDocumentNumber,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'receivedBy';
        _receivedBy?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'GoodsReceiptSummaryResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
