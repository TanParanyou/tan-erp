// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'inventory_period_close_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$InventoryPeriodCloseResponse extends InventoryPeriodCloseResponse {
  @override
  final String? id;
  @override
  final int? periodYear;
  @override
  final int? periodMonth;
  @override
  final Date? periodEndDate;
  @override
  final String? note;
  @override
  final int? lineCount;
  @override
  final double? totalOnHandQuantity;
  @override
  final double? totalValuationAmount;
  @override
  final InventoryPersonResponse? closedBy;
  @override
  final DateTime? closedAtUtc;
  @override
  final BuiltList<InventoryPeriodCloseLineResponse>? lines;

  factory _$InventoryPeriodCloseResponse(
          [void Function(InventoryPeriodCloseResponseBuilder)? updates]) =>
      (InventoryPeriodCloseResponseBuilder()..update(updates))._build();

  _$InventoryPeriodCloseResponse._(
      {this.id,
      this.periodYear,
      this.periodMonth,
      this.periodEndDate,
      this.note,
      this.lineCount,
      this.totalOnHandQuantity,
      this.totalValuationAmount,
      this.closedBy,
      this.closedAtUtc,
      this.lines})
      : super._();
  @override
  InventoryPeriodCloseResponse rebuild(
          void Function(InventoryPeriodCloseResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  InventoryPeriodCloseResponseBuilder toBuilder() =>
      InventoryPeriodCloseResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is InventoryPeriodCloseResponse &&
        id == other.id &&
        periodYear == other.periodYear &&
        periodMonth == other.periodMonth &&
        periodEndDate == other.periodEndDate &&
        note == other.note &&
        lineCount == other.lineCount &&
        totalOnHandQuantity == other.totalOnHandQuantity &&
        totalValuationAmount == other.totalValuationAmount &&
        closedBy == other.closedBy &&
        closedAtUtc == other.closedAtUtc &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, periodYear.hashCode);
    _$hash = $jc(_$hash, periodMonth.hashCode);
    _$hash = $jc(_$hash, periodEndDate.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, lineCount.hashCode);
    _$hash = $jc(_$hash, totalOnHandQuantity.hashCode);
    _$hash = $jc(_$hash, totalValuationAmount.hashCode);
    _$hash = $jc(_$hash, closedBy.hashCode);
    _$hash = $jc(_$hash, closedAtUtc.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'InventoryPeriodCloseResponse')
          ..add('id', id)
          ..add('periodYear', periodYear)
          ..add('periodMonth', periodMonth)
          ..add('periodEndDate', periodEndDate)
          ..add('note', note)
          ..add('lineCount', lineCount)
          ..add('totalOnHandQuantity', totalOnHandQuantity)
          ..add('totalValuationAmount', totalValuationAmount)
          ..add('closedBy', closedBy)
          ..add('closedAtUtc', closedAtUtc)
          ..add('lines', lines))
        .toString();
  }
}

class InventoryPeriodCloseResponseBuilder
    implements
        Builder<InventoryPeriodCloseResponse,
            InventoryPeriodCloseResponseBuilder> {
  _$InventoryPeriodCloseResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  int? _periodYear;
  int? get periodYear => _$this._periodYear;
  set periodYear(int? periodYear) => _$this._periodYear = periodYear;

  int? _periodMonth;
  int? get periodMonth => _$this._periodMonth;
  set periodMonth(int? periodMonth) => _$this._periodMonth = periodMonth;

  Date? _periodEndDate;
  Date? get periodEndDate => _$this._periodEndDate;
  set periodEndDate(Date? periodEndDate) =>
      _$this._periodEndDate = periodEndDate;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  int? _lineCount;
  int? get lineCount => _$this._lineCount;
  set lineCount(int? lineCount) => _$this._lineCount = lineCount;

  double? _totalOnHandQuantity;
  double? get totalOnHandQuantity => _$this._totalOnHandQuantity;
  set totalOnHandQuantity(double? totalOnHandQuantity) =>
      _$this._totalOnHandQuantity = totalOnHandQuantity;

  double? _totalValuationAmount;
  double? get totalValuationAmount => _$this._totalValuationAmount;
  set totalValuationAmount(double? totalValuationAmount) =>
      _$this._totalValuationAmount = totalValuationAmount;

  InventoryPersonResponseBuilder? _closedBy;
  InventoryPersonResponseBuilder get closedBy =>
      _$this._closedBy ??= InventoryPersonResponseBuilder();
  set closedBy(InventoryPersonResponseBuilder? closedBy) =>
      _$this._closedBy = closedBy;

  DateTime? _closedAtUtc;
  DateTime? get closedAtUtc => _$this._closedAtUtc;
  set closedAtUtc(DateTime? closedAtUtc) => _$this._closedAtUtc = closedAtUtc;

  ListBuilder<InventoryPeriodCloseLineResponse>? _lines;
  ListBuilder<InventoryPeriodCloseLineResponse> get lines =>
      _$this._lines ??= ListBuilder<InventoryPeriodCloseLineResponse>();
  set lines(ListBuilder<InventoryPeriodCloseLineResponse>? lines) =>
      _$this._lines = lines;

  InventoryPeriodCloseResponseBuilder() {
    InventoryPeriodCloseResponse._defaults(this);
  }

  InventoryPeriodCloseResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _periodYear = $v.periodYear;
      _periodMonth = $v.periodMonth;
      _periodEndDate = $v.periodEndDate;
      _note = $v.note;
      _lineCount = $v.lineCount;
      _totalOnHandQuantity = $v.totalOnHandQuantity;
      _totalValuationAmount = $v.totalValuationAmount;
      _closedBy = $v.closedBy?.toBuilder();
      _closedAtUtc = $v.closedAtUtc;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(InventoryPeriodCloseResponse other) {
    _$v = other as _$InventoryPeriodCloseResponse;
  }

  @override
  void update(void Function(InventoryPeriodCloseResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  InventoryPeriodCloseResponse build() => _build();

  _$InventoryPeriodCloseResponse _build() {
    _$InventoryPeriodCloseResponse _$result;
    try {
      _$result = _$v ??
          _$InventoryPeriodCloseResponse._(
            id: id,
            periodYear: periodYear,
            periodMonth: periodMonth,
            periodEndDate: periodEndDate,
            note: note,
            lineCount: lineCount,
            totalOnHandQuantity: totalOnHandQuantity,
            totalValuationAmount: totalValuationAmount,
            closedBy: _closedBy?.build(),
            closedAtUtc: closedAtUtc,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'closedBy';
        _closedBy?.build();

        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'InventoryPeriodCloseResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
