// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'transfer_stock_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$TransferStockRequest extends TransferStockRequest {
  @override
  final String? fromWarehouseId;
  @override
  final String? toWarehouseId;
  @override
  final String? reason;
  @override
  final BuiltList<StockLineRequest>? lines;

  factory _$TransferStockRequest(
          [void Function(TransferStockRequestBuilder)? updates]) =>
      (TransferStockRequestBuilder()..update(updates))._build();

  _$TransferStockRequest._(
      {this.fromWarehouseId, this.toWarehouseId, this.reason, this.lines})
      : super._();
  @override
  TransferStockRequest rebuild(
          void Function(TransferStockRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  TransferStockRequestBuilder toBuilder() =>
      TransferStockRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is TransferStockRequest &&
        fromWarehouseId == other.fromWarehouseId &&
        toWarehouseId == other.toWarehouseId &&
        reason == other.reason &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, fromWarehouseId.hashCode);
    _$hash = $jc(_$hash, toWarehouseId.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'TransferStockRequest')
          ..add('fromWarehouseId', fromWarehouseId)
          ..add('toWarehouseId', toWarehouseId)
          ..add('reason', reason)
          ..add('lines', lines))
        .toString();
  }
}

class TransferStockRequestBuilder
    implements Builder<TransferStockRequest, TransferStockRequestBuilder> {
  _$TransferStockRequest? _$v;

  String? _fromWarehouseId;
  String? get fromWarehouseId => _$this._fromWarehouseId;
  set fromWarehouseId(String? fromWarehouseId) =>
      _$this._fromWarehouseId = fromWarehouseId;

  String? _toWarehouseId;
  String? get toWarehouseId => _$this._toWarehouseId;
  set toWarehouseId(String? toWarehouseId) =>
      _$this._toWarehouseId = toWarehouseId;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  ListBuilder<StockLineRequest>? _lines;
  ListBuilder<StockLineRequest> get lines =>
      _$this._lines ??= ListBuilder<StockLineRequest>();
  set lines(ListBuilder<StockLineRequest>? lines) => _$this._lines = lines;

  TransferStockRequestBuilder() {
    TransferStockRequest._defaults(this);
  }

  TransferStockRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _fromWarehouseId = $v.fromWarehouseId;
      _toWarehouseId = $v.toWarehouseId;
      _reason = $v.reason;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(TransferStockRequest other) {
    _$v = other as _$TransferStockRequest;
  }

  @override
  void update(void Function(TransferStockRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  TransferStockRequest build() => _build();

  _$TransferStockRequest _build() {
    _$TransferStockRequest _$result;
    try {
      _$result = _$v ??
          _$TransferStockRequest._(
            fromWarehouseId: fromWarehouseId,
            toWarehouseId: toWarehouseId,
            reason: reason,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'TransferStockRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
