// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'adjust_stock_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$AdjustStockRequest extends AdjustStockRequest {
  @override
  final String? warehouseId;
  @override
  final String? reason;
  @override
  final BuiltList<AdjustmentLineRequest>? lines;
  @override
  final String? locationId;
  @override
  final DateTime? occurredAtUtc;

  factory _$AdjustStockRequest(
          [void Function(AdjustStockRequestBuilder)? updates]) =>
      (AdjustStockRequestBuilder()..update(updates))._build();

  _$AdjustStockRequest._(
      {this.warehouseId,
      this.reason,
      this.lines,
      this.locationId,
      this.occurredAtUtc})
      : super._();
  @override
  AdjustStockRequest rebuild(
          void Function(AdjustStockRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  AdjustStockRequestBuilder toBuilder() =>
      AdjustStockRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is AdjustStockRequest &&
        warehouseId == other.warehouseId &&
        reason == other.reason &&
        lines == other.lines &&
        locationId == other.locationId &&
        occurredAtUtc == other.occurredAtUtc;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, warehouseId.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jc(_$hash, locationId.hashCode);
    _$hash = $jc(_$hash, occurredAtUtc.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'AdjustStockRequest')
          ..add('warehouseId', warehouseId)
          ..add('reason', reason)
          ..add('lines', lines)
          ..add('locationId', locationId)
          ..add('occurredAtUtc', occurredAtUtc))
        .toString();
  }
}

class AdjustStockRequestBuilder
    implements Builder<AdjustStockRequest, AdjustStockRequestBuilder> {
  _$AdjustStockRequest? _$v;

  String? _warehouseId;
  String? get warehouseId => _$this._warehouseId;
  set warehouseId(String? warehouseId) => _$this._warehouseId = warehouseId;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  ListBuilder<AdjustmentLineRequest>? _lines;
  ListBuilder<AdjustmentLineRequest> get lines =>
      _$this._lines ??= ListBuilder<AdjustmentLineRequest>();
  set lines(ListBuilder<AdjustmentLineRequest>? lines) => _$this._lines = lines;

  String? _locationId;
  String? get locationId => _$this._locationId;
  set locationId(String? locationId) => _$this._locationId = locationId;

  DateTime? _occurredAtUtc;
  DateTime? get occurredAtUtc => _$this._occurredAtUtc;
  set occurredAtUtc(DateTime? occurredAtUtc) =>
      _$this._occurredAtUtc = occurredAtUtc;

  AdjustStockRequestBuilder() {
    AdjustStockRequest._defaults(this);
  }

  AdjustStockRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _warehouseId = $v.warehouseId;
      _reason = $v.reason;
      _lines = $v.lines?.toBuilder();
      _locationId = $v.locationId;
      _occurredAtUtc = $v.occurredAtUtc;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(AdjustStockRequest other) {
    _$v = other as _$AdjustStockRequest;
  }

  @override
  void update(void Function(AdjustStockRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  AdjustStockRequest build() => _build();

  _$AdjustStockRequest _build() {
    _$AdjustStockRequest _$result;
    try {
      _$result = _$v ??
          _$AdjustStockRequest._(
            warehouseId: warehouseId,
            reason: reason,
            lines: _lines?.build(),
            locationId: locationId,
            occurredAtUtc: occurredAtUtc,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'AdjustStockRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
