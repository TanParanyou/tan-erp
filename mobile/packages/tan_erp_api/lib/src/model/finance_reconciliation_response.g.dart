// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'finance_reconciliation_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$FinanceReconciliationResponse extends FinanceReconciliationResponse {
  @override
  final double? erpBilled;
  @override
  final double? erpPaid;
  @override
  final double? confirmedBilled;
  @override
  final double? confirmedPaid;
  @override
  final int? pendingCount;
  @override
  final int? failedCount;
  @override
  final int? deadCount;
  @override
  final BuiltList<FinanceReconciliationRowResponse>? rows;

  factory _$FinanceReconciliationResponse(
          [void Function(FinanceReconciliationResponseBuilder)? updates]) =>
      (FinanceReconciliationResponseBuilder()..update(updates))._build();

  _$FinanceReconciliationResponse._(
      {this.erpBilled,
      this.erpPaid,
      this.confirmedBilled,
      this.confirmedPaid,
      this.pendingCount,
      this.failedCount,
      this.deadCount,
      this.rows})
      : super._();
  @override
  FinanceReconciliationResponse rebuild(
          void Function(FinanceReconciliationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  FinanceReconciliationResponseBuilder toBuilder() =>
      FinanceReconciliationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is FinanceReconciliationResponse &&
        erpBilled == other.erpBilled &&
        erpPaid == other.erpPaid &&
        confirmedBilled == other.confirmedBilled &&
        confirmedPaid == other.confirmedPaid &&
        pendingCount == other.pendingCount &&
        failedCount == other.failedCount &&
        deadCount == other.deadCount &&
        rows == other.rows;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, erpBilled.hashCode);
    _$hash = $jc(_$hash, erpPaid.hashCode);
    _$hash = $jc(_$hash, confirmedBilled.hashCode);
    _$hash = $jc(_$hash, confirmedPaid.hashCode);
    _$hash = $jc(_$hash, pendingCount.hashCode);
    _$hash = $jc(_$hash, failedCount.hashCode);
    _$hash = $jc(_$hash, deadCount.hashCode);
    _$hash = $jc(_$hash, rows.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'FinanceReconciliationResponse')
          ..add('erpBilled', erpBilled)
          ..add('erpPaid', erpPaid)
          ..add('confirmedBilled', confirmedBilled)
          ..add('confirmedPaid', confirmedPaid)
          ..add('pendingCount', pendingCount)
          ..add('failedCount', failedCount)
          ..add('deadCount', deadCount)
          ..add('rows', rows))
        .toString();
  }
}

class FinanceReconciliationResponseBuilder
    implements
        Builder<FinanceReconciliationResponse,
            FinanceReconciliationResponseBuilder> {
  _$FinanceReconciliationResponse? _$v;

  double? _erpBilled;
  double? get erpBilled => _$this._erpBilled;
  set erpBilled(double? erpBilled) => _$this._erpBilled = erpBilled;

  double? _erpPaid;
  double? get erpPaid => _$this._erpPaid;
  set erpPaid(double? erpPaid) => _$this._erpPaid = erpPaid;

  double? _confirmedBilled;
  double? get confirmedBilled => _$this._confirmedBilled;
  set confirmedBilled(double? confirmedBilled) =>
      _$this._confirmedBilled = confirmedBilled;

  double? _confirmedPaid;
  double? get confirmedPaid => _$this._confirmedPaid;
  set confirmedPaid(double? confirmedPaid) =>
      _$this._confirmedPaid = confirmedPaid;

  int? _pendingCount;
  int? get pendingCount => _$this._pendingCount;
  set pendingCount(int? pendingCount) => _$this._pendingCount = pendingCount;

  int? _failedCount;
  int? get failedCount => _$this._failedCount;
  set failedCount(int? failedCount) => _$this._failedCount = failedCount;

  int? _deadCount;
  int? get deadCount => _$this._deadCount;
  set deadCount(int? deadCount) => _$this._deadCount = deadCount;

  ListBuilder<FinanceReconciliationRowResponse>? _rows;
  ListBuilder<FinanceReconciliationRowResponse> get rows =>
      _$this._rows ??= ListBuilder<FinanceReconciliationRowResponse>();
  set rows(ListBuilder<FinanceReconciliationRowResponse>? rows) =>
      _$this._rows = rows;

  FinanceReconciliationResponseBuilder() {
    FinanceReconciliationResponse._defaults(this);
  }

  FinanceReconciliationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _erpBilled = $v.erpBilled;
      _erpPaid = $v.erpPaid;
      _confirmedBilled = $v.confirmedBilled;
      _confirmedPaid = $v.confirmedPaid;
      _pendingCount = $v.pendingCount;
      _failedCount = $v.failedCount;
      _deadCount = $v.deadCount;
      _rows = $v.rows?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(FinanceReconciliationResponse other) {
    _$v = other as _$FinanceReconciliationResponse;
  }

  @override
  void update(void Function(FinanceReconciliationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  FinanceReconciliationResponse build() => _build();

  _$FinanceReconciliationResponse _build() {
    _$FinanceReconciliationResponse _$result;
    try {
      _$result = _$v ??
          _$FinanceReconciliationResponse._(
            erpBilled: erpBilled,
            erpPaid: erpPaid,
            confirmedBilled: confirmedBilled,
            confirmedPaid: confirmedPaid,
            pendingCount: pendingCount,
            failedCount: failedCount,
            deadCount: deadCount,
            rows: _rows?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'rows';
        _rows?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'FinanceReconciliationResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
