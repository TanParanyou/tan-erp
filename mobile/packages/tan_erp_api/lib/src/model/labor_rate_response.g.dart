// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'labor_rate_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$LaborRateResponse extends LaborRateResponse {
  @override
  final String? id;
  @override
  final ProjectPersonResponse? worker;
  @override
  final double? hourlyRate;
  @override
  final DateTime? updatedAtUtc;
  @override
  final String? rowVersion;

  factory _$LaborRateResponse(
          [void Function(LaborRateResponseBuilder)? updates]) =>
      (LaborRateResponseBuilder()..update(updates))._build();

  _$LaborRateResponse._(
      {this.id,
      this.worker,
      this.hourlyRate,
      this.updatedAtUtc,
      this.rowVersion})
      : super._();
  @override
  LaborRateResponse rebuild(void Function(LaborRateResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  LaborRateResponseBuilder toBuilder() =>
      LaborRateResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is LaborRateResponse &&
        id == other.id &&
        worker == other.worker &&
        hourlyRate == other.hourlyRate &&
        updatedAtUtc == other.updatedAtUtc &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, worker.hashCode);
    _$hash = $jc(_$hash, hourlyRate.hashCode);
    _$hash = $jc(_$hash, updatedAtUtc.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'LaborRateResponse')
          ..add('id', id)
          ..add('worker', worker)
          ..add('hourlyRate', hourlyRate)
          ..add('updatedAtUtc', updatedAtUtc)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class LaborRateResponseBuilder
    implements Builder<LaborRateResponse, LaborRateResponseBuilder> {
  _$LaborRateResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  ProjectPersonResponseBuilder? _worker;
  ProjectPersonResponseBuilder get worker =>
      _$this._worker ??= ProjectPersonResponseBuilder();
  set worker(ProjectPersonResponseBuilder? worker) => _$this._worker = worker;

  double? _hourlyRate;
  double? get hourlyRate => _$this._hourlyRate;
  set hourlyRate(double? hourlyRate) => _$this._hourlyRate = hourlyRate;

  DateTime? _updatedAtUtc;
  DateTime? get updatedAtUtc => _$this._updatedAtUtc;
  set updatedAtUtc(DateTime? updatedAtUtc) =>
      _$this._updatedAtUtc = updatedAtUtc;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  LaborRateResponseBuilder() {
    LaborRateResponse._defaults(this);
  }

  LaborRateResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _worker = $v.worker?.toBuilder();
      _hourlyRate = $v.hourlyRate;
      _updatedAtUtc = $v.updatedAtUtc;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(LaborRateResponse other) {
    _$v = other as _$LaborRateResponse;
  }

  @override
  void update(void Function(LaborRateResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  LaborRateResponse build() => _build();

  _$LaborRateResponse _build() {
    _$LaborRateResponse _$result;
    try {
      _$result = _$v ??
          _$LaborRateResponse._(
            id: id,
            worker: _worker?.build(),
            hourlyRate: hourlyRate,
            updatedAtUtc: updatedAtUtc,
            rowVersion: rowVersion,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'worker';
        _worker?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'LaborRateResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
