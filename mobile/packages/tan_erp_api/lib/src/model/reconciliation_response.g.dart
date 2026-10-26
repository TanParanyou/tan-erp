// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reconciliation_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$ReconciliationResponse extends ReconciliationResponse {
  @override
  final int? rowCount;
  @override
  final int? inconsistentCount;
  @override
  final BuiltList<ReconciliationRowResponse>? rows;

  factory _$ReconciliationResponse(
          [void Function(ReconciliationResponseBuilder)? updates]) =>
      (ReconciliationResponseBuilder()..update(updates))._build();

  _$ReconciliationResponse._({this.rowCount, this.inconsistentCount, this.rows})
      : super._();
  @override
  ReconciliationResponse rebuild(
          void Function(ReconciliationResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  ReconciliationResponseBuilder toBuilder() =>
      ReconciliationResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is ReconciliationResponse &&
        rowCount == other.rowCount &&
        inconsistentCount == other.inconsistentCount &&
        rows == other.rows;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, rowCount.hashCode);
    _$hash = $jc(_$hash, inconsistentCount.hashCode);
    _$hash = $jc(_$hash, rows.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'ReconciliationResponse')
          ..add('rowCount', rowCount)
          ..add('inconsistentCount', inconsistentCount)
          ..add('rows', rows))
        .toString();
  }
}

class ReconciliationResponseBuilder
    implements Builder<ReconciliationResponse, ReconciliationResponseBuilder> {
  _$ReconciliationResponse? _$v;

  int? _rowCount;
  int? get rowCount => _$this._rowCount;
  set rowCount(int? rowCount) => _$this._rowCount = rowCount;

  int? _inconsistentCount;
  int? get inconsistentCount => _$this._inconsistentCount;
  set inconsistentCount(int? inconsistentCount) =>
      _$this._inconsistentCount = inconsistentCount;

  ListBuilder<ReconciliationRowResponse>? _rows;
  ListBuilder<ReconciliationRowResponse> get rows =>
      _$this._rows ??= ListBuilder<ReconciliationRowResponse>();
  set rows(ListBuilder<ReconciliationRowResponse>? rows) => _$this._rows = rows;

  ReconciliationResponseBuilder() {
    ReconciliationResponse._defaults(this);
  }

  ReconciliationResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _rowCount = $v.rowCount;
      _inconsistentCount = $v.inconsistentCount;
      _rows = $v.rows?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(ReconciliationResponse other) {
    _$v = other as _$ReconciliationResponse;
  }

  @override
  void update(void Function(ReconciliationResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  ReconciliationResponse build() => _build();

  _$ReconciliationResponse _build() {
    _$ReconciliationResponse _$result;
    try {
      _$result = _$v ??
          _$ReconciliationResponse._(
            rowCount: rowCount,
            inconsistentCount: inconsistentCount,
            rows: _rows?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'rows';
        _rows?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'ReconciliationResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
