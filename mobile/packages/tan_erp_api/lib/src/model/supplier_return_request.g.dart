// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'supplier_return_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SupplierReturnRequest extends SupplierReturnRequest {
  @override
  final String? goodsReceiptId;
  @override
  final DateTime? returnedAtUtc;
  @override
  final String? reason;
  @override
  final BuiltList<SupplierReturnLineRequest>? lines;

  factory _$SupplierReturnRequest(
          [void Function(SupplierReturnRequestBuilder)? updates]) =>
      (SupplierReturnRequestBuilder()..update(updates))._build();

  _$SupplierReturnRequest._(
      {this.goodsReceiptId, this.returnedAtUtc, this.reason, this.lines})
      : super._();
  @override
  SupplierReturnRequest rebuild(
          void Function(SupplierReturnRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SupplierReturnRequestBuilder toBuilder() =>
      SupplierReturnRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SupplierReturnRequest &&
        goodsReceiptId == other.goodsReceiptId &&
        returnedAtUtc == other.returnedAtUtc &&
        reason == other.reason &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, goodsReceiptId.hashCode);
    _$hash = $jc(_$hash, returnedAtUtc.hashCode);
    _$hash = $jc(_$hash, reason.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SupplierReturnRequest')
          ..add('goodsReceiptId', goodsReceiptId)
          ..add('returnedAtUtc', returnedAtUtc)
          ..add('reason', reason)
          ..add('lines', lines))
        .toString();
  }
}

class SupplierReturnRequestBuilder
    implements Builder<SupplierReturnRequest, SupplierReturnRequestBuilder> {
  _$SupplierReturnRequest? _$v;

  String? _goodsReceiptId;
  String? get goodsReceiptId => _$this._goodsReceiptId;
  set goodsReceiptId(String? goodsReceiptId) =>
      _$this._goodsReceiptId = goodsReceiptId;

  DateTime? _returnedAtUtc;
  DateTime? get returnedAtUtc => _$this._returnedAtUtc;
  set returnedAtUtc(DateTime? returnedAtUtc) =>
      _$this._returnedAtUtc = returnedAtUtc;

  String? _reason;
  String? get reason => _$this._reason;
  set reason(String? reason) => _$this._reason = reason;

  ListBuilder<SupplierReturnLineRequest>? _lines;
  ListBuilder<SupplierReturnLineRequest> get lines =>
      _$this._lines ??= ListBuilder<SupplierReturnLineRequest>();
  set lines(ListBuilder<SupplierReturnLineRequest>? lines) =>
      _$this._lines = lines;

  SupplierReturnRequestBuilder() {
    SupplierReturnRequest._defaults(this);
  }

  SupplierReturnRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _goodsReceiptId = $v.goodsReceiptId;
      _returnedAtUtc = $v.returnedAtUtc;
      _reason = $v.reason;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SupplierReturnRequest other) {
    _$v = other as _$SupplierReturnRequest;
  }

  @override
  void update(void Function(SupplierReturnRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SupplierReturnRequest build() => _build();

  _$SupplierReturnRequest _build() {
    _$SupplierReturnRequest _$result;
    try {
      _$result = _$v ??
          _$SupplierReturnRequest._(
            goodsReceiptId: goodsReceiptId,
            returnedAtUtc: returnedAtUtc,
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
            r'SupplierReturnRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
