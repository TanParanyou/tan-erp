// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'unit_conversion_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UnitConversionResponse extends UnitConversionResponse {
  @override
  final String? id;
  @override
  final String? fromUnitId;
  @override
  final String? fromUnitCode;
  @override
  final String? toUnitId;
  @override
  final String? toUnitCode;
  @override
  final String? factor;
  @override
  final Date? effectiveFrom;
  @override
  final Date? effectiveTo;
  @override
  final String? reason;
  @override
  final String? status;
  @override
  final String? rowVersion;

  factory _$UnitConversionResponse(
          [void Function(UnitConversionResponseBuilder)? updates]) =>
      (UnitConversionResponseBuilder()..update(updates))._build();

  _$UnitConversionResponse._(
      {this.id,
      this.fromUnitId,
      this.fromUnitCode,
      this.toUnitId,
      this.toUnitCode,
      this.factor,
      this.effectiveFrom,
      this.effectiveTo,
      this.reason,
      this.status,
      this.rowVersion})
      : super._();
  @override
  UnitConversionResponse rebuild(
          void Function(UnitConversionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UnitConversionResponseBuilder toBuilder() =>
      UnitConversionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UnitConversionResponse &&
        id == other.id &&
        fromUnitId == other.fromUnitId &&
        fromUnitCode == other.fromUnitCode &&
        toUnitId == other.toUnitId &&
        toUnitCode == other.toUnitCode &&
        factor == other.factor &&
        effectiveFrom == other.effectiveFrom &&
        effectiveTo == other.effectiveTo &&
        reason == other.reason &&
        status == other.status &&
        rowVersion == other.rowVersion;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, fromUnitId.hashCode);
    _$hash = $jc(_$hash, fromUnitCode.hashCode);
    _$hash = $jc(_$hash, toUnitId.hashCode);
    _$hash = $jc(_$hash, toUnitCode.hashCode);
    _$hash = $jc(_$hash, factor.hashCode);
    _$hash = $jc(_$hash, effectiveFrom.hashCode);
    _$hash = $jc(_$hash, effectiveTo.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, status.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UnitConversionResponse')
          ..add('id', id)
          ..add('fromUnitId', fromUnitId)
          ..add('fromUnitCode', fromUnitCode)
          ..add('toUnitId', toUnitId)
          ..add('toUnitCode', toUnitCode)
          ..add('factor', factor)
          ..add('effectiveFrom', effectiveFrom)
          ..add('effectiveTo', effectiveTo)
          ..add('reason', reason)
          ..add('status', status)
          ..add('rowVersion', rowVersion))
        .toString();
  }
}

class UnitConversionResponseBuilder
    implements Builder<UnitConversionResponse, UnitConversionResponseBuilder> {
  _$UnitConversionResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _fromUnitId;
  String? get fromUnitId => _$this._fromUnitId;
  set fromUnitId(String? fromUnitId) => _$this._fromUnitId = fromUnitId;

  String? _fromUnitCode;
  String? get fromUnitCode => _$this._fromUnitCode;
  set fromUnitCode(String? fromUnitCode) => _$this._fromUnitCode = fromUnitCode;

  String? _toUnitId;
  String? get toUnitId => _$this._toUnitId;
  set toUnitId(String? toUnitId) => _$this._toUnitId = toUnitId;

  String? _toUnitCode;
  String? get toUnitCode => _$this._toUnitCode;
  set toUnitCode(String? toUnitCode) => _$this._toUnitCode = toUnitCode;

  String? _factor;
  String? get factor => _$this._factor;
  set factor(String? factor) => _$this._factor = factor;

  Date? _effectiveFrom;
  Date? get effectiveFrom => _$this._effectiveFrom;
  set effectiveFrom(Date? effectiveFrom) =>
      _$this._effectiveFrom = effectiveFrom;

  Date? _effectiveTo;
  Date? get effectiveTo => _$this._effectiveTo;
  set effectiveTo(Date? effectiveTo) => _$this._effectiveTo = effectiveTo;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  String? _status;
  String? get status => _$this._status;
  set status(String? status) => _$this._status = status;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  UnitConversionResponseBuilder() {
    UnitConversionResponse._defaults(this);
  }

  UnitConversionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _fromUnitId = $v.fromUnitId;
      _fromUnitCode = $v.fromUnitCode;
      _toUnitId = $v.toUnitId;
      _toUnitCode = $v.toUnitCode;
      _factor = $v.factor;
      _effectiveFrom = $v.effectiveFrom;
      _effectiveTo = $v.effectiveTo;
      _reason = $v.reason;
      _status = $v.status;
      _rowVersion = $v.rowVersion;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UnitConversionResponse other) {
    _$v = other as _$UnitConversionResponse;
  }

  @override
  void update(void Function(UnitConversionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UnitConversionResponse build() => _build();

  _$UnitConversionResponse _build() {
    final _$result = _$v ??
        _$UnitConversionResponse._(
          id: id,
          fromUnitId: fromUnitId,
          fromUnitCode: fromUnitCode,
          toUnitId: toUnitId,
          toUnitCode: toUnitCode,
          factor: factor,
          effectiveFrom: effectiveFrom,
          effectiveTo: effectiveTo,
          reason: reason,
          status: status,
          rowVersion: rowVersion,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
