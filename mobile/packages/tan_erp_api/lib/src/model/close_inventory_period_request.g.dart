// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'close_inventory_period_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$CloseInventoryPeriodRequest extends CloseInventoryPeriodRequest {
  @override
  final int? periodYear;
  @override
  final int? periodMonth;
  @override
  final String? note;

  factory _$CloseInventoryPeriodRequest(
          [void Function(CloseInventoryPeriodRequestBuilder)? updates]) =>
      (CloseInventoryPeriodRequestBuilder()..update(updates))._build();

  _$CloseInventoryPeriodRequest._(
      {this.periodYear, this.periodMonth, this.note})
      : super._();
  @override
  CloseInventoryPeriodRequest rebuild(
          void Function(CloseInventoryPeriodRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  CloseInventoryPeriodRequestBuilder toBuilder() =>
      CloseInventoryPeriodRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is CloseInventoryPeriodRequest &&
        periodYear == other.periodYear &&
        periodMonth == other.periodMonth &&
        note == other.note;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, periodYear.hashCode);
    _$hash = $jc(_$hash, periodMonth.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'CloseInventoryPeriodRequest')
          ..add('periodYear', periodYear)
          ..add('periodMonth', periodMonth)
          ..add('note', note))
        .toString();
  }
}

class CloseInventoryPeriodRequestBuilder
    implements
        Builder<CloseInventoryPeriodRequest,
            CloseInventoryPeriodRequestBuilder> {
  _$CloseInventoryPeriodRequest? _$v;

  int? _periodYear;
  int? get periodYear => _$this._periodYear;
  set periodYear(int? periodYear) => _$this._periodYear = periodYear;

  int? _periodMonth;
  int? get periodMonth => _$this._periodMonth;
  set periodMonth(int? periodMonth) => _$this._periodMonth = periodMonth;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  CloseInventoryPeriodRequestBuilder() {
    CloseInventoryPeriodRequest._defaults(this);
  }

  CloseInventoryPeriodRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _periodYear = $v.periodYear;
      _periodMonth = $v.periodMonth;
      _note = $v.note;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(CloseInventoryPeriodRequest other) {
    _$v = other as _$CloseInventoryPeriodRequest;
  }

  @override
  void update(void Function(CloseInventoryPeriodRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  CloseInventoryPeriodRequest build() => _build();

  _$CloseInventoryPeriodRequest _build() {
    final _$result = _$v ??
        _$CloseInventoryPeriodRequest._(
          periodYear: periodYear,
          periodMonth: periodMonth,
          note: note,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
