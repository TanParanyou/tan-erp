// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'goods_receipt_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$GoodsReceiptRequest extends GoodsReceiptRequest {
  @override
  final DateTime? receivedAtUtc;
  @override
  final String? note;
  @override
  final BuiltList<GoodsReceiptLineRequest>? lines;

  factory _$GoodsReceiptRequest(
          [void Function(GoodsReceiptRequestBuilder)? updates]) =>
      (GoodsReceiptRequestBuilder()..update(updates))._build();

  _$GoodsReceiptRequest._({this.receivedAtUtc, this.note, this.lines})
      : super._();
  @override
  GoodsReceiptRequest rebuild(
          void Function(GoodsReceiptRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  GoodsReceiptRequestBuilder toBuilder() =>
      GoodsReceiptRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is GoodsReceiptRequest &&
        receivedAtUtc == other.receivedAtUtc &&
        note == other.note &&
        lines == other.lines;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, receivedAtUtc.hashCode);
    _$hash = $jc(_$hash, note.hashCode);
    _$hash = $jc(_$hash, lines.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'GoodsReceiptRequest')
          ..add('receivedAtUtc', receivedAtUtc)
          ..add('note', note)
          ..add('lines', lines))
        .toString();
  }
}

class GoodsReceiptRequestBuilder
    implements Builder<GoodsReceiptRequest, GoodsReceiptRequestBuilder> {
  _$GoodsReceiptRequest? _$v;

  DateTime? _receivedAtUtc;
  DateTime? get receivedAtUtc => _$this._receivedAtUtc;
  set receivedAtUtc(DateTime? receivedAtUtc) =>
      _$this._receivedAtUtc = receivedAtUtc;

  String? _note;
  String? get note => _$this._note;
  set note(String? note) => _$this._note = note;

  ListBuilder<GoodsReceiptLineRequest>? _lines;
  ListBuilder<GoodsReceiptLineRequest> get lines =>
      _$this._lines ??= ListBuilder<GoodsReceiptLineRequest>();
  set lines(ListBuilder<GoodsReceiptLineRequest>? lines) =>
      _$this._lines = lines;

  GoodsReceiptRequestBuilder() {
    GoodsReceiptRequest._defaults(this);
  }

  GoodsReceiptRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _receivedAtUtc = $v.receivedAtUtc;
      _note = $v.note;
      _lines = $v.lines?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(GoodsReceiptRequest other) {
    _$v = other as _$GoodsReceiptRequest;
  }

  @override
  void update(void Function(GoodsReceiptRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  GoodsReceiptRequest build() => _build();

  _$GoodsReceiptRequest _build() {
    _$GoodsReceiptRequest _$result;
    try {
      _$result = _$v ??
          _$GoodsReceiptRequest._(
            receivedAtUtc: receivedAtUtc,
            note: note,
            lines: _lines?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'lines';
        _lines?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'GoodsReceiptRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
